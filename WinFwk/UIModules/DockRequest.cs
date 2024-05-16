using WeifenLuo.WinFormsUI.Docking;

using WinFwk.UIMessages;

namespace WinFwk.UIModules
{
    public class DockRequest(UIModule uiModule, DockState dockState = DockState.Document) : AbstractUIMessage
    {
        public UIModule UIModule { get; private set; } = uiModule;
        public DockState DockState { get; internal set; } = dockState;

        public override string ToString()
        {
            return $"DockState: {DockState}, Module: {UIModule.Name}";
        }
    }
}