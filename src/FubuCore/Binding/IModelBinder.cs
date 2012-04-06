using System;

namespace FubuCore.Binding
{
    public interface IModelBinder
    {
        bool Matches(Type type);
        object Bind(Type type, IBindingContext context);
    }
}