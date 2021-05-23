using Microsoft.Build.Evaluation;

using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using MsbProject = Microsoft.Build.Evaluation.Project;

namespace NuGetSwitcher.Interface.Entity
{
    public class ProjectReference : IProjectReference
    {
        public MsbProject MsbProject
        {
            get;
            protected set;
        }

        public string UniqueName
        {
            get => _msbProperties["MSBuildProjectFullPath"];
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
            get => _msbProperties["TargetFrameworkMoniker"];
        }

        /// <summary>
        /// Target Framework Identifier.
        /// </summary>
        public string TFI
        {
            get;
            protected set;
        }

        /// <summary>
        /// Target Framework Version.
        /// </summary>
        public string TFV
        {
            get;
            protected set;
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
            protected set;
        }

        protected readonly
            Dictionary<string, string> _msbProperties = new
            Dictionary<string, string>
            {
                { "MSBuildProjectFullPath" , "" },
                { "TargetFrameworkMoniker" , "" }
            };

        public ProjectReference(string solutionFile, MsbProject project)
        {
            MsbProject = project;

            foreach (ProjectProperty property in MsbProject.Properties)
            {
                if (_msbProperties.ContainsKey(property.Name))
                {
                    _msbProperties[property.Name] = property.EvaluatedValue;
                }
            }

            Match match = Regex.Match(TFM, "\\A(?<TFI>[a-zA-Z.]+),.*=v(?<TFV>[0-9.]+)\\z"); // Ex: .NETFramework,Version=v4.7.2

            TFI = match.Groups["TFI"].Value;
            TFV = match.Groups["TFV"].Value;

            IsTemp = !Directory.GetFiles(Path.GetDirectoryName(solutionFile), Path.GetFileName(MsbProject.FullPath), SearchOption.AllDirectories).Any();
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
        public virtual string GetLockFile()
        {
            string path = Path.Combine(MsbProject.DirectoryPath, "obj", "project.assets.json");

            if (!File.Exists(path))
            {
                /*
                 * If there are no NuGet 
                 * dependencies, then do 
                 * not need to throw the 
                 * exception.
                 */

#pragma warning disable S1066 // Collapsible "if" statements should be merged
                if (MsbProject.GetItems(nameof(ReferenceType.PackageReference)).Any())
#pragma warning restore S1066 // Collapsible "if" statements should be merged
                {
                    throw new SwitcherFileNotFoundException(MsbProject, $"File { path }. Message: Project lock file not found");
                }
            }

            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Save()
        {
            MsbProject.Save();
        }
    }
}