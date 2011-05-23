using FubuCore.Configuration;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class AppSettingsSettingSourceTester
    {
        [Test]
        public void can_read_in_data_from_the_config_app_settings()
        {
            var source = new AppSettingsSettingSource(SettingCategory.profile);


            var data = source.FindSettingData().Single();

            data.Category.ShouldEqual(SettingCategory.profile);
            data["a"].ShouldEqual("1");
            data["b"].ShouldEqual("2");
            data["c"].ShouldEqual("3");
        }
    }
}