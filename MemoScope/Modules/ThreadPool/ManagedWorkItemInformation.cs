using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.ThreadPool
{
    public class ManagedWorkItemInformation(ManagedWorkItem workItem) : ITypeNameData
    {
        private readonly ManagedWorkItem workItem = workItem;

        [OLVColumn()]
        public ulong Object => workItem.Object;
        [OLVColumn(Title = "Type")]
        public string TypeName => workItem.Type.Name;
    }
}