namespace Govan.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Entities;

    abstract public class Runner
    {
        private Computer computer;

        public Runner(Computer computer)
        {
            this.computer = computer;
        }
    }
}
