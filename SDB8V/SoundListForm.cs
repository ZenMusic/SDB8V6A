#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class SoundListForm : Form
    {
        private readonly GlobalVars _gv;
        private readonly List<SoundItems> _soundList;
        private readonly AudioPlayer _audio;
        private CancellationTokenSource _playCts;
        private string _currentFilePath;
        AudioPlayer audioPlayer;
        GlobalVars gv;
        public SoundListForm(GlobalVars g, List<SoundItems> soundList)
        {
            InitializeComponent();
            gv = g;
            audioPlayer = gv.mainWindow.audio;
            numVolume.Value = gv.soundVolume;
            _soundList = soundList ?? new List<SoundItems>();
            _audio.Volume = gv.soundVolume;

            // Wire up EndReached so Play All advances to the next track
            _audio.PlaybackEndReached += Audio_PlaybackEndReached;

            PopulateList();
        }

        // ---------------------------------------------------------------
        // List population
        // ---------------------------------------------------------------
        private void PopulateList()
        {
            lstSounds.Items.Clear();
            foreach (var si in _soundList)
                lstSounds.Items.Add(si.fname);
        }

        // ---------------------------------------------------------------
        // Single-click to play
        // ---------------------------------------------------------------
        private void lstSounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lstSounds.SelectedIndex;
            if (idx < 0 || idx >= _soundList.Count) return;

            StopPlayAll();
            PlaySound(_soundList[idx]);
        }

        // ---------------------------------------------------------------
        // Play All
        // ---------------------------------------------------------------
        private async void btnPlayAll_Click(object sender, EventArgs e)
        {
            StopPlayAll();

            _playCts = new CancellationTokenSource();
            btnPlayAll.Enabled = false;
            btnStop.Enabled = true;

            try
            {
                await PlayAllAsync(_playCts.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                SetButtonsIdle();
            }
        }

        private async Task PlayAllAsync(CancellationToken ct)
        {
            for (int i = 0; i < _soundList.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                var si = _soundList[i];

                // Highlight the current row
                lstSounds.SelectedIndex = i;
                if (i >= 0 && i < lstSounds.Items.Count)
                {
                    lstSounds.TopIndex = i;
                }

                PlaySound(si);

                // Wait roughly for the sound to finish; poll until not playing or timed-out
                const int pollMs = 200;
                const int maxWaitMs = 10_000; // 10 s safety cap per sound
                int waited = 0;

                // Give LibVLC a moment to start
                await Task.Delay(300, ct);

                while (_audio.IsPlaying && waited < maxWaitMs)
                {
                    ct.ThrowIfCancellationRequested();
                    await Task.Delay(pollMs, ct);
                    waited += pollMs;
                }

                // Short gap between sounds
                await Task.Delay(400, ct);
            }

            lblCurrentFile.Text = "Done.";
        }

        // ---------------------------------------------------------------
        // Stop button
        // ---------------------------------------------------------------
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopPlayAll();
            _audio.Stop();
            lblCurrentFile.Text = "Stopped.";
            SetButtonsIdle();
        }
        // ---------------------------------------------------------------
        // Copy Path
        // ---------------------------------------------------------------
        private void btnCopyPath_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath)) return;

            try
            {
                Clipboard.SetText(_currentFilePath);

                // Brief visual confirmation on the button
                string original = btnCopyPath.Text;
                btnCopyPath.Text = "✔  Copied!";
                var restoreTimer = new System.Windows.Forms.Timer { Interval = 1200 };
                restoreTimer.Tick += (s, _) =>
                {
                    btnCopyPath.Text = original;
                    restoreTimer.Stop();
                    restoreTimer.Dispose();
                };
                restoreTimer.Start();
            }
            catch (Exception ex)
            {
                _gv?.debug?.w("SoundListForm.CopyPath error: ", ex.Message);
            }
        }
        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------
        private void PlaySound(SoundItems si)
        {
            try
            {
                _currentFilePath = si.fpath;

                string display = si.fpath;
                // Update label on UI thread

                if (lblCurrentFile.InvokeRequired)
                    lblCurrentFile.Invoke(() => lblCurrentFile.Text = display);
                else
                    lblCurrentFile.Text = display;

                _audio.Stop();
                _audio.Load(si.fpath, autoPlay: true);
            }
            catch (Exception ex)
            {
                _gv?.debug?.w("SoundListForm.PlaySound error: ", ex.Message);
            }
        }

        private void StopPlayAll()
        {
            if (_playCts != null && !_playCts.IsCancellationRequested)
            {
                _playCts.Cancel();
                _playCts.Dispose();
                _playCts = null;
            }
        }

        private void SetButtonsIdle()
        {
            if (btnPlayAll.InvokeRequired)
            {
                btnPlayAll.Invoke(SetButtonsIdle);
                return;
            }
            btnPlayAll.Enabled = true;
            btnStop.Enabled = false;
        }

        private void Audio_PlaybackEndReached(object sender, EventArgs e)
        {
            // Nothing extra needed – PlayAllAsync polls IsPlaying
        }

        // ---------------------------------------------------------------
        // Form closing – clean up AudioPlayer
        // ---------------------------------------------------------------
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopPlayAll();
            _audio.Stop();
            _audio.PlaybackEndReached -= Audio_PlaybackEndReached;
            _audio.Dispose();
            base.OnFormClosing(e);
        }

        private void numVolume_ValueChanged(object sender, EventArgs e)
        {
            gv.mainWindow.audio.Volume = (int)numVolume.Value;

        }
    }
}