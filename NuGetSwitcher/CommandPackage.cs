using NuGetSwitcher.Interface.Contract;

using NuGetSwitcher.VSIXMenu;

namespace NuGetSwitcher
{
    internal sealed class CommandPackage : GlobalCommand
    {
        internal CommandPackage(ICommandProvider commandRouter, IMessageProvider messageProvider) : base(commandRouter, messageProvider)
        { }
    }
}