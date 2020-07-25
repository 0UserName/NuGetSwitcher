using Microsoft.Build.Evaluation;

using System;

namespace NuGetSwitcher.Interface.Entity.Error
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public sealed class SwitcherFileNotFoundException : SwitcherException
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        public SwitcherFileNotFoundException(Project msbProject, string message) : base(msbProject, message)
        { }

        public SwitcherFileNotFoundException(Project msbProject, Exception exception) : base(msbProject, exception)
        { }
    }
}