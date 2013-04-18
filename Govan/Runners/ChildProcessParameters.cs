namespace Govan.Runners
{
    using System.Collections.Generic;
    using System.IO;

    public class ChildProcessParameters
    {
        public ChildProcessParameters()
        {
            LogFilesToMonitor = new List<string>();
        }

        public string Arguments { get; set; }

        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the log file to which the process output is redirected.
        /// </summary>
        /// <value>
        /// The name of the log file to which the process output is redirected.
        /// </value>
        public string LogFileName { get; set; }

        /// <summary>
        /// Gets or sets the process title.
        /// </summary>
        /// <value>The process title.</value>
        public string ProcessTitle { get; set; }

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets the log files to monitor.
        /// </summary>
        /// <value>The log files to monitor.</value>
        public ICollection<string> LogFilesToMonitor { get; private set; }

        /// <summary>
        /// Gets or sets the TextWriter that will receive output from the monitored log files.
        /// </summary>
        /// <value>The TextWriter to receive the output.</value>
        public TextWriter Log { get; set; }
    }
}