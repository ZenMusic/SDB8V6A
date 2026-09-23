using System;
using System.Drawing;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class AllDisplaysInfoForm : Form
    {
        private ListView lv;
        GlobalVars gv;
        public AllDisplaysInfoForm(GlobalVars globalVars)
        {
            gv = globalVars;
            this.Text = "Display Information";
            this.Size = new Size(620, 280);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.TopMost = true;

            lv = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 11f)
            };

            lv.Columns.Add("#", 50, HorizontalAlignment.Center);
            lv.Columns.Add("Device", 110, HorizontalAlignment.Left);
            lv.Columns.Add("X", 80, HorizontalAlignment.Right);
            lv.Columns.Add("Y", 80, HorizontalAlignment.Right);
            lv.Columns.Add("Width", 90, HorizontalAlignment.Right);
            lv.Columns.Add("Height", 90, HorizontalAlignment.Right);
            lv.Columns.Add("Primary", 70, HorizontalAlignment.Center);

            this.Controls.Add(lv);
        }

        public void PopulateScreens()
        {
            lv.Items.Clear();

            foreach (var s in Screen.AllScreens)
            {
                int winNum = GetWindowsDisplayNumber(s);
                var item = new ListViewItem(winNum.ToString());
                item.SubItems.Add(s.DeviceName);
                item.SubItems.Add(s.Bounds.X.ToString());
                item.SubItems.Add(s.Bounds.Y.ToString());
                item.SubItems.Add(s.Bounds.Width.ToString());
                item.SubItems.Add(s.Bounds.Height.ToString());
                item.SubItems.Add(s.Primary ? "YES" : "");

                if (s.Primary)
                    item.BackColor = Color.LightSteelBlue;

                lv.Items.Add(item);
            }
            lv.Columns[1].Width = -2;  // -2 = auto-fit to column header or content (whichever is wider)
            // sort by Windows display number
            lv.ListViewItemSorter = new DisplayNumberSorter();
        }

        private static int GetWindowsDisplayNumber(Screen s)
        {
            if (s.Primary) return 1;

            static int GdiIndex(Screen scr)
            {
                var m = System.Text.RegularExpressions.Regex.Match(scr.DeviceName ?? "", @"\d+$");
                return m.Success ? int.Parse(m.Value) : 0;
            }

            var nonPrimary = new System.Collections.Generic.List<Screen>();
            foreach (var scr in Screen.AllScreens)
                if (!scr.Primary) nonPrimary.Add(scr);
            nonPrimary.Sort((a, b) => GdiIndex(a).CompareTo(GdiIndex(b)));

            int idx = nonPrimary.FindIndex(x => x.DeviceName == s.DeviceName);
            return idx + 2;
        }

        private class DisplayNumberSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                int a = int.TryParse(((ListViewItem)x).Text, out int ia) ? ia : 0;
                int b = int.TryParse(((ListViewItem)y).Text, out int ib) ? ib : 0;
                return a.CompareTo(b);
            }
        }

        private void AllDisplaysInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            gv.mainWindow._allDisplaysInfoForm = null;
        }
    }
}