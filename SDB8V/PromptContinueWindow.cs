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
    public partial class PromptContinueWindow : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        GlobalVars gv;
        int counter = 1;
        public PromptContinueWindow(GlobalVars g)
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = 1000;
            timer.Enabled = true;                       // Enable the timer
            timer.Start();
            counter = 1;
            gv = g;
            numCount.Value = gv.maxCountCopy;
            numCounter.Value = gv.SECONDS_TO_CONTINUE;
        }
        void timer_Tick(object sender, EventArgs e) // TIMER EVENT <<<<<<<<<<<<<<<<<<<< gv.nextSlide 
        {
            if (cbPause.Checked)
                return;
            tbCounter.Text =  counter++.ToString();
            if (gv.bCopyOnlyOneBatch && counter > 1)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }
            if (bCancel)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }
            if (counter > gv.SECONDS_TO_CONTINUE)
            {
                DialogResult = DialogResult.OK;
                return;
            }
        }

        private void PromptContinueWindow_Activated(object sender, EventArgs e)
        {
            counter = 1;
            timer.Start();
        }

        private void btContinue_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void numCounter_ValueChanged(object sender, EventArgs e)
        {
            gv.SECONDS_TO_CONTINUE = (int)numCounter.Value;
        }

        private void numCount_ValueChanged(object sender, EventArgs e)
        {
            gv.maxCountCopy = (int)numCount.Value;
        }
        bool bCancel = false;
        private void btCancel_Click(object sender, EventArgs e)
        {
            bCancel = true;
        }

        private void bt1000_Click(object sender, EventArgs e)
        {
        }
    }
}
