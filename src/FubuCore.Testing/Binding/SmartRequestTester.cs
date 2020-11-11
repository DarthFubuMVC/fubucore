using System;
using FubuCore.Binding;
using FubuCore.Conversion;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    public class Model{}

    [TestFixture]
    public class SmartRequestTester
    {
        private InMemoryRequestData theData;
        private SmartRequest theRequest;
        private ObjectConverter objectConverter;
        private ConverterLibrary theLibrary;

        [SetUp]
        public void SetUp()
        {
            theData = new InMemoryRequestData();
            theLibrary = new ConverterLibrary();
            objectConverter = new ObjectConverter(new InMemoryServiceLocator(), theLibrary);
            theRequest = new SmartRequest(theData, objectConverter);
        }


        [Test]
        public void if_value_is_null_return_null_from_Value()
        {
            theData["blob"] = null;
            theRequest.Value<Blob>("blob").ShouldBeNull();
        }

        [Test]
        public void do_not_convert_the_type_if_it_is_already_in_the_correct_type()
        {
            theLibrary.RegisterConverter<Blob>(b => new Blob());
            var theBlob = new Blob();
            theData["blob"] = theBlob;
            theRequest.Value<Blob>("blob").ShouldBeTheSameAs(theBlob);
        }

        [Test]
        public void convert_the_type_if_it_necessary()
        {
            theData["int"] = "5";
            theRequest.Value<int>("int").ShouldEqual(5);
            theRequest.Value(typeof(int), "int").ShouldEqual(5);
        }

        public enum SampleEnum {a, B}
        [Test]
        public void convert_case_miss_enum()
        {

            theData["enum-a"] = "A";
            theData["enum-b"] = "b";
            theRequest.Value<SampleEnum>("enum-a").ShouldEqual(SampleEnum.a);
            theRequest.Value<SampleEnum>("enum-b").ShouldEqual(SampleEnum.B);
            theRequest.Value(typeof (SampleEnum), "enum-a").ShouldEqual(SampleEnum.a);

        }

        [Test]
        public void missing_value_with_continuation_does_nothing()
        {
            theRequest.Value<int>("number", i =>
            {
                Assert.Fail("I should not have been called");
            }).ShouldBeFalse();
        }

        [Test]
        public void found_value_with_continuation()
        {
            var action = new Mock<Action<int>>();
            theData["int"] = "5";

            theRequest.Value<int>("int", action.Object).ShouldBeTrue();

            action.Verify(x => x.Invoke(5));
        }
    }

    public class Blob
    {
        
    }
}