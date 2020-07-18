using NuGetSwitcher.Abstract;

using NuGetSwitcher.Menu;

using NuGetSwitcher.Option;

namespace NuGetSwitcher.Core.Router
{
    public sealed class CommandRouter : ICommandRouter
    {
        private readonly IPackageOption _packageOption;
        private readonly AbstractSwitch _projectSwitch;
        private readonly AbstractSwitch _packageSwitch;
        private readonly AbstractSwitch _librarySwitch;

        public CommandRouter(IPackageOption packageOption, AbstractSwitch projectSwitch, AbstractSwitch packageSwitch, AbstractSwitch librarySwitch)
        {
            _packageOption = packageOption;
            _projectSwitch = projectSwitch;
            _packageSwitch = packageSwitch;
            _librarySwitch = librarySwitch;
        }

        public void Route(string command)
        {
            switch (command)
            {
                case "CommandProject":
                    _projectSwitch.Switch();
                    break;
                case "CommandPackage":
                    _packageSwitch.Switch();
                    break;
                case "CommandLibrary":
                    _librarySwitch.Switch();
                    break;
            }
        }
    }
}