using NuGetSwitcher.Interface.Projects;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuGetSwitcher.Interface.Solution
{
    public interface ISolution<TProject> where TProject : IProject
    {
        /// <summary>
        /// Absolute path to the solution file.
        /// </summary>
        string AbsolutePath
        {
            get;
        }

        /// <summary>
        /// Loads a project
        /// with the specified filename, using 
        /// the collection's global properties 
        /// and tools version.
        /// </summary>
        TProject GetLoadedProject(string project);

        /// <summary>
        /// Returns solution projects.
        /// </summary>
        ValueTask<IEnumerable<TProject>> GetLoadedProjectsAsync();
    }
}