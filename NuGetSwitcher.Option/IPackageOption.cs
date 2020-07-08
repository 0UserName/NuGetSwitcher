using System.Collections.Generic;

namespace NuGetSwitcher.Core.Option
{
    public interface IPackageOption
    {
        /// <summary>
        /// Returns a set of user-defined projects 
        /// location used when switching reference.
        /// </summary>
        HashSet<string> GetProjectLocation();
    }
}