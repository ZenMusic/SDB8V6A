// LibVLC 3.9+ with LibVLCSharp 3.6+
// Plan (pseudocode, detailed):
// 1. Add a System.Windows.Forms.Timer field `_posTimer` to the Player class.
// 2. Initialize `_posTimer` in the constructor with a sensible interval (e.g. 1000ms).
// 3. On each Tick:
//    - Read current playback position (seconds) via GetPositionSeconds().
//    - Optionally update local playback info via GetPlaybackInfo().
//    - Call `pTraverser.movieTimerNotice(currentPosition)` if pTraverser is not null.
//    - Surround with try/catch to avoid exceptions bubbling up.
// 4. Start `_posTimer` when playback starts (on Play() call and in MediaPlayer_Playing event).
// 5. Stop `_posTimer` when paused or stopped (in Pause(), Stop(), and corresponding events).
// 6. Dispose `_posTimer` in Dispose(bool) to free resources.
// 7. Keep existing behavior otherwise unchanged.
//
// This approach uses System.Windows.Forms.Timer so Tick executes on the UI thread of the Player form,
// avoiding cross-thread UI issues when calling pTraverser (a Form). Interval can be adjusted as needed.

using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SymbolDB.WindowsThumbnailProvider;

namespace SymbolDB
{
    public class Player : Form
    {
        // External references
        GlobalVars gv;
        DialogTraverser pTraverser;

        // LibVLC objects
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private VideoView _videoView;

        public MediaPlayer MediaPlayer => _mediaPlayer;

        // State
        private string currentMediaPath;
        private bool cbSetFocusOnParent = false;
        private bool isDisposed = false;

        // Context menu
        private ContextMenuStrip _ctx;

        // Timer to report playback position
        private System.Windows.Forms.Timer _posTimer;
        SearchForMediaByName basicForm;
        public void SetRotation(string transformType) => _videoTransform = transformType;
        public void ClearRotation() => _videoTransform = null;
        
        private string _videoTransform = null;

        // State
        // Constructor
        public Player(GlobalVars g, DialogTraverser parent, string fpath = null, SearchForMediaByName form = null)
        {
            gv = g;
            pTraverser = parent;
            if (pTraverser == null)
            {
                if (form != null)
                    basicForm = form;
            }
            // Initialize form
            Text = "LibVLC Player";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.WindowsDefaultLocation;


            // 🔹 Make sure the form is resizable and shows resize controls
            FormBorderStyle = FormBorderStyle.Sizable;      // allows resizing
            MaximizeBox = true;                             // show/enable Maximize button
            MinimizeBox = true;                             // show/enable Minimize button
            ControlBox = true;                             // show title bar + system buttons
            SizeGripStyle = SizeGripStyle.Show;             // optional: show bottom-right resize grip


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
            _mediaPlayer.Opening += MediaPlayer_Opening;

            // Simple context menu for basic controls
            _ctx = new ContextMenuStrip();
            _ctx.Items.Add("Play").Click += (s, e) => Play();
            _ctx.Items.Add("Pause").Click += (s, e) => Pause();
            _ctx.Items.Add("Stop").Click += (s, e) => Stop();
            _ctx.Items.Add("Toggle Fullscreen").Click += (s, e) => SetFullScreen(!IsFullScreen());
            _ctx.Items.Add("Mute/Unmute").Click += (s, e) => ToggleMute(!_mediaPlayer.Mute);

            // Assign context menu to VideoView
            _videoView.ContextMenuStrip = _ctx;

            // Ensure context menu is shown even if the VideoView render path doesn't raise mouse events:
            // 1) Try to show on VideoView.MouseDown (typical case).
            _videoView.MouseDown += (s, e) =>
            {
                try
                {
                    if (e.Button == MouseButtons.Right)
                        _ctx.Show(_videoView, e.Location);
                }
                catch { }
            };

            // 2) Fallback: if VideoView doesn't receive mouse events, the Form may receive them.
            //    When right-click occurs over the VideoView area, show the menu at the correct position.
            this.MouseDown += (s, e) =>
            {
                try
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        // Client point on the form where the click happened
                        var clientPoint = this.PointToClient(Cursor.Position);
                        if (_videoView.Bounds.Contains(clientPoint))
                        {
                            // Convert to VideoView client coordinates and show menu
                            var viewPoint = _videoView.PointToClient(Cursor.Position);
                            _ctx.Show(_videoView, viewPoint);
                        }
                    }
                }
                catch { }
            };

            // Extra handler for mouse click on VideoView
            _videoView.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                    _ctx.Show(_videoView, e.Location);
            };

            // Timer to notify parent of playback position
            _posTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // milliseconds; adjust as needed
            };
            _posTimer.Tick += PosTimer_Tick;

            // load the provided file if any
            if (!string.IsNullOrWhiteSpace(fpath))
            {
                LoadMedia(fpath, autoPlay: true);
            }

            // Add subscriptions in constructor (place near other mouse handlers)
            // (If constructor already exists, insert these two lines after existing mouse handler wiring)
            // Example insertion:
            // _videoView.MouseDoubleClick += VideoView_MouseDoubleClick;
            // this.MouseDoubleClick += Player_MouseDoubleClick;

            // Double-click handlers and centering helper
            _videoView.MouseDoubleClick += VideoView_MouseDoubleClick;
            this.MouseDoubleClick += Player_MouseDoubleClick;
        }
        int numIntTimerSeconds = 1;
        public void SetSlowerTimer()
        {
            _posTimer.Interval = 3000;
            numIntTimerSeconds = 3;
        }
        public void SetFasterTimer()
        {
            _posTimer.Interval = 1000;
            numIntTimerSeconds = 1;
        }
        public void Reset()
        {
                       Stop();
            currentMediaPath = null;
            Text = "LibVLC Player";

        }
        /// <summary>
        /// Stops playback, reinitialises LibVLC with (or without) the rotation filter,
        /// reloads the current file, and seeks back to where it was.
        /// Call this after SetRotation() or ClearRotation().
        /// </summary>
        public void ApplyRotationAndResume()
        {
            string path = currentMediaPath;
            double resumeSec = GetPositionSeconds();

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return;

            // 1. Tear down existing player/libvlc cleanly
            try { _posTimer?.Stop(); } catch { }
            try { _mediaPlayer?.Stop(); } catch { }

            // Unhook events before dispose to avoid callbacks on dead objects
            try
            {
                _mediaPlayer.EndReached -= MediaPlayer_EndReached;
                _mediaPlayer.Playing -= MediaPlayer_Playing;
                _mediaPlayer.Paused -= MediaPlayer_Paused;
                _mediaPlayer.Stopped -= MediaPlayer_Stopped;
                _mediaPlayer.Opening -= MediaPlayer_Opening;
            }
            catch { }

            try { _mediaPlayer?.Dispose(); } catch { }
            try { _libVLC?.Dispose(); } catch { }

            // 2. Recreate LibVLC — with or without rotation
            if (!string.IsNullOrEmpty(_videoTransform))
                _libVLC = new LibVLC("--video-filter=transform",
                                     $"--transform-type={_videoTransform}");
            else
                _libVLC = new LibVLC();

            // 3. Recreate MediaPlayer and rewire events
            _mediaPlayer = new MediaPlayer(_libVLC);
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
            _mediaPlayer.Playing += MediaPlayer_Playing;
            _mediaPlayer.Paused += MediaPlayer_Paused;
            _mediaPlayer.Stopped += MediaPlayer_Stopped;
            _mediaPlayer.Opening += MediaPlayer_Opening;

            // Also rewire VideoView to new player
            if (_videoView != null)
                _videoView.MediaPlayer = _mediaPlayer;

            // 4. Load and play
            var media = new Media(_libVLC, new Uri(path));
            _mediaPlayer.Media = media;
            _mediaPlayer.Play();

            // 5. Seek back to saved position (needs a short delay for media to open)
            if (resumeSec > 1)
            {
                System.Threading.Tasks.Task.Run(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(600); // wait for media to open
                    this.Invoke(() =>
                    {
                        try { _mediaPlayer.Time = (long)(resumeSec * 1000); } catch { }
                        try { _posTimer?.Start(); } catch { }
                    });
                });
            }
            else
            {
                try { _posTimer?.Start(); } catch { }
            }
        }
        
        public void SetVolume(int volume)  // 0 = mute, 100 = full
        {
            if (_mediaPlayer != null)
                _mediaPlayer.Volume = Math.Clamp(volume, 0, 100);
        }

        public int GetVolume()
        {
            return _mediaPlayer?.Volume ?? 0;
        }
        public void DisplayResizeControls()
        {
            // 🔹 Make sure the form is resizable and shows resize controls
            FormBorderStyle = FormBorderStyle.Sizable;      // allows resizing
            MaximizeBox = true;                             // show/enable Maximize button
            MinimizeBox = true;                             // show/enable Minimize button
            ControlBox = true;                             // show title bar + system buttons
            SizeGripStyle = SizeGripStyle.Show;             // optional: show bottom-right resize grip
        }
        public double GetPosition()
        {
            return position;
        }   
        // Timer tick handler: send current position to traverser
        private void PosTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                // Update local cached playback info
                GetPlaybackInfo();

                if (position > 2)
                {
                    // Once past 10 seconds, get video size if not already done
                    if (videoWidth == 0 && videoHeight == 0)
                    {
                        // "0" is the video track index (usually 0)
                        _mediaPlayer.Size(0, ref videoWidth, ref videoHeight);
                    }
                }

                // If traverser wants notification, send current position (seconds)
                // Using GetPositionSeconds() which returns double (seconds)
                if (pTraverser != null)
                {
                    // Expecting traverser to have movieTimerNotice(double position) or compatible signature
                    if (pTraverser != null)
                        pTraverser.movieTimerNotice(GetPositionSeconds());
                }
            }
            catch { }
        }

       
        public string GetMovieFPath()
        {
            return currentMediaPath;
        }
        public void HidePlayer()
        {
            this.Hide();
        }
        public void UnHidePlayer()
        {
            this.Show();

        }
        // Playback controls
        public void Play()
        {
            if (_mediaPlayer != null && !_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Play();
                GetPlaybackInfo();
                try { _posTimer?.Start(); } catch { }
            }
        }

        public void Pause()
        {
            if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
                try { _posTimer?.Stop(); } catch { }
            }
        }
        public void ResumeNormalSpeed()
        {
            if (_mediaPlayer != null)
                _mediaPlayer.SetRate(1.0f);   // Back to normal speed
        }
        public void FastForward()
        {
            _mediaPlayer.SetRate(3.0f);
        }
        public void ScanForward()
        {
            if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
                _mediaPlayer.SetRate(2.0f);
        }
        public void SkipForwards()
        {
            // Skip forward 10 seconds
            _mediaPlayer.Time += 10_000;
        }
        public void SkipForward(int seconds)
        {
            // Skip forward specified seconds
            _mediaPlayer.Time += seconds * 1000;
        }
        public void SkipForwardBig(int seconds = 60)
        {
            // Skip forward specified seconds
            _mediaPlayer.Time += seconds * 1000;
        }
        const int stepMs = 500; // half a second
        public void SkipBackward()
        {
            var newTime = Math.Max(0, _mediaPlayer.Time - stepMs);
            _mediaPlayer.Time = newTime;
        }
        public void SkipBackwards()
        {
            // Skip back 10 seconds (clamped to 0)
            _mediaPlayer.Time = Math.Max(0, _mediaPlayer.Time - 10_000);
        }
        public void SkipBackwards3()
        {
            // Skip back 30 seconds (clamped to 0)
            _mediaPlayer.Time = Math.Max(0, _mediaPlayer.Time - 30_000);
        }
        public void Stop()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
                try { _posTimer?.Stop(); } catch { }
            }
        }
        public void SeekTo(TimeSpan position)
        {
            _mediaPlayer.Time = (long)position.TotalMilliseconds;
        }
        public void Restart()
        {
            _mediaPlayer.SeekTo(TimeSpan.Zero);
        }
        public void gotoEnd(int seconds)
        {
            double pos = duration - seconds;
            SeekTo(TimeSpan.FromSeconds(pos));
        }
        public void SetSpeed(float speed)
        {
            _mediaPlayer.SetRate(speed);
        }
        public void SetNormalSpeed()
        {
            _mediaPlayer.SetRate(1.0f);
        }
        public void Reverse()
        {
           // _mediaPlayer.SetRate(-1.0f);
        }
        public void SlowMotion(int slowFactor)
        {
            float rate = 1.0f / slowFactor;
            _mediaPlayer.SetRate(rate);
        }
        public void SetSlowerDelay(bool slowFactor, int durationOfSampleSeconds)
        {
            float rate = 0.5f;
            _mediaPlayer.SetRate(rate);
        }
        public void SetSlowDelay(int slowFactor, int durationOfSampleSeconds)
        {
            float rate = 1.0f / slowFactor;
            _mediaPlayer.SetRate(rate);
        }
        // usage

        bool bCallParent = false;
        public void SetCallParent(bool flag)
        {
            bCallParent = flag;
        }

        public async void ConvertFirstWebp(string path = null)
{
    if (path == null) path = firstWebpPath;
    if (string.IsNullOrEmpty(path)) return;

    string ext = Path.GetExtension(path).TrimStart('.');
    if (!string.Equals(ext, "webp", StringComparison.OrdinalIgnoreCase)) return;

    var (bitmap, savedPngPath) = await LoadWebPAsBitmapAsync(path);
    if (bitmap != null)
    {
        bitmap.Dispose();
        if (!string.IsNullOrEmpty(savedPngPath))
            pTraverser?.OnWebpConverted(path, savedPngPath);
    }
}
        

        string firstWebpPath = null;
        bool firstPlayed = false;
        // Load media and optionally start playing
        public bool LoadMedia(string path, bool autoPlay = false)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new FileNotFoundException("Media file not found", path);

            if (gv.webpConverionMode)
            {
                if (firstPlayed)
                {
                    string ext = Path.GetExtension(path).TrimStart('.');
                    if (string.Equals(ext, "webp", StringComparison.OrdinalIgnoreCase))
                    {
                        _ = Task.Run(async () =>
                        {
                            var (bitmap, savedPngPath) = await LoadWebPAsBitmapAsync(path);
                            if (bitmap != null)
                            {
                                this.Invoke(() =>
                                {
                                    bitmap.Dispose();
                                    if (!string.IsNullOrEmpty(savedPngPath))
                                        pTraverser?.OnWebpConverted(path, savedPngPath);
                                });
                            }
                        });
                        return true;
                    }
                }
                else
                {
                    firstWebpPath = path;
                }
            }
            firstPlayed = true;

            try
            {
                if (_mediaPlayer != null)
                {
                    // ✅ Stop on a background thread — never call Stop() on the UI thread.
                    // LibVLC's Stop() is synchronous and waits for internal threads to drain,
                    // which can try to post completion events back to the UI thread → deadlock.
                    if (_mediaPlayer.IsPlaying)
                    {
                        Task.Run(() =>
                        {
                            try { _mediaPlayer.Stop(); } catch { }
                        }).Wait(TimeSpan.FromSeconds(3)); // cap wait; don't hang forever
                    }

                    var oldMedia = _mediaPlayer.Media;
                    if (oldMedia != null)
                    {
                        _mediaPlayer.Media = null;
                        oldMedia.Dispose();
                    }
                }

                currentMediaPath = path;
                var media = CreateMediaWithTransform(path);

                if (!string.IsNullOrEmpty(_videoTransform))
                {
                    media.AddOption(":video-filter=transform");
                    media.AddOption($":transform-type={_videoTransform}");
                    media.AddOption("--video-filter=transform");
                    media.AddOption($"--transform-type={_videoTransform}");
                }

                _mediaPlayer.Media = media;

                if (autoPlay)
                    _mediaPlayer.Play();

                GetPlaybackInfo();
                Text = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading media: {ex.Message}\nPath: {path}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentMediaPath = null;
            }
            return false;
        }
        // Call this instead of LoadMedia when you need rotation applied
        // Re-initialises _libVLC with the transform baked in
        private Media CreateMediaWithTransform(string path)
        {
            var media = new Media(_libVLC, new Uri(path));

            if (!string.IsNullOrEmpty(_videoTransform))
            {
                // These are the correct LibVLC 3.x option strings
                media.AddOption($":video-filter=transform");
                media.AddOption($":transform-type={_videoTransform}");
            }

            return media;
        }
        //2026 april: add
        /// <summary>
        /// Loads a WebP (or any LibVLC-supported image format) into a Bitmap
        /// by opening it as a 1-frame media, seeking to position 0, and taking a snapshot.
        /// </summary>
        /// <param name="filePath">Full path to the .webp file</param>
        /// <returns>Bitmap decoded by L
        public Bitmap LoadWebPAsBitmap(string filePath) //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< WEBP SUPPORT
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return null;

            string targetFolder2;
            string tempSnapshot;
            string targetFolder = gv.initParm1List[0].targetDir1;

            if (Directory.Exists(targetFolder))
            {
                targetFolder2 = Path.Combine(targetFolder, "Temp");
                if (!Directory.Exists(targetFolder2))
                    Directory.CreateDirectory(targetFolder2);

                if (gv.webpConverionMode)
                {
                    // Use the original filename (with .png extension) as the output name
                    string baseName = Path.GetFileNameWithoutExtension(filePath) + ".png";
                    string candidate = Path.Combine(targetFolder2, baseName);

                    // If it already exists, fall back to a datetime-stamped name
                    if (File.Exists(candidate))
                    {
                        string dtName = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".png";
                        candidate = Path.Combine(targetFolder2, dtName);
                    }

                    tempSnapshot = candidate;
                }
                else
                {
                    tempSnapshot = Path.Combine(targetFolder2, $"webp_snap_{Guid.NewGuid():N}.png");
                }
            }
            else
            {
                tempSnapshot = Path.Combine(Path.GetTempPath(), $"webp_snap_{Guid.NewGuid():N}.png");
            }

            // ⚠️ This method must NOT be called on the UI thread — use LoadWebPAsBitmapAsync instead.
            // All Thread.Sleep calls here are safe only when running on a background thread.
            try
            {
                using var snapLibVLC = new LibVLC("--vout=dummy", "--no-audio");
                using var media = new Media(snapLibVLC, new Uri(filePath));
                using var snapPlayer = new MediaPlayer(snapLibVLC) { Media = media };

                snapPlayer.Play();

                // Poll up to 2 seconds for the player to reach Playing state
                // instead of a hard sleep, to avoid unnecessary delays and UI hangs
                for (int i = 0; i < 20; i++)
                {
                    System.Threading.Thread.Sleep(100);
                    if (snapPlayer.State == VLCState.Playing) break;
                }

                snapPlayer.Pause();
                System.Threading.Thread.Sleep(200); // let the frame settle before snapshot

                bool ok = snapPlayer.TakeSnapshot(0, tempSnapshot, 0, 0);

                // Stop asynchronously to avoid blocking if LibVLC tries to marshal
                // events back to a thread that is already waiting (deadlock scenario)
                Task.Run(() =>
                {
                    try { snapPlayer.Stop(); } catch { }
                }).Wait(TimeSpan.FromSeconds(3)); // cap the wait to avoid hanging forever

                if (ok && File.Exists(tempSnapshot))
                {
                    using var fs = new FileStream(tempSnapshot, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var tmp = Image.FromStream(fs);
                    return new Bitmap(tmp);
                }

                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                // ✅ Only delete the temp snapshot if we are NOT in webp conversion mode.
                // In conversion mode the PNG is the desired output — keep it.
                // In normal playback mode it is a true temp file and must be cleaned up.
                if (!gv.webpConverionMode)
                {
                    try { if (File.Exists(tempSnapshot)) File.Delete(tempSnapshot); } catch { }
                }
            }
        }

/// <summary>
/// Async wrapper — always call this from the UI thread instead of LoadWebPAsBitmap directly.
/// Runs the blocking LibVLC snapshot work on a background thread to prevent UI hangs.
/// </summary>
/// <summary>
/// Loads a WebP file via LibVLC snapshot and saves it as PNG.
/// Returns the bitmap and, when in conversion mode, the path where the PNG was saved.
/// </summary>
public (Bitmap Bitmap, string SavedPngPath) LoadWebPAsBitmapxx(string filePath) //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< WEBP SUPPORT
{
    if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        return (null, null);

    string targetFolder2;
    string tempSnapshot;
    string targetFolder = gv.initParm1List[0].targetDir1;

    if (Directory.Exists(targetFolder))
    {
        targetFolder2 = Path.Combine(targetFolder, "Temp");
        if (!Directory.Exists(targetFolder2))
            Directory.CreateDirectory(targetFolder2);

        if (gv.webpConverionMode)
        {
            // Use the original filename (with .png extension) as the output name
            string baseName = Path.GetFileNameWithoutExtension(filePath) + ".png";
            string candidate = Path.Combine(targetFolder2, baseName);

            // If it already exists, fall back to a datetime-stamped name
            if (File.Exists(candidate))
            {
                string dtName = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".png";
                candidate = Path.Combine(targetFolder2, dtName);
            }

            tempSnapshot = candidate;
        }
        else
        {
            tempSnapshot = Path.Combine(targetFolder2, $"webp_snap_{Guid.NewGuid():N}.png");
        }
    }
    else
    {
        tempSnapshot = Path.Combine(Path.GetTempPath(), $"webp_snap_{Guid.NewGuid():N}.png");
    }

    // ⚠️ Must NOT be called on the UI thread — use LoadWebPAsBitmapAsync instead.
    try
    {
        using var snapLibVLC = new LibVLC("--vout=dummy", "--no-audio");
        using var media = new Media(snapLibVLC, new Uri(filePath));
        using var snapPlayer = new MediaPlayer(snapLibVLC) { Media = media };

        snapPlayer.Play();

        // Poll up to 2 seconds for Playing state
        for (int i = 0; i < 20; i++)
        {
            System.Threading.Thread.Sleep(100);
            if (snapPlayer.State == VLCState.Playing) break;
        }

        snapPlayer.Pause();
        System.Threading.Thread.Sleep(200); // let frame settle

        bool ok = snapPlayer.TakeSnapshot(0, tempSnapshot, 0, 0);

        Task.Run(() => { try { snapPlayer.Stop(); } catch { } }).Wait(TimeSpan.FromSeconds(3));

        if (ok && File.Exists(tempSnapshot))
        {
            using var fs = new FileStream(tempSnapshot, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var tmp = Image.FromStream(fs);
            // Return bitmap AND the path where the PNG was saved (in conversion mode)
            string savedPath = gv.webpConverionMode ? tempSnapshot : null;
            return (new Bitmap(tmp), savedPath);
        }

        return (null, null);
    }
    catch
    {
        return (null, null);
    }
    finally
    {
        // Keep the file in conversion mode — it IS the output.
        // Delete it in normal playback mode (it's a true temp file).
        if (!gv.webpConversionMode)
        {
            try { if (File.Exists(tempSnapshot)) File.Delete(tempSnapshot); } catch { }
        }
    }
}

/// <summary>
/// Async wrapper — always call this from the UI thread.
/// </summary>
/// <summary>
/// Async wrapper — always call this from the UI thread.
/// </summary>
public Task<(Bitmap, string)> LoadWebPAsBitmapAsync(string filePath)
    => Task.Run(() => LoadWebPAsBitmapxx(filePath));

        public void SetFocusOnParent()
        {
            cbSetFocusOnParent = true;
        }
        public void SetFocusOnParent(bool focus)
        {
            cbSetFocusOnParent = focus;
        }
        //SeekTo(TimeSpan.FromMinutes(1.5)); // 1:30

        // Position and duration (seconds) <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<  GET STATUS and State
        //
        public double duration = 0;
        public double position = 0;
        public int speed = 0;

        public double GetPlaybackInfo()
        {
            duration = GetDurationSeconds();
            position = GetPositionSeconds();
            speed = (int)_mediaPlayer.Rate;
            return duration;
        }
        public double GetDuration()
        {
            duration = GetDurationSeconds();
            return duration;
        }
        public Point GetResolution()
        {
            Point size = new Point(0, 0);
            size.X = (int)videoWidth;
            size.Y = (int)videoHeight;
            return size;
        }
        //
        //
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
        public void MinimizeMovieWin(bool hide)
        {
            if (hide)
                this.WindowState = FormWindowState.Minimized;
            else
                this.WindowState = FormWindowState.Normal;

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
        public void Mute()
        {
            if (_mediaPlayer != null)
                _mediaPlayer.Mute = true;
        }
        // Fullscreen behavior: toggle the form window state and keep videoView docked //////////////////////////////////////<<<<<<<<<<<<<< scren 
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
        public void setFocusOnParent(bool sendFocus)
        {
            cbSetFocusOnParent = sendFocus; //true
            if (sendFocus)
            {
                //setFocusOnParent();
                //start_timer();
            }
        }
        public void setFocusOnParent()
        {
        }
        public void SetSmallScreen(int width = 300, int height = 200)
        {
            this.WindowState = FormWindowState.Normal;
            this.Width = width;
            this.Height = height;
            DisplayResizeControls();
        }
        public bool IsFullScreen()
        {
            return WindowState == FormWindowState.Maximized && FormBorderStyle == FormBorderStyle.None;
        }
        bool bSetRequestCallback = false;
        int secondsBetweenPositionUpdate = 3;
        int waitSeconds = 1;
        int wait1Second = 1;
        public void requestTimerCallToParent(DialogTraverser p1)
        {
            if (pTraverser == null)
            {
                pTraverser = p1;
                //numIntTimerSeconds = numIntTimerSeconds;
                if (!bSetRequestCallback)
                    secondsBetweenPositionUpdate = (int)numIntTimerSeconds;
                //this.WindowState = FormWindowState.Maximized;
                //start_timer();
                bSetRequestCallback = true;
            }
        }
        // Get video resolution from video track if available
        public Size GetResolutionDetails()
        {
            // LibVLC exposes Track information on videoView or media; try to fetch video size
            try
            {
                if (_videoView != null && _videoView.MediaPlayer != null)
                {
                   //dmc99 var track = _videoView.MediaPlayer.GetTrackDescription(MediaTrackType.Video);
                    // Getting exact width/height may require native calls; return 0,0 if unknown
                }
            }
            catch { }
            return new Size(0, 0);
        }
        bool funnelThruStatusMessage = true;
        // Notify parent on end of media  //////////////////////////////////////////////////////// state notifications
        private void MediaPlayer_EndReached(object? sender, EventArgs e)
        {
            if (funnelThruStatusMessage)
            {
                if (pTraverser != null)
                    pTraverser.SetStatusMessage("ENDING");
                return;
            }
            try
            {
                if (pTraverser != null)
                    pTraverser.endOfSteamMessage();
            }
            catch { }
        }
        private void MediaPlayer_Paused(object? sender, EventArgs e)
        {
            if (funnelThruStatusMessage)
            {
                if (pTraverser != null)
                    pTraverser.SetStatusMessage("Paused");
                return;
            }
            try { _posTimer?.Stop(); } catch { }
            sendPlayerState("Paused");
        }
        /// <summary>
        /// /////////////////////////////////////////////////
        uint videoWidth = 0;
        uint videoHeight = 0;

        private void MediaPlayer_Playing(object? sender, EventArgs e)
        {
            if (pTraverser != null)
            {
                if (funnelThruStatusMessage)
                {
                    pTraverser.SetStatusMessage("Playing");
                    return;
                }
                try { _posTimer?.Start(); } catch { }
                sendPlayerState("Playing");
            }
        }
        private void MediaPlayer_Opening(object? sender, EventArgs e)
        {
            if (funnelThruStatusMessage)
            {
                if (pTraverser != null)
                    pTraverser.SetStatusMessage("Opening");
                return;
            }
            try { _posTimer?.Stop(); } catch { }
            sendPlayerState("Opening");
        }

        private void MediaPlayer_Stopped(object? sender, EventArgs e)
        {
            if (funnelThruStatusMessage)
            {
                if (pTraverser != null)
                    pTraverser.SetStatusMessage("Stopped");
                return;
            }
            try { _posTimer?.Stop(); } catch { }
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
                if (_posTimer != null)
                {
                    try
                    {
                        _posTimer.Stop();
                        _posTimer.Tick -= PosTimer_Tick;
                        _posTimer.Dispose();
                        _posTimer = null;
                    }
                    catch { }
                }
            }
            isDisposed = true;
            base.Dispose(disposing);
        }
        /// <summary>
        /// //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< FROM WmPlayer  retired 
        int newState = 0;
        public void skipForwardPercent(int divideBy)
        {
            if (newState != 3)//play
                return;
            double ahead = duration / divideBy;
            ahead = ahead - (ahead / 9);
            double pos = position;
            double nextPos = pos + ahead;
            if (nextPos > duration)
            {
                ahead = ahead / 2;
                nextPos = pos + ahead;
            }
            if (nextPos < duration)
                position += ahead;

            gv.debug.w($"movies skipForward Percent");
            SetSpeed(1.0f);
            //resetButtonColor();
            //btSkimForward.BackColor = pushed;
            if (bSetFocusOnParent)
                pTraverser.Activate();
        }

        bool bSetFocusOnParent = true;

        protected override void WndProc(ref Message m)
        {
            const int WM_CONTEXTMENU = 0x007B;
            const int WM_RBUTTONUP   = 0x0205;
            if ((_ctx != null) && (m.Msg == WM_CONTEXTMENU || m.Msg == WM_RBUTTONUP))
            {
                var screenPt = Cursor.Position;
                // ensure click is over the VideoView
                if (_videoView != null && _videoView.Bounds.Contains(this.PointToClient(screenPt)))
                {
                    var viewPt = _videoView.PointToClient(screenPt);
                    _ctx.Show(_videoView, viewPt);
                    return; // consumed
                }
            }
            base.WndProc(ref m);
        }

        public void SetOnErrContinue(bool onErrContinue)
        {
            bOnErrContinue = onErrContinue;

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
        bool bOnErrContinue = false;
        public void ErrorDisplay()
        {
            ERROR err = new ERROR(gv, this, bOnErrContinue);
            err.ShowDialog(this);
            gv.ERROR_STOP = true;
            //  gv.CloseMovies();
        }


        public string ResetAndPlay()
        {
            Stop();
            if (pTraverser != null)
            {
                string newPath = pTraverser.GetNextMovieFullpath();
                LoadMedia(newPath, false);
                Play();
                return newPath;
            }
            return null;
        }
        public void ResetAndPlay(string movieFpath)
        {
            Stop();
            LoadMedia(movieFpath, false);
            Play();
        }
        public string getPreviousTitle()
        {
            return "test";
        }
        public string getPreviousTitle2()
        {
            return "test";
        }
        public string getTitle()
        {
            return currentMediaPath;
        }
        public void SetSmallMoveWindowAndCenter()
        {
            this.WindowState = FormWindowState.Normal;
            this.Width = 300;
            this.Height = 200;
            if (gv == null || gv.screen == null) return;
            if (gv.displayCount > 1)
                myScreen = 0;
            if (myScreen >= gv.screen.Length)
                myScreen = 0;
            if (myScreen >= gv.displayCount)
                myScreen = 0;
            if (gv.screen[myScreen] == null) return;
            int screenX = gv.screen[myScreen].Bounds.X;
            int screenY = gv.screen[myScreen].Bounds.Y;
            int screenW = gv.screen[myScreen].Bounds.Width;
            int screenH = gv.screen[myScreen].Bounds.Height;
            int posX = screenX + (screenW - this.Width) / 2;
            int posY = screenY + (screenH - this.Height) / 2;
            this.Location = new Point(posX, posY);
        }
        private void VideoView_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            // Example: Toggle fullscreen on double-click
            //SetFullScreen(!IsFullScreen());
            SetSmallMoveWindowAndCenter();
        }
        private void Player_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            // Example: Toggle fullscreen on double-click
            //SetFullScreen(!IsFullScreen());
            SetSmallMoveWindowAndCenter();
        }

        /// <summary>
        /// Transcodes the current (or given) MP4 with rotation baked in, saving to outputPath.
        /// transformType: "90", "180", "270" (=-90°), "hflip", "vflip"
        /// Progress 0..100 reported via onProgress; onComplete called when done or on error.
        /// </summary>
        public void SaveRotatedVideo(
            string outputPath,
            string transformType = "270",
            Action<float> onProgress = null,
            Action<bool> onComplete = null,
            string inputPath = null)
        {
            string src = inputPath ?? currentMediaPath;

            if (string.IsNullOrWhiteSpace(src) || !File.Exists(src))
            {
                onComplete?.Invoke(false);
                return;
            }

            System.Threading.Tasks.Task.Run(() =>
            {
                LibVLC transLibVLC = null;
                MediaPlayer transPlayer = null;
                Media transMedia = null;

                try
                {
                    string dst = outputPath.Replace("\\", "/");

                    // vb=0 encodes at 0 kbps → black video.
                    // Use a real bitrate; 4000 kb/s is a safe default for HD.
                    // fps=25 avoids encoder stalls on variable-fps sources.
                    string sout =
                        $"#transcode{{" +
                        $"vcodec=h264," +
                        $"vb=4000," +
                        $"fps=25," +
                        $"vfilter=transform{{type={transformType}}}," +
                        $"acodec=mp4a," +
                        $"ab=192," +
                        $"channels=2," +
                        $"samplerate=44100" +
                        $"}}:std{{access=file,mux=mp4,dst={dst}}}";

                    transLibVLC = new LibVLC("--no-video-title-show");
                    transMedia = new Media(transLibVLC, new Uri(src));

                    transMedia.AddOption($":sout={sout}");
                    transMedia.AddOption(":sout-keep");
                    transMedia.AddOption(":no-sout-video-palette");   // avoid palette issues

                    transPlayer = new MediaPlayer(transLibVLC) { Media = transMedia };

                    transPlayer.PositionChanged += (s, ev) =>
                        onProgress?.Invoke(ev.Position * 100f);

                    bool finished = false;
                    bool errored = false;

                    transPlayer.EndReached += (s, ev) => { finished = true; };
                    transPlayer.EncounteredError += (s, ev) => { finished = true; errored = true; };

                    transPlayer.Play();

                    // Wait up to 3 hours
                    int maxWaitMs = 3 * 60 * 60 * 1000;
                    int waited = 0;
                    while (!finished && waited < maxWaitMs)
                    {
                        System.Threading.Thread.Sleep(500);
                        waited += 500;
                    }

                    transPlayer.Stop();
                    System.Threading.Thread.Sleep(500); // let libvlc flush and close the mp4

                    bool success = !errored
                                && File.Exists(outputPath)
                                && new FileInfo(outputPath).Length > 10_000; // sanity-check size

                    this.Invoke(() => onComplete?.Invoke(success));
                }
                catch (Exception ex)
                {
                    this.Invoke(() =>
                    {
                        MessageBox.Show($"Transcode error:\n{ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        onComplete?.Invoke(false);
                    });
                }
                finally
                {
                    try { transPlayer?.Dispose(); } catch { }
                    try { transMedia?.Dispose(); } catch { }
                    try { transLibVLC?.Dispose(); } catch { }
                }
            });
        }
    }
}

// Note: This code assumes you have the appropriate LibVLCSharp packages installed and referenced in your project.
/*
 * 
 Summary Table
#	State	        How Entered	        Duration Valid?	Timer Running?
1	Idle/Null	    App start	            ❌	❌
2	Opening	        Play() called	            ❌	❌
3	Playing	        LibVLC ready	        ✅	✅
4	Paused	        Pause()	                 ✅	❌
5	Playing         (resumed)	Play()	    ✅	✅
6	Stopped	        Stop()	                ❌	❌
7	EndReached	    Natural end	            ✅	⚠️ still on
8	Disposed	    Form closed	—	            ❌
*/