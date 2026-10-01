#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfPath = System.IO.Path;
using WpfPoint = System.Windows.Point;
using WpfRectangle = System.Windows.Shapes.Rectangle;

namespace SymbolDB
{
    public partial class AnnotationDisplayWpf : Window
    {
        private const double VisualRowTolerance = 0.04;

        private readonly string _deckFolder;
        private readonly string _deckName;
        private readonly string _initialRootRelativePath;
        private readonly List<ImageAnnotation> _annotations;
        private readonly List<string> _cardPaths;

        private List<AnnotationDisplayItem> _currentItems = new();
        private BitmapSource _currentBitmap;
        private string _currentRootRelativePath;
        private int _currentCardIndex = -1;
        private AnnotationOverlayMode _overlayMode =
            AnnotationOverlayMode.All;

        public AnnotationDisplayWpf(
            string deckFolder,
            string deckName,
            string initialRootRelativePath,
            IEnumerable<ImageAnnotation> annotations)
        {
            InitializeComponent();

            _deckFolder = WpfPath.GetFullPath(
                deckFolder ??
                throw new ArgumentNullException(nameof(deckFolder)));

            _deckName = string.IsNullOrWhiteSpace(deckName)
                ? new DirectoryInfo(_deckFolder).Name
                : deckName;

            _initialRootRelativePath =
                NormalizeRootRelativePath(initialRootRelativePath);

            _annotations = (annotations ??
                    Enumerable.Empty<ImageAnnotation>())
                .Where(annotation => annotation != null)
                .ToList();

            _cardPaths = BuildCardPathList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeckNameTextBlock.Text = _deckName;
            DeckFolderTextBlock.Text = _deckFolder;

            if (_cardPaths.Count == 0)
            {
                ClearCardDisplay();
                StatusTextBlock.Text =
                    "The currently opened deck has no annotated cards.";
                UpdateNavigationButtons();
                return;
            }

            _currentCardIndex = FindInitialCardIndex();
            DisplayCard(_currentCardIndex);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            RelatedImage.Source = null;
            //SelectionImage.Source = null;
            AnnotationListBox.ItemsSource = null;
            AnnotationOverlay.Children.Clear();

            _currentBitmap = null;
            _currentItems.Clear();
        }

        private List<string> BuildCardPathList()
        {
            List<string> paths = _annotations
                .Where(annotation =>
                    !string.IsNullOrWhiteSpace(
                        annotation.RootRelativePath))
                .GroupBy(
                    annotation => NormalizeRootRelativePath(
                        annotation.RootRelativePath),
                    StringComparer.OrdinalIgnoreCase)
                .OrderBy(group => group.Min(annotation =>
                    annotation.CardNumber))
                .ThenBy(group => group.Key,
                    StringComparer.CurrentCultureIgnoreCase)
                .Select(group => group.Key)
                .ToList();

            if (!string.IsNullOrWhiteSpace(_initialRootRelativePath) &&
                !paths.Contains(
                    _initialRootRelativePath,
                    StringComparer.OrdinalIgnoreCase))
            {
                paths.Insert(0, _initialRootRelativePath);
            }

            return paths;
        }

        private int FindInitialCardIndex()
        {
            if (!string.IsNullOrWhiteSpace(_initialRootRelativePath))
            {
                int index = _cardPaths.FindIndex(path =>
                    string.Equals(
                        path,
                        _initialRootRelativePath,
                        StringComparison.OrdinalIgnoreCase));

                if (index >= 0)
                    return index;
            }

            return 0;
        }

        private void DisplayCard(int cardIndex)
        {
            if (cardIndex < 0 || cardIndex >= _cardPaths.Count)
                return;

            _currentCardIndex = cardIndex;
            _currentRootRelativePath = _cardPaths[cardIndex];

            AnnotationOverlay.Children.Clear();
            RelatedImage.Source = null;
            //SelectionImage.Source = null;
            _currentBitmap = null;

            MissingImageTextBlock.Visibility = Visibility.Collapsed;

            List<ImageAnnotation> cardAnnotations = _annotations
                .Where(annotation =>
                    string.Equals(
                        NormalizeRootRelativePath(
                            annotation.RootRelativePath),
                        _currentRootRelativePath,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            _currentItems = CreateSpatiallySortedItems(cardAnnotations);

            AnnotationListBox.ItemsSource = null;
            AnnotationListBox.ItemsSource = _currentItems;

            AnnotationCountTextBlock.Text =
                $"{_currentItems.Count:N0} annotation" +
                (_currentItems.Count == 1 ? string.Empty : "s");

            CardPositionTextBlock.Text =
                $"Card {_currentCardIndex + 1:N0} of {_cardPaths.Count:N0}";

            LoadCurrentImage();

            if (_currentItems.Count > 0)
            {
                AnnotationListBox.SelectedIndex = 0;
                AnnotationListBox.ScrollIntoView(
                    AnnotationListBox.SelectedItem);
            }
            else
            {
                ClearAnnotationDetails();
                RenderOverlays();
                StatusTextBlock.Text =
                    $"{WpfPath.GetFileName(_currentRootRelativePath)} " +
                    "has no annotations.";
            }

            UpdateNavigationButtons();
        }

        private List<AnnotationDisplayItem> CreateSpatiallySortedItems(
            IEnumerable<ImageAnnotation> annotations)
        {
            List<SpatialAnnotation> source = annotations
                .Select(annotation =>
                {
                    NormalizedBounds bounds =
                        GetNormalizedBounds(annotation);

                    return new SpatialAnnotation(
                        annotation,
                        bounds.CenterX,
                        bounds.CenterY);
                })
                .OrderBy(item => item.CenterY)
                .ThenBy(item => item.CenterX)
                .ToList();

            var rows = new List<List<SpatialAnnotation>>();

            foreach (SpatialAnnotation item in source)
            {
                List<SpatialAnnotation> row = rows.LastOrDefault();

                if (row == null ||
                    Math.Abs(item.CenterY - AverageCenterY(row)) >
                    VisualRowTolerance)
                {
                    row = new List<SpatialAnnotation>();
                    rows.Add(row);
                }

                row.Add(item);
            }

            var result = new List<AnnotationDisplayItem>();
            int sequence = 1;

            foreach (List<SpatialAnnotation> row in rows)
            {
                foreach (SpatialAnnotation item in
                         row.OrderBy(value => value.CenterX))
                {
                    result.Add(new AnnotationDisplayItem(
                        item.Annotation,
                        sequence++,
                        item.CenterX,
                        item.CenterY));
                }
            }

            return result;
        }

        private static double AverageCenterY(
            IEnumerable<SpatialAnnotation> row)
        {
            return row.Average(item => item.CenterY);
        }

        private static NormalizedBounds GetNormalizedBounds(
            ImageAnnotation annotation)
        {
            if (annotation.GeometryVersion >= 2 &&
                annotation.NormalizedWidth > 0 &&
                annotation.NormalizedHeight > 0)
            {
                return new NormalizedBounds(
                    ClampUnit(annotation.NormalizedX),
                    ClampUnit(annotation.NormalizedY),
                    ClampUnit(annotation.NormalizedWidth),
                    ClampUnit(annotation.NormalizedHeight));
            }

            double sourceWidth = annotation.SourcePixelWidth > 0
                ? annotation.SourcePixelWidth
                : Math.Max(1, annotation.X + annotation.Width);

            double sourceHeight = annotation.SourcePixelHeight > 0
                ? annotation.SourcePixelHeight
                : Math.Max(1, annotation.Y + annotation.Height);

            return new NormalizedBounds(
                ClampUnit(annotation.X / sourceWidth),
                ClampUnit(annotation.Y / sourceHeight),
                ClampUnit(annotation.Width / sourceWidth),
                ClampUnit(annotation.Height / sourceHeight));
        }

        private void LoadCurrentImage()
        {
            string imagePath = ResolveImagePath(
                _currentRootRelativePath);

            if (imagePath == null || !File.Exists(imagePath))
            {
                MissingImageTextBlock.Text =
                    "The related image could not be found:\n" +
                    _currentRootRelativePath;

                MissingImageTextBlock.Visibility = Visibility.Visible;
                StatusTextBlock.Text =
                    "The related image could not be found.";
                return;
            }

            try
            {
                _currentBitmap = LoadBitmap(imagePath);
                RelatedImage.Source = _currentBitmap;
                MissingImageTextBlock.Visibility = Visibility.Collapsed;
                RenderOverlays();
            }
            catch (Exception ex) when (
                ex is IOException or
                UnauthorizedAccessException or
                ArgumentException or
                NotSupportedException)
            {
                MissingImageTextBlock.Text =
                    "Unable to load the related image:\n" +
                    ex.Message;

                MissingImageTextBlock.Visibility = Visibility.Visible;
                StatusTextBlock.Text =
                    "The related image could not be loaded.";
            }
        }

        private string ResolveImagePath(string rootRelativePath)
        {
            if (string.IsNullOrWhiteSpace(rootRelativePath))
                return null;

            string relativePath = rootRelativePath.Replace(
                '/',
                WpfPath.DirectorySeparatorChar);

            string fullPath = WpfPath.GetFullPath(
                WpfPath.Combine(_deckFolder, relativePath));

            string relativeCheck = WpfPath.GetRelativePath(
                _deckFolder,
                fullPath);

            if (WpfPath.IsPathRooted(relativeCheck) ||
                relativeCheck.Equals(
                    "..",
                    StringComparison.Ordinal) ||
                relativeCheck.StartsWith(
                    ".." + WpfPath.DirectorySeparatorChar,
                    StringComparison.Ordinal))
            {
                return null;
            }

            return fullPath;
        }

        private static BitmapSource LoadBitmap(string imagePath)
        {
            using var stream = new FileStream(
                imagePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        private void AnnotationListBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (AnnotationListBox.SelectedItem is not
                AnnotationDisplayItem item)
            {
                ClearAnnotationDetails();
                RenderOverlays();
                UpdateNavigationButtons();
                return;
            }

            DisplayAnnotationDetails(item);
            RenderOverlays();
            UpdateNavigationButtons();
        }

        private void DisplayAnnotationDetails(
            AnnotationDisplayItem item)
        {
            ImageAnnotation annotation = item.Annotation;

            //SelectionImage.Source = annotation.SelectionImage;
            TitleTextBlock.Text = annotation.Title ?? string.Empty;
            CommentTextBlock.Text = annotation.Comment ?? string.Empty;
            KeywordsTextBlock.Text = annotation.Keywords ?? string.Empty;
            ReferencesTextBlock.Text =
                annotation.CrossReferences ?? string.Empty;
            ShapeTextBlock.Text = annotation.Shape ?? string.Empty;
            CardNumberTextBlock.Text =
                annotation.CardNumber.ToString("N0");
            ImagePathTextBlock.Text =
                annotation.RootRelativePath ?? string.Empty;
            CreatedTextBlock.Text =
                annotation.CreatedUtc.ToLocalTime().ToString("g");
            ModifiedTextBlock.Text =
                annotation.ModifiedUtc.ToLocalTime().ToString("g");
            AnnotationIdTextBlock.Text =
                annotation.AnnotationId.ToString();

            int index = AnnotationListBox.SelectedIndex;

            StatusTextBlock.Text =
                $"Annotation {index + 1:N0} of {_currentItems.Count:N0}: " +
                item.Title;
        }

        private void ClearAnnotationDetails()
        {
            //SelectionImage.Source = null;
            TitleTextBlock.Text = string.Empty;
            CommentTextBlock.Text = string.Empty;
            KeywordsTextBlock.Text = string.Empty;
            ReferencesTextBlock.Text = string.Empty;
            ShapeTextBlock.Text = string.Empty;
            CardNumberTextBlock.Text = string.Empty;
            ImagePathTextBlock.Text = string.Empty;
            CreatedTextBlock.Text = string.Empty;
            ModifiedTextBlock.Text = string.Empty;
            AnnotationIdTextBlock.Text = string.Empty;
        }

        private void PreviewHost_SizeChanged(
            object sender,
            SizeChangedEventArgs e)
        {
            RenderOverlays();
        }

        private void RenderOverlays()
        {
            if (!IsInitialized || AnnotationOverlay == null)
            {
                return;
            }

            AnnotationOverlay.Children.Clear();

            if (_currentBitmap == null ||
                _overlayMode == AnnotationOverlayMode.Hidden ||
                PreviewHost.ActualWidth <= 0 ||
                PreviewHost.ActualHeight <= 0)
            {
                return;
            }

            AnnotationDisplayItem selectedItem =
                AnnotationListBox.SelectedItem as AnnotationDisplayItem;

            IEnumerable<AnnotationDisplayItem> items =
                _overlayMode == AnnotationOverlayMode.SelectedOnly
                    ? _currentItems.Where(item =>
                        item == selectedItem)
                    : _currentItems
                        .Where(item => item != selectedItem)
                        .Concat(_currentItems.Where(item =>
                            item == selectedItem));

            foreach (AnnotationDisplayItem item in items)
            {
                DrawOverlay(
                    item,
                    item == selectedItem);
            }
        }

        private void DrawOverlay(
            AnnotationDisplayItem item,
            bool selected)
        {
            ImageAnnotation annotation = item.Annotation;
            Rect imageRect = GetRenderedImageRect();

            if (imageRect.IsEmpty)
                return;

            NormalizedBounds bounds =
                GetNormalizedBounds(annotation);

            Rect displayBounds = new(
                imageRect.Left + bounds.X * imageRect.Width,
                imageRect.Top + bounds.Y * imageRect.Height,
                Math.Max(2, bounds.Width * imageRect.Width),
                Math.Max(2, bounds.Height * imageRect.Height));

            SolidColorBrush stroke =
                CreateBrush(annotation.Color);

            AddHitTarget(item, displayBounds, selected);

            if (string.Equals(
                    annotation.Shape,
                    "Freehand",
                    StringComparison.OrdinalIgnoreCase))
            {
                DrawFreehand(
                    item,
                    imageRect,
                    displayBounds,
                    stroke,
                    selected);
            }
            else
            {
                DrawShape(
                    item,
                    displayBounds,
                    stroke,
                    selected);
            }

            DrawSequenceMarker(
                item,
                displayBounds,
                stroke,
                selected);
        }

        private void AddHitTarget(
            AnnotationDisplayItem item,
            Rect bounds,
            bool selected)
        {
            var hitTarget = new WpfRectangle
            {
                Width = Math.Max(14, bounds.Width),
                Height = Math.Max(14, bounds.Height),
                Fill = Brushes.Transparent,
                Stroke = Brushes.Transparent,
                Cursor = Cursors.Hand,
                Tag = item
            };

            hitTarget.MouseLeftButtonDown +=
                Overlay_MouseLeftButtonDown;

            Canvas.SetLeft(hitTarget, bounds.Left);
            Canvas.SetTop(hitTarget, bounds.Top);
            Canvas.SetZIndex(hitTarget, selected ? 30 : 10);

            AnnotationOverlay.Children.Add(hitTarget);
        }

        private void DrawShape(
            AnnotationDisplayItem item,
            Rect bounds,
            SolidColorBrush stroke,
            bool selected)
        {
            Shape shape = string.Equals(
                    item.Annotation.Shape,
                    "Ellipse",
                    StringComparison.OrdinalIgnoreCase)
                ? new Ellipse()
                : new WpfRectangle();

            shape.Width = bounds.Width;
            shape.Height = bounds.Height;
            shape.Stroke = stroke;
            shape.StrokeThickness = selected ? 4 : 2;
            shape.IsHitTestVisible = false;

            if (string.Equals(
                    item.Annotation.Shape,
                    "Highlight",
                    StringComparison.OrdinalIgnoreCase))
            {
                Color color = stroke.Color;
                shape.Fill = new SolidColorBrush(
                    Color.FromArgb(
                        selected ? (byte)90 : (byte)45,
                        color.R,
                        color.G,
                        color.B));
            }
            else
            {
                shape.Fill = Brushes.Transparent;
            }

            if (!selected)
                shape.Opacity = 0.72;

            Canvas.SetLeft(shape, bounds.Left);
            Canvas.SetTop(shape, bounds.Top);
            Canvas.SetZIndex(shape, selected ? 22 : 2);

            AnnotationOverlay.Children.Add(shape);
        }

        private void DrawFreehand(
            AnnotationDisplayItem item,
            Rect imageRect,
            Rect displayBounds,
            SolidColorBrush stroke,
            bool selected)
        {
            List<WpfPoint> points =
                GetNormalizedPoints(item.Annotation)
                    .Select(point => new WpfPoint(
                        imageRect.Left +
                        point.X * imageRect.Width,
                        imageRect.Top +
                        point.Y * imageRect.Height))
                    .ToList();

            if (points.Count < 2)
            {
                DrawShape(
                    item,
                    displayBounds,
                    stroke,
                    selected);
                return;
            }

            var polyline = new Polyline
            {
                Stroke = stroke,
                StrokeThickness = selected ? 4 : 2,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                IsHitTestVisible = false,
                Opacity = selected ? 1.0 : 0.72
            };

            foreach (WpfPoint point in points)
                polyline.Points.Add(point);

            Canvas.SetZIndex(polyline, selected ? 22 : 2);
            AnnotationOverlay.Children.Add(polyline);
        }

        private static List<WpfPoint> GetNormalizedPoints(
            ImageAnnotation annotation)
        {
            if (annotation.NormalizedPoints?.Count >= 2)
            {
                return annotation.NormalizedPoints
                    .Select(point => new WpfPoint(
                        ClampUnit(point.X),
                        ClampUnit(point.Y)))
                    .ToList();
            }

            if (annotation.Points?.Count < 2)
                return new List<WpfPoint>();

            double sourceWidth = annotation.SourcePixelWidth > 0
                ? annotation.SourcePixelWidth
                : Math.Max(
                    1,
                    annotation.Points.Max(point => point.X));

            double sourceHeight = annotation.SourcePixelHeight > 0
                ? annotation.SourcePixelHeight
                : Math.Max(
                    1,
                    annotation.Points.Max(point => point.Y));

            return annotation.Points
                .Select(point => new WpfPoint(
                    ClampUnit(point.X / sourceWidth),
                    ClampUnit(point.Y / sourceHeight)))
                .ToList();
        }

        private void DrawSequenceMarker(
            AnnotationDisplayItem item,
            Rect bounds,
            SolidColorBrush color,
            bool selected)
        {
            var text = new TextBlock
            {
                Text = item.Sequence.ToString(),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var marker = new Border
            {
                Width = selected ? 28 : 24,
                Height = selected ? 28 : 24,
                CornerRadius = new CornerRadius(14),
                Background = color,
                BorderBrush = selected
                    ? Brushes.White
                    : Brushes.Transparent,
                BorderThickness = selected
                    ? new Thickness(2)
                    : new Thickness(0),
                Child = text,
                Cursor = Cursors.Hand,
                Tag = item
            };

            marker.MouseLeftButtonDown +=
                Overlay_MouseLeftButtonDown;

            Canvas.SetLeft(
                marker,
                Math.Max(0, bounds.Left - marker.Width / 2));

            Canvas.SetTop(
                marker,
                Math.Max(0, bounds.Top - marker.Height / 2));

            Canvas.SetZIndex(marker, selected ? 40 : 20);
            AnnotationOverlay.Children.Add(marker);
        }

        private void Overlay_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element &&
                element.Tag is AnnotationDisplayItem item)
            {
                AnnotationListBox.SelectedItem = item;
                AnnotationListBox.ScrollIntoView(item);
                e.Handled = true;
            }
        }

        private Rect GetRenderedImageRect()
        {
            if (_currentBitmap == null ||
                PreviewHost.ActualWidth <= 0 ||
                PreviewHost.ActualHeight <= 0)
            {
                return Rect.Empty;
            }

            double imageWidth = _currentBitmap.PixelWidth;
            double imageHeight = _currentBitmap.PixelHeight;

            double scale = Math.Min(
                PreviewHost.ActualWidth / imageWidth,
                PreviewHost.ActualHeight / imageHeight);

            double renderedWidth = imageWidth * scale;
            double renderedHeight = imageHeight * scale;

            return new Rect(
                (PreviewHost.ActualWidth - renderedWidth) / 2,
                (PreviewHost.ActualHeight - renderedHeight) / 2,
                renderedWidth,
                renderedHeight);
        }

        private void OverlayModeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (OverlayModeComboBox.SelectedItem is
                    ComboBoxItem selectedItem &&
                Enum.TryParse(
                    selectedItem.Tag?.ToString(),
                    out AnnotationOverlayMode mode))
            {
                _overlayMode = mode;
                RenderOverlays();
            }
        }

        private void PreviousCard_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_currentCardIndex > 0)
                DisplayCard(_currentCardIndex - 1);
        }

        private void NextCard_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_currentCardIndex >= 0 &&
                _currentCardIndex < _cardPaths.Count - 1)
            {
                DisplayCard(_currentCardIndex + 1);
            }
        }

        private void PreviousAnnotation_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectAnnotation(
                AnnotationListBox.SelectedIndex - 1);
        }

        private void NextAnnotation_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectAnnotation(
                AnnotationListBox.SelectedIndex + 1);
        }

        private void SelectAnnotation(int index)
        {
            if (index < 0 || index >= _currentItems.Count)
                return;

            AnnotationListBox.SelectedIndex = index;
            AnnotationListBox.ScrollIntoView(
                AnnotationListBox.SelectedItem);
        }

        private void Window_PreviewKeyDown(
            object sender,
            KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    SelectAnnotation(
                        AnnotationListBox.SelectedIndex - 1);
                    e.Handled = true;
                    break;

                case Key.Down:
                    SelectAnnotation(
                        AnnotationListBox.SelectedIndex + 1);
                    e.Handled = true;
                    break;

                case Key.Home:
                    SelectAnnotation(0);
                    e.Handled = true;
                    break;

                case Key.End:
                    SelectAnnotation(_currentItems.Count - 1);
                    e.Handled = true;
                    break;

                case Key.PageUp:
                    if (_currentCardIndex > 0)
                        DisplayCard(_currentCardIndex - 1);

                    e.Handled = true;
                    break;

                case Key.PageDown:
                    if (_currentCardIndex <
                        _cardPaths.Count - 1)
                    {
                        DisplayCard(_currentCardIndex + 1);
                    }

                    e.Handled = true;
                    break;
            }
        }

        private void UpdateNavigationButtons()
        {
            PreviousCardButton.IsEnabled =
                _currentCardIndex > 0;

            NextCardButton.IsEnabled =
                _currentCardIndex >= 0 &&
                _currentCardIndex < _cardPaths.Count - 1;

            int annotationIndex =
                AnnotationListBox.SelectedIndex;

            PreviousAnnotationButton.IsEnabled =
                annotationIndex > 0;

            NextAnnotationButton.IsEnabled =
                annotationIndex >= 0 &&
                annotationIndex < _currentItems.Count - 1;
        }

        private void ClearCardDisplay()
        {
            RelatedImage.Source = null;
            //SelectionImage.Source = null;
            AnnotationListBox.ItemsSource = null;
            AnnotationOverlay.Children.Clear();

            _currentBitmap = null;
            _currentItems.Clear();

            AnnotationCountTextBlock.Text = "0 annotations";
            CardPositionTextBlock.Text = string.Empty;
            MissingImageTextBlock.Visibility = Visibility.Collapsed;

            ClearAnnotationDetails();
        }

        private static string NormalizeRootRelativePath(
            string rootRelativePath)
        {
            return (rootRelativePath ?? string.Empty)
                .Replace('\\', '/')
                .TrimStart('/');
        }

        private static double ClampUnit(double value)
        {
            return Math.Clamp(value, 0.0, 1.0);
        }

        private static SolidColorBrush CreateBrush(string value)
        {
            try
            {
                object converted = ColorConverter.ConvertFromString(
                    string.IsNullOrWhiteSpace(value)
                        ? "#4E91E8"
                        : value);

                if (converted is Color color)
                    return new SolidColorBrush(color);
            }
            catch (Exception ex) when (
                ex is FormatException or
                NotSupportedException)
            {
            }

            return new SolidColorBrush(
                Color.FromRgb(78, 145, 232));
        }

        private enum AnnotationOverlayMode
        {
            All,
            SelectedOnly,
            Hidden
        }

        private sealed class SpatialAnnotation
        {
            public SpatialAnnotation(
                ImageAnnotation annotation,
                double centerX,
                double centerY)
            {
                Annotation = annotation;
                CenterX = centerX;
                CenterY = centerY;
            }

            public ImageAnnotation Annotation { get; }

            public double CenterX { get; }

            public double CenterY { get; }
        }

        private readonly struct NormalizedBounds
        {
            public NormalizedBounds(
                double x,
                double y,
                double width,
                double height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public double X { get; }

            public double Y { get; }

            public double Width { get; }

            public double Height { get; }

            public double CenterX => X + Width / 2;

            public double CenterY => Y + Height / 2;
        }
    }

    internal sealed class AnnotationDisplayItem
    {
        public AnnotationDisplayItem(
            ImageAnnotation annotation,
            int sequence,
            double centerX,
            double centerY)
        {
            Annotation = annotation ??
                throw new ArgumentNullException(nameof(annotation));

            Sequence = sequence;
            CenterX = centerX;
            CenterY = centerY;
        }

        public ImageAnnotation Annotation { get; }

        public int Sequence { get; }

        public double CenterX { get; }

        public double CenterY { get; }

        public string Title =>
            string.IsNullOrWhiteSpace(Annotation.Title)
                ? "(Untitled annotation)"
                : Annotation.Title;

        public string CommentPreview
        {
            get
            {
                string text = Annotation.Comment ?? string.Empty;

                return string.Join(
                    " ",
                    text.Split(
                        new[] { '\r', '\n' },
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries));
            }
        }

        public string PositionText =>
            $"Position: {CenterY:P0} down, {CenterX:P0} across";

        public BitmapSource Thumbnail =>
            Annotation.SelectionImage;
    }
}