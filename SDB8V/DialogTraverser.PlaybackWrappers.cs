#nullable disable
using System.Windows.Forms;

namespace SymbolDB
{
    /// <summary>
    /// Public wrapper methods and state properties consumed by
    /// <see cref="PlaybackControl"/>. Kept in a separate partial-class file
    /// so the main DialogTraverser.cs stays untouched.
    /// </summary>
    public partial class DialogTraverser : Form
    {
        // ── Backing fields (replace the removed checkboxes) ────────────────
        private bool _isSlowMode;
        private bool _isSlowMode2;
        private bool _isPaused;

        // ── State properties ───────────────────────────────────────────────

        /// <summary>True when slow-motion (½ speed) is active.</summary>
        public bool IsSlowMode => _isSlowMode;

        /// <summary>True when slower slow-motion (⅕ speed) is active.</summary>
        public bool IsSlowMode2 => _isSlowMode2;

        /// <summary>True when the player is manually paused.</summary>
        public bool IsPaused => _isPaused;

        /// <summary>True when the player is muted.</summary>
        public bool IsMuted => cbMute.Checked;

        /// <summary>True when a 90° rotation has been staged via SetRotation90.</summary>
        public bool IsRotation90 => _rotation90Active;
        private bool _rotation90Active;

        // ── Speed / pause setters ──────────────────────────────────────────

        /// <summary>Enable / disable ½-speed slow mode.</summary>
        public void SetSlow(bool on)
        {
            _isSlowMode = on;
            if (vlcPlayer == null || vlcPlayer.IsDisposed) return;
            if (on)
            {
                sliderSlowMotion.Visible = true;
                sliderSlowMotion.Value = 5;
                vlcPlayer.SetSpeed(0.5f);
            }
            else
            {
                _isSlowMode2 = false;
                vlcPlayer.ResumeNormalSpeed();
                sliderSlowMotion.Visible = false;
            }
        }

        /// <summary>Enable / disable ⅕-speed slow mode.</summary>
        public void SetSlow2(bool on)
        {
            _isSlowMode2 = on;
            if (vlcPlayer == null || vlcPlayer.IsDisposed) return;
            if (on)
            {
                if (!_isSlowMode) SetSlow(true);
                vlcPlayer.SlowMotion(5);
            }
            else
            {
                if (_isSlowMode) vlcPlayer.SlowMotion(10);
                else vlcPlayer.ResumeNormalSpeed();
            }
        }

        /// <summary>Pause or resume.</summary>
        public void SetPause(bool on)
        {
            _isPaused = on;
            if (vlcPlayer == null || vlcPlayer.IsDisposed) return;
            if (on) vlcPlayer.Pause();
            else vlcPlayer.Play();
        }

        // ── Transport wrappers ─────────────────────────────────────────────

        /// <summary>Play the currently selected file (equivalent to clicking btPlay).</summary>
        public void PlaySelected()
        {
            if (!cbLoadedAndReady.Checked) return;
            PlayButtonGo();
            OpenTransparentWin();
        }

        /// <summary>Skip back 5 seconds (fine navigation step).</summary>
        public void SkipBackSmall()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.SkipForward(-5);   // negative = backwards
                getMovieStatus();
            }
        }
        /// <summary>Restart playback from the beginning of the current file.</summary>
        public void RestartPlayback()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.Restart();
        }
        /// <summary>Attempt to play in reverse (note: LibVLC support varies).</summary>
        public void ReversePlayback()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.Reverse();
        }

        /// <summary>Fast-forward at 3× speed.</summary>
        public void FastForwardPlayback()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.FastForward();
        }
        // ── Keystroke routing ──────────────────────────────────────────────────

        /// <summary>
        /// Called by <see cref="PlaybackControl"/> so that hotkeys (catalog copy,
        /// rating, playback transport) work even when focus is on the floating
        /// <see cref="PlaybackControlForm"/> rather than this form.
        /// </summary>
        public bool ForwardCmdKey(ref Message msg, Keys keyData)
            => ProcessCmdKey(ref msg, keyData);
        // ── Mute ──────────────────────────────────────────────────────────

        /// <summary>Toggle mute on/off (mirrors cbMute checkbox).</summary>
        public void ToggleMute() => cbMute.Checked = !cbMute.Checked;

        // ── Window management ─────────────────────────────────────────────

        /// <summary>Move the player window to the next monitor.</summary>
        public void MoveMovieToNextScreen()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.MoveToNextScreen();
        }

        /// <summary>Stop, close, and dispose the player window.</summary>
        public void CloseMovie()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.Close();
                vlcPlayer.Dispose();
                vlcPlayer = null;
            }
        }
        

        public PlaybackControlForm _playbackControlForm;

        /// <summary>
        /// Opens the floating <see cref="PlaybackControlForm"/> panel.
        /// If the form is already open, brings it to the front and syncs its state.
        /// Creates a fresh instance if it has been closed or never opened.
        /// </summary>
        public void ShowPlaybackForm()
        {
            if (_playbackControlForm == null || _playbackControlForm.IsDisposed)
            {
                _playbackControlForm = new PlaybackControlForm(this);
                _playbackControlForm.FormClosed += (s, e) => _playbackControlForm = null;
                _playbackControlForm.Show(this);   // non-modal; owned by DialogTraverser
            }
            else
            {
                // Already open — just bring it forward and refresh state
                if (_playbackControlForm.WindowState == FormWindowState.Minimized)
                    _playbackControlForm.WindowState = FormWindowState.Normal;

                _playbackControlForm.BringToFront();
            }

            _playbackControlForm?.SyncState();
        }
    }

}