using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MemoScope.Core.Dac
{
    public class DacFinder32(string searchPath) : AbstractDacFinder(searchPath)
    {
        private const string libDbghelpDll = "libs\\_x86\\dbghelp.dll";

        [DllImport(libDbghelpDll, EntryPoint = "SymInitialize", SetLastError = true)]
        private static extern bool SymInitialize32(IntPtr hProcess, string symPath, bool fInvadeProcess);

        [DllImport(libDbghelpDll, EntryPoint = "SymCleanup", SetLastError = true)]
        private static extern bool SymCleanup32(IntPtr hProcess);

        [DllImport(libDbghelpDll, EntryPoint = "SymFindFileInPath", SetLastError = true)]
        private static extern bool SymFindFileInPath32(IntPtr hProcess, string searchPath, string filename, uint id, uint two, uint three, uint flags, StringBuilder filePath, IntPtr callback, IntPtr context);

        protected override bool SymCleanup(IntPtr hProcess)
        {
            if (hProcess == IntPtr.Zero)
                return false;
            return SymCleanup32(hProcess);
        }

        protected override bool SymInitialize(IntPtr hProcess, string symPath, bool fInvadeProcess)
        {
            if (hProcess == IntPtr.Zero)
                return false;
            return SymInitialize32(hProcess, symPath, fInvadeProcess);
        }

        protected override void InitDbgHelpModule()
        {
            dbgHelpLib = LoadLibrary(libDbghelpDll);
        }

        protected override bool SymFindFileInPath(IntPtr hProcess, string searchPath, string filename, uint id, uint two, uint three, uint flags, StringBuilder filePath, IntPtr callback, IntPtr context)
        {
            if (hProcess == IntPtr.Zero)
                return false;
            return SymFindFileInPath32(hProcess, searchPath, filename, id, two, three, flags, filePath, callback, context);
        }
    }
}