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
    public partial class DialogDeleteDup : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        GlobalVars gv;
        int counter = 1;
        int SECONDS_TO_CONTINUE = 6;
        public DialogDeleteDup(GlobalVars g)
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = 1000;
            timer.Enabled = true;                       // Enable the timer
            timer.Start();
            counter = 1;
            gv = g;
            numCount.Value = gv.maxCountCopy;
            numCounter.Value = SECONDS_TO_CONTINUE;
        }
        void timer_Tick(object sender, EventArgs e) // TIMER EVENT <<<<<<<<<<<<<<<<<<<< gv.nextSlide 
        {

            tbCounter.Text = counter++.ToString();
            if (counter > SECONDS_TO_CONTINUE)
            {
                timer.Stop();
                gv.mainWindow.markForDeletion(true);
                endAndClose();
                return;
            }
        }
        void endAndClose()
        {
            timer.Stop();
            if (counter == -1)
            {
                DialogResult = DialogResult.Cancel;
                gv.mainWindow.DelReturnCodeContinue(false);
            }
            else  //counter > gv.SECONDS_TO_CONTINUE)
            {
                DialogResult = DialogResult.OK;
                gv.mainWindow.DelReturnCodeContinue(true);
            }
        }
        public void Restart()
        {
            counter = 0;
            timer.Start();

        }
        private void btContinue_Click(object sender, EventArgs e)
        {
            timer.Stop();
            gv.mainWindow.markForDeletion(true);
            endAndClose();
            return;
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            counter = -1;
            endAndClose();
            return;
        }
    }
}
