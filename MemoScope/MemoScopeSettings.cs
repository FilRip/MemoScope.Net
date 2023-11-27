using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using MemoScope.Core.Helpers;
using MemoScope.Tools.CodeTriggers;

using WinFwk.UITools.Settings;

namespace MemoScope
{
    public enum DockPanelPosition { Left, Right }
    public class MemoScopeSettings : UISettings
    {
        public new static MemoScopeSettings Instance => UISettings.Instance as MemoScopeSettings;

        [Category("_Main_")]
        public string RootDir { get; set; }

        [Category("Explorer")]
        public DockPanelPosition InitialPosition { get; set; } = DockPanelPosition.Left;

        [Category("Explorer")]
        public bool Visible { get; set; } = true;

        [Category("Display")]
        public List<TypeAlias> TypeAliases { get; set; } = [];

        [Category("Process Dump")]
        public string LastProcessName { get; set; }

        [Category("Process Dump")]
        public List<CodeTrigger> ProcessTriggers { get; set; } = [];

        [Category("Instances")]
        public List<CodeTrigger> InstanceFilters { get; set; } = [];

        public void Save()
        {
            UISettingsMgr<MemoScopeSettings>.Save(Application.ProductName, this);
        }
    }
}
