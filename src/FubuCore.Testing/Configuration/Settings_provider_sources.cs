using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class Settings_provider_sources
    {
        private SettingsProvider theProvider;
        private TestSettingsSource _settingsSource;


        public class TestSettingsSource : ISettingsSource
        {
            public int FindOccurrenceCount { get; set; }

            public IEnumerable<SettingsData> FindSettingData()
            {
                FindOccurrenceCount += 1;
                return new[] {new SettingsData(SettingCategory.profile)};
            }
        }

        [SetUp]
        public void SetUp()
        {
            var resolver = ObjectResolver.Basic();
            _settingsSource = new TestSettingsSource();
            theProvider = new SettingsProvider(resolver, new[] { _settingsSource });
        }

        [Test]
        public void should_only_find_their_settings_once()
        {
            theProvider.SettingsFor<OneSettings>();

            _settingsSource.FindOccurrenceCount.ShouldEqual(1);
        }
    }
}