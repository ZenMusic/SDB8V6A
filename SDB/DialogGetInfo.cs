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
            d1.displayThisImage(img);
        }
        
        public void init2()
        {

        }
        public void displayThisImage(Image img)
        {
            pb1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pb1.Image = img;
            this.pb1.BorderStyle = BorderStyle.None;
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
    }
}
