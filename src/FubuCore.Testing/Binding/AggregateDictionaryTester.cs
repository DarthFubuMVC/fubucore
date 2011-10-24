using System;
using System.Collections.Generic;
using FubuCore.Binding;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class AggregateDictionaryTester
    {
        [Test]
        public void value_from_a_specific_locator()
        {
            var dictionary = new AggregateDictionary();
            dictionary.AddDictionary("Headers", new Dictionary<string, object>{
                {"key1", 1},
                {"key2", 2},
                {"key3", 3},
                {"key4", 4},
            });

            dictionary.AddDictionary("Post", new Dictionary<string, object>{
                {"key1", 11},
                {"key2", 22},
                {"key3", 33},
                {"key4", 44},
            });

            dictionary.AddDictionary("Querystring", new Dictionary<string, object>{
                {"key1", 111},
                {"key2", 222},
                {"key3", 333},
                {"key4", 444},
            });

            object lastNumber = null;
            Action<string, object> callback = (s, o) => lastNumber = o;
            
            dictionary.Value("Headers", "key2", callback);
            lastNumber.ShouldEqual(2);

            dictionary.Value("Post", "key2", callback);
            lastNumber.ShouldEqual(22);

            dictionary.Value("Querystring", "key2", callback);
            lastNumber.ShouldEqual(222);
        }
    }
}