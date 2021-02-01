using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity.Enum;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace NuGetSwitcher.VSIXService.Option
{
    [Guid("00000000-0000-0000-0000-000000000000")]
    public class VsixPackageOption : DialogPage, IOptionProvider
    {
        /// <summary>
        /// For internal use only.
        /// </summary>
        [Category("Main"), DisplayName("Include Project File"), Description("Path to a file containing enumerations of directories used to include projects")]
        public string IncludeProjectFile
        {
            get
            {
                return OptionProvider.IncludeProjectFile;
            }
            set
            {
                if (OptionProvider != default)
                {
                    OptionProvider.IncludeProjectFile = value;
                }
            }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [Category("Main"), DisplayName("Include Library File"), Description("Path to a file containing enumerations of directories used to include libraries")]
        public string IncludeLibraryFile
        {
            get
            {
                return OptionProvider.IncludeLibraryFile;
            }
            set
            {
                if (OptionProvider != default)
                {
                    OptionProvider.IncludeLibraryFile = value;
                }
            }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [Category("Main"), DisplayName("General Exclude File")]
        public string ExcludeProjectFile
        {
            get
            {
                return OptionProvider.ExcludeProjectFile;
            }
            set
            {
                if (OptionProvider != default)
                {
                    OptionProvider.ExcludeProjectFile = value;
                }
            }
        }

        protected IOptionProvider OptionProvider
        {
            get;
            set;
        }

        public VsixPackageOption Init(IOptionProvider optionProvider)
        {
            OptionProvider = optionProvider;

            return this;
        }

        /// <summary>
        /// Returns a dictionary where file names are 
        /// used as keys, and absolute file paths are 
        /// values.
        /// </summary>
        /// 
        /// <exception cref="FileNotFoundException"/>
        /// 
        /// <exception cref="ArgumentException">
        public ReadOnlyDictionary<string, string> GetIncludeItems(ReferenceType type)
        {
            return OptionProvider.GetIncludeItems(type);
        }
    }
}