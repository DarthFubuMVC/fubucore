using System;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;

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

            theWatcher = new FileChangePollingWatcher();

            action1 = MockRepository.GenerateMock<System.Action>();
            action2 = MockRepository.GenerateMock<System.Action>();
            action3 = MockRepository.GenerateMock<System.Action>();
        
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
            theWatcher.StartWatching(1000);
            new FileSystem().WriteStringToFile("a.txt", "more");

            var reset = new ManualResetEvent(false);
            theWatcher.PollingCallback = () => reset.Set();

            reset.WaitOne(2000);

            action1.AssertWasCalled(x => x.Invoke());
            action2.AssertWasNotCalled(x => x.Invoke());
            action3.AssertWasNotCalled(x => x.Invoke());
        }


        [Test]
        public void catch_changes_2()
        {
            theWatcher.StartWatching(1000);
            new FileSystem().WriteStringToFile("a.txt", "more");
            new FileSystem().WriteStringToFile("c.txt", "more");

            var reset = new ManualResetEvent(false);
            theWatcher.PollingCallback = () => reset.Set();

            reset.WaitOne(2000);

            action1.AssertWasCalled(x => x.Invoke());
            action2.AssertWasNotCalled(x => x.Invoke());
            action3.AssertWasCalled(x => x.Invoke());
        }
    }
}