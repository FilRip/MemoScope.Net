using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core.Data
{
    public class ClrDumpThread(ClrDump clrDump, ClrThread thread, string name)
    {
        public ClrDump ClrDump { get; } = clrDump;
        public ClrThread ClrThread { get; } = thread;
        public string Name { get; } = name;
    }
}
