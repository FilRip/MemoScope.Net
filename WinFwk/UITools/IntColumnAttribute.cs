using System.Windows.Forms;

using BrightIdeasSoftware;

namespace WinFwk.UITools
{
    public class IntColumnAttribute : OLVColumnAttribute
    {
        public IntColumnAttribute()
        {
            AspectToStringFormat = "{0:###,###,###,##0}";
            TextAlign = HorizontalAlignment.Right;
            Width = 150;
        }
    }
}
