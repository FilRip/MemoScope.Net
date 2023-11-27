using WinFwk.UIMessages;

namespace MemoScope.Core.Data
{
    public class SelectedClrDumpObjectMessage(ClrDumpObject selectedInstance) : AbstractUIMessage
    {
        public ClrDumpObject SelectedInstance { get; } = selectedInstance;
    }
}
