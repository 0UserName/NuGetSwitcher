using Microsoft.Build.Evaluation;

using System;

namespace NuGetSwitcher.Helper.Entity.Error
{
    public sealed class SwitcherInvalidOperationException : SwitcherException
    {
        public SwitcherInvalidOperationException(Project msbProject, string message) : base(msbProject, message)
        { }

        public SwitcherInvalidOperationException(Project msbProject, Exception exception) : base(msbProject, exception)
        { }
    }
}