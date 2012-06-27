using System;

namespace FubuCore.Dates
{
    public class SystemTime : ISystemTime
    {
        private readonly IClock _clock;
        private readonly ITimeZoneContext _context;

        public SystemTime(IClock clock, ITimeZoneContext context)
        {
            _clock = clock;
            _context = context;
        }

        public DateTime LocalNow()
        {
            return _clock.UtcNow().ToLocalTime(_context.GetTimeZone());
        }

        public Date LocalDay()
        {
            return new Date(LocalNow().Date);
        }

        public TimeSpan LocalTime()
        {
            return LocalNow().TimeOfDay;
        }

        public DateTime UtcNow()
        {
            return _clock.UtcNow();
        }

        public static SystemTime Default()
        {
            return new SystemTime(new Clock(), new MachineTimeZoneContext());
        }
    }
}