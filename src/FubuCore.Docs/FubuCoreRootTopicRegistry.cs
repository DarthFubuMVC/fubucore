namespace FubuCore.Docs
{
    public class FubuCoreRootTopicRegistry : FubuDocs.TopicRegistry
    {
        public FubuCoreRootTopicRegistry()
        {
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Binding.ModelBinding>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Reflection.ReflectionHelpers>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.FileSystem.WorkingWithTheFileSystem>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Extensions.ExtensionLibraries>();

            For<FubuCore.Docs.Reflection.ReflectionHelpers>().Append<FubuCore.Docs.Reflection.TypeExtensions>();
            For<FubuCore.Docs.Reflection.ReflectionHelpers>().Append<FubuCore.Docs.Reflection.Typeresolver>();

            For<FubuCore.Docs.FileSystem.WorkingWithTheFileSystem>().Append<FubuCore.Docs.FileSystem.Ifilesystem>();
            For<FubuCore.Docs.FileSystem.WorkingWithTheFileSystem>().Append<FubuCore.Docs.FileSystem.Filechangepollingwatcher>();
            For<FubuCore.Docs.FileSystem.WorkingWithTheFileSystem>().Append<FubuCore.Docs.FileSystem.ManipulatingTextFiles>();

            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.Safedispose>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.ContinuationPassingStyleExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.BooleanExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.DictionaryExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.EnumerableExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.FileHashingExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.NumberExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.ReaderwriterlockExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.StreamExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.StringExtensions>();
            For<FubuCore.Docs.Extensions.ExtensionLibraries>().Append<FubuCore.Docs.Extensions.TimespanExtensions>();

        }
    }
}
