using NUnit.Framework;
using FubuTestingSupport;

namespace FubuCore.Testing
{
    [TestFixture]
    public class FileHashingExtensionsTester
    {
        [Test]
        public void hash_by_modified_is_repeatable()
        {
            var file1 = "a.txt";
            var file2 = "b.txt";
            new FileSystem().WriteStringToFile(file1, "something");
            new FileSystem().WriteStringToFile(file2, "else");

            file1.GetModifiedDateFileText().ShouldEqual(file1.GetModifiedDateFileText());
            file2.GetModifiedDateFileText().ShouldEqual(file2.GetModifiedDateFileText());
            file2.GetModifiedDateFileText().ShouldNotEqual(file1.GetModifiedDateFileText());

            file1.HashByModifiedDate().ShouldEqual(file1.HashByModifiedDate());
            file2.HashByModifiedDate().ShouldEqual(file2.HashByModifiedDate());
            file2.HashByModifiedDate().ShouldNotEqual(file1.HashByModifiedDate());
        
        }

        [Test]
        public void hash_by_modified_is_dependent_upon_the_last_modified_time()
        {
            var file1 = "a.txt";
            new FileSystem().WriteStringToFile(file1, "something");

            var hash1 = file1.HashByModifiedDate();

            new FileSystem().WriteStringToFile(file1, "else");

            var hash2 = file1.HashByModifiedDate();

            hash1.ShouldNotEqual(hash2);
        }

        [Test]
        public void hash_group_of_files_by_modified_date()
        {
            var file1 = "a.txt";
            var file2 = "b.txt";
            var file3 = "c.txt";
            new FileSystem().WriteStringToFile(file1, "something");
            new FileSystem().WriteStringToFile(file2, "else");
            new FileSystem().WriteStringToFile(file3, "altogether");

            // Isn't dependent upon order of the files
            var hash1 = new string[]{file1, file2, file3}.HashByModifiedDate();
            var hash2 = new string[] { file2, file3, file1 }.HashByModifiedDate();
            var hash3 = new string[] { file2, file1, file3 }.HashByModifiedDate();

            hash1.ShouldEqual(hash2).ShouldEqual(hash3);

            var hash4 = new string[] { file1, file2 }.HashByModifiedDate();
            hash4.ShouldNotEqual(hash1);

            new FileSystem().WriteStringToFile(file1, "else");
            var hash5 = new string[] { file2, file1, file3 }.HashByModifiedDate();

            hash5.ShouldNotEqual(hash1);
        }
    }
}