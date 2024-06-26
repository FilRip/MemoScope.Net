﻿using System.Windows.Forms;

using WinFwk.UICommands;
using WinFwk.UIModules;
using WinFwk.UITools.ToolBar;

namespace WinFwk.UITools.Settings
{
    public class UISettingsCommand : AbstractVoidUICommand
    {
        public UISettingsCommand() : base("Settings", "Edit settings", UIToolBarSettings.Main.Name, Properties.Resources.gear_in, Keys.None, UIToolBarSettings.SubGroupOptions)
        {
        }

        public override void Run()
        {
            UIModuleFactory.CreateModule<UISettingsModule>(module => { }, module => DockModule(module));
        }
    }
}