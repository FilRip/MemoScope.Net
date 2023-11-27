using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using ClrObject = MemoScope.Core.Data.ClrObject;

namespace MemoScope.Modules.Delegates
{
    public class LoneTargetInformation(ClrObject target, ClrMethod methInfo, ClrObject owner) : IAddressData, ITypeNameData
    {
        private readonly ClrMethod methInfo = methInfo;
        private readonly ClrObject target = target;
        readonly ClrObject owner = owner;

        [OLVColumn()]
        public ulong Address => target.Address;

        [OLVColumn()]
        public string TypeName => target.Type.Name;

        [OLVColumn()]
        public string Method => methInfo?.GetFullSignature();

        [OLVColumn()]
        public ulong OwnerAddress => owner.Address;

        [OLVColumn()]
        public string OwnerType => owner.Type?.Name;
    }
}