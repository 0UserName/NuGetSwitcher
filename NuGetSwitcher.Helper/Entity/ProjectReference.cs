using Microsoft.Build.Evaluation;

using NuGetSwitcher.Helper.Entity.Enum;
using NuGetSwitcher.Helper.Entity.Error;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NuGetSwitcher.Helper.Entity
{
    public sealed class ProjectReference
    {
        public EnvDTE.Project DteProject
        {
            get;
            private set;
        }

        public Project MsbProject
        {
            get;
            private set;
        }

        /// <summary>
        /// Target Framework Moniker.
        /// </summary>
        /// 
        /// <remarks>
        /// .NETFramework,Version=v4.6.1
        /// </remarks>
        public string TFM
        {
            get => _dteProperties["TargetFrameworkMoniker"];
        }

        /// <summary>
        /// Target Framework Moniker [Multiple].
        /// </summary>
        /// 
        /// <remarks>
        /// .NETFramework,Version=v4.6.1
        /// .NETFramework,Version=v4.6.2
        /// .NETFramework,Version=v4.7.2
        /// .NETStandard,Version=v2.0
        /// .NETStandard,Version=v2.1
        /// </remarks>
        public string TFMs
        {
            get => _dteProperties["TargetFrameworkMonikers"];
        }

        /// <summary>
        /// Target Framework Identifier.
        /// </summary>
        public string TFI
        {
            get;
            private set;
        }

        /// <summary>
        /// Target Framework Version.
        /// </summary>
        public string TFV
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the path to the project.assets.json
        /// lock file containing the project dependency
        /// graph.
        /// </summary>
        ///
        /// <exception cref="SwitcherFileNotFoundException"/>
        /// 
        /// <remarks>
        /// project.assets.json lists all the dependencies of the project. It is
        /// created in the /obj folder when using dotnet restore or dotnet build 
        /// as it implicitly calls restore before build, or msbuid.exe /t:restore
        /// with msbuild CLI.
        /// </remarks>
        public string LockFile
        {
            get
            {
                string path = Path.Combine(MsbProject.DirectoryPath, "obj\\project.assets.json");

                if (!File.Exists(path))
                {
                    /*
                     * If there are no NuGet 
                     * dependencies, then do 
                     * not need to throw the 
                     * exception.
                     */

                    if (MsbProject.GetItems(nameof(ReferenceType.PackageReference)).Any())
                    {
                        throw new SwitcherFileNotFoundException(MsbProject, $"File { path }. Message: Project lock file not found");
                    }
                }

                return path;
            }
        }

        /// <summary>
        /// Checks that the project 
        /// is inside the directory 
        /// of the current solution.
        /// </summary>
        /// 
        /// <remarks>
        /// Projects outside the directory are considered temporary.
        /// </remarks>
        public bool IsTemp
        {
            get;
            private set;
        }

        private readonly Dictionary<string, string> _dteProperties = new
                         Dictionary<string, string>
        {
            { "TargetFrameworkMoniker" , "" },
            { "TargetFrameworkMonikers", "" }
        };

        public ProjectReference(EnvDTE.Project dteProject, Project msbProject)
        {
            DteProject = dteProject;
            MsbProject = msbProject;

            Init(dteProject);
        }

        /// <summary>
        /// Retrieves property values defined
        /// by keys in dictionary <see cref="_dteProperties"/>
        /// from <paramref name="project"/> and assigns them
        /// as values of the same keys.
        /// </summary>
        private void Init(EnvDTE.Project project)
        {
            foreach (EnvDTE.Property property in project.Properties)
            {
                if (_dteProperties.ContainsKey(property.Name))
                {
                    _dteProperties[property.Name] = property.Value as string;
                }
            }

            Match match = Regex.Match(TFM, "\\A(?<TFI>[a-zA-Z.]+),.*=v(?<TFV>[0-9.]+)\\z");

            TFI = match.Groups["TFI"].Value;
            TFV = match.Groups["TFV"].Value;

            IsTemp = !Directory.GetFiles(Path.GetDirectoryName(DteProject.DTE.Solution.FullName), Path.GetFileName(MsbProject.FullPath), SearchOption.AllDirectories).Any();
        }

        public void Save()
        {
            MsbProject.Save();
        }
    }
}