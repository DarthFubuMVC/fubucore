using FubuCore.Binding;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class InMemoryRequestDataTester
    {
        public interface ICalledUpon{void Action();}
        private InMemoryRequestData _data;
        private Mock<ICalledUpon> _calledUpon;

        [SetUp]
        public void SetUp()
        {
            _calledUpon = new Mock<ICalledUpon>();
            _data = new InMemoryRequestData();
            _calledUpon.Setup(c => c.Action());
        }

        [Test]
        public void should_cache_value()
        {
            const int value = 6;
            _data["key"] = value;
            _data["key"].ShouldEqual(value);
            _data.Value("key").ShouldEqual(value);
            _data.Value("key", o => _calledUpon.Object.Action());
            _calledUpon.Verify(c=>c.Action());
            _data.Value("non_existing_key").ShouldBeNull();
        }

        [Test]
        public void find_value()
        {
            _data["a"] = 2;
            _data["a1"] = 2;
            _data["a3"] = 2;

            _data.Value("b").ShouldBeNull();
            
            
        }

        [Test]
        public void all_keys()
        {
            _data["a"] = 2;
            _data["a1"] = 2;
            _data["a3"] = 2;

            _data.GetKeys().ShouldHaveTheSameElementsAs("a", "a1", "a3");
        }

        [Test]
        public void has_prefixed_key()
        {
            _data["pre1"] = 1;
            _data["pre3"] = 1;
            _data["pre-ab"] = 1;
            _data["pre4"] = 1;

            _data.HasChildRequest("pre").ShouldBeTrue();

        
            _data.HasChildRequest("else").ShouldBeFalse();
        }
    }
}