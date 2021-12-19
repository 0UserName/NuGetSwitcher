using NuGetSwitcher.Interface.Provider.Command.Contract;
using NuGetSwitcher.Interface.Provider.Message.Contract;

using NuGetSwitcher.VSIXMenu;

namespace NuGetSwitcher.VSIX
{
    internal sealed class CommandProject : GlobalCommand
    {
        internal CommandProject(ICommandProvider commandRouter, IMessageProvider messageProvider) : base(commandRouter, messageProvider)
        {
            Mode = Interface.Entity.Enum.Mode.PR;
        }
    }
}