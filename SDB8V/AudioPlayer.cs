using LibVLCSharp.Shared;
using System;
using System.IO;

namespace SymbolDB
{
    /// <summary>
    /// Lightweight audio-only playback using LibVLC.
    /// Supports WAV and MP3 files. No UI/video surface needed.
    /// </summary>
    public class AudioPlayer : IDisposable
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;
        private Media _currentMedia;
        private bool _isDisposed;

        // ---------------------------------------------------------------
        // Events
        // ---------------------------------------------------------------
        public event EventHandler PlaybackStarted;
        public event EventHandler PlaybackPaused;
        public event EventHandler PlaybackStopped;
        public event EventHandler PlaybackEndReached;
        public event EventHandler<string> ErrorOccurred;

        // ---------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------
        public AudioPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            _mediaPlayer.Playing  += (s, e) => PlaybackStarted?.Invoke(this, EventArgs.Empty);
            _mediaPlayer.Paused   += (s, e) => PlaybackPaused?.Invoke(this, EventArgs.Empty);
            _mediaPlayer.Stopped  += (s, e) => PlaybackStopped?.Invoke(this, EventArgs.Empty);
            _mediaPlayer.EndReached += (s, e) => PlaybackEndReached?.Invoke(this, EventArgs.Empty);
            _mediaPlayer.EncounteredError += (s, e) =>
                ErrorOccurred?.Invoke(this, "LibVLC encountered a playback error.");
        }

        // ---------------------------------------------------------------
        // Properties
        // ---------------------------------------------------------------

        /// <summary>Current RootRelativePath of the loaded audio file.</summary>
        public string CurrentFilePath { get; private set; }

        /// <summary>Playback volume, 0–100.</summary>
        public int Volume
        {
            get => _mediaPlayer.Volume;
            set => _mediaPlayer.Volume = Math.Clamp(value, 0, 100);
        }

        /// <summary>Current playback position in seconds.</summary>
        public double PositionSeconds
        {
            get
            {
                long ms = _mediaPlayer.Time;
                return ms < 0 ? 0.0 : ms / 1000.0;
            }
            set
            {
                long ms = (long)(value * 1000);
                _mediaPlayer.Time = ms;
            }
        }

        /// <summary>Total duration of the loaded media in seconds. Returns 0 if unknown.</summary>
        public double DurationSeconds
        {
            get
            {
                long ms = _mediaPlayer.Length;
                return ms < 0 ? 0.0 : ms / 1000.0;
            }
        }

        /// <summary>True while audio is actively playing.</summary>
        public bool IsPlaying => _mediaPlayer.IsPlaying;

        /// <summary>Current state as a display string.</summary>
        public string State => _mediaPlayer.State.ToString();

        // ---------------------------------------------------------------
        // Playback control
        // ---------------------------------------------------------------

        /// <summary>Load a WAV or MP3 file and begin playing immediately.</summary>
        public void Load(string filePath, bool autoPlay = true)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Audio file not found.", filePath);

            DisposeCurrentMedia();

            CurrentFilePath = filePath;
            _currentMedia = new Media(_libVLC, new Uri(filePath));
            _mediaPlayer.Media = _currentMedia;

            if (autoPlay)
                _mediaPlayer.Play();
        }

        /// <summary>Play or resume.</summary>
        public void Play()
        {
            if (_mediaPlayer.Media != null)
                _mediaPlayer.Play();
        }

        /// <summary>Pause playback.</summary>
        public void Pause()
        {
            if (_mediaPlayer.CanPause)
                _mediaPlayer.Pause();
        }

        /// <summary>Toggle between play and pause.</summary>
        public void TogglePlayPause()
        {
            if (IsPlaying)
                Pause();
            else
                Play();
        }

        /// <summary>Stop playback and reset position to the beginning.</summary>
        public void Stop()
        {
            _mediaPlayer.Stop();
        }

        /// <summary>Skip forward by <paramref name="seconds"/> seconds.</summary>
        public void SkipForward(double seconds = 5.0)
        {
            long newTime = _mediaPlayer.Time + (long)(seconds * 1000);
            long length  = _mediaPlayer.Length;
            _mediaPlayer.Time = length > 0 ? Math.Min(newTime, length) : newTime;
        }

        /// <summary>Skip backward by <paramref name="seconds"/> seconds.</summary>
        public void SkipBack(double seconds = 5.0)
        {
            long newTime = _mediaPlayer.Time - (long)(seconds * 1000);
            _mediaPlayer.Time = Math.Max(0, newTime);
        }

        // ---------------------------------------------------------------
        // Cleanup
        // ---------------------------------------------------------------

        private void DisposeCurrentMedia()
        {
            if (_currentMedia != null)
            {
                _currentMedia.Dispose();
                _currentMedia = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
            {
                _mediaPlayer.Stop();
                DisposeCurrentMedia();
                _mediaPlayer.Dispose();
                _libVLC.Dispose();
            }
            _isDisposed = true;
        }
    }
}