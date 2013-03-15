namespace FubuCore.Docs
{
    public class FubuCoreRootTopicRegistry : FubuDocs.TopicRegistry
    {
        public FubuCoreRootTopicRegistry()
        {
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.IocAbstractions>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.CommandLine.BuildingCommandLineTools>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Configuration.ConfigurationTheFubuWay>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Csv.WorkingWithFlatFiles>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Dates.DateUtilities>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.DependencyAnalysis.DependencyAnalysis>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Descriptions.SelfDescribingObjects>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Formatting.DisplayFormatting>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Logging.GenericLoggingSupport>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Reflection.ReflectionHelpers>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Utility.WritingTextReports>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Utility.Cache>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Utility.CompositeActionsAndFilters>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.FileSystem.WorkingWithTheFileSystem>();
            For<FubuCore.Docs.FubuCoreRoot>().Append<FubuCore.Docs.Extensions.ExtensionLibraries>();

            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.TypeConversions>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.UsingWithAnIocTool>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.UsingWithoutAnIocTool>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.Architecture>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.BuiltInBinders>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.BlacklistingProperties>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.WhitelistingProperties>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.Extending>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.UnitTesting>();
            For<FubuCore.Docs.Binding.ModelBindingAndTypeConversion>().Append<FubuCore.Docs.Binding.LoggingAndDiagnostics>();

            For<FubuCore.Docs.Binding.TypeConversions>().Append<FubuCore.Docs.Binding.BuiltInConversions>();
            For<FubuCore.Docs.Binding.TypeConversions>().Append<FubuCore.Docs.Binding.Customizing>();
            For<FubuCore.Docs.Binding.TypeConversions>().Append<FubuCore.Docs.Binding.Architecture>();

            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Objectresolver>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.IntegrationWithIoc>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Iconverterstrategy>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Converterlibrary>();

            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Iobjectresolver>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Bindingregistry>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Ivaluesource>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Ikeyvalues>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Imodelbinder>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Ibindingcontext>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Ipropertybinder>();
            For<FubuCore.Docs.Binding.Architecture>().Append<FubuCore.Docs.Binding.Iconverterfamily>();

            For<FubuCore.Docs.Binding.Extending>().Append<FubuCore.Docs.Binding.IntegrationWithYourIocContainer>();
            For<FubuCore.Docs.Binding.Extending>().Append<FubuCore.Docs.Binding.CustomValueSources>();
            For<FubuCore.Docs.Binding.Extending>().Append<FubuCore.Docs.Binding.CustomModelBinders>();
            For<FubuCore.Docs.Binding.Extending>().Append<FubuCore.Docs.Binding.CustomPropertyBinders>();
            For<FubuCore.Docs.Binding.Extending>().Append<FubuCore.Docs.Binding.CustomValueConversions>();

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
