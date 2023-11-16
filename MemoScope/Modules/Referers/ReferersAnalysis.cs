using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using MemoScope.Core;
using MemoScope.Core.Helpers;

using Microsoft.Diagnostics.Runtime;

using WinFwk.UIMessages;
using WinFwk.UIModules;

namespace MemoScope.Modules.Referers
{
    public static class ReferersAnalysis
    {
        public static bool HasReferers(ClrDump clrDump, IEnumerable<ulong> addresses)
        {
            return addresses.Any(adr => clrDump.HasReferers(adr));
        }

        public static List<ReferersInformation> AnalyzeReferers(MessageBus msgBus, ClrDump clrDump, HashSet<ulong> addresses)
        {
            var referers = new List<ReferersInformation>();
            var dicoByRefererType = new Dictionary<ClrType, Dictionary<string, ReferersInformation>>();
            CancellationTokenSource token = new();
            msgBus.BeginTask("Analyzing referers...", token);
            Application.DoEvents(); // todo: avoid this call to Application.DoEvents()
            int count = addresses.Count;
            int i = 0;
            foreach (var address in addresses)
            {
                i++;
                if (token.IsCancellationRequested)
                {
                    msgBus.EndTask("Referers analyze: cancelled.");
                    return referers;
                }
                if (i % 1024 == 0)
                {
                    msgBus.Status($"Analyzing referers: {(double)i / count:p2}, {i:###,###,###,##0} / {count:###,###,###,##0}...");
                    Application.DoEvents();// todo: avoid this call to Application.DoEvents()
                }
                foreach (var refererAddress in clrDump.EnumerateReferers(address))
                {
                    var type = clrDump.GetObjectType(refererAddress);
                    string field;
                    if (type.IsArray)
                    {
                        field = "[ x ]";
                    }
                    else
                    {
                        field = clrDump.GetFieldNameReference(address, refererAddress);
                        field = TypeHelpers.RealName(field);
                    }
                    if (!dicoByRefererType.TryGetValue(type, out Dictionary<string, ReferersInformation> dicoRefInfoByFieldName))
                    {
                        dicoRefInfoByFieldName = new Dictionary<string, ReferersInformation>();
                        dicoByRefererType[type] = dicoRefInfoByFieldName;
                    }

                    if (!dicoRefInfoByFieldName.TryGetValue(field, out ReferersInformation referersInformation))
                    {
                        referersInformation = new ReferersInformation(clrDump, type, field, msgBus, count);
                        dicoRefInfoByFieldName[field] = referersInformation;
                    }

                    referersInformation.References.Add(address);
                    referersInformation.Instances.Add(refererAddress);
                }
            }

            foreach (var kvpType in dicoByRefererType)
            {
                foreach (ReferersInformation kvpField in kvpType.Value.Select(v => v.Value))
                {
                    var refInfo = kvpField;
                    referers.Add(refInfo);
                    refInfo.Init();
                }
            }
            msgBus.EndTask("Referers analyzed.");
            Application.DoEvents();// todo: avoid this call to Application.DoEvents()
            return referers;
        }
    }
}
