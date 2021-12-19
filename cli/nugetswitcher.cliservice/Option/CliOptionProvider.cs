using NuGetSwitcher.Interface.Provider.Message.Contract;
using NuGetSwitcher.Interface.Provider.Option;

namespace NuGetSwitcher.CLIService.Option
{
    public sealed class CliOptionProvider : AbstractOptionProvider
    {
        public CliOptionProvider(IMessageProvider messageProvider) : base(messageProvider)
        { }
    }
}