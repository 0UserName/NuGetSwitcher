using NuGetSwitcher.Interface.Entity.Enum;

using NuGetSwitcher.Interface.Provider.Message.Contract;

using NuGetSwitcher.Interface.Provider.Option.Contract;
using NuGetSwitcher.Interface.Provider.Option.Entity;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Interface.Provider.Option
{
    public abstract class AbstractOptionProvider : IOptionProvider
    {
        protected IMessageProvider MessageProvider
        {
            get;
            private set;
        }

        public string IncludeProjectFile
        {
            get;
            set;
        }

        public string ExcludeProjectFile
        {
            get;
            set;
        }

        protected AbstractOptionProvider(IMessageProvider messageProvider)
        {
            MessageProvider = messageProvider;
        }

        /// <summary>
        /// Returns true if a directory containing the file
        /// is found. Level at which the file is located is 
        /// not taken into account.
        /// </summary>
        protected bool Contains(IEnumerable<string> excludeDirectories, string absolutePath)
        {
            const int NOT_FOUND = -1;

            return excludeDirectories.Any(d => absolutePath.IndexOf(d) != NOT_FOUND);
        }

        /// <summary>
        /// Returns a dictionary where file names are 
        /// used as keys, and absolute file paths are 
        /// values.
        /// </summary>
        /// 
        /// <exception cref="FileNotFoundException"/>
        public virtual ReadOnlyDictionary<string, string> GetIncludeItems(ReferenceType type)
        {
            FileOption option = new FileOption(IncludeProjectFile, ExcludeProjectFile, "*.*proj");

            Dictionary<string, string> output = new
            Dictionary<string, string>
            (30);

            foreach (string directory in option.Include)
            {
                if (!Directory.Exists(directory))
                {
                    MessageProvider.AddMessage($"Directory: { directory } does not exist and will be skipped", MessageCategory.WG);

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
                        MessageProvider.AddMessage($"Project: { absolutePath } is a duplicate and will be skipped", MessageCategory.WG);
                    }
                }
            }

            return new ReadOnlyDictionary<string, string>(output);
        }
    }
}