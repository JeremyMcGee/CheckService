// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParseTests.cs" company="Bassett Data">
//   Copyright (c) 2013 Bassett Data
// </copyright>
// <summary>
//   Tests the command line parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService.Tests
{
    using System;
    using System.Net;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Checks the command-line parser.
    /// </summary>
    [TestFixture]
    public class ArgumentParseTests
    {
        /// <summary>
        /// A command line parser test.
        /// </summary>
        [Test]
        public void DisplaysHelpWhenNoArguments()
        {
            using (ConsoleRedirection redirection = new ConsoleRedirection(true))
            {
                int result = Program.Main(new string[] { });
                Assert.That(result == -1);
                Assert.That(redirection.ConsoleOutput.Contains("Usage:"));
            }
        }

        /// <summary>
        /// A command line parser test.
        /// </summary>
        [Test]
        public void PerformsTestOKWhenOneArgument()
        {
            const string Uri = "http://www.google.com";
            const string StatusText = "<status></status>";

            Mock<IHostChecker> mockChecker = new Mock<IHostChecker>(MockBehavior.Strict);
            Program.Checker = mockChecker.Object;

            mockChecker
                .Setup(mc => mc.Check(It.Is<string>(st => st == Uri), It.Is<string>(st => st == null)));
            mockChecker
                .Setup(mc => mc.ResultStatus)
                .Returns(HttpStatusCode.OK);

            mockChecker
                .Setup(mc => mc.ResultText)
                .Returns(StatusText);

            mockChecker
                .Setup(mc => mc.Warnings)
                .Returns<string>(null);

            using (ConsoleRedirection redirection = new ConsoleRedirection(true))
            {
                int result = Program.Main(new string[] { Uri });
                Assert.That(result == 0);
                Assert.That(redirection.ConsoleOutput.Contains("200 OK from HTTP GET"));
                Assert.That(redirection.ConsoleOutput.Contains(StatusText));
            }

            mockChecker.VerifyAll();
        }

        /// <summary>
        /// A command line parser test.
        /// </summary>
        [Test]
        public void PerformsTest500WhenOneArgument()
        {
            const string Uri = "http://www.google.com";
            const string StatusText = "<status></status>";

            Mock<IHostChecker> mockChecker = new Mock<IHostChecker>(MockBehavior.Strict);
            Program.Checker = mockChecker.Object;

            mockChecker
                .Setup(mc => mc.Check(It.Is<string>(st => st == Uri), It.Is<string>(st => st == null)));
            mockChecker
                .Setup(mc => mc.ResultStatus)
                .Returns(HttpStatusCode.InternalServerError);

            mockChecker
                .Setup(mc => mc.ResultText)
                .Returns(StatusText);

            mockChecker
                .Setup(mc => mc.Warnings)
                .Returns<string>(null);

            using (ConsoleRedirection redirection = new ConsoleRedirection(true))
            {
                int result = Program.Main(new string[] { Uri });
                Assert.That(result == 500);
                Assert.That(redirection.ConsoleOutput.Contains("500 InternalServerError from HTTP GET"));
                Assert.That(redirection.ConsoleOutput.Contains(StatusText));
            }

            mockChecker.VerifyAll();
        }

        /// <summary>
        /// A command line parser test.
        /// </summary>
        [Test]
        public void ExceptionDoesSensibleMessage()
        {
            const string Uri = "http://www.google.com/";
            const string ExceptionText = "Exception text here";

            Mock<IHostChecker> mockChecker = new Mock<IHostChecker>(MockBehavior.Strict);
            Program.Checker = mockChecker.Object;

            mockChecker
                .Setup(mc => mc.Check(It.Is<string>(st => st == Uri), It.Is<string>(st => st == null)))
                .Throws(new Exception(ExceptionText));

            using (ConsoleRedirection redirection = new ConsoleRedirection(true))
            {
                int result = Program.Main(new string[] { Uri });
                Assert.That(result == -2);
                Assert.That(redirection.ConsoleOutput.Contains("Exception thrown during HTTP GET"));
                Assert.That(redirection.ConsoleOutput.Contains(ExceptionText));
            }

            mockChecker.VerifyAll();
        }

        /// <summary>
        /// A command line parser test.
        /// </summary>
        [Test]
        public void ParsesWebException()
        {
            Program.Checker = new HostChecker();
            const string Uri = "http://www.google.com/incorrecturi";

            using (ConsoleRedirection redirection = new ConsoleRedirection(true))
            {
                int result = Program.Main(new string[] { Uri });
                Assert.That(result == 404);
                Assert.That(redirection.ConsoleOutput.Contains("404 NotFound from HTTP GET"));
            }
        }

        /// <summary>
        /// A command line parser test.
        /// </summary>
        [Test]
        public void ParsesWebExceptionForFaultyDnsName()
        {
            Program.Checker = new HostChecker();
            const string Uri = "http://www.google.com/";
            const string HostDnsName = "cq1apiisXXX.brislabs.com";

            using (ConsoleRedirection redirection = new ConsoleRedirection(true))
            {
                int result = Program.Main(new string[] { Uri, "-d", HostDnsName });
                Assert.That(result == -2);
                Assert.That(redirection.ConsoleOutput.Contains("cannot be contacted"));
            }
        }

        /// <summary>
        /// A command line parser test.
        /// </summary>
        [Test]
        public void CanParseDnsName()
        {
            const string Uri = "http://www.google.com/";
            const string StatusText = "<status></status>";
            const string HostDnsName = "myhost.mydomain.com";

            Mock<IHostChecker> mockChecker = new Mock<IHostChecker>(MockBehavior.Strict);
            Program.Checker = mockChecker.Object;

            mockChecker
                .Setup(mc => mc.Check(It.Is<string>(st => st == Uri), It.Is<string>(st => st == HostDnsName)));
            mockChecker
                .Setup(mc => mc.ResultStatus)
                .Returns(HttpStatusCode.OK);
            mockChecker
                .Setup(mc => mc.Warnings)
                .Returns<string>(null);

            mockChecker
                .Setup(mc => mc.ResultText)
                .Returns(StatusText);

            using (ConsoleRedirection redirection = new ConsoleRedirection(true))
            {
                int result = Program.Main(new string[] { Uri, "-d", HostDnsName });
                Assert.That(result == 0);
                Assert.That(redirection.ConsoleOutput.Contains("200 OK from HTTP GET"));
                Assert.That(redirection.ConsoleOutput.Contains(HostDnsName));
                Assert.That(redirection.ConsoleOutput.Contains(StatusText));
            }

            mockChecker.VerifyAll();
        }

        /// <summary>
        /// A command line parser test.
        /// </summary>
        [Test]
        public void CanParseWarningText()
        {
            const string Uri = "http://www.google.com";
            const string StatusText = "<status></status>";
            const string WarningText = "Warning!";
            const string HostDnsName = "myhost.mydomain.com";

            Mock<IHostChecker> mockChecker = new Mock<IHostChecker>(MockBehavior.Strict);
            Program.Checker = mockChecker.Object;

            mockChecker
                .Setup(mc => mc.Check(It.Is<string>(st => st == Uri), It.Is<string>(st => st == HostDnsName)));
            mockChecker
                .Setup(mc => mc.ResultStatus)
                .Returns(HttpStatusCode.OK);
            mockChecker
                .Setup(mc => mc.Warnings)
                .Returns(WarningText);

            mockChecker
                .Setup(mc => mc.ResultText)
                .Returns(StatusText);

            using (ConsoleRedirection redirection = new ConsoleRedirection(true))
            {
                int result = Program.Main(new string[] { Uri, "-d", HostDnsName });
                Assert.That(result == 0);
                Assert.That(redirection.ConsoleOutput.Contains("200 OK from HTTP GET"));
                Assert.That(redirection.ConsoleOutput.Contains(HostDnsName));
                Assert.That(redirection.ConsoleOutput.Contains(WarningText));
            }

            mockChecker.VerifyAll();
        }
    }
}
