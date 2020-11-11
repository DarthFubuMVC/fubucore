using FubuCore.Binding;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ConvertProblemTester
    {
        private ConvertProblem _problem;

        [SetUp]
        public void SetUp()
        {
            _problem = new ConvertProblem { Item = "some item", Value = new BindingValue() { RawValue = "some value" }, ExceptionText = "exception message" };
            _problem.Property = typeof (PropertyHolder).GetProperty("SomeProperty");
        }

        [Test]
        public void return_to_string()
        {
            _problem.ToString().ShouldEqual(@"Item type:       {0}
Property:        {1}
Property Type:   {2}
Attempted Value: RawKey: , Source: , RawValue: {3}
Exception:
{4} 
".ToFormat("System.String", "SomeProperty", "System.String", "some value", "exception message"));
            
        }

        public class PropertyHolder { public string SomeProperty { get; set; } }
    }
}