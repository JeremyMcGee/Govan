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

    abstract public class AdminCommand : ConsoleCommand
    {
        private string computerName = string.Empty;
        private string adminPassword = string.Empty;
        protected Computer computer;

        public AdminCommand()
        {
            // TODO extend to support manifests
            HasRequiredOption<string>("c|computername=", "The name of the computer.", c => computerName = c);
            HasRequiredOption<string>("a|adminpassword=", "The admin password.", p => adminPassword = p);
        }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            computer = new Computer(name: computerName, adminPassword: adminPassword);

            return base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
        }
    }
}
