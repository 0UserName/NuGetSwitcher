﻿using Microsoft.Build.Evaluation;

using NuGet.Common;
using NuGet.Frameworks;
using NuGet.ProjectModel;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using NuGetSwitcher.Interface.Reference.Project.Contract;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using MsbProject = Microsoft.Build.Evaluation.Project;

namespace NuGetSwitcher.Interface.Reference.Project
{
    public sealed class ProjectReference : IProjectReference
    {
        private readonly
            Dictionary<string, string> _msbProperties = new
            Dictionary<string, string>
            {
                { "MSBuildProjectFullPath" , default },
                { "TargetFrameworkMoniker" , default },
                { "TargetFramework"        , default },
                { "TargetFrameworks"       , default }
            };

        /// <summary>
        /// A portable implementation 
        /// of the .NET FrameworkName 
        /// type with added support for NuGet folder names.
        /// </summary>
        private readonly NuGetFramework _nuGetFramework;

        public MsbProject MsbProject
        {
            get;
            private set;
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
            get => _msbProperties["TargetFrameworkMoniker"] ?? _msbProperties["TargetFramework"];
        }

        /// <summary>
        /// Target Framework Moniker.
        /// </summary>
        /// 
        /// <remarks>
        /// netcoreapp3.1;net5.0
        /// </remarks>
        public string TFMs
        {
            get => _msbProperties["TargetFrameworks"] ?? TFM;
        }

        public ProjectReference(MsbProject project)
        {
            MsbProject = project;

            foreach (ProjectProperty property in MsbProject.Properties)
            {
                if (_msbProperties.ContainsKey(property.Name))
                {
                    _msbProperties[property.Name] = property.EvaluatedValue;
                }
            }

            /*
             * Either an explicitly specified value is
             * used, or the first one from the list of 
             * available ones. 
             */

            _nuGetFramework = NuGetFramework.Parse(TFM ?? TFMs.Split(';')[0]);
        }

        /// <summary>
        /// Returns the <see cref="LockFile"/> object that 
        /// represents the contents of project.assets.json. 
        /// Used to identify project dependencies.
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
        public LockFile GetLockFile()
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

                if (MsbProject.GetItems(nameof(ReferenceType.PackageReference)).Any())
                {
                    throw new SwitcherFileNotFoundException(MsbProject, $"File { path }. Message: Project lock file not found");
                }
            }

            return LockFileUtilities.GetLockFile(path, NullLogger.Instance) ?? new LockFile();
        }

        /// <summary>
        /// Returns the <see cref="LockFileTarget"/> section
        /// for a project TFM from the lock file provided by 
        /// <see cref="LockFile"/>.
        /// </summary>
        /// 
        /// <exception cref="SwitcherFileNotFoundException"/>
        public LockFileTarget GetProjectTarget()
        {
            return GetLockFile().GetTarget(_nuGetFramework, string.Empty) ?? new LockFileTarget()
            {
                Libraries = new List<LockFileTargetLibrary>()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="unevaluatedInclude">
        /// Must contain the assembly 
        /// name or the absolute path 
        /// to the project or Package
        /// Id.
        /// </param>
        public bool IsReferencePresent(string unevaluatedInclude)
        {
            return MsbProject.GetItemsByEvaluatedInclude(unevaluatedInclude).Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="unevaluatedInclude">
        /// Must contain the assembly 
        /// name or the absolute path 
        /// to the project or Package
        /// Id.
        /// </param>
        public bool AddReference(string unevaluatedInclude, ReferenceType type, Dictionary<string, string> metadata)
        {
            bool output = !IsReferencePresent(unevaluatedInclude);

            if (output)
            {
                MsbProject.AddItem(type.ToString(), unevaluatedInclude, metadata);
            }

            return output;
        }

        /// <summary>
        /// Save the project to the file system, 
        /// if dirty. Uses the default encoding.
        /// </summary>
        public void Save()
        {
            MsbProject.Save();
        }
    }
}