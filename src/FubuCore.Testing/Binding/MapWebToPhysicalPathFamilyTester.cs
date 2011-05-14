using System.Reflection;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class MapWebToPhysicalPathFamilyTester
    {
        [SetUp]
        public void SetUp()
        {
            family = new MapWebToPhysicalPathFamily();
            webAppFolder = "DRIVE:\\MapWebToPhysicalPathFamilyTester";
            UrlContext.Stub(webAppFolder);

            expandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DefaultPath);
            noExpandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DoNotExpand);
        }

        private MapWebToPhysicalPathFamily family;
        private PropertyInfo noExpandProp;
        private PropertyInfo expandProp;
        private string webAppFolder;

        public class TestSettings
        {
            [MapWebToPhysicalPath]
            public string DefaultPath { get; set; }

            public string DoNotExpand { get; set; }
        }

        [Test]
        public void resolve_to_full_paths_for_settings_marked_for_local_path_resolution()
        {
            var value = new InMemoryBindingContext().WithPropertyValue("~/App_Data/file.txt");

            object result = family.Build(null, expandProp).Convert(value);
            result.ShouldEqual(webAppFolder + "\\App_Data\\file.txt");
        }

        [Test]
        public void should_match_properties_with_attribute()
        {
            family.Matches(expandProp).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_properties_without_attribute()
        {
            family.Matches(noExpandProp).ShouldBeFalse();
        }
    }
}