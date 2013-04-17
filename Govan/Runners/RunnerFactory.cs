namespace Govan.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Entities;

    public class RunnerFactory
    {
        public virtual IRunner Create(RunnerType runnerType, Computer computer)
        {
            IRunner runner;

            switch (runnerType)
            {
                case RunnerType.PsExec:
                    runner = new PsExecRunner(computer);
                    break;
                case RunnerType.Powershell:
                    runner = new PowershellRunner(computer);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown runner type {0}", runnerType));
            }

            return runner;
        }
    }
}
