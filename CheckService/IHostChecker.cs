// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHostChecker.cs" company="Bassett Data">
//   Copyright (c) 2010 Bassett Data
// </copyright>
// <summary>
//   Denotes that the class allows checking of hosts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService
{
    using System.Net;

    /// <summary>
    /// Denotes that the class allows checking of hosts.
    /// </summary>
    public interface IHostChecker
    {
        /// <summary>
        /// Gets the result text.
        /// </summary>
        /// <value>The result text.</value>
        string ResultText { get; }

        /// <summary>
        /// Gets the result status.
        /// </summary>
        /// <value>The result status.</value>
        HttpStatusCode? ResultStatus { get; }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        string Warnings { get; }

        /// <summary>
        /// Checks the specified URI.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="dnsHostName">Optionally, the DNS name of the host.</param>
        void Check(string endpoint, string dnsHostName);
    }
}
