namespace Govan.Commands
{
    using System;

    public class Copier : ICopier
    {
        public void Copy(string computerName, string adminPassword, string source, string destination)
        {
            Console.WriteLine("Copying from {0} to {1} on Computer {2}...", source, destination, computerName);
        }
    }
}