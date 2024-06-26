using System;
using System.Drawing;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using WinFwk.UIMessages;
using WinFwk.UIModules;

namespace WinFwk.UICommands
{
    public abstract class AbstractUICommand(string name, string toolTip, string group, Image icon, Keys shortcut = Keys.None, string subGroup = null)
    {
        // Properties
        public string Name { get; private set; } = name;
        public string Group { get; private set; } = group;
        public string SubGroup { get; private set; } = subGroup;
        public string ToolTip { get; private set; } = toolTip;
        public Image Icon { get; private set; } = icon;
        public bool Enabled { get; protected set; }
        public MessageBus MessageBus { get; private set; }
        public Keys Shortcut { get; private set; } = shortcut;

        protected UIModule selectedModule;

        public abstract void SetSelectedModule(UIModule module);
        public abstract void Run();

        public void SetMasterModule(UIModule module)
        {
            SetSelectedModule(module);
            if (module != null)
            {
                module.Disposed += OnDisposedMasterModule;
            }
        }

        private void OnDisposedMasterModule(object sender, EventArgs e)
        {
            if (selectedModule != null)
            {
                selectedModule.Disposed -= OnDisposedMasterModule;
            }
            SetSelectedModule(null);
            MessageBus.Unsubscribe(this);
        }

        public void InitBus(MessageBus msgBus)
        {
            MessageBus = msgBus;
            MessageBus.Subscribe(this);
        }
        protected void DockModule(UIModule uiModule, DockState dockState = DockState.Document)
        {
            DockModule(MessageBus, uiModule, dockState);
        }

        protected static void DockModule(MessageBus MessageBus, UIModule uiModule, DockState dockState = DockState.Document)
        {
            MessageBus.SendMessage(new DockRequest(uiModule, dockState));
        }
    }
}