using NuGet.ProjectModel;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using System.Collections.Generic;

using MsbProject = Microsoft.Build.Evaluation.Project;

namespace NuGetSwitcher.Interface.Reference.Project.Contract
{
    public interface IProjectReference
    {
        MsbProject MsbProject
        {
            get;
        }

        string UniqueName
        {
            get;
        }

        /// <summary>
        /// Target Framework Moniker.
        /// </summary>
        /// 
        /// <remarks>
        /// .NETFramework,Version=v4.6.1
        /// </remarks>
        string TFM
        {
            get;
        }

        /// <summary>
        /// Target Framework Moniker.
        /// </summary>
        /// 
        /// <remarks>
        /// netcoreapp3.1;net5.0
        /// </remarks>
        string TFMs
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="LockFileTarget"/> section
        /// for a project TFM from the lock file provided by 
        /// <see cref="LockFile"/>.
        /// </summary>
        /// 
        /// <exception cref="SwitcherFileNotFoundException"/>
        LockFileTarget GetProjectTarget();

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
        bool IsReferencePresent(string unevaluatedInclude);

        /// <summary>
        /// 
        /// </summary>
        bool AddReference(string unevaluatedInclude, ReferenceType type, Dictionary<string, string> metadata);

        /// <summary>
        /// Save the project to the file system, 
        /// if dirty. Uses the default encoding.
        /// </summary>
        void Save();
    }
}