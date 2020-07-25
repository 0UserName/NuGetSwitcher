using NuGetSwitcher.Interface.Entity.Enum;

using System;
using System.Collections.ObjectModel;
using System.IO;

namespace NuGetSwitcher.Interface.Contract
{
    public interface IOptionProvider
    {
        string IncludeProjectFile
        {
            get;
            set;
        }

        string IncludeLibraryFile
        {
            get;
            set;
        }

        string ExcludeProjectFile
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a dictionary where file names are 
        /// used as keys, and absolute file paths are 
        /// values.
        /// </summary>
        /// 
        /// <exception cref="FileNotFoundException"/>
        /// 
        /// <exception cref="ArgumentException">
        ReadOnlyDictionary<string, string> GetIncludeItems(ReferenceType type);
    }
}