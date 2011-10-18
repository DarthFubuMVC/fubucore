using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
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

            var run = factory.BuildRun("my Jeremy -force");

            run.Command.ShouldBeOfType<MyCommand>();
            var input = run.Input.ShouldBeOfType<MyCommandInput>();

            input.Name.ShouldEqual("Jeremy");
            input.ForceFlag.ShouldBeTrue();
        }


        private CommandFactory GetCustomValidFileCommandFactory(TextWriter textWriter = null)
        {
            var factory = new CommandFactory(textWriter ?? Console.Out);
            factory.RegisterCommands(GetType().Assembly);

            factory.AddCustomInputConvention(
                propetyInfo => propetyInfo.Name.EndsWith("FileName"),
                propertyInfo => new MyValidFileTokenHandler(propertyInfo)
            );
            return factory;
        }

        [Test]
        public void build_command_with_custom_input_handler()
        {
            var factory = GetCustomValidFileCommandFactory();

            string tempFileName = Path.GetTempFileName();

            var run = factory.BuildRun("file \"" + tempFileName + "\"");

            run.Command.ShouldBeOfType<ValidFileNameCommand>();
            var input = run.Input.ShouldBeOfType<ValidFileNameInput>();

            input.InputFileName.ShouldEqual(tempFileName);

            if (File.Exists(tempFileName))
                File.Delete(tempFileName);
        }

        [Test]
        public void build_command_with_custom_input_handler_but_fails_()
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);
            var factory = GetCustomValidFileCommandFactory(stringWriter);

            var run = factory.BuildRun("file \"M:\\FileShouldNotBeFound.txt\"");

            stringBuilder.ToString().ShouldContain("FileNotFoundException");
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

            var run = factory.BuildRun(new string[] {"help"});
            run.Command.ShouldBeOfType<HelpCommand>();
            run.Input.ShouldBeOfType<HelpInput>().CommandTypes
                .ShouldContain(typeof (MyCommand));
        }


        [Test]
        public void fetch_the_help_command_if_the_command_is_question_mark()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(GetType().Assembly);

            var run = factory.BuildRun(new string[] {"?"});
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
    }


    [CommandDescription("valid file name", Name = "file")]
    public class ValidFileNameCommand : FubuCommand<ValidFileNameInput>
    {
        public override bool Execute(ValidFileNameInput input)
        {
            throw new NotImplementedException();
        }
    }
    public class ValidFileNameInput
    {
        public string InputFileName { get; set; }
    }
    public class MyValidFileTokenHandler : TokenHandlerBase
    {
        private readonly PropertyInfo _property;

        public MyValidFileTokenHandler(PropertyInfo property)
            : base(property)
        {
            _property = property;
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            var requestedFileName = tokens.Dequeue();

            if (System.IO.File.Exists(requestedFileName))
            {
                _property.SetValue(input, requestedFileName, null);
                return true;
            }

            throw new FileNotFoundException("File supplied does not exist", requestedFileName);
        }


        public override string ToUsageDescription()
        {
            var flagName = InputParser.ToFlagName(_property);

            return "[{0} <{1}>]".ToFormat(flagName, _property.Name.ToLower());
        }
    }
}
