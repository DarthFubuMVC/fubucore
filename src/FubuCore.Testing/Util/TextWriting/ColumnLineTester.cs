using System.IO;
using FubuCore.Util.TextWriting;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuCore.Testing.Util.TextWriting
{
    [TestFixture]
    public class ColumnLineTester
    {
        [Test]
        public void write_a_line()
        {
            var set = new ColumnSet(3);
            var line = set.Add("a", "b", "c");

            var writer = new StringWriter();

            line.Write(writer);

            var text = "a*****b*****c\r\n".Replace("*", " ");

            writer.ToString().ShouldEqual(text);
        }

        [Test]
        public void width()
        {
            var set = new ColumnSet(3);
            var line = set.Add("aaaaa", "bbb", "cc");

            // the default padding between columns is 5 spaces
            line.Width.ShouldEqual(20);
        }
    }
}