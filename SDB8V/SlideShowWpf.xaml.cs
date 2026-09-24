#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Screen = System.Windows.Forms.Screen;

namespace SymbolDB
{
    public partial class SlideShowWpf : Window
    {
        private const int FilmstripRadius = 5;
        private readonly GlobalVars _gv;
        private readonly DialogTraverser _traverser;
        private readonly DispatcherTimer _slideTimer;
        private readonly DispatcherTimer _displayNamesTimer;
        private readonly Dictionary<int, Display1Wpf> _displayWindows = new Dictionary<int, Display1Wpf>();
        private DisplayPreviewSetWpf _previewSetWindow;
        private bool _closing;
        private int _currentIndex;
        private decimal _speedSeconds = 2.0m;

        public SlideShowWpf(GlobalVars gv, DialogTraverser traverser)
        {
            InitializeComponent();

            _gv = gv ?? throw new ArgumentNullException(nameof(gv));
            _traverser = traverser;
            _slideTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds((double)_speedSeconds) };
            _slideTimer.Tick += SlideTimer_Tick;
            _displayNamesTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            _displayNamesTimer.Tick += (_, _) =>
            {
                _displayNamesTimer.Stop();
                if (cbShowDisplayNames.IsChecked == true)
                    cbShowDisplayNames.IsChecked = false;
            };

            _currentIndex = Math.Clamp(_gv.nextIdx, 0, Math.Max(0, ImageCount - 1));
            Loaded += SlideShowWpf_Loaded;
            Closed += SlideShowWpf_Closed;
        }

        private void ShowPreviewSet_Click(object sender, RoutedEventArgs e) => displayPreviewSetWpf();

        public void displayPreviewSetWpf(int startingImageNumber = -1, int numberOfRows = 3)
        {
            if (_closing) return;
            if (_previewSetWindow == null)
            {
                _previewSetWindow = new DisplayPreviewSetWpf(_gv, this,
                    startingImageNumber < 0 ? _currentIndex : startingImageNumber, numberOfRows)
                { Owner = this };
                _previewSetWindow.Closed += (_, _) => _previewSetWindow = null;
                ElementHost.EnableModelessKeyboardInterop(_previewSetWindow);
                _previewSetWindow.Show();
            }
            else
                _previewSetWindow.Activate();
        }

        public void ShowPreviewImage(int index) => ShowImage(index);

        public void MarkPreviewImageForDeletion(int index)
        {
            if (index < 0 || index >= ImageCount) return;
            Images[index].bDelete = true;
            if (index == _currentIndex)
                DeleteCheckBox.IsChecked = true;
        }

        private void DeleteCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (ImageCount == 0 || _currentIndex < 0 || _currentIndex >= ImageCount)
                return;

            Images[_currentIndex].bDelete = DeleteCheckBox.IsChecked == true;
        }

        private void MarkAllInvalid_Click(object sender, RoutedEventArgs e)
        {
            int marked = 0;
            foreach (FileInfoItem item in Images)
            {
                if (item.bInvalid && !item.bDelete)
                {
                    item.bDelete = true;
                    marked++;
                }
            }

            if (ImageCount > 0)
                DeleteCheckBox.IsChecked = Images[_currentIndex].bDelete;
            StatusTextBlock.Text = $"Marked {marked:N0} invalid image(s) for deletion.";
        }

        private void DeleteMarked_Click(object sender, RoutedEventArgs e)
        {
            var list = _gv.imageFileList1?.finfoList;
            if (list == null)
                return;

            int marked = list.Count(item => item.bDelete);
            if (marked == 0)
            {
                StatusTextBlock.Text = "No images are marked for deletion.";
                return;
            }

            if (System.Windows.MessageBox.Show(this,
                    $"Permanently delete {marked:N0} marked image file(s) from disk and remove their entries from the image list?",
                    "Delete Marked", MessageBoxButton.YesNo, MessageBoxImage.Warning,
                    MessageBoxResult.No) != MessageBoxResult.Yes)
                return;

            StopSlideshow();
            // Release the displayed file before deleting it; thumbnail bitmaps use OnLoad caching.
            MainImage.Source = null;
            FilmstripItemsControl.ItemsSource = null;
            foreach (Display1Wpf window in _displayWindows.Values)
                window.clearDisplay();

            int deleted = 0;
            int failed = 0;
            int nextIndex = _currentIndex;
            for (int index = list.Count - 1; index >= 0; index--)
            {
                FileInfoItem item = list[index];
                if (!item.bDelete)
                    continue;

                try
                {
                    if (string.IsNullOrWhiteSpace(item.fpath) || Directory.Exists(item.fpath))
                        throw new IOException("The image path is empty or is a directory.");
                    // A missing file is already gone; remove its stale catalog entry too.
                    if (File.Exists(item.fpath))
                        File.Delete(item.fpath);
                    if (File.Exists(item.fpath))
                        throw new IOException("The file still exists after deletion.");

                    list.RemoveAt(index);
                    if (index < nextIndex)
                        nextIndex--;
                    deleted++;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException
                                           or NotSupportedException or System.Security.SecurityException)
                {
                    failed++;
                }
            }

            _gv.slideCount1 = list.Count;
            _currentIndex = Math.Clamp(nextIndex, 0, Math.Max(0, ImageCount - 1));
            ShowImage(_currentIndex);
            _previewSetWindow?.RefreshImageList();
            StatusTextBlock.Text = $"Removed {deleted:N0} marked image(s); {failed:N0} could not be deleted and remain marked.";
        }

        public void SetPreviewNextSlideNumber(int index) => ShowImage(index);

        private IReadOnlyList<FileInfoItem> Images =>
            _gv.imageFileList1?.finfoList ?? (IReadOnlyList<FileInfoItem>)Array.Empty<FileInfoItem>();

        private int ImageCount => Images.Count;

        private void SlideShowWpf_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDisplays();
            ShowImage(_currentIndex);
            Activate();
            Focus();
        }

        public void RefreshImageList()
        {
            _currentIndex = Math.Clamp(_gv.nextIdx, 0, Math.Max(0, ImageCount - 1));
            ShowImage(_currentIndex);
        }

        private void SlideShowWpf_Closed(object sender, EventArgs e)
        {
            _closing = true;
            StopSlideshow();
            _displayNamesTimer.Stop();
            _previewSetWindow?.Close();
            _previewSetWindow = null;
            foreach (Display1Wpf window in _displayWindows.Values.ToArray())
                window.Close();
            _displayWindows.Clear();
            MainImage.Source = null;
            FilmstripItemsControl.ItemsSource = null;
        }

        private void ShowImage(int index)
        {
            if (ImageCount == 0)
            {
                ClearDisplay("No images are available in gv.imageFileList1.");
                foreach (Display1Wpf window in _displayWindows.Values)
                    window.clearDisplay();
                return;
            }

            _currentIndex = Math.Clamp(index, 0, ImageCount - 1);
            _gv.nextIdx = _currentIndex;
            FileInfoItem item = Images[_currentIndex];
            item.ndx = _currentIndex;

            TitleTextBlock.Text = item.fname;
            PositionTextBlock.Text = $"Slide {_currentIndex + 1:N0} / {ImageCount:N0}";
            // Do not overwrite an in-progress edit. The field is refreshed after
            // navigation once it no longer owns keyboard focus.
            if (!JumpTextBox.IsKeyboardFocusWithin)
                JumpTextBox.Text = (_currentIndex + 1).ToString();
            FolderTextBox.Text = item.dpath ?? string.Empty;

            FileNameValue.Text = item.fname ?? string.Empty;
            FolderValue.Text = item.dpath ?? string.Empty;
            FullPathValue.Text = item.fpath ?? string.Empty;
            TypeValue.Text = string.IsNullOrWhiteSpace(item.ext) ? item.type : item.ext.TrimStart('.').ToUpperInvariant();
            ModifiedValue.Text = item.stimestamp ?? string.Empty;
            RatingValue.Text = BuildRating(item.rating);
            InvalidCheckBox.IsChecked = item.bInvalid;
            DeleteCheckBox.IsEnabled = true;
            DeleteCheckBox.IsChecked = item.bDelete;
            CommentTextBox.Text = item.comment ?? string.Empty;

            try
            {
                MainImage.Source = LoadBitmap(item.fpath, 1600);
                UpdateImageInfo(item);
                StatusTextBlock.Text = BuildStatus(item);
            }
            catch (Exception ex)
            {
                MainImage.Source = null;
                item.bInvalid = true;
                InvalidCheckBox.IsChecked = true;
                StatusTextBlock.Text = $"Unable to load {item.fname}: {ex.Message}";
            }

            WidthValue.Text = item.width.ToString("N0");
            HeightValue.Text = item.height.ToString("N0");
            LenValue.Text = item.len.ToString("N0");

            RefreshFilmstrip();
            foreach (Display1Wpf window in _displayWindows.Values)
                window.displayThisImage(item);
        }

        private static int GetGdiDisplayNumber(Screen screen)
        {
            Match match = Regex.Match(screen.DeviceName ?? string.Empty, @"\d+$");
            return match.Success && int.TryParse(match.Value, out int number) ? number : 0;
        }

        private static Screen GetScreen(int number) =>
            Screen.AllScreens.FirstOrDefault(screen => GetGdiDisplayNumber(screen) == number);

        private CheckBox GetDisplayCheckBox(int number) => number switch
        {
            1 => cbDisplay1,
            2 => cbDisplay2,
            3 => cbDisplay3,
            4 => cbDisplay4,
            _ => null
        };

        private void RefreshDisplays()
        {
            Screen[] screens = Screen.AllScreens;
            var otherScreens = screens.Where(screen => !screen.Primary)
                .OrderBy(GetGdiDisplayNumber).ToList();
            for (int number = 1; number <= 4; number++)
            {
                CheckBox checkBox = GetDisplayCheckBox(number);
                Screen screen = GetScreen(number);
                checkBox.IsEnabled = screen != null;
                checkBox.ToolTip = screen == null ? "Not connected" :
                    $"{screen.DeviceName}: {screen.Bounds.Width} × {screen.Bounds.Height} at ({screen.Bounds.X}, {screen.Bounds.Y})";
                if (screen == null)
                    checkBox.IsChecked = false;
            }

            DisplayInfoItemsControl.ItemsSource = screens.Select(screen =>
            {
                int windowsNumber = screen.Primary ? 1 : otherScreens.FindIndex(other => other.DeviceName == screen.DeviceName) + 2;
                return $"Windows display {windowsNumber} ({screen.Primary switch { true => "Primary", false => "Secondary" }})\n" +
                    $"{screen.DeviceName}  |  {screen.Bounds.Width} × {screen.Bounds.Height}\n" +
                    $"Position: {screen.Bounds.X}, {screen.Bounds.Y}  |  Working area: {screen.WorkingArea.Width} × {screen.WorkingArea.Height}";
            }).ToArray();
        }
        private void ShowDisplayNames_Changed(object sender, RoutedEventArgs e)
        {
            if (DisplayInfoPanel == null)
                return;
            _gv.mainWindow.Hide();
            if (cbShowDisplayNames.IsChecked == true)
            {
                RefreshDisplays();
                _displayNamesTimer.Stop();
                _displayNamesTimer.Start();
            }
            else
            {
                _displayNamesTimer.Stop();
            }
            DisplayInfoPanel.Visibility = cbShowDisplayNames.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            _gv.mainWindow?.ShowDisplayNames(cbShowDisplayNames.IsChecked == true);
        }

        private void Display_Changed(object sender, RoutedEventArgs e)
        {
            if (_closing || sender is not CheckBox checkBox || !int.TryParse(checkBox.Tag?.ToString(), out int number))
                return;

            if (checkBox.IsChecked != true)
            {
                if (_displayWindows.TryGetValue(number, out Display1Wpf existing))
                {
                    _displayWindows.Remove(number);
                    existing.Close();
                }
                return;
            }

            Screen screen = GetScreen(number);
            if (screen == null)
            {
                checkBox.IsChecked = false;
                StatusTextBlock.Text = $"Display {number} is not connected.";
                return;
            }

            if (_displayWindows.ContainsKey(number))
                return;

            var window = new Display1Wpf(_gv) { Owner = this, WindowStartupLocation = WindowStartupLocation.Manual };
            window.SetSlideshowHost(MoveBy, StopSlideshow, StartSlideshow);
            window.Closed += (_, _) =>
            {
                if (_displayWindows.TryGetValue(number, out Display1Wpf current) && ReferenceEquals(current, window))
                {
                    _displayWindows.Remove(number);
                    if (!_closing)
                        checkBox.IsChecked = false;
                }
            };
            _displayWindows[number] = window;
            ElementHost.EnableModelessKeyboardInterop(window);
            window.Show();
            window.setDisplayMonitor(screen, number);
            if (ImageCount > 0)
                window.displayThisImage(Images[_currentIndex]);
        }

        private void FullScreenAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Display1Wpf window in _displayWindows.Values)
                window.setFullScreenMode(true);
        }

        private void StopSlideshow()
        {
            _slideTimer.Stop();
            PlayPauseButton.Content = "Play";
            _gv.bSlideShow = false;
        }

        private void StartSlideshow()
        {
            if (ImageCount == 0 || _slideTimer.IsEnabled)
                return;
            _slideTimer.Start();
            PlayPauseButton.Content = "Pause";
            _gv.bSlideShow = true;
        }

        private void RefreshFilmstrip()
        {
            if (ImageCount == 0)
            {
                FilmstripItemsControl.ItemsSource = null;
                return;
            }

            int first = Math.Max(0, _currentIndex - FilmstripRadius);
            int last = Math.Min(ImageCount - 1, _currentIndex + FilmstripRadius);
            var thumbnails = new List<ThumbnailItem>(last - first + 1);

            for (int index = first; index <= last; index++)
            {
                FileInfoItem item = Images[index];
                BitmapSource thumbnail = null;
                try
                {
                    thumbnail = LoadBitmap(item.fpath, 240);
                }
                catch
                {
                    // Keep the filmstrip usable when one file is missing or corrupt.
                }

                thumbnails.Add(new ThumbnailItem
                {
                    Index = index,
                    Caption = $"{index + 1:N0}  {item.fname}",
                    Thumbnail = thumbnail,
                    Background = index == _currentIndex
                        ? new SolidColorBrush(Color.FromRgb(78, 145, 232))
                        : new SolidColorBrush(Color.FromRgb(52, 54, 66))
                });
            }

            FilmstripItemsControl.ItemsSource = thumbnails;
        }

        private static void UpdateImageInfo(FileInfoItem item)
        {
            // The viewer image is decoded to at most 1600 pixels wide. Read the
            // original frame dimensions rather than the resized display bitmap.
            using (var stream = File.OpenRead(item.fpath))
            {
                BitmapDecoder decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnDemand);
                item.width = decoder.Frames[0].PixelWidth;
                item.height = decoder.Frames[0].PixelHeight;
            }

            item.len = new FileInfo(item.fpath).Length;
        }

        private static BitmapImage LoadBitmap(string path, int decodePixelWidth)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new InvalidOperationException("The image path is empty.");
            if (!File.Exists(path))
                throw new FileNotFoundException("Image file not found.", path);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            if (decodePixelWidth > 0)
                bitmap.DecodePixelWidth = decodePixelWidth;
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private static string BuildRating(char rating)
        {
            int stars = char.IsDigit(rating) ? Math.Clamp(rating - '0', 0, 5) : 0;
            if (stars == 0 && rating != ' ' && rating != '\0')
                return $"{rating}  ☆☆☆☆☆";
            return new string('★', stars) + new string('☆', 5 - stars);
        }

        private string BuildStatus(FileInfoItem item)
        {
            string size = item.len > 0 ? $"{item.len / 1024.0:N0} KB" : "size unknown";
            string dimensions = item.width > 0 && item.height > 0 ? $"{item.width:N0} × {item.height:N0}" : "dimensions unknown";
            return $"Slide {_currentIndex + 1:N0} / {ImageCount:N0}  |  {dimensions}  |  {item.ext}  |  {size}  |  Catalog loaded";
        }

        private void ClearDisplay(string message)
        {
            MainImage.Source = null;
            WidthValue.Text = string.Empty;
            HeightValue.Text = string.Empty;
            LenValue.Text = string.Empty;
            InvalidCheckBox.IsChecked = false;
            DeleteCheckBox.IsChecked = false;
            DeleteCheckBox.IsEnabled = false;
            TitleTextBlock.Text = "No image";
            PositionTextBlock.Text = "Slide 0 / 0";
            StatusTextBlock.Text = message;
            FilmstripItemsControl.ItemsSource = null;
        }

        private void MoveBy(int offset) => ShowImage(_currentIndex + offset);

        private void First_Click(object sender, RoutedEventArgs e) => ShowImage(0);

        private void Previous_Click(object sender, RoutedEventArgs e) => MoveBy(-1);

        private void Next_Click(object sender, RoutedEventArgs e) => MoveBy(1);

        private void Last_Click(object sender, RoutedEventArgs e) => ShowImage(ImageCount - 1);

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_slideTimer.IsEnabled)
            {
                _slideTimer.Stop();
                PlayPauseButton.Content = "Play";
                _gv.bSlideShow = false;
            }
            else if (ImageCount > 0)
            {
                _slideTimer.Start();
                PlayPauseButton.Content = "Pause";
                _gv.bSlideShow = true;
            }
        }

        private void SlideTimer_Tick(object sender, EventArgs e)
        {
            if (_currentIndex >= ImageCount - 1)
            {
                _slideTimer.Stop();
                PlayPauseButton.Content = "Play";
                _gv.bSlideShow = false;
                StatusTextBlock.Text = "End of slideshow.";
                return;
            }

            MoveBy(1);
        }

        private void SpeedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitSpeed();
                e.Handled = true;
            }
        }

        private void SpeedTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
            => CommitSpeed();

        private void DecreaseSpeed_Click(object sender, RoutedEventArgs e)
            => ChangeSpeed(0.1m);

        private void IncreaseSpeed_Click(object sender, RoutedEventArgs e)
            => ChangeSpeed(-0.1m);

        private void ChangeSpeed(decimal difference)
        {
            // Commit a typed value before stepping; invalid input reverts to the last speed.
            CommitSpeed();
            SetSpeed(Math.Clamp(_speedSeconds + difference, 0.1m, 5.0m));
        }

        private void CommitSpeed()
        {
            string text = SpeedTextBox.Text?.Trim();
            const NumberStyles speedNumberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign |
                                                  NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
            if ((decimal.TryParse(text, speedNumberStyle, CultureInfo.CurrentCulture, out decimal seconds) ||
                 decimal.TryParse(text, speedNumberStyle, CultureInfo.InvariantCulture, out seconds)) &&
                seconds >= 0.1m && seconds <= 5.0m)
            {
                SetSpeed(seconds);
            }
            else
            {
                SpeedTextBox.Text = _speedSeconds.ToString("0.0#", CultureInfo.CurrentCulture);
                StatusTextBlock.Text = "Speed must be between 0.1 and 5 seconds.";
            }
        }

        private void SetSpeed(decimal seconds)
        {
            _speedSeconds = seconds;
            bool wasPlaying = _slideTimer.IsEnabled;
            if (wasPlaying)
                _slideTimer.Stop();
            _slideTimer.Interval = TimeSpan.FromSeconds((double)seconds);
            SpeedTextBox.Text = seconds.ToString("0.0#", CultureInfo.CurrentCulture);
            if (wasPlaying)
                _slideTimer.Start();
        }

        private void Jump_Click(object sender, RoutedEventArgs e) => JumpToRequestedSlide();

        private void JumpTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                JumpToRequestedSlide();
                e.Handled = true;
            }
        }

        private void JumpTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(JumpTextBox.Text))
                JumpTextBox.SelectAll();
        }

        private void JumpToRequestedSlide()
        {
            if (int.TryParse(JumpTextBox.Text, out int slideNumber))
            {
                ShowImage(slideNumber - 1);
                JumpTextBox.Text = (_currentIndex + 1).ToString();
            }
            else
                StatusTextBlock.Text = "Enter a valid slide number.";
        }

        private void Search_Click(object sender, RoutedEventArgs e) => FindNextMatch();

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                FindNextMatch();
        }

        private void FindNextMatch()
        {
            string query = SearchTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(query) || ImageCount == 0)
                return;

            for (int offset = 1; offset <= ImageCount; offset++)
            {
                int index = (_currentIndex + offset) % ImageCount;
                FileInfoItem item = Images[index];
                if ((item.fname?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.fpath?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    ShowImage(index);
                    return;
                }
            }

            StatusTextBlock.Text = $"No image matched “{query}”.";
        }

        private void Thumbnail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int index)
                ShowImage(index);
        }

        private void ShowMain_Click(object sender, RoutedEventArgs e)
        {
            if (_gv.mainWindow == null || _gv.mainWindow.IsDisposed)
                return;

            _gv.mainWindow.Show();
            _gv.mainWindow.Activate();
            _gv.mainWindow.BringToFront();
        }

        private void ShowTraverser_Click(object sender, RoutedEventArgs e)
        {
            DialogTraverser traverser = _traverser ?? _gv.dialogTraverser1;
            if (traverser == null || traverser.IsDisposed)
                return;

            traverser.Show();
            traverser.Activate();
            traverser.BringToFront();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                Close();
                e.Handled = true;
                return;
            }

            // Keep editing keys, including Delete, available inside editable textboxes.
            if (Keyboard.FocusedElement is TextBox { IsReadOnly: false })
                return;

            switch (e.Key)
            {
                case Key.Delete:
                    if (ImageCount > 0 && _currentIndex >= 0 && _currentIndex < ImageCount)
                    {
                        FileInfoItem item = Images[_currentIndex];
                        item.bDelete = !item.bDelete;
                        DeleteCheckBox.IsChecked = item.bDelete;
                        e.Handled = true;
                    }
                    break;
                case Key.Left:
                    MoveBy(-1);
                    e.Handled = true;
                    break;
                case Key.Right:
                    MoveBy(1);
                    e.Handled = true;
                    break;
                case Key.Home:
                    ShowImage(0);
                    e.Handled = true;
                    break;
                case Key.End:
                    ShowImage(ImageCount - 1);
                    e.Handled = true;
                    break;
                case Key.Space:
                    PlayPause_Click(sender, e);
                    e.Handled = true;
                    break;
            }
        }

        private sealed class ThumbnailItem
        {
            public int Index { get; init; }
            public string Caption { get; init; }
            public BitmapSource Thumbnail { get; init; }
            public Brush Background { get; init; }
        }
    }
}
