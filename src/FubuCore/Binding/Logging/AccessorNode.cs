using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuCore.Binding.Logging
{
    public class AccessorNode
    {
        private static readonly MethodInfo _indexer;

        private int _index;
        private readonly Accessor _accessor;

        static AccessorNode()
        {
            _indexer = ReflectionHelper.GetMethod<IList>(x => x[0]);
        }

        public AccessorNode(IEnumerable<IValueGetter> values, PropertyInfo property)
        {
            if (values.Any())
            {
                _accessor = new PropertyChain(values.Concat(new IValueGetter[] { new PropertyValueGetter(property), }).ToArray());
            }
            else
            {
                _accessor = new SingleProperty(property);
            }
        }

        public AccessorReport Nested()
        {
            return new AccessorReport(_accessor.Getters());
        }

        public AccessorReport Element()
        {
            var report = new AccessorReport(_accessor.Getters().Concat(new IValueGetter[] { new MethodValueGetter(_indexer, new object[] { _index }) }));
            _index++;
            return report;
        }

        public Accessor Accessor
        {
            get { return _accessor; }
        }

    }
}