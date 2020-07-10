using EnvDTE;

using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Enum;
using NuGetSwitcher.Helper.Entity.Error;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public abstract class AbstractSwitch
    {
        /// <summary>
        /// Indicates that the call 
        /// is from Visual Studio.
        /// </summary>
        protected bool IsVSIX
        {
            get;
            private set;
        }

        protected IMessageHelper MessageHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the DTE object if the call is
        /// made from Visual Studio, otherwise 
        /// null.
        /// </summary>
        ///
        /// <remarks>
        /// DTE - Development Tools Environment. See: https://www.viva64.com/en/a/0082/#ID4ED3F15B53.
        /// </remarks>
        protected DTE DTE
        {
            get;
            private set;
        }

        /// <param name="isVSIX">
        /// Indicates that the call 
        /// is from Visual Studio.
        /// </param>
        public AbstractSwitch(bool isVSIX, IMessageHelper messageHelper)
        {
            IsVSIX = isVSIX;

            if (IsVSIX)
            {
                DTE = Package.GetGlobalService(typeof(DTE)) as DTE;
            }

            MessageHelper = messageHelper;
        }

        /// <summary>
        /// Adds reference to the project. It is assumed
        /// that the original reference has been removed 
        /// earlier.
        /// </summary>
        /// 
        /// <param name="unevaluatedInclude">
        /// Must contain the assembly 
        /// name or the absolute path 
        /// to the project or Package
        /// Id.
        /// </param>
        /// 
        /// <exception cref="SwitcherException"/>
        /// 
        /// <returns>
        /// Returns false for duplicate unevaluatedInclude values.
        /// </returns>
        protected virtual bool AddReference(ProjectReference reference, ReferenceType type, string unevaluatedInclude, Dictionary<string, string> metadata)
        {
            bool output = true;

            switch (type)
            {
                case ReferenceType.ProjectReference:
                case ReferenceType.PackageReference:
                case ReferenceType.Reference:
                    if (reference.MsbProject.GetItemsByEvaluatedInclude(unevaluatedInclude).Any())
                    {
                        output = false;
                    }
                    else
                    {
                        reference.MsbProject.AddItem(type.ToString(), unevaluatedInclude, metadata);
                    }
                    break;
                default:
                    throw new SwitcherException(reference.MsbProject, $"Reference type not supported: { type }");
            }

            if (output)
            {
                MessageHelper.AddMessage(reference.DteProject.UniqueName, $"Dependency: { Path.GetFileName(unevaluatedInclude) } has been added. Type: { type }", TaskErrorCategory.Message);
            }

            return output;
        }
    }
}