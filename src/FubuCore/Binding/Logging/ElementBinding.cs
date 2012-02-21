using System;
using System.Linq;
using System.Collections.Generic;

namespace FubuCore.Binding.Logging
{
    public class ElementBinding : BindingReport
    {
        private readonly int _index;

        public ElementBinding(int index, Type elementType, IModelBinder binder) : base(elementType, binder)
        {
            _index = index;
        }

        public int Index
        {
            get { return _index; }
        }

        public override void AcceptVisitor(IBindingReportVisitor visitor)
        {
            visitor.Element(this);

            OrderedProperties().ToList().Each(prop => prop.AcceptVisitor(visitor));

            visitor.EndElement();
        }
    }
}