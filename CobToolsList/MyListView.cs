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
        private int ItemIndex = -1;
        public Size Size1;
        public Size Size2;
        private Color DarkColor = Color.FromArgb(0x22, 0x22, 0x22);

        public MyListView()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Scrollable = true;
            this.OwnerDraw = true;
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            base.OnDrawItem(e);
            Image img;
            ListViewItem item = e.Item;            
            
            bool MouseOver = item.Index == ItemIndex;
            Font font =  item.Font;
            

            string text = /*((MouseOver) ? Environment.NewLine : "") + */item.Text;

            int X = e.Bounds.X, Y = 0;
            if (View == System.Windows.Forms.View.LargeIcon)
            {
                img = LargeImageList.Images[e.Item.ImageIndex];
                X = (e.Bounds.Width - img.Width) / 2 + e.Bounds.X;
            }
            else
            {
                img = SmallImageList.Images[e.Item.ImageIndex];
                Y = (e.Bounds.Height - img.Height) / 2 + e.Bounds.Y;
            }
            Rectangle imgrect = new Rectangle(X, Y, img.Width, img.Height);

            // draw background
            if (MouseOver)
            {
                Brush HighLight = new LinearGradientBrush(e.Bounds, DarkColor, Color.Transparent, 90.0f);
                e.Graphics.FillRectangle(HighLight, e.Bounds);
            }
            if (item.Selected)
            {
                Brush HighLight = new LinearGradientBrush(e.Bounds, Color.FromArgb(60, Color.DodgerBlue), Color.Transparent, 90.0f);
                e.Graphics.FillRectangle(HighLight, e.Bounds);
            }
            // draw icon
            e.Graphics.DrawImage(img, imgrect);

            // draw text            
            Rectangle labelBounds = item.GetBounds(ItemBoundsPortion.Label);
            Size textSize = e.Graphics.MeasureString(text, font).ToSize(); ;

            if (!MouseOver)
                while (true) // shorten the item text
                {
                    textSize = e.Graphics.MeasureString(text, font).ToSize();
                    if (textSize.Width <= e.Bounds.Width)
                        break;
                    text = text.Substring(0, text.Length - 4) + "..";
                }
            e.Graphics.DrawString(text, font, Brushes.DodgerBlue, (e.Bounds.Width - textSize.Width) / 2 + e.Bounds.X, labelBounds.Y + ((MouseOver) ? (int)(textSize.Height*0.7) : 0));
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

            bool ctrl = false; //  ((GetAsyncKeyState(Keys.ControlKey) & 0x8000) == 0x8000);

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
                if (ItemIndex != -1)
                {
                    Rectangle temprect = GetItemRect(ItemIndex);
                    if (rect != temprect)
                    {
                        rect = temprect;
                        Invalidate();
                    }
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

        protected override void OnMouseLeave(EventArgs e)
        {
            ItemIndex = -1;
            rect = Rectangle.Empty;
            Invalidate();
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
                rect = tempRect;
                Invalidate();
                Application.DoEvents();
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
    }
}
