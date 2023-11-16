using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.Stack
{

    public class StackInstanceInformation : IAddressData, ITypeNameData
    {
        ClrRoot ClrRoot { get; }
        public StackInstanceInformation(ClrRoot clrRoot)
        {
            ClrRoot = clrRoot;
        }

        [OLVColumn()]
        public ulong Address => ClrRoot.Object;

        [OLVColumn(Title = "Name")]
        public string TypeName => ClrRoot.Type?.Name;

        public ClrType Type => ClrRoot.Type;
    }
}