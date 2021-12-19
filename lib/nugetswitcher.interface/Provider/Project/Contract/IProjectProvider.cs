using NuGetSwitcher.Interface.Reference.Project.Contract;

using System.Collections.Generic;

using MsbProject = Microsoft.Build.Evaluation.Project;

namespace NuGetSwitcher.Interface.Provider.Project.Contract
{
    public interface IProjectProvider<out TProjectReference> where TProjectReference : class, IProjectReference
    {
        /// <summary>
        /// Absolute path to the solution file.
        /// </summary>
        string Solution
        {
            get;
        }

        /// <summary>
        /// Returns solution projects.
        /// </summary>
        IEnumerable<TProjectReference> GetLoadedProject();

        /// <summary>
        /// Loads a project with the specified filename, using the 
        /// collection's global properties and tools version. If a 
        /// matching project is already loaded, it will be returned,otherwise a new project will be loaded.
        /// </summary>
        MsbProject GetLoadedProject(string project);
    }
}