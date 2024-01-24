using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core.Data
{
    public class TypeInstancesAddressList(ClrDump clrDump, ClrType clrType) : AddressList(clrDump, clrType)
    {
        public TypeInstancesAddressList(ClrDumpType clrDumpType) : this(clrDumpType.ClrDump, clrDumpType.ClrType)
        {

        }

        public TypeInstancesAddressList(ClrDump clrDump, string typeName) : this(clrDump, clrDump.GetClrType(typeName))
        {
        }

        public void Init()
        {
            Init(new TypeAddressContainer(ClrDump, ClrType));
        }
    }

    public class TypeAddressContainer(ClrDump clrDump, ClrType clrType) : IAddressContainer
    {
        readonly AddressContainerList addressList = new(clrDump.GetInstances(clrType));

        public ulong this[int index] => addressList[index];
        public int Count => addressList.Count;
    }
}