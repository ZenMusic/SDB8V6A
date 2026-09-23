using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class DialogGetInfo : Form
    {
        Display1 display1;
        // display1was
        Rectangle tagBounds, tagRec;

        FileInfoItem finfo;

        public DialogGetInfo(Rectangle rec, Rectangle bounds, Display1 d1, FileInfoItem fileinfo, Image img)
        {
            InitializeComponent();

            // Make pb1 resize with the form
            pb1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pbx1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            finfo = fileinfo;

            display1 = d1;
            tagBounds = bounds;
            tagRec = rec;
            tbX.Text = bounds.X.ToString();
            tbY.Text = bounds.Y.ToString();
            tbWidth.Text = bounds.Width.ToString();
            tbHeight.Text = bounds.Height.ToString();

            this.Location = new Point(1300, 200);
            //pb1.Image = img;
            displayThisImage(img);
            if (finfo != null)
                this.rtbImageFullPath.Text = finfo.fpath;
            display1.hideTag();
            // d1.clearDisplay();
            //  d1.displayThisImage(img);
        }

        public void init2()
        {

        }
        private bool imageInitialized = false;

        public void displayThisImage(Image img)
        {
            pb1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pb1.Image = img;
            this.pb1.BorderStyle = BorderStyle.None;
            pb1.Image = null;
            pb1.Hide();

            pbx1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pbx1.Image = img;
            this.pbx1.BorderStyle = BorderStyle.None;
            pbx1.BringToFront();
            this.Refresh();
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void DialogGetInfo_Load(object sender, EventArgs e)
        {

        }

        private void DialogGetInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            //display1.cancelOutlining();
            display1.eraseOutline();
        }

        private void DialogGetInfo_Move(object sender, EventArgs e)
        {
            display1.repaint();
        }

        private void btReset_Click(object sender, EventArgs e)
        {
            tagBounds.X += 4;
            tagBounds.Y += 4;
            display1.reshowTag(tagBounds);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            display1.hideTag();
            this.Close();
        }

        public void closeWithoutSaving()
        {
            this.Close();
        }
        private void btClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bDisplayTag_Click(object sender, EventArgs e)
        {

        }

        private void btSave_Click(object sender, EventArgs e)
        {

        }

        private void btPbxShow_Click(object sender, EventArgs e)
        {
            pb1.Hide();
            pbx1.BringToFront();
        }
        // --- pbx1 fullscreen toggle ---
        private Rectangle _pbx1SavedBounds;
        private bool _pbx1IsFullscreen = false;

        private void SetPbx1Fullscreen()
        {
            // Save current bounds so we can restore later
            _pbx1SavedBounds = pbx1.Bounds;
            _pbx1IsFullscreen = true;

            pbx1.Anchor = AnchorStyles.None;   // detach anchor while repositioning
            pbx1.SetBounds(0, 0, ClientSize.Width, ClientSize.Height);
            pbx1.BringToFront();
            pbx1.Focus();
        }

        private void RestorePbx1Size()
        {
            _pbx1IsFullscreen = false;
            pbx1.SetBounds(_pbx1SavedBounds.X,
                           _pbx1SavedBounds.Y,
                           _pbx1SavedBounds.Width,
                           _pbx1SavedBounds.Height);
            pbx1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                        | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void pbx1_DoubleClick(object sender, EventArgs e)
        {
            if (_pbx1IsFullscreen)
                RestorePbx1Size();
            else
                SetPbx1Fullscreen();
        }
        private void SetPbx1MaxSize()
        {
            int margin = 6;
            int left = tbKeys.Right + margin;
            int top = margin;
            int width = ClientSize.Width - left - margin;
            int height = btReset.Top - margin * 2;

            pbx1.SetBounds(left, top, width, height);
            pbx1.BringToFront();
        }

        private void btMaxSize_Click(object sender, EventArgs e)
        {
            //SetPbx1MaxSize();
            SetPbx1Fullscreen();
        }
        
    }
}
