using System;
using System.Net;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Core.Data
{
    public static class SimpleValueHelper
    {
        private const string GuidTypeName = "System.Guid";
        private const string TimeSpanTypeName = "System.TimeSpan";
        private const string DateTimeTypeName = "System.DateTime";
        private const string IPAddressTypeName = "System.Net.IPAddress";

        public static bool IsSimpleValue(ClrType type)
        {
            if (type == null)
            {
                return false;
            }
            if (type.IsPrimitive || type.IsString)
                return true;

            return type.Name switch
            {
                GuidTypeName or TimeSpanTypeName or DateTimeTypeName or IPAddressTypeName => true,
                _ => false,
            };
        }
        public static object GetSimpleValue(ClrObject obj)
        {
            return GetSimpleValue(obj.Address, obj.Type, obj.IsInterior);
        }

        public static object GetSimpleValue(ulong objAddress, ClrType clrType, bool isInterior = false)
        {
            if (objAddress == 0)
                throw new ArgumentNullException(nameof(objAddress));

            ClrHeap heap = clrType.Heap;
            if (clrType.IsEnum)
            {
                var val = clrType.GetValue(objAddress);
                return clrType.GetEnumName(val);
            }

            if (clrType.IsPrimitive || clrType.IsString)
                return clrType.GetValue(objAddress);

            ulong address = isInterior ? objAddress : objAddress + (ulong)heap.PointerSize;

            switch (clrType.Name)
            {
                case GuidTypeName:
                    {
                        byte[] buffer = ReadBuffer(heap, address, 16);
                        return new Guid(buffer);
                    }

                case TimeSpanTypeName:
                    {
                        byte[] buffer = ReadBuffer(heap, address, 8);
                        long ticks = BitConverter.ToInt64(buffer, 0);
                        return new TimeSpan(ticks);
                    }

                case DateTimeTypeName:
                    {
                        byte[] buffer = ReadBuffer(heap, address, 8);
                        ulong dateData = BitConverter.ToUInt64(buffer, 0);
                        return GetDateTime(dateData);
                    }

                case IPAddressTypeName:
                    {
                        return GetIPAddress(new ClrObject(objAddress, clrType, isInterior));
                    }
            }

            throw new InvalidOperationException(string.Format("SimpleValue not available for type '{0}'", clrType.Name));
        }

        public static string GetSimpleValueString(ClrObject obj)
        {
            object value = obj.SimpleValue;

            if (value == null)
                return "null";

            ClrType type = obj.Type;
            if (type != null && type.IsEnum)
                return type.GetEnumName(value) ?? value.ToString();

            DateTime? dateTime = value as DateTime?;
            if (dateTime != null)
                return GetDateTimeString(dateTime.Value);

            return value.ToString();
        }

        private static byte[] ReadBuffer(ClrHeap heap, ulong address, int length)
        {
            byte[] buffer = new byte[length];
            int byteRead = heap.ReadMemory(address, buffer, 0, buffer.Length);

            if (byteRead != length)
                throw new InvalidOperationException(string.Format("Expected to read {0} bytes and actually read {1}", length, byteRead));

            return buffer;
        }

        private static DateTime GetDateTime(ulong dateData)
        {
            const ulong DateTimeTicksMask = 0x3FFFFFFFFFFFFFFF;
            const ulong DateTimeKindMask = 0xC000000000000000;
            const ulong KindUnspecified = 0x0000000000000000;
            const ulong KindUtc = 0x4000000000000000;

            long ticks = (long)(dateData & DateTimeTicksMask);
            ulong internalKind = dateData & DateTimeKindMask;

            return internalKind switch
            {
                KindUnspecified => new DateTime(ticks, DateTimeKind.Unspecified),
                KindUtc => new DateTime(ticks, DateTimeKind.Utc),
                _ => new DateTime(ticks, DateTimeKind.Local),
            };
        }

        private static IPAddress GetIPAddress(ClrObject ipAddress)
        {
            const int AddressFamilyInterNetworkV6 = 23;
            const int IPv4AddressBytes = 4;
            const int IPv6AddressBytes = 16;
            const int NumberOfLabels = IPv6AddressBytes / 2;

            byte[] bytes;
            int family = (int)ipAddress["m_Family"].SimpleValue;

            if (family == AddressFamilyInterNetworkV6)
            {
                bytes = new byte[IPv6AddressBytes];
                int j = 0;

                var numbers = ipAddress["m_Numbers"];

                for (int i = 0; i < NumberOfLabels; i++)
                {
                    ushort number = (ushort)numbers[i].SimpleValue;
                    bytes[j++] = (byte)((number >> 8) & 0xFF);
                    bytes[j++] = (byte)(number & 0xFF);
                }
            }
            else
            {
                long address = (long)ipAddress["m_Address"].SimpleValue;
                bytes = new byte[IPv4AddressBytes];
                bytes[0] = (byte)(address);
                bytes[1] = (byte)(address >> 8);
                bytes[2] = (byte)(address >> 16);
                bytes[3] = (byte)(address >> 24);
            }

            return new IPAddress(bytes);
        }

        private static string GetDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString(@"yyyy-MM-dd HH\:mm\:ss.FFFFFFF") + GetDateTimeKindString(dateTime.Kind);
        }

        private static string GetDateTimeKindString(DateTimeKind kind)
        {
            return kind switch
            {
                DateTimeKind.Unspecified => " (Unspecified)",
                DateTimeKind.Utc => " (Utc)",
                _ => " (Local)",
            };
        }
    }
}