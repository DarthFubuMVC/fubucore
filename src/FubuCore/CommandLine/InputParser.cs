using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore.Conversion;
using FubuCore.Reflection;

namespace FubuCore.CommandLine
{
    public static class InputParser
    {
        private static readonly string LONG_FLAG_PREFIX = "--";
        private static readonly Regex LONG_FLAG_REGEX = new Regex("^{0}[^-]+".ToFormat(LONG_FLAG_PREFIX));
        
        private static readonly string SHORT_FLAG_PREFIX = "-";
        private static readonly Regex SHORT_FLAG_REGEX = new Regex("^{0}[^-]+".ToFormat(SHORT_FLAG_PREFIX)); 
        
        private static readonly string FLAG_SUFFIX = "Flag";
        private static readonly ObjectConverter _converter = new ObjectConverter();


        public static List<ITokenHandler> GetHandlers(Type inputType)
        {
            return inputType.GetProperties()
                .Where(prop => prop.CanWrite)
                .Where(prop => !prop.HasAttribute<IgnoreOnCommandLineAttribute>())
                .Select(BuildHandler).ToList();
        }

        public static ITokenHandler BuildHandler(PropertyInfo property)
        {
            if (property.PropertyType != typeof(string) && property.PropertyType.Closes(typeof(IEnumerable<>)))
            {
                return new EnumerableArgument(property, _converter);
            }

            if (!property.Name.EndsWith(FLAG_SUFFIX))
            {
                return new Argument(property, _converter);
            }

            if (property.PropertyType == typeof(bool))
            {
                return new BooleanFlag(property);
            }
            
            return new Flag(property, _converter);
        }

        public static bool IsFlag(string token)
        {
            return  IsShortFlag(token) || IsLongFlag(token);
        }

        public static bool IsShortFlag(string token)
        {
            return SHORT_FLAG_REGEX.IsMatch(token);
        }

        public static bool IsLongFlag(string token)
        {
            return LONG_FLAG_REGEX.IsMatch(token);
        }

        public static bool IsFlagFor(string token, PropertyInfo property)
        {
            return ToFlagAliases(property).Matches(token);
        }

        public static FlagAliases ToFlagAliases(PropertyInfo property)
        {
            var name = property.Name;
            if (name.EndsWith("Flag"))
            {
                name = name.Substring(0, property.Name.Length - 4);
            }

            var oneLetterName = name[0];

            property.ForAttribute<FlagAliasAttribute>(att =>
                                                          {
                                                              name = att.LongAlias;
                                                              oneLetterName = att.OneLetterAlias;
                                                          });
            return new FlagAliases
                       {
                           ShortForm = (SHORT_FLAG_PREFIX + oneLetterName).ToLower(),
                           LongForm = LONG_FLAG_PREFIX + name.ToLower()
                       };
        }

    }
}