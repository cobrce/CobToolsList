using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace CobToolsList
{
    public partial class MyListView : ListView
    {

        private Point MouseLocation = Point.Empty;
        private Rectangle rect = Rectangle.Empty;
        private int ItemIndex;
        public Size Size1;
        public Size Size2;

        public MyListView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Scrollable = true;
        }

        #region Hide scrollbar and Disable tooltip
        // Credit 1 : http://stackoverflow.com/a/2500089/5822322
        // Credit 2 : http://stackoverflow.com/a/33964099/5822322
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public Int32 code;
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x83;
            const int GWL_STYLE = -16;
            const int WS_VSCROLL = 0x00200000;
            const int WS_HSCROLL = 0x00100000;
            const int WM_NOTIFY = 78;
            const int TTN_FIRST = -520;
            const int TTN_NEEDTEXT = (TTN_FIRST - 10);

            if (m.Msg == WM_NOTIFY)
            {
                var nmHdr = (NMHDR)m.GetLParam(typeof(NMHDR));
                if (nmHdr.code == TTN_NEEDTEXT)
                    return;
            }

            if (m.Msg == WM_NCCALCSIZE)
            {
                int style = (int)GetWindowLong(this.Handle, GWL_STYLE);
                if ((style & WS_HSCROLL) == WS_HSCROLL || (style & WS_VSCROLL) == WS_VSCROLL)
                {
                    style &= ~(WS_HSCROLL | WS_VSCROLL);

                    SetWindowLong(this.Handle, GWL_STYLE, style);
                }
            }
            base.WndProc(ref m);
        }
        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
                return (int)GetWindowLong32(hWnd, nIndex);
            else
                return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
        }
        public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 4)
                return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            else
                return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }
        #endregion

        #region Programatically Scroll / credit : https://social.msdn.microsoft.com/Forums/en-US/06aee8b9-1c74-490e-a009-2904166e87a9/programmatically-scroll-a-listview-vertically?forum=winforms
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            const int WM_HSCROLL = 0x114;

            bool ctrl = ((GetAsyncKeyState(Keys.ControlKey) & 0x8000) == 0x8000);

            if (e.Delta > 0)
                if (ctrl)
                {
                    this.View = System.Windows.Forms.View.LargeIcon;
                    if (!Size1.IsEmpty)
                        this.Size = Size1;
                }
                else
                    SendMessage(this.Handle, (uint)WM_HSCROLL, (System.UIntPtr)ScrollEventType.SmallDecrement, (System.IntPtr)0); // todo : manual scroll
            else
                if (ctrl)
                {
                    this.View = System.Windows.Forms.View.SmallIcon;
                    if (!Size2.IsEmpty)
                        this.Size = Size2;
                }
                else SendMessage(this.Handle, (uint)WM_HSCROLL, (System.UIntPtr)ScrollEventType.SmallIncrement, (System.IntPtr)0); // todo : manual scroll; // todo : manual scroll

            if (!ctrl)
            {
                int index = ItemFromLocation(this.PointToClient(MousePosition));

                if (index != -1)
                    ItemIndex = index;
                Rectangle temprect = GetItemRect(ItemIndex);
                if (rect != temprect)
                {
                    rect = temprect;
                    Invalidate();
                    Application.DoEvents();
                    Draw();
                }
            }
            //base.OnMouseWheel(e);
        }
        #endregion

        #region  Native APIs
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        #endregion
        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            base.OnItemSelectionChanged(e);
            Rectangle tempRect = GetItemRect(e.ItemIndex);
            if (rect == tempRect)
            {
                Application.DoEvents();
                Draw();
            }
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            this.Select();
            base.OnMouseEnter(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int index = ItemFromLocation(e.Location);
            if (index == -1) return;
            Rectangle tempRect = GetItemRect(index); ;// GetItemRect(i);
            if (tempRect != rect)
            {
                ItemIndex = index;
                Invalidate();
                Application.DoEvents();
                rect = tempRect;
                Draw();
            }
        }

        private int ItemFromLocation(Point point)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Rectangle rect = GetItemRect(i);
                if (rect.Contains(point))
                    return i;
            }
            return -1;
        }

        private void Draw()
        {
            Graphics g = Graphics.FromHwnd(this.Handle);
            Brush HighLight = new LinearGradientBrush(rect, Color.FromArgb(20, Color.White), Color.Transparent, 90.0f);
            g.FillRectangle(HighLight, rect);
        }
    }
}
