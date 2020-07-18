using Microsoft.VisualStudio.Shell;

using NuGetSwitcher.Helper;
using NuGetSwitcher.Helper.Entity.Enum;

using NuGetSwitcher.Option.Entity;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace NuGetSwitcher.Option
{
    [Guid("00000000-0000-0000-0000-000000000000")]
    public class PackageOption : DialogPage, IPackageOption
    {
        /// <summary>
        /// For internal use only.
        /// </summary>
        [Category("Main"), DisplayName("Include Project File"), Description("Path to a file containing enumerations of directories used to include projects")]
        public string IncludeProjectFile
        {
            get;
            set;
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [Category("Main"), DisplayName("Include Library File"), Description("Path to a file containing enumerations of directories used to include libraries")]
        public string IncludeLibraryFile
        {
            get;
            set;
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [Category("Main"), DisplayName("Exclude File"), Description("Path to a file containing enumerations of directories used to exclude projects and libraries")]
        public string ExcludeProjectFile
        {
            get;
            set;
        }

        protected IMessageHelper MessageHelper
        {
            get;
            set;
        }

        public void Init(IMessageHelper messageHelper)
        {
            MessageHelper = messageHelper;
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
        public virtual ReadOnlyDictionary<string, string> GetIncludeItems(ReferenceType type)
        {
            FileOption option = GetFileOption(type);

            Dictionary<string, string> output = new
            Dictionary<string, string>(30);

            foreach (string directory in option.Include)
            {
                if (!Directory.Exists(directory))
                {
                    MessageHelper.AddMessage($"Directory: { directory } does not exist and will be skipped", TaskErrorCategory.Warning);

                    continue;
                }

                foreach (string absolutePath in Directory.GetFiles(directory, option.Pattern, SearchOption.AllDirectories).Where(i => !Contains(option.Exclude, i)))
                {
                    string key = Path.GetFileNameWithoutExtension(absolutePath);

                    if (!output.ContainsKey(key))
                    {
                        output.Add(key, absolutePath);
                    }
                    else
                    {
                        MessageHelper.AddMessage($"Project: { absolutePath } is a duplicate and will be skipped", TaskErrorCategory.Warning);
                    }
                }
            }

            return new ReadOnlyDictionary<string, string>(output);
        }

        /// <summary>
        /// Returns the structure containing an array of
        /// directories to search for and directories to
        /// exclude from searches.
        /// </summary>
        /// 
        /// <exception cref="FileNotFoundException"/>
        /// 
        /// <exception cref="ArgumentException">
        protected virtual FileOption GetFileOption(ReferenceType type)
        {
            FileOption option;

            switch (type)
            {
                case ReferenceType.ProjectReference:
                    option = new FileOption(IncludeProjectFile, ExcludeProjectFile, "*.csproj");
                    break;
                case ReferenceType.Reference:
                    option = new FileOption(IncludeLibraryFile, ExcludeProjectFile, "*.dll");
                    break;
                default: throw new ArgumentException($"Reference type not supported: { type }");
            }

            return option;
        }

        /// <summary>
        /// Returns true if a directory containing the file
        /// is found. Level at which the file is located is 
        /// not taken into account.
        /// </summary>
        protected virtual bool Contains(string[] excludeDirectories, string absolutePath)
        {
            const int NOT_FOUND = -1;

            return excludeDirectories.Any(d => absolutePath.IndexOf(d) != NOT_FOUND);
        }
    }
}