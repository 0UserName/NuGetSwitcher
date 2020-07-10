using Microsoft.VisualStudio.Shell;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace NuGetSwitcher.Core.Option
{
    [Guid("00000000-0000-0000-0000-000000000000")]
    public sealed class PackageOption : DialogPage, IPackageOption
    {
        [Category("Main"), DisplayName("Configuration File"), Description("Path to the file containing the enumeration path to search for projects")]
        public string ConfigurationFile
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a set of user-defined projects 
        /// location used when switching reference.
        /// </summary>
        /// 
        /// <exception cref="FileNotFoundException"/>
        public HashSet<string> GetProjectLocation()
        {
            if (!File.Exists(ConfigurationFile))
            {
                throw new FileNotFoundException("Configuration file not specified or not found");
            }

            return new HashSet<string>(File.ReadAllLines(ConfigurationFile));
        }
    }
}