﻿using System.Collections.Generic;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core.Data
{
    public class ClrDumpType(ClrDump clrDump, ClrType clrType)
    {
        public ClrDump ClrDump { get; } = clrDump;
        public ClrType ClrType { get; } = clrType;
        public List<ulong> Instances => ClrDump.GetInstances(ClrType);

        public bool IsAbstract => ClrDump.Eval(() => ClrType.IsAbstract);
        public bool IsPrimitive => ClrDump.Eval(() => ClrType.IsPrimitive);
        public bool IsString => ClrDump.Eval(() => ClrType.IsString);
        public bool IsPrimitiveOrString => ClrDump.Eval(() => ClrType.IsPrimitive || ClrType.IsString);
        public bool IsArray => ClrDump.Eval(() => ClrType.IsArray);

        public bool IsFinalizable => ClrDump.Eval(() => ClrType.IsFinalizable);
        public string BaseTypeName => ClrDump.Eval(() => ClrType.BaseType?.Name);

        public IList<ClrInstanceField> Fields => ClrDump.Eval(() => ClrType.Fields);
        public IList<ClrMethod> Methods => ClrDump.Eval(() => ClrType.Methods);

        public ClrElementType ElementType => ClrDump.Eval(() => ClrType.ElementType);

        public ClrDumpType BaseType => ClrDump.Eval(() => ClrType.BaseType == null ? null : new ClrDumpType(ClrDump, ClrType.BaseType));

        public string TypeName => ClrType == null ? null : ClrDump.Eval(() => ClrType.Name);

        public IList<ClrInterface> Interfaces => ClrDump.Eval(() => ClrType.Interfaces);

        public ClrDumpType(ClrDump clrDump, string typeName) : this(clrDump, clrDump.GetClrType(typeName))
        {
        }

        internal IEnumerable<ulong> EnumerateInstances()
        {
            return ClrDump.EnumerateInstances(ClrType);
        }
    }
}