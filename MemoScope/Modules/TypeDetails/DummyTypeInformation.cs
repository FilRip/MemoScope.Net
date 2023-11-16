using System.Collections.Generic;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.TypeDetails
{
    public class DummyTypeInformation : AbstractTypeInformation
    {
        public DummyTypeInformation(ClrInterface interf)
        {
            TypeName = interf.Name;
        }

        public override bool CanExpand => false;
        public override List<AbstractTypeInformation> Children => null;
    }
}