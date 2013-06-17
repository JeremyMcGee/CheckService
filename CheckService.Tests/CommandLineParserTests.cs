// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineParserTests.cs" company="Bassett Data">
//   Copyright (c) 2010 Bassett Data
// </copyright>
// <summary>
//   Tests the command line parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Tests the command line parser.
    /// </summary>
    [TestFixture]
    public class CommandLineParserTests
    {
        /// <summary>
        /// Tests the command line parser.
        /// </summary>
        [Test]
        public void CanParsePositionalArguments()
        {
            string[] args = new string[] { "first", "second" };
            CommandLineParser parser = new CommandLineParser(args);
            var result = parser.Parse();

            Assert.That(result.Options.Count == 0);
            Assert.That(result.PositionalArguments.Count == 2);
            Assert.That(result.PositionalArguments[0] == "first");
            Assert.That(result.PositionalArguments[1] == "second");
        }

        /// <summary>
        /// Tests the command line parser.
        /// </summary>
        [Test]
        public void CanParseOptionArgumentsWithHyphens()
        {
            string[] args = new string[] { "-a", "first", "-b", "second" };
            CommandLineParser parser = new CommandLineParser(args);
            var result = parser.Parse();

            Assert.That(result.Options.Count == 2);
            Assert.That(result.PositionalArguments.Count == 0);
            Assert.That(result.Options["a"] == "first");
            Assert.That(result.Options["b"] == "second");
        }

        /// <summary>
        /// Tests the command line parser.
        /// </summary>
        [Test]
        public void CanParseOptionArgumentsWithSlashes()
        {
            string[] args = new string[] { "/a", "first", "/b", "second" };
            CommandLineParser parser = new CommandLineParser(args);
            var result = parser.Parse();

            Assert.That(result.Options.Count == 2);
            Assert.That(result.PositionalArguments.Count == 0);
            Assert.That(result.Options["a"] == "first");
            Assert.That(result.Options["b"] == "second");
        }

        /// <summary>
        /// Tests the command line parser.
        /// </summary>
        [Test]
        public void CanParseMixedArgumentsWithPositionalAtStart()
        {
            string[] args = new string[] { "positional", "/a", "first", "/b", "second" };
            CommandLineParser parser = new CommandLineParser(args);
            var result = parser.Parse();

            Assert.That(result.Options.Count == 2);
            Assert.That(result.Options["a"] == "first");
            Assert.That(result.Options["b"] == "second");

            Assert.That(result.PositionalArguments.Count == 1);
            Assert.That(result.PositionalArguments[0] == "positional");
        }

        /// <summary>
        /// Tests the command line parser.
        /// </summary>
        [Test]
        public void CanParseFlags()
        {
            string[] args = new string[] { "-a", "-b" };
            CommandLineParser parser = new CommandLineParser(args);
            var result = parser.Parse();

            Assert.That(result.Options.Count == 2);
            Assert.That(result.Options["a"] == string.Empty);
            Assert.That(result.Options["b"] == string.Empty);

            Assert.That(result.PositionalArguments.Count == 0);
        }

        /// <summary>
        /// Tests the command line parser.
        /// </summary>
        [Test]
        public void CanParseFlagsMixedWithPositionalsAndOptions()
        {
            string[] args = new string[] { "-a", "-b", "foo", "bah", "-c", "-d" };
            CommandLineParser parser = new CommandLineParser(args);
            var result = parser.Parse();

            Assert.That(result.Options.Count == 4);
            Assert.That(result.Options["a"] == string.Empty);
            Assert.That(result.Options["b"] == "foo");
            Assert.That(result.Options["c"] == string.Empty);
            Assert.That(result.Options["d"] == string.Empty);

            Assert.That(result.PositionalArguments.Count == 1);
            Assert.That(result.PositionalArguments[0] == "bah");
        }
    }
}
