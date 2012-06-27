using System;

namespace FubuCore.Dates
{
    public class MachineTimeZoneContext : ITimeZoneContext
    {
        public TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.Local;
        }
    }
}