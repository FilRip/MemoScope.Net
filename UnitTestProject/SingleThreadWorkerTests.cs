using System.Threading;
using System.Threading.Tasks;

using MemoScope.Core;

using NUnit.Framework;

namespace UnitTestProject
{
    [TestFixture()]
    public class SingleThreadWorkerTests
    {
        [Test()]
        public async Task RunAsyncTest()
        {
            using SingleThreadWorker worker = new("Test");
            string msg = null;
            bool done = false;

            worker.RunAsync(() => { msg = "Yo !"; }, () => { done = true; });

            await Task.Delay(100);

            Assert.That(msg, Is.EqualTo("Yo !"));
            Assert.That(done, Is.True);
        }

        [Test()]
        public void RunTest()
        {
            SingleThreadWorker worker = new("Test");
            string msg = null;
            worker.Run(() => msg = "Yo !");
            Assert.That(msg, Is.EqualTo("Yo !"));
            worker.Run(() => msg = "Yo !!!");
            Assert.That(msg, Is.EqualTo("Yo !!!"));
        }
    }
}