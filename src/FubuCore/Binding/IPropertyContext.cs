using System;
using System.Reflection;
using FubuCore.Binding.InMemory;
using FubuCore.Conversion;

namespace FubuCore.Binding
{
    /// <summary>
    /// Represents the current property in a model binding
    /// session.  This interface is typically consumed within
    /// custom PropertyBinder's
    /// </summary>
    public interface IPropertyContext : IConversionRequest
    {
        /// <summary>
        /// The raw value in the current request data
        /// that matches the name of the current property.
        /// </summary>
        BindingValue RawValueFromRequest { get; }

        /// <summary>
        /// The current property
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// The current object being bound
        /// </summary>
        object Object { get; }

        /// <summary>
        /// Service locator method to the IoC container for the current request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Service<T>();

        /// <summary>
        /// Service locator method to the IoC container for the current request
        /// </summary>
        /// <param name="typeToFind">The type to find</param>
        /// <returns></returns>
        object Service(Type typeToFind);

        /// <summary>
        /// Retrieves the PropertyValue converted to the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ValueAs<T>();

        /// <summary>
        /// CPS version of ValueAs.  The continuation is only called if there is
        /// data in the current request data with the same name as the property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="continuation"></param>
        /// <returns></returns>
        bool ValueAs<T>(Action<T> continuation);

        /// <summary>
        /// Expose logging to the model binding subsystem
        /// </summary>
        IBindingLogger Logger { get; }

        /// <summary>
        /// Shortcut to call PropertyInfo.SetValue(Object, value, null)
        /// </summary>
        /// <param name="value"></param>
        void SetPropertyValue(object value);
        
        /// <summary>
        /// Gets the current value of the property on the object
        /// </summary>
        /// <returns></returns>
        object GetPropertyValue();

        /// <summary>
        /// Shortcut to get converted data from the raw request by name
        /// </summary>
        IContextValues Data { get; }
    }
}