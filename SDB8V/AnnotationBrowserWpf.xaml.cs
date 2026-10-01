#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfPath = System.IO.Path;
using WpfPoint = System.Windows.Point;
using WpfRectangle = System.Windows.Shapes.Rectangle;

namespace SymbolDB
{
    public partial class AnnotationBrowserWpf : Window
    {
        private readonly string _deckFolder;
        private readonly string _deckName;
        private readonly List<AnnotationBrowserItem> _items;

        private BitmapSource _currentBitmap;

        public AnnotationBrowserWpf(
            string deckFolder,
            string deckName,
            IEnumerable<ImageAnnotation> annotations)
        {
            InitializeComponent();

            _deckFolder = WpfPath.GetFullPath(
                deckFolder ?? throw new ArgumentNullException(nameof(deckFolder)));

            _deckName = string.IsNullOrWhiteSpace(deckName)
                ? new DirectoryInfo(_deckFolder).Name
                : deckName;

            _items = (annotations ?? Enumerable.Empty<ImageAnnotation>())
                .OrderBy(annotation => annotation.CardNumber)
                .ThenBy(annotation => annotation.Title)
                .Select(annotation => new AnnotationBrowserItem(annotation))
                .ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeckNameTextBlock.Text = _deckName;
            DeckFolderTextBlock.Text = _deckFolder;
            AnnotationCountTextBlock.Text =
                $"{_items.Count:N0} annotation(s)";

            AnnotationListBox.ItemsSource = _items;

            if (_items.Count > 0)
            {
                AnnotationListBox.SelectedIndex = 0;
                AnnotationListBox.ScrollIntoView(
                    AnnotationListBox.SelectedItem);
            }
            else
            {
                StatusTextBlock.Text =
                    "The currently opened deck has no annotations.";
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            RelatedImage.Source = null;
            AnnotationListBox.ItemsSource = null;
            AnnotationOverlay.Children.Clear();
            _currentBitmap = null;
        }

        private void AnnotationListBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (AnnotationListBox.SelectedItem is not
                AnnotationBrowserItem item)
            {
                ClearDisplay();
                return;
            }

            DisplayAnnotation(item.Annotation);
        }

        private void DisplayAnnotation(ImageAnnotation annotation)
        {
            TitleTextBlock.Text = annotation.Title;
            CommentTextBlock.Text = annotation.Comment;
            KeywordsTextBlock.Text = annotation.Keywords;
            ReferencesTextBlock.Text = annotation.CrossReferences;
            ImagePathTextBlock.Text = annotation.RootRelativePath;

            AnnotationOverlay.Children.Clear();
            RelatedImage.Source = null;
            _currentBitmap = null;

            string imagePath = ResolveImagePath(annotation.RootRelativePath);

            if (imagePath == null || !File.Exists(imagePath))
            {
                MissingImageTextBlock.Text =
                    $"The related image could not be found:\n" +
                    $"{annotation.RootRelativePath}";

                MissingImageTextBlock.Visibility = Visibility.Visible;
                StatusTextBlock.Text =
                    $"Missing image for annotation: {annotation.Title}";
                return;
            }

            try
            {
                _currentBitmap = LoadBitmap(imagePath);
                RelatedImage.Source = _currentBitmap;
                MissingImageTextBlock.Visibility = Visibility.Collapsed;

                StatusTextBlock.Text =
                    $"Card {annotation.CardNumber:N0}: {annotation.Title}";

                RenderAnnotation(annotation);
            }
            catch (Exception ex) when (
                ex is IOException or
                UnauthorizedAccessException or
                ArgumentException or
                NotSupportedException)
            {
                MissingImageTextBlock.Text =
                    $"Unable to load the related image:\n{ex.Message}";

                MissingImageTextBlock.Visibility = Visibility.Visible;
                StatusTextBlock.Text = "The related image could not be loaded.";
            }
        }

        private string ResolveImagePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            string normalizedRelativePath = relativePath.Replace(
                '/',
                WpfPath.DirectorySeparatorChar);

            string fullPath = WpfPath.GetFullPath(
                WpfPath.Combine(_deckFolder, normalizedRelativePath));

            string rootWithSeparator = _deckFolder.TrimEnd(
                WpfPath.DirectorySeparatorChar,
                WpfPath.AltDirectorySeparatorChar)
                + WpfPath.DirectorySeparatorChar;

            if (!fullPath.StartsWith(
                    rootWithSeparator,
                    StringComparison.OrdinalIgnoreCase))
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

        private void PreviewHost_SizeChanged(
            object sender,
            SizeChangedEventArgs e)
        {
            if (AnnotationListBox.SelectedItem is
                AnnotationBrowserItem item)
            {
                RenderAnnotation(item.Annotation);
            }
        }

        private void RenderAnnotation(ImageAnnotation annotation)
        {
            AnnotationOverlay.Children.Clear();

            if (_currentBitmap == null ||
                PreviewHost.ActualWidth <= 0 ||
                PreviewHost.ActualHeight <= 0)
            {
                return;
            }

            double bitmapWidth = _currentBitmap.PixelWidth;
            double bitmapHeight = _currentBitmap.PixelHeight;

            double scale = Math.Min(
                PreviewHost.ActualWidth / bitmapWidth,
                PreviewHost.ActualHeight / bitmapHeight);

            double renderedWidth = bitmapWidth * scale;
            double renderedHeight = bitmapHeight * scale;
            double imageLeft =
                (PreviewHost.ActualWidth - renderedWidth) / 2;
            double imageTop =
                (PreviewHost.ActualHeight - renderedHeight) / 2;

            double sourceWidth = annotation.SourcePixelWidth > 0
                ? annotation.SourcePixelWidth
                : bitmapWidth;

            double sourceHeight = annotation.SourcePixelHeight > 0
                ? annotation.SourcePixelHeight
                : bitmapHeight;

            double scaleX = renderedWidth / sourceWidth;
            double scaleY = renderedHeight / sourceHeight;

            Brush stroke = CreateBrush(annotation.Color);
            const double strokeThickness = 3;

            if (string.Equals(
                    annotation.Shape,
                    "Freehand",
                    StringComparison.OrdinalIgnoreCase) &&
                annotation.Points?.Count >= 2)
            {
                var polyline = new Polyline
                {
                    Stroke = stroke,
                    StrokeThickness = strokeThickness,
                    StrokeLineJoin = PenLineJoin.Round
                };

                foreach (AnnotationPointData point in annotation.Points)
                {
                    polyline.Points.Add(new WpfPoint(
                        imageLeft + point.X * scaleX,
                        imageTop + point.Y * scaleY));
                }

                AnnotationOverlay.Children.Add(polyline);
                return;
            }

            Shape shape;

            if (string.Equals(
                    annotation.Shape,
                    "Ellipse",
                    StringComparison.OrdinalIgnoreCase))
            {
                shape = new Ellipse();
            }
            else
            {
                shape = new WpfRectangle();
            }

            shape.Width = Math.Max(1, annotation.Width * scaleX);
            shape.Height = Math.Max(1, annotation.Height * scaleY);
            shape.Stroke = stroke;
            shape.StrokeThickness = strokeThickness;

            if (string.Equals(
                    annotation.Shape,
                    "Highlight",
                    StringComparison.OrdinalIgnoreCase))
            {
                Color color = ((SolidColorBrush)stroke).Color;
                shape.Fill = new SolidColorBrush(
                    Color.FromArgb(70, color.R, color.G, color.B));
            }
            else
            {
                shape.Fill = Brushes.Transparent;
            }

            Canvas.SetLeft(
                shape,
                imageLeft + annotation.X * scaleX);

            Canvas.SetTop(
                shape,
                imageTop + annotation.Y * scaleY);

            AnnotationOverlay.Children.Add(shape);
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
            catch (FormatException)
            {
            }

            return new SolidColorBrush(Color.FromRgb(78, 145, 232));
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (AnnotationListBox.SelectedIndex > 0)
                SelectAnnotation(AnnotationListBox.SelectedIndex - 1);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (AnnotationListBox.SelectedIndex >= 0 &&
                AnnotationListBox.SelectedIndex < _items.Count - 1)
            {
                SelectAnnotation(AnnotationListBox.SelectedIndex + 1);
            }
        }

        private void SelectAnnotation(int index)
        {
            AnnotationListBox.SelectedIndex = index;
            AnnotationListBox.ScrollIntoView(
                AnnotationListBox.SelectedItem);
        }

        private void ClearDisplay()
        {
            RelatedImage.Source = null;
            AnnotationOverlay.Children.Clear();
            MissingImageTextBlock.Visibility = Visibility.Collapsed;
            _currentBitmap = null;

            TitleTextBlock.Text = string.Empty;
            CommentTextBlock.Text = string.Empty;
            KeywordsTextBlock.Text = string.Empty;
            ReferencesTextBlock.Text = string.Empty;
            ImagePathTextBlock.Text = string.Empty;
        }
    }

    internal sealed class AnnotationBrowserItem
    {
        public AnnotationBrowserItem(ImageAnnotation annotation)
        {
            Annotation = annotation ??
                throw new ArgumentNullException(nameof(annotation));
        }

        public ImageAnnotation Annotation { get; }

        public string Title =>
            string.IsNullOrWhiteSpace(Annotation.Title)
                ? "(Untitled annotation)"
                : Annotation.Title;

        public string CardText => $"Card {Annotation.CardNumber:N0}";

        public string ImageName =>
            WpfPath.GetFileName(Annotation.RootRelativePath);

        public BitmapSource Thumbnail => Annotation.SelectionImage;
    }
}