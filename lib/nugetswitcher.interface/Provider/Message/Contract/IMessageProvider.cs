using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using System;
using System.Runtime.CompilerServices;

namespace NuGetSwitcher.Interface.Provider.Message.Contract
{
    public interface IMessageProvider
    {
        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        void AddMessage(string message, MessageCategory category, [CallerMemberName] string caller = "");

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        void AddMessage(string project, string message, MessageCategory category, [CallerMemberName] string caller = "");

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        void AddMessage(Exception exception, [CallerMemberName] string caller = "");

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        /// 
        /// <param name="project">
        /// Full path to the solution project.
        /// </param>
        void AddMessage(string project, Exception exception, [CallerMemberName] string caller = "");

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        void AddMessage(SwitcherException exception, [CallerMemberName] string caller = "");

        /// <summary>
        /// Clears previously 
        /// created messages.
        /// </summary>
        IMessageProvider Clear();
    }
}