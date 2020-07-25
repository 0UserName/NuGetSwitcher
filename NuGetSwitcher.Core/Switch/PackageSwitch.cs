using Microsoft.Build.Evaluation;

using NuGetSwitcher.Core.Abstract;

using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity;
using NuGetSwitcher.Interface.Entity.Enum;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class PackageSwitch : AbstractSwitch
    {
        public PackageSwitch(ReferenceType type, IOptionProvider optionProvider, IProjectProvider projectHelper, IMessageProvider messageHelper) : base(type, optionProvider, projectHelper, messageHelper)
        { }

        /// <summary>
        /// Replaces ProjectReference references marked
        /// with the Temp attribute to PackageReference.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        public override IEnumerable<string> Switch()
        {
            MessageProvider.Clear();

            foreach (IProjectReference reference in ProjectProvider.GetLoadedProject())
            {
                SwitchDependency(reference, ReferenceType.Reference);
                SwitchDependency(reference, ReferenceType.ProjectReference);

                reference.Save();
            }

            return default;
        }

        /// <summary>
        /// Removes references that have
        /// the Temp attribute and which 
        /// were added from the Dependencies 
        /// and FrameworkAssemblies sections
        /// of the lock file.
        /// </summary>
        public virtual void SwitchDependency(IProjectReference reference, ReferenceType type)
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

                MessageProvider.AddMessage(reference.UniqueName, $"Dependency: { Path.GetFileNameWithoutExtension(item.EvaluatedInclude) } has been switched back. Type: { Type }", MessageCategory.ME);
            }
        }

        /// <summary>
        /// Returns references with the
        /// passed type and marked with 
        /// the Temp attribute.
        /// </summary>
        protected virtual IReadOnlyList<ProjectItem> GetTempItem(IProjectReference reference, ReferenceType type)
        {
            return reference.MsbProject.GetItems(type.ToString()).Where(i => i.HasMetadata("Temp")).ToImmutableList();
        }
    }
}