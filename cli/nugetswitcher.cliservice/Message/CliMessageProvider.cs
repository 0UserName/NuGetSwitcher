using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using NuGetSwitcher.Interface.Provider.Message.Contract;

using System;
using System.Runtime.CompilerServices;

namespace NuGetSwitcher.CLIService.Message
{
    public sealed class CliMessageProvider : IMessageProvider
    {
        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        public void AddMessage(string message, MessageCategory category, [CallerMemberName] string caller = "")
        {
            Console.WriteLine($"{ message }");
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        public void AddMessage(string project, string message, MessageCategory category, [CallerMemberName] string caller = "")
        {
            Console.WriteLine($"{ project } - { message }");
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        public void AddMessage(Exception exception, [CallerMemberName] string caller = "")
        {
            Console.WriteLine(exception.ToString());
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        /// 
        /// <param name="project">
        /// Full path to the solution project.
        /// </param>
        public void AddMessage(string project, Exception exception, [CallerMemberName] string caller = "")
        {
            Console.WriteLine($"{ project } - { exception}");
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type.
        /// </summary>
        public void AddMessage(SwitcherException exception, [CallerMemberName] string caller = "")
        {
            Console.WriteLine(exception.ToString());
        }

        /// <summary>
        /// Clears previously 
        /// created messages.
        /// </summary>
        public IMessageProvider Clear()
        {
            return this;
        }
    }
}