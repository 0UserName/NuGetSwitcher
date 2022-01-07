using NuGetSwitcher.CLI.Args;

using NuGetSwitcher.CLIService.Message;
using NuGetSwitcher.CLIService.Option;
using NuGetSwitcher.CLIService.Project;

using NuGetSwitcher.Core.Abstract;
using NuGetSwitcher.Core.Command;
using NuGetSwitcher.Core.Switch;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Provider.Option.Contract;

using System.Reflection;

namespace NuGetSwitcher.CLI
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            CliArguments cliArgs = args;


            CliMessageProvider messageProvider = new CliMessageProvider();
            CliProjectProvider projectProvider = new CliProjectProvider(cliArgs.Solution);


            IOptionProvider optionProvider = new CliOptionProvider(messageProvider)
            {
                IncludeProjectFile = cliArgs.IncludeProjectFile,
                ExcludeProjectFile = cliArgs.ExcludeProjectFile
            };


            AbstractSwitch projectSwtich = new ProjectSwitch(optionProvider, projectProvider, messageProvider);
            AbstractSwitch packageSwitch = new PackageSwitch(optionProvider, projectProvider, messageProvider);


            CommandProvider commandProvider = new
            CommandProvider
            (projectSwtich, packageSwitch);


            switch (cliArgs.Mode)
            {
                case Mode.VR:
                    messageProvider.AddMessage(Assembly.GetExecutingAssembly().GetName().Version.ToString(), MessageCategory.ME);
                    break;
                case Mode.PR:
                case Mode.PK:
                    commandProvider.Route(cliArgs.Mode);
                    break;
                default:
                    messageProvider.AddMessage($"Unable to continue program execution, input arguments: { string.Join(" ", args) }", MessageCategory.ER);
                    break;
            }
        }
    }
}