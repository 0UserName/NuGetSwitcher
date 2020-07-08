using NuGetSwitcher.Helper;

using NuGetSwitcher.Menu;

using System;
using System.ComponentModel.Design;

namespace NuGetSwitcher
{
    internal sealed class CommandProject : GlobalCommand
    {
        public CommandProject(ICommandRouter commandRouter, IMessageHelper messageHelper) : base(commandRouter, messageHelper)
        { }

        /// <summary>
        /// Callback for <see cref="MenuCommand"/>.
        /// </summary>
        public override void Callback(object sender, EventArgs eventArgs)
        {
            try
            {
                CommandRouter.Route(Name);
            }
            catch (Exception exception)
            {
                MessageHelper.AddMessage(exception);
            }
        }
    }
}