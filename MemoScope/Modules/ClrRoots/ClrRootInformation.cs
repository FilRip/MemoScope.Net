﻿using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.ClrRoots
{
    public class ClrRootsInformation(ClrRoot clrRoot) : IAddressData, ITypeNameData
    {
        ClrRoot ClrRoot { get; } = clrRoot;

        [AddressColumn()]
        public ulong Address => ClrRoot.Address;

        [BoolColumn()]
        public bool IsInterior => ClrRoot.IsInterior;
        [BoolColumn()]
        public bool IsPinned => ClrRoot.IsPinned;
        [BoolColumn()]
        public bool IsPossibleFalsePositive => ClrRoot.IsPossibleFalsePositive;
        [OLVColumn()]
        public GCRootKind Kind => ClrRoot.Kind;
        [AddressColumn()]
        public ulong ObjectAddress => ClrRoot.Object;

        [OLVColumn(Title = "Name", Width = 300)]
        public string Name => ClrRoot.Name;

        [OLVColumn(Title = "Type", Width = 300)]
        public string TypeName => ClrRoot.Type?.Name;

        public ClrType Type => ClrRoot.Type;
    }
}