using System;
using System.Collections.Generic;
using NUnit.Framework;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using System.Linq;
using FubuCore.CommandLine;
using FubuCore.Reflection;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class UsageGraphTester
    {
        private UsageGraph theUsageGraph;

        [SetUp]
        public void SetUp()
        {
            theUsageGraph = new FakeLinkCommand().Usages;
        }

        [Test]
        public void has_the_command_name()
        {
            theUsageGraph.CommandName.ShouldEqual("list the links");
        }

        [Test]
        public void has_the_description()
        {
            theUsageGraph.Description.ShouldEqual("Manage links");
        }

        [Test]
        public void has_both_usages()
        {
            theUsageGraph.Usages.Select(x => x.Description).OrderBy(x => x).ShouldHaveTheSameElementsAs("Link an application folder to a package folder", "List the links");
        }

        [Test]
        public void has_the_arguments()
        {
            theUsageGraph.Arguments.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("AppFolder", "PackageFolder", "Stuff");
        }

        [Test]
        public void has_the_flags()
        {
            theUsageGraph.Flags.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("RemoveFlag", "CleanAllFlag", "CleanFlag", "NotepadFlag");
        }

        [Test]
        public void first_usage_has_all_the_right_mandatories()
        {
            theUsageGraph.FindUsage("Link an application folder to a package folder").Arguments.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("AppFolder", "PackageFolder");
        }

        [Test]
        public void first_usage_has_all_the_right_flags()
        {
            theUsageGraph.FindUsage("Link an application folder to a package folder").ValidFlags.Select(x => x.PropertyName).OrderBy(x => x).ShouldHaveTheSameElementsAs("CleanAllFlag", "CleanFlag", "RemoveFlag");
        }

        [Test]
        public void second_usage_has_all_the_right_mandatories()
        {
            theUsageGraph.FindUsage("List the links").Arguments.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("AppFolder");
        }

        [Test]
        public void second_usage_has_all_the_right_flags()
        {
            theUsageGraph.FindUsage("List the links").ValidFlags.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("RemoveFlag", "CleanAllFlag", "CleanFlag", "NotepadFlag");
        }

        [Test]
        public void get_the_description_of_both_usages()
        {
            theUsageGraph.FindUsage("Link an application folder to a package folder").ShouldNotBeNull();
            theUsageGraph.FindUsage("List the links").Description.ShouldNotBeNull();
        }

        [Test]
        public void get_the_command_usage_of_the_list_usage()
        {
            theUsageGraph.FindUsage("Link an application folder to a package folder").ToUsage("fubu", "link").ShouldEqual("fubu link <appfolder> <packagefolder> [-r, --remove] [-C, --clean-all] [-c, --clean <clean>]");
        }

        [Test]
        public void get_the_command_usage_of_the_link_usage()
        {
            var usg = theUsageGraph.FindUsage("List the links");
            usg.ToUsage("fubu", "list").ShouldEqual("fubu list <appfolder> [-r, --remove] [-C, --clean-all] [-c, --clean <clean>] [-n, --notepad]");
        }

        [Test]
        public void smoke_test_writing_usage()
        {
            theUsageGraph.WriteUsages("fubu");
        }

        [Test]
        public void derive_a_single_usage_for_any_command_that_has_no_specific_usages()
        {
            var graph = new UsageGraph(typeof (SimpleCommand));
            var usage = graph.Usages.Single();
            usage.Description.ShouldEqual(typeof (SimpleCommand).GetAttribute<CommandDescriptionAttribute>().Description);
            usage.Arguments.Select(x => x.PropertyName).ShouldHaveTheSameElementsAs("Arg1", "Arg2");
        }
    }

    [TestFixture]
    public class enumberable_argument_usages
    {
        UsageGraph theUsageGraph;

        [SetUp]
        public void SetUp()
        {
            theUsageGraph = new UsageGraph(typeof(ComplexCommand));
        }

        [Test]
        public void smoke_test_writing_usage()
        {
            theUsageGraph.WriteUsages("fubu");
        }
    }

    public class ComplexInput
    {
        public string Name { get; set; }
        public IEnumerable<string> NickNames { get; set; }

        public IEnumerable<string> HerpDerpFlag { get; set; }
    }

    [CommandDescription("does complex thing")]
    public class ComplexCommand : FubuCommand<ComplexInput>
    {
        public override bool Execute(ComplexInput input)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class valid_usage_tester
    {
        private UsageGraph theUsageGraph;

        [SetUp]
        public void SetUp()
        {
            theUsageGraph = new FakeLinkCommand().Usages;
        }

        private bool isValidUsage(params string[] args)
        {
            var handlers = theUsageGraph.Handlers.Where(x => args.Contains(x.PropertyName));
            return theUsageGraph.IsValidUsage(handlers);
        }

        [Test]
        public void valid_with_all_required_arguments()
        {
            isValidUsage("AppFolder").ShouldBeTrue();
            isValidUsage("AppFolder", "PackageFolder").ShouldBeTrue();
        }

        [Test]
        public void invalid_with_missing_arguments()
        {
            isValidUsage().ShouldBeFalse();
        }

        [Test]
        public void invalid_flags()
        {
            isValidUsage("AppFolder", "PackageFolder", "NotepadFlag").ShouldBeFalse();
        }
    }
    
    public class FakeLinkInput
    {
        [Description("The root directory of the web folder")]
        public string AppFolder { get; set; }

        [Description("The root directory of a package project")]
        public string PackageFolder { get; set; }

        [Description("Removes the link from the application to the package")]
        [FlagAlias("remove", 'r')]
        public bool RemoveFlag { get; set; }

        [Description("Removes all links from the application folder")]
        [FlagAlias('C')]
        public bool CleanAllFlag { get; set; }
        
        [Description("clean a single folder")]
        public string CleanFlag { get; set; }

        [Description("Opens the application manifest in notepad")]
        public bool NotepadFlag { get; set; }

        [Description("An array of stuff")]
        public string[] Stuff { get; set; }
    }

    [CommandDescription("Manage links", Name = "List the links")]
    public class FakeLinkCommand : FubuCommand<FakeLinkInput>
    {
        public FakeLinkCommand()
        {
            Usage("List the links").Arguments(x => x.AppFolder);
            Usage("Link an application folder to a package folder").Arguments(x => x.AppFolder, x => x.PackageFolder).ValidFlags(x => x.RemoveFlag, x => x.CleanAllFlag, x => x.CleanFlag);
        }

        public override bool Execute(FakeLinkInput input)
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleInput
    {
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
    }

   


    [CommandDescription("does simple thing")]
    public class SimpleCommand : FubuCommand<SimpleInput>
    {
        public override bool Execute(SimpleInput input)
        {
            throw new NotImplementedException();
        }
    }


}