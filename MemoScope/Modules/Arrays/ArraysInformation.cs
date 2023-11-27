using BrightIdeasSoftware;

using MemoScope.Core.Data;

using WinFwk.UITools;

namespace MemoScope.Modules.Arrays
{
    public class ArraysInformation(ClrDumpType arrayType) : ITypeNameData
    {
        public ClrDumpType ClrDumpType { get; } = arrayType;

        public ArraysInformation(ClrDumpType arrayType, ulong nbInstances, ulong totalLength, ulong maxLength, ulong totalSize) : this(arrayType)
        {
            NbInstances = nbInstances;
            TotalLength = totalLength;
            MaxLength = maxLength;
            TotalSize = totalSize;
        }

        [OLVColumn()]
        public string TypeName { get; } = arrayType.ClrType.Name;
        [IntColumn()]
        public ulong NbInstances { get; }
        [IntColumn()]
        public ulong TotalLength { get; }
        [IntColumn()]
        public ulong MaxLength { get; }
        [IntColumn()]
        public ulong TotalSize { get; }
    }
}