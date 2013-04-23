namespace Govan.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Runners;

    public class Prep : AdminCommand
    {
        
        private readonly ICopier copier;

        public Prep()
            : this(new Copier())
        {
        }

        public Prep(ICopier copier)
            : base()
        {
            this.copier = copier;
            IsCommand("prep", "Prepares a host for remote Powershell control.");
        }

        public override int Run(string[] remainingArguments)
        {
            string sourceFile = Path.Combine(Environment.CurrentDirectory, "Resources", "AllowRemotePowershellAccess.reg");
            string targetFile = @"c:\Govan\AllowRemotePowershellAccess.reg";

            copier.Copy(base.Computer, sourceFile, targetFile);

            return 0;
        }
    }
}
