using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FubuCore.Conversion
{
    public class TimeSpanConverter : StatelessConverter<TimeSpan>
    {
        private const string TIMESPAN_PATTERN =
            @"
(?<quantity>\d+     # quantity is expressed as some digits
(\.\d+)?)           # optionally followed by a decimal point and more digits
\s*                 # optional whitespace
(?<units>\w+)       # units is expressed as a word";


        public static TimeSpan GetTimeSpan(string timeString)
        {
            Match match = Regex.Match(timeString, TIMESPAN_PATTERN, RegexOptions.IgnorePatternWhitespace);
            if (!match.Success)
            {
                return TimeSpan.Parse(timeString);
            }

            double number = double.Parse(match.Groups["quantity"].Value);
            string units = match.Groups["units"].Value.ToLower();
            switch (units)
            {
                case "s":
                case "second":
                case "seconds":
                    return TimeSpan.FromSeconds(number);
                case "m":
                case "minute":
                case "minutes":
                    return TimeSpan.FromMinutes(number);

                case "h":
                case "hour":
                case "hours":
                    return TimeSpan.FromHours(number);

                case "d":
                case "day":
                case "days":
                    return TimeSpan.FromDays(number);
            }

            throw new ApplicationException("Time periods must be expressed in seconds, minutes, hours, or days.");
        }

        protected override TimeSpan convert(string text)
        {
            return GetTimeSpan(text);
        }
    }
}