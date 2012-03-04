using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.Util
{
    public interface IKeyValues
    {
        bool Has(string key);
        string Get(string key);
        IEnumerable<string> GetKeys();

        bool ForValue(string key, Action<string, string> callback);
    }

    public static class KeyValueExtensions
    {
        public static void ReadAll(this IKeyValues values, Action<string, string> callback)
        {
            values.GetKeys().ToList().Each(key => callback(key, values.Get(key)));
        }
    }
}