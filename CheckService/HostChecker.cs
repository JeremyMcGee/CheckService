// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostChecker.cs" company="Bassett Data">
//   Copyright (c) 2010 Bassett Data
// </copyright>
// <summary>
//   Checks the status method of a host.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Checks the status method of a host.
    /// </summary>
    public class HostChecker : IHostChecker
    {
        /// <summary>
        /// Gets the result text.
        /// </summary>
        /// <value>The result text.</value>
        public string ResultText { get; private set; }

        /// <summary>
        /// Gets the result status.
        /// </summary>
        /// <value>The result status.</value>
        public HttpStatusCode? ResultStatus { get; private set; }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public string Warnings { get; private set; }

        /// <summary>
        /// Checks the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="dnsHostName">Optionally, the DNS name of the host.</param>
        public void Check(string endpoint, string dnsHostName)
        {
            Uri uri;
            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out uri))
            {
                throw new InvalidOperationException(string.Format(
                                                        CultureInfo.InvariantCulture, 
                                                        "Unknown endpoint {0}", 
                                                        endpoint));
            }

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
                {
                    Warnings = "SSL certificate issue: " + sslPolicyErrors.ToString();
                }
                
                return true;
            };

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            if (!string.IsNullOrEmpty(dnsHostName))
            {
                var port = uri.Port;
                request.Proxy = new WebProxy(dnsHostName, port);
            }

            request.KeepAlive = true;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            this.ResultStatus = response.StatusCode;

            using (Stream stm = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stm);
                this.ResultText = reader.ReadToEnd();
            }

            response.Close();
        }
    }
}
