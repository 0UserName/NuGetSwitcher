using NuGetSwitcher.Core.Option;
using NuGetSwitcher.Core.Switch;

using NuGetSwitcher.Menu;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Router
{
    public class CommandRouter : ICommandRouter
    {
        protected readonly IProjectSwtich ProjectSwitch;
        protected readonly IPackageSwitch PackageSwitch;
        protected readonly IPackageOption PackageOption;

        public CommandRouter(IProjectSwtich projectSwitch, IPackageSwitch packageSwitch, IPackageOption packageOption)
        {
            ProjectSwitch = projectSwitch;
            PackageSwitch = packageSwitch;
            PackageOption = packageOption;
        }

        public virtual void Route(string command)
        {
            switch (command)
            {
                case "CommandProject":
                    ProjectSwitch.SwithPackage(GetAllProjects());
                    break;
                case "CommandPackage":
                    PackageSwitch.SwithProject();
                    break;
            }
        }

        /// <summary>
        /// Returns a dictionary where project file names 
        /// are used as keys, and absolute file paths are 
        /// values.
        /// </summary>
        private ReadOnlyDictionary<string, string> GetAllProjects()
        {
            Dictionary<string, string> projects = new
            Dictionary<string, string>(30);

            foreach (string location in PackageOption.GetProjectLocation())
            {
                if (!Directory.Exists(location))
                {
                    continue;
                }

                foreach (string project in Directory.GetDirectories(location).Select(d => Directory.GetFiles(d, $"*.csproj", SearchOption.AllDirectories)).SelectMany(p => p))
                {
                    if (!projects.ContainsKey(
                                     Path.GetFileNameWithoutExtension(project)))
                    {
                        projects.Add(Path.GetFileNameWithoutExtension(project), project);
                    }
                }
            }

            return new ReadOnlyDictionary<string, string>(projects);
        }
    }
}