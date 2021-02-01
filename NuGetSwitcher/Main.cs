using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using NuGetSwitcher.Core.Abstract;
using NuGetSwitcher.Core.Command;
using NuGetSwitcher.Core.Option;
using NuGetSwitcher.Core.Switch;

using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.VSIXService.Message;
using NuGetSwitcher.VSIXService.Option;
using NuGetSwitcher.VSIXService.Project;

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;

namespace NuGetSwitcher
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

            OleMenuCommandService oleMenuCommandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            IVsSolution vsSolution = await GetServiceAsync(typeof(IVsSolution)) as IVsSolution;

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            IMessageProvider messageProvider = new VsixMessageProvider(vsSolution, new ErrorListProvider(this));
            IProjectProvider projectProvider = new VsixProjectProvider();

            IOptionProvider optionProvider = new OptionProvider(messageProvider);

            VsixPackageOption vsixPackageOption = ((VsixPackageOption)GetDialogPage(typeof(VsixPackageOption))).Init(optionProvider);

            AbstractSwitch projectSwtich = new ProjectSwitch(ReferenceType.ProjectReference, vsixPackageOption, projectProvider, messageProvider);
            AbstractSwitch packageSwitch = new PackageSwitch(ReferenceType.PackageReference, vsixPackageOption, projectProvider, messageProvider);
            AbstractSwitch librarySwitch = new LibrarySwitch(ReferenceType.Reference       , vsixPackageOption, projectProvider, messageProvider);

            ICommandProvider commandRouter = new CommandProvider(vsixPackageOption, projectSwtich, packageSwitch, librarySwitch);

            new CommandProject(commandRouter, messageProvider).Initialize(oleMenuCommandService, new Guid("c6018e68-fcab-41d2-a34a-42f7df92b162"), 0x0100);
            new CommandPackage(commandRouter, messageProvider).Initialize(oleMenuCommandService, new Guid("c6018e68-fcab-41d2-a34a-42f7df92b162"), 0x0200);
            new CommandLibrary(commandRouter, messageProvider).Initialize(oleMenuCommandService, new Guid("c6018e68-fcab-41d2-a34a-42f7df92b162"), 0x0300);
        }
    }
}