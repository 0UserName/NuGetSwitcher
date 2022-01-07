using CliWrap.Builders;

using Microsoft.Build.Evaluation;

using NuGet.ProjectModel;

using NuGetSwitcher.Core.Abstract;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using NuGetSwitcher.Interface.Provider.Message.Contract;
using NuGetSwitcher.Interface.Provider.Option.Contract;
using NuGetSwitcher.Interface.Provider.Project.Contract;

using NuGetSwitcher.Interface.Reference.Project.Contract;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class ProjectSwitch : AbstractSwitch
    {
        public ProjectSwitch(IOptionProvider optionProvider, IProjectProvider<IProjectReference> projectHelper, IMessageProvider messageHelper) : base(ReferenceType.ProjectReference, optionProvider, projectHelper, messageHelper)
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
            {
                { "Temp", library.Name }
            };

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
                    { "Name", library.Name },
                    { "Temp", library.Name }
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

                MessageProvider.AddMessage(reference.MsbProject.FullPath, $"Dependency: { library.Name } has been switched. Type: { Type }", MessageCategory.ME);
            }
        }

        /// <summary>
        /// Adds one or more projects to the solution file.
        /// </summary>
        protected virtual void AddToSolution(string solution, IEnumerable<string> projects)
        {
            base.SlnAction(solution, projects, new ArgumentsBuilder().Add("add").Add(projects).Add("--solution-folder").Add("Temporary"));
        }
    }
}