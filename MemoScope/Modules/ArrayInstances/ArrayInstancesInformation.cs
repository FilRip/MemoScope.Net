using BrightIdeasSoftware;

using MemoScope.Core.Data;

using WinFwk.UITools;

namespace MemoScope.Modules.ArrayInstances
{
    public class ArrayInstanceInformation(ulong address, int length, float? nullRatio, float? uniqueRatio) : IAddressData
    {
        [OLVColumn()]
        public ulong Address { get; } = address;
        [IntColumn()]
        public int Length { get; } = length;
        [PercentColumn]
        public float? NullRatio { get; } = nullRatio;
        [PercentColumn]
        public float? UniqueRatio { get; } = uniqueRatio;
    }
}