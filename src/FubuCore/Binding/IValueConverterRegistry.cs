using System.Collections.Generic;
using System.Reflection;

namespace FubuCore.Binding
{
    public interface IValueConverterRegistry
    {
        ValueConverter FindConverter(PropertyInfo property);
        IEnumerable<IConverterFamily> AllConverterFamilies();
    }
}