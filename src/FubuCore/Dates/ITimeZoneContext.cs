using System;

namespace FubuCore.Dates
{
    public interface ITimeZoneContext
    {
        TimeZoneInfo GetTimeZone();
    }
}