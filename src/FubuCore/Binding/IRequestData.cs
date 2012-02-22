using System;
using System.Collections.Generic;

namespace FubuCore.Binding
{
    public interface IRequestData
    {
        object Value(string key);
        bool Value(string key, Action<BindingValue> callback);
        bool HasChildRequest(string key);

        IRequestData GetChildRequest(string prefixOrChild);
        IEnumerable<IRequestData> GetEnumerableRequests(string prefixOrChild);
    }
}