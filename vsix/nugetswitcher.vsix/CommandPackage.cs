using NuGetSwitcher.Interface.Provider.Command.Contract;
using NuGetSwitcher.Interface.Provider.Message.Contract;

using NuGetSwitcher.VSIXMenu;

namespace NuGetSwitcher.VSIX
{
    internal sealed class CommandPackage : GlobalCommand
    {
        internal CommandPackage(ICommandProvider commandRouter, IMessageProvider messageProvider) : base(commandRouter, messageProvider)
        {
            Mode = Interface.Entity.Enum.Mode.PK;
        }
    }
}