using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.CommandLine;
using FubuCore.Conversion;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore.Reflection;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class FlagTester
    {
        private Flag forProp(Expression<Func<FlagTarget, object>> expression)
        {
            return new Flag(expression.ToAccessor().InnerProperty, new ObjectConverter());
        }

        [Test]
        public void to_usage_description_for_a_simple_non_aliased_field()
        {
            forProp(x => x.NameFlag).ToUsageDescription().ShouldEqual("[-n, --name <name>]");
        }

        [Test]
        public void to_usage_description_for_a_simple_aliased_field()
        {
            forProp(x => x.AliasFlag).ToUsageDescription().ShouldEqual("[-a, --aliased <alias>]");
        }

        [Test]
        public void to_usage_description_for_an_enum_field()
        {
            forProp(x => x.EnumFlag).ToUsageDescription().ShouldEqual("[-e, --enum red|blue|green]");
        }

        [Test]
        public void flag_is_always_optional_if_no_attribute_stating_otherwise()
        {
            forProp(x => x.NameFlag).OptionalForUsage("a").ShouldBeTrue();
            forProp(x => x.NameFlag).OptionalForUsage("b").ShouldBeTrue();
            forProp(x => x.NameFlag).OptionalForUsage("c").ShouldBeTrue();
        }

        [Test]
        public void flag_is_selectively_optional_if_attribute_states_specific_usages()
        {
            forProp(x => x.EnumFlag).OptionalForUsage("a").ShouldBeTrue();
            forProp(x => x.EnumFlag).OptionalForUsage("b").ShouldBeTrue();
            forProp(x => x.EnumFlag).OptionalForUsage("c").ShouldBeFalse();
        }

        [Test]
        public void should_provide_useful_error_message_when_no_value_provided()
        {
            typeof(InvalidUsageException).ShouldBeThrownBy(() =>
                forProp(x => x.AliasFlag).Handle(new FlagTarget(), new Queue<string>(new[] { "-a" })))
                .Message.ShouldEqual("No value specified for flag -a.");
        }

        [Test]
        public void should_catch_invalid_enum_value()
        {
            typeof(ArgumentException).ShouldBeThrownBy(() =>
                forProp(x => x.EnumFlag).Handle(new FlagTarget(), new Queue<string>(new[] { "-e", "x" })));
        }
    }

    public enum FlagEnum
    {
        red, blue, green
    }

    public class FlagTarget
    {
        public string NameFlag { get; set; }

        [ValidUsage("a", "b")]
        public FlagEnum EnumFlag { get; set; }

        [FlagAlias("aliased", 'a')]
        public string AliasFlag { get; set;}
    }
}