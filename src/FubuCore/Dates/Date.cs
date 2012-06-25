using System;
using FubuCore.Conversion;

namespace FubuCore.Dates
{
    public class Date
    {
        public const string TimeFormat = "ddMMyyyy";
        private DateTime _date;

        // This *has* to be here for serialization
        public Date()
        {
        }

        public Date(DateTime date)
            : this(date.ToString(TimeFormat))
        {
        }

        public Date(int month, int day, int year)
        {
            _date = new DateTime(year, month, day);
        }

        public Date(string ddmmyyyy)
        {
            _date = DateTime.ParseExact(ddmmyyyy, TimeFormat, null);
        }

        public Date NextDay()
        {
            return new Date(_date.AddDays(1));
        }

        public DateTime Day
        {
            get { return _date; }
            set { _date = value; }
        }

        public bool Equals(Date other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._date.Equals(_date);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Date)) return false;
            return Equals((Date)obj);
        }

        public static bool operator ==(Date left, Date right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Date left, Date right)
        {
            return !Equals(left, right);
        }

        public override int GetHashCode()
        {
            return _date.GetHashCode();
        }

        public override string ToString()
        {
            return _date.ToString(TimeFormat);
        }

        public Date AddDays(int daysFromNow)
        {
            return new Date(_date.AddDays(daysFromNow));
        }

        public DateTime AtTime(TimeSpan time)
        {
            return _date.Date.Add(time);
        }

        public DateTime AtTime(string mmhh)
        {
            return _date.Date.Add(TimeSpanConverter.GetTimeSpan(mmhh));
        }

        public static Date Today()
        {
            return new Date(DateTime.Today);
        }
    }
}