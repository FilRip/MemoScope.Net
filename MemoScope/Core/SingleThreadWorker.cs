using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MemoScope.Core
{
    public class SingleThreadWorker : IDisposable
    {
        private readonly BlockingCollection<SimpleTask> queue = [];
        private bool stopRequested;
        public string Name { get; }

        private Thread WorkerThread { get; }

        public SingleThreadWorker(string name)
        {
            Name = name;
            WorkerThread = new Thread(Run) { Name = name, IsBackground = true };
            WorkerThread.Start();
        }

        public bool Active => !stopRequested && WorkerThread.IsAlive;

        private bool disposedValue;
        public bool IsDisposed
        {
            get { return disposedValue; }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                stopRequested = true;
                disposedValue = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Run()
        {
            while (!stopRequested)
            {

                if (!queue.TryTake(out SimpleTask task, TimeSpan.FromMilliseconds(100)))
                {
                    continue;
                }
                try
                {
                    task.Work();
                }
                catch (Exception e)
                {
                    task.OnError?.Invoke(e);
                }
                if (task.Callback != null)
                {
                    void callback()
                    {
                        try
                        {
                            task.Callback();
                        }
                        catch (Exception ex)
                        {
                            task.OnError?.Invoke(ex);
                        }
                    }
                    if (task.Scheduler == null)
                    {
                        Task.Factory.StartNew(callback);
                    }
                    else
                    {
                        Task.Factory.StartNew(callback, CancellationToken.None, TaskCreationOptions.None, task.Scheduler);
                    }
                }
            }
        }

        public void RunAsync(Action work, Action callback)
        {
            queue.Add(new SimpleTask(work, callback));
        }

        public void RunAsync(Action work, Action callback, TaskScheduler sched)
        {
            queue.Add(new SimpleTask(work, callback, sched));
        }

        public void Run(Action work)
        {
            ManualResetEvent myEvent = new(false);
            queue.Add(new SimpleTask(work, () => myEvent.Set()));
            myEvent.WaitOne();
        }
        public void Run(Action work, Action<Exception> onError)
        {
            ManualResetEvent myEvent = new(false);
            queue.Add(new SimpleTask(work, () => myEvent.Set(), onError));
            myEvent.WaitOne();
        }

        public T Eval<T>(Func<T> func)
        {
            var result = default(T);
            Run(() => result = func());
            return result;
        }
    }
}