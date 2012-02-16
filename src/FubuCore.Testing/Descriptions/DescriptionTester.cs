using System;
using FubuCore.Descriptions;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

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
            
            description.Children.Any().ShouldBeFalse();
            description.LongDescription.ShouldBeNull();
            description.Title.ShouldEqual(typeof (SimpleTarget).Name);
            description.ShortDescription.ShouldEqual(target.ToString());
        }

        [Test]
        public void create_description_for_class_decorated_with_the_description_attribute()
        {
            var target = new DecoratedClass();
            var description = Description.GetDescription(target);

            description.Children.Any().ShouldBeFalse();
            description.LongDescription.ShouldBeNull();

            description.ShortDescription.ShouldEqual("description from the attribute");
            description.Title.ShouldEqual(typeof(DecoratedClass).Name);
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
        }
    }

    public class SimpleTarget{}

    public class MakesOwnDescription : IHasDescription
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