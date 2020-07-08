using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Helper;

namespace NuGetSwitcher.Core.Switch
{
    public class PackageSwitch : IPackageSwitch
    {
        protected IProjectHelper ProjectHelper
        {
            get;
            set;
        }

        protected IMessageHelper MessageHelper
        {
            get;
            set;
        }

        public PackageSwitch(IProjectHelper projectHelper, IMessageHelper messageHelper)
        {
            ProjectHelper = projectHelper;
            MessageHelper = messageHelper;
        }

        public virtual void SwithProject()
        {
            MessageHelper.AddMessage("Switching to packages is not implemented", TaskErrorCategory.Warning);
        }
    }
}