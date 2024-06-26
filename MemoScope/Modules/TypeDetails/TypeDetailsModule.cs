﻿using System.Collections.Generic;
using System.Linq;

using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Core.Data;
using MemoScope.Core.Helpers;

using Microsoft.Diagnostics.Runtime;

namespace MemoScope.Modules.TypeDetails
{
    public partial class TypeDetailsModule : UIClrDumpModule
    {
        private ClrType type;
        public TypeDetailsModule()
        {
            InitializeComponent();
            Icon = Properties.Resources.blueprint;
        }

        public void Setup(ClrDumpType dumpType)
        {
            type = dumpType.ClrType;
            ClrDump = dumpType.ClrDump;
            pgTypeInfo.SelectedObject = new TypeInformations(dumpType);

            dlvFields.InitColumns<FieldInformation>();
            dlvFields.SetUpTypeColumn(this);
            dlvFields.SetObjects(dumpType.Fields.Select(clrField => new FieldInformation(clrField)));

            dlvMethods.InitColumns<MethodInformation>();
            dlvMethods.SetUpTypeColumn(this);
            dlvMethods.SetObjects(dumpType.Methods.Select(clrMethod => new MethodInformation(clrMethod)));

            dtlvParentClasses.InitData<AbstractTypeInformation>();
            dtlvParentClasses.SetUpTypeColumn(this);

            var l = new List<object>();
            var typeInformation = new TypeInformation(dumpType.BaseType);
            var interfaceInformations = InterfaceInformation.GetInterfaces(dumpType);
            l.Add(typeInformation);
            l.AddRange(interfaceInformations);
            dtlvParentClasses.Roots = l;
        }

        public override void PostInit()
        {
            Name = $"#{ClrDump.Id} - {type.Name}";
            Summary = type.Name;
        }

#pragma warning disable IDE0300 // Simplifier l'initialisation des collections
        public override IEnumerable<ObjectListView> ListViews => new ObjectListView[] { dlvFields, dlvMethods, dtlvParentClasses, };
#pragma warning restore IDE0300 // Simplifier l'initialisation des collections
    }
}
