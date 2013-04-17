using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Govan.Runners
{
    public enum RunnerType
    {
        PsExec,
        Powershell
    }

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

    public interface IRunner
    {
        void ExecuteCommand(string command);
    }

    abstract public class Runner
    {
        private Computer computer;

        public Runner(Computer computer)
        {
            this.computer = computer;
        }
    }

    public class PsExecRunner : Runner, IRunner
    {
        public PsExecRunner(Computer computer)
            : base(computer)
        {
        }

        public void ExecuteCommand(string command)
        {
        }
    }

    public class PowershellRunner : Runner, IRunner
    {
        public PowershellRunner(Computer computer)
            : base(computer)
        {
        }

        public void ExecuteCommand(string command)
        {
        }
    }

    public class Computer
    {
        public virtual string Name { get; private set; }

        public Computer(string name)
        {
            this.Name = name;
        }

    }
}
