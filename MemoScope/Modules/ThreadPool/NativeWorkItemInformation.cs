using BrightIdeasSoftware;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.ThreadPool
{
    public class NativeWorkItemInformation(NativeWorkItem workItem)
    {
        private readonly NativeWorkItem workItem = workItem;

        [OLVColumn()]
        public WorkItemKind Kind => workItem.Kind;
        [OLVColumn()]
        public ulong Data => workItem.Data;
    }
}