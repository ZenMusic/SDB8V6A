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
    public partial class DialogInvalidImage : Form
    {
        GlobalVars gv;
        public DialogInvalidImage()
        {
            InitializeComponent();
        }
        public DialogInvalidImage(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
        }
        int count = 0;
        public void Setup(string filePath, int rowNumber, int total, int nextImageId)
        {
            gv.todoAction = "";
            ++count;
            tbFilePath.Text = filePath;
            tbRowNumber.Text = rowNumber.ToString();
            tbTotal.Text = total.ToString();
            numNextImageId.Value = nextImageId;
            tbCount.Text = count.ToString();
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            gv.nextIdx = (int)numNextImageId.Value;
            if (gv.nextIdx >= gv.slideCount1)
            {
                gv.nextIdx = 0;
            }
            gv.todoAction = "stop";
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            gv.todoAction = "cancel";

        }

        private void btContinue_Click(object sender, EventArgs e)
        {
            gv.todoAction = "continue";

        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
