namespace Govan.Commands
{
    using System;
    using Govan.Entities;

    public class Copier : ICopier
    {
        public void Copy(Computer computer, string source, string destination)
        {
            Console.WriteLine("Copying from {0} to {1} on Computer {2}...", source, destination, computer.Name);


        }
    }
}