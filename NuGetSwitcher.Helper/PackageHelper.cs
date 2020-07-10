using NuGet.Common;
using NuGet.Frameworks;
using NuGet.ProjectModel;

using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Error;

using System;
using System.Collections.Generic;

namespace NuGetSwitcher.Helper
{
    public class PackageHelper
    {
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
    }
}