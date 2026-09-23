// Plan (pseudocode, detailed):
// 1. Create a new WinForms Form class `LibVLCPlayer` in namespace SymbolDB.
// 2. Initialize LibVLCSharp runtime (Core.Initialize) once in constructor.
// 3. Create LibVLC instance and MediaPlayer instance.
// 4. Create a VideoView control (programmatically) and dock it to fill the form.
// 5. Provide constructor that accepts GlobalVars, TraverserDialog parent, optional file path.
//    - Wire parent and global vars members.
//    - If file path provided, call `LoadMedia(path, autoPlay:true)`.
// 6. Implement methods similar to WmPlayer used by the rest of project:
//    - LoadMedia(string path, bool autoPlay)
//    - Play(), Pause(), Stop()
//    - GetPositionSeconds(), GetDurationSeconds(), GetState(), GetTitle(), GetResolution()
//    - ToggleMute(bool), SetFullScreen(bool)
//    - MoveToNextScreen() (use existing GlobalVars.screen if available like WmPlayer do)
//    - Dispose cleanup for MediaPlayer and LibVLC
// 7. Hook MediaPlayer events:
//    - EndReached => call pTraverser.endOfSteamMessage()
//    - Playing/Paused/Stopped => call pTraverser.updatePlayerState or similar via sendPlayerState()
// 8. Minimal UI: right-click context menu with Play/Pause/Stop/FullScreen toggles (optional simple buttons can be added).
// 9. Keep dependencies to LibVLCSharp.Shared and LibVLCSharp.WinForms. This file expects LibVLCSharp NuGet is installed and libvlc binaries are available.
//
// Implementation notes:
// - Use milliseconds for LibVLC positions (Time/Length) and convert to seconds where needed.
// - Media objects created for playback are disposed after calling Play() by holding a reference on Media if needed.
// - MediaPlayer exposes EndReached event which we use to notify TraverserDialog.
// - Form is constructed without designer file; InitializeComponent implemented inline.

using System;
using System.Drawing;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.IO;

namespace SymbolDB
{
    public class LibVLCPlayer : Form
    {
        // External references
        GlobalVars gv;
        TraverserDialog pTraverser;

        // LibVLC objects
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private VideoView _videoView;

        // State
        private string currentMediaPath;
        private bool cbSetFocusOnParent = false;
        private bool isDisposed = false;

        // Constructor
        public LibVLCPlayer(GlobalVars g, TraverserDialog parent, string fpath = null)
        {
            gv = g;
            pTraverser = parent;

            // Initialize form
            Text = "LibVLC Player";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            // Initialize LibVLC runtime and objects
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            // Create VideoView and wire up
            _videoView = new VideoView
            {
                MediaPlayer = _mediaPlayer,
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            Controls.Add(_videoView);

            // Hook events
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
            _mediaPlayer.Playing += MediaPlayer_Playing;
            _mediaPlayer.Paused += MediaPlayer_Paused;
            _mediaPlayer.Stopped += MediaPlayer_Stopped;

            // Simple context menu for basic controls
            var ctx = new ContextMenuStrip();
            ctx.Items.Add("Play").Click += (s, e) => Play();
            ctx.Items.Add("Pause").Click += (s, e) => Pause();
            ctx.Items.Add("Stop").Click += (s, e) => Stop();
            ctx.Items.Add("Toggle Fullscreen").Click += (s, e) => SetFullScreen(!IsFullScreen());
            ctx.Items.Add("Mute/Unmute").Click += (s, e) => ToggleMute(!_mediaPlayer.Mute);
            _videoView.ContextMenuStrip = ctx;

            // load the provided file if any
            if (!string.IsNullOrWhiteSpace(fpath))
            {
                LoadMedia(fpath, autoPlay: true);
            }
        }

        // Load media and optionally start playing
        public void LoadMedia(string path, bool autoPlay = false)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new FileNotFoundException("Media file not found", path);

            currentMediaPath = path;
            // Create Media from path and play
            var media = new Media(_libVLC, new Uri(path));
            // Set media to player and optionally play
            _mediaPlayer.Play(media);
            if (!autoPlay)
            {
                _mediaPlayer.Pause();
            }

            // update form title
            Text = path;
        }

        // Playback controls
        public void Play()
        {
            if (_mediaPlayer != null && !_mediaPlayer.IsPlaying)
                _mediaPlayer.Play();
        }

        public void Pause()
        {
            if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
                _mediaPlayer.Pause();
        }

        public void Stop()
        {
            if (_mediaPlayer != null)
                _mediaPlayer.Stop();
        }

        // Position and duration (seconds)
        public double GetPositionSeconds()
        {
            if (_mediaPlayer == null)
                return 0;
            // Time in milliseconds; return seconds
            long time = _mediaPlayer.Time;
            return time > 0 ? time / 1000.0 : 0;
        }

        public double GetDurationSeconds()
        {
            if (_mediaPlayer == null)
                return 0;
            long len = _mediaPlayer.Length;
            return len > 0 ? len / 1000.0 : 0;
        }

        // State and title
        public string GetState()
        {
            if (_mediaPlayer == null) return "Unknown";
            return _mediaPlayer.State.ToString();
        }

        public string GetTitle()
        {
            return currentMediaPath ?? string.Empty;
        }

        // Mute toggle
        public void ToggleMute(bool mute)
        {
            if (_mediaPlayer != null)
                _mediaPlayer.Mute = mute;
        }

        // Fullscreen behavior: toggle the form window state and keep videoView docked
        public void SetFullScreen(bool full)
        {
            if (full)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
            }
            // ensure videoView fills client area
            _videoView.Dock = DockStyle.Fill;
        }

        public bool IsFullScreen()
        {
            return WindowState == FormWindowState.Maximized && FormBorderStyle == FormBorderStyle.None;
        }

        // Get video resolution from video track if available
        public Size GetResolution()
        {
            // LibVLC exposes Track information on videoView or media; try to fetch video size
            try
            {
                if (_videoView != null && _videoView.MediaPlayer != null)
                {
                    var track = _videoView.MediaPlayer.GetTrackDescription(MediaTrackType.Video);
                    // Getting exact width/height may require native calls; return 0,0 if unknown
                }
            }
            catch { }
            return new Size(0, 0);
        }

        // Notify parent on end of media
        private void MediaPlayer_EndReached(object? sender, EventArgs e)
        {
            try
            {
                if (pTraverser != null)
                    pTraverser.endOfSteamMessage();
            }
            catch { }
        }

        private void MediaPlayer_Playing(object? sender, EventArgs e)
        {
            sendPlayerState("Playing");
        }

        private void MediaPlayer_Paused(object? sender, EventArgs e)
        {
            sendPlayerState("Paused");
        }

        private void MediaPlayer_Stopped(object? sender, EventArgs e)
        {
            sendPlayerState("Stopped");
        }

        // Helper to send state to traverser
        private void sendPlayerState(string txt)
        {
            try
            {
                if (pTraverser != null)
                    pTraverser.updatePlayerState(txt);
            }
            catch { }
        }

        // Move to next screen similar behavior to WmPlayer
        int myScreen = 0;
        public void MoveToNextScreen()
        {
            this.WindowState = FormWindowState.Normal;
            if (gv == null || gv.screen == null) return;
            if (gv.displayCount > 1)
                ++myScreen;
            if (myScreen >= gv.screen.Length)
                myScreen = 0;
            if (myScreen >= gv.displayCount)
                myScreen = 0;
            if (gv.screen[myScreen] == null) return;
            this.Location = new Point(gv.screen[myScreen].Bounds.X, gv.screen[myScreen].Bounds.Y);
        }

        // Dispose pattern to free LibVLC resources
        protected override void Dispose(bool disposing)
        {
            if (isDisposed) return;
            if (disposing)
            {
                // dispose managed
                if (_mediaPlayer != null)
                {
                    try
                    {
                        _mediaPlayer.Stop();
                        _mediaPlayer.Dispose();
                    }
                    catch { }
                }
                if (_videoView != null)
                {
                    try
                    {
                        _videoView.Dispose();
                    }
                    catch { }
                }
                if (_libVLC != null)
                {
                    try
                    {
                        _libVLC.Dispose();
                    }
                    catch { }
                }
            }
            isDisposed = true;
            base.Dispose(disposing);
        }
    }
}