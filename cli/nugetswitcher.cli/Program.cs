using NuGetSwitcher.CLI.Args;

using NuGetSwitcher.CLIService.Logger;
using NuGetSwitcher.CLIService.Options;

using NuGetSwitcher.Core.Commands;
using NuGetSwitcher.Core.Commands.Enums;

using NuGetSwitcher.Core.Projects;
using NuGetSwitcher.Core.Solution;

using NuGetSwitcher.Interface.Logger.Enums;

using System.Reflection;
using System.Threading.Tasks;

namespace NuGetSwitcher.CLI
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            CliArguments cliArgs = new
            CliArguments
            (args);

            CliLogger logger = new
            CliLogger
            ();

            CliOptions options = new
            CliOptions
            (cliArgs.IncludeProjectFile, cliArgs.ExcludeProjectFile, logger);

            Solution<ProjectReference> solution = new
            Solution<ProjectReference>
            (cliArgs.AbsolutePath);

            ProjectCommand<ProjectReference> projectCommand = new ProjectCommand<ProjectReference>(options, solution, logger);
            PackageCommand<ProjectReference> packageCommand = new PackageCommand<ProjectReference>(options, solution, logger);

            switch (cliArgs.Mode)
            {
                case Mode.VR:
                    logger.LogMessage(Assembly.GetExecutingAssembly().GetName().Version.ToString(), Category.I);
                    break;
                case Mode.PK:
                    await packageCommand.SwitchAsync();
                    break;
                case Mode.PR:
                    await projectCommand.SwitchAsync();
                    break;
                default:
                    logger.LogMessage($"Unable to continue program execution, input arguments: { string.Join(' ', args) }", Category.E);
                    break;
            }
        }
    }
}