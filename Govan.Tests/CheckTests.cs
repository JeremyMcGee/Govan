namespace Govan.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using NUnit.Framework;
    using Moq;

    using Commands;
    using Runners;
    using ManyConsole;
    using System.IO;

    [TestFixture]
    public class CheckTests
    {
        [Test]
        public void CanCheckPsExec()
        {
            // given
            Mock<IRunner> runner = new Mock<IRunner>(MockBehavior.Strict);
            runner
                .Setup(r => r.ExecuteCommand("systeminfo"));

            Mock<RunnerFactory> mockRunnerFactory = new Mock<RunnerFactory>(MockBehavior.Strict);
            mockRunnerFactory
                .Setup(mrf => mrf.Create(RunnerType.PsExec, It.Is<Computer>(c => c.Name == "mycomputer")))
                .Returns(runner.Object);

            // when
            Check check = new Check(mockRunnerFactory.Object);

            StringWriter output = new StringWriter();
            ConsoleCommandDispatcher.DispatchCommand(check, 
                new[] { "-runnertype", "psexec", "-computername", "mycomputer" }, 
            output);

            int result = check.Run(new string[] { });
            Assert.That(result, Is.EqualTo(0));

            runner.VerifyAll();
            mockRunnerFactory.VerifyAll();
        }
    }
}
