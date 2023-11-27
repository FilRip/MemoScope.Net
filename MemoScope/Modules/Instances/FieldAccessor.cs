using System;
using System.Collections.Generic;
using System.Linq;

using MemoScope.Core;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.Instances
{
    public class FieldAccessor(ClrDump clrDump, ClrType clrType)
    {
        public ClrDump ClrDump { get; } = clrDump;
        public ClrType ClrType { get; } = clrType;
        public ulong Address { get; set; }

        string lastArg = null;
        List<string> lastFields;

        private readonly Dictionary<string, List<string>> cacheField = [];

        private object Eval(string arg)
        {
            List<string> fields;
            if (ReferenceEquals(arg, lastArg))
            {
                fields = lastFields;
            }
            else
            {
                if (!cacheField.TryGetValue(arg, out fields))
                {
                    fields = arg.Split('.').ToList();
                    cacheField[arg] = fields;
                }
                lastFields = fields;
                lastArg = arg;
            }
            var value = ClrDump.GetFieldValueImpl(Address, ClrType, fields);
            return value;
        }

        internal static string GetFuncName(ClrElementType elementType)
        {
            return elementType switch
            {
                ClrElementType.Boolean => nameof(Bool),
                ClrElementType.Double => nameof(Double),
                ClrElementType.Float => nameof(Float),
                ClrElementType.Char => nameof(Char),
                ClrElementType.Int16 => nameof(Short),
                ClrElementType.Int32 => nameof(Int),
                ClrElementType.Int64 => nameof(Long),
                ClrElementType.Int8 => nameof(Byte),
                ClrElementType.String => nameof(String),
                ClrElementType.UInt16 => nameof(Ushort),
                ClrElementType.UInt32 => nameof(Uint),
                ClrElementType.UInt64 => nameof(Ulong),
                ClrElementType.Pointer or ClrElementType.NativeInt or ClrElementType.NativeUInt => nameof(Ptr),
                _ => nameof(Obj),
            };
        }

        public char Char(string arg)
        {
            return (char)Eval(arg);
        }

        public short Short(string arg)
        {
            return (short)Eval(arg);
        }

        public ushort Ushort(string arg)
        {
            return (ushort)Eval(arg);
        }
        public double Double(string arg)
        {
            var val = Eval(arg);
            if (val != null)
            {
                return (double)val;
            }
            return double.NaN;
        }
        public bool Bool(string arg)
        {
            return (bool)Eval(arg);
        }
        public int Int(string arg)
        {
            return (int)Eval(arg);
        }
        public long Long(string arg)
        {
            return (long)Eval(arg);
        }
        public string String(string arg)
        {
            return (string)Eval(arg);
        }
        public float Float(string arg)
        {
            return (float)Eval(arg);
        }
        public byte Byte(string arg)
        {
            return (byte)Eval(arg);
        }
        public uint Uint(string arg)
        {
            return (uint)Eval(arg);
        }
        public ulong Ulong(string arg)
        {
            return (ulong)Eval(arg);
        }
        public DateTime Datetime(string arg)
        {
            return (DateTime)Eval(arg);
        }
        public decimal Decimal(string arg)
        {
            return (decimal)Eval(arg);
        }
        public object Obj(string arg)
        {
            return Eval(arg);
        }
        public long Ptr(string arg)
        {
            var ptr = Eval(arg);
            var val = (long)ptr;
            return val;
        }
        public override string ToString()
        {
            var obj = ClrDump.GetSimpleValue(Address, ClrType);
            return obj != null ? obj.ToString() : string.Empty;
        }
    }
}