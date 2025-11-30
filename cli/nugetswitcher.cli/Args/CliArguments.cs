using NuGetSwitcher.Core.Commands.Enums;

namespace NuGetSwitcher.CLI.Args
{
    internal sealed class CliArguments
    {
        /// <summary>
        /// Absolute path to the solution file.
        /// </summary>
        public string AbsolutePath
        {
            get;
            private set;
        }

        public string IncludeProjectFile
        {
            get;
            private set;
        }

        public string ExcludeProjectFile
        {
            get;
            private set;
        }

        public Mode Mode
        {
            get;
            private set;
        }

        public CliArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--help":
                        Mode = Mode.HL;
                        break;
                    case "--version":
                        Mode = Mode.VR;
                        break;
                    case "--project":
                        Mode = Mode.PR;
                        break;
                    case "--package":
                        Mode = Mode.PK;
                        break;
                    case "-include":
                        IncludeProjectFile = args[i + 1];
                        ++i;
                        break;
                    case "-exclude":
                        ExcludeProjectFile = args[i + 1];
                        ++i;
                        break;
                    default:
                        AbsolutePath = args[i];
                        break;
                }
            }
        }
    }
}