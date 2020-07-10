using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using NuGetSwitcher.Core.Option;
using NuGetSwitcher.Core.Router;
using NuGetSwitcher.Core.Switch;

using NuGetSwitcher.Helper;

using NuGetSwitcher.Menu;

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;

namespace NuGetSwitcher
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("fdb266b8-b91a-4bfd-b391-a4e013c176e2")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(PackageOption), "NuGet Switcher", "Common", 0, 0, true)]
    public sealed class Main : AsyncPackage
    {
        /// <summary>
        /// Initialization of the package; this method is called right
        /// after the package is sited, so this is the place where you 
        /// can put all the initialization code that rely on services 
        /// provided by VisualStudio.
        /// </summary>
        /// 
        /// <param name="cancellationToken">
        /// A cancellation token to monitor
        /// for initialization cancellation,
        /// which can occur when VS is 
        /// shutting down.
        /// </param>
        /// 
        /// <param name="progress">
        /// A provider for progress updates.
        /// </param>
        /// 
        /// <returns>
        /// A task representing the async work of package initialization,
        /// or an already completed task if there is none. Do not return 
        /// null from this method.
        /// </returns>
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            OleMenuCommandService oleMenuCommandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            IVsSolution vsSolution = await GetServiceAsync(typeof(IVsSolution)) as IVsSolution;

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            IMessageHelper messageHelper = new MessageHelper(vsSolution, new ErrorListProvider(this));
            IProjectHelper projectHelper = new ProjectHelper();

            IProjectSwtich projectSwtich = new ProjectSwitch(projectHelper, messageHelper, true);
            IPackageSwitch packageSwitch = new PackageSwitch(projectHelper, messageHelper, true);

            IPackageOption packageOption = (IPackageOption)GetDialogPage(typeof(PackageOption));

            ICommandRouter commandRouter = new CommandRouter(projectSwtich, packageSwitch, packageOption);

            new CommandProject(commandRouter, messageHelper).Initialize(oleMenuCommandService, new Guid("c6018e68-fcab-41d2-a34a-42f7df92b162"), 0x0100);
            new CommandPackage(commandRouter, messageHelper).Initialize(oleMenuCommandService, new Guid("c6018e68-fcab-41d2-a34a-42f7df92b162"), 0x0200);
        }
    }
}