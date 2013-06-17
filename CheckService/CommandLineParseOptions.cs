// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineParseOptions.cs" company="Bassett Data">
//   Copyright (c) 2010 Bassett Data
// </copyright>
// <summary>
//   Command line parsing options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService
{
    using System;

    /// <summary>
    /// Command line parsing options.
    /// </summary>
    [Flags]
    public enum CommandLineParseOptions
    {
        /// <summary>
        /// No option specified (use defaults).
        /// </summary>
        None = 0,

        /// <summary>
        /// Command-line options are case-sensitive.
        /// </summary>
        CaseSensitiveOptions,

        /// <summary>
        /// A duplicate option causes an exception to be thrown. The default is not to throw an exception.
        /// </summary>
        DuplicateOptionCausesError,

        /// <summary>
        /// When duplicate options are detected, the last specified value is used. The default is to use the 
        /// first specified value.
        /// </summary>
        DuplicateOptionUseLast
    }
}
