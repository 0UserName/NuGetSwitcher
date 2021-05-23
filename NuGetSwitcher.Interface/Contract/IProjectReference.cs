using Microsoft.Build.Evaluation;

using NuGetSwitcher.Interface.Entity.Error;

namespace NuGetSwitcher.Interface.Contract
{
    public interface IProjectReference
    {
        Project MsbProject
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
        /// Target Framework Identifier.
        /// </summary>
        string TFI
        {
            get;
        }

        /// <summary>
        /// Target Framework Version.
        /// </summary>
        string TFV
        {
            get;
        }

        bool IsTemp
        {
            get;
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
        string GetLockFile();

        /// <summary>
        /// 
        /// </summary>
        void Save();
    }
}