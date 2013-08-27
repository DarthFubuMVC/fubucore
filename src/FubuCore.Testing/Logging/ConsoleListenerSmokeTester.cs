using System;
using FubuCore.Logging;
using NUnit.Framework;

namespace FubuCore.Testing.Logging
{
    [TestFixture]
    public class ConsoleListenerSmokeTester
    {
        ConsoleListener theListener = new ConsoleListener(Level.All);

        [Test]
        public void debug_message()
        {
            theListener.DebugMessage(this);
        }

        [Test]
        public void info_message()
        {
            theListener.InfoMessage(this);
        }

        [Test]
        public void debug()
        {
            theListener.Debug("Hello.");
        }

        [Test]
        public void info()
        {
            theListener.Info("Bye.");
        }

        [Test]
        public void error()
        {
            theListener.Error("what?", new NotImplementedException());
        }

        [Test]
        public void error_with_correlator()
        {
            theListener.Error(Guid.NewGuid(), "what happened?", new NotImplementedException());
        }
    }
}