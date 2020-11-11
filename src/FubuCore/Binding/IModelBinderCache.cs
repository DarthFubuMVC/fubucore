using System;

namespace FubuCore.Binding
{
    public interface IModelBinderCache
    {
        IModelBinder BinderFor(Type modelType);
    }
}