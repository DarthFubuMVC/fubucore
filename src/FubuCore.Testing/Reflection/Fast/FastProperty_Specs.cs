using System.Linq;
using System.Reflection;
using FubuCore.Reflection.Fast;
using NUnit.Framework;

namespace FubuCore.Testing.Reflection.Fast
{
    [TestFixture]
	public class FastProperty_Specs
	{
		[Test]
		public void Should_be_able_to_access_a_private_setter()
		{
			PrivateSetter instance = new PrivateSetter();

			var property = instance.GetType()
				.GetProperties(BindingFlags.Instance|BindingFlags.Public)
				.Where(x => x.Name == "Name")
				.First();

		
			var fastProperty = new FastProperty<PrivateSetter>(property, BindingFlags.NonPublic);

			const string expectedValue = "Chris";
			fastProperty.Set(instance, expectedValue);

			Assert.AreEqual(expectedValue, fastProperty.Get(instance));
		}
	}

	public class PrivateSetter
	{
		public string Name { get; private set; }
	}
}