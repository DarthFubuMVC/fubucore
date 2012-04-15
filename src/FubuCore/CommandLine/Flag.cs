using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Conversion;

namespace FubuCore.CommandLine
{
    public class Flag : TokenHandlerBase
    {
        private readonly PropertyInfo _property;
        private readonly ObjectConverter _converter;

        public Flag(PropertyInfo property, ObjectConverter converter) : base(property)
        {
            _property = property;
            _converter = converter;
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            if (tokens.NextIsFlagFor(_property))
            {
                var flag = tokens.Dequeue();

                if( tokens.Count == 0 ) throw new InvalidUsageException("No value specified for flag {0}.".ToFormat(flag));

                var rawValue = tokens.Dequeue();
                var value = _converter.FromString(rawValue, _property.PropertyType);

                _property.SetValue(input, value, null);

                return true;
            }


            return false;
        }

        public override string ToUsageDescription()
        {
            var flagAliases = InputParser.ToFlagAliases(_property);

            if (_property.PropertyType.IsEnum)
            {
                var enumValues = Enum.GetNames(_property.PropertyType).Join("|");
                return "[{0} {1}]".ToFormat(flagAliases, enumValues);
            }

            
            return "[{0} <{1}>]".ToFormat(flagAliases, _property.Name.ToLower().TrimEnd('f', 'l','a','g'));
        }
    }
}