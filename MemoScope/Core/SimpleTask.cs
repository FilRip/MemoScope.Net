using System;
using System.Threading.Tasks;

namespace MemoScope.Core
{
    public class SimpleTask(Action work, Action callback, Action<Exception> onError, TaskScheduler sched)
    {
        public Action Work { get; } = work;
        public Action Callback { get; } = callback;
        public Action<Exception> OnError { get; } = onError;

        public TaskScheduler Scheduler { get; } = sched;

        public SimpleTask(Action work, Action callback) : this(work, callback, null, null)
        {
        }

        public SimpleTask(Action work) : this(work, null)
        {
        }

        public SimpleTask(Action work, Action callback, TaskScheduler sched) : this(work, callback, null, sched)
        {
        }

        public SimpleTask(Action work, Action callback, Action<Exception> onError) : this(work, callback, onError, null)
        {
        }
    }
}