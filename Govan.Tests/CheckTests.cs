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
    public class CheckTests
    {
        [TestCase("psexec", RunnerType.PsExec)]
        [TestCase("powershell", RunnerType.Powershell)]
        public void CanCallFactoryForTest(string runnerTypeParameter, RunnerType actualRunnerType)
        {
            // given
            Mock<IRunner> runner = new Mock<IRunner>(MockBehavior.Strict);
            runner
                .Setup(r => r.ExecuteCommand("systeminfo"));

            Mock<RunnerFactory> mockRunnerFactory = new Mock<RunnerFactory>(MockBehavior.Strict);
            mockRunnerFactory
                .Setup(mrf => mrf.Create(actualRunnerType, It.Is<Computer>(c => c.Name == "mycomputer")))
                .Returns(runner.Object);

            // when
            Check check = new Check(mockRunnerFactory.Object);

            StringWriter output = new StringWriter();
            ConsoleCommandDispatcher.DispatchCommand(check,
                new[] { "-runnertype", runnerTypeParameter, "-computername", "mycomputer" }, 
            output);

            int result = check.Run(new string[] { });
            Assert.That(result, Is.EqualTo(0));

            runner.VerifyAll();
            mockRunnerFactory.VerifyAll();
        }
    }
}
