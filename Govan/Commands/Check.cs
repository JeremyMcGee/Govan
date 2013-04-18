namespace Govan.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using ManyConsole;

    using Runners;
    using Entities;
    using System.IO;

    public class Check : AdminCommand
    {
        private RunnerType runnerType;
        private string computerName;
        private RunnerFactory runnerFactory = new RunnerFactory();

        public Check(RunnerFactory runnerFactory)
            : base()
        {
            this.runnerFactory = runnerFactory;

            IsCommand("check", "Checks that a host can be controlled.");
            HasOption<RunnerType>("r|runnertype=", "Use a specific runner type: psexec | Powershell", r => runnerType = r);
        }

        public override int Run(string[] remainingArguments)
        {
            IRunner runner = runnerFactory.Create(runnerType, computer);
            runner.ExecuteCommand("systeminfo");

            return 0;
        }
    }

    public interface ICopier
    {
        void Copy(string hostname, string adminPassword, string source, string destination);
    }
}
