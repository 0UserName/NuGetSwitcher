using Microsoft.Build.Evaluation;

using System;

namespace NuGetSwitcher.Core.Exceptions
{
    public sealed class SwitcherFileNotFoundException : SwitcherException
    {
        public SwitcherFileNotFoundException(string message) : base(message)
        { }

        public SwitcherFileNotFoundException(string message, Project project) : base(message, project)
        { }

        public SwitcherFileNotFoundException(Exception exception, Project project) : base(exception, project)
        { }
    }
}