using System.Collections.Generic;
using System.Linq;

using MemoScope.Core;
using MemoScope.Core.Helpers;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.Stack
{
    public partial class StackModule : UIClrDumpModule
    {
        public ClrThread Thread { get; private set; }
        public List<StackInstanceInformation> StackInstances { get; private set; }

        public StackModule()
        {
            InitializeComponent();
        }

        public void Setup(ClrDump clrDump, ClrThread thread, UIClrDumpModule parentModule = null)
        {
            ClrDump = clrDump;
            Thread = thread;
            Icon = Properties.Resources.formatting_dublicate_value_small;
            Name = $"#{clrDump.Id} - Stack - Id: {Thread?.ManagedThreadId}";

            dlvStack.InitColumns<StackInstanceInformation>();
            dlvStack.SetUpAddressColumn(parentModule ?? this);
            dlvStack.SetUpTypeColumn(parentModule ?? this);
            dlvStack.AddSimpleValueColumn(o => ((StackInstanceInformation)o).Address, ClrDump, o => ((StackInstanceInformation)o).Type);
            dlvStack.RebuildColumns();
            dlvStack.SetTypeNameFilter<StackInstanceInformation>(regexFilterControl);
        }

        public override void PostInit()
        {
            base.PostInit();
            Summary = $"{StackInstances.Count:###,###,###,##0} instances";
        }

        public override void Init()
        {
            if (Thread == null)
            {
                StackInstances = null;
            }
            else
            {
                StackInstances = ClrDump.Eval(() => Thread.EnumerateStackObjects().GroupBy(clrRoot => clrRoot.Object).Select(clrRootGroup => new StackInstanceInformation(clrRootGroup.First())).ToList());
            }
            dlvStack.Objects = StackInstances;
        }
    }
}
