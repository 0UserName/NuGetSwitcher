using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell;
using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity;
using NuGetSwitcher.VSIXService.Project.Entity;
using System.Collections.Generic;
using System.Linq;

using MsbProject = Microsoft.Build.Evaluation.Project;

namespace NuGetSwitcher.VSIXService.Project
{
    public class VsixProjectProvider : IProjectProvider
    {
        protected EnvDTE.DTE DTE
        {
            get;
            set;
        }

        public VsixProjectProvider()
        {
            DTE = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
        }

        /// <summary>
        /// Absolute path to the solution file.
        /// </summary>
        public string Solution
        {
            get => DTE.Solution.FullName;
        }

        /// <summary>
        /// Returns solution projects
        /// that are also loaded into 
        /// the GPC.
        /// </summary>
        public virtual IEnumerable<IProjectReference> GetLoadedProject()
        {
            var rootProjects = new List<EnvDTE.Project>();

            foreach (EnvDTE.Project item in DTE.Solution.Projects)
                rootProjects.Add(item);

            var loadedProjects = TraverseProjects(rootProjects);
            return loadedProjects;
        }

        /// <summary>
        /// Recursively builds projects references
        /// </summary>
        /// <returns>Projects references including nested items</returns>
        protected virtual IEnumerable<IProjectReference> TraverseProjects(IEnumerable<EnvDTE.Project> projects)
        {
            var traversedProjects = new List<IProjectReference>();

            foreach (EnvDTE.Project project in projects)
            {
                if (string.IsNullOrWhiteSpace(project.FileName))
                {
                    var nestedProjects = new List<EnvDTE.Project>();

                    foreach (EnvDTE.ProjectItem nestedProjectItem in project.ProjectItems)
                {
                        var nestedProject = nestedProjectItem.Object as EnvDTE.Project;
                        if (nestedProject != null)
                            nestedProjects.Add(nestedProject);
                    }

                    traversedProjects.AddRange(TraverseProjects(nestedProjects));
                    continue;
                }

                traversedProjects.Add(new VsixProjectReference(project, GetLoadedProject(project.FullName)));
            }

            return traversedProjects;
        }

        /// <summary>
        /// Returns projects from GPC.
        /// </summary>
        public virtual IEnumerable<MsbProject> GetLoadedProject(IEnumerable<string> projects)
        {
            return projects.Select(p => GetLoadedProject(p));
        }

        /// <summary>
        /// Returns projects from GPC.
        /// </summary>
        public virtual MsbProject GetLoadedProject(string project)
        {
            return ProjectCollection.GlobalProjectCollection.LoadProject(project);
        }
    }
}