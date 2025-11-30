using System.Collections.Generic;

namespace NuGetSwitcher.Interface.Options
{
    public interface IOptions
    {
        /// <summary>
        /// Returns a dictionary 
        /// in which file names serve as 
        /// keys and absolute file paths 
        /// serve as values.
        /// </summary>
        IReadOnlyDictionary<string, string> GetIncludeProjects();
    }
}