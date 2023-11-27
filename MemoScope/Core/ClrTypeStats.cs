using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Core
{
    public class ClrTypeStats(int id, ClrType type) : ITypeNameData
    {
        public int Id { get; } = id;
        public ClrType Type { get; } = type;
        public ulong MethodTable { get; } = type.MethodTable;

        [OLVColumn(Title = "Type Name", Width = 500)]
        public string TypeName { get; }

        [IntColumn(Title = "Nb")]
        public long NbInstances { get; private set; }

        [IntColumn(Title = "Total Size")]
        public ulong TotalSize { get; private set; }

        public ClrTypeStats(int id, ClrType type, long nbInstances, ulong totalSize) : this(id, type)
        {
            NbInstances = nbInstances;
            TotalSize = totalSize;
            TypeName = Type.Name;
        }

        public void Inc(ulong size)
        {
            TotalSize += size;
            NbInstances++;
        }
    }

}