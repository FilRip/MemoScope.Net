﻿using System.Collections.Generic;
using System.Linq;

using MemoScope.Core;
using MemoScope.Core.Helpers;

namespace MemoScope.Modules.Handles
{
    public partial class HandlesModule : UIClrDumpModule
    {
        private List<HandleInformation> Handles;
        public HandlesModule()
        {
            InitializeComponent();
        }

        public void Setup(ClrDump clrDump)
        {
            ClrDump = clrDump;
            Icon = Properties.Resources.plugin_link_small;
            Name = $"#{clrDump.Id} - Handles";

            dlvHandles.InitColumns<HandleInformation>();
        }

        public override void Init()
        {
            base.Init();
            Handles = ClrDump.Handles().Select(handle => new HandleInformation(handle)).ToList();
            dlvHandles.SetUpAddressColumn(nameof(HandleInformation.Object), this);
            dlvHandles.SetUpTypeColumn(this);
        }

        public override void PostInit()
        {
            base.PostInit();
            Summary = $"{Handles.Count} Handles";
            dlvHandles.Objects = Handles;
        }
    }
}
