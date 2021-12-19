using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using NuGetSwitcher.Interface.Provider.Message.Contract;

using System;
using System.Runtime.CompilerServices;

namespace NuGetSwitcher.VSIXService.Message
{
    public sealed class VsixMessageProvider : IMessageProvider
    {
        /// <summary>
        /// Provides top-level manipulation 
        /// or maintenance of the solution.
        /// </summary>
        private readonly IVsSolution _vsSolution;

        /// <summary>
        /// 
        /// </summary>
        private readonly ErrorListProvider _errorList;

        public VsixMessageProvider(IVsSolution vsSolution, ErrorListProvider errorList)
        {
            _vsSolution = vsSolution;

            _errorList = errorList;
        }

        /// <summary>
        /// Returns the project in the solution,
        /// given a unique name.
        /// </summary>
        /// 
        /// <param name="project">
        /// Full path to the solution project.
        /// </param>
        /// 
        /// <returns>
        /// Pointer to the IVsHierarchy interface 
        /// of the project referred to by project.
        /// </returns>
        /// 
        /// <remarks>
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivshierarchy.
        /// </remarks>
        private IVsHierarchy GetProjectHierarchyItem(string project)
        {
            const int S_OK = 0;

            int exitCode = _vsSolution.GetProjectOfUniqueName(project, out IVsHierarchy hierarchyItem);

            if (exitCode != S_OK)
            {
                AddMessage($"Exit code: { exitCode }", MessageCategory.ER);
            }

            return hierarchyItem;
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// This overload fills the Project
        /// column.
        /// </summary>
        /// 
        /// <param name="project">
        /// Full path to the solution project.
        /// </param>
        public void AddMessage(string project, string message, MessageCategory category, [CallerMemberName] string caller = "")
        {
            IVsHierarchy vsh = project == default ? default : GetProjectHierarchyItem(project);

            _errorList.Tasks.Add(new ErrorTask { Text = $"Caller: { caller }. Message: { message }", ErrorCategory = (TaskErrorCategory)category, HierarchyItem = vsh, Document = project });
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        public void AddMessage(string message, MessageCategory category, [CallerMemberName] string caller = "")
        {
            AddMessage(default, message, category, caller);
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        public void AddMessage(Exception exception, [CallerMemberName] string caller = "")
        {
            SwitcherException se = exception as SwitcherException;

            if (se == default)
            {
                AddMessage(exception.Message, MessageCategory.ER, caller);
            }
            else
            {
                AddMessage(se);
            }

            _errorList.BringToFront();

            _errorList.ForceShowErrors();
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// This overload fills the Project
        /// column.
        /// </summary>
        /// 
        /// <param name="project">
        /// Full path to the solution project.
        /// </param>
        public void AddMessage(string project, Exception exception, [CallerMemberName] string caller = "")
        {
            AddMessage(project, exception.Message, MessageCategory.ER, caller);
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        public void AddMessage(SwitcherException exception, [CallerMemberName] string caller = "")
        {
            AddMessage(exception.MsbProject.FullPath, exception.Message, MessageCategory.ER, caller);
        }

        /// <summary>
        /// Clears previously created tasks 
        /// in <see cref="ErrorListProvider"/>.
        /// </summary>
        public IMessageProvider Clear()
        {
            _errorList.Tasks.Clear();

            return this;
        }
    }
}