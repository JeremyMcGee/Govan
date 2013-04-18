namespace Govan.Runners
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Reads streams from a process's <see cref="Process.StandardOutput"/> and <see cref="Process.StandardError"/>
    /// streams.
    /// </summary>
    public class ProcessOutputReader
    {
        /// <summary>
        /// Builds the error output string.
        /// </summary>
        private readonly StringBuilder errorOutputBuilder = new StringBuilder();

        /// <summary>
        /// Builds the combined output of StandardError and StandardOutput.
        /// </summary>
        private readonly StringBuilder combinedOutputBuilder = new StringBuilder();

        /// <summary>
        /// Object that is locked to control access to <see cref="combinedOutputBuilder"/>.
        /// </summary>
        private readonly object combinedOutputLock = new object();

        /// <summary>
        /// The <see cref="Process"/> that this instance is reading.
        /// </summary>
        private readonly Process process;

        /// <summary>
        /// Builds the standard output string.
        /// </summary>
        private readonly StringBuilder standardOutputBuilder = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessOutputReader"/> class.
        /// </summary>
        /// <param name="process">
        /// The process.
        /// </param>
        public ProcessOutputReader(Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }

            this.process = process;
        }

        /// <summary>
        /// Gets the error output.
        /// </summary>
        /// <value>
        /// The error output.
        /// </value>
        public string StandardError { get; private set; }

        /// <summary>
        /// Gets the standard output.
        /// </summary>
        /// <value>
        /// The standard output.
        /// </value>
        public string StandardOutput { get; private set; }

        /// <summary>
        /// Gets the combined output of StandardOutput and StandardError, interleaved in the correct order.
        /// </summary>
        /// <value>The combined output of StandardOutput and StandardError.</value>
        public string CombinedOutput { get; private set; }

        /// <summary>
        /// Reads the process output.
        /// </summary>
        public void ReadProcessOutput()
        {
            // Check that the process has been started. In fact, process.Handle will itself throw an 
            // InvalidOperationException if the process hasn't been started.
            if (this.process.Handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Process has exited");
            }

            // Create and start thread to read StandardError
            var standardErrorThread = new Thread(this.ReadProcessOutputProc) { Name = "ReadStandardError" };
            standardErrorThread.Start(
                new ThreadParams { Builder = this.errorOutputBuilder, Stream = this.process.StandardError });

            // Create and start thread to read StandardOutput
            var standardOutputThread = new Thread(this.ReadProcessOutputProc) { Name = "ReadStandardOutput" };
            standardOutputThread.Start(
                new ThreadParams
                    {
                        Builder = this.standardOutputBuilder,
                        Stream = this.process.StandardOutput
                    });

            // Wait for both threads to exit
            standardErrorThread.Join();
            standardOutputThread.Join();

            this.StandardOutput = this.standardOutputBuilder.ToString();
            this.StandardError = this.errorOutputBuilder.ToString();
            this.CombinedOutput = this.combinedOutputBuilder.ToString();
        }

        /// <summary>
        /// Thread callback to read a process output stream.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void ReadProcessOutputProc(object state)
        {
            ThreadParams threadParams = state as ThreadParams;
            Debug.Assert(threadParams != null, "threadParams != null");

            var line = threadParams.Stream.ReadLine();
            while (line != null)
            {
                threadParams.Builder.AppendLine(line);
                lock (this.combinedOutputLock)
                {
                    this.combinedOutputBuilder.AppendLine(line);
                }

                line = threadParams.Stream.ReadLine();
            }
        }

        /// <summary>
        /// Parameters to a thread procedure.
        /// </summary>
        private class ThreadParams
        {
            /// <summary>
            /// Gets or sets the process output stream to read from.
            /// </summary>
            /// <value>
            /// The process output stream to read from.
            /// </value>
            public StreamReader Stream { get; set; }

            /// <summary>
            /// Gets or sets the target builder.
            /// </summary>
            /// <value>
            /// The target builder.
            /// </value>
            public StringBuilder Builder { get; set; }
        }
    }
}