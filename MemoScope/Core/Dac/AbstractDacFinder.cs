using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Diagnostics.Runtime;
using Microsoft.Win32.SafeHandles;

namespace MemoScope.Core.Dac
{
    public abstract class AbstractDacFinder(string localCache) : IDisposable
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern LibrarySafeHandle LoadLibrary(string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        protected LibrarySafeHandle dbgHelpLib;
        protected Process process = Process.GetCurrentProcess();
        protected readonly string searchPath = $"SRV*{localCache}*http://msdl.microsoft.com/download/symbols";

        protected abstract bool SymCleanup(IntPtr hProcess);
        protected abstract bool SymInitialize(IntPtr hProcess, string symPath, bool fInvadeProcess);
        protected abstract void InitDbgHelpModule();
        protected abstract bool SymFindFileInPath(IntPtr hProcess, string searchPath, string filename, uint id, uint two, uint three, uint flags, StringBuilder filePath, IntPtr callback, IntPtr context);

        public void Init()
        {
            InitDbgHelpModule();

            if (dbgHelpLib.IsInvalid)
                throw new InvalidOperationException("Could not load dbghelp.dll", new Win32Exception(Marshal.GetLastWin32Error()));

            if (!SymInitialize(process.Handle, searchPath, false))
                throw new InvalidOperationException("SymInitialize: Unexpected error occured.", new Win32Exception(Marshal.GetLastWin32Error()));
        }

        public string FindDac(ClrInfo clrInfo)
        {
            var dac = FindDac(clrInfo.DacInfo.FileName, clrInfo.DacInfo.TimeStamp, clrInfo.DacInfo.FileSize);
            return dac;
        }

        public string FindDac(string dacname, uint timestamp, uint fileSize)
        {
            StringBuilder symbolFile = new(2048);
            if (SymFindFileInPath(process.Handle, searchPath, dacname, timestamp, fileSize, 0, 0x02, symbolFile, IntPtr.Zero, IntPtr.Zero))
            {
                return symbolFile.ToString();
            }

            throw new InvalidOperationException($"Unable to find dac file '{dacname}' in symbol server.", new Win32Exception(Marshal.GetLastWin32Error()));
        }

        public class LibrarySafeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public LibrarySafeHandle() : base(true)
            {
            }
            public LibrarySafeHandle(IntPtr handle) : base(true)
            {
                SetHandle(handle);
            }
            protected override bool ReleaseHandle()
            {
                return FreeLibrary(this.handle);
            }
        }

        private bool disposedValue;
        public bool IdDisposed
        {
            get { return disposedValue; }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                if (process != null)
                {
                    SymCleanup(process.Handle);
                    process = null;
                }
                if (dbgHelpLib == null || dbgHelpLib.IsClosed)
                    return;
                dbgHelpLib.Dispose();
                dbgHelpLib = null;
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}