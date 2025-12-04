using CliWrap.Builders;

using Microsoft.Build.Evaluation;

using NuGetSwitcher.Core.Constants;

using NuGetSwitcher.Interface.Logger;
using NuGetSwitcher.Interface.Logger.Enums;

using NuGetSwitcher.Interface.Options;
using NuGetSwitcher.Interface.Projects;
using NuGetSwitcher.Interface.Solution;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NuGetSwitcher.Core.Commands
{
    public sealed class PackageCommand<TReference>(IOptions option, ISolution<TReference> solution, ILogger logger) : AbstractCommand(logger)where TReference: class, IProject
    {
        private const string METADATA_VERSION = "Version";

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="absolutePath">
        /// Absolute path to the solution file.
        /// </param>      
        private async ValueTask RemoveFromSolutionAsync(string absolutePath, IEnumerable<string> projects)
        {
            await SlnActionAsync(absolutePath, new ArgumentsBuilder().Add("remove").Add(projects));
        }

        /// <summary>
        /// Removes references 
        /// that marked with the Temp attribute and originated from 
        /// the Dependencies or FrameworkAssemblies sections of the 
        /// lock file.
        /// </summary>
        private IEnumerable<string> SwitchDependencies(IProject reference, string referenceType)
        {
            List<string> projects = new
            List<string>
            ();

            foreach (ProjectItem item in reference.GetItems(referenceType, METADATA_TEMP))
            {
                if (referenceType == ReferenceType.ProjectReference)
                {
                    projects.Add(item.EvaluatedInclude);
                }

                // Implicit.
                if (!item.HasMetadata(METADATA_VERSION))
                {
                    reference.RemoveReference(item);
                }
                // Explicit.
                else
                {
                    item.ItemType = ReferenceType.PackageReference;

                    item.UnevaluatedInclude = item.GetMetadataValue(METADATA_TEMP);

                    item.RemoveMetadata(METADATA_TEMP);
                    item.RemoveMetadata(METADATA_NAME);
                }

                Logger.LogMessage($"{referenceType}: {Path.GetFileName(item.EvaluatedInclude)} has been switched back", Category.I, project: reference.ToString());
            }

            return projects;
        }

        /// <inheritdoc/>
        public override async ValueTask SwitchAsync()
        {
            HashSet<string> projects = new
            HashSet<string>
            ();

            foreach (TReference reference in await solution.GetLoadedProjectsAsync())
            {
                SwitchDependencies(reference, ReferenceType.Reference);
                projects.UnionWith(SwitchDependencies(reference, ReferenceType.ProjectReference));

                reference.Save();
            }

            if (projects.Count != default)
            {
                await RemoveFromSolutionAsync(solution.AbsolutePath, projects);
            }
        }
    }
}