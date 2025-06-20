using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class DisplayMessage : Form
    {
        GlobalVars gv;
        TraverserDialog pTraverser = null;
        public DisplayMessage(GlobalVars g, TraverserDialog p)
        {
            InitializeComponent();
            gv = g;
            pTraverser = p;
        }
        public void DisplayCopyMessageText(bool specialFolder,  string targetDir1, string txt2 = null, string targetDir = "root")
        {
            if (!specialFolder)
                tbAAA.Text = "";
            else
                tbAAA.Text = "AAAA";

            string folderName = Path.GetFileName(targetDir1);
            tbFolder.Text = folderName;

            tbMessage.Text = targetDir;
            if (txt2 != null)
            {
                tbCopy2.Text = txt2;
                tbCopy3.Text = targetDir;
            }

            string fileName = Path.GetFileName(txt2);
            tbFileName.Text = fileName;
            this.Refresh();
        }
        public void SetTopMost(bool on)
        {
            this.TopMost = on;
        }
        public void HideWin()
        {
            tbMessage.Text = "copy";
            SetTopMost(false);
        }
        public void ShowWin()
        {
            SetTopMost(true);
        }

        private void btHideMovie_Click(object sender, EventArgs e)
        {
            pTraverser.btHideMovie_Click(sender, e);
        }
    }
}
