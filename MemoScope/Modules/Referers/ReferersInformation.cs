﻿using System.Collections.Generic;

using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UIMessages;
using WinFwk.UITools;

namespace MemoScope.Modules.Referers
{
    public class ReferersInformation(ClrDump clrDump, ClrType clrType, string fieldName, MessageBus messageBus, int parentCount) : TreeNodeInformationAdapter<ReferersInformation>, ITypeNameData
    {
        [OLVColumn()]
        public string TypeName => ClrType.Name;

        [IntColumn(Title = "# Instances")]
        public int InstancesCount => Instances.Count;

        [IntColumn(Title = "# References")]
        public int ReferencesCount => References.Count;

        [PercentColumn(Title = "References %")]
        public double ReferencesPercent => References.Count != 0 ? (double)References.Count / parentCount : 0;

        [OLVColumn()]
        public string FieldName { get; } = fieldName;

        public HashSet<ulong> Instances { get; } = [];
        public HashSet<ulong> References { get; } = [];

        public ClrType ClrType { get; } = clrType;
        ClrDump ClrDump { get; } = clrDump;
        MessageBus MessageBus { get; } = messageBus;

        private readonly int parentCount = parentCount;

        public ReferersInformation(ClrDump clrDump, ClrType clrType, MessageBus messageBus, IAddressContainer addresses) : this(clrDump, clrType, null, messageBus, 0)
        {
            for (int i = 0; i < addresses.Count; i++)
            {
                Instances.Add(addresses[i]);
            }
            Init();
        }

        public void Init()
        {
            CanExpand = ReferersAnalysis.HasReferers(ClrDump, Instances);
        }

        public override bool CanExpand { get; set; }

        public override List<ReferersInformation> Children
        {
            get
            {
                return ReferersAnalysis.AnalyzeReferers(MessageBus, ClrDump, Instances);
            }
        }
    }
}
