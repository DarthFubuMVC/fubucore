using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore.Reflection;
using System.Linq;
using FubuCore.Util;

namespace FubuCore.Descriptions
{
    public class Description
    {
        private readonly Cache<string, string> _properties = new Cache<string, string>();

        public Description()
        {
            BulletLists = new List<BulletList>();
        }

        public Type TargetType { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public Cache<string, string> Properties
        {
            get { return _properties; }
        }

        public bool HasExplicitShortDescription()
        {
            if (ShortDescription.IsEmpty()) return false;

            if (TargetType == null) return true;

            return ShortDescription != TargetType.FullName.ToString();
        }

        public IList<BulletList> BulletLists { get; private set; }

        public BulletList AddList(string name, IEnumerable objects)
        {
            var list = new BulletList{
                Name = name
            };
            
            BulletLists.Add(list);

            objects.Each(x =>
            {
                var desc = Description.For(x);
                list.Children.Add(desc);
            });

            return list;
        }

        public static Description For(object target)
        {
            var type = target.GetType();

            var description = new Description{
                TargetType = target.GetType(),
                Title = type.Name,
                ShortDescription = target.ToString()
            };



            type.ForAttribute<DescriptionAttribute>(x => description.ShortDescription = x.Description);
            type.ForAttribute<TitleAttribute>(x => description.Title = x.Title);

            (target as DescribesItself).CallIfNotNull(x => x.Describe(description));

            return description;
        }

        public static bool HasExplicitDescription(Type type)
        {
            return type.CanBeCastTo<DescribesItself>() || type.HasAttribute<DescriptionAttribute>() ||
                   type.HasAttribute<TitleAttribute>();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Title, ShortDescription);
        }

        public void AcceptVisitor(IDescriptionVisitor visitor)
        {
            visitor.Start(this);

            BulletLists.Each(x => x.AcceptVisitor(visitor));


            visitor.End();
        }

        public bool IsMultiLevel()
        {
            return BulletLists.Any();
        }
    }
}