using Microsoft.Build.Evaluation;

using System;

namespace NuGetSwitcher.Helper.Entity.Error
{
    public class SwitcherException : Exception
    {
        public Project MsbProject
        {
            get;
            private set;
        }

        public SwitcherException(Project project, string message) : base(message)
        {
            MsbProject = project;
        }

        public SwitcherException(Project project, Exception exception) : base(exception.Message, exception)
        {
            MsbProject = project;
        }
    }
}