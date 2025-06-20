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
    public partial class TopMost : Form
    {
        GlobalVars gv;
        public TopMost(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            this.TopMost = true;
        }

        private void cbContinue_CheckedChanged(object sender, EventArgs e)
        {
            gv.continueCopy = cbContinue.Checked;
        }
        public bool CheckContinue()
        {
            return gv.continueCopy;
        }
    }
}
