using NuGetSwitcher.Interface.Logger.Enums;

using System;
using System.Runtime.CompilerServices;

namespace NuGetSwitcher.Interface.Logger
{
    public interface ILogger
    {
        /// <summary>
        /// Displays a message of the specified type.
        /// </summary>
        void LogMessage(string message, Category category, [CallerMemberName] string caller = "");

        /// <inheritdoc cref="LogMessage(string, Category, string)"/>
        void LogMessage(string message, Category category, string project, [CallerMemberName] string caller = "");

        /// <inheritdoc cref="LogMessage(string, Category, string)"/>
        void LogMessage(Exception exception, [CallerMemberName] string caller = "");

        /// <inheritdoc cref="LogMessage(string, Category, string)"/>
        void LogMessage(Exception exception, string project, [CallerMemberName] string caller = "");

        /// <summary>
        /// Clears previously created messages.
        /// </summary>
        void Clear();
    }
}