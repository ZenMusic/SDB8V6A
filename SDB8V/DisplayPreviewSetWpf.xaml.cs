#nullable disable
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SymbolDB
{
    /// <summary>WPF alternative to the WinForms DisplayPreviewSet; the original form is unchanged.</summary>
    public partial class DisplayPreviewSetWpf : Window
    {
        private const int MaxImagesPerPage = 300;
        private readonly GlobalVars _gv;
        private readonly SlideShowWpf _host;
        private readonly FileFunctions _files;
        private readonly DispatcherTimer _pageTimer;
        private CancellationTokenSource _loading;
        private int _startIndex;
        private int _rows = 3;
        private int _columns = 1;
        private int _selectedIndex = -1;
        private int _sequence;
        private bool _copyAndRename;
        private bool _renameOnClick;
        private bool _readPromptValue;
        private bool _showNames;
        private bool _closing;

        public DisplayPreviewSetWpf(GlobalVars gv, SlideShowWpf host, int startingImageNumber = 0, int numberOfRows = 3)
        {
            InitializeComponent();
            _gv = gv ?? throw new ArgumentNullException(nameof(gv));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _files = new FileFunctions(gv);
            _startIndex = startingImageNumber;
            _rows = Math.Clamp(numberOfRows, 1, 8);
            _pageTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2 * _rows) };
            _pageTimer.Tick += (_, _) =>
            {
                int next = ClampStart(_startIndex + PageSize);
                if (next == _startIndex)
                    _pageTimer.Stop();
                else
                    _ = LoadPageAsync(next);
            };
            Loaded += (_, _) => _ = LoadPageAsync(_startIndex);
        }

        private int ImageCount => _gv.imageFileList1?.finfoList?.Count ?? 0;
        private int PageSize => Math.Min(MaxImagesPerPage, _rows * _columns);
        private int ClampStart(int index) => Math.Clamp(index, 0, Math.Max(0, ImageCount - PageSize));

        public void RefreshImageList()
        {
            if (!_closing)
                _ = LoadPageAsync(_startIndex);
        }

        private void RecalculateColumns()
        {
            double height = Math.Max(150, PreviewSurface.ActualHeight / _rows - 24);
            double width = Math.Max(90, height * .7 + 10);
            _columns = Math.Clamp((int)(Math.Max(100, PreviewSurface.ActualWidth - 24) / width), 1,
                Math.Max(1, MaxImagesPerPage / _rows));
            ImageGrid.Rows = _rows;
            ImageGrid.Columns = _columns;
        }

        private void PreviewSurface_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsLoaded || _closing) return;
            int oldColumns = _columns;
            RecalculateColumns();
            if (oldColumns != _columns)
                _ = LoadPageAsync(_startIndex);
        }

        private static BitmapSource LoadThumbnail(string path, int pixelWidth)
        {
            using var stream = File.OpenRead(path);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.DecodePixelWidth = pixelWidth;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private async Task LoadPageAsync(int index)
        {
            _loading?.Cancel();
            var source = new CancellationTokenSource();
            _loading = source;
            CancellationToken token = source.Token;
            RecalculateColumns();
            _startIndex = ClampStart(index);
            _selectedIndex = -1;
            LoadingOverlay.Visibility = Visibility.Visible;
            PageStatus.Text = $"Loading images from {_startIndex + 1:N0} ...";
            ImageGrid.Children.Clear();
            // Permit layout/render to display the loading notice before decoding the first image.
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Render);
            try
            {
                token.ThrowIfCancellationRequested();
                int count = Math.Min(PageSize, ImageCount - _startIndex);
                double cellHeight = Math.Max(110, PreviewSurface.ActualHeight / _rows - 24);
                int decodeWidth = (int)Math.Clamp(cellHeight * .7, 80, 700);
                for (int slot = 0; slot < count; slot++)
                {
                    token.ThrowIfCancellationRequested();
                    int fileIndex = _startIndex + slot;
                    FileInfoItem item = _gv.imageFileList1.finfoList[fileIndex];
                    BitmapSource bitmap = null;
                    try
                    {
                        bitmap = await Task.Run(() => LoadThumbnail(item.fpath, decodeWidth), token);
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException
                                               or FileFormatException or ArgumentException or InvalidOperationException
                                               or System.Runtime.InteropServices.COMException)
                    {
                        // Invalid or missing files should not prevent the rest of the page from loading.
                        item.bInvalid = true;
                    }
                    token.ThrowIfCancellationRequested();
                    ImageGrid.Children.Add(CreateTile(fileIndex, item, bitmap, cellHeight));
                }
                PageStatus.Text = ImageCount == 0 ? "No images available" :
                    $"Images {_startIndex + 1:N0}–{_startIndex + count:N0} of {ImageCount:N0}   |   {_rows} rows";
                Title = PageStatus.Text;
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (ReferenceEquals(_loading, source))
                {
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                    _loading = null;
                    source.Dispose();
                }
            }
        }

        private Border CreateTile(int index, FileInfoItem item, BitmapSource bitmap, double cellHeight)
        {
            var tile = new Border
            {
                Tag = index,
                Margin = new Thickness(4),
                Padding = new Thickness(3),
                Height = cellHeight,
                Background = Brushes.Black,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(2),
                ToolTip = item.fpath,
                Child = new Grid()
            };
            var grid = (Grid)tile.Child;
            grid.Children.Add(bitmap == null
                ? new TextBlock { Text = "Image unavailable", Foreground = Brushes.White, VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center }
                : new Image { Source = bitmap, Stretch = Stretch.Uniform });
            if (_showNames)
            {
                grid.Children.Add(new TextBlock { Text = item.fname, Background = Brushes.Black, Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top, TextTrimming = TextTrimming.CharacterEllipsis });
            }
            tile.MouseLeftButtonUp += (_, _) => SelectTile(tile, true);
            tile.MouseRightButtonUp += (_, e) => { SelectTile(tile, false); e.Handled = true; tile.ContextMenu.IsOpen = true; };
            tile.MouseLeftButtonDown += (_, e) =>
            {
                if (e.ClickCount == 2)
                {
                    SelectTile(tile, false);
                    DisplaySelected();
                    e.Handled = true;
                }
            };
            tile.ContextMenu = CreateMenu();
            return tile;
        }

        private ContextMenu CreateMenu()
        {
            var menu = new ContextMenu();
            AddMenuItem(menu, "Copy", (_, _) => { }, enabled: false);
            AddMenuItem(menu, "Delete", (_, _) => DeleteSelected());
            AddMenuItem(menu, "Move to Top", (_, _) => MoveToTop());
            AddMenuItem(menu, "Display1", (_, _) => DisplaySelected());
            AddMenuItem(menu, "Copy and Rename", (_, _) => { _copyAndRename = !_copyAndRename; PageStatus.Text = _copyAndRename ? "Copy and Rename enabled" : "Copy and Rename disabled"; });
            AddMenuItem(menu, "Rename With Sequence", (_, _) => { _renameOnClick = !_renameOnClick; PageStatus.Text = _renameOnClick ? "Rename With Sequence enabled" : "Rename With Sequence disabled"; });
            AddMenuItem(menu, "Prompt for sequence #", (_, _) =>
            {
                var prompt = new PromptData(_gv);
                prompt.Show();
                prompt.Activate();
                _readPromptValue = true;
            });
            AddMenuItem(menu, "Toggle Image Width", (_, _) => { }, enabled: false);
            return menu;
        }

        private static void AddMenuItem(ContextMenu menu, string title, RoutedEventHandler click, bool enabled = true)
        {
            var item = new MenuItem { Header = title, IsEnabled = enabled };
            item.Click += click;
            menu.Items.Add(item);
        }

        private FileInfoItem SelectedItem => _selectedIndex >= 0 && _selectedIndex < ImageCount
            ? _gv.imageFileList1.finfoList[_selectedIndex] : null;

        private void SelectTile(Border tile, bool actOnClick)
        {
            _selectedIndex = (int)tile.Tag;
            foreach (Border other in ImageGrid.Children.OfType<Border>())
                other.BorderBrush = ReferenceEquals(other, tile) ? Brushes.Gold : Brushes.Transparent;
            if (actOnClick && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (_gv.bAllowDeleteWithControlClick)
                    DeleteSelected();
                else
                {
                    DisplaySelected();
                    MoveToTop();
                    Close();
                }
                return;
            }
            if (!actOnClick) return;
            if (_copyAndRename) CopyAndRename();
            else if (_renameOnClick) RenameSelected();
        }

        private void DisplaySelected()
        {
            if (SelectedItem != null)
                _host.ShowPreviewImage(_selectedIndex);
        }

        private void DeleteSelected()
        {
            if (SelectedItem == null) return;
            _host.MarkPreviewImageForDeletion(_selectedIndex);
            var tile = ImageGrid.Children.OfType<Border>().FirstOrDefault(b => (int)b.Tag == _selectedIndex);
            if (tile != null) tile.Background = Brushes.LightCoral;
        }

        private void MoveToTop()
        {
            if (SelectedItem == null) return;
            int index = _selectedIndex;
            _host.SetPreviewNextSlideNumber(index);
            _ = LoadPageAsync(index);
        }

        private string NextSequenceName()
        {
            if (_readPromptValue)
            {
                _readPromptValue = false;
                _sequence = _gv.iValue;
            }
            return $"{_sequence:00}.jpg";
        }

        private void CopyAndRename()
        {
            if (SelectedItem == null) return;
            try
            {
                if (string.IsNullOrWhiteSpace(_gv.targetFolder))
                    throw new InvalidOperationException("Set a target folder before copying.");
                _files.CopyFileRename(SelectedItem.fpath, _gv.targetFolder, NextSequenceName());
                _sequence++;
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(this, ex.Message, "Copy and Rename"); }
        }

        private void RenameSelected()
        {
            FileInfoItem item = SelectedItem;
            if (item == null) return;
            try
            {
                string destination = Path.Combine(Path.GetDirectoryName(item.fpath) ?? "", NextSequenceName());
                if (_files.renameFile(item.fpath, destination))
                {
                    _gv.imageFileList1.updatePath(destination, _selectedIndex);
                    _sequence++;
                    _ = LoadPageAsync(_startIndex);
                }
                else
                    System.Windows.MessageBox.Show(this, "The file could not be renamed.", "Rename With Sequence");
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(this, ex.Message, "Rename With Sequence"); }
        }

        private void SetRows(int rows)
        {
            _rows = Math.Clamp(rows, 1, 8);
            _pageTimer.Interval = TimeSpan.FromSeconds(2 * _rows);
            _ = LoadPageAsync(_startIndex);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int next = _startIndex;
            if (e.Key >= Key.D1 && e.Key <= Key.D8) SetRows(e.Key - Key.D0);
            else if (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad8) SetRows(e.Key - Key.NumPad0);
            else if (e.Key == Key.Escape) Close();
            else if (e.Key == Key.Home) next = 0;
            else if (e.Key == Key.End) next = ClampStart(ImageCount);
            else if (e.Key == Key.Left) next--;
            else if (e.Key == Key.Right) next++;
            else if (e.Key == Key.Up) next -= _columns;
            else if (e.Key == Key.Down) next += _columns;
            else if (e.Key == Key.PageUp) next -= PageSize;
            else if (e.Key == Key.PageDown) next += PageSize;
            else if (e.Key == Key.F) { _showNames = !_showNames; _ = LoadPageAsync(_startIndex); }
            else if (e.Key == Key.S) SetRows(_rows + 1);
            else if (e.Key == Key.L) SetRows(_rows - 1);
            else if (e.Key == Key.R || e.Key == Key.T)
            {
                _pageTimer.Interval = TimeSpan.FromSeconds((e.Key == Key.R ? 1 : 2) * _rows);
                _pageTimer.Start();
            }
            else if (e.Key == Key.Q) Close();
            else if (e.Key == Key.F5) _host.Activate();
            else return;
            e.Handled = true;
            if (next != _startIndex)
            {
                _pageTimer.Stop();
                _ = LoadPageAsync(next);
            }
        }

        private void First_Click(object sender, RoutedEventArgs e) => _ = LoadPageAsync(0);
        private void Previous_Click(object sender, RoutedEventArgs e) => _ = LoadPageAsync(_startIndex - PageSize);
        private void Next_Click(object sender, RoutedEventArgs e) => _ = LoadPageAsync(_startIndex + PageSize);
        private void Last_Click(object sender, RoutedEventArgs e) => _ = LoadPageAsync(ImageCount);

        private void Window_Closed(object sender, EventArgs e)
        {
            _closing = true;
            _pageTimer.Stop();
            _loading?.Cancel();
        }
    }
}
