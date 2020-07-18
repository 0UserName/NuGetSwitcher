using Microsoft.Build.Evaluation;

using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Abstract;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Enum;
using NuGetSwitcher.Helper.Entity.Error;

using NuGetSwitcher.Option;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class PackageSwitch : AbstractSwitch
    {
        public PackageSwitch(bool isVSIX, ReferenceType type, IPackageOption packageOption, IProjectHelper projectHelper, IMessageHelper messageHelper) : base(isVSIX, type, packageOption, projectHelper, messageHelper)
        { }

        /// <summary>
        /// Replaces ProjectReference references marked
        /// with the Temp attribute to PackageReference.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        public override void Switch()
        {
            MessageHelper.Clear();

            foreach (ProjectReference reference in ProjectHelper.GetLoadedProject())
            {
                SwitchDependency(reference, ReferenceType.Reference);
                SwitchDependency(reference, ReferenceType.ProjectReference);

                reference.Save();
            }

            CleanSolution();
        }

        /// <summary>
        /// Removes references that have
        /// the Temp attribute and which 
        /// were added from the Dependencies 
        /// and FrameworkAssemblies sections
        /// of the lock file.
        /// </summary>
        public virtual void SwitchDependency(ProjectReference reference, ReferenceType type)
        {
            foreach (ProjectItem item in GetTempItem(reference, type))
            {
                // Implicit.
                if (!item.HasMetadata("Version"))
                {
                    reference.MsbProject.RemoveItem(item);
                }
                // Explicit.
                else
                {
                    item.UnevaluatedInclude = item.GetMetadataValue("Temp");

                    item.RemoveMetadata("Temp");
                    item.RemoveMetadata("Name");

                    item.ItemType = Type.ToString();
                }

                MessageHelper.AddMessage(reference.DteProject.UniqueName, $"Dependency: { Path.GetFileNameWithoutExtension(item.EvaluatedInclude) } has been switched back. Type: { Type }", TaskErrorCategory.Message);
            }
        }

        /// <summary>
        /// Returns references with the
        /// passed type and marked with 
        /// the Temp attribute.
        /// </summary>
        public virtual IReadOnlyList<ProjectItem> GetTempItem(ProjectReference reference, ReferenceType type)
        {
            return reference.MsbProject.GetItems(type.ToString()).Where(i => i.HasMetadata("Temp")).ToImmutableList();
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
        private void CleanSolution()
        {
            if (IsVSIX)
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
}