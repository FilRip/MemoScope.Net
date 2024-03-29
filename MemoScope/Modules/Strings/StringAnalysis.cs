﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;

using MemoScope.Core;
using MemoScope.Core.Data;

using WinFwk.UIMessages;
using WinFwk.UIModules;

namespace MemoScope.Modules.Strings
{
    public static class StringAnalysis
    {
        internal static List<StringInformation> Analyse(ClrDump clrDump, MessageBus msgBus)
        {
            var stringType = clrDump.GetClrType(typeof(string).FullName);
            var stringInstances = clrDump.EnumerateInstances(stringType);
            int nbStrings = clrDump.CountInstances(stringType);
            Dictionary<string, List<ulong>> result = [];
            CancellationTokenSource token = new();
            msgBus.BeginTask("Analyzing strings...", token);
            int n = 0;
            clrDump.Run(() =>
            {
                foreach (var address in stringInstances)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    n++;
                    if (SimpleValueHelper.GetSimpleValue(address, stringType, false) is not string value)
                    {
                        continue;
                    }
                    if (!result.TryGetValue(value, out List<ulong> addresses))
                    {
                        addresses = [];
                        result[value] = addresses;
                    }
                    addresses.Add(address);
                    if (n % 1024 == 0)
                    {
                        float pct = (float)n / nbStrings;
                        msgBus.Status($"Analyzing strings: {pct:p2}, n= {n:###,###,###,##0} / {nbStrings:###,###,###,##0}");
                    }
                }
            });
            msgBus.EndTask($"Strings analyzed. Instances: {n:###,###,###,##0}, unique values: {result.Count:###,###,###,##0}");

            var strings = result.Select(kvp => new StringInformation(kvp.Key, kvp.Value)).ToList();
            return strings;
        }
    }
}
