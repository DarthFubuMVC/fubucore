using System;
using System.IO;
using System.Xml.Serialization;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
{
	[XmlType("serializeMe")]
	public class SerializeMe
	{
		public static string SerializedXml = @"<?xml version=""1.0""?><serializeMe xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><Name>Serialized Name</Name><Index>42</Index></serializeMe>";

		public string Name { get; set; }
		public int Index { get; set; }
	}
	
	[TestFixture]
	public class FilesSystem_load_from_file
	{
		[Test]
		public void should_deserialize_xml()
		{
			var fileSystem = new FileSystem();
			var fileName = Path.GetTempFileName();
			fileSystem.WriteStringToFile(fileName, SerializeMe.SerializedXml);

			var result = fileSystem.LoadFromFile<SerializeMe>(fileName);

			result.Name.ShouldEqual("Serialized Name");
			result.Index.ShouldEqual(42);
		}

		[Test]
		public void should_return_empty_instance_when_file_does_not_exist()
		{
			var fileSystem = new FileSystem();
			const string fileName = "does not exist";

			var result = fileSystem.LoadFromFile<SerializeMe>(fileName);

			result.Name.ShouldBeNull();
			result.Index.ShouldEqual(0);
		}

		[Test]
		public void should_thrown_when_file_is_not_xml()
		{
			var fileSystem = new FileSystem();
			var fileName = Path.GetTempFileName();
			fileSystem.WriteStringToFile(fileName, "not xml!");

			typeof(ApplicationException).ShouldBeThrownBy(() => fileSystem.LoadFromFile<SerializeMe>(fileName));
		}

		[Test]
		public void load_from_file_or_throw_shuld_throw_when_file_does_not_exist()
		{
			var fileSystem = new FileSystem();
			const string fileName = "does not exist";

			typeof(ApplicationException).ShouldBeThrownBy(() => fileSystem.LoadFromFileOrThrow<SerializeMe>(fileName));
		}
	}

	[TestFixture]
	public class FilesSystem_load_from_file_or_throw
	{
		[Test]
		public void should_throw_when_file_does_not_exist()
		{
			var fileSystem = new FileSystem();
			const string fileName = "does not exist";

			typeof(ApplicationException).ShouldBeThrownBy(() => fileSystem.LoadFromFileOrThrow<SerializeMe>(fileName));
		}
	}
}