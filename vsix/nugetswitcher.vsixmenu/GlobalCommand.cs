using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Interface.Entity.Enum;

using NuGetSwitcher.Interface.Provider.Command.Contract;
using NuGetSwitcher.Interface.Provider.Message.Contract;

using System;
using System.ComponentModel.Design;

namespace NuGetSwitcher.VSIXMenu
{
    /// <summary>
    /// Basic interface of the 
    /// <see cref="MenuCommand"/> 
    /// buttons.
    /// </summary>
    public abstract class GlobalCommand : IGlobalCommand
    {
        public ICommandProvider CommandProvider
        {
            get;
            protected set;
        }

        public IMessageProvider MessageProvider
        {
            get;
            protected set;
        }

        public Mode Mode
        {
            get;
            protected set;
        }

        protected GlobalCommand(ICommandProvider commandProvider, IMessageProvider messageProvider)
        {
            CommandProvider = commandProvider;
            MessageProvider = messageProvider;
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
        public virtual void Callback(object sender, EventArgs eventArgs)
        {
            try
            {
                CommandProvider.Route(Mode);
            }
            catch (Exception exception)
            {
                MessageProvider.AddMessage(exception);
            }
        }
    }
}