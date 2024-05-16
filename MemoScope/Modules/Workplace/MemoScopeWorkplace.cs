using System.Collections.Generic;
using System.Linq;

using MemoScope.Core;

using WinFwk.UICommands;
using WinFwk.UITools.Workplace;

namespace MemoScope.Modules.Workplace
{
    public class MemoScopeWorkplace : WorkplaceModule, IUIDataProvider<List<ClrDump>>
    {
        public List<ClrDump> Data
        {
            get
            {
                var modules = SelectedModules;
#pragma warning disable S2365 // Properties should not make collection or array copies
                return modules.OfType<UIClrDumpModule>().Select(mod => mod.ClrDump).ToList();
#pragma warning restore S2365 // Properties should not make collection or array copies
            }
        }
    }
}
