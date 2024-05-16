using System.Windows.Forms;

using BrightIdeasSoftware;

namespace WinFwk.UITools
{
    public class AddressColumnAttribute : OLVColumnAttribute
    {
        public AddressColumnAttribute()
        {
            AspectToStringFormat = "{0:X}";
            TextAlign = HorizontalAlignment.Right;
            Width = 150;
        }
    }
}
