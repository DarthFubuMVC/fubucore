using System;
using System.ComponentModel;
using FubuCore.Descriptions;

namespace FubuCore.Conversion
{
    [Description("Preprocesses 'NULL' or 'EMPTY' as string values during conversion")]
    public class StringConverterStrategy : StatelessConverter<string>
    {
        public const string EMPTY = "EMPTY";
        public const string BLANK = "BLANK";

        protected override string convert(string text)
        {
            var stringValue = text;
            if (stringValue == BLANK || stringValue == EMPTY)
            {
                return string.Empty;
            }

            if (stringValue == ObjectConverter.NULL)
            {
                return null;
            }

            return stringValue;
        }

    }
}