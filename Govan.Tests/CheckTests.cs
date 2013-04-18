namespace Govan.Tests
{
    using System.IO;

    using Commands;
    using Entities;
    using ManyConsole;
    using Moq;
    using NUnit.Framework;
    using Runners;

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
                .Setup(r => r.ExecuteCommand(@"c:\temp", "systeminfo", ""));

            Mock<RunnerFactory> mockRunnerFactory = new Mock<RunnerFactory>(MockBehavior.Strict);
            mockRunnerFactory
                .Setup(mrf => mrf.Create(
                    actualRunnerType,
                    It.Is<Computer>(c => (c.Name == "mycomputer") && (c.AdminPassword == "foobah"))))
                .Returns(runner.Object);

            // when
            Check check = new Check(mockRunnerFactory.Object);

            StringWriter output = new StringWriter();
            ConsoleCommandDispatcher.DispatchCommand(
                check,
                new[] 
                { 
                    "-runnertype", runnerTypeParameter, 
                    "-computername", "mycomputer",
                    "-adminpassword", "foobah"
                },
                output);

            int result = check.Run(new string[] { });
            Assert.That(result, Is.EqualTo(0));

            runner.VerifyAll();
            mockRunnerFactory.VerifyAll();
        }

        [Test]
        public void CanCallFactoryMultipleTimesWhenNoRunnerTypeSpecified()
        {
            // given
            Mock<IRunner> runner = new Mock<IRunner>(MockBehavior.Strict);
            runner
                .Setup(r => r.ExecuteCommand(@"c:\temp", "systeminfo", ""));

            Mock<RunnerFactory> mockRunnerFactory = new Mock<RunnerFactory>(MockBehavior.Loose);
            mockRunnerFactory
                .Setup(
                    mrf => mrf.Create(
                            RunnerType.PsExec,
                            It.Is<Computer>(c => (c.Name == "mycomputer") && (c.AdminPassword == "foobah"))))
                .Returns(runner.Object);

            mockRunnerFactory
                .Setup(
                    mrf => mrf.Create(
                            RunnerType.Powershell,
                            It.Is<Computer>(c => (c.Name == "mycomputer") && (c.AdminPassword == "foobah"))))
                .Returns(runner.Object);

            // when
            Check check = new Check(mockRunnerFactory.Object);

            StringWriter output = new StringWriter();
            ConsoleCommandDispatcher.DispatchCommand(
                check,
                new[] 
                { 
                    "-computername", "mycomputer",
                    "-adminpassword", "foobah"
                },
                output);

            int result = check.Run(new string[] { });
            Assert.That(result, Is.EqualTo(0));

            runner.VerifyAll();
            mockRunnerFactory.VerifyAll();
        }

        [Test]
        public void FailedCheckCausesNonZeroReturnCode()
        {
            Assert.Fail();
        }
    }
}
