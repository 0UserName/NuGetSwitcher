using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Helper;

using System;
using System.ComponentModel.Design;

namespace NuGetSwitcher.Menu
{
    /// <summary>
    /// Basic interface of the 
    /// <see cref="MenuCommand"/> 
    /// buttons.
    /// </summary>
    public abstract class GlobalCommand : IGlobalCommand
    {
        public ICommandRouter CommandRouter
        {
            get;
            private set;
        }

        public IMessageHelper MessageHelper
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        protected GlobalCommand(ICommandRouter commandRouter, IMessageHelper messageHelper)
        {
            CommandRouter = commandRouter;
            MessageHelper = messageHelper;

            Name = GetType().UnderlyingSystemType.Name;
        }

        /// <summary>
        /// Registers <see cref="MenuCommand"/>.
        /// </summary>
        public void Initialize(OleMenuCommandService commandService, Guid groupId, int commandId)
        {
            commandService.AddCommand(new MenuCommand(Callback, new CommandID(groupId, commandId)));
        }

        /// <summary>
        /// Callback for <see cref="MenuCommand"/>.
        /// </summary>
        public abstract void Callback(object sender, EventArgs eventArgs);
    }
}