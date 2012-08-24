using System.Collections.Generic;

namespace FubuCore.Binding.Values
{
    public class ValueReport : ValueReportBase
    {
        private readonly IList<ValueSourceReport> _reports = new List<ValueSourceReport>();
        private ValueSourceReport _currentReport;

        protected override void store(string fullKey, object value)
        {
            _currentReport.Store(fullKey, value);
        }

        protected override void startSource(IValueSource source)
        {
            _currentReport = new ValueSourceReport(source.Provenance);
            _reports.Add(_currentReport);
        }

        public IList<ValueSourceReport> Reports
        {
            get { return _reports; }
        }
    }
}