using NuGetSwitcher.Core.Abstract;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Provider.Command.Contract;

namespace NuGetSwitcher.Core.Command
{
    public sealed class CommandProvider : ICommandProvider
    {
        private readonly AbstractSwitch _projectSwitch;
        private readonly AbstractSwitch _packageSwitch;

        public CommandProvider(AbstractSwitch projectSwitch, AbstractSwitch packageSwitch)
        {
            _projectSwitch = projectSwitch;
            _packageSwitch = packageSwitch;
        }

        public void Route(Mode command)
        {
            switch (command)
            {
                case Mode.PR:
                    _projectSwitch.Switch();
                    break;
                case Mode.PK:
                    _packageSwitch.Switch();
                    break;
            }
        }
    }
}