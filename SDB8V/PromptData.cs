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
    public partial class PromptData : Form
    {
        GlobalVars gv;

        public PromptData(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int result = 0;
            try
            {
                result = Convert.ToInt32(tb1.Text);

            }
            catch (OverflowException)
            {

            }
            catch (FormatException)
            {

            }
            gv.iValue = result;
            this.Close();
        }
    }
}
