namespace Govan.MachineDependentTests
{
    using System.IO;

    using Govan.Commands;
    using Govan.Entities;

    using NUnit.Framework;

    [TestFixture]
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

        [Test]
        public void CopyFailsIfLocalFileDoesNotExist()
        {
            Assert.Fail();
        }
    }
}
