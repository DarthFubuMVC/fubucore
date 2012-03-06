using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Binding.Values
{
    public class ValueDiagnosticReport : IValueReport
    {
        private readonly Stack<string> _prefixes = new Stack<string>();
        private string _prefix;
        private string _source;
        private readonly Cache<string, DiagnosticValue> _values = new Cache<string, DiagnosticValue>(key => new DiagnosticValue(key));

        private void pushPrefix(string prefix)
        {
            _prefixes.Push(prefix);
            resetPrefix();
        }

        private void resetPrefix()
        {
            _prefix = GenericEnumerableExtensions.Join((IEnumerable<string>) _prefixes.Reverse(), ".");
        }

        private void popPrefix()
        {
            _prefixes.Pop();
            resetPrefix();
        }

        public DiagnosticValue For(string propertyName)
        {
            return _values[propertyName];
        }

        public IEnumerable<DiagnosticValue> AllValues()
        {
            return _values.GetAll().OrderBy(x => x.Key);
        }

        public void StartSource(IValueSource source)
        {
            _prefixes.Clear();
            _source = source.Provenance;
            _prefix = string.Empty;
        }

        public void EndSource()
        {
            // no-op;
        }

        public void Value(string key, object value)
        {
            var fullKey = _prefix.IsEmpty() ? key : _prefix + "." + key;
            _values[fullKey].Add(_source, value);
        }

        public void StartChild(string key)
        {
            pushPrefix(key);
        }

        public void EndChild()
        {
            popPrefix();
        }

        public void StartChild(string key, int index)
        {
            pushPrefix("{0}[{1}]".ToFormat(key, index));
        }
    }
}