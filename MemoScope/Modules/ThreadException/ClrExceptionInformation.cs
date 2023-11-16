using System.Collections.Generic;
using System.Linq;

using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Modules.StackTrace;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.ThreadException
{
    public class ClrExceptionInformation
    {
        [OLVColumn(Width = 450)]
        public string TypeName { get; }
        [OLVColumn(Width = 500)]
        public string Message { get; }

        public List<StackFrameInformation> StackFrames { get; }

        public ClrExceptionInformation(ClrDump clrDump, ClrException exception)
        {
            Message = clrDump.Eval(() => exception.Message);
            StackFrames = clrDump.Eval(() => exception.StackTrace.Select(frame => new StackFrameInformation(clrDump, frame))).ToList();
            TypeName = exception.Type.Name;
        }
    }
}