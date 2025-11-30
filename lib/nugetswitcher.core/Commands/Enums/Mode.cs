namespace NuGetSwitcher.Core.Commands.Enums
{
    public enum Mode
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        UN = 0,

        /// <summary>
        /// Show help.
        /// </summary>
        /// 
        /// <remarks>
        /// Cli only.
        /// </remarks>
        HL = 1,

        /// <summary>
        /// Show version.
        /// </summary>
        /// 
        /// <remarks>
        /// Cli only.
        /// </remarks>
        VR = 2,

        /// <summary>
        /// Switch packages.
        /// </summary>
        PK = 3,

        /// <summary>
        /// Switch projects.
        /// </summary>
        PR = 4
    }
}