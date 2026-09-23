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
    public partial class TagPB : Form
    {
        Rectangle bounds = new Rectangle();
        public TagPB()
        {
            InitializeComponent();
           
        }
        public TagPB(Rectangle myBounds)
        {
            InitializeComponent();
            this.Bounds = myBounds;
            bounds = myBounds;
            showSize();
        }
        public void setTag(Rectangle myBounds)
        {
            this.Bounds = myBounds;
            bounds = myBounds;
            showSize();

        }
        public void showSize()
        {
            tbX.Text = bounds.X.ToString();
            tbY.Text = bounds.Y.ToString();
            tbW.Text = bounds.Width.ToString();
            tbH.Text = bounds.Height.ToString();
        }
        public void showSize(Rectangle rec)
        {
            tbX.Text = rec.X.ToString();
            tbY.Text = rec.Y.ToString();
            tbW.Text = rec.Width.ToString();
            tbH.Text = rec.Height.ToString();
        }
        private void Tag_Load(object sender, EventArgs e)
        {

        }

        private void Tag_SizeChanged(object sender, EventArgs e)
        {
            showSize(this.Bounds);
            //this.Text = this.Bounds
            this.Update();
        }
    }
}
