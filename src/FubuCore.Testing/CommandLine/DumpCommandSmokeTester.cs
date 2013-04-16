using System;
using System.Linq;
using FubuCore.CommandLine;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class DumpCommandSmokeTester
    {
        [SetUp]
        public void SetUp()
        {
            theFactory = new CommandFactory();
            theFactory.RegisterCommands(GetType().Assembly);

            theInput = new DumpUsagesInput
            {
                ApplicationName = "ripple",
                Location = "somewhere.xml",
                Commands = theFactory
            };

            theCommand = new DumpUsagesCommand();

            report = theCommand.BuildReport(theInput);

            commandReport = report.Commands.Single(x => x.Name == "crazy");
        }

        private CommandFactory theFactory;
        private DumpUsagesInput theInput;
        private DumpUsagesCommand theCommand;
        private CommandLineApplicationReport report;
        private CommandReport commandReport;

        [Test]
        public void can_build_a_report()
        {

            report.ShouldNotBeNull();
            report.Commands.Any().ShouldBeTrue();
        }

        [Test]
        public void spot_check_a_command()
        {
            commandReport.Description.ShouldEqual("The crazy command");
            commandReport.Name.ShouldEqual("crazy");
            

        }

        [Test]
        public void spot_check_args()
        {
            commandReport.Arguments.Select(x => x.Name)
    .ShouldHaveTheSameElementsAs("arg1", "arg2", "arg3");

            var arg1 = commandReport.Arguments.Single(x => x.Name == "arg1");
            arg1.Name.ShouldEqual("arg1");
            arg1.Description.ShouldEqual("The first arg");
        }

        [Test]
        public void spot_check_flags()
        {
            commandReport.Flags.Select(x => x.Description)
                .ShouldHaveTheSameElementsAs("make it red", "make it blue", "make it green");

            commandReport.Flags.Select(x => x.UsageDescription)
                .ShouldHaveTheSameElementsAs("[-r, --red]", "[-b, --blue]", "[-g, --green]");
        }

        [Test]
        public void spot_check_usages()
        {
            commandReport.Usages.Select(x => x.Description)
                .ShouldHaveTheSameElementsAs("Only one", "only two", "All");

            commandReport.Usages.First().Usage
                .ShouldEqual("ripple crazy <arg1> [-r, --red] [-b, --blue] [-g, --green]");
        }

        [Test]
        public void dump_the_file()
        {
            theCommand.Execute(theInput).ShouldBeTrue();

            var report = new FileSystem().LoadFromFile<CommandLineApplicationReport>(theInput.Location);

            report.ShouldNotBeNull();
            report.Commands.Any().ShouldBeTrue();
        }

        [Test]
        public void can_read_usage_for_a_single_usage_command()
        {
            var simpleReport = report.Commands.Single(x => x.Name == "simple");
            simpleReport.Usages.Count().ShouldEqual(1);
        }
    }

    public class CrazyInput
    {
        [System.ComponentModel.Description("The first arg")]
        public string Arg1 { get; set; }

        [System.ComponentModel.Description("The second arg")]
        public string Arg2 { get; set; }

        [System.ComponentModel.Description("The third arg")]
        public string Arg3 { get; set; }

        [System.ComponentModel.Description("make it red")]
        public bool RedFlag { get; set; }
        [System.ComponentModel.Description("make it blue")]
        public bool BlueFlag { get; set; }
        [System.ComponentModel.Description("make it green")]
        public bool GreenFlag { get; set; }
    }

    [CommandDescription("The crazy command", Name = "crazy")]
    public class CrazyCommand : FubuCommand<CrazyInput>
    {
        public CrazyCommand()
        {
            Usage("Only one").Arguments(x => x.Arg1);
            Usage("only two").Arguments(x => x.Arg1, x => x.Arg2);
            Usage("All").Arguments(x => x.Arg1, x => x.Arg2, x => x.Arg3);
        }

        public override bool Execute(CrazyInput input)
        {
            throw new NotImplementedException();
        }
    }

    [CommandDescription("The simple one")]
    public class SingleCommand : FubuCommand<CrazyInput>
    {
        public override bool Execute(CrazyInput input)
        {
            throw new NotImplementedException();
        }
    }
}