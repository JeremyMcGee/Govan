namespace Govan.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Entities;

    public class PowershellRunner : Runner, IRunner
    {
        public PowershellRunner(Computer computer)
            : base(computer)
        {
        }

        public void ExecuteCommand(string command)
        {
        }
    }
}
