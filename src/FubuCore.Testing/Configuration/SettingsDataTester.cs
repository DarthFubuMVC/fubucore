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
            data.Provenance.ShouldEqual("settings.txt");
            data.Category.ShouldEqual(SettingCategory.profile);

            data.AllKeys.ShouldHaveTheSameElementsAs("A", "B", "C", "D");

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

            data.AllKeys.ShouldHaveTheSameElementsAs("Key", "A.Key");

            data.Get("Key").ShouldEqual("Value1");
            data.Get("A.Key").ShouldEqual("Value2");
        }

        [Test]
        public void read_complex_escaped_value()
        {
            var data = new SettingsData(SettingCategory.core);
            data.Read("DatabaseSettings.ConnectionString=\"Data Source=localhost;Initial Catalog=DovetailDAI;User Id=sa;Password=sa;\"");

            data.AllKeys.ShouldHaveTheSameElementsAs("DatabaseSettings.ConnectionString");

            data.Get("DatabaseSettings.ConnectionString").ShouldEqual("Data Source=localhost;Initial Catalog=DovetailDAI;User Id=sa;Password=sa;");
        }

        [Test]
        public void subset_by_prefix()
        {
            var data = new SettingsData(SettingCategory.core);
            data.With("One.A", "1");
            data.With("One.B", "2");
            data.With("One.C", "1");
            data.With("Two.A", "11");
            data.With("Two.B", "12");
            data.With("Two.C", "13");
            data.With("Two.D", "14");
            data.With("Three.A", "21");
            data.With("Three.B", "22");
            data.With("Three.C", "23");

            var subsetOne = data.SubsetPrefixedBy("One.");
            subsetOne.AllKeys.ShouldHaveTheSameElementsAs("A", "B", "C");
            subsetOne.Get("A").ShouldEqual("1");

            var subsetTwo = data.SubsetPrefixedBy("Two.");
            subsetTwo.AllKeys.ShouldHaveTheSameElementsAs("A", "B", "C", "D");
            subsetTwo.Get("A").ShouldEqual("11");

            var subsetThree = data.SubsetPrefixedBy("Three.");
            subsetThree.AllKeys.ShouldHaveTheSameElementsAs("A", "B", "C");
            subsetThree.Get("A").ShouldEqual("21");
        }

        [Test]
        public void subset_by_prefix_copies_provenance()
        {
            var data = new SettingsData(SettingCategory.core){
                Provenance = Guid.NewGuid().ToString()
            };

            data.SubsetPrefixedBy("a").Provenance.ShouldEqual(data.Provenance);
        }

        [Test]
        public void subset_by_prefix_copies_category()
        {
            var data = new SettingsData(SettingCategory.package);
            data.SubsetPrefixedBy("a").Category.ShouldEqual(data.Category);
        }

        [Test]
        public void subset_by_key_filter()
        {
            var data = new SettingsData(SettingCategory.core);
            data.With("One.A", "1");
            data.With("One.B", "2");
            data.With("One.C", "1");
            data.With("Two.A", "11");
            data.With("Two.B", "12");
            data.With("Two.C", "13");
            data.With("Two.D", "14");
            data.With("Three.A", "21");
            data.With("Three.B", "22");
            data.With("Three.C", "23");

            var subsetA = data.SubsetByKey(key => key.Contains("A"));
            subsetA.AllKeys.ShouldHaveTheSameElementsAs("One.A", "Two.A", "Three.A");
            subsetA.Get("One.A").ShouldEqual("1");


            var subsetOne = data.SubsetByKey(key => key.StartsWith("One"));
            subsetOne.AllKeys.ShouldHaveTheSameElementsAs("One.A", "One.B", "One.C");


        }

        [Test]
        public void subset_by_filter_copies_provenance()
        {
            var data = new SettingsData(SettingCategory.core){
                Provenance = Guid.NewGuid().ToString()
            };

            data.SubsetByKey(key => true).Provenance.ShouldEqual(data.Provenance);
        }

        [Test]
        public void subset_by_key_copies_category()
        {
            var data = new SettingsData(SettingCategory.package);
            data.SubsetByKey(key => true).Category.ShouldEqual(data.Category);
        }
    }
}