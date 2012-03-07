using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding.Values;
using FubuCore.Util;

namespace FubuCore.Binding
{
    // TODO -- go look at all of the usages of this thing and clean stuff up
    public class RequestData : IRequestData
    {
        private readonly IList<IValueSource> _sources = new List<IValueSource>();

        public RequestData()
        {
        }

        public void AddValues(string name, IKeyValues values)
        {
            var source = new FlatValueSource(values, name);
            _sources.Add(source);
        }

        public void AddValues(IValueSource source)
        {
            _sources.Add(source);
        }

        public IValueSource ValuesFor(string nameOrProvenance)
        {
            return _sources.FirstOrDefault(x => x.Provenance == nameOrProvenance);
        }

        public void WriteReport(IValueReport report)
        {
            _sources.Each(source =>
            {
                report.StartSource(source);
                source.WriteReport(report);
                report.EndSource();
            });
        }

        public RequestData(IValueSource source)
        {
            _sources.Add(source);
        }

        public RequestData(IEnumerable<IValueSource> sources)
        {
            _sources.AddRange(sources);
        }

        public object Value(string key)
        {
            object output = null;

            Value(key, val => output = val.RawValue);

            return output;
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            return _sources.Any(x => x.Value(key, callback));
        }

        public bool HasChildRequest(string key)
        {
            return _sources.Any(x => x.HasChild(key));
        }

        public IRequestData GetChildRequest(string prefixOrChild)
        {
            var sources = _sources.Where(x => x.HasChild(prefixOrChild)).Select(x => x.GetChild(prefixOrChild));
            return new RequestData(sources);
        }

        // Living with the limitation that only *ONE* source can ever have 
        public IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild)
        {
            foreach (var valueSource in _sources)
            {
                var childrenSources = valueSource.GetChildren(prefixOrChild);
                if (childrenSources.Any())
                {
                    return childrenSources.Select(x => new RequestData(x)).ToList();
                }
            }

            return Enumerable.Empty<IRequestData>();
        }
    }

}