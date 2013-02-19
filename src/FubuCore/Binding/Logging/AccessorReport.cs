using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuCore.Binding.Logging
{
    public class AccessorReport
    {
        private readonly IEnumerable<IValueGetter> _values;
        private readonly IList<AccessorNode> _nodes = new List<AccessorNode>();

        public AccessorReport()
            : this(new IValueGetter[0])
        {
        }

        public AccessorReport(AccessorReport parent)
            : this(parent._values)
        {
        }

        public AccessorReport(IEnumerable<IValueGetter> values)
        {
            _values = values;
        }


        public void AddNode(PropertyInfo property)
        {
            _nodes.Add(new AccessorNode(_values, property));
        }

        public AccessorNode LastNode
        {
            get { return _nodes.LastOrDefault(); }
        }

        public AccessorNode NodeOf(PropertyInfo property)
        {
            return _nodes.FirstOrDefault(x => x.Accessor.InnerProperty == property);
        }
    }
}