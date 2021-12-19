using NuGetSwitcher.Interface.Entity.Enum;

namespace NuGetSwitcher.CLI.Args
{
    internal class CliArguments
    {
        public string Solution
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
            Solution = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
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
                }
            }
        }

        public static implicit operator CliArguments(string[] args)
        {
            return new CliArguments(args);
        }
    }
}