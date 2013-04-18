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
    public class CopyTests
    {
        [Test]
        public void CanCallCopier()
        {
            // given
            Mock<ICopier> copier = new Mock<ICopier>(MockBehavior.Strict);
            copier
                .Setup(r => r.Copy("mycomputer", "foobah", @"c:\temp\foo.bat", @"\bah\thing.bat"));

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
                .Setup(r => r.Copy("mycomputer", "foobah", @"c:\temp\foo.bat", @"\bah\thing.bat"))
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
