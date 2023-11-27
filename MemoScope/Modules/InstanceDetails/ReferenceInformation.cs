using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Core.Data;
using MemoScope.Core.Helpers;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.InstanceDetails
{
    public class ReferenceInformation(ClrDump clrDump, ulong address) : TreeNodeInformationAdapter<ReferenceInformation>, IAddressData, ITypeNameData
    {
        ClrDump ClrDump { get; } = clrDump;

        [OLVColumn(Title = "Address")]
        public ulong Address { get; } = address;

        [OLVColumn()]
        public string FieldName { get; }

        [OLVColumn(Title = "Type")]
        public string TypeName => ClrDump.GetObjectTypeName(Address);

        public ClrType ClrType => ClrDump.GetObjectType(Address);

        public ReferenceInformation(ClrDump clrDump, ulong address, ulong refAddress) : this(clrDump, address)
        {
            FieldName = ClrDump.GetFieldNameReference(refAddress, address);
            FieldName = TypeHelpers.RealName(FieldName);
        }

        public override bool CanExpand => ClrDump.HasReferers(Address);
    }
}
