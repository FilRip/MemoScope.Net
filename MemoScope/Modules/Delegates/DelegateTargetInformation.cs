using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.Delegates
{
    public class DelegateTargetInformation(ulong address, ClrDumpType clrDumpType) : IAddressData, ITypeNameData
    {
        private readonly ClrMethod methInfo;

        ClrDumpType ClrDumpType { get; } = clrDumpType;

        public DelegateTargetInformation(ulong address, ClrDumpType clrDumpType, ClrMethod methInfo) : this(address, clrDumpType)
        {
            this.methInfo = methInfo;
        }

        [OLVColumn()]
        public ulong Address { get; } = address;

        [OLVColumn(Title = "Type")]
        public string TypeName => ClrDumpType?.TypeName;

        [OLVColumn(Width = 500)]
        public string Method => methInfo?.GetFullSignature();
    }
}