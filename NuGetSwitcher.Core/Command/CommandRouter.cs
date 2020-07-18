using NuGetSwitcher.Core.Option;
using NuGetSwitcher.Core.Switch;

using NuGetSwitcher.Menu;

using NuGetSwitcher.Option;

namespace NuGetSwitcher.Core.Router
{
    public class CommandRouter : ICommandRouter
    {
        protected readonly IProjectSwtich ProjectSwitch;
        protected readonly IPackageSwitch PackageSwitch;
        protected readonly IPackageOption PackageOption;

        public CommandRouter(IProjectSwtich projectSwitch, IPackageSwitch packageSwitch, IPackageOption packageOption)
        {
            ProjectSwitch = projectSwitch;
            PackageSwitch = packageSwitch;
            PackageOption = packageOption;
        }

        public virtual void Route(string command)
        {
            switch (command)
            {
                case "CommandProject":
                    _projectSwitch.Switch();
                    break;
                case "CommandPackage":
                    PackageSwitch.SwithProject();
                    break;
            }
        }
    }
}