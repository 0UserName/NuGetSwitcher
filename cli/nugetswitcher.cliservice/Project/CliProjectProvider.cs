using Microsoft.Build.Evaluation;

using NuGetSwitcher.Interface.Provider.Project;
using NuGetSwitcher.Interface.Reference.Project;

using MsbProject = Microsoft.Build.Evaluation.Project;

namespace NuGetSwitcher.CLIService.Project
{
    public sealed class CliProjectProvider : AbstractProjectProvider<ProjectReference>
    {
        /// <summary>
        /// This class encapsulates a set of related projects, their toolsets, a default set of global properties, 
        /// and the loggers that should be used to build them. A global version of this class acts as the default 
        /// ProjectCollection. Multiple ProjectCollections can exist within an appdomain. However, these must not 
        /// build concurrently.
        /// </summary>
        private readonly
            ProjectCollection _projectCollection = new
            ProjectCollection
            ();

        public CliProjectProvider(string solution) : base(solution)
        { }

        /// <summary>
        /// Loads a project with the specified filename, using the 
        /// collection's global properties and tools version. If a 
        /// matching project is already loaded, it will be returned, otherwise a new project will be loaded.
        /// </summary>
        public override MsbProject GetLoadedProject(string project)
        {
            return new MsbProject(project, default, default, _projectCollection, ProjectLoadSettings.IgnoreMissingImports);
        }
    }
}