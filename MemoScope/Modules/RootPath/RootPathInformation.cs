using BrightIdeasSoftware;

using MemoScope.Core.Data;

using WinFwk.UITools;

namespace MemoScope.Modules.RootPath
{
    public class RootPathInformation(ClrDumpObject clrDumpObject, string fieldName) : ITypeNameData, IAddressData
    {
        [AddressColumn()]
        public ulong Address => ClrDumpObject.Address;

        [OLVColumn()]
        public string TypeName => ClrDumpObject.TypeName;

        [OLVColumn()]
        public string FieldName { get; } = fieldName;

        ClrDumpObject ClrDumpObject { get; } = clrDumpObject;
    }
}
