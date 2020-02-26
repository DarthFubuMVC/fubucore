using System;
using System.IO;
using FubuCore.Util.TextWriting;
using NSubstitute;
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

            var writer = Substitute.For<TextWriter>();

            line.Write(writer);

            writer.Received().WriteLine("=====");
        }

        [Test]
        public void write_to_console()
        {
            var line = new DividerLine('=');
            line.Width = 5;

            var writer = Substitute.For<TextWriter>();
            Console.SetOut(writer);

            line.WriteToConsole();

            writer.Received().WriteLine("=====");
        }
    }
}