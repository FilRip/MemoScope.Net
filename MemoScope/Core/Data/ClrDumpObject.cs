using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core.Data
{
    public class ClrDumpObject(ClrDump dump, ClrType type, ulong address, bool isInterior = false) : ClrDumpType(dump, type)
    {
        public ulong Address { get; } = address;
        public object Value => ClrDump.Eval(GetValue);
        public bool IsInterior { get; private set; } = isInterior;
        public int ArrayLength => ClrDump.Eval(() => ClrType.GetArrayLength(Address));

        public ClrObject ClrObject => new(Address, ClrType, IsInterior);

        private object GetValue()
        {
            var clrObject = new ClrObject(Address, ClrType, IsInterior);
            if (clrObject.HasSimpleValue && !clrObject.IsNull)
            {
                return clrObject.SimpleValue;
            }
            else
            {
                return null;
            }

        }

        public ClrDumpObject(ClrDumpType clrDumpType, ulong address) : this(clrDumpType.ClrDump, clrDumpType.ClrType, address)
        {
        }
    }
}
