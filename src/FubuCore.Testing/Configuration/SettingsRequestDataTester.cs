using System;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuCore.Testing.Configuration
{
    [TestFixture]
    public class SettingsRequestDataTester
    {
        [Test]
        public void ForValue_miss()
        {
            var core1 = new SettingsData(SettingCategory.core).With("key1", "core1");

            var requestData = new SettingsRequestData(new SettingsData[]{core1});

            var action = MockRepository.GenerateMock<Action<string, string>>();
            requestData.ForValue("key2", action).ShouldBeFalse();

            action.AssertWasNotCalled(x => x.Invoke(null, null), x => x.IgnoreArguments());
        }

        [Test]
        public void ForValue_hit()
        {
            var core1 = new SettingsData(SettingCategory.core).With("key1", "core1");

            var requestData = new SettingsRequestData(new SettingsData[] { core1 });

            var action = MockRepository.GenerateMock<Action<string, string>>();
            requestData.ForValue("key1", action).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke("key1", "core1"));
        }


        [Test]
        public void contains_key_negative()
        {
            var core1 = new SettingsData(SettingCategory.core).With("key1", "core1");

            var environment = new SettingsData(SettingCategory.environment).With("key1", "environment1");

            var request = SettingsRequestData.For(core1, environment);

            request.Has("notKey").ShouldBeFalse();
        }

        [Test]
        public void contains_key_positive()
        {
            var core1 = new SettingsData(SettingCategory.core).With("key1", "core1");
            var environment = new SettingsData(SettingCategory.environment).With("key1", "environment1");

            var request = SettingsRequestData.For(core1, environment);  

            request.Has("key1").ShouldBeTrue();
        }

        

        [Test]
        public void environment_has_priority_over_core_when_resolving_data()
        {
            var core1 = new SettingsData(SettingCategory.core).With("key1", "core1");

            var environment = new SettingsData(SettingCategory.environment).With("key1", "environment1");

            var request = SettingsRequestData.For(core1, environment);
            request.Value("key1").ShouldEqual(environment.ReadProperty("key1"));
        }

        [Test]
        public void package_has_priority_over_core_when_resolving_data()
        {
            var core1 = new SettingsData(SettingCategory.core).With("key1", "core1");

            var package = new SettingsData(SettingCategory.package).With("key1", "environment1");

            var request = SettingsRequestData.For(core1, package);
            request.Value("key1").ShouldEqual(package.ReadProperty("key1"));
        }

        [Test]
        public void can_find_a_value_in_multiple_sources()
        {
            var core1 = new SettingsData().With("key1", "val1");
            var core2 = new SettingsData().With("key2", "val2");
            var core3 = new SettingsData().With("key3", "val3");

            var request = SettingsRequestData.For(core1, core2, core3);

            request.Value("key1").ShouldEqual("val1");
            request.Value("key2").ShouldEqual("val2");
            request.Value("key3").ShouldEqual("val3");
        }

        [Test]
        public void value_CPS_style_with_multiple_sources_with_a_match()
        {
            var core1 = new SettingsData().With("key1", "val1");
            var core2 = new SettingsData().With("key2", "val2");
            var core3 = new SettingsData().With("key3", "val3");

            var request = SettingsRequestData.For(core1, core2, core3);

            var action = MockRepository.GenerateMock<Action<object>>();

            request.Value("key2", source => action(source.RawValue)).ShouldBeTrue();

            action.AssertWasCalled(x => x.Invoke("val2"));
        }

        [Test]
        public void value_CPS_style_with_multiple_source_with_no_match()
        {
            var core1 = new SettingsData().With("key1", "val1");
            var core2 = new SettingsData().With("key2", "val2");
            var core3 = new SettingsData().With("key3", "val3");

            var request = SettingsRequestData.For(core1, core2, core3);

            var action = MockRepository.GenerateMock<Action<object>>();

            request.Value("missing key", action).ShouldBeFalse();

            action.AssertWasNotCalled(x => x.Invoke(null), x => x.IgnoreArguments());
        }

        [Test]
        public void has_any_value_prefixed_with_key()
        {
            var core1 = new SettingsData().With("One.key1", "val1");
            var core2 = new SettingsData().With("Two.key2", "val2");
            var core3 = new SettingsData().With("Three.key3", "val3");

            var request = SettingsRequestData.For(core1, core2, core3);

            request.HasChildRequest("One").ShouldBeTrue();
            request.HasChildRequest("Two").ShouldBeTrue();
            request.HasChildRequest("Three").ShouldBeTrue();
            request.HasChildRequest("NotInTheRequestDataAnywhere").ShouldBeFalse();
        }
    }
}