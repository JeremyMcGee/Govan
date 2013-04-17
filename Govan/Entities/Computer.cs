namespace Govan.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Computer
    {
        public virtual string Name { get; private set; }

        public Computer(string name)
        {
            this.Name = name;
        }
    }
}
