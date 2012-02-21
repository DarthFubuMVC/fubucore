using System;

namespace FubuCore.Binding.InMemory
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
    }
}