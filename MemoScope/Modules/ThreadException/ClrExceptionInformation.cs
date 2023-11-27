using System.Collections.Generic;
using System.Linq;

using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Modules.StackTrace;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.ThreadException
{
    public class ClrExceptionInformation(ClrDump clrDump, ClrException exception)
    {
        [OLVColumn(Width = 450)]
        public string TypeName { get; } = exception.Type.Name;
        [OLVColumn(Width = 500)]
        public string Message { get; } = clrDump.Eval(() => exception.Message);

        public List<StackFrameInformation> StackFrames { get; } = clrDump.Eval(() => exception.StackTrace.Select(frame => new StackFrameInformation(clrDump, frame))).ToList();
    }
}