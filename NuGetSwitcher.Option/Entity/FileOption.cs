using System.IO;

namespace NuGetSwitcher.Option.Entity
{
    public struct FileOption
    {
        public string[] Include
        {
            get;
            set;
        }

        public string[] Exclude
        {
            get;
            set;
        }

        public string Pattern
        {
            get;
            set;
        }

        public FileOption(string includeFile, string excludeFile, string pattern)
        {
            if (!File.Exists(includeFile))
            {
                throw new FileNotFoundException($"Configuration file: { includeFile } not specified or not found");
            }
            else
            {
                Include = File.ReadAllLines(includeFile);
            }

            if (!File.Exists(excludeFile))
            {
                Exclude = new string[0];
            }
            else
            {
                Exclude = File.ReadAllLines(excludeFile);
            }

            Pattern = pattern;
        }
    }
}