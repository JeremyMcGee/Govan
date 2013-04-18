namespace Govan.Commands
{
    using System;

    public class Copy : AdminCommand
    {
        private string source;

        private string destination;

        private readonly ICopier copier;

        public Copy()
            : this(new Copier())
        {
        }


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
                copier.Copy(Computer.Name, Computer.AdminPassword, source, destination);
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
