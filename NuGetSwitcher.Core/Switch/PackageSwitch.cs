using Microsoft.Build.Evaluation;

using Microsoft.VisualStudio.Shell;

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
        /// Removes references that have 
        /// the Temp attribute and which 
        /// were added from the 
        /// FrameworkAssemblies section 
        /// of the lock file.
        /// </summary>
        public virtual void SwitchSysDependency(ProjectReference reference)
        {
            reference.MsbProject.RemoveItems(GetTempReference(reference, ReferenceType.Reference));
        }

        /// <summary>
        /// Removes references that have the Temp attribute
        /// and which were added from the target section of
        /// the lock file.
        /// </summary>
        public virtual void SwitchPkgDependency(ProjectReference reference)
        {
            IList<ProjectItem> items = GetTempReference(reference, ReferenceType.ProjectReference).ToList();

            for (int i = 0; i < items.Count; i++)
            {
                ProjectItem item = items[i];

                if (!item.HasMetadata("Version"))
                {
                    reference.MsbProject.RemoveItem(item);
                }
                else
                {
                    item.UnevaluatedInclude = item.GetMetadataValue("Include");

                    item.RemoveMetadata("Temp");
                    item.RemoveMetadata("Name");
                    item.RemoveMetadata("Include");

                    item.ItemType = nameof(ReferenceType.PackageReference);
                }

                MessageHelper.AddMessage(reference.DteProject.UniqueName, $"Dependency: { item.EvaluatedInclude } has been switched back. Type: { ReferenceType.PackageReference }", TaskErrorCategory.Message);
            }
        }

        /// <summary>
        /// Returns references with the
        /// passed type and marked with 
        /// the Temp attribute.
        /// </summary>
        public virtual IEnumerable<ProjectItem> GetTempReference(ProjectReference reference, ReferenceType type)
        {
            return reference.MsbProject.GetItems(type.ToString()).Where(i => i.HasMetadata("Temp"));
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