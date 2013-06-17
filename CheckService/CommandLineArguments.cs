// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineArguments.cs" company="Bassett Data">
//   Copyright (c) 2010 Bassett Data
// </copyright>
// <summary>
//   A set of parsed command line arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService
{
    using System.Collections.Generic;

    /// <summary>
    /// A set of parsed command line arguments.
    /// </summary>
    public class CommandLineArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArguments"/> class.
        /// </summary>
        /// <param name="options">The option arguments.</param>
        /// <param name="positionalArguments">The positional arguments.</param>
        internal CommandLineArguments(Dictionary<string, string> options, List<string> positionalArguments)
        {
            this.PositionalArguments = positionalArguments.AsReadOnly();
            this.Options = new Dictionary<string, string>(options);
        }

        /// <summary>
        /// Gets the option command-line arguments.
        /// </summary>
        /// <value>
        /// The option command-line arguments.
        /// </value>
        public IDictionary<string, string> Options { get; private set; }

        /// <summary>
        /// Gets the positional command-line arguments.
        /// </summary>
        /// <value>The positional command-line arguments.</value>
        public IList<string> PositionalArguments { get; private set; }
    }
}
