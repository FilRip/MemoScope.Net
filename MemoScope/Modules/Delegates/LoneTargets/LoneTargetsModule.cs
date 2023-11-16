using System.Collections.Generic;

using MemoScope.Core;
using MemoScope.Core.Helpers;

namespace MemoScope.Modules.Delegates.LoneHandlers
{
    public partial class LoneTargetsModule : UIClrDumpModule
    {
        List<LoneTargetInformation> loneTargetInformations;

        public LoneTargetsModule()
        {
            InitializeComponent();
        }

        public void Setup(ClrDump clrDump)
        {
            ClrDump = clrDump;
            Icon = Properties.Resources.table_lightning_small;
            Name = $"#{ClrDump.Id} - Lone Targets";

            dlvLoneHandlers.InitColumns<LoneTargetInformation>();
            dlvLoneHandlers.SetUpAddressColumn<LoneTargetInformation>(this);
            dlvLoneHandlers.SetUpTypeColumn<LoneTargetInformation>(this);
            dlvLoneHandlers.SetUpAddressColumn(nameof(LoneTargetInformation.OwnerAddress), this, "Owner");
            dlvLoneHandlers.SetUpTypeColumn(nameof(LoneTargetInformation.OwnerType), this, "Owner");

        }

        public override void Init()
        {
            loneTargetInformations = DelegatesAnalysis.GetLoneTargetInformations(ClrDump);
        }

        public override void PostInit()
        {
            Summary = $"{loneTargetInformations.Count} Lone Targets";
            dlvLoneHandlers.Objects = loneTargetInformations;
        }
    }
}
