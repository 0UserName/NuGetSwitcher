using System.Collections.Generic;
using System.IO;

namespace NuGetSwitcher.Core.Option
{
    public interface IPackageOption
    {
        /// <summary>
        /// Returns a set of user-defined projects 
        /// location used when switching reference.
        /// </summary>
        /// 
        /// <exception cref="FileNotFoundException"/>
        HashSet<string> GetProjectLocation();
    }
}