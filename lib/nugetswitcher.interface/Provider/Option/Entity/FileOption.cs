using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Interface.Provider.Option.Entity
{
    public struct FileOption
    {
        public IEnumerable<string> Include
        {
            get;
            private set;
        }

        public IEnumerable<string> Exclude
        {
            get;
            private set;
        }

        public string Pattern
        {
            get;
            private set;
        }

        public FileOption(string includeFile, string excludeFile, string pattern) : this()
        {
            Include = Enumerable.Empty<string>();
            Exclude = Enumerable.Empty<string>();

            if (File.Exists(includeFile))
            {
                Include = ReadConfig(includeFile);
            }
            else
            {
                throw new FileNotFoundException($"Configuration file: { includeFile } not specified or not found");
            }

            if (File.Exists(excludeFile))
            {
                Exclude = ReadConfig(excludeFile);
            }

            Pattern = pattern;
        }

        private IEnumerable<string> ReadConfig(string path)
        {
            return File.ReadAllLines(path).Where(l => !l.StartsWith("#")) ?? Enumerable.Empty<string>();
        }
    }
}