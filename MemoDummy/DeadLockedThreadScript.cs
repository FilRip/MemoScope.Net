﻿using System.Threading;
namespace MemoDummy
{
    public class DeadLockedThreadScript : AbstractMemoScript
    {
        public override string Name => "Dead Locked Threads";
        public override string Description => "Creates two threads dead locked on the same objects";

        public override void Run()
        {
            object lockB = "Lock_A";
            object lockA = "Lock_B";
            Thread thread = new(() =>
            {
                lock (lockA)
                {
                    Thread.Sleep(100);
                    lock (lockB)
                    {
                        while (!stopRequested)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            })
            {
                Name = $"thread 1: locks A then B"
            };
            thread.Start();

            thread = new Thread(() =>
            {
                lock (lockB)
                {
                    lock (lockA)
                    {
                        while (!stopRequested)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            })
            {
                Name = $"thread 2: locks B then A"
            };
            thread.Start();
        }
    }
}
