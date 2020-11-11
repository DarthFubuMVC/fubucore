using System;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class FileChangePollingWatcherTester
    {
        private FileChangePollingWatcher theWatcher;
        private Mock<System.Action> action1;
        private Mock<System.Action> action2;
        private Mock<System.Action> action3;

        [SetUp]
        public void SetUp()
        {
            var system = new FileSystem();
            system.WriteStringToFile("a.txt", "something");
            system.WriteStringToFile("b.txt", "else");
            system.WriteStringToFile("c.txt", "altogether");
			
			Thread.Sleep(1000);
			
            theWatcher = new FileChangePollingWatcher();

            action1 = new Mock<System.Action>();
            action2 = new Mock<System.Action>();
            action3 = new Mock<System.Action>();
        
            theWatcher.WatchFile("a.txt", action1.Object);
            theWatcher.WatchFile("b.txt", action2.Object);
            theWatcher.WatchFile("c.txt", action3.Object);
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

            action1.Verify(x => x.Invoke());
            action2.VerifyNotCalled(x => x.Invoke());
            action3.VerifyNotCalled(x => x.Invoke());
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

            action1.Verify(x => x.Invoke());
            action2.VerifyNotCalled(x => x.Invoke());
            action3.Verify(x => x.Invoke());
        }
    }
}