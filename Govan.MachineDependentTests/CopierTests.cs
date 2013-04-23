namespace Govan.MachineDependentTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.IO;

    using Govan.Commands;
    using Govan.Entities;

    using NUnit.Framework;
    using System.Net;

    static public class TestContext
    {
        public static NetworkCredential LocalAdminCredential
        {
            get { return new NetworkCredential("TestUser", "!+Pa55word"); }
        }
    }

    public class CopierTests
    {
        [Test]
        public void CanCopyLocalFile()
        {
            string source = Path.GetTempFileName();
            string destination = Path.GetTempFileName();

            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            Computer computer = new Computer("localhost", TestContext.LocalAdminCredential);

            Copier copier = new Copier();
            copier.Copy(computer, source, destination);

            Assert.That(File.Exists(destination));
        }
    }
}
