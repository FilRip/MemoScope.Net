using System.Windows.Forms;

using WinFwk.UIModules;

namespace WinFwk.UITools.Configuration
{
    public partial class UIConfigEditorModule : UIModule
    {
        private IModuleConfig original;
        private IModuleConfig workingCopy;
        private IModuleConfigEditor editor;
        private IConfigurableModule ownerModule;

        public UIConfigEditorModule()
        {
            InitializeComponent();
        }

        public void Setup(IConfigurableModule module, IModuleConfig moduleConfig)
        {
            ownerModule = module;
            // Todo : find something to avoid those cast / as
            UIModuleParent = module as UIModule;
            editor = module.CreateEditor();
            if (editor is not UserControl editorControl)
            {
                return;
            }
            Name = moduleConfig?.GetType().Name;
            Icon = Properties.Resources.small_wrench_orange;
            Summary = moduleConfig?.ToString();

            original = moduleConfig;
            ResetEditor();
            editorControl.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(editorControl);

            if (editor.EditorActions != null)
            {
                toolStrip1.Items.Add(new ToolStripSeparator());
                foreach (IEditorAction action in editor.EditorActions)
                {
                    ToolStripButton tsb = new()
                    {
                        ToolTipText = action.Text,
                        Image = action.Icon
                    };
                    tsb.Click += (o, e) => action.DoWork();
                    toolStrip1.Items.Add(tsb);
                }
            }
        }

        private void ResetEditor()
        {
            if (original == null)
            {
                workingCopy = null;
            }
            else
            {
                workingCopy = original.Clone() as IModuleConfig;
            }
            editor.Init(workingCopy);
        }

        private void TsbUndo_Click(object sender, System.EventArgs e)
        {
            ResetEditor();
        }

        private void TsbApply_Click(object sender, System.EventArgs e)
        {
            IModuleConfig config = editor.ModuleConfig;
            ownerModule.Apply(config);
        }
    }
}
