using WinFwk.UIMessages;

namespace WinFwk.UIModules
{
    public class CloseRequest(UIModule module) : AbstractUIMessage
    {
        public UIModule Module { get; private set; } = module;

        public override string ToString()
        {
            return $"Module: {Module.Name}";
        }
    }
}