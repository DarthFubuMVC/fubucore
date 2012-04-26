using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Conversion;

namespace FubuCore.CommandLine
{
    public class EnumerableFlag : Flag
    {
        private readonly ObjectConverter _converter;
        private readonly PropertyInfo _property;

        public EnumerableFlag(PropertyInfo property, ObjectConverter converter)
            : base(property, converter)
        {
            _property = property;
            _converter = converter;
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            var elementType = _property.PropertyType.GetGenericArguments().First();
            var list = typeof(List<>).CloseAndBuildAs<IList>(elementType);

            var wasHandled = false;

            var flag = "";
            if (tokens.NextIsFlagFor(_property))
            {
                flag = tokens.Dequeue();
                while (tokens.Count > 0 && !tokens.NextIsFlag())
                {
                    var value = _converter.FromString(tokens.Dequeue(), elementType);
                    list.Add(value);

                    wasHandled = true;
                }

                if(!wasHandled)
                {
                    throw new InvalidUsageException("No values specified for flag {0}.".ToFormat(flag));
                }

                _property.SetValue(input, list, null);
            }

            return wasHandled;
        }

        public override string ToUsageDescription()
        {
            var flagAliases = InputParser.ToFlagAliases(_property);

            return "[{0} [<{1}1 {1}2 {1}3 ...>]]".ToFormat(flagAliases, _property.Name.ToLower().TrimEnd('f', 'l', 'a', 'g'));
            
        }
    }
}