using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using System;
using System.Runtime.CompilerServices;

namespace NuGetSwitcher.VSIXService.Message
{
    public class VsixMessageProvider : IMessageProvider
    {
        protected IVsSolution VsSolution
        {
            get;
            set;
        }

        protected ErrorListProvider ErrorList
        {
            get;
            set;
        }

        public VsixMessageProvider(IVsSolution vsSolution, ErrorListProvider errorList)
        {
            VsSolution = vsSolution;

            ErrorList = errorList;
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        public virtual void AddMessage(string message, MessageCategory category, [CallerMemberName] string caller = "")
        {
            ErrorList.Tasks.Add(new ErrorTask { Text = $"Caller: { caller }. Message: { message }", ErrorCategory = (TaskErrorCategory)category });
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
        public virtual void AddMessage(string project, string message, MessageCategory category, [CallerMemberName] string caller = "")
        {
            ErrorList.Tasks.Add(new ErrorTask { Text = $"Caller: { caller }. Message: { message }", ErrorCategory = (TaskErrorCategory)category, HierarchyItem = GetProjectHierarchyItem(project), Document = project });
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        public virtual void AddMessage(Exception exception, [CallerMemberName] string caller = "")
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
        public virtual void AddMessage(string project, Exception exception, [CallerMemberName] string caller = "")
        {
            AddMessage(project, exception.Message, MessageCategory.ER, caller);
        }

        /// <summary>
        /// Displays a message 
        /// with the specified 
        /// type in the associated GUI tab.
        /// </summary>
        public virtual void AddMessage(SwitcherException exception, [CallerMemberName] string caller = "")
        {
            AddMessage(exception.MsbProject.FullPath, exception.Message, MessageCategory.ER, caller);
        }

        /// <summary>
        /// Clears previously created tasks 
        /// in <see cref="ErrorListProvider"/>.
        /// </summary>
        public virtual void Clear()
        {
            ErrorList.Tasks.Clear();
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
        protected virtual IVsHierarchy GetProjectHierarchyItem(string project)
        {
            const int S_OK = 0;

            int exitCode = VsSolution.GetProjectOfUniqueName(project, out IVsHierarchy hierarchyItem);

            if (exitCode != S_OK)
            {
                AddMessage($"Exit code: { exitCode }", MessageCategory.ER);
            }

            return hierarchyItem;
        }
    }
}