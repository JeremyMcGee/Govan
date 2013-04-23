namespace Govan.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ManyConsole;
    using NUnit.Framework;
    using Moq;
    using Commands;
    using Runners;
    using Entities;

    [TestFixture]
    public class PrepCommandTests
    {
        [Test]
        public void PrepCopiesPowershellRegFile()
        {
            // given
            Mock<ICopier> copier = new Mock<ICopier>(MockBehavior.Strict);
            copier
                .Setup(r => r.Copy(
                    It.Is<Computer>(c => c.Name == "mycomputer" && c.NetworkCredential.Password == "foobah"),
                    It.Is<string>(source => source.Contains("AllowRemotePowershellAccess.reg")),
                    It.Is<string>(target => target.Contains("AllowRemotePowershellAccess.reg"))));

            // when
            Prep prep = new Prep();

            StringWriter output = new StringWriter();
            ConsoleCommandDispatcher.DispatchCommand(
                prep,
                new[] 
                { 
                    "-computername", "mycomputer",
                    "-adminpassword", "foobah"
                },
                output);

            int result = prep.Run(new string[] { });
            Assert.That(result, Is.EqualTo(0));

            copier.VerifyAll();
        }

        [Test]
        public void PrepExecutesPowershellRegFileWithPsExec()
        {
            Assert.Fail();
        }

        [Test]
        public void PrepOpensPowershellRemotingWsmanAccessForThisHost()
        {
            Assert.Fail();
        }
    }
}
