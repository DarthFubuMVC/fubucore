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
            var described = target as IHasDescription;
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
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TitleAttribute : Attribute
    {
        private readonly string _title;

        public TitleAttribute(string title)
        {
            _title = title;
        }

        public string Title
        {
            get { return _title; }
        }
    }

    public interface IHasDescription
    {
        Description GetDescription();
    }
}