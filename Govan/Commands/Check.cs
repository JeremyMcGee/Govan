namespace Govan.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Govan.Runners;
    using ManyConsole;

    public class Check : ConsoleCommand
    {
        RunnerFactory runnerFactory = new RunnerFactory();

        public Check(RunnerFactory runnerFactory)
        {
            this.runnerFactory = runnerFactory;
        }

        public override int Run(string[] remainingArguments)
        {
            throw new NotImplementedException();
        }
    }
}
