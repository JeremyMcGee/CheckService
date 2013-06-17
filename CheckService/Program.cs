// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Bassett Data">
//   Copyright (c) 2010 Bassett Data
// </copyright>
// <summary>
//   Main entry point. Executes HTTP GET on a given host, optionally overriding the host-header.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Net;

    /// <summary>
    /// Executes HTTP GET on a given host, optionally overriding the host-header.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Required for console application")]
    public class Program
    {
        /// <summary>
        /// The URI to check.
        /// </summary>
        private static string uriToCheck;

        /// <summary>
        /// The remote host DNS name.
        /// </summary>
        private static string hostDnsName;

        /// <summary>
        /// Initializes static members of the <see cref="Program"/> class.
        /// </summary>
        /// <remarks>Here so we can do poor-mans IoC using
        /// direct injection. See tests.</remarks>
        static Program()
        {
            Checker = new HostChecker();
        }

        /// <summary>
        /// Gets or sets the checker.
        /// </summary>
        /// <value>The checker.</value>
        public static IHostChecker Checker { get; set; }

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The DOS error code: 0 if OK and check passed, the HTTP status code if the check failed,
        /// -1 if incorrect parameters</returns>
        public static int Main(string[] args)
        {
            ShowBanner();
            if (!ParseArguments(args))
            {
                ShowHelp();
                return -1;
            }

            return PerformCheck();
        }

        /// <summary>
        /// Performs the check.
        /// </summary>
        /// <returns>The DOS result code.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required for proper error handling")]
        private static int PerformCheck()
        {
            int result = -2;
            string resultText = null;

            try
            {
                Checker.Check(uriToCheck, hostDnsName);
                if (Checker.ResultStatus.HasValue)
                {
                    result = (int)Checker.ResultStatus.Value;
                    string resultString = Checker.ResultStatus.ToString();
                    resultText = Checker.ResultText;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    WriteStatus(result, resultString);
                    Console.ResetColor();
                }

                if (!string.IsNullOrEmpty(Checker.Warnings))
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(Checker.Warnings);
                    Console.ResetColor();
                }
            }
            catch (WebException ex)
            {
                using (HttpWebResponse errorResponse = (HttpWebResponse)ex.Response)
                {
                    if (errorResponse != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        WriteStatus((int)errorResponse.StatusCode, errorResponse.StatusCode.ToString());
                        Console.ResetColor();
                        result = (int)errorResponse.StatusCode;
                    }
                    else if (ex.Message.Contains("The proxy name could not be resolved"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(string.Format(
                            CultureInfo.CurrentUICulture,
                            "Host {0} cannot be contacted or does not exist.",
                            hostDnsName));
                        Console.ResetColor();
                    }
                    else
                    {
                        resultText = HandleException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                resultText = HandleException(ex);
            }

            Console.WriteLine();
            Console.WriteLine(resultText);

            if (result == 200)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Writes the status.
        /// </summary>
        /// <param name="result">The result code.</param>
        /// <param name="resultString">The result string.</param>
        private static void WriteStatus(int result, string resultString)
        {
            if (string.IsNullOrEmpty(hostDnsName))
            {
                Console.WriteLine(string.Format(
                    CultureInfo.CurrentUICulture,
                    "{0} {1} from HTTP GET to {2}",
                    result,
                    resultString,
                    uriToCheck));
            }
            else
            {
                Console.WriteLine(string.Format(
                    CultureInfo.CurrentUICulture,
                    "{0} {1} from HTTP GET to {2} on {3}",
                    result,
                    resultString,
                    uriToCheck,
                    hostDnsName));
            }
        }

        /// <summary>
        /// Handles the given exception, returning an appropriate message.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The message.</returns>
        private static string HandleException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format(
                CultureInfo.CurrentUICulture,
                "Exception thrown during HTTP GET to {0}",
                uriToCheck));
            Console.ResetColor();
            
            return ex.ToString();
        }

        /// <summary>
        /// Checks the arguments are valid, and parses them.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>True if valid, otherwise, false.</returns>
        private static bool ParseArguments(IEnumerable<string> args)
        {
            uriToCheck = null;
            hostDnsName = null;

            CommandLineParser parser = new CommandLineParser(args);
            var parseResults = parser.Parse();

            if (parseResults.PositionalArguments.Count != 1)
            {
                return false;
            }

            uriToCheck = parseResults.PositionalArguments[0];

            if (parseResults.Options.Count == 0)
            {
                return true;
            }

            if (parseResults.Options.Count > 1 || !parseResults.Options.Keys.Contains("d"))
            {
                return false;
            }

            hostDnsName = parseResults.Options["d"];
            return true;
        }

        /// <summary>
        /// Shows the banner.
        /// </summary>
        private static void ShowBanner()
        {
            Console.WriteLine();
            Console.WriteLine("CheckService 1.1.0   Checks a REST service.");
            Console.WriteLine();
        }

        /// <summary>
        /// Shows the help.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("Performs an HTTP GET on a given host.");
            Console.WriteLine("Usage:  CheckService http://hostname:port/path -d dnsname");
            Console.WriteLine();
            Console.WriteLine("http://hostname:port/path     The URI to check.");
            Console.WriteLine("-d dnsname [optional]         The actual host to which the GET should be sent.");
            Console.WriteLine();
            Console.WriteLine("Specifying the dnsname allows a check to be run directly on a host, bypassing");
            Console.WriteLine("any redirections or pooling performed by local traffic managers and suchlike.");
            Console.WriteLine();
            Console.WriteLine("For latest version, check http://github.com/JeremyMcGee/CheckService .");
        }
    }
}