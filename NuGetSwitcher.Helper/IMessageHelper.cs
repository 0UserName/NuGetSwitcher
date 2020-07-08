using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Helper.Entity.Error;

using System;
using System.Runtime.CompilerServices;

namespace NuGetSwitcher.Helper
{
    public interface IMessageHelper
    {
        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        void AddMessage(string message, TaskErrorCategory category, [CallerMemberName]string caller = "");

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// This overload fills the Project
        /// column.
        /// </summary>
        /// 
        /// <param name="project">
        /// Full path to the solution project.
        /// </param>
        void AddMessage(string project, string message, TaskErrorCategory category, [CallerMemberName]string caller = "");

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        void AddMessage(Exception exception, [CallerMemberName]string caller = "");

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// This overload fills the Project
        /// column.
        /// </summary>
        /// 
        /// <param name="project">
        /// Full path to the solution project.
        /// </param>
        void AddMessage(string project, Exception exception, [CallerMemberName]string caller = "");

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        void AddMessage(SwitcherException exception, [CallerMemberName]string caller = "");

        /// <summary>
        /// Clears previously created tasks 
        /// in <see cref="ErrorListProvider"/>.
        /// </summary>
        void Clear();
    }
}