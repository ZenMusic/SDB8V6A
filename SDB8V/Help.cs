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
    public partial class Help : Form
    {
        GlobalVars gv;
        string subject;

        public Help()
        {
            InitializeComponent();
        }

        public Help(GlobalVars g, string s)
        {
            InitializeComponent();
            gv = g;
            subject = s;
            this.Text = "Help " + subject;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void setSubject(string s)
        {
            subject = s;
            this.Text = "Help " + subject;
        }
    }
}
