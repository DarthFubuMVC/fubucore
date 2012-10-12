using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Binding.Values;
using FubuCore.Reflection;

namespace FubuCore.Csv
{
    public abstract class ColumnMapping<T> : IValueSourceProvider
    {
        private readonly IList<Accessor> _accessors = new List<Accessor>();   

        // TODO -- We *could* enable aliasing here and let the column order vary
        public void Column(Expression<Func<T, object>> expression)
        {
            _accessors.Add(expression.ToAccessor());
        }

        IValueSource IValueSourceProvider.Build(string data)
        {
            // TODO -- Harden this
            var dictionary = new Dictionary<string, string>();
            var values = data.Split(new[] {","}, StringSplitOptions.None);
            for (var i = 0; i < values.Length; i++)
            {
                // TODO -- Blow up with a nicer error
                dictionary.Add(_accessors[i].Name, values[i]);
            }
                
            return new FlatValueSource(dictionary);
        }
    }
}