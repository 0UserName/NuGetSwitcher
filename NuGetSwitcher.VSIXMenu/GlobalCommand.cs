using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Interface.Contract;

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

        public string Name
        {
            get;
            protected set;
        }

        protected GlobalCommand(ICommandProvider commandProvider, IMessageProvider messageProvider)
        {
            CommandProvider = commandProvider;
            MessageProvider = messageProvider;

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
        public virtual void Callback(object sender, EventArgs eventArgs)
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