using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.CommandLine;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore.Reflection;


namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class InputParserTester
    {
        private InputModel theInput;

        [SetUp]
        public void SetUp()
        {
            theInput = new InputModel();
        }


        private ITokenHandler handlerFor(Expression<Func<InputModel, object>> expression)
        {
            var property = expression.ToAccessor().InnerProperty;
            return InputParser.BuildHandler(property);
        }

        private bool handle(Expression<Func<InputModel, object>> expression, params string[] args)
        {
            var queue = new Queue<string>(args);

            var handler = handlerFor(expression);

            return handler.Handle(theInput, queue);
        }

        [Test]
        public void the_handler_for_a_normal_property_not_marked_as_flag()
        {
            handlerFor(x => x.File).ShouldBeOfType<Argument>();
        }

        [Test]
        public void the_handler_for_an_enumeration_property_marked_as_flag()
        {
            handlerFor(x => x.ColorFlag).ShouldBeOfType<Flag>();
        }

        [Test]
        public void the_handler_for_an_enumeration_property_not_marked_as_flag()
        {
            handlerFor(x => x.Color).ShouldBeOfType<Argument>();
        }

        [Test]
        public void the_handler_for_a_property_suffixed_by_flag()
        {
            handlerFor(x => x.OrderFlag).ShouldBeOfType<Flag>();
        }

        [Test]
        public void the_handler_for_a_boolean_flag()
        {
            handlerFor(x => x.TrueFalseFlag).ShouldBeOfType<BooleanFlag>();
        }

        [Test]
        public void handler_for_an_array()
        {
            handlerFor(x => x.Ages).ShouldBeOfType<EnumerableArgument>();
        }

        [Test]
        public void get_the_long_flag_name_for_a_property()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.OrderFlag);
            InputParser.ToFlagAliases(property).LongForm.ShouldEqual("--order");
        }

        [Test]
        public void the_long_name_should_allow_for_dashes()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.TrueOrFalseFlag);
            InputParser.ToFlagAliases(property).LongForm.ShouldEqual("--true-or-false");
        }

        [Test]
        public void the_long_name_should_allow_overriding()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.MakeSuckModeFlag);
            InputParser.ToFlagAliases(property).LongForm.ShouldEqual("--makesuckmode");
        }

        [Test]
        public void get_the_short_flag_name_for_a_property()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.OrderFlag);
            InputParser.ToFlagAliases(property).ShortForm.ShouldEqual("-o");
        }
        
        [Test]
        public void get_the_long_flag_name_for_a_property_with_an_alias()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.AliasedFlag);
            InputParser.ToFlagAliases(property).LongForm.ShouldEqual("--aliased");
        }

        [Test]
        public void get_the_short_flag_name_for_a_property_with_an_alias()
        {
            var property = ReflectionHelper.GetProperty<InputModel>(x => x.AliasedFlag);
            InputParser.ToFlagAliases(property).ShortForm.ShouldEqual("-a");
        }

        [Test]
        public void boolean_flag_does_not_catch()
        {
            handle(x => x.TrueFalseFlag, "nottherightthing").ShouldBeFalse();
            theInput.TrueFalseFlag.ShouldBeFalse();
        }

        [Test]
        public void boolean_flag_long_form_should_be_case_insensitive()
        {
            handle(x => x.TrueFalseFlag, "--True-False").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }

        [Test]
        public void boolean_flag_does_catch_2()
        {
            handle(x => x.TrueFalseFlag, "--true-false").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }

        [Test]
        public void enumerable_argument()
        {
            handle(x => x.Ages, "1", "2", "3").ShouldBeTrue();
            theInput.Ages.ShouldHaveTheSameElementsAs(1, 2, 3);
        }

        [Test]
        public void enumeration_argument()
        {
            handle(x => x.Color, "red").ShouldBeTrue();
            theInput.Color.ShouldEqual(Color.red);
        }

        [Test]
        public void enumeration_argument_2()
        {
            handle(x => x.Color, "green").ShouldBeTrue();
            theInput.Color.ShouldEqual(Color.green);
        }

        [Test]
        public void enumeration_flag_negative()
        {
            handle(x => x.ColorFlag, "green").ShouldBeFalse();
        }

        [Test]
        public void enumeration_flag_positive()
        {
            handle(x => x.ColorFlag, "--color", "blue").ShouldBeTrue();
            theInput.ColorFlag.ShouldEqual(Color.blue);
        }
        
        [Test]
        public void string_argument()
        {
            handle(x => x.File, "the file").ShouldBeTrue();
            theInput.File.ShouldEqual("the file");
        }

        [Test]
        public void int_flag_does_not_catch()
        {
            handle(x => x.OrderFlag, "not order flag").ShouldBeFalse();
            theInput.OrderFlag.ShouldEqual(0);
        }

        [Test]
        public void int_flag_catches()
        {
            handle(x => x.OrderFlag, "--order", "23").ShouldBeTrue();
            theInput.OrderFlag.ShouldEqual(23);
        }

        private InputModel build(params string[] tokens)
        {
            var queue = new Queue<string>(tokens);
            var graph = new UsageGraph("fubu",typeof (InputCommand));

            return (InputModel) graph.BuildInput(queue);
        }

        [Test]
        public void integrated_test_arguments_only()
        {
            var input = build("file1", "red");
            input.File.ShouldEqual("file1");
            input.Color.ShouldEqual(Color.red);

            // default is not touched
            input.OrderFlag.ShouldEqual(0);
        }

        [Test]
        public void integrated_test_with_mix_of_flags()
        {
            var input = build("file1", "--color", "green", "blue", "--order", "12");
            input.File.ShouldEqual("file1");
            input.Color.ShouldEqual(Color.blue);
            input.ColorFlag.ShouldEqual(Color.green);
            input.OrderFlag.ShouldEqual(12);
        }

        [Test]
        public void integrated_test_with_a_boolean_flag()
        {
            var input = build("file1", "blue", "--true-false");
            input.TrueFalseFlag.ShouldBeTrue();

            build("file1", "blue").TrueFalseFlag.ShouldBeFalse();
        }

        [Test]
        public void long_flag_with_dashes_should_pass()
        {
            var input = build("file1", "blue", "--herp-derp");
            input.HerpDerpFlag.ShouldBeTrue();

            build("file1", "blue").HerpDerpFlag.ShouldBeFalse();
        }

        [Test]
        public void isflag_should_match_on_double_hyphen()
        {
            InputParser.IsFlag("--f").ShouldBeTrue();
        }

        [Test]
        public void isflag_should_not_match_without_double_hyphen()
        {
            InputParser.IsFlag("f").ShouldBeFalse();
        }

        [Test]
        public void boolean_short_flag_does_not_catch()
        {
            handle(x => x.TrueFalseFlag, "-f").ShouldBeFalse();
            theInput.TrueFalseFlag.ShouldBeFalse();
        }

        [Test]
        public void boolean_short_flag_does_catch()
        {
            handle(x => x.TrueFalseFlag, "-t").ShouldBeTrue();
            theInput.TrueFalseFlag.ShouldBeTrue();
        }

        [Test]
        public void boolean_short_flag_case_uppercase()
        {

            var input = build("file1", "blue", "-T");
            input.TrueFalseFlag.ShouldBeFalse();
            input.TrueOrFalseFlag.ShouldBeTrue();
        }

        [Test]
        public void boolean_short_flag_case_lowercase()
        {

           var  input = build("file1", "blue", "-t");
            input.TrueFalseFlag.ShouldBeTrue();
            input.TrueOrFalseFlag.ShouldBeFalse();
        }

        [Test]
        public void boolean_short_flag_case_both()
        {
            var input = build("file1", "blue", "-t", "-T");
            input.TrueFalseFlag.ShouldBeTrue();
            input.TrueOrFalseFlag.ShouldBeTrue();
        }

        [Test]
        public void integration_test_with_enumerable_flags()
        {
            var input = build("file1", "blue", "-t", "-s", "suck", "fail", "-T");
            input.TrueFalseFlag.ShouldBeTrue();
            input.TrueOrFalseFlag.ShouldBeTrue();
            input.SillyFlag.ShouldHaveTheSameElementsAs("suck","fail");
        }

        [Test]
        public void enumeration_short_flag_negative()
        {
            handle(x => x.ColorFlag, "green").ShouldBeFalse();
        }

        [Test]
        public void enumeration_short_flag_positive()
        {
            handle(x => x.ColorFlag, "-c", "blue").ShouldBeTrue();
            theInput.ColorFlag.ShouldEqual(Color.blue);
        }

        [Test]
        public void IsFlag_should_match_for_short_flag()
        {
            InputParser.IsFlag("-x").ShouldBeTrue();
        }

        [Test]
        public void IsFlag_should_match_for_long_flag()
        {
            InputParser.IsFlag("--xerces").ShouldBeTrue();
        }

        [Test]
        public void IsFlag_negative()
        {
            InputParser.IsFlag("x").ShouldBeFalse();
        }

        [Test]
        public void IsFlag_negative_2()
        {
            InputParser.IsFlag("---x").ShouldBeFalse();
        }

        [Test]
        public void IsShortFlag_should_match_for_short_flag()
        {
            InputParser.IsShortFlag("-x").ShouldBeTrue();
        }

        [Test]
        public void IsShortFlag_should_not_match_for_long_flag()
        {
            InputParser.IsShortFlag("--xerces").ShouldBeFalse();
        }

        [Test]
        public void IsLongFlag_should_not_match_for_short_flag()
        {
            InputParser.IsLongFlag("-x").ShouldBeFalse();
        }

        [Test]
        public void IsLongFlag_should_match_for_long_flag()
        {
            InputParser.IsLongFlag("--xerces").ShouldBeTrue();
        }

        [Test]
        public void IsLongFlag_with_dashes_should_match_for_long_flag()
        {
            InputParser.IsLongFlag("--herp-derp").ShouldBeTrue();
        }

        [Test]
        public void IsLongFlag_should_not_match_for_triple_long_flag()
        {
            InputParser.IsLongFlag("---xerces").ShouldBeFalse();
        }

        [Test]
        public void complex_usage_smoketest()
        {
            new UsageGraph("derp", typeof(InputCommand)).WriteUsages();
        }
      
    }


    public enum Color
    {
        red,
        green,
        blue
    }

    public class InputModel
    {
        [RequiredUsage("default", "ages")]
        public string File { get; set; }
        public Color ColorFlag { get; set; }

        [RequiredUsage("default", "ages")]
        public Color Color { get; set; }
        public int OrderFlag { get; set; }
        public bool TrueFalseFlag { get; set; }
        [FlagAlias('T')]
        public bool TrueOrFalseFlag { get; set; }

        public IEnumerable<string> SillyFlag { get; set; } 

        public bool HerpDerpFlag { get; set; }

        [FlagAlias("makesuckmode")]
        public bool MakeSuckModeFlag { get; set; }

        [RequiredUsage("ages")]
        public IEnumerable<int> Ages { get; set; }

        [FlagAlias("aliased", 'a')]
        public string AliasedFlag { get; set; }
    }

    [Usage("default", "default")]
    [Usage("ages", "ages")]
    public class InputCommand : FubuCommand<InputModel>
    {
        public override bool Execute(InputModel input)
        {
            throw new NotImplementedException();
        }
    }
}