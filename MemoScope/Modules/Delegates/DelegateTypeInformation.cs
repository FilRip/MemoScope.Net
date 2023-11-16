using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.Delegates
{
    public class DelegateTypeInformation : ITypeNameData
    {
        public ClrType ClrType { get; }

        public DelegateTypeInformation(ClrType clrType, int count, long targetCount)
        {
            ClrType = clrType;
            Count = count;
            Targets = targetCount;
        }

        [OLVColumn(Title = "Type")]
        public string TypeName => ClrType.Name;

        [IntColumn(Title = "Count")]
        public int Count { get; }

        [IntColumn(Title = "Total Targets")]
        public long Targets { get; }
    }
}