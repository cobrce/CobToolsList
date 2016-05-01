using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CobToolsList
{
    public partial class Form1 : Form
    {
        List<string> files = new List<string>();
        private bool close = true;
        public Form1()
        {
            InitializeComponent();
            this.Text = string.Empty;
            this.ControlBox = false;
            Opacity = 0.90;
            Color B = Color.FromArgb(0x22, 0x22, 0x22);
            BackColor = B;
            ForeColor = Color.DodgerBlue;
            foreach (Control ctrl in splitContainer1.Panel1.Controls)
            {
                ctrl.BackColor = B;
                ctrl.ForeColor = Color.DodgerBlue;
            }
            listView1.BackColor = Color.FromArgb(0x11, 0x11, 0x11);
            listView1.ForeColor = Color.DodgerBlue;

            
        }
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int WM_NCPAINT = 0x0085;
            //const int WM_SYSCOMMAND=0x0112;
            //const int SC_VSCROLL=0xF070;

            if (m.Msg == WM_NCHITTEST)
                return;
            else if (m.Msg == WM_NCPAINT)
            {
                IntPtr DC = GetWindowDC(Handle);
                Graphics g = Graphics.FromHdc(DC);
                g.Clear(Color.FromArgb(0x22, 0x22, 0x22));
                g.Flush();
                return;
            }
            base.WndProc(ref m);
        }

        #region DropsShadow / credit : http://stackoverflow.com/a/9775337/5822322
        private const int CS_DROPSHADOW = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {

                CreateParams parameters = base.CreateParams;

                if (DropShadowSupported)
                {
                    parameters.ClassStyle = (parameters.ClassStyle | CS_DROPSHADOW);
                }

                return parameters;
            }
        }
        public static bool DropShadowSupported
        {
            get
            {
                OperatingSystem system = Environment.OSVersion;
                bool runningNT = system.Platform == PlatformID.Win32NT;

                return runningNT && system.Version.CompareTo(new Version(5, 1, 0, 0)) >= 0;
            }
        }
        #endregion

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (close && !checkBox1.Checked)
                Close();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
            else if (e.KeyCode == Keys.Enter)
                listView1_Click(null, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Select();
            List<Item> Items = Settings.Load();
            files = (from Item i in Items select i.path).ToList();
            ImageList list = new ImageList() { ImageSize = SystemInformation.IconSize, ColorDepth = ColorDepth.Depth32Bit };
            ImageList smalllist = new ImageList() { ImageSize = SystemInformation.SmallIconSize, ColorDepth = ColorDepth.Depth32Bit };
            foreach (Item item in Items)
            {
                Icon icon = Icon.ExtractAssociatedIcon(item.path);
                list.Images.Add(icon);
                smalllist.Images.Add(icon);
                listView1.Items.Add(new ListViewItem(item.label, list.Images.Count - 1) { Tag = item.path });
            }
            listView1.LargeImageList = list;
            listView1.SmallImageList = smalllist;           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            close = false;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!files.Contains(openFileDialog1.FileName))
                {
                    string file = openFileDialog1.FileName;
                    files.Add(file);
                    Icon icon = Icon.ExtractAssociatedIcon(file);
                    ImageList list = listView1.LargeImageList;
                    listView1.SmallImageList.Images.Add(icon);
                    list.Images.Add(icon);
                    listView1.Items.Add(new ListViewItem(Path.GetFileNameWithoutExtension(file), list.Images.Count - 1) { Tag = file });
                }
            }
            close = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<Item> itms = new List<Item>();
            foreach (ListViewItem item in listView1.Items)
            {
                itms.Add(new Item() { label = item.Text, path = item.Tag.ToString() });
            }
            Settings.Save(itms);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Sort();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems!=null && listView1.SelectedItems.Count>0)
            {
                close = false;
                if (MessageBox.Show("Delete selected item?", "Sure?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    ListViewItem lvm = listView1.SelectedItems[0];
                    lvm.Remove();
                    files.Remove(lvm.Tag.ToString());
                }
                close = true;
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems!=null && listView1.SelectedItems.Count>0)
            {
                this.Hide();
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo(listView1.SelectedItems[0].Tag.ToString()) { UseShellExecute = true};
                process.Start();
                Close();
            }
        }

        private void checkBox1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 5, 10, 10);
            
            e.Graphics.Clear(checkBox1.BackColor);
            e.Graphics.DrawEllipse(Pens.DodgerBlue,rect);
            if (checkBox1.Checked)
                e.Graphics.FillEllipse(Brushes.DodgerBlue, rect);
            e.Graphics.DrawString(checkBox1.Text, checkBox1.Font, Brushes.DodgerBlue, new PointF(15, 2));
        }

    }
}
