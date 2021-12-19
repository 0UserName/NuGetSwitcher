using Microsoft.Build.Evaluation;

using NuGetSwitcher.Interface.Provider.Project;
using NuGetSwitcher.Interface.Reference.Project;

using MsbProject = Microsoft.Build.Evaluation.Project;

namespace NuGetSwitcher.VSIXService.Project
{
    public sealed class VsixProjectProvider : AbstractProjectProvider<ProjectReference>
    {
        public VsixProjectProvider(string solution) : base(solution)
        { }

        /// <summary>
        /// Loads a project with the specified filename, using the 
        /// collection's global properties and tools version. If a 
        /// matching project is already loaded, it will be returned,otherwise a new project will be loaded.
        /// </summary>
        public override MsbProject GetLoadedProject(string project)
        {
            return ProjectCollection.GlobalProjectCollection.LoadProject(project);
        }
    }
}