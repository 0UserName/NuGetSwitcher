using CliWrap;
using CliWrap.Buffered;

using Microsoft.Build.Evaluation;

using NuGet.ProjectModel;

using NuGetSwitcher.Core.Abstract;

using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity;
using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class ProjectSwitch : AbstractSwitch
    {
        public ProjectSwitch(ReferenceType type, IOptionProvider optionProvider, IProjectProvider projectHelper, IMessageProvider messageHelper) : base(type, optionProvider, projectHelper, messageHelper)
        { }

        /// <summary>
        /// Replaces PackageReference references marked
        /// with the Temp attribute to ProjectReference.
        /// Transitive dependencies will be included.
        /// </summary>
        ///
        /// <exception cref="SwitcherFileNotFoundException"/>
        /// 
        /// <exception cref="FileNotFoundException"/>
        /// 
        /// <exception cref="ArgumentException">
        /// 
        /// <remarks>
        /// All projects detected during
        /// the work will be included in 
        /// the solution.
        /// </remarks>
        public override IEnumerable<string> Switch()
        {
            HashSet<string> output = new
            HashSet<string>
            ();

            IEnumerable<IProjectReference> references = ProjectProvider.GetLoadedProject();

            void Executor(IProjectReference reference, LockFileTargetLibrary library, string absolutePath)
            {
                SwitchSysDependency(reference, library);
                SwitchPkgDependency(reference, library, absolutePath);

                output.Add(absolutePath);
            }

            IterateAndExecute(references, Executor);

            AddToSolution(ProjectProvider.Solution, output);

            return output;
        }

        /// <summary>
        /// Includes references to the GAC assemblies
        /// listed in the FrameworkAssemblies section
        /// of the lock file.
        /// </summary>
        public virtual void SwitchSysDependency(IProjectReference reference, LockFileTargetLibrary library)
        {
            Dictionary<string, string> metadata = new
            Dictionary<string, string>
            (1);

            foreach (string assembly in library.FrameworkAssemblies)
            {
                base.AddReference(reference, ReferenceType.Reference, assembly, metadata);
            }
        }

        /// <summary>
        /// Includes implicit, explicit project references
        /// listed in the Dependencies section of the lock
        /// file.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        /// 
        /// <remarks>
        /// Implicit dependencies mean transitive.
        /// </remarks>
        public virtual void SwitchPkgDependency(IProjectReference reference, LockFileTargetLibrary library, string absolutePath)
        {
            /*
             * References can be represented by several values in
             * an ItemGroup, for example, when included using the 
             * Condition attribute.
             */

            ICollection<ProjectItem> items = reference.MsbProject.GetItemsByEvaluatedInclude(library.Name);

            // Implicit.
            if (!items.Any())
            {
                base.AddReference(reference, Type, absolutePath, new Dictionary<string, string>(2)
                {
                    { "Name", library.Name }
                });
            }
            // Explicit.
            else
            {
                /*
                 * Re-creating an item can lead to the loss
                 * of user metadata; in order to avoid this,
                 * the item is redefined.
                 */

                foreach (ProjectItem item in items)
                {
                    item.ItemType = Type.ToString();

                    item.SetMetadataValue("Temp", item.EvaluatedInclude);
                    item.SetMetadataValue("Name", item.EvaluatedInclude);

                    item.UnevaluatedInclude = absolutePath;
                }

                MessageProvider.AddMessage(reference.MsbProject.FullPath, $"Dependency: {library.Name } has been switched. Type: { Type }", MessageCategory.ME);
            }
        }

        protected virtual void AddToSolution(string solution, IEnumerable<string> projects)
        {
            CliWrap.Command command = Cli.Wrap("dotnet")

                .WithWorkingDirectory(Path.GetDirectoryName(solution))

                .WithArguments(

                args => args.Add("sln")
                       .Add(solution)
                       .Add("add")
                       .Add(projects)

                       /*
                        * Since .NET Core 3.0 SDK.
                        * 
                        * Destination solution folder 
                        * path to add the projects to.
                        */

                       .Add("--solution-folder")
                       .Add("Temporary"));

            BufferedCommandResult result = command.ExecuteBufferedAsync().GetAwaiter().GetResult();

            if (!string.IsNullOrWhiteSpace(result.StandardOutput))
            {
                MessageProvider.AddMessage(result.StandardOutput, MessageCategory.ME);
            }

            if (!string.IsNullOrWhiteSpace(result.StandardError))
            {
                MessageProvider.AddMessage(result.StandardError , MessageCategory.ER);
            }
        }
    }
}