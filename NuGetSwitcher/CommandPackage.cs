using NuGetSwitcher.Interface.Contract;

using NuGetSwitcher.VSIXMenu;

using System;
using System.ComponentModel.Design;

namespace NuGetSwitcher
{
    internal sealed class CommandPackage : GlobalCommand
    {
        public CommandPackage(ICommandProvider commandRouter, IMessageProvider messageProvider) : base(commandRouter, messageProvider)
        { }

        /// <summary>
        /// Callback for <see cref="MenuCommand"/>.
        /// </summary>
        public override void Callback(object sender, EventArgs eventArgs)
        {
            try
            {
                CommandProvider.Route(Name);
            }
            catch (Exception exception)
            {
                MessageProvider.AddMessage(exception);
            }
        }
    }
}