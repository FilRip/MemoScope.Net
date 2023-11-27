using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.Delegates
{
    public class DelegateTypeInformation(ClrType clrType, int count, long targetCount) : ITypeNameData
    {
        public ClrType ClrType { get; } = clrType;

        [OLVColumn(Title = "Type")]
        public string TypeName => ClrType.Name;

        [IntColumn(Title = "Count")]
        public int Count { get; } = count;

        [IntColumn(Title = "Total Targets")]
        public long Targets { get; } = targetCount;
    }
}