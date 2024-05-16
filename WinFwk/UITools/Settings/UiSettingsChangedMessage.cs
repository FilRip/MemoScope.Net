using WinFwk.UIMessages;

namespace WinFwk.UITools.Settings
{
    public class UISettingsChangedMessage(UISettings uiSettings) : AbstractUIMessage
    {
        public UISettings UiSettings { get; set; } = uiSettings;
    }
}