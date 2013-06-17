// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineParser.cs" company="Bassett Data">
//   Copyright (c) 2010 Bassett Data
// </copyright>
// <summary>
//   A command-line arguments parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A command-line arguments parser.
    /// </summary>
    public class CommandLineParser
    {
        /// <summary>
        /// The supplied command line arguments.
        /// </summary>
        private readonly IEnumerable<string> arguments;

        /// <summary>
        /// The options in the command line.
        /// </summary>
        private readonly Dictionary<string, string> options;

        /// <summary>
        /// The command-line parsing options.
        /// </summary>
        private readonly CommandLineParseOptions parseOptions;

        /// <summary>
        /// The positional arguments in the command line.
        /// </summary>
        private readonly List<string> positionals;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineParser"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public CommandLineParser(IEnumerable<string> arguments)
            : this(arguments, CommandLineParseOptions.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineParser"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="parseOptions">The parse options.</param>
        public CommandLineParser(IEnumerable<string> arguments, CommandLineParseOptions parseOptions)
        {
            this.arguments = arguments;
            this.parseOptions = parseOptions;

            StringComparer comparer;
            if ((this.parseOptions & CommandLineParseOptions.CaseSensitiveOptions) ==
                CommandLineParseOptions.CaseSensitiveOptions)
            {
                comparer = StringComparer.InvariantCulture;
            }
            else
            {
                comparer = StringComparer.InvariantCultureIgnoreCase;
            }

            this.options = new Dictionary<string, string>(comparer);
            this.positionals = new List<string>();
        }

        /// <summary>
        /// Parses the command line.
        /// </summary>
        /// <returns>
        /// A <see cref="CommandLineArguments"/> instance that represents the command line arguments.
        /// </returns>
        public CommandLineArguments Parse()
        {
            List<string> argumentList = this.arguments.ToList();

            for (int i = 0; i < argumentList.Count; i++)
            {
                if (IsAnOption(argumentList, i))
                {
                    if (!IsAnOption(argumentList, i + 1) && (i + 1 < argumentList.Count))
                    {
                        this.AddOption(argumentList, i);
                        i++;
                    }
                    else
                    {
                        this.AddFlag(argumentList, i);
                    }
                }
                else
                {
                    this.AddPositionalParameter(argumentList, i);
                }
            }

            return new CommandLineArguments(this.options, this.positionals);
        }

        /// <summary>
        /// Determines whether the given item in the argument list is an option or something else. 
        /// Options are specified with a hyphen "-" or a slash "/".
        /// </summary>
        /// <param name="argumentList">The argument list.</param>
        /// <param name="i">The zero-based item index.</param>
        /// <returns>
        /// True of the item is an option, otherwise, false.
        /// </returns>
        private static bool IsAnOption(List<string> argumentList, int i)
        {
            if (i + 1 > argumentList.Count)
            {
                return false;
            }

            string argument = argumentList[i];
            if (argument.Length < 2)
            {
                return false;
            }

            return argument.StartsWith("-", StringComparison.Ordinal) 
                || argument.StartsWith("/", StringComparison.Ordinal);
        }

        /// <summary>
        /// Adds the positional parameter at the given index.
        /// </summary>
        /// <param name="argumentList">The argument list.</param>
        /// <param name="i">The index.</param>
        private void AddPositionalParameter(List<string> argumentList, int i)
        {
            this.positionals.Add(argumentList[i]);
        }

        /// <summary>
        /// Adds the option at the given index.
        /// </summary>
        /// <param name="argumentList">The argument list.</param>
        /// <param name="i">The index.</param>
        private void AddOption(List<string> argumentList, int i)
        {
            string optionCode = argumentList[i].Substring(1);
            this.options.Add(optionCode, argumentList[i + 1]);
        }

        /// <summary>
        /// Adds the flag at the given index.
        /// </summary>
        /// <param name="argumentList">The argument list.</param>
        /// <param name="i">The index.</param>
        private void AddFlag(List<string> argumentList, int i)
        {
            string optionCode = argumentList[i].Substring(1);
            this.options.Add(optionCode, string.Empty);
        }
    }
}
