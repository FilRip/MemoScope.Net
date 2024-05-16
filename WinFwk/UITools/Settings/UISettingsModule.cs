using System;
using System.Reflection;

using WinFwk.UIModules;

namespace WinFwk.UITools.Settings
{
    public partial class UISettingsModule : UIModule
    {
        public UISettingsModule()
        {
            InitializeComponent();
            Name = "Settings";
            Icon = Properties.Resources.small_gear_in;
            Summary = "Application settings";
        }

        private void UIConfigModule_Load(object sender, EventArgs e)
        {
            pgUiSettings.SelectedObject = UISettings.Instance;
            pgUiSettings.CollapseAllGridItems();
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            Type type = UISettings.Instance.GetType();
            Type mgrType = typeof(UISettingsMgr<>).MakeGenericType(type);
            MethodInfo meth = mgrType.GetMethod(nameof(UISettingsMgr<UISettings>.Save), [type]);
            meth.Invoke(null, [UISettings.Instance]);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            Type type = UISettings.Instance.GetType();
            Type mgrType = typeof(UISettingsMgr<>).MakeGenericType(type);
            MethodInfo meth = mgrType.GetMethod(nameof(UISettingsMgr<UISettings>.Load), []);
            object res = meth.Invoke(null, null);
            UISettings.InitSettings((UISettings)res);
            pgUiSettings.SelectedObject = UISettings.Instance;
        }

        public void SendUISettingsChangedMessage()
        {
            MessageBus.SendMessage(new UISettingsChangedMessage(UISettings.Instance));
        }

        private void BtnApplyUISettingsChanges_Click(object sender, EventArgs e)
        {
            SendUISettingsChangedMessage();
        }
    }
}