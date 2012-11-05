using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Binding.Values
{
    public class ValueSourceReport
    {
        private readonly string _name;
        private readonly Cache<string, IList<string>> _values;

        public ValueSourceReport(string name)
        {
            _name = name;
            _values = new Cache<string, IList<string>>(x => new List<string>());
        }

        public string Name
        {
            get { return _name; }
        }

        public void Store(string key, object value)
        {
            var stored = value == null ? "NULL" : value.ToString();
            _values[key].Add(stored);
        }

        public Cache<string, IList<string>> Values
        {
            get { return _values; }
        }
    }

    public abstract class ValueReportBase : IValueReport
    {
        private readonly Stack<string> _prefixes = new Stack<string>();
        private string _prefix;
        protected string _source;
        protected readonly Cache<string, DiagnosticValue> _values = new Cache<string, DiagnosticValue>(key => new DiagnosticValue(key));

        private void pushPrefix(string prefix)
        {
            _prefixes.Push(prefix);
            resetPrefix();
        }

        private void resetPrefix()
        {
            _prefix = _prefixes.Reverse().Join(".");
        }

        private void popPrefix()
        {
            _prefixes.Pop();
            resetPrefix();
        }

        public void StartSource(IValueSource source)
        {
            _prefixes.Clear();
            _source = source.Provenance;
            _prefix = string.Empty;

            startSource(source);
        }

        protected virtual void startSource(IValueSource source)
        {
            // no-op;
        }

        public void EndSource()
        {
            // no-op;
        }

        public void Value(string key, object value)
        {
            var fullKey = _prefix.IsEmpty() ? key : _prefix + "." + key;
            store(fullKey, value);
        }

        protected abstract void store(string fullKey, object value);

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