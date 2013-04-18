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

    public class Copy : AdminCommand
    {
        private string source;
        private string destination;
        private ICopier copier;

        public Copy(ICopier copier)
            : base()
        {
            this.copier = copier;

            IsCommand("copy", "Copies a file to a remote host.");
            HasRequiredOption("s|source=", "Source filename on this local system.", s => source = s);
            HasRequiredOption("t|target=", "Target filename on remote system.", t => destination = t);
        }

        public override int Run(string[] remainingArguments)
        {
            try
            {
                copier.Copy(computer.Name, computer.AdminPassword, source, destination);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }

            return 0;
        }
    }
}
