using System;
using System.Collections.Generic;
using System.Reflection;

namespace FubuCore.Binding
{
    /// <summary>
    /// BindingContext represents the state of model binding as well
    /// as providing some common functionality
    /// </summary>
    public interface IBindingContext
    {

        /// <summary>
        /// Expose logging to the model binding subsystem
        /// </summary>
        IBindingLogger Logger { get; }

        /// <summary>
        /// List of all data conversion errors encountered during
        /// a model binding operation
        /// </summary>
        IList<ConvertProblem> Problems { get; }
        

        /// <summary>
        /// Performs the binding action against the Property.  Note: this
        /// method is no longer tied to the existence of a data field in the
        /// IRequestData that matches the property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="action"></param>
        void ForProperty(PropertyInfo property, Action<IPropertyContext> action);

        /// <summary>
        /// CPS method to "tell" the IBindingContext what the current target of
        /// model binding is. 
        /// </summary>
        /// <param name="object"></param>
        /// <param name="action"></param>
        void ForObject(object @object, Action action);

        /// <summary>
        /// Binds an object of the requested type to the IRequestData.  Use this
        /// as a helper method to bind nested objects and collection objects
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        void BindObject(IRequestData data, Type type, Action<object> continuation);

        /// <summary>
        /// The current object being bound
        /// </summary>
        object Object { get; }

        /// <summary>
        /// The underlying data for this binding context
        /// </summary>
        IRequestData RequestData { get; }

        /// <summary>
        /// Service locator method to the IoC container for the current request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Service<T>();

        /// <summary>
        /// Shortcut to get converted data from the raw request by name
        /// </summary>
        IContextValues Data { get; }
    }
}