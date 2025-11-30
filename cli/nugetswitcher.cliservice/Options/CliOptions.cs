using NuGetSwitcher.Core.Options.Abstracts;

using NuGetSwitcher.Interface.Logger;

namespace NuGetSwitcher.CLIService.Options
{
    public sealed class CliOptions(string includeProjectFile, string excludeProjectFile, ILogger logger) : AbstractOptions(includeProjectFile, excludeProjectFile, logger)
    { }
}