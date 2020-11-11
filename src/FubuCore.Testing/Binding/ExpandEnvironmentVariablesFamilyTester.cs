using System;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ExpandEnvironmentVariablesFamilyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _family = new ExpandEnvironmentVariablesFamily();
            expandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DefaultPath);
            noExpandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DoNotExpand);
        }

        #endregion

        private ExpandEnvironmentVariablesFamily _family;
        private PropertyInfo noExpandProp;
        private PropertyInfo expandProp;

        public class TestSettings
        {
            [ExpandEnvironmentVariables]
            public string DefaultPath { get; set; }

            public string DoNotExpand { get; set; }
        }

        [Test, Ignore("Mono really doesn't like this test.  Alex, can you look at it someday?")]
        public void expand_environment_variables_for_settings_marked_for_expansion()
        {
            string expandedVariable = Environment.GetEnvironmentVariable("SystemRoot");

            var scenario = BindingScenario<TestSettings>.For(x =>
            {
                x.Data(o => o.DefaultPath, "%SystemRoot%\\foo");
            });

            scenario.Model.DefaultPath.ShouldEqual(expandedVariable + @"\foo");
        }

        [Test]
        public void should_match_properties_with_attribute()
        {
            _family.Matches(expandProp).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_properties_without_attribute()
        {
            _family.Matches(noExpandProp).ShouldBeFalse();
        }
    }
}