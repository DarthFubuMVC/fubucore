using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class RequestDataTesting
    {
        [Test]
        public void can_add_key_values()
        {
            
        }
    }

    public class RequestDataTarget
    {
        public string Name { get; set; }
        public string Direction { get; set; }
    }
}