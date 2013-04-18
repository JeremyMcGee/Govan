namespace Govan.Commands
{
    using Entities;
    using ManyConsole;

    public abstract class AdminCommand : ConsoleCommand
    {
        protected Computer Computer;

        private string computerName = string.Empty;
        private string adminPassword = string.Empty;
        
        protected AdminCommand()
        {
            // TODO extend to support manifests
            HasRequiredOption<string>("c|computername=", "The name of the Computer.", c => computerName = c);
            HasRequiredOption<string>("a|adminpassword=", "The admin password.", p => adminPassword = p);
        }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            Computer = new Computer(name: computerName, adminPassword: adminPassword);

            return base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);
        }
    }
}
