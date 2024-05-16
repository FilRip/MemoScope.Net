using WinFwk.UIMessages;

namespace WinFwk.UIModules
{
    public enum ModuleEventType
    {
        Added,
        Removed,
        Activated
    }

    public class ModuleEventMessage(UIModule module, ModuleEventType moduleEvent) : AbstractUIMessage
    {
        public UIModule Module { get; private set; } = module;
        public ModuleEventType ModuleEvent { get; private set; } = moduleEvent;

        public override string ToString()
        {
            return $"Event: {ModuleEvent}, module: {Module.Name}";
        }
    }
}