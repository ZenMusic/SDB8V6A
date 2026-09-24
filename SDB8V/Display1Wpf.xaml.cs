#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DrawingImage = System.Drawing.Image;
using WpfPoint = System.Windows.Point;
using WpfRectangle = System.Windows.Shapes.Rectangle;

namespace SymbolDB
{
    /// <summary>An opt-in WPF alternative to the existing WinForms Display1.</summary>
    public partial class Display1Wpf : Window
    {
        private readonly GlobalVars _gv;
        private readonly DispatcherTimer _slideshowSync;
        private readonly List<Int32Rect> _tags = new List<Int32Rect>();
        private FileSystemWatcher _watcher;
        private FileInfoItem _fileInfo;
        private string _currentPath;
        private BitmapSource _original;
        private D1function _mode = D1function.SLIDESHOW;
        private bool _holdSlide;
        private bool _dragging;
        private WpfPoint _dragStart;
        private WpfRectangle _selection;
        private int _screenNumber;
        private double _zoom = 1;
        private Action<int> _navigateSlideshow;
        private Action _stopSlideshow;
        private Action _startSlideshow;

        public Display1Wpf(GlobalVars gv)
        {
            InitializeComponent();
            _gv = gv ?? throw new ArgumentNullException(nameof(gv));
            _slideshowSync = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            _slideshowSync.Tick += (_, _) =>
            {
                if (!_gv.bSlideShow || _holdSlide)
                {
                    if (!_gv.bSlideShow)
                        _slideshowSync.Stop();
                    return;
                }

                var list = _gv.imageFileList1;
                int index = _gv.nextIdx;
                if (list?.finfoList == null || index < 0 || index >= list.finfoList.Count)
                    return;

                var info = list.finfoList[index];
                if (!string.Equals(_currentPath, info.fpath, StringComparison.OrdinalIgnoreCase))
                    displayThisImage(info);
            };
        }

        /// <summary>Open modelessly from any existing WinForms form, with working WPF keyboard input.</summary>
        public static Display1Wpf ShowFromWinForms(GlobalVars gv, Form owner)
        {
            var window = new Display1Wpf(gv);
            if (owner != null)
                new WindowInteropHelper(window).Owner = owner.Handle;
            ElementHost.EnableModelessKeyboardInterop(window);
            window.Show();
            return window;
        }

        public bool bHoldThisImage => _holdSlide;

        /// <summary>Connect a mirrored display to its WPF slideshow instead of the legacy WinForms slideshow.</summary>
        public void SetSlideshowHost(Action<int> navigate, Action stop, Action start)
        {
            _navigateSlideshow = navigate;
            _stopSlideshow = stop;
            _startSlideshow = start;
        }

        private void StopSlideshow()
        {
            if (_navigateSlideshow != null)
                _stopSlideshow?.Invoke();
            else
                _gv.mainWindow?.stopSlideShow();
        }

        public D1function getDisplayMode() => _mode;

        public void setMode(D1function mode)
        {
            _mode = mode;
            AnnotationMenuItem.IsChecked = mode == D1function.ANNOTATE;
        }

        public void setCatalogMode()
        {
            setMode(D1function.CATALOG);
            StopSlideshow();
            _slideshowSync.Stop();
        }

        public void setDisplayMonitor(Screen screen, int number)
        {
            if (screen == null)
                return;

            _screenNumber = number;
            var transform = PresentationSource.FromVisual(this)?.CompositionTarget?.TransformFromDevice;
            var topLeft = transform?.Transform(new WpfPoint(screen.Bounds.Left, screen.Bounds.Top))
                ?? new WpfPoint(screen.Bounds.Left, screen.Bounds.Top);
            var bottomRight = transform?.Transform(new WpfPoint(screen.Bounds.Right, screen.Bounds.Bottom))
                ?? new WpfPoint(screen.Bounds.Right, screen.Bounds.Bottom);

            WindowState = WindowState.Normal;
            Left = topLeft.X;
            Top = topLeft.Y;
            Width = bottomRight.X - topLeft.X;
            Height = bottomRight.Y - topLeft.Y;
            Title = $"Display {number}";
            WindowState = WindowState.Maximized;
        }

        public bool displayThisImage(FileInfoItem item)
        {
            if (item == null || _holdSlide)
                return false;
            if (!displayThisImage(item.fpath))
                return false;
            _fileInfo = item;
            Title = $"{item.fname}  {item.len:N0}  {item.fpath}";
            return true;
        }

        public bool displayThisImage(string path)
        {
            if (_holdSlide)
                return false;
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = new Uri(Path.GetFullPath(path), UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();
                DisplayBitmap(bitmap, path);
                Title = $"{Path.GetFileName(path)}  {path}";
                return true;
            }
            catch (Exception ex)
            {
                ShowStatus($"Unable to display {path}: {ex.Message}");
                return false;
            }
        }

        public void displayThisImage(DrawingImage image) => displayThisImage(image, "Direct image");

        public void displayThisImage(DrawingImage image, string title)
        {
            if (image == null || _holdSlide)
                return;
            using var stream = new MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            DisplayBitmap(bitmap, null);
            Title = title;
        }

        private void DisplayBitmap(BitmapSource source, string path)
        {
            _original = source;
            _currentPath = path;
            _fileInfo = null;
            _zoom = 1;
            DisplayedImage.Stretch = Stretch.Uniform;
            DisplayedImage.RenderTransform = Transform.Identity;
            DisplayedImage.Source = source;
            _tags.Clear();
            AnnotationCanvas.Children.Clear();
            StatusText.Visibility = Visibility.Collapsed;
        }

        public void clearDisplay()
        {
            _original = null;
            _currentPath = null;
            _fileInfo = null;
            DisplayedImage.Source = null;
            _tags.Clear();
            AnnotationCanvas.Children.Clear();
        }

        public void resetPB() => clearDisplay();

        public void showMessage(string text) => Title = text;

        public void setPictureBoxSize(PictureBoxSizeMode sizeMode)
        {
            _zoom = 1;
            DisplayedImage.RenderTransform = Transform.Identity;
            DisplayedImage.Stretch = sizeMode switch
            {
                PictureBoxSizeMode.StretchImage => Stretch.Fill,
                PictureBoxSizeMode.Normal or PictureBoxSizeMode.AutoSize or PictureBoxSizeMode.CenterImage => Stretch.None,
                _ => Stretch.Uniform
            };
            DisplayedImage.HorizontalAlignment = sizeMode == PictureBoxSizeMode.Normal
                ? System.Windows.HorizontalAlignment.Left : System.Windows.HorizontalAlignment.Center;
            DisplayedImage.VerticalAlignment = sizeMode == PictureBoxSizeMode.Normal
                ? System.Windows.VerticalAlignment.Top : System.Windows.VerticalAlignment.Center;
        }

        public void setFullScreenMode(bool full)
        {
            WindowState = WindowState.Normal;
            WindowStyle = full ? WindowStyle.None : WindowStyle.SingleBorderWindow;
            ResizeMode = full ? ResizeMode.NoResize : ResizeMode.CanResize;
            Topmost = full;
            WindowState = full ? WindowState.Maximized : WindowState.Normal;
            Cursor = full ? System.Windows.Input.Cursors.None : null;
        }

        public void SetFullScreenModeOn() => setFullScreenMode(true);

        private void ImageSurface_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Display1 stops automatic advance when its right-click menu opens.
            StopSlideshow();
            _slideshowSync.Stop();
            Cursor = null;
        }

        private void Annotation_Click(object sender, RoutedEventArgs e)
        {
            setMode(AnnotationMenuItem.IsChecked ? D1function.ANNOTATE : D1function.DISPLAY1);
            if (_mode == D1function.ANNOTATE)
            {
                StopSlideshow();
                _tags.Clear();
                AnnotationCanvas.Children.Clear();
                _gv.mainWindow?.HaveMouse(false);
            }
        }

        private void SlideShow_Click(object sender, RoutedEventArgs e)
        {
            setMode(D1function.SLIDESHOW);
            if (_navigateSlideshow != null)
            {
                _startSlideshow?.Invoke();
                return;
            }
            if (_gv.imageFileList1?.finfoList == null || _gv.imageFileList1.finfoList.Count == 0)
            {
                ShowStatus("Load an image list to start the slideshow.");
                return;
            }

            if (_gv.mainWindow?.startSlideShow(-1, true, _gv.iShowDirection) == true)
            {
                int index = _gv.nextIdx;
                var list = _gv.imageFileList1.finfoList;
                if (index >= 0 && index < list.Count)
                    displayThisImage(list[index]);
                _slideshowSync.Start();
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e) => RedrawImage();
        private void Redraw_Click(object sender, RoutedEventArgs e) => RedrawImage();

        private void RedrawImage()
        {
            if (_currentPath != null && !_holdSlide)
                displayThisImage(_fileInfo ?? new FileInfoItem(_currentPath));
            else
                DisplayedImage.Source = _original;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e) => Close();

        private void CatalogImages_Click(object sender, RoutedEventArgs e)
        {
            setCatalogMode();
            setFullScreenMode(false);
            Title = "Catalog mode .. " + (_currentPath ?? string.Empty);
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            setFullScreenMode(true);
            DisplayedImage.Stretch = Stretch.Uniform;
        }

        private void Resizable_Click(object sender, RoutedEventArgs e)
        {
            var bounds = RestoreBounds;
            setFullScreenMode(false);
            Left = bounds.Left + 100;
            Top = bounds.Top + 100;
            Width = Math.Max(MinWidth, bounds.Width * 0.8);
            Height = Math.Max(MinHeight, bounds.Height * 0.8);
        }

        private void ShowAllTags_Click(object sender, RoutedEventArgs e)
        {
            AnnotationCanvas.Children.Clear();
            foreach (var tag in _tags)
                DrawTag(tag);
        }

        private void MonitorFolder_Click(object sender, RoutedEventArgs e)
        {
            Resizable_Click(sender, e);
            startWatcher();
        }

        private void HoldSlide_Click(object sender, RoutedEventArgs e)
            => _holdSlide = HoldSlideMenuItem.IsChecked;

        public void startWatcher()
        {
            string folder = _gv.watchFolderPath;
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                ShowStatus($"Watch folder does not exist: {folder}");
                return;
            }

            _watcher?.Dispose();
            try
            {
                _watcher = new FileSystemWatcher(folder)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
                    Filter = "*.*"
                };
                _watcher.Created += Watcher_Changed;
                _watcher.Changed += Watcher_Changed;
                _watcher.Renamed += Watcher_Changed;
                _watcher.Error += (_, args) => Dispatcher.BeginInvoke(() => ShowStatus(args.GetException().Message));
                _watcher.EnableRaisingEvents = true;
                ShowStatus($"Watching {folder}");
            }
            catch (Exception ex)
            {
                _watcher?.Dispose();
                _watcher = null;
                ShowStatus($"Cannot watch {folder}: {ex.Message}");
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string ext = Path.GetExtension(e.FullPath);
            if (!new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff" }
                .Contains(ext, StringComparer.OrdinalIgnoreCase))
                return;
            Dispatcher.BeginInvoke(() =>
            {
                if (IsLoaded && !_holdSlide)
                    displayThisImage(e.FullPath);
            });
        }

        private void ImageSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_mode != D1function.ANNOTATE || _original == null)
                return;
            _dragStart = e.GetPosition(ImageSurface);
            _selection = new WpfRectangle { Stroke = Brushes.Red, StrokeThickness = 2, StrokeDashArray = new DoubleCollection { 4, 3 } };
            AnnotationCanvas.Children.Add(_selection);
            _dragging = true;
            ImageSurface.CaptureMouse();
            e.Handled = true;
        }

        private void ImageSurface_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_dragging)
                return;
            var end = e.GetPosition(ImageSurface);
            double left = Math.Min(_dragStart.X, end.X);
            double top = Math.Min(_dragStart.Y, end.Y);
            Canvas.SetLeft(_selection, left);
            Canvas.SetTop(_selection, top);
            _selection.Width = Math.Abs(end.X - _dragStart.X);
            _selection.Height = Math.Abs(end.Y - _dragStart.Y);
        }

        private void ImageSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_dragging)
                return;
            ImageSurface_MouseMove(sender, e);
            _dragging = false;
            ImageSurface.ReleaseMouseCapture();
            AnnotationCanvas.Children.Remove(_selection);
            var rect = ImageRectToSource(new Rect(Canvas.GetLeft(_selection), Canvas.GetTop(_selection),
                _selection.Width, _selection.Height));
            if (rect.Width <= 0 || rect.Height <= 0)
                return;
            _tags.Add(rect);
            var crop = new CroppedBitmap(_original, rect);
            crop.Freeze();
            ShowAnnotationPreview(crop, rect);
            e.Handled = true;
        }

        private Int32Rect ImageRectToSource(Rect selection)
        {
            Rect imageBounds = GetImageBounds();
            if (imageBounds.IsEmpty)
                return Int32Rect.Empty;
            selection.Intersect(imageBounds);
            if (selection.IsEmpty)
                return Int32Rect.Empty;

            double scaleX = imageBounds.Width / _original.PixelWidth;
            double scaleY = imageBounds.Height / _original.PixelHeight;
            int x = Math.Clamp((int)((selection.X - imageBounds.X) / scaleX), 0, _original.PixelWidth - 1);
            int y = Math.Clamp((int)((selection.Y - imageBounds.Y) / scaleY), 0, _original.PixelHeight - 1);
            int right = Math.Clamp((int)Math.Ceiling((selection.Right - imageBounds.X) / scaleX), x + 1, _original.PixelWidth);
            int bottom = Math.Clamp((int)Math.Ceiling((selection.Bottom - imageBounds.Y) / scaleY), y + 1, _original.PixelHeight);
            return new Int32Rect(x, y, right - x, bottom - y);
        }

        private Rect GetImageBounds()
        {
            if (_original == null || ImageSurface.ActualWidth <= 0 || ImageSurface.ActualHeight <= 0)
                return Rect.Empty;

            double width = ImageSurface.ActualWidth;
            double height = ImageSurface.ActualHeight;
            double imageWidth;
            double imageHeight;
            if (DisplayedImage.Stretch == Stretch.Fill)
            {
                imageWidth = width;
                imageHeight = height;
            }
            else if (DisplayedImage.Stretch == Stretch.None)
            {
                imageWidth = _original.PixelWidth;
                imageHeight = _original.PixelHeight;
            }
            else
            {
                double scale = Math.Min(width / _original.PixelWidth, height / _original.PixelHeight);
                imageWidth = _original.PixelWidth * scale;
                imageHeight = _original.PixelHeight * scale;
            }

            double x = DisplayedImage.HorizontalAlignment == System.Windows.HorizontalAlignment.Left
                ? 0 : (width - imageWidth) / 2;
            double y = DisplayedImage.VerticalAlignment == System.Windows.VerticalAlignment.Top
                ? 0 : (height - imageHeight) / 2;
            // RenderTransform scales the image about the center of its surface.
            return new Rect(width / 2 + (x - width / 2) * _zoom,
                height / 2 + (y - height / 2) * _zoom, imageWidth * _zoom, imageHeight * _zoom);
        }

        private void ShowAnnotationPreview(BitmapSource crop, Int32Rect rect)
        {
            var preview = new Window
            {
                Title = "Annotation selection",
                Owner = this,
                Width = 600,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = Brushes.Black
            };
            var layout = new DockPanel();
            var label = new TextBlock
            {
                Text = $"{_currentPath ?? _fileInfo?.fpath ?? "Direct image"}   X={rect.X} Y={rect.Y}  {rect.Width} × {rect.Height}",
                Foreground = Brushes.White,
                Margin = new Thickness(10),
                TextWrapping = TextWrapping.Wrap
            };
            DockPanel.SetDock(label, Dock.Top);
            layout.Children.Add(label);
            layout.Children.Add(new Image { Source = crop, Stretch = Stretch.Uniform, Margin = new Thickness(10) });
            preview.Content = layout;
            preview.ShowDialog();
            AnnotationCanvas.Children.Clear(); // Display1 hides the temporary tag after the dialog.
        }

        private void DrawTag(Int32Rect tag)
        {
            if (_original == null || tag.IsEmpty)
                return;
            Rect imageBounds = GetImageBounds();
            if (imageBounds.IsEmpty)
                return;
            double scaleX = imageBounds.Width / _original.PixelWidth;
            double scaleY = imageBounds.Height / _original.PixelHeight;
            var rectangle = new WpfRectangle
            {
                Width = tag.Width * scaleX, Height = tag.Height * scaleY,
                Stroke = Brushes.Red, StrokeThickness = 2
            };
            Canvas.SetLeft(rectangle, imageBounds.X + tag.X * scaleX);
            Canvas.SetTop(rectangle, imageBounds.Y + tag.Y * scaleY);
            AnnotationCanvas.Children.Add(rectangle);
        }

        private void ImageSurface_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_original == null)
                return;
            _zoom = Math.Clamp(_zoom + (e.Delta > 0 ? 0.1 : -0.1), 0.1, 5);
            DisplayedImage.RenderTransformOrigin = new WpfPoint(0.5, 0.5);
            DisplayedImage.RenderTransform = new ScaleTransform(_zoom, _zoom);
            if (AnnotationCanvas.Children.Count > 0 && !_dragging)
            {
                AnnotationCanvas.Children.Clear();
                foreach (var tag in _tags)
                    DrawTag(tag);
            }
            e.Handled = true;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                if (_navigateSlideshow != null)
                    _navigateSlideshow(1);
                else
                {
                    _gv.mainWindow?.showNextSlide3(_gv.nextIdx, 1);
                    ShowCurrentFromMain();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                StopSlideshow();
                if (_navigateSlideshow != null)
                    _navigateSlideshow(-1);
                else
                {
                    _gv.mainWindow?.showPreviousSlide();
                    ShowCurrentFromMain();
                }
                e.Handled = true;
            }
        }

        private void ShowCurrentFromMain()
        {
            var items = _gv.imageFileList1?.finfoList;
            if (items != null && _gv.nextIdx >= 0 && _gv.nextIdx < items.Count)
                displayThisImage(items[_gv.nextIdx]);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _slideshowSync.Stop();
            _watcher?.Dispose();
            _watcher = null;
            Cursor = null;
            if (_navigateSlideshow == null)
                _gv.mainWindow?.stopSlideShow();
        }

        private void ShowStatus(string message)
        {
            StatusText.Text = message;
            StatusText.Visibility = Visibility.Visible;
        }
    }
}
