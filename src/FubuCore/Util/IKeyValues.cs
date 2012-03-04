using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Util
{
    public interface IKeyValues<T>
    {
        bool Has(string key);
        T Get(string key);
        IEnumerable<string> GetKeys();

        bool ForValue(string key, Action<string, T> callback);
    }

    public interface IKeyValues : IKeyValues<string>
    {

    }

    public static class KeyValueExtensions
    {
        public static void ReadAll(this IKeyValues values, Action<string, string> callback)
        {
            values.GetKeys().ToList().Each(key => callback(key, values.Get(key)));
        }
    }
}