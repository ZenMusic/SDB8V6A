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
    public partial class PromptDirectory : Form
    {
        string folderName;

        public PromptDirectory()
        {
            InitializeComponent();
        }
        
        public string selectDir()
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                folderName = folderBrowserDialog1.SelectedPath;
                this.Text = folderName;
                return folderName;
            }
            return null;
        }
        private void PromptDirectory_Load(object sender, EventArgs e)
        {
            
            
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void PromptDirectory_MouseClick(object sender, MouseEventArgs e)
        {
            selectDir();
        }
    }
}
