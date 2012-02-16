using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore.Reflection;

namespace FubuCore.Descriptions
{
    public class Description
    {
        public Description()
        {
            Children = new List<Description>();
        }

        public Type TargetType { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }


        public IList<Description> Children { get; private set; }
    
    
        public static Description GetDescription(object target)
        {
            var described = target as HasDescription;
            if (described != null)
            {
                return described.GetDescription();
            }

            var description = new Description();
            var type = target.GetType();
            type.ForAttribute<DescriptionAttribute>(x => description.ShortDescription = x.Description);
            type.ForAttribute<TitleAttribute>(x => description.Title = x.Title);

            description.Title = description.Title ?? type.Name;
            description.ShortDescription = description.ShortDescription ?? target.ToString();

            return description;
        }

        public static bool HasExplicitDescription(Type type)
        {
            return type.CanBeCastTo<HasDescription>() || type.HasAttribute<DescriptionAttribute>() ||
                   type.HasAttribute<TitleAttribute>();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Title, ShortDescription);
        }
    }
}