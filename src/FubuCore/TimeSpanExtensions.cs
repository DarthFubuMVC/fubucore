using System;
using FubuCore.Conversion;

namespace FubuCore
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Values are 0 to 2359
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static TimeSpan ToTime(this int minutes)
        {
            var text = minutes.ToString().PadLeft(4, '0');
            return text.ToTime();
        }

        public static TimeSpan ToTime(this string timeString)
        {
            return TimeSpanConverter.GetTimeSpan(timeString);
        }


        public static TimeSpan Minutes(this int number)
        {
            return new TimeSpan(0, 0, number, 0);
        }

        public static TimeSpan Hours(this int number)
        {
            return new TimeSpan(0, number, 0, 0);
        }

        public static TimeSpan Days(this int number)
        {
            return new TimeSpan(number, 0, 0, 0);
        }

        public static TimeSpan Seconds(this int number)
        {
            return new TimeSpan(0, 0, number);
        }
    }
}