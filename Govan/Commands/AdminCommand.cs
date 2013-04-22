namespace Govan.Commands
{
    using Entities;
    using ManyConsole;
    using System.Net;

    public abstract class AdminCommand : ConsoleCommand
    {
        protected Computer Computer;

        private string computerName = string.Empty;
        private string adminUsername = "Administrator";
        private string adminPassword = string.Empty;

        
        protected AdminCommand()
        {
            // TODO extend to support manifests
            HasRequiredOption<string>("c|computername=", "The name of the computer.", c => computerName = c);
            HasOption<string>("u|adminusername=", "The admin username, if not Administrator.", u => adminUsername = u);
            HasRequiredOption<string>("a|adminpassword=", "The admin password.", p => adminPassword = p);
        }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            NetworkCredential networkCredential = new NetworkCredential(adminUsername, adminPassword);

            Computer = new Computer(name: computerName, networkCredential:networkCredential);

            return base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
        }
    }
}
