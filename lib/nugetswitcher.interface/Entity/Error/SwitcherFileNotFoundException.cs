using Microsoft.Build.Evaluation;

using System;

namespace NuGetSwitcher.Interface.Entity.Error
{
    public sealed class SwitcherFileNotFoundException : SwitcherException
    {
        public SwitcherFileNotFoundException(Project msbProject, string message) : base(msbProject, message)
        { }

        public SwitcherFileNotFoundException(Project msbProject, Exception exception) : base(msbProject, exception)
        { }
    }
}