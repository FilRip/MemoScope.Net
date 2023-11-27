using WinFwk.UIMessages;

namespace MemoScope.Modules.Process
{
    public class DumpRequest(ProcessWrapper processWrapper) : AbstractUIMessage
    {
        public ProcessWrapper ProcessWrapper { get; private set; } = processWrapper;
    }
}