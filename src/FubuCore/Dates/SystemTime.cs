using System;
using System.Threading;

namespace FubuCore.Dates
{
    public interface IClock
    {
        DateTime UtcNow();
    }

    public interface ITimeZoneContext
    {
        TimeZoneInfo GetTimeZone();
    }

    public class MachineTimeZoneContext : ITimeZoneContext
    {
        public TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.Local;
        }
    }

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

    public class Clock : IClock
    {
        private Func<DateTime> _now = () => DateTime.UtcNow;

        public DateTime UtcNow()
        {
            return _now();
        }

        public void Live()
        {
            _now = () => DateTime.Now;
        }

        public Clock LocalNow(DateTime localTime, TimeZoneInfo localZone = null)
        {
            var zone = localZone ?? TimeZoneInfo.Local;
            var now = localTime.ToUniversalTime(zone);

            _now = () => now;
            return this;
        }

        public Clock RestartAtLocal(DateTime desiredLocalTime, TimeZoneInfo localZone = null)
        {
            var zone = localZone ?? TimeZoneInfo.Local;
            var desired = desiredLocalTime.ToUniversalTime(zone);

            var delta = desired.Subtract(DateTime.UtcNow);
            _now = () => DateTime.UtcNow.Add(delta);

            return this;
        }
    }


    public class SystemTime : ISystemTime
    {
        private readonly IClock _clock;
        private readonly ITimeZoneContext _context;

        public static SystemTime Default()
        {
            return new SystemTime(new Clock(), new MachineTimeZoneContext());
        }

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
    }
}