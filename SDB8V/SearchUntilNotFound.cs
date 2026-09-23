using SymbolDB;
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
    public partial class SearchUntilNotFound : Form
    {
        GlobalVars gv;
        DialogTraverser pTrav;
        public SearchUntilNotFound(GlobalVars g, DialogTraverser pTrav2)
        {
            gv = g;
            pTrav = pTrav2;
            InitializeComponent();
        }
        public void FileName(string s)
        {
            tbFileName.Text = s;
        }   

        private void searchTimer_Tick(object sender, EventArgs e)
        {
            if (!cbSearch.Checked)
                return;

            if (!SearchUntilNotFoundFunc())
            {
                cbSearch.Checked = false;
                searchTimer.Enabled = false;
                btStart.Text = "start";
            }
        }

        public bool SearchUntilNotFoundFunc()
        {
            return pTrav.SearchUntilNotFoundStep();
        }
        bool bFound = false;
        private void btStart_Click(object sender, EventArgs e)
        {
            cbSearch.Checked = !cbSearch.Checked;
            searchTimer.Enabled = cbSearch.Checked;
            btStart.Text = cbSearch.Checked ? "stop" : "start";

            if (cbSearch.Checked)
            {
                bFound = SearchUntilNotFoundFunc();
                if (!bFound)
                {
                    cbSearch.Checked = false;
                    searchTimer.Enabled = false;
                    btStart.Text = "start";
                }
            }
        }

        private void cbSearch_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btSkipToNext_Click(object sender, EventArgs e)
        {
            pTrav.playNextMovie();
            if (!cbSearch.Checked) 
            { 
                btStart_Click(null, null);
            }
        }
    }
}
