using System;
using System.Drawing;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class Playback : Form
    {
        private Player wmPlayer;
        private GlobalVars gv;
        private DialogTraverser parentDialog;
        private string currentFilePath;
        private bool onErrContinueFlag;

        public Playback(GlobalVars g, DialogTraverser parent, bool onErrContinue = false, string fpath = null)
        {
            InitializeComponent();
            gv = g;
            parentDialog = parent;
            onErrContinueFlag = onErrContinue;
            currentFilePath = fpath;

            // Initialize and embed WmPlayer
            wmPlayer = new Player(gv, parentDialog, currentFilePath);
            wmPlayer.TopLevel = false;
            wmPlayer.FormBorderStyle = FormBorderStyle.None;
            wmPlayer.Dock = DockStyle.Fill;
            this.Controls.Add(wmPlayer);
            wmPlayer.Show();
        }

        public void LoadMovie(string fpath, bool zoom = false)
        {
            currentFilePath = fpath;
            wmPlayer.LoadMedia(fpath, true);
        }

        public void Play()
        {
            wmPlayer.Play();
        }

        public void Pause()
        {
            wmPlayer.Pause();
        }

        public void Stop()
        {
            wmPlayer.Stop();
        }

        public void SkipForward(int seconds = 0)
        {
            wmPlayer.SkipForward(seconds);
        }

        public void SkipBack(double seconds = 0)
        {
            wmPlayer.SkipBackwards3();
        }

        public void FastForward()
        {
            wmPlayer.FastForward();
        }

        public void Reverse()
        {
            wmPlayer.Reverse();
        }

        public void ZoomFullScreen(bool fullScreen = true)
        {
            wmPlayer.SetFullScreen(fullScreen);
        }

        public void SetSmallScreen(Rectangle rect)
        {
            wmPlayer.SetSmallScreen(rect.X, rect.Y);
        }

        public double GetPosition()
        {
            return wmPlayer.GetPosition();
        }

        public double GetDuration()
        {
            return wmPlayer.GetDuration();
        }

        public string GetState()
        {
            return wmPlayer.GetState();
        }

        public string GetTitle()
        {
            return wmPlayer.GetTitle();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (wmPlayer != null)
            {
                wmPlayer.Close();
                wmPlayer.Dispose();
            }
            base.OnFormClosed(e);
        }
    }
}
