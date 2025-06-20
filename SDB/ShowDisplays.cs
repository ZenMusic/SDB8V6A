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
    public partial class ShowDisplay : Form
    {
        public ShowDisplay()
        {
            InitializeComponent();
            this.Text = "1";
            this.Refresh();
        }
        public ShowDisplay(int iscreen)
        {
            InitializeComponent();
            setText(iscreen.ToString());
            this.Refresh();
        }
        public ShowDisplay(int iscreen, int X, int Y)
        {
            InitializeComponent();
            setText(iscreen.ToString());
            this.Text = String.Format("{0:##}   {1:####}x{2:####}", iscreen, X, Y);
            btTopLeftX.Text = X.ToString();
            btTopLeftY.Text = Y.ToString();
            this.Refresh();
        }

        public void YourLocation(Point xLocation)
        {
            btWidth.Text = xLocation.X.ToString();
            btHeight.Text = xLocation.Y.ToString();
        }
        public void setText(string stext)
        {
            this.Text = stext;
            this.richTextBox1.Text = stext;
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
           
        }
    }
}
