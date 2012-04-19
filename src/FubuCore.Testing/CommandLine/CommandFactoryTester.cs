using System;
using System.Collections.Generic;
using FubuCore.CommandLine;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.CommandLine
{
    [TestFixture]
    public class CommandFactoryTester
    {
        [Test]
        public void get_the_command_name_for_a_class_not_decorated_with_the_attribute()
        {
            CommandFactory.CommandNameFor(typeof (MyCommand)).ShouldEqual("my");
        }

        [Test]
        public void get_the_command_name_for_a_class_not_ending_in_command()
        {
            CommandFactory.CommandNameFor(typeof(Silly)).ShouldEqual("silly");
        }
        [Test]
        public void get_the_command_name_for_a_class_that_has_a_longer_name()
        {
            CommandFactory.CommandNameFor(typeof(RebuildAuthorizationCommand)).ShouldEqual("rebuildauthorization");
        }

        [Test]
        public void get_the_command_name_for_a_class_decorated_with_the_attribute()
        {
            CommandFactory.CommandNameFor(typeof (DecoratedCommand)).ShouldEqual("this");
        }

        [Test]
        public void get_the_description_for_a_class_not_decorated_with_the_attribute()
        {
            CommandFactory.DescriptionFor(typeof (MyCommand)).ShouldEqual(typeof (MyCommand).FullName);
        }

        [Test]
        public void get_the_description_for_a_class_decorated_with_the_attribute()
        {
            CommandFactory.DescriptionFor(typeof (My2Command)).ShouldEqual("something");
        }

        [Test]
        public void get_the_command_name_for_a_class_decorated_with_the_attribute_but_without_the_name_specified()
        {
            CommandFactory.CommandNameFor(typeof (My2Command)).ShouldEqual("my2");
        }

        [Test]
        public void build()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            factory.Build("my").ShouldBeOfType<MyCommand>();
            factory.Build("my2").ShouldBeOfType<My2Command>();
            factory.Build("this").ShouldBeOfType<DecoratedCommand>();
        }

        [Test]
        public void trying_to_build_a_missing_command_will_list_the_existing_commands()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var commandRun = factory.BuildRun("junk");
            var theInput = commandRun.Input.ShouldBeOfType<HelpInput>();
            theInput.Name.ShouldEqual("junk");
            theInput.InvalidCommandName.ShouldBeTrue();
            commandRun.Command.ShouldBeOfType<HelpCommand>();
        }

        [Test]
        public void build_help_command_with_valid_name_argument()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var commandRun = factory.BuildRun("help my");
            var theInput = commandRun.Input.ShouldBeOfType<HelpInput>();
            theInput.Name.ShouldEqual("my");
            theInput.InvalidCommandName.ShouldBeFalse();
            theInput.Usage.ShouldNotBeNull();
            commandRun.Command.ShouldBeOfType<HelpCommand>();
        }

        [Test]
        public void build_help_command_with_invalid_name_argument()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var commandRun = factory.BuildRun("help junk");
            var theInput = commandRun.Input.ShouldBeOfType<HelpInput>();
            theInput.Name.ShouldEqual("junk");
            theInput.InvalidCommandName.ShouldBeTrue();
            theInput.Usage.ShouldBeNull();
            commandRun.Command.ShouldBeOfType<HelpCommand>();
        }

        [Test]
        public void build_command_from_a_string()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.BuildRun("my Jeremy --force");

            run.Command.ShouldBeOfType<MyCommand>();
            var input = run.Input.ShouldBeOfType<MyCommandInput>();

            input.Name.ShouldEqual("Jeremy");
            input.ForceFlag.ShouldBeTrue();
        }

        [Test]
        public void fetch_the_help_command_run()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.HelpRun(new Queue<string>());
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }

        [Test]
        public void fetch_the_help_command_if_the_args_are_empty()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.BuildRun(new string[0]);
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }

        [Test]
        public void fetch_the_help_command_if_the_command_is_help()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.BuildRun(new [] {"help"});
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }


        [Test]
        public void fetch_the_help_command_if_the_command_is_question_mark()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.BuildRun(new [] {"?"});
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }

        [Test]
        public void smoke_test_the_writing()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);
            factory.HelpRun(new Queue<string>()).Execute();
        }

        [Test]
        public void build_command_with_multiargs()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.BuildRun("my Jeremy -ft");
            var input = run.Input.ShouldBeOfType<MyCommandInput>();
            input.ForceFlag.ShouldBeTrue();
            input.SecondFlag.ShouldBeFalse();
            input.ThirdFlag.ShouldBeTrue();
        }

    }


    public class RebuildAuthorizationCommand : FubuCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }

    public class Silly : FubuCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }

    public class MyCommand : FubuCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }


    }

    [CommandDescription("something")]
    public class My2Command : FubuCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }

    [CommandDescription("this", Name = "this")]
    public class DecoratedCommand : FubuCommand<MyCommandInput>
    {
        public override bool Execute(MyCommandInput input)
        {
            throw new NotImplementedException();
        }
    }


    public class MyCommandInput
    {
        public string Name { get; set; }
        public bool ForceFlag { get; set; }
        public bool SecondFlag { get; set; }
        public bool ThirdFlag { get; set; }
    }
}