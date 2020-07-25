using Microsoft.Build.Evaluation;

using System;

namespace NuGetSwitcher.Interface.Entity.Error
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class SwitcherException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
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