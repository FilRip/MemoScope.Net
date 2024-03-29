﻿using System.Collections.Generic;
using System.Threading;

using MemoScope.Modules.Arrays;

using WinFwk.UIMessages;
using WinFwk.UIModules;

namespace MemoScope.Modules.ArrayInstances
{
    public static class ArrayInstanceAnalysis
    {
        public static List<ArrayInstanceInformation> Analyze(ArraysAddressList arrayAddressList, MessageBus msgBus)
        {
            var clrDump = arrayAddressList.ClrDump;
            var clrType = arrayAddressList.ClrType;
            var typeName = clrType.Name;
            CancellationTokenSource token = new();
            msgBus.BeginTask($"Analyzing arrays: {typeName}...", token);

            List<ArrayInstanceInformation> result = clrDump.Eval(() =>
            {
                var tmp = new List<ArrayInstanceInformation>();
                var count = arrayAddressList.Addresses.Count;
                HashSet<object> addresses = [];

                for (int i = 0; i < count; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    var address = arrayAddressList.Addresses[i];
                    int length = clrType.GetArrayLength(address);
                    if (i % 128 == 0)
                    {
                        msgBus.Status($"Analyzing instance: {i:###,###,###,##0} / {count:###,###,###,##0}");
                    }
                    float? nullRatio = null;
                    float? uniqueRatio = null;

                    if (clrType.ContainsPointers)
                    {
                        int nbNull = 0;
                        addresses.Clear();
                        for (int j = 0; j < length; j++)
                        {
                            object elemAddress = clrType.GetArrayElementValue(address, j);
                            if (elemAddress is ulong v && v == 0)
                            {
                                nbNull++;
                            }
                            else if (elemAddress == null)
                            {
                                nbNull++;
                            }
                            else
                            {
                                addresses.Add(elemAddress);
                            }
                            if (j % 1024 == 0 && j != 0)
                            {
                                msgBus.Status($"Analyzing instance: {i:###,###,###,##0} / {count:###,###,###,##0}, element {j:###,###,###,##0} / {length:###,###,###,##0}");
                            }
                        }
                        nullRatio = ((float)nbNull) / length;
                        uniqueRatio = ((float)addresses.Count + nbNull) / length;
                    }

                    ArrayInstanceInformation info = new(address, length, nullRatio, uniqueRatio);
                    tmp.Add(info);
                }
                return tmp;
            });
            msgBus.EndTask("Arrays analyzed.");
            return result;
        }
    }
}
