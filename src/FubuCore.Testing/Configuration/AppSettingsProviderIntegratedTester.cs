using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Configuration;

namespace FubuCore.Testing.Configuration
{

    /******************************************
     * 
     * 
     * See the FubuCore.Testing.dll.config file for the data
     * 
     * 
     ******************************************/

    [TestFixture]
    public class AppSettingsProviderIntegratedTester
    {
        private AppSettings theSettings;

        [SetUp]
        public void SetUp()
        {
            theSettings = new AppSettingsProvider(ObjectResolver.Basic())
                .SettingsFor<AppSettings>();

        }

        [Test]
        public void get_value_for()
        {
            AppSettingsProvider.GetValueFor<AppSettings>(x => x.Flag1).ShouldEqual("f1");
        }

        [Test]
        public void can_get_basic_properties()
        {
            theSettings.Flag1.ShouldEqual("f1");
            theSettings.Flag2.ShouldEqual("f2");
        }

        [Test]
        public void can_get_a_nested_object()
        {
            theSettings.Nested.ShouldNotBeNull();
            theSettings.Nested.Flag3.ShouldEqual("f3");
        }

        [Test]
        public void can_build_enumeration_properties()
        {
            theSettings.Nested.Files.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("control", "home");
        }
    }

    public class AppSettings
    {
        public string Flag1 { get; set; }
        public string Flag2 { get; set; }
        public NestedSetting Nested { get; set; }
    }

    public class NestedSetting
    {
        public string Flag3 { get; set; }
        public IEnumerable<AppFile> Files { get; set; }
    }

    public class AppFile
    {
        public string Name { get; set; }
        public string Location { get; set; }
    }
}