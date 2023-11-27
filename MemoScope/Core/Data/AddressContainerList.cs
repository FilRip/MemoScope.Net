using System.Collections.Generic;

namespace MemoScope.Core.Data
{
    internal class AddressContainerList(List<ulong> addresses) : IAddressContainer
    {
        private readonly List<ulong> addresses = addresses;

        public ulong this[int index] => addresses[index];
        public int Count => addresses.Count;
    }
}