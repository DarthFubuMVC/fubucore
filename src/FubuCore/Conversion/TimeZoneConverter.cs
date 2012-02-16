using System;
using System.ComponentModel;

namespace FubuCore.Conversion
{
    [Description("Converts to a TimeZoneInfo object by calling TimeZoneInfo.FindSystemTimeZoneById(text)")]
    public class TimeZoneConverter : StatelessConverter<TimeZoneInfo>
    {
        protected override TimeZoneInfo convert(string text)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(text);
        }
    }
}