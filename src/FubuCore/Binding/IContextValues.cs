using System;

namespace FubuCore.Binding
{
    public interface IContextValues
    {
        /// <summary>
        /// Fetches the value in the request data by name and converts the value
        /// to the supplied type.  Respects the current prefix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T ValueAs<T>(string name);

        /// <summary>
        /// CPS style call to ValueAs<T>().  The continuation is only called if the named value is
        /// in the current request data.  Respects the current prefix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        bool ValueAs<T>(string name, Action<T> continuation);

        /// <summary>
        /// Gets an unconverted value from the underlying data
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        BindingValue RawValue(string name);

        /// <summary>
        /// Gets an unconverted value from the underlying data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        bool RawValue(string name, Action<BindingValue> continuation);
    }
}