using NuGetSwitcher.Helper.Entity.Enum;

using System;
using System.Collections.ObjectModel;
using System.IO;

namespace NuGetSwitcher.Option
{
    public interface IPackageOption
    {
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