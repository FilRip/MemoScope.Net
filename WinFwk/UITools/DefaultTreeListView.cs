using System;
using System.Drawing;

using BrightIdeasSoftware;

namespace WinFwk.UITools
{
    public class DefaultTreeListView : DataTreeListView
    {
        public DefaultTreeListView()
        {
            FullRowSelect = true;
            HideSelection = false;
            ShowImagesOnSubItems = true;
        }

        public void InitData<T>() where T : ITreeNodeInformation<T>
        {
            Generator.GenerateColumns(this, typeof(T), false);
            CanExpandGetter = o =>
            {
                T item = (T)o;
                try
                {
                    return item.CanExpand;
                }
                catch (Exception e)
                {
                    item.BackColor = Color.Red;
                    item.ForeColor = Color.Black;
                    item.Tooltip = $"Error: {e.Message}";
                    return false;
                }
            };

            ChildrenGetter = o =>
            {
                T item = (T)o;
                try
                {
                    return item.Children;
                }
                catch (Exception e)
                {
                    item.BackColor = Color.Red;
                    item.ForeColor = Color.Black;
                    item.Tooltip = $"Error: {e.Message}";
                    return null;
                }
            };

            CellToolTipGetter = (col, o) => ((T)o).Tooltip;


            FormatRow += (sender, arg) =>
            {
                ITreeNodeInformation<T> treeNode = (arg.Model) as ITreeNodeInformation<T>;
                if (treeNode.BackColor != Color.Transparent)
                {
                    arg.Item.BackColor = treeNode.BackColor;
                }
                if (treeNode.ForeColor != Color.Transparent)
                {
                    arg.Item.ForeColor = treeNode.ForeColor;
                }
            };
            UseCellFormatEvents = true;

            this.InitColumnTooltips();
        }


        public OLVColumn this[string columnName] => this.AllColumns.Find(col => col.Name == columnName);
    }
}