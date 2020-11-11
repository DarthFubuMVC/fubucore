using FubuCore.Util.TextWriting;
using NUnit.Framework;

namespace FubuCore.Testing.Util.TextWriting
{
    [TestFixture]
    public class ColumnTester
    {
        [Test]
        public void watch_data_changes_width_left_justification()
        {
            var column = new Column(ColumnJustification.left, 0, 5);
            column.Width.ShouldEqual(5);

            column.WatchData("123");
            column.Width.ShouldEqual(8);

            column.WatchData("12345678");
            column.Width.ShouldEqual(13);

            // shouldn't change
            column.WatchData("123");
            column.Width.ShouldEqual(13);
        }

        [Test]
        public void watch_data_changes_width_right_justification()
        {
            var column = new Column(ColumnJustification.left, 5, 0);
            column.Width.ShouldEqual(5);

            column.WatchData("123");
            column.Width.ShouldEqual(8);

            column.WatchData("12345678");
            column.Width.ShouldEqual(13);

            // shouldn't change
            column.WatchData("123");
            column.Width.ShouldEqual(13);
        }

        [Test]
        public void watch_data_changes_width_padding_on_both_sides()
        {
            var column = new Column(ColumnJustification.left, 3, 4);
            column.Width.ShouldEqual(7);

            column.WatchData("123");
            column.Width.ShouldEqual(10);

            column.WatchData("12345678");
            column.Width.ShouldEqual(15);

            // shouldn't change
            column.WatchData("123");
            column.Width.ShouldEqual(15);
        }

        [Test]
        public void write_left_justified_with_right_padding()
        {
            var column = new Column(ColumnJustification.left, 0, 5);
            column.WatchData("12345");

            column.GetText("123").ShouldEqual("123*******".Replace("*", " "));
        }

        [Test]
        public void write_left_justified_with_left_padding()
        {
            var column = new Column(ColumnJustification.left, 5,0);
            column.WatchData("12345");

            column.GetText("123").ShouldEqual("*****123**".Replace("*", " "));
        }

        [Test]
        public void write_left_justified_with_both_right_and_left_padding()
        {
            var column = new Column(ColumnJustification.left, 2, 2);
            column.WatchData("12345");

            column.GetText("123").ShouldEqual("**123****".Replace("*", " "));
        }



        [Test]
        public void write_right_justified_with_right_padding()
        {
            var column = new Column(ColumnJustification.right, 0, 5);
            column.WatchData("12345");

            column.GetText("123").ShouldEqual("**123*****".Replace("*", " "));
        }

        [Test]
        public void write_right_justified_with_left_padding()
        {
            var column = new Column(ColumnJustification.right, 5, 0);
            column.WatchData("12345");

            column.GetText("123").ShouldEqual("*******123".Replace("*", " "));
        }

        [Test]
        public void write_right_justified_with_both_left_and_right_padding()
        {
            var column = new Column(ColumnJustification.right, 2, 2);
            column.WatchData("12345");

            column.GetText("123").ShouldEqual("****123**".Replace("*", " "));
        }

    
    }
}