﻿using System;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core.Data
{
    // This code was extracted from https://github.com/JeffCyr/ClrMD.Extensions
    // Thanks a lot to Jeff Cyr !
    // TODO: remove this code when ClrMd merges 'clrobject' branches into master and releases a new version.
    public struct ClrObject(ulong address, ClrType type, bool isInterior = false)
    {
        public ulong Address { get; } = address;
        public ClrType Type { get; } = type;
        public bool IsInterior { get; } = isInterior;

        public ClrHeap Heap => Type.Heap;
        public bool IsNull => Address == 0;

        public ClrObject this[string fieldName]
        {
            get
            {
                ClrInstanceField field = GetField(fieldName) ?? throw new ArgumentException($"Field '{fieldName}' not found in Type '{Type.Name}'");
                return this[field];
            }
        }

        public ClrObject this[ClrInstanceField field] => GetInnerObject(field.GetAddress(Address, IsInterior), field.Type);
        public ClrObject this[int arrayIndex] => GetInnerObject(Type.GetArrayElementAddress(Address, arrayIndex), Type.ComponentType);
        public bool HasSimpleValue => SimpleValueHelper.IsSimpleValue(Type);
        public object SimpleValue => SimpleValueHelper.GetSimpleValue(this);

        public ClrInstanceField GetField(string fieldName)
        {
            ClrInstanceField field;
            string backingFieldName = GetAutomaticPropertyField(fieldName);

            field = Type.GetFieldByName(fieldName);

            field ??= Type.GetFieldByName(backingFieldName);

            return field;
        }

        public static string GetAutomaticPropertyField(string propertyName)
        {
            return "<" + propertyName + ">" + "k__BackingField";
        }

        public static ClrObject GetInnerObject(ulong pointer, ClrType type)
        {
            ulong fieldAddress;
            ClrType actualType = type;

            if (type.IsObjectReference)
            {
                type.Heap.ReadPointer(pointer, out fieldAddress);

                if (!type.IsSealed && fieldAddress != 0)
                    actualType = type.Heap.GetObjectType(fieldAddress);
            }
            else if (type.IsPrimitive)
            {
                // Unfortunately, ClrType.GetValue for primitives assumes that the value is boxed,
                // we decrement PointerSize because it will be added when calling ClrType.GetValue.
                // ClrMD should be updated in a future version to include ClrType.GetValue(int interior).
                fieldAddress = pointer - (ulong)type.Heap.PointerSize;
            }
            else if (type.IsValueClass)
            {
                fieldAddress = pointer;
            }
            else
            {
                throw new NotSupportedException(string.Format("Object type not supported '{0}'", type.Name));
            }

            return new ClrObject(fieldAddress, actualType, !type.IsObjectReference);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ClrObject @object)
            {
                return Address == @object.Address;
            }
            return false;
        }
    }
}