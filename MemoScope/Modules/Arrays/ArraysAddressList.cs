using MemoScope.Core.Data;

namespace MemoScope.Modules.Arrays
{
#pragma warning disable S2094 // Classes should not be empty
    public class ArraysAddressList(ClrDumpType clrDumpType) : AddressList(clrDumpType.ClrDump, clrDumpType.ClrType, clrDumpType.Instances)
    {
    }
#pragma warning restore S2094 // Classes should not be empty
}