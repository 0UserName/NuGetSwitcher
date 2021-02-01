using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;

using NuGet.Common;
using NuGet.Frameworks;
using NuGet.ProjectModel;

using NuGetSwitcher.Interface.Contract;
using NuGetSwitcher.Interface.Entity;
using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace NuGetSwitcher.Core.Abstract
{
    public abstract class AbstractSwitch
    {
        protected ReferenceType Type
        {
            get;
            private set;
        }

        protected IOptionProvider OptionProvider
        {
            get;
            set;
        }

        protected IProjectProvider ProjectProvider
        {
            get;
            set;
        }

        protected IMessageProvider MessageProvider
        {
            get;
            set;
        }

        protected AbstractSwitch(ReferenceType type, IOptionProvider optionProvider, IProjectProvider projectProvider, IMessageProvider messageProvider)
        {
            Type = type;

            OptionProvider  = optionProvider;
            ProjectProvider = projectProvider;
            MessageProvider = messageProvider;
        }

        /// <summary>
        /// Returns the <see cref="LockFile"/> object that 
        /// represents the contents of project.assets.json. 
        /// Used to identify project dependencies.
        /// </summary>
        /// 
        /// <exception cref="SwitcherFileNotFoundException"/>
        protected virtual LockFile GetLockFile(IProjectReference reference)
        {
            return LockFileUtilities.GetLockFile(reference.GetLockFile(), NullLogger.Instance) ?? new LockFile();
        }

        /// <summary>
        /// Returns the <see cref="LockFileTarget"/> section
        /// for a project TFM from the lock file provided by 
        /// <see cref="LockFile"/>.
        /// </summary>
        /// 
        /// <exception cref="SwitcherFileNotFoundException"/>
        protected virtual LockFileTarget GetProjectTarget(IProjectReference reference)
        {
            return GetLockFile(reference).GetTarget(new NuGetFramework(reference.TFI, new Version(reference.TFV), string.Empty), null) ??

                new LockFileTarget()
                {
                    Libraries = new List<LockFileTargetLibrary>()
                };
        }

        /// <summary>
        /// Basic operation of 
        /// switching one type 
        /// of reference to 
        /// another.
        /// </summary>
        public abstract IEnumerable<string> Switch();

        /// <summary>
        /// Iterates through the dependencies provided in the lock file and matches 
        /// against items found in the directories listed in the configuration file. 
        /// For each matched item, the passed delegate is executed.
        /// </summary>
        ///
        /// <exception cref="SwitcherFileNotFoundException"/>
        /// 
        /// <exception cref="FileNotFoundException"/>
        /// 
        /// <exception cref="ArgumentException">
        protected virtual void IterateAndExecute(IEnumerable<IProjectReference> references, Action<IProjectReference, LockFileTargetLibrary, string> func)
        {
            MessageProvider.Clear();

            ReadOnlyDictionary<string, string> items = OptionProvider.GetIncludeItems(Type);

            foreach (IProjectReference reference in references)
            {
                foreach (LockFileTargetLibrary library in GetProjectTarget(reference).Libraries)
                {
                    if (items.TryGetValue(library.Name, out string absolutePath))
                    {
                        func(reference, library, absolutePath);
                    }
                }

                reference.Save();
            }
        }

        /// <summary>
        /// Performs metadata validation
        /// and filling with data common 
        /// to all references.
        /// </summary>
        /// 
        /// <remarks>
        /// See: <seealso cref="AddReference(ProjectReference, ReferenceType, string, Dictionary{string, string})"/>.
        /// </remarks>
        protected virtual Dictionary<string, string> AdaptMetadata(string unevaluatedInclude, Dictionary<string, string> metadata)
        {
            if (!metadata.ContainsKey("Temp"))
            {
                metadata.Add("Temp", unevaluatedInclude);
            }

            return metadata;
        }

        /// <summary>
        /// Adds reference to the project. It is assumed
        /// that the original reference has been removed 
        /// earlier.
        /// </summary>
        /// 
        /// <param name="unevaluatedInclude">
        /// Must contain the assembly 
        /// name or the absolute path 
        /// to the project or Package
        /// Id.
        /// </param>
        /// 
        /// <exception cref="SwitcherException"/>
        /// 
        /// <returns>
        /// Returns false for duplicate <paramref name="unevaluatedInclude"/> values.
        /// </returns>
        protected virtual bool AddReference(IProjectReference reference, ReferenceType type, string unevaluatedInclude, Dictionary<string, string> metadata)
        {
            bool output = true;

            switch (type)
            {
                case ReferenceType.ProjectReference:
                case ReferenceType.PackageReference:
                case ReferenceType.Reference:
                    if (reference.MsbProject.GetItemsByEvaluatedInclude(unevaluatedInclude).Any())
                    {
                        output = false;
                    }
                    else
                    {
                        reference.MsbProject.AddItem(type.ToString(), unevaluatedInclude, AdaptMetadata(unevaluatedInclude, metadata));
                    }
                    break;
                default:
                    throw new SwitcherException(reference.MsbProject, $"Reference type not supported: { type }");
            }

            if (output)
            {
                MessageProvider.AddMessage(reference.MsbProject.FullPath, $"Dependency: { Path.GetFileName(unevaluatedInclude) } has been added. Type: { type }", MessageCategory.ME);
            }

            return output;
        }

        /// <summary>
        /// Provides a convenient way to list 
        /// and modify projects in a solution 
        /// file.
        /// </summary>
        protected virtual void SlnAction(string solution, IEnumerable<string> projects, ArgumentsBuilder builder)
        {
            if (projects.Any())
            {
                CliWrap.Command command = Cli.Wrap("dotnet").WithWorkingDirectory(Path.GetDirectoryName(solution))

                    .WithArguments(args => args.Add("sln").Add(solution).Add(builder.Build(), false));

                BufferedCommandResult result = command.ExecuteBufferedAsync().GetAwaiter().GetResult();

                if (!string.IsNullOrWhiteSpace(result.StandardOutput))
                {
                    MessageProvider.Clear();

                    MessageProvider.AddMessage(result.StandardOutput, MessageCategory.ME);
                }

                if (!string.IsNullOrWhiteSpace(result.StandardError))
                {
                    MessageProvider.Clear();

                    MessageProvider.AddMessage(result.StandardError,  MessageCategory.ER);
                }
            }
        }
    }
}