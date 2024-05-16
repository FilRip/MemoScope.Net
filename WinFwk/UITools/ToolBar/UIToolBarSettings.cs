using System.Drawing;

using WeifenLuo.WinFormsUI.Docking;

namespace WinFwk.UITools.ToolBar
{
    public class UIToolBarSettings(string name, int priority, Bitmap icon, DockState dockState = DockState.DockTopAutoHide, bool mainToolbar = false)
    {
        public static readonly UIToolBarSettings Main = new("Main", int.MinValue, Properties.Resources.small_folder_vertical_open, DockState.DockTop, true);
        public static readonly UIToolBarSettings Help = new("Help", int.MaxValue, Properties.Resources.small_help, DockState.DockTop);

        public static readonly string SubGroupView = "View";
        public static readonly string SubGroupEdit = "Edit";
        public static readonly string SubGroupData = "Data";
        public static readonly string SubGroupOptions = "Options";
        public static readonly string SubGroupDebug = "Debug";
        public static readonly string SubGroupHelp = "Help";

        public string Name { get; } = name;
        public int Priority { get; } = priority;
        public Bitmap Icon { get; } = icon;
        public DockState DockState { get; } = dockState;
        public bool MainToolbar { get; } = mainToolbar;
    }
}
