using System.Drawing;
using System.Windows.Forms;

using WinFwk.UIModules;

namespace WinFwk.UICommands
{
    public abstract class AbstractDataUICommand<T> : AbstractUICommand
    {
        protected IUIDataProvider<T> dataProvider;

        protected AbstractDataUICommand(string name, string toolTip, string group, Image icon, Keys shortcut = Keys.None, string subGroup = null) : base(name, toolTip, group, icon, shortcut, subGroup)
        {
            Enabled = false;
        }

        // Abstract
        protected abstract void HandleData(T data);

        public override void SetSelectedModule(UIModule module)
        {
            selectedModule = module;
            InitDataProvider(module as IUIDataProvider<T>);
        }

        public void InitDataProvider(IUIDataProvider<T> uiDataProvider)
        {
            dataProvider = uiDataProvider;
            Enabled = (dataProvider != null);
        }
        public override void Run()
        {
            if (dataProvider == null)
            {
                return;
            }

            T data = dataProvider.Data;
            HandleData(data);
        }
    }
}