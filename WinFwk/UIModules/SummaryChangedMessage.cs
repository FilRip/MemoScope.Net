using WinFwk.UIMessages;

namespace WinFwk.UIModules
{
    public class SummaryChangedMessage(UIModule module) : AbstractUIMessage
    {
        public UIModule Module { get; private set; } = module;

        public override string ToString()
        {
            return $"Module: {Module.Name}, Summary: {Module.Summary}";
        }
    }
}