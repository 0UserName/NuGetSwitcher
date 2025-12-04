using Microsoft.Build.Evaluation;

using NuGet.Common;
using NuGet.ProjectModel;

using NuGetSwitcher.Core.Exceptions;
using NuGetSwitcher.Interface.Projects;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Projects
{
    [DebuggerDisplay("UniqueName => { ToString() }")]
    public sealed class ProjectReference(Project project) : IProject
    {
        private string GetProjectValue(string name)
        {
            string value = project.GetPropertyValue(name);

            return string.IsNullOrEmpty(value) ? null : value;
        }

        private string GetFramework()
        {            
            return 
                GetProjectValue("TargetFrameworkMoniker") ?? // Can be null
                GetProjectValue("TargetFramework") ??
                GetProjectValue("TargetFrameworks").Split(';')[0];
        }
        /// <inheritdoc/>
        public IEnumerable<LockFileTargetLibrary> GetLockFileLibraries() // refactor
        {
            string path = Path.Combine(project.DirectoryPath, "obj", "project.assets.json");

            return LockFileUtilities.GetLockFile(path, NullLogger.Instance)?.GetTarget(GetFramework(), default)?.Libraries ?? throw new SwitcherFileNotFoundException($"{path} not found, try to rebuild project", project);
        }

        /// <inheritdoc/>>
        public IEnumerable<ProjectItem> GetItems(string itemType, string metadata)
        {
            foreach (ProjectItem item in project.GetItems(itemType).ToList())
            {
                if (item.HasMetadata(metadata))
                {
                    yield return item;
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<ProjectItem> GetReference(string include)
        {
            return project.GetItemsByEvaluatedInclude(include);
        }

        /// <inheritdoc/>
        public void AddReference(string include, string referenceType, Dictionary<string, string> metadata)
        {
            if (!ContainsReference(include)) // maybe need to remove..
            {
                project.AddItem(referenceType, include, metadata);
            }
        }

        /// <inheritdoc/>
        public void RemoveReference(ProjectItem reference)
        {
            project.RemoveItem(reference);
        }

        /// <inheritdoc/>
        public bool ContainsReference(string include)
        {
            return GetReference(include).Count != 0;
        }

        /// <inheritdoc/>
        public void Save()
        {
            project.Save();
        }

        public override string ToString()
        {
            return project.FullPath;
        }
    }
}