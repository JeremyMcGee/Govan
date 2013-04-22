namespace Govan.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public class Computer
    {
        public Computer(string name, NetworkCredential networkCredential)
        {
            this.Name = name;
            this.NetworkCredential = networkCredential;
        }

        public string Name { get; private set; }

        public NetworkCredential NetworkCredential { get; private set; }
    }
}
