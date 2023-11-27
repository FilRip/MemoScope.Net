using System.Collections.Generic;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core.Data
{
    public class AddressList(ClrDump clrDump, ClrType clrType)
    {
        public IAddressContainer Addresses { get; private set; }
        public ClrDump ClrDump { get; } = clrDump;
        public ClrType ClrType { get; } = clrType;

        public AddressList(ClrDump clrDump, ClrType clrType, List<ulong> addresseList) : this(clrDump, clrType, new AddressContainerList(addresseList))
        {
        }
        public AddressList(ClrDump clrDump, ClrType clrType, IAddressContainer addresses) : this(clrDump, clrType)
        {
            Init(addresses);
        }

        protected void Init(IAddressContainer addresses)
        {
            Addresses = addresses;
        }
    }

    public interface IAddressContainer
    {
        int Count { get; }
        ulong this[int index] { get; }
    }
}