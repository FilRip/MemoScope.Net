﻿using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.Handles
{
    public class HandleInformation(ClrHandle clrHandle) : ITypeNameData
    {
        private readonly ClrHandle clrHandle = clrHandle;

        [OLVColumn()]
        public ulong Object => clrHandle.Object;
        [OLVColumn(Title = "Name")]
        public string TypeName => clrHandle.Type.Name;
        [OLVColumn()]
        public HandleType HandleType => clrHandle.HandleType;
        [BoolColumn(Width = 50)]
        public bool IsPinned => clrHandle.IsPinned;
        [BoolColumn(Width = 50)]
        public bool IsStrong => clrHandle.IsStrong;
        [IntColumn(Width = 50)]
        public uint RefCount => clrHandle.RefCount;
    }
}