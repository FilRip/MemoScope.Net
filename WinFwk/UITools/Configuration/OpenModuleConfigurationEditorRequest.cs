using WinFwk.UIMessages;

namespace WinFwk.UITools.Configuration
{
    public class OpenModuleConfigurationEditorRequest(IConfigurableModule configurableModule, IModuleConfig moduleConfig) : AbstractUIMessage
    {
        public IConfigurableModule ConfigurableModule { get; } = configurableModule;
        public IModuleConfig ModuleConfig { get; } = moduleConfig;
    }
}
