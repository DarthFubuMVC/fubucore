using System;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class SettingsDataTester
    {
        [Test]
        public void read_settings_data_from_a_file()
        {
            new FileSystem().AlterFlatFile("settings.txt", list =>
            {
                list.Clear();
                list.Add("A=1");
                list.Add("B=2");
                list.Add("C=3");
                list.Add("D=4");
            });

            var data = SettingsData.ReadFromFile(SettingCategory.profile, "settings.txt");
            data.Name.ShouldEqual("settings.txt");
            data.Category.ShouldEqual(SettingCategory.profile);

            data.GetKeys().ShouldHaveTheSameElementsAs("A", "B", "C", "D");

            data.Get("A").ShouldEqual("1");
        }

        [Test]
        public void reading_an_entry_with_a_key_but_no_value_should_bork()
        {
            Exception<Exception>.ShouldBeThrownBy(() =>
            {
                var data = new SettingsData(SettingCategory.core);
                data.Read("Key");
            });
        }
        
        [Test]
        public void read_text()
        {
            var data = new SettingsData(SettingCategory.core);
            data.Read("Key=Value1");
            data.Read("A.Key=Value2");

            data.GetKeys().ShouldHaveTheSameElementsAs("Key", "A.Key");

            data.Get("Key").ShouldEqual("Value1");
            data.Get("A.Key").ShouldEqual("Value2");
        }

        [Test]
        public void read_complex_escaped_value()
        {
            var data = new SettingsData(SettingCategory.core);
            data.Read("DatabaseSettings.ConnectionString=\"Data Source=localhost;Initial Catalog=DovetailDAI;User Id=sa;Password=sa;\"");

            data.GetKeys().ShouldHaveTheSameElementsAs("DatabaseSettings.ConnectionString");

            data.Get("DatabaseSettings.ConnectionString").ShouldEqual("Data Source=localhost;Initial Catalog=DovetailDAI;User Id=sa;Password=sa;");
        }



    }
}