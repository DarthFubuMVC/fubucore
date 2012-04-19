using System.Diagnostics;
using System.IO;
using FubuCore.Util.TextWriting;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Util.TextWriting
{
    [TestFixture]
    public class TextReportTester
    {
        [Test]
        public void simple_dividers_and_text()
        {
            var report = new TextReport();
            report.AddDivider('=');
            report.AddText("the title of this");
            report.AddDivider('=');

            var writer = new StringWriter();
        
            report.Write(writer);

            writer.ToString().ShouldEqualWithLineEndings(@"
=================
the title of this
=================
");
        }

        [Test]
        public void simple_dividers_and_text_2()
        {
            var report = new TextReport();
            report.AddDivider('=');
            report.AddText("the title of this");
            report.AddDivider('=');
            report.AddText("some more stuff that is longer");

            var writer = new StringWriter();

            report.Write(writer);

            writer.ToString().ShouldEqualWithLineEndings(@"
==============================
the title of this
==============================
some more stuff that is longer
");
        }

        [Test]
        public void add_dividers_and_columns()
        {
            var report = new TextReport();
            report.AddDivider('=');
            report.StartColumns(3);
            report.AddText("This is the header");
            report.AddDivider('=');

            report.AddColumnData("a1", "b1", "c1");
            report.AddColumnData("a2", "b2", "c2");
            report.AddColumnData("a3", "b3", "c3");
            report.AddDivider('=');

            var writer = new StringWriter();

            report.Write(writer);

            Debug.WriteLine(writer.ToString());

            writer.ToString().ShouldEqualWithLineEndings(@"
==================
This is the header
==================
a1     b1     c1
a2     b2     c2
a3     b3     c3
==================
");
        }




        [Test]
        public void add_dividers_and_jagged_columns()
        {
            var report = new TextReport();
            report.AddDivider('=');
            report.StartColumns(3);
            report.AddText("This is the header");
            report.AddDivider('=');

            report.AddColumnData("a1***", "b1", "c1");
            report.AddColumnData("a2", "b2***", "c2");
            report.AddColumnData("a3", "b3", "c3***");
            report.AddDivider('=');

            var writer = new StringWriter();

            report.Write(writer);

            Debug.WriteLine(writer.ToString());

            writer.ToString().ShouldEqualWithLineEndings(@"
=========================
This is the header
=========================
a1***     b1        c1   
a2        b2***     c2   
a3        b3        c3***
=========================
");
        }


    }
}