using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FubuCore.Util;

namespace FubuCore
{
    public static class TemplateParser
    {
        private static readonly string TemplateGroup;
        private static readonly Regex TemplateExpression;

        static TemplateParser()
        {
            TemplateGroup = "Template";
            TemplateExpression = new Regex(@"\{(?!\{)(?<" + TemplateGroup + @">[A-Za-z0-9_-]+)\}(?!\})", RegexOptions.Compiled);
        }

        public static string Parse(string template, IDictionary<string, string> substitutions)
        {
            var values = new DictionaryKeyValues(substitutions);

            return Parse(template, values);
        }

        public static bool ContainsTemplates(string template)
        {
            return TemplateExpression.Matches(template).Count > 0;
        }

        public static IEnumerable<string> GetSubstitutions(string template)
        {
            var matches = TemplateExpression.Matches(template);
            foreach (Match match in matches)
            {
                yield return match.Groups[TemplateGroup].Value;
            }
        }

        public static string Parse(string template, IKeyValues values)
        {
            while(ContainsTemplates(template))
            {
                template = parse(template, values);
            }

            template = nowFlattenDoubleCurlies(template);
            return template;
        }

        private static string nowFlattenDoubleCurlies(string template)
        {
            return template.Replace("{{", "{").Replace("}}", "}");
        }

        static string parse(string template, IKeyValues values)
        {
            var matches = TemplateExpression.Matches(template);
            if (matches.Count == 0) return template;

            var lastIndex = 0;
            var builder = new StringBuilder();
            foreach (Match match in matches)
            {
                var key = match.Groups[TemplateGroup].Value;
                if ((lastIndex == 0 || match.Index > lastIndex) && values.Has(key))
                {
                    builder.Append(template.Substring(lastIndex, match.Index - lastIndex));
                    builder.Append(values.Get(key));
                }

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < template.Length)
            {
                builder.Append(template.Substring(lastIndex, template.Length - lastIndex));
            }

            return builder.ToString();
        }
    }
}