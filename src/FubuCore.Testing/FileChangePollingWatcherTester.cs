using System;
using System.Threading;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class FileChangePollingWatcherTester
    {
        private FileChangePollingWatcher theWatcher;
        private Action action1;
        private Action action2;
        private Action action3;

        [SetUp]
        public void SetUp()
        {
            var system = new FileSystem();
            system.WriteStringToFile("a.txt", "something");
            system.WriteStringToFile("b.txt", "else");
            system.WriteStringToFile("c.txt", "altogether");
			
			Thread.Sleep(1000);
			
            theWatcher = new FileChangePollingWatcher();

            action1 = Substitute.For<System.Action>();
            action2 = Substitute.For<System.Action>();
            action3 = Substitute.For<System.Action>();
        
            theWatcher.WatchFile("a.txt", action1);
            theWatcher.WatchFile("b.txt", action2);
            theWatcher.WatchFile("c.txt", action3);
        }

        [TearDown]
        public void TearDown()
        {
            theWatcher.Stop();
        }

        [Test]
        public void catch_changes_1()
        {
            theWatcher.StartWatching(1500);
            new FileSystem().WriteStringToFile("a.txt", "more");

            var reset = new ManualResetEvent(false);
            theWatcher.PollingCallback = () => reset.Set();

            reset.WaitOne(2500);

            action1.Received().Invoke();
            action2.Received(0).Invoke();
            action3.Received(0).Invoke();
        }


        [Test]
        public void catch_changes_2()
        {
            theWatcher.StartWatching(1500);
            new FileSystem().WriteStringToFile("a.txt", "more");
            new FileSystem().WriteStringToFile("c.txt", "more");

            var reset = new ManualResetEvent(false);
            theWatcher.PollingCallback = () => reset.Set();

            reset.WaitOne(2500);

            action1.Received().Invoke();
            action2.Received(0).Invoke();
            action3.Received().Invoke();
        }
    }
}