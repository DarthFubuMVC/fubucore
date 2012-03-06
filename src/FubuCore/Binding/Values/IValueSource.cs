using System;
using System.Collections.Generic;

namespace FubuCore.Binding.Values
{
    public interface IValueSource
    {
        string Provenance { get; }
        bool Has(string key);
        object Get(string key);

        bool HasChild(string key);
        IValueSource GetChild(string key);

        IEnumerable<IValueSource> GetChildren(string key);

        void WriteReport(IValueReport report);

        bool Value(string key, Action<BindingValue> callback);
    }
}