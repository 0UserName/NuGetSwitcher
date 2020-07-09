using Microsoft.Build.Evaluation;

using Microsoft.VisualStudio.Shell;

using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.ProjectModel;

using NuGetSwitcher.Core.Enum;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Error;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Switch
{
    public class ProjectSwitch : IProjectSwtich
    {
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

        public ProjectSwitch(IProjectHelper projectHelper, IMessageHelper messageHelper)
        {
            ProjectHelper = projectHelper;
            MessageHelper = messageHelper;
        }

        /// <summary>
        /// Returns the <see cref="LockFile"/> object that 
        /// represents the contents of project.assets.json. 
        /// Used to identify project dependencies.
        /// </summary>
        /// 
        /// <exception cref="SwitcherFileNotFoundException"/>
        public static LockFile GetLockFile(ProjectReference reference)
        {
            return LockFileUtilities.GetLockFile(reference.LockFile, NullLogger.Instance) ?? new LockFile();
        }

        /// <summary>
        /// Returns the <see cref="LockFileTarget"/> section
        /// for a project TFM from the lock file provided by 
        /// <see cref="LockFile"/>.
        /// </summary>
        /// 
        /// <exception cref="SwitcherFileNotFoundException"/>
        public static LockFileTarget GetProjectTarget(ProjectReference reference)
        {
            return GetLockFile(reference).GetTarget(new NuGetFramework(reference.TFI, new Version(reference.TFV), string.Empty), null) ??

                new LockFileTarget()
                {
                    Libraries = new List<LockFileTargetLibrary>()
                };
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
                foreach (LockFileTargetLibrary library in GetProjectTarget(reference).Libraries)
                {
                    if (projectPaths.TryGetValue(library.Name, out string absolutePath))
                    {
                        SwitchSysDependency(reference, library);
                        SwitchPkgDependency(reference, library, ref toInclude, absolutePath);
                        SwitchPkgDependency(reference, library, ref toInclude, projectPaths);
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
            Dictionary<string, string>(4)
            {
                { "Type", "Temp" }
            };

            foreach (string assembly in library.FrameworkAssemblies)
            {
                AddReference(ref reference, ReferenceType.Reference, assembly, metadata);
            }
        }

        /// <summary>
        /// Includes references to the project. If <paramref name="library"/>
        /// is null, then dependency is considered as implicit, otherwise as 
        /// explicit.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        /// 
        /// <remarks>
        /// Implicit dependencies mean transitive.
        /// </remarks>
        public virtual void SwitchPkgDependency(ProjectReference reference, LockFileTargetLibrary library, ref HashSet<string> toInclude, string absolutePath)
        {
            Dictionary<string, string> metadata = new
            Dictionary<string, string>(4)
            {
                { "Type", "Temp" }
            };

            ProjectItem item;

            /*
             * References can be represented by several values in
             * an ItemGroup, for example, when included using the 
             * Condition attribute.
             */

            ICollection<ProjectItem> items = reference.MsbProject.GetItemsByEvaluatedInclude(library?.Name ?? string.Empty);

            if (!items.Any())
            {
                if (library != default)
                {
                    return;
                }

                metadata.Add("Name", Path.GetFileNameWithoutExtension(absolutePath));
            }
            else
            {
                item = items.First();

                metadata.Add("Name"   , item.EvaluatedInclude);
                metadata.Add("Include", item.EvaluatedInclude);
                metadata.Add("Version", item.
                    GetMetadataValue("Version"));

                reference.MsbProject.RemoveItems(items);
            }

            AddReference(ref reference, ReferenceType.ProjectReference, absolutePath, metadata);

            toInclude.Add(absolutePath);
        }

        /// <summary>
        /// Includes references presented as
        /// implicit project dependencies.
        /// </summary>
        /// 
        /// <exception cref="SwitcherException"/>
        /// 
        /// <remarks>
        /// Implicit dependencies mean transitive.
        /// </remarks>
        public virtual void SwitchPkgDependency(ProjectReference reference, LockFileTargetLibrary library, ref HashSet<string> toInclude, ReadOnlyDictionary<string, string> projectPaths)
        {
            foreach (PackageDependency dependency in library.Dependencies)
            {
                if (projectPaths.TryGetValue(dependency.Id, out string absolutePath))
                {
                    SwitchPkgDependency(reference, null, ref toInclude, absolutePath);
                }
            }
        }

        /// <summary>
        /// Adds reference to the project. It is assumed
        /// that the original reference has been removed 
        /// earlier.
        /// </summary>
        /// 
        /// <param name="type">
        /// All except <see cref="ReferenceType.PackageReference"/>
        /// </param>
        /// 
        /// <param name="unevaluatedInclude">
        /// Must contain the assembly 
        /// name or the absolute path 
        /// to the project.
        /// </param>
        /// 
        /// <remarks>
        /// https://github.com/Microsoft/msbuild/issues/3923.
        /// </remarks>
        protected virtual void AddReference(ref ProjectReference reference, ReferenceType type, string unevaluatedInclude, Dictionary<string, string> metadata)
        {
            if (reference.MsbProject.GetItemsByEvaluatedInclude(unevaluatedInclude).Any())
            {
                return;
            }

            switch (type)
            {
                case ReferenceType.ProjectReference:
                    ;
                    break;
                case ReferenceType.Reference:
                    ;
                    break;
                default:
                    throw new SwitcherException(reference.MsbProject, $"Reference type not supported: { type }");
            }

            reference.MsbProject.AddItem(type.ToString(), unevaluatedInclude, metadata);

            MessageHelper.AddMessage(reference.MsbProject.FullPath, $"Dependency: { Path.GetFileName(unevaluatedInclude) } has been added. Type: { type }", TaskErrorCategory.Message);
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
        public virtual void IncludeProject(IEnumerable<string> projects)
        {
            EnvDTE.DTE dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            try
            {
                foreach (string project in projects)
                {
                    dte.Solution.AddFromFile(project, false);
                }

                dte.Solution.SaveAs(dte.Solution.FileName);
            }
            catch (Exception exception)
            {
                MessageHelper.AddMessage(exception);
            }
        }
    }
}