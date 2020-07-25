using Microsoft.Build.Evaluation;

namespace NuGetSwitcher.Interface.Entity
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
        string TFMs
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

        string GetLockFile();

        void Save();
    }
}