using System;
using System.Collections.Generic;
using FubuCore.Binding.Values;
using FubuCore.Configuration;

namespace FubuCore.Binding
{
    public interface IObjectResolver
    {
        BindResult BindModel(Type type, IRequestData data);

        [MarkedForTermination("Eliminate!")]
        BindResult BindModel(Type type, IBindingContext context);

        /// <summary>
        ///   Use this method when the type may not have a matching IModelBinder
        /// </summary>
        /// <param name = "type"></param>
        /// <param name = "context"></param>
        /// <param name = "onResult"></param>
        [MarkedForTermination]
        void TryBindModel(Type type, IBindingContext context, Action<BindResult> continuation);

        void BindProperties<T>(T model, IBindingContext context);


        void BindProperties(Type type, object model, IBindingContext context);


        void TryBindModel(Type type, IRequestData data, Action<BindResult> continuation);

        BindResult BindModel(Type type, IValueSource values);
    }

    public static class ObjectResolverExtensions
    {
        public static T Bind<T>(this IObjectResolver resolver, IDictionary<string, object> dictionary)
        {
            var source = new SettingsData(dictionary);
            var result = resolver.BindModel(typeof (T), source);

            result.AssertNoProblems(typeof (T));

            return (T) result.Value;
        }
    }
}