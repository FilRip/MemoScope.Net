using System.Diagnostics;
using System.IO;

using BrightIdeasSoftware;

using MemoScope.Core;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.Modules
{
    public class ModuleInformation(ClrDump clrDump, ClrModule module)
    {
        private readonly ClrModule module = module;

        [OLVColumn()]
        public string Name => Path.GetFileName(module.AssemblyName);
        [IntColumn()]
        public ulong Size => module.Size;
        [OLVColumn()]
        public DebuggableAttribute.DebuggingModes DebuggingMode { get; } = module.DebuggingMode;
        [OLVColumn()]
        public string FileName => module.FileName;

        [BoolColumn()]
        public bool IsDynamic => module.IsDynamic;
        [BoolColumn()]
        public bool IsFile => module.IsFile;
        [OLVColumn()]
        public string Pdb { get; } = clrDump.Eval(() =>
           {
               if (module.IsFile && module.Pdb != null)
               {
                   return module.Pdb.FileName;
               }
               return null;
           });
    }
}