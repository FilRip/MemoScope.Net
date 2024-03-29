﻿using System.Collections.Generic;

using BrightIdeasSoftware;

using MemoScope.Core;

namespace MemoScope.Modules.DumpDiff
{
    public class DiffColumn : OLVColumn
    {
        public ClrDump ClrDump { get; }

        private readonly Dictionary<string, ClrTypeStats> dicoStats;
        private readonly Dictionary<string, ClrTypeStats> dicoPrevStats;

        public DiffColumn(ClrDump clrDump, List<ClrTypeStats> stats, List<ClrTypeStats> prevStats)
        {
            ClrDump = clrDump;
            Text = "#" + clrDump.Id;
            TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            Width = 150;
            ToolTipText = Text + " / " + clrDump.DumpPath;

            dicoStats = BuildDicoStat(stats);
            if (prevStats != null)
            {
                AspectToStringFormat = "{0:+###,###,###,##0;-###,###,###,##0;0}";
                dicoPrevStats = BuildDicoStat(prevStats);
            }
            else
            {
                AspectToStringFormat = "{0:###,###,###,##0}";
            }
            AspectGetter = GetCountForType;
        }

        private static Dictionary<string, ClrTypeStats> BuildDicoStat(List<ClrTypeStats> stats)
        {
            var dico = new Dictionary<string, ClrTypeStats>();
            foreach (var stat in stats)
            {
                int n = 0;
                string name = stat.Type.Name;
                while (dico.ContainsKey(name))
                {
                    n++;
                    name = stat.Type.Name + " #" + n;
                }
                dico.Add(name, stat);
            }
            return dico;
        }

        private object GetCountForType(object rowObject)
        {
            if (rowObject is not string typeName || !dicoStats.TryGetValue(typeName, out ClrTypeStats stat))
            {
                return null;
            }
            long n = stat.NbInstances;

            if (dicoPrevStats != null &&
                dicoPrevStats.TryGetValue(typeName, out ClrTypeStats prevStat))
            {
                n -= prevStat.NbInstances;
            }
            return n;
        }
    }
}
