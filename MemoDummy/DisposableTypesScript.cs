using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MemoDummy
{
    public class DisposableTypes : AbstractMemoScript
    {
        public override string Name => "Disposable Types";
        public override string Description => "Create instances of IDisposable type and don't call Dispose()";

        [Category("Config")]
        public int N { get; set; } = 300;

        public override void Run()
        {
            for (int i = 0; i < N; i++)
            {
                _ = new MyDisposableType();
            }

            GC.WaitForFullGCComplete();
        }
    }

    public class MyDisposableType : IDisposable
    {
        readonly IntPtr hglobal = Marshal.AllocHGlobal(100000);

        private bool dispoedValue;
        public bool IsDisposed
        {
            get { return dispoedValue; }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Marshal.FreeHGlobal(hglobal);
                dispoedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
