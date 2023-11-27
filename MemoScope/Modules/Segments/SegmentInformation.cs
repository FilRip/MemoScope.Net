using System.Collections.Generic;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.Segments
{
    public class SegmentInformation(ClrSegment segment)
    {
        private readonly ClrSegment segment = segment;

        [AddressColumn()]
        public ulong Start => segment.Start;
        [IntColumn()]
        public ulong Length => segment.Length;
        [BoolColumn()]
        public bool IsLarge => segment.IsLarge;
        [AddressColumn()]
        public ulong End => segment.End;
        [IntColumn(Title = "Gen 0 Length")]
        public ulong Gen0Length => segment.Gen0Length;
        [IntColumn(Title = "Gen 1 Length")]
        public ulong Gen1Length => segment.Gen1Length;
        [IntColumn(Title = "Gen 2 Length")]
        public ulong Gen2Length => segment.Gen2Length;
        [BoolColumn()]
        public bool IsEphemeral => segment.IsEphemeral;

        public IEnumerable<ulong> Instances => segment.EnumerateObjectAddresses();
    }
}