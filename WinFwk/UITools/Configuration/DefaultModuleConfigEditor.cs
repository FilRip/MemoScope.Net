using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFwk.UITools.Configuration
{

    public partial class DefaultModuleConfigEditor : UserControl, IModuleConfigEditor
    {
        public IModuleConfig ModuleConfig => propertyGrid.SelectedObject as IModuleConfig;
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
        public IEnumerable<IEditorAction> EditorActions => null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

        public DefaultModuleConfigEditor()
        {
            InitializeComponent();
        }

        public void Init(IModuleConfig moduleConfig)
        {
            propertyGrid.SelectedObject = moduleConfig;
        }
    }
}
