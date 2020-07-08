using NuGetSwitcher.Helper.Entity.Error;

using System.Collections.ObjectModel;

namespace NuGetSwitcher.Core.Switch
{
    public interface IProjectSwtich
    {
        /// <summary>
        /// Switches references to NuGet packages to
        /// references to projects, given implicit /
        /// transitive dependencies.
        /// </summary>
        /// 
        /// <exception cref="SwitcherFileNotFoundException"/>
        void SwithPackage(ReadOnlyDictionary<string, string> projectPaths);
    }
}