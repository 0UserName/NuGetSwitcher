using NuGetSwitcher.Core.Exceptions;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Options
{
    public sealed class FileOption
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

        private static IEnumerable<string> ReadConfig(string path)
        {
            return File.ReadAllLines(path).Where(l => !l.StartsWith('#')) ?? Enumerable.Empty<string>();
        }

        public FileOption(string includeFile, string excludeFile)
        {
            Include = Enumerable.Empty<string>();
            Exclude = Enumerable.Empty<string>();

            if (File.Exists(includeFile))
            {
                Include = ReadConfig(includeFile);
            }
            else
            {
                throw new SwitcherFileNotFoundException($"{ includeFile } not found");
            }

            if (File.Exists(excludeFile))
            {
                Exclude = ReadConfig(excludeFile);
            }
        }
    }
}