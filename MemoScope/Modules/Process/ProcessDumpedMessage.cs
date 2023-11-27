using WinFwk.UIMessages;

namespace MemoScope.Modules.Process
{
    public class ProcessDumpedMessage(string dumpPath, int id) : AbstractUIMessage
    {
        public string DumpPath { get; } = dumpPath;
        public int Id { get; } = id;
    }
}
