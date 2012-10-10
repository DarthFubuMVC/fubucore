using System;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuCore.Binding
{
    [Serializable]
    public class ConvertProblem
    {
        public object Item { get; set; }
        public BindingValue Value { get; set; }
        public string ExceptionText { get; set; }
        public PropertyInfo Property { get { return Accessor == null ? null : Accessor.InnerProperty; } }
        public Accessor Accessor { get; set; }

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