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

namespace CobToolsList
{
    public partial class MyListView : ListView
    {
        
        private Point MouseLocation = Point.Empty;
        private Rectangle rect = Rectangle.Empty;

        public MyListView()
        {
            InitializeComponent();
        }
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
            for (int i =0;i<Items.Count;i++)
            {
                Rectangle tempRect = GetItemRect(i);
                if (tempRect.Contains(e.Location) && tempRect != rect)
                {
                    Invalidate();
                    Application.DoEvents();
                    rect = tempRect;
                    Draw();
                }
            }
        }

        private void Draw()
        {
            Graphics g = Graphics.FromHwnd(this.Handle);
            Brush HighLight = new LinearGradientBrush(rect, Color.FromArgb(20, Color.White), Color.Transparent, 90.0f);
            g.FillRectangle(HighLight, rect);
        }
    }
}
