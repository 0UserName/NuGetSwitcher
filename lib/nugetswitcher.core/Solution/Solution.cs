using Microsoft.Build.Evaluation;

using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

using NuGetSwitcher.Interface.Projects;
using NuGetSwitcher.Interface.Solution;

using System;
using System.Collections.Generic;

using System.IO;
using System.Threading.Tasks;

namespace NuGetSwitcher.Core.Solution
{
    public sealed class Solution<TProject>(string absolutePath) : ISolution<TProject> where TProject : class, IProject
    {
        /// <inheritdoc/>
        public string AbsolutePath
        {
            get => absolutePath;
        }

        /// <inheritdoc/>
        public TProject GetLoadedProject(string project)
        {
            return (TProject)Activator.CreateInstance(typeof(TProject), new Project(project, default, default, ProjectCollection.GlobalProjectCollection, ProjectLoadSettings.IgnoreMissingImports));
        }

        /// <inheritdoc/>
        public async ValueTask<IEnumerable<TProject>> GetLoadedProjectsAsync()
        {
            List<TProject> projects = new
            List<TProject>
            (30);

            SolutionModel solution = await SolutionSerializers.GetSerializerByMoniker(absolutePath).OpenAsync(absolutePath, default);

            foreach (SolutionProjectModel project in solution.SolutionProjects)
            {
                projects.Add(GetLoadedProject(Path.Combine(Path.GetDirectoryName(absolutePath), project.FilePath)));
            }

            return projects;
        }
    }
}