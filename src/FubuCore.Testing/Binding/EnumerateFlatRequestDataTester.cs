using System.Linq;
using FubuCore.Binding;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class EnumerateFlatRequestDataTester
    {
        [Test]
        public void find_enumerable_request_data_from_parent()
        {
            var requestData = new InMemoryRequestData();
            requestData.ReadData(@"
Name=Jeremy
Age=38
List[0]Name=Max
List[0]Hometown=Austin
List[1]Name=Natalie
List[1]Hometown=Bentonville
List[2]Name=Nadine
List[2]Hometown=Jasper
List[3]Hometown=Montrose
");

            var elements = EnumerateFlatRequestData.For(requestData, "List");
            elements.Count().ShouldEqual(4);
            elements.Select(x => x.Value("Hometown"))
                .ShouldHaveTheSameElementsAs("Austin", "Bentonville", "Jasper", "Montrose");
            

        }
    }
}