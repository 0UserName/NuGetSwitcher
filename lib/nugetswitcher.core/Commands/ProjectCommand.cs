using CliWrap.Builders;

using Microsoft.Build.Evaluation;

using NuGet.ProjectModel;

using NuGetSwitcher.Core.Constants;

using NuGetSwitcher.Interface.Logger;
using NuGetSwitcher.Interface.Logger.Enums;

using NuGetSwitcher.Interface.Options;
using NuGetSwitcher.Interface.Projects;
using NuGetSwitcher.Interface.Solution;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuGetSwitcher.Core.Commands
{
    public sealed class ProjectCommand<TReference>(IOptions option, ISolution<TReference> solution, ILogger logger) : AbstractCommand(logger) where TReference : class, IProject
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="absolutePath">
        /// Absolute path to the solution file.
        /// </param>      
        private async ValueTask AddToSolutionAsync(string absolutePath, IEnumerable<string> projects)
        {
            await SlnActionAsync(absolutePath, new ArgumentsBuilder().Add("add").Add(projects).Add("--solution-folder").Add("Temporary"));
        }

        /// <summary>
        /// Includes references to the GAC assemblies
        /// listed in the FrameworkAssemblies section
        /// of the lock file.
        /// </summary>
        private static void AddAssemblies(IProject reference, LockFileTargetLibrary package)
        {
            Dictionary<string, string> metadata = new
            Dictionary<string, string>
            {
                { METADATA_TEMP, package.Name }
            };

            foreach (string assembly in package.FrameworkAssemblies)
            {
                reference.AddReference(assembly, ReferenceType.Reference, metadata);
            }
        }

        /// <summary>
        /// Includes implicit, explicit project references
        /// listed in the Dependencies section of the lock
        /// file.
        /// </summary>
        private void SwitchPackages(IProject reference, LockFileTargetLibrary package, string absolutePath)
        {
            AddAssemblies(reference, package);

            /*
             * References can be represented by several values in
             * an ItemGroup, for example, when included using the 
             * Condition attribute.
             */

            ICollection<ProjectItem> items = reference.GetReference(package.Name);

            // Implicit.
            if (items.Count == 0)
            {
                reference.AddReference(absolutePath, ReferenceType.ProjectReference, new Dictionary<string, string>(1)
                {
                    { METADATA_TEMP, package.Name }
                });
            }
            // Explicit.
            else
            {
                foreach (ProjectItem item in items)
                {
                    item.ItemType = ReferenceType.ProjectReference;

                    item.SetMetadataValue(METADATA_TEMP, item.EvaluatedInclude);
                    item.SetMetadataValue(METADATA_NAME, item.EvaluatedInclude);

                    item.UnevaluatedInclude = absolutePath;
                }

                Logger.LogMessage($"{ReferenceType.PackageReference} {package.Name} has been switched", Category.I, project: reference.ToString());
            }
        }

        /// <inheritdoc/>
        public override async ValueTask SwitchAsync()
        {
            HashSet<string> switchedProjects = new
            HashSet<string>
            ();

            IReadOnlyDictionary<string, string> projects = option.GetIncludeProjects();

            foreach (TReference reference in await solution.GetLoadedProjectsAsync())
            {
                foreach (LockFileTargetLibrary library in reference.GetLockFileLibraries()) // iterate over library.Dependencies
                {
                    if (projects.TryGetValue(library.Name, out string absolutePath))
                    {
                        SwitchPackages(reference, library, absolutePath);

                        switchedProjects.Add(absolutePath);
                    }
                }

                reference.Save();
            }

            if (projects.Count != default)
            {
                await AddToSolutionAsync(solution.AbsolutePath, switchedProjects);
            }
        }
    }
}