namespace Govan.Commands
{
    using System.Collections.Generic;

    using Runners;

    /// <summary>
    /// Executes a check.
    /// </summary>
    public class Check : AdminCommand
    {
        private readonly List<RunnerType> runnerTypes = new List<RunnerType>();

        private readonly RunnerFactory runnerFactory = new RunnerFactory();

        public Check()
            : this(new RunnerFactory())
        {
        }

        public Check(RunnerFactory runnerFactory)
            : base()
        {
            this.runnerFactory = runnerFactory;

            IsCommand("check", "Checks that a host can be controlled.");
            HasOption<RunnerType>("r|runnertype=", "Use a specific runner type: psexec | Powershell", r => runnerTypes.Add(r));
        }

        public override int Run(string[] remainingArguments)
        {
            if (runnerTypes.Count == 0)
            {
                runnerTypes.Add(RunnerType.PsExec);
                runnerTypes.Add(RunnerType.Powershell);
            }

            foreach (var runnerType in runnerTypes)
            {
                IRunner runner = runnerFactory.Create(runnerType, Computer);
                runner.ExecuteCommand(@"c:\temp", "systeminfo", "");
            }

            return 0;
        }
    }
}