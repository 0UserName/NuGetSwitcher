using Microsoft.Build.Construction;

using NuGetSwitcher.Interface.Provider.Project.Contract;
using NuGetSwitcher.Interface.Reference.Project.Contract;

using System;
using System.Collections.Generic;

using MsbProject = Microsoft.Build.Evaluation.Project;

namespace NuGetSwitcher.Interface.Provider.Project
{
    public abstract class AbstractProjectProvider<TProjectReference> : IProjectProvider<TProjectReference> where TProjectReference : class, IProjectReference
    {
        /// <summary>
        /// Absolute path to the solution file.
        /// </summary>
        public string Solution
        {
            get;
            private set;
        }

        private AbstractProjectProvider()
        { }

        protected AbstractProjectProvider(string solution) : this()
        {
            Solution = solution;
        }

        /// <summary>
        /// Returns solution projects.
        /// </summary>
        public IEnumerable<TProjectReference> GetLoadedProject()
        {
            List<TProjectReference> projects = new
            List<TProjectReference>
            (30);

            foreach (ProjectInSolution project in SolutionFile.Parse(Solution).ProjectsByGuid.Values)
            {
                if (project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
                {
                    projects.Add((TProjectReference)Activator.CreateInstance(typeof(TProjectReference), GetLoadedProject(project.AbsolutePath)));
                }
            }

            return projects;
        }

        /// <summary>
        /// Loads a project with the specified filename, using the 
        /// collection's global properties and tools version. If a 
        /// matching project is already loaded, it will be returned,otherwise a new project will be loaded.
        /// </summary>
        public abstract MsbProject GetLoadedProject(string project);
    }
}