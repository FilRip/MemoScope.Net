﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WinFwk.UITools.Settings.Skins
{
    public class SkinTypeEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            ListBox listBox = new()
            {
                SelectionMode = SelectionMode.One
            };
            listBox.SelectedValueChanged += OnListBoxSelectedValueChanged;
            listBox.DisplayMember = nameof(AbstractSkin.Name);
            listBox.BeginUpdate();
            foreach (Type skinType in WinFwkHelper.GetDerivedTypes(typeof(AbstractSkin)))
            {
                object skin = Activator.CreateInstance(skinType);
                listBox.Items.Add(skin);
            }
            listBox.EndUpdate();
            listBox.Sorted = true;
            editorService.DropDownControl(listBox);
            if (listBox.SelectedItem == null) // no selection, return the passed-in value as is
                return value;

            return listBox.SelectedItem;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            editorService.CloseDropDown();
        }
    }
}
