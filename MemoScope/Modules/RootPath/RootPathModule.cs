﻿using System.Collections.Generic;

using MemoScope.Core;
using MemoScope.Core.Data;
using MemoScope.Core.Helpers;

namespace MemoScope.Modules.RootPath
{
    public partial class RootPathModule : UIClrDumpModule
    {
        private List<RootPathInformation> RootPath { get; set; }
        private ClrDumpObject ClrDumpObject { get; set; }
        public RootPathModule()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            RootPath = RootPathAnalysis.AnalyzeRootPath(MessageBus, ClrDumpObject);

            dlvRootPath.InitColumns<RootPathInformation>();
            dlvRootPath.SetUpTypeColumn(this);
            dlvRootPath.SetUpAddressColumn(this);
        }

        internal void Setup(ClrDumpObject clrDumpObject)
        {
            ClrDump = clrDumpObject.ClrDump;
            Icon = Properties.Resources.molecule_small;
            ClrDumpObject = clrDumpObject;
            Name = $"#{ClrDump.Id} - RootPath- {ClrDumpObject.Address:X}";
        }

        public override void PostInit()
        {
            dlvRootPath.Objects = RootPath;
            Summary = $"";
        }
    }
}
