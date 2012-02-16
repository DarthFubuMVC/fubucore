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
            BulletLists = new List<BulletList>();
        }

        public Type TargetType { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }


        public IList<BulletList> BulletLists { get; private set; }
    
    
        public static Description GetDescription(object target)
        {
            Description description = null;
            var type = target.GetType();

            var described = target as HasDescription;
            description = described != null 
                ? described.GetDescription() 
                : GetDescriptionByType(target, type);

            description.TargetType = target.GetType();

            return description;
        }

        public static Description GetDescriptionByType(object target, Type type)
        {
            var description = new Description();
            
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

        public void AcceptVisitor(IDescriptionVisitor visitor)
        {
            visitor.Start(this);

            BulletLists.Each(x => x.AcceptVisitor(visitor));


            visitor.End();
        }
    }

    public class BulletList
    {
        public BulletList()
        {
            Children = new List<Description>();
        }

        public IList<Description> Children { get; private set; }
        public string Name { get; set; }
        public bool IsOrderDependent { get; set; }

        public void AcceptVisitor(IDescriptionVisitor visitor)
        {
            visitor.StartList(this);

            Children.Each(x => x.AcceptVisitor(visitor));

            visitor.EndList();
        }
    }

    public interface IDescriptionVisitor
    {
        void Start(Description description);
        void StartList(BulletList list);
        void EndList();
        void End();
    }
}