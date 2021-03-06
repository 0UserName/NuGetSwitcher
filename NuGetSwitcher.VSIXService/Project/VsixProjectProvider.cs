﻿using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity;

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
            List<IProjectReference> projects = new
            List<IProjectReference>
            (30);

            foreach (var kv in SolutionFile.Parse(DTE.Solution.FullName).ProjectsByGuid)
            {
                switch (kv.Value.ProjectType)
                {
                    case SolutionProjectType.KnownToBeMSBuildFormat:
                        projects.Add(new ProjectReference(DTE.Solution.FullName, GetLoadedProject(kv.Value.AbsolutePath)));
                        break;
                }
            }

            return projects;
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