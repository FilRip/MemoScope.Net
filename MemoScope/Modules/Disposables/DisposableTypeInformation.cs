using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UITools;

namespace MemoScope.Modules.Disposables
{
    public class DisposableTypeInformation : ITypeNameData
    {
        public DisposableTypeInformation(ClrType type, long nbInstances)
        {
            ClrType = type;
            TypeName = ClrType.Name;
            Count = nbInstances;
        }

        public ClrType ClrType { get; private set; }

        [OLVColumn()]
        public string TypeName { get; }
        [IntColumn()]
        public long Count { get; }
    }
}