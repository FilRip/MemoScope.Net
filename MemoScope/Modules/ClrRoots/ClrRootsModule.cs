using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using MemoScope.Core;
using MemoScope.Core.Data;
using MemoScope.Core.Helpers;

using WinFwk.UICommands;

namespace MemoScope.Modules.ClrRoots
{
    public partial class ClrRootsModule : UIClrDumpModule, IUIDataProvider<ClrDumpType>
    {
        private List<ClrRootsInformation> ClrRootsQueue;

        public ClrRootsModule()
        {
            InitializeComponent();
        }

        public void Setup(ClrDump clrDump)
        {
            ClrDump = clrDump;
            Icon = Properties.Resources.broom_small;
            Name = $"#{clrDump.Id} - ClrRoots";

            dlvClrRoots.InitColumns<ClrRootsInformation>();
            dlvClrRoots.SetUpAddressColumn(this);
            dlvClrRoots.SetUpAddressColumn(nameof(ClrRootsInformation.ObjectAddress), this);
            dlvClrRoots.SetUpTypeColumn(this);

            dlvClrRoots.SetRegexFilter(rfcName, o => ((ClrRootsInformation)o).Name);
        }

        public override void Init()
        {
            base.Init();
            ClrRootsQueue = ClrDump.GetClrRoots().Select(clrRoot => new ClrRootsInformation(clrRoot)).ToList();
        }

        public override void PostInit()
        {
            base.PostInit();
            Summary = $"{ClrRootsQueue.Count:###,###,###,##0} ClrRoots";
            dlvClrRoots.Objects = ClrRootsQueue;
            var colGroup = dlvClrRoots[nameof(ClrRootsInformation.Kind)];
            dlvClrRoots.BuildGroups(colGroup, SortOrder.Ascending);
            dlvClrRoots.ShowGroups = true;
        }

        ClrDumpType IUIDataProvider<ClrDumpType>.Data
        {
            get
            {
                var ClrRootsInformation = dlvClrRoots.SelectedObject<ClrRootsInformation>();
                if (ClrRootsInformation != null)
                {
                    return new ClrDumpType(ClrDump, ClrRootsInformation.Type);
                }
                return null;
            }
        }

    }
}
