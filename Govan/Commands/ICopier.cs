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

    public interface ICopier
    {
        void Copy(string hostname, string adminPassword, string source, string destination);
    }
}
