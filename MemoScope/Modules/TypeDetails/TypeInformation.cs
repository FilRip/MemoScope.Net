﻿using System.Collections.Generic;
using System.Linq;

using MemoScope.Core.Data;
using MemoScope.Core.Helpers;

namespace MemoScope.Modules.TypeDetails
{
    public class TypeInformation : AbstractTypeInformation
    {
        private ClrDumpType ClrDumpType { get; }
        public TypeInformation(ClrDumpType clrDumpType)
        {
            ClrDumpType = clrDumpType;
            TypeName = TypeHelpers.ManageAlias(ClrDumpType.TypeName);
        }
        public override List<AbstractTypeInformation> Children
        {
            get
            {
                var baseType = new TypeInformation(ClrDumpType.BaseType);
                var interfs = ClrDumpType.Interfaces.Select(interf => new DummyTypeInformation(interf));
                var l = new List<AbstractTypeInformation>
                {
                    baseType
                };
                l.AddRange(interfs);
                return l;
            }
        }

        public override bool CanExpand
        {
            get
            {
                bool baseTypeExists = ClrDumpType.BaseType != null;
                bool hasInterfaces = ClrDumpType.Interfaces.Count != 0;
                return baseTypeExists || hasInterfaces;
            }
        }
    }
}
