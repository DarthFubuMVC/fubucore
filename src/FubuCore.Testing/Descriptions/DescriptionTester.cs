using System;
using FubuCore.Descriptions;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuCore.Testing.Descriptions
{
    [TestFixture]
    public class DescriptionTester
    {
        [Test]
        public void default_values_with_a_simple_type()
        {
            var target = new SimpleTarget();
            var description = Description.GetDescription(target);
            
            description.BulletLists.Any().ShouldBeFalse();
            description.LongDescription.ShouldBeNull();
            description.Title.ShouldEqual(typeof (SimpleTarget).Name);
            description.ShortDescription.ShouldEqual(target.ToString());
        }

        [Test]
        public void create_description_for_class_decorated_with_the_description_attribute()
        {
            var target = new DecoratedClass();
            var description = Description.GetDescription(target);

            description.BulletLists.Any().ShouldBeFalse();
            description.LongDescription.ShouldBeNull();

            description.ShortDescription.ShouldEqual("description from the attribute");
            description.Title.ShouldEqual(typeof(DecoratedClass).Name);

            description.TargetType.ShouldEqual(typeof (DecoratedClass));
        }

        [Test]
        public void create_description_for_a_class_decorated_with_a_title()
        {
            Description.GetDescription(new TitledAndDescribedClass())
                .Title.ShouldEqual("The titled class");
        }

        [Test]
        public void create_description_for_HasDescription_class_just_delegates()
        {
            var description = Description.GetDescription(new MakesOwnDescription());
            description.Title.ShouldEqual("the title");
            description.ShortDescription.ShouldEqual("the short description");
            description.TargetType.ShouldEqual(typeof (MakesOwnDescription));
        }

        [Test]
        public void accept_visitor_if_it_only_has_itself()
        {
            var repo = new MockRepository();
            var visitor = repo.StrictMock<IDescriptionVisitor>();

            var description = new Description();

            using (repo.Record())
            {
                visitor.Start(description);
                visitor.End();
            }

            using (repo.Playback())
            {
                description.AcceptVisitor(visitor);
            }
        }

        [Test]
        public void accept_visitory_with_multiple_bullet_lists()
        {
            var repo = new MockRepository();
            var visitor = repo.StrictMock<IDescriptionVisitor>();

            var description = new Description();

            var list = new BulletList();
            list.Children.Add(new Description());
            list.Children.Add(new Description());
            list.Children.Add(new Description());

            description.BulletLists.Add(list);
            description.BulletLists.Add(new BulletList());


            using (repo.Record())
            {
                visitor.Start(description);

                visitor.StartList(list);

                visitor.Start(list.Children[0]);
                visitor.End();

                visitor.Start(list.Children[1]);
                visitor.End();

                visitor.Start(list.Children[2]);
                visitor.End();

                visitor.EndList();

                visitor.StartList(description.BulletLists.Last());
                visitor.EndList();



                visitor.End();
            }

            using (repo.Playback())
            {
                description.AcceptVisitor(visitor);
            }
        }

        [Test]
        public void bullet_list_accept_visitor_with_no_innards()
        {
            var repo = new MockRepository();
            var visitor = repo.StrictMock<IDescriptionVisitor>();

            var list = new BulletList();

            using (repo.Record())
            {
                visitor.StartList(list);
                visitor.EndList();
            }

            using (repo.Playback())
            {
                list.AcceptVisitor(visitor);
            }
        }

        [Test]
        public void bullet_list_accept_visitor_with_children()
        {
            var repo = new MockRepository();
            var visitor = repo.StrictMock<IDescriptionVisitor>();

            var list = new BulletList();
            list.Children.Add(new Description());
            list.Children.Add(new Description());
            list.Children.Add(new Description());

            using (repo.Record())
            {
                visitor.StartList(list);

                visitor.Start(list.Children[0]);
                visitor.End();

                visitor.Start(list.Children[1]);
                visitor.End();

                visitor.Start(list.Children[2]);
                visitor.End();

                visitor.EndList();
            }

            using (repo.Playback())
            {
                list.AcceptVisitor(visitor);
            }
        }
    }

    public class SimpleTarget{}

    public class MakesOwnDescription : HasDescription
    {
        public Description GetDescription()
        {
            return new Description(){
                ShortDescription = "the short description",
                Title = "the title"
            };
        }
    }

    
    [System.ComponentModel.Description("description from the attribute")]
    public class DecoratedClass
    {
        
    }

    
    [Title("The titled class")]
    public class TitledAndDescribedClass
    {
        
    }
}