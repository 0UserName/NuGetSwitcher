using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using NuGetSwitcher.Core.Abstract;
using NuGetSwitcher.Core.Command;
using NuGetSwitcher.Core.Switch;

using NuGetSwitcher.Interface.Provider.Command.Contract;
using NuGetSwitcher.Interface.Provider.Option.Contract;

using NuGetSwitcher.VSIXMenu;

using NuGetSwitcher.VSIXService.Message;
using NuGetSwitcher.VSIXService.Option;
using NuGetSwitcher.VSIXService.Project;

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;

namespace NuGetSwitcher.VSIX
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("fdb266b8-b91a-4bfd-b391-a4e013c176e2")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(VsixPackageOption), "NuGet Switcher", "Common", 0, 0, true)]
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

#pragma warning disable VSSDK006 // Check services exist
            IVsSolution vsSolution = await GetServiceAsync(typeof(IVsSolution)) as IVsSolution;
#pragma warning restore VSSDK006 // Check services exist

            vsSolution.GetSolutionInfo(out string pbstrSolutionDirectory, out string pbstrSolutionFile, out string pbstrUserOptsFile);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);


            // Provider.
            VsixMessageProvider messageProvider = new VsixMessageProvider(vsSolution, new ErrorListProvider(this));
            VsixProjectProvider projectProvider = new VsixProjectProvider(pbstrSolutionFile);


            // Option provider.
            IOptionProvider optionProvider = new VsixOptionProvider(messageProvider);

            RegisterService(optionProvider);


            // Package option.
            VsixPackageOption vsixPackageOption = (VsixPackageOption)GetDialogPage(typeof(VsixPackageOption));


            // Switch.
            AbstractSwitch projectSwtich = new ProjectSwitch(vsixPackageOption, projectProvider, messageProvider);
            AbstractSwitch packageSwitch = new PackageSwitch(vsixPackageOption, projectProvider, messageProvider);


            // Package command.
            OleMenuCommandService oleMenuCommandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            ICommandProvider commandRouter = new CommandProvider(projectSwtich, packageSwitch);

            new CommandProject(commandRouter, messageProvider).Initialize(oleMenuCommandService, new Guid("c6018e68-fcab-41d2-a34a-42f7df92b162"), 0x0100);
            new CommandPackage(commandRouter, messageProvider).Initialize(oleMenuCommandService, new Guid("c6018e68-fcab-41d2-a34a-42f7df92b162"), 0x0200);
        }

        private void RegisterService<T>(T service)
        {
            ((IServiceContainer)this).AddService(typeof(T), service, true);
        }
    }
}