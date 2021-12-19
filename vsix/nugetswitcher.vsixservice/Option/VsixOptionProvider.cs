using NuGetSwitcher.Interface.Provider.Message.Contract;
using NuGetSwitcher.Interface.Provider.Option;

namespace NuGetSwitcher.VSIXService.Option
{
    public sealed class VsixOptionProvider : AbstractOptionProvider
    {
        public VsixOptionProvider(IMessageProvider messageProvider) : base(messageProvider)
        { }
    }
}