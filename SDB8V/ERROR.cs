#nullable disable
using System;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class ERROR : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        GlobalVars gv;
        Movies parentMovies;
        //Movies2 parentMovies2;
        Player parentPlayer;
        Player parentPlayer2;

        public ERROR(GlobalVars g, Player parentWin, bool onErrContinueFlag)
        {
            InitializeComponent();
            gv = g;
            parentPlayer = parentWin;

            //this.TopMost = true;
            gv.errWindow = this;
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
                                                        //this.Parent = (Form) parentWin;
            initTimer();

            tbPreviousFpath.Text = gv.vlcPlayer.getPreviousTitle();
            tbFpath.Text = gv.vlcPlayer.getTitle();
            tbPreviousPath2.Text = gv.vlcPlayer.getPreviousTitle2();
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

            tbPreviousFpath.Text = gv.vlcPlayer.getPreviousTitle();
            tbFpath.Text = gv.vlcPlayer.getTitle();
            tbPreviousPath2.Text = gv.vlcPlayer.getPreviousTitle2();
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
            bool closeThis = gv.dialogTraverser1.OnErrContinue();
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
                gv.vlcPlayer.Close();
                this.Close();
            }
            else if (counter < 50 && cbHandleErrorAndContinue.Checked)
                HandleErrAndContinue();

            this.BringToFront();
            gv.dialogTraverser1.PlaybackErrorInMovies(gv.vlcPlayer.getPreviousTitle());
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
                gv.vlcPlayer.Close();
                gv.vlcPlayer = null;
            }
            gv.ERROR_STOP = false;
            //gv.dialogTraverser.ResetMoviesConnection(gv.vlcPlayer);
            this.Close();
            
        }

        private void ERROR_FormClosing(object sender, FormClosingEventArgs e)
        {
            gv.errWindow = null;
            try
            {
                timer?.Stop();
                timer?.Dispose();
            }
            catch { }
        }
        
        private void btPause_Click(object sender, EventArgs e)
        {
            counter = 20;
            Player mp = gv.vlcPlayer;
            if (mp == null)
            {
                tbTraverserMovieState.Text = "mp == null";
                return;
            }
            cbMovies.Checked = !(mp == null);
            if (mp != null && !mp.IsDisposed)
            {
                tbTraverserMovieState.Text = mp.GetState();
                //tbTraverserMovieState.Text = gv.dialogTraverser.getMoviePlayerState();
            }
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            tbPreviousFpath.Text =  gv.vlcPlayer.getPreviousTitle();
            tbPreviousPath2.Text = gv.vlcPlayer.getPreviousTitle2();
            tbFpath.Text = gv.vlcPlayer.getTitle();
        }
    }

    // Lightweight stubs for Movies / Movies2 so the real implementations can be removed.
    // These provide only the minimal public surface that ERROR.cs uses.
    // Replace with full implementations later.

    public partial class Movies : Form
    {
        public Movies()
        {
        }

        // Called by ERROR.OK_Continue()
        public virtual void AfterErrorRestart2()
        {
            // intentionally empty stub
        }

        // Additional helpers (safe no-op) in case other parts of the solution call them.
       // public virtual void SetOnErrContinue(bool onErrContinue) { }
      //  public virtual void MinimizeMovieWin(bool min = true) { }
        public virtual string GetTitle() => string.Empty;
        public virtual string GetPreviousTitle() => string.Empty;
        public virtual string GetPreviousTitle2() => string.Empty;
    }

    public class Movies3 : Movies
    {
        public Movies3()
        {
        }

        
    }
}
