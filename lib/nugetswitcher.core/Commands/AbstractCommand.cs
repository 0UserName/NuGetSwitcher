using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;

using NuGetSwitcher.Interface.Logger;
using NuGetSwitcher.Interface.Logger.Enums;

using System.Threading.Tasks;

namespace NuGetSwitcher.Core.Commands
{
    public abstract class AbstractCommand(ILogger logger)
    {
        protected const string METADATA_TEMP = "Temp";
        protected const string METADATA_NAME = "Name";

        protected ILogger Logger
        {
            get => logger;
        }

        /// <summary>
        /// Performs an action on the solution
        /// file using the builder's values as 
        /// arguments.
        /// </summary>
        ///
        /// <param name="absolutePath">
        /// Absolute path to the solution file.
        /// </param>
        protected async Task SlnActionAsync(string absolutePath, ArgumentsBuilder builder)
        {
            BufferedCommandResult result = await Cli.Wrap("dotnet").WithArguments(args => args.Add("sln").Add(absolutePath).Add(builder.Build(), default)).ExecuteBufferedAsync();

            if (result.IsSuccess)
            {
                logger.LogMessage(result.StandardOutput, Category.I);
            }
            else
            {
                logger.LogMessage(result.StandardError, Category.E);
            }
        }

        public abstract ValueTask SwitchAsync();
    }
}