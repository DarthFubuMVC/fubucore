using System;

namespace FubuCore.Dates
{
    public static class DateTimeExtensions
    {
        public static Date ToDate(this DateTime time)
        {
            return new Date(time);
        }

        public static Date FirstDayOfMonth(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, 1).ToDate();
        }

        public static Date LastDayOfMonth(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, 1).AddMonths(1).AddDays(-1).ToDate();
        }
    }
}