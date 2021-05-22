using NuGetSwitcher.Interface.Contract;

using NuGetSwitcher.VSIXMenu;

namespace NuGetSwitcher
{
    internal sealed class CommandLibrary : GlobalCommand
    {
        internal CommandLibrary(ICommandProvider commandRouter, IMessageProvider messageProvider) : base(commandRouter, messageProvider)
        { }
    }
}