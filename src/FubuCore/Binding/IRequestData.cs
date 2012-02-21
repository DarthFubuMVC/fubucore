using System;
using System.Collections.Generic;

namespace FubuCore.Binding
{
    public interface IRequestData
    {
        object Value(string key);
        bool Value(string key, Action<BindingValue> callback);
        bool HasAnyValuePrefixedWith(string key);

        IRequestData GetSubRequest(string prefixOrChild);
        IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild);
    }
}