using NuGetSwitcher.Interface.Logger;
using NuGetSwitcher.Interface.Logger.Enums;

using NuGetSwitcher.Interface.Options;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Options.Abstracts
{
    public abstract class AbstractOptions(string includeProjectFile, string excludeProjectFile, ILogger logger) : IOptions
    {
        /// <inheritdoc/>
        public IReadOnlyDictionary<string, string> GetIncludeProjects()
        {
            FileOption options = new
            FileOption
            (includeProjectFile, excludeProjectFile);

            Dictionary<string, string> projects = new
            Dictionary<string, string>
            (30);

            foreach (string directory in options.Include)
            {
                if (!Directory.Exists(directory))
                {
                    logger.LogMessage($"{ directory } does not exist and will be skipped", Category.W);

                    continue;
                }

                foreach (string file in Directory.GetFiles(directory, "*.*proj", SearchOption.AllDirectories).Where(i => !options.Exclude.Any(d => i.Contains(d))))
                {
                    if (!projects.TryAdd(Path.GetFileNameWithoutExtension(file), file))
                    {
                        logger.LogMessage($"{ file } is a duplicate and will be skipped", Category.W);
                    }
                }
            }

            return projects;
        }
    }
}