using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class TemplateParserTester
    {
        [Test]
        public void should_replace_a_single_variable()
        {
            var template = "this is a {test} template";
            var substitutions = new Dictionary<string, string>
            {
                {"test", "replaced"},
            };

            TemplateParser
                .Parse(template, substitutions)
                .ShouldEqual("this is a replaced template");
        }

        [Test]
        public void should_return_remaining_string_if_variable_not_found()
        {
            var template = "http://testreplacement.com/{optional}";
            var substitiutions = new Dictionary<string, string>();

            TemplateParser
                .Parse(template, substitiutions)
                .ShouldEqual("http://testreplacement.com/");
        }

        [Test]
        public void should_replace_variables_that_are_found_and_replace_unknown_with_nothing()
        {
            var template = "http://testreplacement.com/{replaced}/{still-replaced}/{optional}";
            var substitiutions = new Dictionary<string, string>
            {
                {"replaced", "gotreplaced"},
                {"still-replaced", "stillreplaced"}
            };

            TemplateParser
                .Parse(template, substitiutions)
                .ShouldEqual("http://testreplacement.com/gotreplaced/stillreplaced/");
        }

        [Test]
        public void should_replace_a_single_variable_with_a_dash()
        {
            var template = "this is a {test-name} template";
            var substitutions = new Dictionary<string, string>
            {
                {"test-name", "replaced"},
            };

            TemplateParser
                .Parse(template, substitutions)
                .ShouldEqual("this is a replaced template");
        }


        [Test]
        public void template_is_one_from_the_end()
        {
            var template = "*{db}*";
            var substitutions = new Dictionary<string, string>
            {
                {"db", "blue"},
            };

            TemplateParser.Parse(template, substitutions).ShouldEqual("*blue*");
        }

        [Test]
        public void template_is_at_the_end()
        {
            var template = "**{db}";
            var substitutions = new Dictionary<string, string>
            {
                {"db", "blue"},
            };

            TemplateParser.Parse(template, substitutions).ShouldEqual("**blue");
        }

        [Test]
        public void should_replace_multiple_variables()
        {
            var template = "this {is} a {test} template with {a} few {variables}";
            var substitutions = new Dictionary<string, string>
            {
                {"is", "is"},
                {"test", "replaced"},
                {"a", "more than a"},
                {"variables", "witty tricks."}
            };

            TemplateParser
                .Parse(template, substitutions)
                .ShouldEqual("this is a replaced template with more than a few witty tricks.");
        }

        [Test]
        public void should_replace_values_when_template_begins_with_a_variable()
        {
            var template = "{that} is a {test} template";
            var substitutions = new Dictionary<string, string>
            {
                {"that", "this"},
                {"test", "replaced"},
            };

            TemplateParser
                .Parse(template, substitutions)
                .ShouldEqual("this is a replaced template");
        }

        [Test]
        public void get_substitutions()
        {
            TemplateParser.GetSubstitutions("{that} is a {test} template").ShouldHaveTheSameElementsAs("that", "test");
        }
    }
}