﻿using System.Collections.Generic;

using BrightIdeasSoftware;

using MemoScope.Core;
using MemoScope.Core.Data;
using MemoScope.Core.Helpers;

namespace MemoScope.Modules.InstanceDetails
{
    public partial class InstanceDetailsModule : UIClrDumpModule
    {
        private ClrDumpObject ClrDumpObject { get; set; }
        private object SimpleValue { get; set; }
        private List<FieldValueInformation> mainFieldValues;

        public InstanceDetailsModule()
        {
            InitializeComponent();
            Icon = Properties.Resources.elements_small;
        }

        internal void Setup(ClrDumpObject clrDumpObject)
        {
            Init(clrDumpObject);
            InitColumns();
        }

        public void InitColumns()
        {
            // Fields
            dtlvFieldsValues.InitData<FieldValueInformation>();
            dtlvFieldsValues.SetUpTypeColumn(this);
            dtlvFieldsValues.SetUpAddressColumn(this);

            // References
            dtlvReferences.InitData<ReferenceInformation>();
            dtlvReferences.SetUpTypeColumn(this);
            dtlvReferences.SetUpAddressColumn(this);
        }

        public void Init(ClrDumpObject clrDumpObject)
        {
            ClrDumpObject = clrDumpObject;
            ClrDump = clrDumpObject.ClrDump;
            tbAddress.Text = clrDumpObject.Address.ToString("X");
            tbType.Text = clrDumpObject.ClrType?.Name;
        }

        public override void Init()
        {
            if (ClrDumpObject == null)
            {
                return;
            }

            mainFieldValues = FieldValueInformation.GetChildren(ClrDumpObject);
            SimpleValue = ClrDump.Eval(() =>
            {
                if (SimpleValueHelper.IsSimpleValue(ClrDumpObject.ClrType))
                {
                    return SimpleValueHelper.GetSimpleValue(ClrDumpObject.Address, ClrDumpObject.ClrType);
                }
                return null;
            });
        }

        public void Clear()
        {
            dtlvFieldsValues.ClearObjects();
            dtlvReferences.ClearObjects();
        }

        public override void PostInit()
        {
            dtlvFieldsValues.ClearObjects();
            dtlvReferences.ClearObjects();

            dtlvFieldsValues.Roots = mainFieldValues;
            tbSimpleValue.Text = SimpleValue?.ToString();
            if (ClrDumpObject != null)
            {
                dtlvReferences.Roots = new[] { new ReferenceInformation(ClrDumpObject.ClrDump, ClrDumpObject.Address) };
                Name = "#" + ClrDumpObject.ClrDump.Id + " - " + ClrDumpObject.Address.ToString("X");
                Summary = ClrDumpObject.ClrType == null ? "Unkown" : ClrDumpObject.ClrType.Name;
            }
        }
#pragma warning disable IDE0300 // Simplifier l'initialisation des collections
        public override IEnumerable<ObjectListView> ListViews => new ObjectListView[] { dtlvFieldsValues, dtlvReferences };
#pragma warning restore IDE0300 // Simplifier l'initialisation des collections
    }
}
