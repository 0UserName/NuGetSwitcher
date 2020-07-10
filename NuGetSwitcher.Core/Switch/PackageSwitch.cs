using Microsoft.Build.Evaluation;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Enum;
using NuGetSwitcher.Helper.Entity.Error;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class PackageSwitch : AbstractSwitch, IPackageSwitch
    {
        protected IProjectHelper ProjectHelper
        {
            get;
            set;
        }

        public PackageSwitch(IProjectHelper projectHelper, IMessageHelper messageHelper, bool isVSIX) : base(isVSIX, messageHelper)
        {
            ProjectHelper = projectHelper;
            MessageHelper = messageHelper;
        }

        /// <summary>
        /// Switches references to
        /// projects to references 
        /// to NuGet packages.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        public virtual void SwithProject()
        {
            MessageHelper.Clear();

            foreach (ProjectReference reference in ProjectHelper.GetLoadedProject())
            {
                SwitchSysDependency(reference);
                SwitchPkgDependency(reference);

                reference.Save();
            }

            CleanSolution();
        }

        /// <summary>
        /// Removes references that have the 
        /// _Temp attribute and which have been added 
        /// from the targets section of the lock file.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        public virtual void SwitchPkgDependency(ProjectReference reference)
        {
            IEnumerable<ProjectItem> items = GetTempReference(reference, ReferenceType.ProjectReference);

            foreach (ProjectItem item in items)
            {
                if (!item.HasMetadata("Version"))
                {
                    continue;
                }

                AddReference(reference,
                    item.GetMetadataValue("Include"),
                    item.GetMetadataValue("Version"));
            }

            reference.MsbProject.RemoveItems(items);
        }

        /// <summary>
        /// Removes references that have the _Temp
        /// attribute and which have been added from the 
        /// FrameworkAssemblies section of the lock file.
        /// </summary>
        public virtual void SwitchSysDependency(ProjectReference reference)
        {
            reference.MsbProject.RemoveItems(GetTempReference(reference, ReferenceType.Reference));
        }

        /// <summary>
        /// Returns references that have
        /// the _Temp attribute with the
        /// Temp value.
        /// </summary>
        public virtual IEnumerable<ProjectItem> GetTempReference(ProjectReference reference, ReferenceType type)
        {
            return reference.MsbProject.GetItems(type.ToString()).Where(i => i.HasMetadata("_Temp"));
        }

        /// <summary>
        /// Adds reference to the project. It is assumed
        /// that the original reference has been removed 
        /// earlier.
        /// </summary>
        /// 
        /// <param name="unevaluatedInclude">
        /// PackageId.
        /// </param>
        /// 
        /// <exception cref="SwitcherException"/>
        /// 
        /// <returns>
        /// Returns false for duplicate unevaluatedInclude values.
        /// </returns>
        protected virtual bool AddReference(ProjectReference reference, string unevaluatedInclude, string version)
        {
            return base.AddReference(reference, ReferenceType.PackageReference, unevaluatedInclude,

                new Dictionary<string, string>(1)
                {
                    { "Version", version }
                });
        }

        /// <summary>
        /// Deletes temporary projects
        /// from the solution.
        /// </summary>
        /// 
        /// <remarks>
        /// See: <seealso cref="ProjectReference.IsTemp"/>
        /// </remarks>
        /// 
        /// <remarks>
        /// Uses DTE - only works from Visual Studio.
        /// </remarks>
        protected virtual void CleanSolution()
        {
            try
            {
                foreach (ProjectReference reference in ProjectHelper.GetLoadedProject())
                {
                    if (reference.IsTemp)
                    {
                        DTE.Solution.Remove(reference.DteProject);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageHelper.AddMessage(exception);
            }
            finally
            {
                DTE.Solution.SaveAs(DTE.Solution.FileName);
            }
        }
    }
}