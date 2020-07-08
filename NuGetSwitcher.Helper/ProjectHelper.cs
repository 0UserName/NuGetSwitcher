using Microsoft.Build.Evaluation;

using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Error;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGetSwitcher.Helper
{
    public class ProjectHelper : IProjectHelper
    {
        protected EnvDTE.DTE DTE
        {
            get;
            set;
        }

        public ProjectHelper()
        {
            DTE = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
        }

        /// <summary>
        /// Returns solution projects
        /// that are also loaded into 
        /// the GPC.
        /// </summary>
        public virtual IEnumerable<ProjectReference> GetLoadedProject()
        {
            List<ProjectReference> projects = new
            List<ProjectReference>(30);

            foreach (EnvDTE.Project dteProject in DTE.Solution.Projects)
            {
                // To filter miscellaneous files.
                if (string.IsNullOrWhiteSpace(dteProject.FileName))
                {
                    continue;
                }

                projects.Add(new ProjectReference(dteProject, GetLoadedProject(dteProject.FullName)));
            }

            return projects;
        }

        /// <summary>
        /// Returns projects from GPC.
        /// </summary>
        public virtual IEnumerable<Project> GetLoadedProject(IEnumerable<string> projects)
        {
            return projects.Select(p => GetLoadedProject(p));
        }

        /// <summary>
        /// Returns projects from GPC.
        /// </summary>
        public virtual Project GetLoadedProject(string project)
        {
            return ProjectCollection.GlobalProjectCollection.LoadProject(project);
        }

        /// <summary>
        /// Unloads projects from GPC.
        /// </summary>
        /// 
        /// <exception cref="SwitcherInvalidOperationException"/>
        public virtual void UnloadProject()
        {
            ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
        }

        /// <summary>
        /// Unloads projects from GPC.
        /// </summary>
        /// 
        /// <exception cref="SwitcherInvalidOperationException"/>
        public virtual void UnloadProject(Project project)
        {
            try
            {
                ProjectCollection.GlobalProjectCollection.UnloadProject(project);
            }
            catch (InvalidOperationException exception)
            {
                throw new SwitcherInvalidOperationException(project, exception);
            }
        }

        /// <summary>
        /// Unloads projects from GPC.
        /// </summary>
        /// 
        /// <exception cref="SwitcherInvalidOperationException"/>
        public virtual void UnloadProject(string project)
        {
            UnloadProject(GetLoadedProject(project));
        }
    }
}