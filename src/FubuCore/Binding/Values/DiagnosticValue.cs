using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Binding.Values
{
    public class DiagnosticValue : IEnumerable<DiagnosticValueSource>
    {
        private readonly IList<DiagnosticValueSource> _sources = new List<DiagnosticValueSource>();

        public DiagnosticValue(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }

        public void Add(string source, object value)
        {
            _sources.Add(new DiagnosticValueSource(source, value));
        }

        public DiagnosticValueSource First()
        {
            return _sources.First();
        }

        public IEnumerator<DiagnosticValueSource> GetEnumerator()
        {
            return _sources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}