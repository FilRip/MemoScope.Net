using WinFwk.UIMessages;

namespace WinFwk.UICommands
{
    public class UICommandRequest(IUICommandRequestor requestor) : AbstractUIMessage
    {
        public IUICommandRequestor Requestor { get; } = requestor;
    }
}
