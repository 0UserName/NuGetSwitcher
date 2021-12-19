namespace NuGetSwitcher.Interface.Entity.Enum
{
    public enum Mode
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        UN = 0,

        /// <summary>
        /// Help.
        /// </summary>
        /// 
        /// <remarks>
        /// Cli only.
        /// </remarks>
        HL = 1,

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