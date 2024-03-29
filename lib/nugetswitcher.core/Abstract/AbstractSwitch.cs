﻿using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;

using NuGet.ProjectModel;

using NuGetSwitcher.Interface.Entity.Enum;
using NuGetSwitcher.Interface.Entity.Error;

using NuGetSwitcher.Interface.Provider.Message.Contract;
using NuGetSwitcher.Interface.Provider.Option.Contract;
using NuGetSwitcher.Interface.Provider.Project.Contract;

using NuGetSwitcher.Interface.Reference.Project.Contract;

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
            set;
        }

        protected IOptionProvider OptionProvider
        {
            get;
            set;
        }

        protected IProjectProvider<IProjectReference> ProjectProvider
        {
            get;
            set;
        }

        protected IMessageProvider MessageProvider
        {
            get;
            set;
        }

        protected AbstractSwitch(ReferenceType type, IOptionProvider optionProvider, IProjectProvider<IProjectReference> projectProvider, IMessageProvider messageProvider)
        {
            Type = type;

            OptionProvider  = optionProvider;
            ProjectProvider = projectProvider;
            MessageProvider = messageProvider;
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
                foreach (LockFileTargetLibrary library in reference.GetProjectTarget().Libraries)
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
        protected virtual void AddReference(IProjectReference reference, ReferenceType type, string unevaluatedInclude, Dictionary<string, string> metadata)
        {
            if (reference.AddReference(unevaluatedInclude, type, metadata))
            {
                MessageProvider.AddMessage(reference.MsbProject.FullPath, $"Dependency: { Path.GetFileName(unevaluatedInclude) } has been added. Type: { type }", MessageCategory.ME);
            }
        }

        /// <summary>
        /// Performs an action on the solution file, taking 
        /// into account the passed <paramref name="builder"/>.
        /// </summary>
        protected virtual void SlnAction(string solution, IEnumerable<string> projects, ArgumentsBuilder builder)
        {
            void Log(string source, MessageCategory category)
            {
                if (!string.IsNullOrWhiteSpace(source))
                {
                    MessageProvider.Clear().AddMessage(source, category);
                }
            }

            if (projects.Any())
            {
                BufferedCommandResult result = Cli.Wrap("dotnet").WithWorkingDirectory(Path.GetDirectoryName(solution)).WithArguments(args => args.Add("sln").Add(solution).Add(builder.Build(), false)).ExecuteBufferedAsync().GetAwaiter().GetResult();

                Log(result.StandardOutput, MessageCategory.ME);
                Log(result.StandardError , MessageCategory.ER);
            }
        }
    }
}