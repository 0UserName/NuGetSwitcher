using NuGetSwitcher.Interface.Contract;

using NuGetSwitcher.VSIXMenu;

namespace NuGetSwitcher
{
    internal sealed class CommandProject : GlobalCommand
    {
        internal CommandProject(ICommandProvider commandRouter, IMessageProvider messageProvider) : base(commandRouter, messageProvider)
        { }
    }
}