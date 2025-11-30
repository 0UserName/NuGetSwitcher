using Microsoft.Build.Evaluation;

using System;

namespace NuGetSwitcher.Core.Exceptions
{
    public class SwitcherException(string message) : Exception(message)
    {
        public Project Project
        {
            get;
            private set;
        }

        public SwitcherException(string message, Project project) : this(message)
        {
            Project = project;
        }

        public SwitcherException(Exception exception, Project project) : this(exception.ToString(), project)
        { }
    }
}