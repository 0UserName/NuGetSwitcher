using NuGetSwitcher.Helper.Entity.Error;

namespace NuGetSwitcher.Core.Switch
{
    public interface IPackageSwitch
    {
        /// <summary>
        /// Switches references to
        /// projects to references 
        /// to NuGet packages.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        void SwithProject();
    }
}