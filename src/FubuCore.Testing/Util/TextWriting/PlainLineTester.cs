using System;
using System.IO;
using FubuCore.Util.TextWriting;
using NUnit.Framework;
using FubuTestingSupport;
using NSubstitute;

namespace FubuCore.Testing.Util.TextWriting
{
    [TestFixture]
    public class PlainLineTester
    {
        [Test]
        public void width_is_the_text_width()
        {
            var text = "a bunch of text";
            var line = new PlainLine(text);

            line.Width.ShouldEqual(text.Length);
        }

        [Test]
        public void write()
        {
            var text = "a bunch of text";
            var line = new PlainLine(text);

            var writer = Substitute.For<TextWriter>();

            line.Write(writer);

            writer.Received().WriteLine(text);
        }

        [Test]
        public void write_to_console()
        {
            var writer = Substitute.For<TextWriter>();
            Console.SetOut(writer);

            var text = "a bunch of text";
            var line = new PlainLine(text);

            line.Write(writer);

            writer.Received().WriteLine(text);
        }
    }
}