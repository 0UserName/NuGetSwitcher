using Microsoft.Build.Evaluation;

using Microsoft.VisualStudio.Shell;

using NuGet.ProjectModel;

using NuGetSwitcher.Abstract;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Enum;
using NuGetSwitcher.Helper.Entity.Error;

using NuGetSwitcher.Option;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class ProjectSwitch : AbstractSwitch
    {
        public ProjectSwitch(bool isVSIX, ReferenceType type, IPackageOption packageOption, IProjectHelper projectHelper, IMessageHelper messageHelper) : base(isVSIX, type, packageOption, projectHelper, messageHelper)
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
        public override void Switch()
        {
            IEnumerable<ProjectReference> references = ProjectHelper.GetLoadedProject();

            HashSet<string> include = new
            HashSet<string>();

            void Executor(ProjectReference reference, LockFileTargetLibrary library, string absolutePath)
            {
                SwitchSysDependency(reference, library);
                SwitchPkgDependency(reference, library, absolutePath);

                include.Add(absolutePath);
            }

            IterateAndExecute(ProjectHelper.GetLoadedProject(), Executor);

            IncludeProject(include.Except(references.Select(r => r.MsbProject.FullPath)));
        }

        /// <summary>
        /// Includes references to the GAC assemblies
        /// listed in the FrameworkAssemblies section
        /// of the lock file.
        /// </summary>
        public virtual void SwitchSysDependency(ProjectReference reference, LockFileTargetLibrary library)
        {
            Dictionary<string, string> metadata = new
            Dictionary<string, string>(1);

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
        public virtual void SwitchPkgDependency(ProjectReference reference, LockFileTargetLibrary library, string absolutePath)
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

                MessageHelper.AddMessage(reference.DteProject.UniqueName, $"Dependency: {library.Name } has been switched. Type: { Type }", TaskErrorCategory.Message);
            }
        }

        /// <summary>
        /// Includes a set of passed 
        /// projects in the solution.
        /// </summary>
        /// 
        /// <param name="projects">
        /// Absolute paths to project
        /// files without duplicates.
        /// </param>
        /// 
        /// <remarks>
        /// Uses DTE - only works from Visual Studio.
        /// </remarks>
        private void IncludeProject(IEnumerable<string> projects)
        {
            /*
             * I don’t know how, but the AddFromFile method depends
             * on the GPC, therefore, to avoid errors, all projects 
             * are forcibly unloaded.
             */

            if (IsVSIX)
            {
                ProjectHelper.UnloadProject();

                try
                {
                    foreach (string project in projects)
                    {
                        DTE.Solution.AddFromFile(project, false);
                    }
                }
                catch (Exception exception)
                {
                    MessageHelper.AddMessage(exception);
                }
                finally
                {
                    DTE.Solution.SaveAs(DTE.Solution.FileName);
                }
            }
        }
    }
}