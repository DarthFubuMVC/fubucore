using FubuCore.Binding;
using FubuCore.Configuration;
using NUnit.Framework;

namespace FubuCore.Testing.Configuration
{
    [TestFixture, Explicit("Tries to look in the wrong folder for the files. Need to rewrite to look in the right folder for the .config files in FubuCore.Testing")]
    public class SettingsProviderIntegratedTester
    {
        private FolderAppSettingsXmlSource theSource;
        private SettingsProvider theProvider;

        [SetUp]
        public void SetUp()
        {
            var resolver = ObjectResolver.Basic();

            theSource = new FolderAppSettingsXmlSource("Configuration");
            theProvider = new SettingsProvider(resolver, new ISettingsSource[]{theSource});
        }

        [Test]
        public void pull_a_settings_object_with_environment_overrides()
        {
            var settings = theProvider.SettingsFor<OneSettings>();
            settings.Name.ShouldEqual("Max");
            settings.Age.ShouldEqual(37);
        }

        [Test]
        public void pull_a_settings_object_without_environment_overrides()
        {
            var settings = theProvider.SettingsFor<ThreeSettings>();
            settings.Direction.ShouldEqual("North");
            settings.Threshold.ShouldEqual(3);
        }

        [Test]
        public void does_the_substitutions()
        {
            var settings = theProvider.SettingsFor<OneSettings>();
            settings.Server.ShouldEqual("*env-server*");
        }
    }
}