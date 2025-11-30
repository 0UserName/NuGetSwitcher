using Microsoft.Build.Evaluation;

using System;

namespace NuGetSwitcher.Core.Exceptions
{
    public sealed class SwitcherInvalidOperationException : SwitcherException
    {
        public SwitcherInvalidOperationException(string message) : base(message)
        { }

        public SwitcherInvalidOperationException(string message, Project project) : base(message, project)
        { }

        public SwitcherInvalidOperationException(Exception exception, Project project) : base(exception, project)
        { }
    }
}