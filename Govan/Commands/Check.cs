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

    public class Check : ConsoleCommand
    {
        private RunnerType runnerType;
        private string computerName;

        RunnerFactory runnerFactory = new RunnerFactory();

        public Check(RunnerFactory runnerFactory)
        {
            this.runnerFactory = runnerFactory;

            IsCommand("check", "Checks that a host can be controlled.");
            HasOption<RunnerType>("r|runnertype=", "Use a specific runner type: psexec | Powershell", r => runnerType = r);
            HasOption<string>("c|computername=", "The name of the computer.", c => computerName = c); 
        }

        public override int Run(string[] remainingArguments)
        {
            Computer computer = new Computer(name: computerName);

            IRunner runner = runnerFactory.Create(runnerType, computer);
            runner.ExecuteCommand("systeminfo");

            return 0;
        }
    }
}
