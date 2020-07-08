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
    public interface IGlobalCommand
    {
        ICommandRouter CommandRouter
        {
            get;
        }

        IMessageHelper MessageHelper
        {
            get;
        }

        string Name
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