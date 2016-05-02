using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CobToolsList
{
    public partial class MyButton : Button
    {
        private bool over = false;
        private bool click = false;
        private Brush Brush = Brushes.DodgerBlue;
        private Brush overBrush = Brushes.SkyBlue;
        private Brush clickBrush = Brushes.DeepSkyBlue;
        public MyButton()
        {
            InitializeComponent();
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            over = true;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            over = false;
        }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            click = true;
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            click = false;
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Brush B;
            if (click)
                B = clickBrush;
            else if (over)
                B = overBrush;
            else
                B = Brush;
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            pevent.Graphics.Clear(BackColor);
            Rectangle rect = ClientRectangle;
            rect.Inflate(-1, -1);

            pevent.Graphics.FillEllipse(B, rect);
            Size size = pevent.Graphics.MeasureString(Text, Font).ToSize();
            Point p = new Point((rect.Width - size.Width) / 2 + rect.X, (rect.Height - size.Height) / 2 + rect.Y);
            pevent.Graphics.DrawString(Text, Font, new SolidBrush(BackColor), p);
        }
    }
}
