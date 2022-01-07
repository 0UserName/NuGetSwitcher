using CliWrap.Builders;

using Microsoft.Build.Evaluation;

using NuGetSwitcher.Core.Abstract;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using NuGetSwitcher.Interface.Provider.Message.Contract;
using NuGetSwitcher.Interface.Provider.Option.Contract;
using NuGetSwitcher.Interface.Provider.Project.Contract;

using NuGetSwitcher.Interface.Reference.Project.Contract;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class PackageSwitch : AbstractSwitch
    {
        public PackageSwitch(IOptionProvider optionProvider, IProjectProvider<IProjectReference> projectHelper, IMessageProvider messageHelper) : base(ReferenceType.PackageReference, optionProvider, projectHelper, messageHelper)
        { }

        /// <summary>
        /// Replaces ProjectReference references marked
        /// with the Temp attribute to PackageReference.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        public override IEnumerable<string> Switch()
        {
            HashSet<string> output = new
            HashSet<string>
            ();

            MessageProvider.Clear();

            foreach (IProjectReference reference in ProjectProvider.GetLoadedProject())
            {
                output.UnionWith(SwitchDependency(reference, ReferenceType.Reference));
                output.UnionWith(SwitchDependency(reference, ReferenceType.ProjectReference));

                reference.Save();
            }

            RemoveFromSolution(ProjectProvider.Solution, output);

            return output;
        }

        /// <summary>
        /// Removes references that have
        /// the Temp attribute and which 
        /// were added from the Dependencies 
        /// and FrameworkAssemblies sections
        /// of the lock file.
        /// </summary>
        public virtual IEnumerable<string> SwitchDependency(IProjectReference reference, ReferenceType type)
        {
            HashSet<string> output = new
            HashSet<string>
            ();

            foreach (ProjectItem item in GetTempItem(reference, type))
            {
                if (type == ReferenceType.ProjectReference)
                {
                    output.Add(item.EvaluatedInclude);
                }

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

            return output;
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

        /// <summary>
        /// Removes projects from the solution file.
        /// </summary>
        protected virtual void RemoveFromSolution(string solution, IEnumerable<string> projects)
        {
            base.SlnAction(solution, projects, new ArgumentsBuilder().Add("remove").Add(projects));
        }
    }
}