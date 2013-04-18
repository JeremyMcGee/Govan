namespace Govan.Runners
{
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Entities;

    public class PsExecRunner : Runner, IRunner
    {
        public PsExecRunner(Computer computer)
            : base(computer)
        {
        }

        public void ExecuteCommand(string workingFolder, string command, string arguments)
        {
            ChildProcessParameters childProcessParameters = new ChildProcessParameters
                                                                {
                                                                    FileName = command,
                                                                    Arguments = arguments,
                                                                    WorkingDirectory = workingFolder
                                                                };

            ChildProcessExecutor executor = new ChildProcessExecutor();
            executor.ExecuteProcess(childProcessParameters);
        }
    }
}
