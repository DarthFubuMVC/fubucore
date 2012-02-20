using System;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;

namespace FubuCore.Binding
{
    [Description("Converts text by ConfigurationManager.ConnectionStrings[text] ")]
    public class ResolveConnectionStringFamily : StatelessConverter
    {
        public override bool Matches(PropertyInfo property)
        {
            return property.HasAttribute<ConnectionStringAttribute>();
        }

        public static Func<string, ConnectionStringSettings> GetConnectionStringSettings = key => ConfigurationManager.ConnectionStrings[key];

        private static string getConnectionString(string name)
        {
            var connectionStringSettings = GetConnectionStringSettings(name);
            return connectionStringSettings != null
                ? connectionStringSettings.ConnectionString
                : name;
        }

        public override object Convert(IPropertyContext context)
        {
            var stringValue = context.RawValueFromRequest.RawValue as String;

            return stringValue.IsNotEmpty()
                       ? getConnectionString(stringValue)
                       : stringValue;
        }
    }
}