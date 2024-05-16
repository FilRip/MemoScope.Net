using System.Windows.Forms;

using BrightIdeasSoftware;

namespace WinFwk.UITools
{
    public class PercentColumnAttribute : OLVColumnAttribute
    {
        public PercentColumnAttribute(string format = "{0:p2}")
        {
            AspectToStringFormat = format;
            TextAlign = HorizontalAlignment.Right;
            Width = 150;
        }
    }
}
