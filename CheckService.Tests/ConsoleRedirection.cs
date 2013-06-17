// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleRedirection.cs" company="Bassett Data">
//   Copyright (c) 2010 Bassett Data
// </copyright>
// <summary>
//   Enables easy redirection of console output to a string.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CheckService.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Enables easy redirection of console output to a string.
    /// </summary>
    public class ConsoleRedirection : IDisposable
    {
        /// <summary>
        /// The new output stream.
        /// </summary>
        private readonly TextWriter redirectedOutput;

        /// <summary>
        /// Whether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The original Console.Error writer.
        /// </summary>
        private TextWriter originalErrorWriter;

        /// <summary>
        /// The original Console.Out writer.
        /// </summary>
        private TextWriter originalOutWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRedirection"/> class.
        /// </summary>
        public ConsoleRedirection()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRedirection"/> class.
        /// </summary>
        /// <param name="redirectError">
        /// If set to <c>true</c> the Error stream is also redirected.
        /// </param>
        public ConsoleRedirection(bool redirectError)
        {
            this.redirectedOutput = new StringWriter();
            this.SetupRedirections(redirectError);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRedirection"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public ConsoleRedirection(string fileName)
            : this(fileName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRedirection"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file in which to store console output.</param>
        /// <param name="redirectError">If set to <c>true</c> the Error stream is also redirected.</param>
        public ConsoleRedirection(string fileName, bool redirectError)
        {
            var streamWriter = new StreamWriter(new FileStream(fileName, FileMode.Create)) { AutoFlush = true };
            this.redirectedOutput = streamWriter;
            this.SetupRedirections(redirectError);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ConsoleRedirection"/> class.
        /// </summary>
        ~ConsoleRedirection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the redirected output.
        /// </summary>
        /// <value>The redirected output.</value>
        /// <returns>A string that contains the redirected console output.</returns>
        public string ConsoleOutput
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("ConsoleRedirection instance already disposed");
                }

                this.redirectedOutput.Flush();
                return this.redirectedOutput.ToString();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>True</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.Assert(this.originalOutWriter != null, "this.originalOutWriter != null");
                Console.SetOut(this.originalOutWriter);

                if (this.originalErrorWriter != null)
                {
                    Console.SetError(this.originalErrorWriter);
                }

                if (this.redirectedOutput != null)
                {
                    this.redirectedOutput.Flush();
                    this.redirectedOutput.Close();
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Setups the console redirections.
        /// </summary>
        /// <param name="redirectError">
        /// If set to <c>true</c> the Error stream is also redirected.
        /// </param>
        private void SetupRedirections(bool redirectError)
        {
            this.originalOutWriter = Console.Out;
            Console.SetOut(this.redirectedOutput);

            if (redirectError)
            {
                this.originalErrorWriter = Console.Error;
                Console.SetError(this.redirectedOutput);
            }
        }
    }
}
