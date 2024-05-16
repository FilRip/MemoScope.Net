using System.Collections.Generic;
using System.Windows.Forms;

using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Core.Data;
using MemoScope.Core.Helpers;
using MemoScope.Modules.Arrays;
using MemoScope.Modules.Instances;

using WinFwk.UICommands;

namespace MemoScope.Modules.TypeStats
{
    public partial class TypeStatModule : UIClrDumpModule,
        IUIDataProvider<ClrDumpType>,
        IUIDataProvider<AddressList>,
        IUIDataProvider<ArraysAddressList>
    {
        private List<ClrTypeStats> typeStats;
        public TypeStatModule()
        {
            InitializeComponent();
            Icon = Properties.Resources.application_view_list;
        }

        public override void Close()
        {
            base.Close();
            ClrDump?.Dispose();
        }

        public void Setup(ClrDump clrDump)
        {
            ClrDump = clrDump;
            Name = $"#{ClrDump.Id} - " + clrDump.DumpPath;
            tbDumpPath.Text = clrDump.DumpPath;
        }

        public override void Init()
        {
            Log("Computing type statistics...", WinFwk.UITools.Log.LogLevelType.Info);
            if (ClrDump.Runtime != null)
            {
                typeStats = ClrDump.GetTypeStats();
                Summary = $"{typeStats.Count:###,###,###,##0} types";
            }
            else
            {
                Summary = $"Error. Dump file not loaded !";
            }
            Log("Type statistics computed.", WinFwk.UITools.Log.LogLevelType.Info);
        }

        public override void PostInit()
        {
            dlvTypeStats.InitColumns<ClrTypeStats>();
            dlvTypeStats.SetUpTypeColumn(this);
            dlvTypeStats.SetObjects(typeStats);
            dlvTypeStats.Sort(nameof(ClrTypeStats.NbInstances), SortOrder.Descending);

            dlvTypeStats.SetTypeNameFilter<ClrTypeStats>(regexFilterControl);
        }

        ClrDumpType IUIDataProvider<ClrDumpType>.Data
        {
            get
            {
                if (dlvTypeStats.SelectedObject is ClrTypeStats obj)
                {
                    return new ClrDumpType(ClrDump, obj.Type);
                }
                return null;
            }
        }

        AddressList IUIDataProvider<AddressList>.Data
        {
            get
            {
                var dumpType = ((IUIDataProvider<ClrDumpType>)this).Data;
                if (dumpType == null)
                {
                    return null;
                }
                TypeInstancesAddressList list = new(dumpType);
                list.Init();
                return list;
            }
        }

        ArraysAddressList IUIDataProvider<ArraysAddressList>.Data
        {
            get
            {
                var dumpType = ((IUIDataProvider<ClrDumpType>)this).Data;
                if (dumpType == null)
                {
                    return null;
                }
                if (!dumpType.IsArray)
                {
                    return null;
                }
                ArraysAddressList list = new(dumpType);
                return list;
            }
        }

        private void DlvTypeStats_CellClick(object sender, CellClickEventArgs e)
        {
            if (e.ClickCount != 2)
            {
                return;
            }
            var clrDumpType = ((IUIDataProvider<ClrDumpType>)this).Data;
            if (clrDumpType != null)
            {
                TypeInstancesModule.Create(clrDumpType, this, mod => RequestDockModule(mod));
            }
        }
    }
}
