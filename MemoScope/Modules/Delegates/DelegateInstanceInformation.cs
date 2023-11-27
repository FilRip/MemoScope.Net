using System.Linq;

using BrightIdeasSoftware;

using MemoScope.Core.Data;

using WinFwk.UITools;

namespace MemoScope.Modules.Delegates
{
    public class DelegateInstanceInformation(ulong address, ClrDumpType clrDumpType, long targetCount) : IAddressData, ITypeNameData
    {
        ClrDumpType ClrDumpType { get; } = clrDumpType;

        [OLVColumn()]
        public ulong Address { get; } = address;

        [IntColumn()]
        public long Targets { get; } = targetCount;

        [OLVColumn(Title = "Type")]
        public string TypeName => ClrDumpType.TypeName;

        [OLVColumn(Width = 250)]
        public string FieldName
        {
            get
            {
                var referers = ClrDumpType.ClrDump.GetReferers(Address);
                var fieldNames = referers.Where(a => a != Address).Take(10).Select(a => ClrDumpType.ClrDump.GetFieldNameReference(Address, a, true));
                return string.Join("; ", fieldNames.Distinct());
            }
        }
    }
}