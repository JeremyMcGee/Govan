namespace Govan.Tests
{
    using System;
    using System.IO;
    using Commands;
    using Govan.Entities;
    using ManyConsole;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class CopyCommandTests
    {
        [Test]
        public void CanCallCopier()
        {
            // given
            Mock<ICopier> copier = new Mock<ICopier>(MockBehavior.Strict);
            copier
                .Setup(r => r.Copy(It.Is<Computer>(c => c.Name == "mycomputer"), @"c:\temp\foo.bat", @"\bah\thing.bat"));

            // when
            Copy copy = new Copy(copier.Object);

            StringWriter output = new StringWriter();
            ConsoleCommandDispatcher.DispatchCommand(
                copy,
                new[] 
                { 
                    "-source", @"c:\temp\foo.bat",
                    "-target", @"\bah\thing.bat",
                    "-computername", "mycomputer",
                    "-adminpassword", "foobah"
                },
                output);

            int result = copy.Run(new string[] { });
            Assert.That(result, Is.EqualTo(0));

            copier.VerifyAll();
        }

        [Test]
        public void FailedCopyCausesNonZeroReturnCode()
        {
            // given
            Mock<ICopier> copier = new Mock<ICopier>(MockBehavior.Strict);
            copier
                .Setup(r => r.Copy(It.Is<Computer>(c => c.Name == "mycomputer"), @"c:\temp\foo.bat", @"\bah\thing.bat"))
                .Throws<InvalidOperationException>();

            // when
            Copy copy = new Copy(copier.Object);

            StringWriter output = new StringWriter();
            ConsoleCommandDispatcher.DispatchCommand(
                copy,
                new[] 
                { 
                    "-source", @"c:\temp\foo.bat",
                    "-target", @"\bah\thing.bat",
                    "-computername", "mycomputer",
                    "-adminpassword", "foobah"
                },
                output);

            int result = copy.Run(new string[] { });
            Assert.That(result, Is.Not.EqualTo(0));

            copier.VerifyAll();
        }
    }
}
