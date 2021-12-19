using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Provider.Option.Contract;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace NuGetSwitcher.VSIXService.Option
{
    [Guid("00000000-0000-0000-0000-000000000000")]
    public sealed class VsixPackageOption : DialogPage, IOptionProvider
    {
        private readonly IOptionProvider _optionProvider;

        /// <summary>
        /// For internal use only.
        /// </summary>
        [Category("Main"), DisplayName("Include Project File"), Description("Path to a file containing enumerations of directories used to include projects")]
        public string IncludeProjectFile
        {
            get
            {
                return _optionProvider.IncludeProjectFile;
            }
            set
            {
                _optionProvider.IncludeProjectFile = value;
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
                return _optionProvider.ExcludeProjectFile;
            }
            set
            {
                _optionProvider.ExcludeProjectFile = value;
            }
        }

        public VsixPackageOption()
        {
            _optionProvider = (IOptionProvider)Package.GetGlobalService(typeof(IOptionProvider));
        }

        /// <summary>
        /// Returns a dictionary where file names are 
        /// used as keys, and absolute file paths are 
        /// values.
        /// </summary>
        /// 
        /// <exception cref="FileNotFoundException"/>
        public ReadOnlyDictionary<string, string> GetIncludeItems(ReferenceType type)
        {
            return _optionProvider.GetIncludeItems(type);
        }
    }
}