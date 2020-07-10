using Microsoft.Build.Evaluation;

using NuGet.ProjectModel;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Enum;
using NuGetSwitcher.Helper.Entity.Error;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class ProjectSwitch : AbstractSwitch, IProjectSwtich
    {
        protected IProjectHelper ProjectHelper
        {
            get;
            set;
        }

        public ProjectSwitch(IProjectHelper projectHelper, IMessageHelper messageHelper, bool isVSIX) : base(isVSIX, messageHelper)
        {
            ProjectHelper = projectHelper;
            MessageHelper = messageHelper;
        }

        /// <summary>
        /// Switches references to NuGet packages to
        /// references to projects, given implicit /
        /// transitive dependencies.
        /// </summary>
        /// 
        /// <exception cref="SwitcherFileNotFoundException"/>
        public virtual void SwithPackage(ReadOnlyDictionary<string, string> projectPaths)
        {
            MessageHelper.Clear();

            IEnumerable<ProjectReference> references = ProjectHelper.GetLoadedProject();

            HashSet<string> toInclude = new
            HashSet<string>();

            foreach (ProjectReference reference in references)
            {
                foreach (LockFileTargetLibrary library in PackageHelper.GetProjectTarget(reference).Libraries)
                {
                    if (projectPaths.TryGetValue(library.Name, out string absolutePath))
                    {
                        // Reference will be changed.
                        SwitchSysDependency(reference, library);
                        SwitchPkgDependency(reference, library, absolutePath);

                        toInclude.Add(absolutePath);
                    }
                }

                reference.Save();
            }

            IncludeProject(toInclude.Except(references.Select(r => r.MsbProject.FullPath)));
        }

        /// <summary>
        /// Includes references to the GAC assemblies
        /// listed in the FrameworkAssemblies section
        /// of the lock file.
        /// </summary>
        public virtual void SwitchSysDependency(ProjectReference reference, LockFileTargetLibrary library)
        {
            Dictionary<string, string> metadata = new
            Dictionary<string, string>(1)
            {
                { "_Temp", "Temp" }
            };

            foreach (string assembly in library.FrameworkAssemblies)
            {
                AddReference(reference, ReferenceType.Reference, assembly, metadata);
            }
        }

        /// <summary>
        /// Includes implicit, explicit project references
        /// listed in the targets section of the lock file.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        /// 
        /// <remarks>
        /// Implicit dependencies mean transitive.
        /// </remarks>
        public virtual void SwitchPkgDependency(ProjectReference reference, LockFileTargetLibrary library, string absolutePath)
        {
            Dictionary<string, string> metadata = new
            Dictionary<string, string>(4)
            {
                { "_Temp", "Temp" }
            };

            /*
             * References can be represented by several values in
             * an ItemGroup, for example, when included using the 
             * Condition attribute.
             */

            ICollection<ProjectItem> items = reference.MsbProject.GetItemsByEvaluatedInclude(library.Name);

            if (!items.Any())
            {
                // As implicit.
                metadata.Add("Name", Path.GetFileNameWithoutExtension(absolutePath));
            }
            else
            {
                // As explicit.
                ProjectItem item = items.First();

                metadata.Add("Name"   , item.EvaluatedInclude);
                metadata.Add("Include", item.EvaluatedInclude);
                metadata.Add("Version", item.
                    GetMetadataValue("Version"));

                reference.MsbProject.RemoveItems(items);
            }

            AddReference(reference, ReferenceType.ProjectReference, absolutePath, metadata);
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
        /// to the project.
        /// </param>
        /// 
        /// <returns>
        /// Returns false for duplicate unevaluatedInclude values.
        /// </returns>
        protected override bool AddReference(ProjectReference reference, ReferenceType type, string unevaluatedInclude, Dictionary<string, string> metadata)
        {
            return base.AddReference(reference, type, unevaluatedInclude, metadata);
        }

        /// <summary>
        /// Includes a set of passed 
        /// projects in the solution.
        /// </summary>
        /// 
        /// <param name="projects">
        /// Absolute paths to project
        /// files without duplicates.
        /// </param>
        /// 
        /// <remarks>
        /// Uses DTE - only works from Visual Studio.
        /// </remarks>
        public virtual void IncludeProject(IEnumerable<string> projects)
        {
            /*
             * I don’t know how, but the AddFromFile method depends
             * on the GPC, therefore, to avoid errors, all projects 
             * are forcibly unloaded.
             */

            ProjectHelper.UnloadProject();

            try
            {
                foreach (string project in projects)
                {
                    DTE.Solution.AddFromFile(project, false);
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