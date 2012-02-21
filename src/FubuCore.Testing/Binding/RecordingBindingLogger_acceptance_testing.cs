using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Reflection;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class RecordingBindingLogger_acceptance_testing
    {
        [Test]
        public void captures_all_properties()
        {
            var report = BindingScenario<Target>.For(x =>
            {
                x.Data(@"
Name=Jeremy
Age=38
EyeColor=Blue
");
            }).Report;

            report.Properties.Select(x => x.Property.Name).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("Age", "EyeColor", "Name");

        }

        [Test]
        public void captures_the_values_used_on_a_property()
        {
            var report = BindingScenario<Target>.For(x =>
            {
                x.Data(@"
Name=Jeremy
Age=38
EyeColor=Blue
");
            }).Report;

            var propReport = report.For(ReflectionHelper.GetProperty<Target>(x => x.Age));
            var value = propReport.Values.Single();
            value.RawKey.ShouldEqual("Age");
            value.RawValue.ShouldEqual("38");
            value.Source.ShouldEqual("in memory");
        }

        [Test]
        public void captures_the_property_binder_and_converter_if_exists()
        {
            var report = BindingScenario<Target>.For(x =>
            {
                x.Data(@"
Name=Jeremy
Age=38
EyeColor=Blue
");
            }).Report;

            var propReport = report.For(ReflectionHelper.GetProperty<Target>(x => x.Age));
            propReport.Binder.ShouldBeOfType<ConversionPropertyBinder>();
            propReport.Converter.ShouldBeOfType<NumericTypeFamily>();
        }

        [Test]
        public void captures_details_for_a_nested_object()
        {
            var report = BindingScenario<ClassThatNestsTarget>.For(x =>
            {
                x.Data(@"
TargetName=Jeremy
TargetAge=38
TargetEyeColor=Blue
");
            }).Report;

            

            var nestedPropReport = report.LastProperty;
            nestedPropReport.Nested.ShouldNotBeNull();

            var age = nestedPropReport.Nested.For<Target>(x => x.Age);
            var value = age.Values.Single();
            value.RawKey.ShouldEqual("TargetAge");
            value.RawValue.ShouldEqual("38");
            value.Source.ShouldEqual("in memory");
        }

        [Test]
        public void log_for_nested_array()
        {
            var report = BindingScenario<ClassWithTargetArray>.For(x =>
            {
                x.Data(@"
Targets[0]Name=Jeremy
Targets[0]Age=38
Targets[0]EyeColor=Blue
Targets[1]Name=Lindsey
Targets[1]Age=29
Targets[1]EyeColor=Hazel


");
            }).Report;


            report.WriteToConsole(true);

            var array = report.For<ClassWithTargetArray>(x => x.Targets);
            array.Elements.ShouldHaveCount(2);

            var age0 = array.Elements.First().For<Target>(x => x.Age);
            age0.Binder.ShouldBeOfType<ConversionPropertyBinder>();
            age0.Converter.ShouldBeOfType<NumericTypeFamily>();
            age0.Values.Single().RawValue.ShouldEqual("38");

            var age1 = array.Elements.Last().For<Target>(x => x.Age);
            age1.Binder.ShouldBeOfType<ConversionPropertyBinder>();
            age1.Converter.ShouldBeOfType<NumericTypeFamily>();
            age1.Values.Single().RawValue.ShouldEqual("29");

        }

        [Test]
        public void log_for_nested_list()
        {
            var report = BindingScenario<ClassWithTargetList>.For(x =>
            {
                x.Data(@"
Targets[0]Name=Jeremy
Targets[0]Age=38
Targets[0]EyeColor=Blue
Targets[1]Name=Lindsey
Targets[1]Age=29
Targets[1]EyeColor=Hazel


");
            }).Report;

            var listReport = report.For<ClassWithTargetList>(x => x.Targets);
            listReport.Elements.ShouldHaveCount(2);

            var age0 = listReport.Elements.First().For<Target>(x => x.Age);
            age0.Binder.ShouldBeOfType<ConversionPropertyBinder>();
            age0.Converter.ShouldBeOfType<NumericTypeFamily>();
            age0.Values.Single().RawValue.ShouldEqual("38");

            var age1 = listReport.Elements.Last().For<Target>(x => x.Age);
            age1.Binder.ShouldBeOfType<ConversionPropertyBinder>();
            age1.Converter.ShouldBeOfType<NumericTypeFamily>();
            age1.Values.Single().RawValue.ShouldEqual("29");

        }

        [Test]
        public void log_for_nested_class_in_array()
        {
            var report = BindingScenario<DeepClass1>.For(x =>
            {
                x.Data(@"
NestedTargets[0]TargetName=Jeremy
NestedTargets[0]TargetAge=38
NestedTargets[0]TargetEyeColor=Blue
NestedTargets[1]TargetName=Lindsey
NestedTargets[1]TargetAge=29
NestedTargets[1]TargetEyeColor=Hazel


");
            }).Report;


            report.WriteToConsole(true);

            var elements = report.For<DeepClass1>(x => x.NestedTargets).Elements;
            elements.Count.ShouldEqual(2);

            var age0 = elements.First().For<ClassThatNestsTarget>(x => x.Target).Nested.For<Target>(x => x.Age);
            age0.Binder.ShouldBeOfType<ConversionPropertyBinder>();
            age0.Converter.ShouldBeOfType<NumericTypeFamily>();
            age0.Values.Single().RawValue.ShouldEqual("38");
            age0.Values.Single().RawKey.ShouldEqual("NestedTargets[0]TargetAge");
        }

        public class ClassWithTargetArray
        {
            public Target[] Targets { get; set; }
        }

        public class ClassWithTargetList
        {
            public IList<Target> Targets { get; set; }
        }

        public class ClassThatNestsTarget
        {
            public Target Target { get; set; }
        }

        public class DeepClass1
        {
            public IEnumerable<ClassThatNestsTarget> NestedTargets { get; set; }
        }

        public class Target
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string EyeColor { get; set; }
        }
    }


}