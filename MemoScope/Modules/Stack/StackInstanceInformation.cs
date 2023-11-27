using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.Stack
{

    public class StackInstanceInformation(ClrRoot clrRoot) : IAddressData, ITypeNameData
    {
        ClrRoot ClrRoot { get; } = clrRoot;

        [OLVColumn()]
        public ulong Address => ClrRoot.Object;

        [OLVColumn(Title = "Name")]
        public string TypeName => ClrRoot.Type?.Name;

        public ClrType Type => ClrRoot.Type;
    }
}