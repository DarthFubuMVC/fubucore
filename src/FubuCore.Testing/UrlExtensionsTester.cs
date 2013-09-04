using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class UrlExtensionsTester
    {
        [Test]
        public void append_url()
        {
            "foo".AppendUrl("bar").ShouldEqual("foo/bar");
            "foo".AppendUrl("/bar").ShouldEqual("foo/bar");
            "foo/".AppendUrl("bar").ShouldEqual("foo/bar");
            "foo/".AppendUrl("/bar").ShouldEqual("foo/bar");
        }

        [Test]
        public void child_url()
        {
            "foo/bar".ChildUrl().ShouldEqual("bar");
            "foo/bar/more".ChildUrl().ShouldEqual("bar/more");
        }

        [Test]
        public void parent_url()
        {
            "foo/bar".ParentUrl().ShouldEqual("foo");
            "foo/bar/more".ParentUrl().ShouldEqual("foo/bar");
        }

        [Test]
        public void move_up()
        {
            "foo/bar".MoveUp().ShouldEqual("bar");
            "foo/bar/more".MoveUp().ShouldEqual("bar/more");

            "foo".MoveUp().ShouldEqual("");
            "".MoveUp().ShouldEqual("");
        }
    }
}