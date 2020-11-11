using FubuCore.Logging;
using NUnit.Framework;

namespace FubuCore.Testing.Logging
{
    [TestFixture]
    public class FilteredListenerTester
    {
        [Test]
        public void level_is_all()
        {
            var listener = new DebugListener(Level.All);
            listener.IsDebugEnabled.ShouldBeTrue();
            listener.IsInfoEnabled.ShouldBeTrue();
        }

        [Test]
        public void level_is_debug_only()
        {
            var listener = new DebugListener(Level.DebugOnly);
            listener.IsDebugEnabled.ShouldBeTrue();
            listener.IsInfoEnabled.ShouldBeFalse();
        }

        [Test]
        public void level_is_info_only()
        {
            var listener = new DebugListener(Level.InfoOnly);
            listener.IsDebugEnabled.ShouldBeFalse();
            listener.IsInfoEnabled.ShouldBeTrue();
        }

        [Test]
        public void listens_for_type_with_no_filters()
        {
            var listener = new DebugListener(Level.InfoOnly);
            listener.ListensFor(GetType()).ShouldBeTrue();
            listener.ListensFor(typeof(Log1)).ShouldBeTrue();
            listener.ListensFor(typeof(Log2)).ShouldBeTrue();
            listener.ListensFor(typeof(Log3)).ShouldBeTrue();
        }

        [Test]
        public void listen_by_exact_type()
        {
            var listener = new DebugListener(Level.InfoOnly);
            listener.ListenFor<Log1>();

            listener.ListensFor(typeof(Log1)).ShouldBeTrue();
            listener.ListensFor(typeof(Log2)).ShouldBeFalse();
        }

        [Test]
        public void listens_for_with_multiple_matches()
        {
            var listener = new DebugListener(Level.InfoOnly);
            listener.ListenFor<Log1>().ListenFor<Log2>();

            listener.ListensFor(typeof(Log1)).ShouldBeTrue();
            listener.ListensFor(typeof(Log2)).ShouldBeTrue();
            listener.ListensFor(typeof(Log3)).ShouldBeFalse();
        }

        [Test]
        public void listen_for_implementing()
        {
            var listener = new DebugListener(Level.All)
                .ListenForAnythingImplementing<LogRecord>();

            listener.ListensFor(GetType()).ShouldBeFalse();

            listener.ListensFor(typeof(Log1)).ShouldBeTrue();
            listener.ListensFor(typeof(Log2)).ShouldBeTrue();
            listener.ListensFor(typeof(Log3)).ShouldBeTrue();
        }

    }

    public class Log1 : LogRecord{}
    public class Log2 : LogRecord{}
    public class Log3 : LogRecord{}
}