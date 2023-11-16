using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MemoDummy
{
    public class FragmentedMemoryScript : AbstractMemoScript
    {
        public override string Name => "Fragmented Memory";
        public override string Description => "Creates some large objects and destroy them until out of memory happens because of memory fragmentation";

        private readonly List<double[]> buffers = new();
        public string PrivateMemory => $"{Process.GetCurrentProcess().PrivateMemorySize64:###,###,###,##0}";
        public string WorkingSet => $"{Process.GetCurrentProcess().WorkingSet64:###,###,###,##0}";

        public override void Run()
        {
            const long M = 1000 * 1000;
            const long LARGE_BUF = 10 * M;
            const long MEDIUM_BUF = 8 * M;

            try
            {
                while (true)
                {
                    _ = new double[LARGE_BUF];
                    var y = new double[MEDIUM_BUF];
                    buffers.Add(y);
                }
            }
            catch (OutOfMemoryException)
            {
#pragma warning disable S1215 // "GC.Collect" should not be called
                GC.Collect();
#pragma warning restore S1215 // "GC.Collect" should not be called
                GC.WaitForFullGCComplete(10 * 1000);
            }
        }
    }
}
