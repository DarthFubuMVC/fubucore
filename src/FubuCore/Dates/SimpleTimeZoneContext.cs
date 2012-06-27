using System;

namespace FubuCore.Dates
{
    public class SimpleTimeZoneContext : ITimeZoneContext
    {
        private readonly TimeZoneInfo _timeZone;

        public SimpleTimeZoneContext(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone;
        }

        public TimeZoneInfo GetTimeZone()
        {
            return _timeZone;
        }
    }
}