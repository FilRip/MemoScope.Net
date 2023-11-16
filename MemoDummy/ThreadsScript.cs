using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;

namespace MemoDummy
{
    public class ThreadsScript : AbstractMemoScript
    {
        public override string Name => "Threads";
        public override string Description => "Create N threads";

        [Category("Config")]
        public int N { get; set; } = 8;

        public override void Run()
        {
            for (int i = 0; i < N; i++)
            {
                Thread t = new(() => DoSomething(i))
                {
                    Name = $"Thread #{i}"
                };
                t.Start();
                switch (i % 8)
                {
                    case 0:
                        t.Abort();
                        break;
                    case 1:
                        t.DisableComObjectEagerCleanup();
                        break;
                    case 2:
                        t.IsBackground = true;
                        break;
                    case 3:
                        t.Interrupt();
                        break;
                    case 4:
                        t.Priority = ThreadPriority.AboveNormal;
                        break;
                    case 5:
                        t.Priority = ThreadPriority.BelowNormal;
                        break;
                    case 6:
                        t.Priority = ThreadPriority.Highest;
                        break;
                    case 7:
                        t.Priority = ThreadPriority.Lowest;
                        break;
                }
            }
        }

#pragma warning disable S1172, IDE0060 // Unused method parameters should be removed
        private void DoSomething(int n)
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(100);
                }
                catch { /* Ignore errors */ }
                long j = 0;
                for (int i = 0; i < 1000; i++)
                {
                    j = Syracuse(j);
                    if (j < 0)
                    {
#pragma warning disable S112 // General exceptions should never be thrown
                        throw new Exception();
#pragma warning restore S112 // General exceptions should never be thrown
                    }
                }
            }
        }
#pragma warning restore S1172, IDE0060 // Unused method parameters should be removed

        private long Syracuse(long j)
        {
            if (j % 2 == 0)
            {
                return j / 2;
            }
            return 3 * j + 1;
        }
    }
}
