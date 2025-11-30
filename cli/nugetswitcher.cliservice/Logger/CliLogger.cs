using NuGetSwitcher.Interface.Logger;
using NuGetSwitcher.Interface.Logger.Enums;

using System;
using System.IO;

using System.Runtime.CompilerServices;

namespace NuGetSwitcher.CLIService.Logger
{
    public sealed class CliLogger : ILogger
    {
        private static readonly
            ConsoleColor[] _colors = new
            ConsoleColor[]
            { ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.DarkGreen };

        /// <inheritdoc/>
        public void LogMessage(string message, Category category, [CallerMemberName] string caller = "")
        {
            Console.ForegroundColor = _colors[(int)category];

            Console.WriteLine(message);

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <inheritdoc/>
        public void LogMessage(string message, Category category, string project, [CallerMemberName] string caller = "")
        {
            LogMessage($"{ project } - { message }", category);
        }

        /// <inheritdoc/>
        public void LogMessage(Exception exception, [CallerMemberName] string caller = "")
        {
            LogMessage(exception.ToString(), Category.E);
        }

        /// <inheritdoc/>
        public void LogMessage(Exception exception, string project, [CallerMemberName] string caller = "")
        {
            LogMessage(exception.ToString(), Category.E, project: project);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            try
            {
                Console.Clear();
            }
            catch (IOException exception)
            {
                LogMessage(exception);
            }
        }
    }
}