using Microsoft.Build.Evaluation;

using NuGet.ProjectModel;

using System.Collections.Generic;

namespace NuGetSwitcher.Interface.Projects
{
    public interface IProject
    {
        /// <summary>
        /// Returns the list of 
        /// libraries from the target 
        /// sections of the lock file.
        /// </summary>
        IEnumerable<LockFileTargetLibrary> GetLockFileLibraries();

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<ProjectItem> GetItems(string itemType, string metadata);

        /// <inheritdoc cref="Project.GetItemsByEvaluatedInclude(string)"/>
        ICollection<ProjectItem> GetReference(string include);

        /// <summary>
        /// 
        /// </summary>
        void AddReference(string include, string referenceType, Dictionary<string, string> metadata);

        /// <inheritdoc cref="Project.RemoveItem(ProjectItem)"/>
        void RemoveReference(ProjectItem reference);

        /// <summary>
        /// 
        /// </summary>
        bool ContainsReference(string include);

        /// <inheritdoc cref="Project.Save()"/>
        void Save();
    }
}