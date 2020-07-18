using NuGet.ProjectModel;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity;
using NuGetSwitcher.Helper.Entity.Enum;
using NuGetSwitcher.Helper.Entity.Error;

using NuGetSwitcher.Option;

using System;
using System.IO;

namespace NuGetSwitcher.Core.Switch
{
    public class LibrarySwitch : ProjectSwitch
    {
        public LibrarySwitch(bool isVSIX, ReferenceType type, IPackageOption packageOption, IProjectHelper projectHelper, IMessageHelper messageHelper) : base(isVSIX, type, packageOption, projectHelper, messageHelper)
        { }

        /// <summary>
        /// Replaces PackageReference references marked with the 
        /// Temp attribute to Reference. Transitive dependencies 
        /// will be included.
        /// </summary>
        ///
        /// <exception cref="SwitcherFileNotFoundException"/>
        /// 
        /// <exception cref="FileNotFoundException"/>
        /// 
        /// <exception cref="ArgumentException">
        public override void Switch()
        {
            void Executor(ProjectReference reference, LockFileTargetLibrary library, string absolutePath)
            {
                SwitchSysDependency(reference, library);
                SwitchPkgDependency(reference, library, absolutePath);
            }

            IterateAndExecute(ProjectHelper.GetLoadedProject(), Executor);
        }
    }
}