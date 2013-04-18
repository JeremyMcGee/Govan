﻿namespace Govan.Commands
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
        private List<RunnerType> runnerTypes = new List<RunnerType>();
        private RunnerFactory runnerFactory = new RunnerFactory();

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
                IRunner runner = runnerFactory.Create(runnerType, computer);
                runner.ExecuteCommand("systeminfo");
            }

            return 0;
        }
    }
}