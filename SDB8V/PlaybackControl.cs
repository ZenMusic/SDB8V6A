#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SymbolDB
{
    /// <summary>
    /// Self-contained playback transport UserControl.
    /// Drop onto any form, then call <see cref="AttachToParent"/> once.
    /// All button actions delegate to the attached <see cref="DialogTraverser"/>.
    /// </summary>
    public partial class PlaybackControl : UserControl
    {
        private DialogTraverser _parent;
        // Prevents re-entrancy while SyncFromParent updates checkboxes
        private bool _suppressSync;

        public PlaybackControl()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbTopMost.Checked = true;   // triggers cbTopMost_CheckedChanged → sets f.TopMost
        }
        // ── Keystroke routing ─────────────────────────────────────────────────

        /// <summary>
        /// Routes keystrokes to <see cref="DialogTraverser"/> so that catalog,
        /// rating, and playback hotkeys all work when this floating control has focus.
        /// <para>
        /// F1–F12 are forwarded unconditionally (using the bare key code so that
        /// modifier combinations such as Shift+F9 are matched correctly).
        /// All other keys are forwarded and only consumed if DialogTraverser handles them.
        /// </para>
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_parent == null)
                return base.ProcessCmdKey(ref msg, keyData);

            // Strip Shift / Ctrl / Alt bits so the range check works for
            // combinations like Shift+F9, Ctrl+F1, etc.
            Keys keyCode = keyData & Keys.KeyCode;

            // F1–F12: always forward to DialogTraverser.
            // Covers F9 (search toggle), F11/F12 (special folder), F1 (help), etc.
            if (keyCode >= Keys.F1 && keyCode <= Keys.F12)
                return _parent.ForwardCmdKey(ref msg, keyData);

            // All other keys: forward and let DialogTraverser decide;
            // fall back to default WinForms handling only when not consumed.
            if (_parent.ForwardCmdKey(ref msg, keyData))
                return true;

            return base.ProcessCmdKey(ref msg, keyData);
        }
        // ─── Wiring ──────────────────────────────────────────────────────────────
        /// <summary>TopMost — keeps the hosting form above all other windows.</summary>
        private void cbTopMost_CheckedChanged(object sender, EventArgs e)
        {
            if (FindForm() is Form f)
                f.TopMost = cbTopMost.Checked;
        }
        /// <summary>
        /// Wires every button / checkbox to <paramref name="parent"/>.
        /// Call this once after the control is added to a form.
        /// </summary>
        public void AttachToParent(DialogTraverser parent)
        {
            _parent = parent;
            SyncFromParent();
        }

        /// <summary>
        /// Refreshes checkbox states to match the parent's current playback state.
        /// Call whenever the parent's state changes externally.
        /// </summary>
        public void SyncFromParent()
        {
            if (_parent == null) return;
            _suppressSync = true;
            try
            {
                cbSlow.Checked   = _parent.IsSlowMode;
                cbSlow2.Checked  = _parent.IsSlowMode2;
                cbPause.Checked  = _parent.IsPaused;
                cbR90.Checked    = _parent.IsRotation90;
                btMute.Text      = _parent.IsMuted ? "Unmute" : "Mute";
                btMute.BackColor = _parent.IsMuted ? Color.LightCoral : SystemColors.Control;
            }
            finally { _suppressSync = false; }
        }

        // ─── Row 1 : Zoom / Slow2 / Slow / Pause / Resume ───────────────────────

        private void btZoom_Click(object sender, EventArgs e)
            => _parent?.ZoomMovieFullScreen();

        private void cbSlow2_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressSync || _parent == null) return;
            _parent.SetSlow2(cbSlow2.Checked);
        }

        private void cbSlow_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressSync || _parent == null) return;
            _parent.SetSlow(cbSlow.Checked);
        }

        private void cbPause_CheckedChanged(object sender, EventArgs e)
        {
            if (_suppressSync || _parent == null) return;
            _parent.SetPause(cbPause.Checked);
        }

        private void btResume_Click(object sender, EventArgs e)
            => _parent?.ResumeNormalSpeed();

        // ─── Row 2 : Transport ───────────────────────────────────────────────────

        /// <summary>REV — restart playback from the beginning.</summary>
        private void btREV_Click(object sender, EventArgs e)
            => _parent?.RestartPlayback();

        /// <summary>&lt;&lt; — skip back 30 s.</summary>

        /// <summary>&lt;&lt; — skip back 10 s.</summary>
        private void btSkipBackMed_Click(object sender, EventArgs e)
            => _parent?.SkipBack();

        /// <summary>&lt; — skip back 5 s (fine step).</summary>
        private void btSkipBack_Click(object sender, EventArgs e)
            => _parent?.SkipBackSmall();

        /// <summary>play — play the currently selected file.</summary>
        private void btPlay_Click(object sender, EventArgs e)
            => _parent?.PlaySelected();

        /// <summary>&gt; — skip forward 10 s.</summary>
        private void btSkipFwd_Click(object sender, EventArgs e)
            => _parent?.SkipForward();

        /// <summary>&gt;&gt; — skip forward 1 min.</summary>
        private void btSkipFwd1Min_Click(object sender, EventArgs e)
            => _parent?.SkipForward1min();

        /// <summary>E — go to end − 15 s.</summary>
        private void btEnd_Click(object sender, EventArgs e)
            => _parent?.GoToEnd(15);

        /// <summary>V — go to end − 5 s.</summary>
        private void btGoToEnd_Click(object sender, EventArgs e)
            => _parent?.GoToEnd(5);

        /// <summary>&gt;&gt; — skip forward 2 min.</summary>
        private void btSkipFwd2Min_Click(object sender, EventArgs e)
            => _parent?.SkipForward2min();

        /// <summary>FF — fast-forward at 3× speed.</summary>
        private void btFF_Click(object sender, EventArgs e)
            => _parent?.FastForwardPlayback();

        // ─── Row 3 : R90 / saveR / S! ───────────────────────────────────────────

        

        // ─── Additional : Mute / MoveMovie / CloseMovie ──────────────────────────

        private void btMute_Click(object sender, EventArgs e)
        {
            _parent?.ToggleMute();
            SyncFromParent();   // refresh label / colour
        }

        private void btMoveMovie_Click(object sender, EventArgs e)
            => _parent?.MoveMovieToNextScreen();

        private void btCloseMovie_Click(object sender, EventArgs e)
            => _parent?.CloseMovie();
    }
}