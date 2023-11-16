using BrightIdeasSoftware;

using MemoScope.Core.Data;

using Microsoft.Diagnostics.Runtime;

using ClrObject = MemoScope.Core.Data.ClrObject;

namespace MemoScope.Modules.Delegates
{
    public class LoneTargetInformation : IAddressData, ITypeNameData
    {
        private readonly ClrMethod methInfo;
        private readonly ClrObject target;
        readonly ClrObject owner;
        public LoneTargetInformation(ClrObject target, ClrMethod methInfo, ClrObject owner)
        {
            this.target = target;
            this.methInfo = methInfo;
            this.owner = owner;
        }

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