﻿using Microsoft.VisualStudio.Shell;

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
    public interface IGlobalCommand
    {
        ICommandProvider CommandProvider
        {
            get;
        }

        IMessageProvider MessageProvider
        {
            get;
        }

        Mode Mode
        {
            get;
        }

        /// <summary>
        /// Registers <see cref="MenuCommand"/>.
        /// </summary>
        void Initialize(OleMenuCommandService commandService, Guid groupId, int commandId);

        /// <summary>
        /// Callback for <see cref="MenuCommand"/>.
        /// </summary>
        void Callback(object sender, EventArgs eventArgs);
    }
}