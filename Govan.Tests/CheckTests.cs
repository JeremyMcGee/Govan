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
            Mock<Computer> computer = new Mock<Computer>(MockBehavior.Loose);
            computer
                .SetupGet(c => c.Name)
                .Returns("MyComputer");

            Mock<RunnerFactory> mockRunnerFactory = new Mock<RunnerFactory>(MockBehavior.Strict);
            mockRunnerFactory
                .Setup(mrf => mrf.Create(RunnerType.PsExec, computer.Object));

            // when
            Check check = new Check(mockRunnerFactory.Object);

//            ConsoleCommandDispatcher.DispatchCommand(check, new[] {"-psexec"},,  )

            check.Run(new string[] { });

        }
    }
}
