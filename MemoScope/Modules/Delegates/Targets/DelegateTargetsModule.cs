﻿using System.Collections.Generic;

using MemoScope.Core;
using MemoScope.Core.Data;
using MemoScope.Core.Helpers;

namespace MemoScope.Modules.Delegates.Targets
{
    public partial class DelegateTargetsModule : UIClrDumpModule
    {
        List<DelegateTargetInformation> delegateTargetInformations;
        ClrDumpObject clrDumpObject;

        public DelegateTargetsModule()
        {
            InitializeComponent();
        }

        public void Setup(ClrDumpObject clrDumpObject)
        {
            this.clrDumpObject = clrDumpObject;
            ClrDump = clrDumpObject.ClrDump;
            Icon = Properties.Resources.table_lightning_small;
            Name = $"#{ClrDump.Id} - {clrDumpObject.Address:X} - Targets";

            dlvDelegateTargets.InitColumns<DelegateTargetInformation>();
            dlvDelegateTargets.SetUpAddressColumn(this);
            dlvDelegateTargets.SetUpTypeColumn(this);
        }

        public override void Init()
        {
            delegateTargetInformations = DelegatesAnalysis.GetDelegateTargetInformations(clrDumpObject);
        }

        public override void PostInit()
        {
            Summary = $"{delegateTargetInformations.Count} Delegates Targets";
            dlvDelegateTargets.Objects = delegateTargetInformations;
        }
    }
}
