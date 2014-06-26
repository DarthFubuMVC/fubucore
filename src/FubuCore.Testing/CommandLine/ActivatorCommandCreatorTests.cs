using System;
using FubuCore.CommandLine;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.CommandLine
{
    public class ActivatorCommandCreatorTests
    {
        [Test]
        public void builds_an_instance_with_no_ctor_parameters()
        {
            var creator = new ActivatorCommandCreator();
            var instance = creator.Create(typeof (NoParamsCommand));

            instance.ShouldBeOfType<NoParamsCommand>();
        }

        [Test]
        public void throws_if_the_ctor_has_parameters()
        {
            var creator = new ActivatorCommandCreator();

            Assert.Throws<MissingMethodException>(() => creator.Create(typeof (ParamsCommand)));
        }

        public class FakeModel
        {
        }

        private class NoParamsCommand : FubuCommand<FakeModel>
        {
            public override bool Execute(FakeModel input)
            {
                throw new System.NotImplementedException();
            }
        }

        private class ParamsCommand : FubuCommand<FakeModel>
        {
            public ParamsCommand(string testArgument)
            {
            }

            public override bool Execute(FakeModel input)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
