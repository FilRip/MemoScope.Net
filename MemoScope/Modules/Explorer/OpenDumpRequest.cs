using System.Collections.Generic;
using System.IO;

using WinFwk.UIMessages;

namespace MemoScope.Modules.Explorer
{
    public class OpenDumpRequest(IEnumerable<FileInfo> fileInfos) : AbstractUIMessage
    {
        public IEnumerable<FileInfo> FileInfos { get; } = fileInfos;
    }
}