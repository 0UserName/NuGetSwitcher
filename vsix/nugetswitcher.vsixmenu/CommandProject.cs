using NuGetSwitcher.Interface.Provider.Command.Contract;
using NuGetSwitcher.Interface.Provider.Message.Contract;

namespace NuGetSwitcher.VSIXMenu
{
    public sealed class CommandProject : GlobalCommand
    {
        public CommandProject(ICommandProvider commandRouter, IMessageProvider messageProvider) : base(commandRouter, messageProvider)
        {
            Mode = Interface.Entity.Enum.Mode.PR;
        }
    }
}