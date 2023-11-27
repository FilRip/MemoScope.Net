using System.Collections.Generic;

using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.Finalizer
{
    public class FinalizerInformation(ClrDump clrDump, ClrType type, List<ulong> addresses) : ITypeNameData
    {
        public AddressList AddressList { get; } = new AddressList(clrDump, type, addresses);

        [OLVColumn()]
        public string TypeName => AddressList.ClrType.Name;

        [IntColumn()]
        public int Count => AddressList.Addresses.Count;
    }
}