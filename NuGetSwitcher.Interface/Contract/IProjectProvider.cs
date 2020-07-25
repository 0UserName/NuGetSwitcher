using Microsoft.Build.Evaluation;

using NuGetSwitcher.Interface.Entity;

using System.Collections.Generic;

namespace NuGetSwitcher.Interface.Contract
{
    public interface IProjectProvider
    {
        /// <summary>
        /// Absolute path to the solution file.
        /// </summary>
        string Solution
        {
            get;
        }

        /// <summary>
        /// Returns solution projects
        /// that are also loaded into 
        /// the GPC.
        /// </summary>
        IEnumerable<IProjectReference> GetLoadedProject();

        /// <summary>
        /// Returns projects from GPC.
        /// </summary>
        IEnumerable<Project> GetLoadedProject(IEnumerable<string> projects);

        /// <summary>
        /// Returns projects from GPC.
        /// </summary>
        Project GetLoadedProject(string project);
    }
}