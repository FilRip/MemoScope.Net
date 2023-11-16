using System;
using System.Collections.Generic;

namespace MemoScope.Modules.Process
{
    public sealed class ProcessWrapper : IEquatable<ProcessWrapper>, IEqualityComparer<ProcessWrapper>
    {
        public long VirtualMemory => Process.VirtualMemorySize64;
        public long PeakVirtualMemory => Process.PeakVirtualMemorySize64;

        public long PagedMemory => Process.PagedMemorySize64;
        public long PeakPagedMemory => Process.PeakPagedMemorySize64;

        public long PeakWorkingSet => Process.PeakWorkingSet64;
        public long WorkingSet => Process.WorkingSet64;

        public long PrivateMemory => Process.PrivateMemorySize64;
        public long HandleCount => Process.HandleCount;

        public TimeSpan TotalProcessorTime => Process.TotalProcessorTime;
        public DateTime StartTime => Process.StartTime;
        public TimeSpan UserProcessorTime => Process.UserProcessorTime;

        public System.Diagnostics.Process Process { get; }

        public ProcessWrapper(System.Diagnostics.Process process)
        {
            Process = process;
        }

        public bool Equals(ProcessWrapper other)
        {
            return other.Process.Id == Process.Id;
        }

        public override string ToString()
        {
            return string.Format("[{1}] {0}", Process.ProcessName, Process.Id);
        }

        public bool Equals(ProcessWrapper x, ProcessWrapper y)
        {
            if (x == null && y == null)
                return true;
            if (x == null)
                return false;
            return x.Equals(y);
        }

        public int GetHashCode(ProcessWrapper obj)
        {
            return obj.GetHashCode();
        }
    }
}