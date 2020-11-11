using System;
using System.IO;
using FubuCore.Util.TextWriting;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Util.TextWriting
{
    [TestFixture]
    public class DividerLineTester
    {
        [Test]
        public void write()
        {
            var line = new DividerLine('=');
            line.Width = 5;

            var writer = new Mock<TextWriter>();

            line.Write(writer.Object);

            writer.Verify(x => x.WriteLine("====="));
        }

        [Test]
        public void write_to_console()
        {
            var line = new DividerLine('=');
            line.Width = 5;

            var writer = new Mock<TextWriter>();
            Console.SetOut(writer.Object);

            line.WriteToConsole();

            writer.Verify(x => x.WriteLine("====="));
        }
    }
}