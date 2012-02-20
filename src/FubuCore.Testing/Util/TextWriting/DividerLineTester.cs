using System;
using System.IO;
using FubuCore.Util.TextWriting;
using NUnit.Framework;
using Rhino.Mocks;

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

            var writer = MockRepository.GenerateMock<TextWriter>();

            line.Write(writer);

            writer.AssertWasCalled(x => x.WriteLine("====="));
        }

        [Test]
        public void write_to_console()
        {
            var line = new DividerLine('=');
            line.Width = 5;

            var writer = MockRepository.GenerateMock<TextWriter>();
            Console.SetOut(writer);

            line.WriteToConsole();

            writer.AssertWasCalled(x => x.WriteLine("====="));
        }
    }
}