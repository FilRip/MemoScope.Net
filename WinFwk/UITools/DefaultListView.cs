using System;
using System.Linq;
using System.Windows.Forms;

using BrightIdeasSoftware;

namespace WinFwk.UITools
{
    public class DefaultListView : FastObjectListView
    {
        public DefaultListView()
        {
            this.Init();

            this.ContextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem menuItem = new("Copy Selected rows");
            ContextMenuStrip.Items.Add(menuItem);
            menuItem.Click += (o, e) =>
            {
                string text = string.Join(Environment.NewLine, this.SelectedToTsv());
                Clipboard.SetText(text);
            };
        }

        public void BuildGroups(string colName, SortOrder order, bool colIsVisible = false)
        {
            OLVColumn col = this[colName];
            col.IsVisible = colIsVisible;
            BuildGroups(col, order);
        }

        public void InitColumns<T>()
        {
            Generator.GenerateColumns(this, typeof(T), false);
            this.InitColumnTooltips();
        }

#pragma warning disable IDE0060 // Supprimer le paramètre inutilisé
        public void Sort(string colName, SortOrder sortOrder = SortOrder.Ascending)
#pragma warning restore IDE0060 // Supprimer le paramètre inutilisé
        {
            OLVColumn col = this[colName];
            Sort(col, SortOrder.Descending);
        }

        public OLVColumn this[string columnName] => this.AllColumns.Find(col => col.Name == columnName);
    }
}