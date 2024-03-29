﻿using System.Collections.Generic;

using BrightIdeasSoftware;

using WinFwk.UITools;

namespace MemoScope.Modules.Strings
{
    public class StringInformation(string value, List<ulong> addresses)
    {
        public List<ulong> Addresses { get; } = addresses;

        [IntColumn(Width = 200)]
        public int Count => Addresses.Count;

        [OLVColumn(Width = 500)]
        public string Value { get; } = value;

        [IntColumn(Width = 300, AspectToStringFormat = "{0:###,###,###,##0}")]
        public int Bytes => Value.Length * Count;


        [IntColumn(Width = 300, AspectToStringFormat = "{0:###,###,###,##0}")]
        public int Length => Value.Length;
    }
}