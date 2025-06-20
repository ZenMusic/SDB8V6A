using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class TransparentForm : Form
    {
        GlobalVars gv;
        public TransparentForm(GlobalVars g, Point loc)
        {
            InitializeComponent();
            gv = g;
            this.BackColor = Color.LightBlue;
            this.TransparencyKey = Color.LightBlue;
            this.Location = loc;
            //this.TopMost = true;
        }
        public void MoveWin(Point loc)
        {
            this.Location = loc;
        }
        public void SetText(string txt)
        {
            tbPosition.Text = txt;
        }
        private void TransparentForm_Load(object sender, EventArgs e)
        {

        }
        Color transKey = Color.LightBlue;
        bool toggleOn = true;
        private void TransparentForm_DoubleClick(object sender, EventArgs e)
        {
            toggleOn = !toggleOn;
            if (toggleOn)
            {
                TransparencyKey = transKey;
            }
            else
                transKey = Color.Black;
        }

        private void TransparentForm_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        contextMenuStrip1.Show(this, new Point(e.X, e.Y));//places the menu at the pointer position
                    }
                    break;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        
        }

        private void visibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransparencyKey = Color.Black;
            //pictureBox1.Visible = false;
        }
        public void SetTopmost(bool bTop)
        {
            this.TopMost = bTop;
        }
        public void UpdateStatus(string pos, string dur)
        {
            tbDuration.Text = pos;
            tbPosition.Text = dur;

        }
        public void FoundFile(string fname)
        {
            tbFound.Text = fname;

        }
        public void FileName(string fname)
        {
            tbFileName.Text = fname;

        }
        public void UpdateStatus2(string dur)
        {
            tbPosition.Text = dur;

        }
        public void UpdateStatus(double pos , double dur)
        {
            int min =  (int)dur / 60;
            int seconds = (int)dur - ((int)(dur / 60) * 60);

            tbDuration.Text = $"{min} {seconds}";

            int pmin = (int)pos / 60;
            int pseconds = (int)dur - ((int)(pos / 60) * 60);

            tbPosition.Text = $"{pmin} {pseconds}";

        }
        private void transparentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransparencyKey = transKey;
            ///pictureBox1.Visible = false;
        }

        private void cbTopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = cbTopMost.Checked;
        }
    }
}
