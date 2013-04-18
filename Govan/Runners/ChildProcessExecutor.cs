namespace Govan.Runners
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;

    /// <summary>
    /// A helper class to run a child process.
    /// </summary>
    public class ChildProcessExecutor
    {
        private const int Win32FileNotFound = 2;

        /// <summary>
        /// The polling interval to poll logs, in milliseconds.
        /// </summary>
        private const int POLLINTERVAL = 200;

        /// <summary>
        /// The combined output of stdout and stderr, or <c>null</c> if <see cref="ExecuteProcess"/> has not
        /// been run.
        /// </summary>
        private string processOutput;

        /// <summary>
        /// Gets the combined output of stdout and stderr after running the process.
        /// </summary>
        /// <value>The combined output of stdout and stderr after running the process.</value>
        public string ProcessOutput
        {
            get
            {
                if (this.processOutput == null)
                {
                    throw new InvalidOperationException("ProcessOutput not valid until ExecuteProcess has been run");
                }

                return this.processOutput;
            }
        }

        /// <summary>
        /// Runs a child process.
        /// </summary>
        /// <param name="parameters">The parameters that are used to execute the child process.</param>
        public void ExecuteProcess(ChildProcessParameters parameters)
        {
            // Default to empty to prevent InvalidOperationException from ProcessOutput - if the process runs OK 
            // then we will update
            this.processOutput = string.Empty;
            bool grabExternalLog = !string.IsNullOrEmpty(parameters.LogFileName);

            string hostName = GetHostName(parameters.FileName);
            var logFileMonitors = LogFileMonitor.CreateMonitors(
                parameters.ProcessTitle,
                hostName,
                parameters.Log,
                parameters.LogFilesToMonitor);

            try
            {
                var info = new ProcessStartInfo
                               {
                                   FileName = parameters.FileName,
                                   Arguments = parameters.Arguments,
                                   WorkingDirectory = parameters.WorkingDirectory,
                                   UseShellExecute = false,
                                   RedirectStandardOutput = grabExternalLog,
                                   RedirectStandardError = grabExternalLog,
                                   CreateNoWindow = true
                               };

                Process process = null;
                try
                {
                    process = Process.Start(info);
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == Win32FileNotFound)
                    {
                        throw new FileNotFoundException(
                            string.Format("The system could not find the file '{0}'", parameters.FileName),
                            parameters.FileName,
                            ex);
                    }
                }

                if (process == null)
                {
                    // TODO: determine the circumstances in which this situation could occur
                    string message = string.Format("Unable to start the {0} process", parameters.ProcessTitle);
                    throw new InvalidOperationException(message);
                }

                var reader = new ProcessOutputReader(process);
                if (grabExternalLog)
                {
                    reader.ReadProcessOutput();
                }

                if (!parameters.LogFilesToMonitor.Any())
                {
                    process.WaitForExit();
                }
                else
                {
                    do
                    {
                        foreach (var logFileMonitor in logFileMonitors)
                        {
                            logFileMonitor.PollFile();
                        }

                        Thread.Sleep(POLLINTERVAL);
                    }
                    while (!process.HasExited);
                }

                if (grabExternalLog)
                {
                    this.processOutput = reader.CombinedOutput;
                    WriteLogFile(parameters.LogFileName, reader.CombinedOutput);
                }

                if (process.ExitCode == 1 && hostName != "localhost")
                {
                    string message = string.Format(
                        "The {1} process exited with code {2}. " +
                        "This generally means that the MSBuild task started by the deployer has failed. " +
                        "Check the log file on the remote machine {3} for details",
                        Environment.NewLine,
                        parameters.ProcessTitle,
                        process.ExitCode,
                        hostName);

                    throw new InvalidOperationException(message);
                }
                else if (process.ExitCode == 1)
                {
                    string message = string.Format(
                        "The {1} process exited with code {2}. " +
                        "This generally means that the MSBuild task started by the deployer has failed. " +
                        "Check the log file on the local machine for details",
                        Environment.NewLine,
                        parameters.ProcessTitle,
                        process.ExitCode);

                    throw new InvalidOperationException(message);
                }
                else if (process.ExitCode != 0)
                {
                    string message = string.Format(
                        "The {0} process exited with code {1}",
                        parameters.ProcessTitle,
                        process.ExitCode);

                    throw new InvalidOperationException(message);
                }
            }
            catch (FileNotFoundException ex)
            {
                string message = string.Format(
                    "File '{0}' was not found for process {1}", parameters.FileName, parameters.ProcessTitle);
                throw new InvalidOperationException(message, ex);
            }
        }

        /// <summary>
        /// Gets the name of the host from the given file, or "localhost" if the file is local.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The name of the host for logging.</returns>
        private static string GetHostName(string fileName)
        {
            string rootString = Path.GetPathRoot(fileName);
            if (rootString.StartsWith(@"\\"))
            {
                return rootString.Substring(2, rootString.Length - 3);
            }

            return "localhost";
        }

        /// <summary>
        /// Writes a log file.
        /// </summary>
        /// <param name="logFileName">The log file name.</param>
        /// <param name="contents">The contents.</param>
        private static void WriteLogFile(string logFileName, string contents)
        {
            try
            {
                File.WriteAllText(logFileName, contents);
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is UnauthorizedAccessException || ex is PathTooLongException ||
                    ex is FileNotFoundException || ex is DirectoryNotFoundException || ex is SecurityException)
                {
                    var message = string.Format(
                        "{0} while writing process log file '{1}': {2}", ex.GetType().Name, logFileName, ex.Message);
                    throw new InvalidOperationException(message, ex);
                }
            }
        }

        /// <summary>
        /// Monitors log files.
        /// </summary>
        private class LogFileMonitor
        {
            /// <summary>
            /// Creates the log file monitors for the given log files.
            /// </summary>
            /// <param name="processTitle">The process title.</param>
            /// <param name="location">The location.</param>
            /// <param name="log">The log where messages should go.</param>
            /// <param name="logFilesToMonitor">The log files to monitor.</param>
            /// <returns>A collection of log file monitors.</returns>
            public static IEnumerable<LogFileMonitor> CreateMonitors(
                string processTitle,
                string location,
                TextWriter log,
                IEnumerable<string> logFilesToMonitor)
            {
                foreach (string fileToMonitor in logFilesToMonitor)
                {
                    yield return new LogFileMonitor
                                     {
                                         processTitle = processTitle,
                                         location = location,
                                         log = log,
                                         fileToMonitor = fileToMonitor
                                     };
                }
            }

            /// <summary>
            /// The title of this process.
            /// </summary>
            private string processTitle;

            /// <summary>
            /// The location of the deployer.
            /// </summary>
            private string location;

            /// <summary>
            /// The log to write to.
            /// </summary>
            private TextWriter log;

            /// <summary>
            /// The file to monitor.
            /// </summary>
            private string fileToMonitor;

            /// <summary>
            /// The last time the file was written.
            /// </summary>
            private DateTime lastTimeFileWritten = DateTime.MinValue;

            /// <summary>
            /// The last point in the file.
            /// </summary>
            private long lastPointInFile;

            /// <summary>
            /// Polls the file.
            /// </summary>
            public void PollFile()
            {
                if (!File.Exists(fileToMonitor))
                    return;

                if (File.GetLastWriteTimeUtc(fileToMonitor) == lastTimeFileWritten)
                    return;

                try
                {
                    using (StreamReader reader = File.OpenText(fileToMonitor))
                    {
                        reader.BaseStream.Seek(lastPointInFile, SeekOrigin.Begin);
                        while (!reader.EndOfStream)
                        {
                            string message = reader.ReadLine();
                            WriteMessageToLog(message);
                        }

                        lastTimeFileWritten = File.GetLastWriteTimeUtc(fileToMonitor);
                        lastPointInFile = reader.BaseStream.Position;
                    }
                }
                catch (IOException)
                {
                    // If we fail, most likely because the file is locked, then silently do it all over again.
                    lastTimeFileWritten = DateTime.UtcNow;
                }
            }

            /// <summary>
            /// Writes the message to the log.
            /// </summary>
            /// <param name="message">The message.</param>
            private void WriteMessageToLog(string message)
            {
                string logMessage = string.Format("[{0}] {1} - {2}", processTitle, location, message);
                log.WriteLine(logMessage);
            }
        }
    }
}
