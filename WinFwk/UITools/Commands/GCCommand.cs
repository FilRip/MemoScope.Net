using System;
using System.Windows.Forms;

using WinFwk.UICommands;
using WinFwk.UITools.ToolBar;

namespace WinFwk.UITools.Commands
{
    public class GCCommand : AbstractVoidUICommand
    {
        public GCCommand() : base("GC", "Garbage collect", UIToolBarSettings.Help.Name, Properties.Resources.bin, Keys.Control | Keys.Shift | Keys.V, UIToolBarSettings.SubGroupDebug)
        {

        }

        public override void Run()
        {
#pragma warning disable S1215 // "GC.Collect" should not be called
            GC.Collect(2, GCCollectionMode.Forced, true);
#pragma warning restore S1215 // "GC.Collect" should not be called
        }
    }
}
