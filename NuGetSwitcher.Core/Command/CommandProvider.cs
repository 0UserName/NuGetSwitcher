using NuGetSwitcher.Core.Abstract;

using NuGetSwitcher.Interface.Contract;

namespace NuGetSwitcher.Core.Command
{
    public sealed class CommandProvider : ICommandProvider
    {
        private readonly IOptionProvider _optionProvider;
        private readonly AbstractSwitch  _projectSwitch;
        private readonly AbstractSwitch  _packageSwitch;
        private readonly AbstractSwitch  _librarySwitch;

        public CommandProvider(IOptionProvider optionProvider, AbstractSwitch projectSwitch, AbstractSwitch packageSwitch, AbstractSwitch librarySwitch)
        {
            _optionProvider = optionProvider;
            _projectSwitch  = projectSwitch;
            _packageSwitch  = packageSwitch;
            _librarySwitch  = librarySwitch;
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