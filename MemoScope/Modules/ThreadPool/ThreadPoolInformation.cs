using MemoScope.Core;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.ThreadPool
{
    public class ThreadPoolInformation(ClrDump clrDump, ClrThreadPool threadPool)
    {
        public ClrDump ClrDump { get; } = clrDump;
        public ClrThreadPool ThreadPool { get; } = threadPool;

        public int CpuUtilization => ThreadPool.CpuUtilization;
        public int FreeCompletionPortCount => ThreadPool.FreeCompletionPortCount;
        public int IdleThreads => ThreadPool.IdleThreads;
        public int MaxCompletionPorts => ThreadPool.MaxCompletionPorts;
        public int MaxFreeCompletionPorts => ThreadPool.MaxFreeCompletionPorts;
        public int MaxThreads => ThreadPool.MaxThreads;
        public int MinCompletionPorts => ThreadPool.MinCompletionPorts;
        public int MinThreads => ThreadPool.MinThreads;
        public int RunningThreads => ThreadPool.RunningThreads;
        public int TotalThreads => ThreadPool.TotalThreads;
    }
}