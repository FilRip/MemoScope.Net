using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Diagnostics.Runtime.Interop;

using WinFwk.UITools;

namespace MemoScope.Controls
{
    public class MyDefaultListView : DefaultListView
    {
        public Color GroupBackgroundColor { get; set; } = Color.Transparent;

        public Color GroupForegroundColor { get; set; } = Color.LightBlue;

        public Color GroupSeparatorColor { get; set; } = Color.DarkGreen;

        public MyDefaultListView() : base()
        {
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMhdr
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public int code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMCustomDraw
        {
            public NMhdr hdr;
            public CDDS dwDrawStage;
            public IntPtr hdc;
            public RECT rc;
            public IntPtr dwItemSpec;
            public uint uItemState;
            public IntPtr lItemlParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NmLVCustomDraw
        {
            public NMCustomDraw nmcd;
            public int clrText;
            public int clrTextBk;
            public int iSubItem;
            public int dwItemType;
            public int clrFace;
            public int iIconEffect;
            public int iIconPhase;
            public int iPartId;
            public int iStateId;
            public RECT rcText;
            public uint uAlign;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct LvGroup
        {
            public uint cbSize;
            public uint mask;
            // <MarshalAs(UnmanagedType.LPTStr)>
            // Public pszHeader As String
            public IntPtr pszHeader;
            public int cchHeader;
            // <MarshalAs(UnmanagedType.LPTStr)>
            // Public pszFooter As String
            public IntPtr pszFooter;
            public int cchFooter;
            public int iGroupId;
            public uint stateMask;
            public uint state;
            public uint uAlign;
            // <MarshalAs(UnmanagedType.LPTStr)>
            // Public pszSubtitle As String
            public IntPtr pszSubtitle;
            public uint cchSubtitle;
            // <MarshalAs(UnmanagedType.LPTStr)>
            // Public pszTask As String
            public IntPtr pszTask;
            public uint cchTask;
            // <MarshalAs(UnmanagedType.LPTStr)>
            // Public pszDescriptionTop As String
            public IntPtr pszDescriptionTop;
            public uint cchDescriptionTop;
            // <MarshalAs(UnmanagedType.LPTStr)>
            // Public pszDescriptionBottom As String
            public IntPtr pszDescriptionBottom;
            public uint cchDescriptionBottom;
            public int iTitleImage;
            public int iExtendedImage;
            public int iFirstItem;
            public uint cItems;
            // <MarshalAs(UnmanagedType.LPTStr)>
            // Public pszSubsetTitle As String
            public IntPtr pszSubsetTitle;
            public uint cchSubsetTitle;
        }

        public enum CDDS
        {
            CDDS_PREPAINT = 0x1,
            CDDS_POSTPAINT = 0x2,
            CDDS_PREERASE = 0x3,
            CDDS_POSTERASE = 0x4,
            CDDS_ITEM = 0x10000,
            CDDS_ITEMPREPAINT = (CDDS_ITEM | CDDS_PREPAINT),
            CDDS_ITEMPOSTPAINT = (CDDS_ITEM | CDDS_POSTPAINT),
            CDDS_ITEMPREERASE = (CDDS_ITEM | CDDS_PREERASE),
            CDDS_ITEMPOSTERASE = (CDDS_ITEM | CDDS_POSTERASE),
            CDDS_SUBITEM = 0x20000
        }

        [Flags()]
        public enum CDRF
        {
            CDRF_DODEFAULT = 0x0,
            CDRF_NEWFONT = 0x2,
            CDRF_SKIPDEFAULT = 0x4,
            CDRF_DOERASE = 0x8,
            CDRF_SKIPPOSTPAINT = 0x100,
            CDRF_NOTIFYPOSTPAINT = 0x10,
            CDRF_NOTIFYITEMDRAW = 0x20,
            CDRF_NOTIFYSUBITEMDRAW = 0x20,
            CDRF_NOTIFYPOSTERASE = 0x40
        }

        [DllImport("User32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
        private extern static int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LvGroup lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
        private extern static int SendMessage(IntPtr hWnd, int Msg, int wParam, ref RECT lParam);

        private const int WM_REFLECT = 0x2000;
        private const int WM_NOFITY = 0x4E;
        private const int NM_FIRST = 0x0;
        private const int NM_CUSTOMDRAW = NM_FIRST - 12;

        public const int LVCDI_GROUP = 0x1;

        public const int LVGGR_HEADER = 1;  // Header only (collapsed group)

        public const int LVM_FIRST = 0x1000;
        public const int LVM_GETGROUPRECT = (LVM_FIRST + 98);
        public const int LVM_GETGROUPINFO = (LVM_FIRST + 149);

        public const int LVGF_HEADER = 0x1;
        public const int LVGF_STATE = 0x4;
        public const int LVGF_GROUPID = 0x10;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_REFLECT + WM_NOFITY)
            {
                var pnmhdr = (NMhdr)m.GetLParam(typeof(NMhdr));
                if (pnmhdr.code == NM_CUSTOMDRAW)
                {
                    var pnmlv = (NmLVCustomDraw)m.GetLParam(typeof(NmLVCustomDraw));
                    if (pnmlv.nmcd.dwDrawStage == CDDS.CDDS_PREPAINT &&
                        pnmlv.dwItemType == LVCDI_GROUP)
                    {
                        RECT rectHeader = new()
                        {
                            top = LVGGR_HEADER,
                        };
                        int nItem = (int)pnmlv.nmcd.dwItemSpec;
                        SendMessage(m.HWnd, LVM_GETGROUPRECT, nItem, ref rectHeader);
                        using (Graphics g = Graphics.FromHdc(pnmlv.nmcd.hdc))
                        {
                            Rectangle rect = new(rectHeader.left, rectHeader.top, rectHeader.right - rectHeader.left, rectHeader.bottom - rectHeader.top);

                            // Dim linGrBrush As New LinearGradientBrush(New System.Drawing.Point(0, 0), New System.Drawing.Point(rectHeader.right, rectHeader.bottom), Color.Blue, Color.LightCyan)
                            SolidBrush BgBrush = new(GroupBackgroundColor);
                            g.FillRectangle(BgBrush, rect);

                            LvGroup lvg = new();
                            lvg.cbSize = (uint)Marshal.SizeOf(lvg);
                            lvg.mask = LVGF_STATE | LVGF_GROUPID | LVGF_HEADER;
                            SendMessage(m.HWnd, LVM_GETGROUPINFO, nItem, ref lvg);
                            var sText = Marshal.PtrToStringUni(lvg.pszHeader);

                            SizeF textSize = g.MeasureString(sText, Font);

                            int RectHeightMiddle = (int)((rect.Height - textSize.Height) / (double)2);

                            rect.Offset(10, RectHeightMiddle);

                            using SolidBrush drawBrush = new(GroupForegroundColor);
                            g.DrawString(sText, Font, drawBrush, rect);
                            rect.Offset(0, -RectHeightMiddle);

                            using SolidBrush lineBrush = new(GroupSeparatorColor);
                            g.DrawLine(new Pen(lineBrush), rect.X + g.MeasureString(sText, Font).Width + 10, rect.Y + (int)(rect.Height / (double)2), rect.X + (int)(rect.Width * 95 / (double)100), rect.Y + (int)(rect.Height / (double)2));
                        }
                        m.Result = new IntPtr((int)CDRF.CDRF_SKIPDEFAULT);
                        return;
                    }
                }
            }
            base.WndProc(ref m);
        }
    }
}
