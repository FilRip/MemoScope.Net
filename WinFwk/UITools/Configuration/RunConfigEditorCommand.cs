using System.Drawing;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using WinFwk.UICommands;
using WinFwk.UIMessages;
using WinFwk.UIModules;
using WinFwk.UITools.ToolBar;

namespace WinFwk.UITools.Configuration
{
    public class RunConfigEditorCommand : AbstractTypedUICommand<IConfigurableModule>
    {
        public readonly static Image LargeIcon = Properties.Resources.wrench_orange;
        public readonly static Image SmallIcon = Properties.Resources.small_wrench_orange;

        public RunConfigEditorCommand() : base("Config", "Edit module's configuration", UIToolBarSettings.Main.Name, LargeIcon, Keys.None, UIToolBarSettings.SubGroupOptions)
        {
        }

        public override void HandleAction(IConfigurableModule configurableModule)
        {
            RunConfigEditor(configurableModule, MessageBus);
        }

        public static void RunConfigEditor(IConfigurableModule configurableModule, MessageBus messageBus)
        {
            IModuleConfig config = configurableModule.ModuleConfig;
            RunConfigEditor(configurableModule, messageBus, config);
        }

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        public static void RunConfigEditor(IConfigurableModule configurableModule, MessageBus messageBus, IModuleConfig moduleConfig)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
        {
            UIModuleFactory.CreateModule<UIConfigEditorModule>(module => module.Setup(configurableModule, moduleConfig), DockState.DockRight);
        }
    }
}
