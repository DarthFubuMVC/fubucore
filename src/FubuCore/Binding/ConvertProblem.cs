using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace FubuCore.Binding
{
    [Serializable]
    public class ConvertProblem
    {
        public object Item { get; set; }
        public BindingValue Value { get; set; }
        public string ExceptionText { get; set; }
        public PropertyInfo Property { get; set; }

        public override string ToString()
        {
            return
                @"Item type:       {0}
Property:        {1}
Property Type:   {2}
Attempted Value: {3}
Exception:
{4} 
"
                    .ToFormat(
                    ((Item != null) ? Item.GetType().FullName : "(null)"),
                    Property.Name,
                    Property.PropertyType,
                    Value,
                    ExceptionText);
        }
    }
}