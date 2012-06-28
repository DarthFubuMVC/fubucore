using System;

namespace FubuCore.Dates
{
    public interface ISystemTime
    {
        DateTime UtcNow();

        LocalTime LocalTime();
    }

    public class LocalTime
    {
        private readonly TimeZoneInfo _timeZone;
        private readonly DateTime _localTime;
        private readonly DateTime _utcTime;

        public static LocalTime AtMachineTime(DateTime time)
        {
            return new LocalTime(time.ToUniversalTime(TimeZoneInfo.Local), TimeZoneInfo.Local);
        }

        public static LocalTime GuessTime(LocalTime localTime, TimeSpan timeOfDay, DateTime? baseTime = null)
        {
            var candidate = localTime.AtTime(timeOfDay);
            if (baseTime == null)
            {
                return candidate;
            }

            while (candidate.UtcTime < baseTime.Value)
            {
                candidate = candidate.Add(1.Days());
            }

            return candidate;
        }

        public LocalTime AtTime(TimeSpan time)
        {
            return BeginningOfDay().Add(time);
        }

        public LocalTime BeginningOfDay()
        {
            var beginningTime = _localTime.Date.ToUniversalTime(_timeZone);
            return new LocalTime(beginningTime, _timeZone);
        }

        public LocalTime(DateTime utcTime, TimeZoneInfo timeZone)
        {
            _timeZone = timeZone;
            _localTime = utcTime.ToLocalTime(timeZone);
            _utcTime = utcTime;
        
        }

        public TimeZoneInfo TimeZone
        {
            get { return _timeZone; }
        }

        public DateTime UtcTime
        {
            get { return _utcTime; }
        }

        public Date Date
        {
            get
            {
                return _localTime.ToDate();
            }
        }

        public TimeSpan TimeOfDay
        {
            get
            {
                return _localTime.TimeOfDay;
            }
        }

        public TimeSpan Subtract(LocalTime otherTime)
        {
            return UtcTime.Subtract(otherTime.UtcTime);
        }

        public LocalTime Add(TimeSpan duration)
        {
            return new LocalTime(_utcTime.Add(duration), _timeZone);
        }

        public DateTime Time
        {
            get { return _localTime; }
        }

        public bool Equals(LocalTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._timeZone, _timeZone) && other._utcTime.Equals(_utcTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LocalTime)) return false;
            return Equals((LocalTime) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_timeZone != null ? _timeZone.GetHashCode() : 0)*397) ^ _utcTime.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("TimeZone: {0}, LocalTime: {1}", _timeZone, _localTime);
        }

        public static bool operator >(LocalTime left, LocalTime right)
        {
            return left.UtcTime > right.UtcTime;
        }

        public static bool operator <(LocalTime left, LocalTime right)
        {
            return left.UtcTime < right.UtcTime;
        }
    }
}