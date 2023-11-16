using System.ComponentModel;
using System.Threading;
namespace MemoDummy
{
    public class LockedThreadScript : AbstractMemoScript
    {
        public override string Name => "Locked Threads";
        public override string Description => "Creates some threads locking/waiting some objects";

        [Category("Config")]
        public long NbThreads { get; set; } = 4;

#pragma warning disable S1450 // Private fields only used as local variables in methods should become local variables
        private object[] locks;
#pragma warning restore S1450 // Private fields only used as local variables in methods should become local variables
        public override void Run()
        {
            locks = new string[NbThreads];
            for (int i = 0; i < NbThreads; i++)
            {
                locks[i] = $"lock_{i}";
            }

            for (int i = 0; i < NbThreads; i++)
            {
                object myLock = locks[i];
                Thread thread = new(() =>
                {
                    lock (myLock)
                    {
                        while (!stopRequested)
                        {
                            Thread.Sleep(100);
                        }
                    }
                })
                {
                    Name = $"thread #{i} LOCK"
                };
                thread.Start();
                Thread.Sleep(100);
                thread = new Thread(() =>
                {
                    lock (myLock)
                    {
                        while (!stopRequested)
                        {
                            Thread.Sleep(100);
                        }
                    }
                })
                {
                    Name = $"thread #{i} WAIT"
                };
                thread.Start();
            }
        }
    }
}
