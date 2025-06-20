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
    public partial class ERROR : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        GlobalVars gv;
        Movies parentMovies;
        Movies2 parentMovies2;
        WmPlayer parentPlayer;
        WmPlayer parentPlayer2;

        public ERROR(GlobalVars g, WmPlayer parentWin, bool onErrContinueFlag)
        {
            InitializeComponent();
            gv = g;
            parentPlayer = parentWin;

            //this.TopMost = true;
            gv.errWindow = this;
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
                                                        //this.Parent = (Form) parentWin;
            initTimer();

            tbPreviousFpath.Text = gv.mp.getPreviousTitle();
            tbFpath.Text = gv.mp.getTitle();
            tbPreviousPath2.Text = gv.mp.getPreviousTitle2();
            SetHandleErrAndContinue(onErrContinueFlag);
            if (gv.bSurpressErrorDialog)
                OK_Continue();
        }
        public ERROR(GlobalVars g, Movies parentWin, bool onErrContinueFlag)
        {
            InitializeComponent();
            gv = g;
            parentMovies = parentWin;

            //this.TopMost = true;
            gv.errWindow = this;
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
                                                        //this.Parent = (Form) parentWin;
            initTimer();

            tbPreviousFpath.Text = gv.mp.getPreviousTitle();
            tbFpath.Text = gv.mp.getTitle();
            tbPreviousPath2.Text = gv.mp.getPreviousTitle2();
            SetHandleErrAndContinue(onErrContinueFlag);
            if (gv.bSurpressErrorDialog)
                OK_Continue();
        }
        public ERROR(GlobalVars g, Movies2 parentWin, bool onErrContinueFlag)
        {
            InitializeComponent();
            gv = g;
            parentMovies2 = parentWin;

            //this.TopMost = true;
            gv.errWindow = this;
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
                                                        //this.Parent = (Form) parentWin;
            initTimer();

            tbPreviousFpath.Text = gv.mp.getPreviousTitle();
            tbFpath.Text = gv.mp.getTitle();
            tbPreviousPath2.Text = gv.mp.getPreviousTitle2();
            SetHandleErrAndContinue(onErrContinueFlag);
            if (gv.bSurpressErrorDialog)
                OK_Continue();
        }
        public void CloseErrorMessage()
        {
            this.Close();
        }
        public void HandleErrAndContinue()
        {
            bool closeThis = gv.dialogTraverser.OnErrContinue();
            if (closeThis)
                this.Close();
        }
        public void SetHandleErrAndContinue(bool handleOnErr)
        {
            cbHandleErrorAndContinue.Checked = handleOnErr;
        }
        int counter = 60;
        public void initTimer()
        {
            timer.Interval = 1000;              // Timer will tick every 1 second
            timer.Enabled = true;                       // Enable the timer
            timer.Start();                              // Start the timer
        }
        private void btExit_Click(object sender, EventArgs e)
        {
            timer.Stop();
            Environment.Exit(0);
        }
        void timer_Tick(object sender, EventArgs e) // TIMER EVENT <<<<<<<<<<<<<<<<<<<< gv.nextSlide 
        {
            tbCounter.Text = $"{counter}";
            if (--counter < 0)
            {
                timer.Stop();
                //Environment.Exit(0);
                gv.mp.Close();
                this.Close();
            }
            else if (counter < 50 && cbHandleErrorAndContinue.Checked)
                HandleErrAndContinue();

            this.BringToFront();
            gv.dialogTraverser.PlaybackErrorInMovies(gv.mp.getPreviousTitle());
        }
        private void btOk_Click(object sender, EventArgs e)
        {
            OK_Continue();
        }
        public void OK_Continue()
        { 
            timer.Stop();
            if (cbCloseMP.Checked)
            {
                gv.mp.Close();
                gv.mp = null;
            }
            gv.ERROR_STOP = false;
            //gv.dialogTraverser.ResetMoviesConnection(gv.mp);
            this.Close();
            if (parentMovies != null)
                parentMovies.AfterErrorRestart();
            else
                parentMovies2.AfterErrorRestart();
        }

        private void ERROR_FormClosing(object sender, FormClosingEventArgs e)
        {
            gv.errWindow = null;
        }
        
        private void btPause_Click(object sender, EventArgs e)
        {
            counter = 20;
            WmPlayer mp =  gv.GetMovies();
            if (mp == null)
            {
                tbTraverserMovieState.Text = "mp == null";
                return;
            }
            cbMovies.Checked = !(mp == null);
            if (mp != null && !mp.IsDisposed)
            {
                tbTraverserMovieState.Text = mp.getState();
                //tbTraverserMovieState.Text = gv.dialogTraverser.getMoviePlayerState();
            }
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            tbPreviousFpath.Text =  gv.mp.getPreviousTitle();
            tbPreviousPath2.Text = gv.mp.getPreviousTitle2();
            tbFpath.Text = gv.mp.getTitle();
        }
    }
}
