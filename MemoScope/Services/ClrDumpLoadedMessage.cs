using MemoScope.Core;

using WinFwk.UIMessages;

namespace MemoScope.Services
{
    public class ClrDumpLoadedMessage(ClrDump clrDump) : AbstractUIMessage
    {
        public ClrDump ClrDump { get; } = clrDump;
    }
}
