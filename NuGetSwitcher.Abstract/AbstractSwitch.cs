using EnvDTE;

using Microsoft.VisualStudio.Shell;

using NuGet.ProjectModel;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Enum;
using NuGetSwitcher.Helper.Entity.Error;

using NuGetSwitcher.Option;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Abstract
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

        protected ReferenceType Type
        {
            get;
            private set;
        }

        protected IPackageOption PackageOption
        {
            get;
            set;
        }

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
        protected AbstractSwitch(bool isVSIX, ReferenceType type, IPackageOption packageOption, IProjectHelper projectHelper, IMessageHelper messageHelper)
        {
            IsVSIX = isVSIX;

            if (IsVSIX)
            {
                DTE = Package.GetGlobalService(typeof(DTE)) as DTE;
            }

            Type = type;

            PackageOption = packageOption;
            ProjectHelper = projectHelper;
            MessageHelper = messageHelper;
        }

        /// <summary>
        /// Basic operation of 
        /// switching one type 
        /// of reference to 
        /// another.
        /// </summary>
        public abstract void Switch();

        /// <summary>
        /// Iterates through the dependencies provided in the lock file and matches 
        /// against items found in the directories listed in the configuration file. 
        /// For each matched item, the passed delegate is executed.
        /// </summary>
        ///
        /// <exception cref="SwitcherFileNotFoundException"/>
        /// 
        /// <exception cref="FileNotFoundException"/>
        /// 
        /// <exception cref="ArgumentException">
        protected virtual void IterateAndExecute(IEnumerable<ProjectReference> references, Action<ProjectReference, LockFileTargetLibrary, string> func)
        {
            MessageHelper.Clear();

            ReadOnlyDictionary<string, string> items = PackageOption.GetIncludeItems(Type);

            foreach (ProjectReference reference in references)
            {
                foreach (LockFileTargetLibrary library in PackageHelper.GetProjectTarget(reference).Libraries)
                {
                    if (items.TryGetValue(library.Name, out string absolutePath))
                    {
                        func(reference, library, absolutePath);
                    }
                }

                reference.Save();
            }
        }

        /// <summary>
        /// Performs metadata validation
        /// and filling with data common 
        /// to all references.
        /// </summary>
        /// 
        /// <remarks>
        /// See: <seealso cref="AddReference(ProjectReference, ReferenceType, string, Dictionary{string, string})"/>.
        /// </remarks>
        protected virtual Dictionary<string, string> AdaptMetadata(string unevaluatedInclude, Dictionary<string, string> metadata)
        {
            if (!metadata.ContainsKey("Temp"))
            {
                metadata.Add("Temp", unevaluatedInclude);
            }

            return metadata;
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
        /// Returns false for duplicate <paramref name="unevaluatedInclude"/> values.
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
                        reference.MsbProject.AddItem(type.ToString(), unevaluatedInclude, AdaptMetadata(unevaluatedInclude, metadata));
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