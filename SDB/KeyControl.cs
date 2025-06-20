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
    public partial class KeyControl : Form
    {
        GlobalVars gv;
        TraverserDialog parent;

        public KeyControl(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            this.TopMost = true;
        }
        public void display()
        {
            this.TopMost = true;
            this.Show();
            this.TopMost = false;
        }
        public void AddWindowToController(TraverserDialog pw3)
        {
            //InitializeComponent();
            parent = pw3;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) /////// override dmcdmc123
        {
            if (keyData == (Keys.F9))
            {
                //gv.main.getContractDialog();
                return true;
            }

            return false;
        }
    }
}
