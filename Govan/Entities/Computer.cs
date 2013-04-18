namespace Govan.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Computer
    {
        public string Name { get; private set; }

        public string AdminPassword { get; private set; }

        public Computer(string name, string adminPassword)
        {
            this.Name = name;
            this.AdminPassword = adminPassword;
        }
    }
}
