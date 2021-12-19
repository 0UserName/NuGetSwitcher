using NuGet.ProjectModel;

using NuGetSwitcher.Interface.Entity.Error;

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
        /// Checks that the project 
        /// is inside the directory 
        /// of the current solution.
        /// </summary>
        /// 
        /// <remarks>
        /// Projects outside the directory are considered temporary.
        /// </remarks>
        bool IsTemp
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
        void Save();
    }
}