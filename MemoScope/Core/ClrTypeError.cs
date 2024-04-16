using System;
using System.Collections.Generic;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core
{
    public class ClrTypeError(string typeName) : ClrType
    {
        protected override GCDesc GCDesc { get; }
        public override ulong MethodTable => 0;
        public override uint MetadataToken => 0;
        public override string Name { get; } = $"Error({typeName})";
        public override ClrHeap Heap => null;
#pragma warning disable IDE0028 // Simplifier l'initialisation des collections
        public override IList<ClrInterface> Interfaces => new List<ClrInterface>();
#pragma warning restore IDE0028 // Simplifier l'initialisation des collections
        public override bool IsFinalizable => false;
        public override bool IsPublic => false;
        public override bool IsPrivate => false;
        public override bool IsInternal => false;
        public override bool IsProtected => false;
        public override bool IsAbstract => false;
        public override bool IsSealed => false;
        public override bool IsInterface => false;
        public override ClrType BaseType => null;
        public override int ElementSize => 0;
        public override int BaseSize => 0;
#pragma warning disable IDE0300 // Simplifier l'initialisation des collections
        public override IEnumerable<ulong> EnumerateMethodTables() => new ulong[0];
#pragma warning restore IDE0300 // Simplifier l'initialisation des collections
#pragma warning disable IDE0028 // Simplifier l'initialisation des collections
        public override IList<ClrInstanceField> Fields => new List<ClrInstanceField>();
#pragma warning restore IDE0028 // Simplifier l'initialisation des collections

        public override void EnumerateRefsOfObject(ulong objRef, Action<ulong, int> action)
        {
        }

        public override void EnumerateRefsOfObjectCarefully(ulong objRef, Action<ulong, int> action)
        {
        }

        public override ulong GetArrayElementAddress(ulong objRef, int index) => 0;
        public override object GetArrayElementValue(ulong objRef, int index) => null;
        public override int GetArrayLength(ulong objRef) => 0;
        public override ClrInstanceField GetFieldByName(string name) => null;

        public override bool GetFieldForOffset(int fieldOffset, bool inner, out ClrInstanceField childField, out int childFieldOffset)
        {
            childField = null;
            childFieldOffset = 0;
            return false;
        }

        public override ulong GetSize(ulong objRef) => 0;
        public override ClrStaticField GetStaticFieldByName(string name) => null;
    }
}
