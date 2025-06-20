using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using PVS.AVPlayer;
using WMPLib;
using SymbolDB;
using System.IO;
using AxWMPLib;

//  in Designer I uncommented  etc
//  private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
//
namespace SymbolDB
{
    public partial class Movies : Form
    {
        //public AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer2;
        //public AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer2;
        private System.ComponentModel.IContainer components = null;
        GlobalVars gv;
        string fpath1;
        string previousFpath;
        string previousFpath2;

        bool bDidZoomFullScreen = false;
        long height, width;
        bool bOnErrContinue = false;

        public Movies(GlobalVars g, TraverserDialog parent, bool onErrContinueFlag = false, string fpath = null) // main
        {
            InitializeComponent();
           // AddMediaPlayer2();
            //fpath = @"//T110/Users/david/Documents/111/aatest.mpg";
            gv = g;
            SavePreviousTitle(fpath);
            fpath1 = fpath;
            //tbMoviePath1.Text = fpath;
            //parentDialogTraverser = parent;
            pTraverser = parent;
            CallParentFirstMessage();
            bOnErrContinue = onErrContinueFlag;
            start_timer();
            try
            {
                axWindowsMediaPlayer1.URL = "" + fpath1; //tbMoviePath1.Text;
            }
            catch (Exception e)
            {
                //pTraverser.updateMovieStatus("ERROR Exception Movies URL");
                return;
            }
            SetTitleBar(fpath1);
            axWindowsMediaPlayer1.PreviewKeyDown += wmPlayer_dmcKeyDown;
            //axWindowsMediaPlayer1.OpenStateChange += _WMPOCXEvents_OpenStateChangeEventHandler(axWindowsMediaPlayer1);
            //WMPOCXEvents_OpenStateChangeEventHandler(axWindowsMediaPlayer1);
            //WindowsMediaPlayerClass wmp = new WindowsMediaPlayerClass();
            //IWMPMedia mediaInfo = axWindowsMediaPlayer1.newMedia("myfile.wmv");
            if (cbSetFocusOnParent.Checked)
                pTraverser.Activate();
            bDidZoomFullScreen = false;
            moveToNextScreen();
            string ext = Path.GetExtension(fpath1);
            if (string.IsNullOrEmpty(ext))
                return;
            ext = ext.ToUpper();
            if (ext.Contains("WEBM"))
            {
                ShowInBrowser(fpath);

            }
        }

        public void AfterErrorRestart()
        {
            pTraverser.PlayButtonGo();
        }
        bool bMinimize = false;
        public void MinimizeMovieWin(bool min = true)
        {
            bMinimize = min;
            if (min)
                this.WindowState = FormWindowState.Minimized;
            else
                this.WindowState = FormWindowState.Maximized;
        }
        public void SetOnErrContinue(bool onErrContinue)
        {
            bOnErrContinue = onErrContinue;

        }
        public void ShowInBrowser(string fpath)
        {
            System.Diagnostics.Process.Start(fpath);
        }

        bool bLockSmallScreen = false;
        public void SetSmallScreen(Rectangle locSize)
        {
            this.WindowState = FormWindowState.Normal;
            bZoomFullScreenOnTimer = false;
            bDidZoomFullScreen = false;
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
                axWindowsMediaPlayer1.fullScreen = false;
            //cbzoom.Checked = false;
           // this.Location = new Point(locSize.X, locSize.Y);
           // this.Size = new Size(locSize.Width, locSize.Height);
            bLockSmallScreen = true;
            if (false)
            {
                //axWindowsMediaPlayer1.stretchToFit = true;
                //axWindowsMediaPlayer1.Width *= 2;
                //axWindowsMediaPlayer1.Height *= 2;

                axWindowsMediaPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
                //again, the player is the same size as form
                axWindowsMediaPlayer1.Size = this.Size;
            }
            else
            {
                Point loc = this.Location;
                long w = this.width;
                long h = this.height;
                this.Text = "SMALL";
                axWindowsMediaPlayer1.Location = new Point(20, 20);
                axWindowsMediaPlayer1.Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 20);
                axWindowsMediaPlayer1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                this.Location = loc;
                this.width = w;
                this.height = h;
            }
        }
        public void SetSmallScreenLock(bool bLockSmallScreenOn)
        {
            if (bLockSmallScreenOn)
            {
                bLockSmallScreen = true;
                Rectangle size = new Rectangle();
                size.Width = (int)this.width;
                size.Height = (int)this.height;
                SetSmallScreen(size);
            }
            else
                bLockSmallScreen = false;
        }
        bool bCallParent = false;
        public void SetCallParent(bool flag)
        {
            bCallParent = flag;
        }
        public void CallParentFirstMessage()
        {
            //bCallParent = false;
            bool flag = pTraverser.CheckWithParent();
            if (flag)
            {
                cbSetFocusOnParent.Checked = true;
            }
        }
        public void FocusOnParent()
        {
            cbSetFocusOnParent.Checked = true;
        }
        public void SavePreviousTitle(string fpath)
        {
            if (string.IsNullOrEmpty(fpath))
                return;
            if (string.IsNullOrEmpty(previousFpath))
            {
                previousFpath = fpath;
                return;
            }
            if (previousFpath.Equals(fpath))
            {
                return;
            }
            previousFpath2 = previousFpath;
            previousFpath = fpath;
        }
        //
        //
        //
        public double loadMovie(string fpath, bool zoom)
        {
            SavePreviousTitle(fpath);
            fpath1 = fpath;
            if (gv.bSurpressErrorDialog)
            {
                gv.ERROR_STOP = false;
                gv.dialogTraverser.ClearError();
                //return 0;
            }
            else if (gv.ERROR_STOP)
            {
                DialogResult dialogResult = MessageBox.Show("Clear the error and continue?", "Media Error Deteched", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    gv.ERROR_STOP = false;
                    gv.dialogTraverser.ClearError();
                }
                else if (dialogResult == DialogResult.No)
                {
                    return 0;
                }
            }
            if (gv.errWindow != null)
            {
                //   return 0;
                gv.errWindow.Close();
            }
            string ext = Path.GetExtension(fpath);
            ext = ext.ToUpper();
            if (ext.Contains("WEBM"))
            {
                //ShowInBrowser(fpath);
                return 0;
            }
            if (string.IsNullOrEmpty(fpath) || string.IsNullOrWhiteSpace(fpath))
            {
                MessageBox.Show("ERROR in Movies");
                //return;
            }
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer1.ResetText(); //2021
            //tbMoviePath1.Text = fpath;
            secondsBetweenPositionUpdate = (int)numIntTimerSeconds.Value;
            bDidZoomFullScreen = false;
            bZoomFullScreenOnTimer = zoom;
            try
            {
                axWindowsMediaPlayer1.URL = "" + fpath1; //tbMoviePath1.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("Exception ERROR in Movies loadMovie URL assignment");
                SetTitleBar("x");
                return 0;
            }
            //if (cbSetFocusOnParent.Checked)
            //  parentDialogTraverser.Activate();
            SetTitleBar(fpath1);
            gv.debug.w("movies load");

            //axWindowsMediaPlayer1.settings.autoStart = true;

            return axWindowsMediaPlayer1.currentMedia.duration;
        }
        public void wmPlayer_PlayStateChange()
        {
            if (pTraverser != null)
                pTraverser.changeState();
            if (bMinimize)
                this.WindowState = FormWindowState.Minimized;
        }


        private void btPlay_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = "" + fpath1; //tbMoviePath1.Text;
            gv.debug.w("movies play");
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Movies_Load(object sender, EventArgs e)
        {

        }

        private void btSkipForward_Click(object sender, EventArgs e)
        {
            // axWindowsMediaPlayer1.PlayStateChange = WMPLib.WMPPlayState.wmppsScanForward;
            //playState = WMPLib.WMPPlayState.wmppsScanForward;
            //position in seconds
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = axWindowsMediaPlayer1.Ctlcontrols.currentPosition + 25;
            gv.debug.w("movies skip forward");
        }
        public void skipForward2()
        {
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = axWindowsMediaPlayer1.Ctlcontrols.currentPosition + 25;
            resetButtonColor();
            //btBack.BackColor = pushed;
            gv.debug.w("movies skip 2 forward");
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        private void axWindowsMediaPlayer1_MediaError(object sender, AxWMPLib._WMPOCXEvents_MediaErrorEvent e)
        {
            try
            // If the Player encounters a corrupt or missing file, 
            // show the hexadecimal error code and URL.
            {
                IWMPMedia2 errSource = e.pMediaObject as IWMPMedia2;
                IWMPErrorItem errorItem = errSource.Error;
                pTraverser.MovieStatusException("Exception DMC");
                // MessageBox.Show("Error Media Player" + errorItem.errorCode.ToString("X")
                //               + " in " + errSource.sourceURL);
                errorDisplay();
            }
            // catch (InvalidCastException)
            catch (Exception ex)
            // In case pMediaObject is not an IWMPMedia item.
            {
                //MessageBox.Show($"Error x Media Error showing error {ex}");
                pTraverser.ErrorDisplay($"Error1 x Media Error showing error {ex}");
                //dmcdmc this.Close();
            }

        }
        public void errorDisplay()
        {
            ERROR err = new ERROR(gv, this, bOnErrContinue);
            err.ShowDialog(this);
            gv.ERROR_STOP = true;
            //  gv.CloseMovies();
        }
        public void resetButtonColor()
        {
        }
        Color pushed = Color.LightBlue;

        private void btBackup_Click(object sender, EventArgs e)
        {
            //position in seconds
            axWindowsMediaPlayer1.settings.rate = 1.0;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition -= 10;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void backup()
        {
            gv.debug.w("movies backup");
            axWindowsMediaPlayer1.settings.rate = 1.0;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition -= 10;
            resetButtonColor();
            //btBack.BackColor = pushed;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void backup3()
        {
            gv.debug.w("movies backup");
            axWindowsMediaPlayer1.settings.rate = 1.0;
            if (axWindowsMediaPlayer1.Ctlcontrols.currentPosition > 30)
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition -= 30;
            resetButtonColor();
            //btBack.BackColor = pushed;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        double fastForwardspeed = 5;
        double fastestForwardspeed = 10;
        double slowMotionspeed = .4;

        private void btFastForward_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.rate = fastForwardspeed;
            gv.debug.w("movies fast forward");
        }
        bool bSentStatusToMain = false;

        public void setFocusOnParent(bool sendFocus)
        {
            cbSetFocusOnParent.Checked = sendFocus; //true
            if (sendFocus)
            {
                //setFocusOnParent();
                //start_timer();
            }
        }
        public void setFocusOnParent()
        {
        }
        public void setFocusOnParent2(bool bForce = false)
        {
            if (pTraverser != null)
            {
                pTraverser.takeFocusIfYouWant(axWindowsMediaPlayer1.Ctlcontrols.currentPosition, bForce);
            }
        }
        public void setFasterDelay(bool bFaster)
        {
            if (bFaster)
                numIntTimerSeconds.Value = 1;
            else
                numIntTimerSeconds.Value = 2;
        }
        /*
           * through overriding the ProcessCmdKey() method of the form. That way, 
            * your key handling logic gets executed no matter what control 
           * has focus at the time of keypress. 
           * Beside that, you even get to choose whether the focused control gets the key 
           * after you processed it (return false) or not (return true).
        *
       * When you call "return true" you are handling the key press
           * */
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) /////// override dmcdmc123
        {
            if (keyData == Keys.F3)
            {
                gotoEnd(6);
                pTraverser.Message1TakeFocus();
               // MessageBox.Show("You pressed the F3 key");
               // return true;
            }
            else if (keyData == Keys.F11)
            {
                gotoEnd(4);
                setFocusOnParent2();
                pTraverser.Message1TakeFocus();
                return true;
            }
            pause();
            return false;
        }
        private void wmPlayer_dmcKeyDown(object sender, EventArgs e)
        {
           // pause();
           pTraverser.Message1TakeFocus();
           // MessageBox.Show($"dmcKeyDown");
            pTraverser.Focus();
        }

        public void setSlowerDelay(bool bSlower, int seconds = 4)
        {
            if (bSlower)
                numIntTimerSeconds.Value = seconds;
            else
                numIntTimerSeconds.Value = 2;
        }
        bool bSetRequestCallback = false;

        public void requestTimerCallToParent(TraverserDialog p1)
        {
            pTraverser = p1;
            //numIntTimerSeconds.Value = numIntTimerSeconds.Value;
            if (!bSetRequestCallback)
                secondsBetweenPositionUpdate = (int)numIntTimerSeconds.Value;
            //this.WindowState = FormWindowState.Maximized;
            //start_timer();
            bSetRequestCallback = true;
        }

        public double getPosition()
        {
            if (axWindowsMediaPlayer1 == null)
                return 0;
            return this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
        }
        public string getState()
        {
            //return currentStateLabel.Text;
            return this.axWindowsMediaPlayer1.playState.ToString();
        }
        public string getTitle()
        {
            return fpath1;
        }
        public string getPreviousTitle()
        {
            return previousFpath;
        }
        public string getPreviousTitle2()
        {
            return previousFpath2;
        }
        public Point getResolution()
        {
            if (axWindowsMediaPlayer1.currentMedia == null)
                return new Point(0,0);
            //IWMPMedia mediaInfo = axWindowsMediaPlayer1.newMedia(tbMoviePath1.Text);
            width = axWindowsMediaPlayer1.currentMedia.imageSourceWidth;
            height = axWindowsMediaPlayer1.currentMedia.imageSourceHeight;

            Point xy = new Point((int)height, (int)width);
            return xy;
        }
        public double getDuration()
        {
            if (axWindowsMediaPlayer1 == null)
                return 0;
            if (axWindowsMediaPlayer1.currentMedia == null)
                return 0;
            return this.axWindowsMediaPlayer1.currentMedia.duration;
        }
        // Form parentDialogTraverser = null;
        TraverserDialog pTraverser = null;

        public void zoomFullScreenFromTimer()
        {
            zoomFullScreen(true);
        }
        Rectangle mySize = new Rectangle();
        public Rectangle GetSize()
        {
            this.WindowState = FormWindowState.Normal;
            Point loc = this.Location;
            mySize.X = loc.X;
            mySize.Y = loc.Y;
            mySize.Width = this.Size.Width;
            mySize.Height = this.Size.Height;
            return mySize;
        }
        public void xxx()
        {
            mySize.X = this.Location.X;
            mySize.Y = this.Location.Y;
            mySize.Width = this.Size.Width;
            mySize.Height = this.Size.Height;
        }
        private void Movies_Resize(object sender, EventArgs e)
        {
        }
        public void SetLocationAndSize(Rectangle myDisplay)
        {
            this.Location = new Point(myDisplay.X, myDisplay.Y);
            //this.Location = myDisplay.Y;
            this.width = myDisplay.Width;
            this.height = myDisplay.Height;
        }
        public void zoomFullScreen(bool bFullScreen = true)
        {
            //cbzoom.Checked = bFullScreen;
            if (!bDidZoomFullScreen && bFullScreen)
            {
                if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    axWindowsMediaPlayer1.fullScreen = bFullScreen;
                    bZoomFullScreenOnTimer = false; //don't need to do this

                    bDidZoomFullScreen = true;
                    gv.debug.w("movies zoom");
                }
            }
            if (!bFullScreen)
            {
                bDidZoomFullScreen = false;
                if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
                    axWindowsMediaPlayer1.fullScreen = false;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
            if (cbSetFocusOnParent.Checked)
            {
                setFocusOnParent();
                gv.debug.w("movies set focus on parent");
            }
            secondsBetweenPositionUpdate = (int)numIntTimerSeconds.Value;
        }

        int secondsBetweenPositionUpdate = 3;
        int waitSeconds = 1;
        int wait1Second = 1;

        void start_timer()
        {
            timer1.Interval = 1000; // specify interval time as you want
            //timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();

        }
        public bool bZoomFullScreenOnTimer = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (secondsBetweenPositionUpdate < 0)
            {
                if (bZoomFullScreenOnTimer)
                    zoomFullScreen(true);

                if (pTraverser != null)
                {
                    pTraverser.movieTimerNotice(this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
                    pTraverser.completedMovieZoomMessage();
                }

                secondsBetweenPositionUpdate = (int)numIntTimerSeconds.Value;
                if (cbSetFocusOnParent.Checked)
                {
                    setFocusOnParent2();
                }
            }
            else
                secondsBetweenPositionUpdate--;
        }


        public void play() // if paused
        {
            try
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            catch (Exception ex)
            {
                gv.dialogTraverser.PlaybackErrorInMovies("PLAY ERR");
                this.Close();
            }
        }
        int myScreen = 0;
        public void moveToNextScreen()
        {
            this.WindowState = FormWindowState.Normal;
            ///MessageBox.Show(gv.displayCount.ToString() );
            gv.debug.w($"{myScreen} of {gv.displayCount}");
            if (gv == null)
                gv.debug.w("{gv is null");
            if (gv.screen == null)
                gv.debug.w("{gv.screen is null");

            if (gv.displayCount > 1)
                ++myScreen;
            if (myScreen >= gv.screen.Length)
                myScreen = 0;
            if (myScreen >= gv.displayCount)
                myScreen = 0;
            if (gv.screen[myScreen] == null)
            {
                gv.debug.w($"gv.screen[{myScreen}] is null");
                return;
            }
            this.Location = new Point(gv.screen[myScreen].Bounds.X, gv.screen[myScreen].Bounds.Y);
        }
        public void fastForward()
        {
            gv.debug.w("movies fast forward");
            axWindowsMediaPlayer1.settings.rate = fastForwardspeed;
            resetButtonColor();
            //btFastForward.BackColor = pushed;
            gv.debug.w("movies fastforward");
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void ScanBackward()
        {
            gv.debug.w("movies backward");
            axWindowsMediaPlayer1.settings.rate = -fastForwardspeed;
            resetButtonColor();
            //btFastForward.BackColor = pushed;
            gv.debug.w("movies fastforward");
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void ScanForward()
        {
            gv.debug.w("movies scan");
            axWindowsMediaPlayer1.settings.rate = fastForwardspeed;
            resetButtonColor();
            //btFastForward.BackColor = pushed;
            gv.debug.w("movies fastforward");
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        private void btFasterForward_Click_1(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.rate = fastestForwardspeed;
        }
        public void fasterForward()
        {
            axWindowsMediaPlayer1.settings.rate = fastestForwardspeed;
            resetButtonColor();
            //btFasterForward.BackColor = pushed;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void playNormalSpeed()
        {
            axWindowsMediaPlayer1.settings.rate = 1.0;
        }
        public void slowMotion(bool slower = false)
        {
            gv.debug.w("movies slow");
            if (slower)
                axWindowsMediaPlayer1.settings.rate = slowMotionspeed / 2;
            else
                axWindowsMediaPlayer1.settings.rate = slowMotionspeed;
            resetButtonColor();
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void gotoEnd(int seconds)
        {
            axWindowsMediaPlayer1.settings.rate = 1;
            double dur = axWindowsMediaPlayer1.currentMedia.duration;
            double pos = dur - seconds;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = pos;
            gv.debug.w("movies view ending");
        }
        private void btSkimForward_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.rate = fastestForwardspeed;
            double dur = axWindowsMediaPlayer1.currentMedia.duration / 20;
            double pos = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            pos += dur;
            if ((pos - axWindowsMediaPlayer1.status.Length) < 0)
                return;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = pos;
            gv.debug.w("movies skim forward");
        }
        bool useEqualIncrements = false;
        public void skipForward(double forwardSeconds = 0)
        {
            if (newState != 3)//play
                return;
            double len = axWindowsMediaPlayer1.currentMedia.duration;
            double ahead = axWindowsMediaPlayer1.currentMedia.duration / 20;
            double pos = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            decimal start = (decimal)pos;

            if (useEqualIncrements)
            {
                ahead = 20;
            }
            if (forwardSeconds > 0 && forwardSeconds < 66)
                ahead = forwardSeconds;
            //ahead = 20;
            if (pos < (axWindowsMediaPlayer1.currentMedia.duration - ahead))
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition += ahead;
            gv.debug.w("movies skipForward");
            axWindowsMediaPlayer1.settings.rate = 1;
            //btBack.BackColor = pushed;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void skipForwardBig()
        {
            if (newState != 3)//play
                return;
            double ahead = axWindowsMediaPlayer1.currentMedia.duration / 10;
            double pos = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;

            if (pos < (axWindowsMediaPlayer1.currentMedia.duration - ahead))
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition += ahead;
            gv.debug.w("movies skipForward BIG");
            axWindowsMediaPlayer1.settings.rate = 1;
            resetButtonColor();
            //btSkimForward.BackColor = pushed;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }

        public void skipForwardPercent(int divideBy)
        {
            if (newState != 3)//play
                return;
            double ahead = axWindowsMediaPlayer1.currentMedia.duration / divideBy;
            ahead = ahead - (ahead / 9);
            double pos = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            double nextPos = pos + ahead;
            if (nextPos > axWindowsMediaPlayer1.currentMedia.duration)
            {
                ahead = ahead / 2;
                nextPos = pos + ahead;
            }
            if (nextPos < axWindowsMediaPlayer1.currentMedia.duration)
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition += ahead;

            gv.debug.w($"movies skipForward Percent");
            axWindowsMediaPlayer1.settings.rate = 1;
            resetButtonColor();
            //btSkimForward.BackColor = pushed;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void skipBack(double seconds = 0)
        {
            double dur = seconds;
            if (seconds <= 0)
                dur = axWindowsMediaPlayer1.currentMedia.duration / 20;
            double pos = axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            pos -= dur;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = pos;

            axWindowsMediaPlayer1.settings.rate = 1;
            //btBack.BackColor = pushed;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void normalSpeed()
        {
            axWindowsMediaPlayer1.settings.rate = 1;
        }
        private void btRestart_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.rate = 1.0;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
        }
        public void restart()
        {
            axWindowsMediaPlayer1.settings.rate = 1.0;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
            resetButtonColor();
            //btRestart.BackColor = pushed;
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void pause()
        {
            if (newState == 2)
                axWindowsMediaPlayer1.Ctlcontrols.play();
            else
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void stop()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer1.URL = "";
            //tbMoviePath1.Text = "";
            resetButtonColor();
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }
        public void SetTitleBar(string txt)
        {
            this.Text = txt;
        }
        private void cbMute_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMute.Checked)
            {
                stop();
                this.Text = "STOP";
                ResetAndPlay();
            }
        }
        public string ResetAndPlay()
        {
            stop();
            string newPath = pTraverser.GetNextMovieFullpath();
            loadMovie(newPath, false);
            string test = axWindowsMediaPlayer1.currentMedia.name;
            this.Text += " // " + test;
            play();
            return newPath;
        }
        public void mute(bool bMute)
        {
            cbMute.Checked = bMute;
            axWindowsMediaPlayer1.settings.mute = bMute;
            axWindowsMediaPlayer1.settings.rate = 1;
            play();
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }

        private void cbSetFocusOnParent_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSetFocusOnParent.Checked)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void Movies_Activated(object sender, EventArgs e)
        {
            if (cbSetFocusOnParent.Checked)
                setFocusOnParent();
        }

        private void axWindowsMediaPlayer1_KeyUpEvent(object sender, AxWMPLib._WMPOCXEvents_KeyUpEvent e)
        {

        }

        private void axWindowsMediaPlayer1_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
        {
            skipForward();
        }
        private void axWindowsMediaPlayer1_KeyPressEvent(object sender, AxWMPLib._WMPOCXEvents_KeyPressEvent e)
        {

            if (e.nKeyAscii == 32)
            {
                //MessageBox.Show("Space Key Pressed ");
            }
            else
                skipForward();
        }

        private void Movies_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                //MessageBox.Show("Space Key Pressed ");
            }
            else
                skipForward();
        }

        private void btGoToMain_Click(object sender, EventArgs e)
        {
            if (pTraverser != null)
                pTraverser.Activate();
        }
        //endstream
        private void AxWindowsMediaPlayer1_EndOfStream(object sender, AxWMPLib._WMPOCXEvents_EndOfStreamEvent e)
        {
            if (pTraverser != null)
                pTraverser.endOfSteamMessage();
        }
        public void sendPlayerState(string txt)
        {
            if (pTraverser != null)
                pTraverser.updatePlayerState(txt);
        }
        int newState = 0;

        private void btSetSize_Click(object sender, EventArgs e)
        {
            if (pTraverser != null)
                pTraverser.movieWindowSize(mySize);
        }

        private void cbzoom_CheckedChanged(object sender, EventArgs e)
        {
            //bZoomFullScreenOnTimer = cbzo
        }

        private void axWindowsMediaPlayer1_NewStream(object sender, EventArgs e)
        {
            if (pTraverser != null)
                pTraverser.startStreamMessage();
        }

        private void Movies_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (pTraverser != null)
                pTraverser.resumeNextPlay();
        }
        public void ResetAndPlay(string movieFpath)
        {
            stop();
            axWindowsMediaPlayer1.URL = "";

            this.Text = movieFpath;
            loadMovie(movieFpath, false);
            play();
        }
        public string GetMovieFPath()
        {
            return fpath1;
        }
        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            newState = e.newState;
            switch (e.newState)
            {
                case 0:    // Undefined
                    currentStateLabel.Text = "Undefined";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 1:    // Stopped
                    currentStateLabel.Text = "Stopped";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 2:    // Paused
                    currentStateLabel.Text = "Paused";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 3:    // Playing
                    currentStateLabel.Text = "Playing";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 4:    // ScanForward
                    currentStateLabel.Text = "ScanForward";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 5:    // ScanReverse
                    currentStateLabel.Text = "ScanReverse";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 6:    // Buffering
                    currentStateLabel.Text = "Buffering";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 7:    // Waiting
                    currentStateLabel.Text = "Waiting";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 8:    // MediaEnded
                    currentStateLabel.Text = "MediaEnded";
                    sendPlayerState(currentStateLabel.Text);
                    if (pTraverser != null)
                        pTraverser.endOfSteamMessage();
                    break;

                case 9:    // Transitioning
                    currentStateLabel.Text = "Transitioning";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 10:   // Ready
                    currentStateLabel.Text = "Ready";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 11:   // Reconnecting
                    currentStateLabel.Text = "Reconnecting";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                case 12:   // Last
                    currentStateLabel.Text = "Last";
                    sendPlayerState(currentStateLabel.Text);
                    break;

                default:
                    currentStateLabel.Text = ("Unknown State: " + e.newState.ToString());
                    sendPlayerState(currentStateLabel.Text);
                    break;
            }//switch
        }//axWindowsMediaPlayer1_PlayStateChange

    }
}
