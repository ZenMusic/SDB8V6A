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
    public partial class ErrorStop : Form
    {
        public ErrorStop()
        {
            InitializeComponent();
            this.AcceptButton = btCloseThisOneOnly;
            this.CancelButton = btExit;
        }

        private void btCloseThisOneOnly_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
