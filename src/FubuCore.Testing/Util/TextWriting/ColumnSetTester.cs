using System;
using FubuCore.Util.TextWriting;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuCore.Testing.Util.TextWriting
{
    [TestFixture]
    public class ColumnSetTester
    {
        [Test]
        public void create_set_by_count()
        {
            var set = new ColumnSet(3);

            set.Columns.ShouldHaveTheSameElementsAs(
                new Column(ColumnJustification.left, 0, 5),
                new Column(ColumnJustification.left, 0, 5),
                new Column(ColumnJustification.left, 0, 0)
                );
        }

        [Test]
        public void adding_a_line_with_the_wrong_counts_throws_exception()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                var set = new ColumnSet(3);
                set.Add("a", "b");
            });
        }
    }
}