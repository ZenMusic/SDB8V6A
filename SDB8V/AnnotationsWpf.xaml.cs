#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static SymbolDB.ImageAnnotation;
using Path = System.IO.Path;
using WinForms = System.Windows.Forms;
using WpfPoint = System.Windows.Point;
using WpfRectangle = System.Windows.Shapes.Rectangle;

namespace SymbolDB
{
    public partial class AnnotationsWpf : Window
    {
        private static readonly HashSet<string> ImageExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".bmp", ".gif", ".jpeg", ".jpg", ".png",
                ".tif", ".tiff", ".webp"
            };

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private readonly GlobalVars _gv;
        private readonly List<string> _deckImages = new();

        private AnnotatedDeckInfo _deck = new();
        private ImageAnnotation _editingAnnotation;
        private BitmapSource _currentBitmap;

        private string _deckFolder;
        private int _currentIndex;
        private AnnotationTool _activeTool = AnnotationTool.Rectangle;

        private bool _dragging;
        private WpfPoint _dragStart;
        private WpfPoint _dragCurrent;
        private readonly List<WpfPoint> _freehandDisplayPoints = new();

        public bool bUseFileStrip = false;
        private bool _useNavigation;
        private NavigationWpf _navigationWindow;

        public AnnotationsWpf(GlobalVars gv)
        {
            InitializeComponent();
            _gv = gv ?? throw new ArgumentNullException(nameof(gv));
        }

        public static AnnotationsWpf ShowFromWinForms(
            GlobalVars gv,
            WinForms.Form owner)
        {
            var window = new AnnotationsWpf(gv);

            if (owner != null)
                new WindowInteropHelper(window).Owner = owner.Handle;

            ElementHost.EnableModelessKeyboardInterop(window);
            window.Show();
            return window;
        }
        private void DeckDetails_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_deckFolder) ||
                !Directory.Exists(_deckFolder))
            {
                StatusTextBlock.Text = "Open a deck before editing its details.";
                return;
            }

            string version = PromptForDeckVersion();

            if (string.IsNullOrWhiteSpace(version))
            {
                StatusTextBlock.Text = "Deck details were not changed.";
                return;
            }

            _deck.Version = version.Trim();
            SaveDeckInfo();

            StatusTextBlock.Text =
                $"Deck details updated: {_deck.Name}, version {_deck.Version}.";
        }
        private string CurrentImagePath =>
            _currentIndex >= 0 && _currentIndex < _deckImages.Count
                ? _deckImages[_currentIndex]
                : null;

        private string CurrentRootRelativePath =>
            CurrentImagePath == null || string.IsNullOrWhiteSpace(_deckFolder)
                ? string.Empty
                : ToRootRelativePath(CurrentImagePath);

        private IEnumerable<ImageAnnotation> CurrentAnnotations =>
            _deck.Annotations.Where(annotation =>
                string.Equals(
                    annotation.RootRelativePath,
                    CurrentRootRelativePath,
                    StringComparison.OrdinalIgnoreCase));

        private string DeckInfoFilePath =>
    Path.Combine(
        _deckFolder ?? string.Empty,
        $"{tarotDeckName}.json");

        private string AnnotationFilePath =>
            Path.Combine(
                _deckFolder ?? string.Empty,
                $"{tarotDeckName}annotations.json");

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _useNavigation = bUseFileStrip;

            string initialFolder = GetInitialDeckFolder();

            if (!string.IsNullOrWhiteSpace(initialFolder) &&
                Directory.Exists(initialFolder))
            {
                OpenDeck(initialFolder);
            }

            Activate();
            Focus();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _navigationWindow?.Close();
            _navigationWindow = null;
            MainImage.Source = null;
            AnnotationItemsControl.ItemsSource = null;
            SearchResultsListBox.ItemsSource = null;
        }

        private string GetInitialDeckFolder()
        {
            var list = _gv.imageFileList1?.finfoList;
            if (list == null || list.Count == 0)
                return null;

            int index = Math.Clamp(_gv.nextIdx, 0, list.Count - 1);
            string path = list[index]?.fpath;

            return string.IsNullOrWhiteSpace(path)
                ? null
                : Path.GetDirectoryName(path);
        }

        private void OpenDeck_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new WinForms.FolderBrowserDialog
            {
                Description =
                    "Select the unique folder containing this deck's images.",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
                SelectedPath = Directory.Exists(_deckFolder)
                    ? _deckFolder
                    : string.Empty
            };

            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
                OpenDeck(dialog.SelectedPath);
        }
        string tarotDeckName;
        private void OpenDeck(string folder)
        {
            try
            {
                string fullFolder = Path.GetFullPath(folder);

                if (!Directory.Exists(fullFolder))
                    throw new DirectoryNotFoundException(fullFolder);

                List<string> images = Directory
                    .EnumerateFiles(fullFolder, "*", SearchOption.TopDirectoryOnly)
                    .Where(path => ImageExtensions.Contains(Path.GetExtension(path)))
                    .OrderBy(path => Path.GetFileName(path),
                        StringComparer.CurrentCultureIgnoreCase)
                    .ToList();

                _deckFolder = fullFolder;
                tarotDeckName = new DirectoryInfo(_deckFolder).Name;

                _deckImages.Clear();
                _deckImages.AddRange(images);
                _navigationWindow?.Close();
                _navigationWindow = null;

                if (!LoadDeckAnnotations())
                    return;

                if (string.Equals(_deck.Type, "Tarot", StringComparison.OrdinalIgnoreCase))
                {
                    bUseFileStrip = true;
                    _useNavigation = true;
                }

                DeckFolderTextBox.Text = _deckFolder;
                DeckNameTextBlock.Text = _deck.Name;


                _currentIndex = FindInitialImageIndex();
                if (_useNavigation && _navigationWindow == null)
                    RefreshNavigation();
                ShowImage(_currentIndex);

                StatusTextBlock.Text =
                    $"Deck opened: {_deck.Name}. {_deckImages.Count:N0} image(s).";
            }
            catch (Exception ex) when (
                ex is IOException or
                UnauthorizedAccessException or
                ArgumentException or
                NotSupportedException)
            {
                StatusTextBlock.Text = $"Unable to open deck: {ex.Message}";
            }
        }
        private bool LoadDeckAnnotations()
        {
            bool saveDeckInfo = false;

            if (File.Exists(DeckInfoFilePath))
            {
                try
                {
                    string json = File.ReadAllText(DeckInfoFilePath);

                    _deck = JsonSerializer.Deserialize<AnnotatedDeckInfo>(
                                json,
                                JsonOptions)
                            ?? new AnnotatedDeckInfo();
                }
                catch (JsonException ex)
                {
                    StatusTextBlock.Text =
                        $"Deck information file is invalid: {ex.Message}";
                    return false;
                }
            }
            else
            {
                _deck = new AnnotatedDeckInfo
                {
                    DeckId = Guid.NewGuid(),
                    Name = tarotDeckName
                };

                saveDeckInfo = true;
            }

            if (_deck.DeckId == Guid.Empty)
            {
                _deck.DeckId = Guid.NewGuid();
                saveDeckInfo = true;
            }

            if (string.IsNullOrWhiteSpace(_deck.Name))
            {
                _deck.Name = tarotDeckName;
                saveDeckInfo = true;
            }

            if (string.IsNullOrWhiteSpace(_deck.Version))
            {
                string version = PromptForDeckVersion();

                if (string.IsNullOrWhiteSpace(version))
                {
                    StatusTextBlock.Text =
                        "The deck was not opened because a version is required.";
                    return false;
                }

                _deck.Version = version.Trim();
                saveDeckInfo = true;
            }

            if (saveDeckInfo)
                SaveDeckInfo();

            if (File.Exists(AnnotationFilePath))
            {
                try
                {
                    string json = File.ReadAllText(AnnotationFilePath);

                    _deck.Annotations =
                        JsonSerializer.Deserialize<List<ImageAnnotation>>(
                            json,
                            JsonOptions)
                        ?? new List<ImageAnnotation>();
                }
                catch (JsonException ex)
                {
                    StatusTextBlock.Text =
                        $"Annotation file is invalid: {ex.Message}";
                    return false;
                }
            }
            else
            {
                _deck.Annotations = new List<ImageAnnotation>();
                SaveAnnotations();
            }

            // Synchronize the deck card count to the number of image files.
            _deck.Count = _deckImages.Count;

            // Apply default suit indices if needed.
            if (_deck.Count == 78)
            {
                if (_deck.WandsIndex == 0) _deck.WandsIndex = 22;
                if (_deck.CupsIndex == 0) _deck.CupsIndex = 36;
                if (_deck.SwordsIndex == 0) _deck.SwordsIndex = 50;
                if (_deck.PentaclesIndex == 0) _deck.PentaclesIndex = 64;
            }

            return true;
        }


        private void SaveDeckInfo()
        {
            if (string.IsNullOrWhiteSpace(_deckFolder))
                return;

            _deck.ModifiedUtc = DateTime.UtcNow;
            WriteJsonAtomically(DeckInfoFilePath, _deck);
        }

        private void SaveAnnotations()
        {
            if (string.IsNullOrWhiteSpace(_deckFolder))
                return;

            _deck.Annotations ??= new List<ImageAnnotation>();

            WriteJsonAtomically(
                AnnotationFilePath,
                _deck.Annotations);
        }

        private static void WriteJsonAtomically<T>(
            string filePath,
            T value)
        {
            string json = JsonSerializer.Serialize(value, JsonOptions);
            string temporaryPath = filePath + ".tmp";

            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, filePath, true);
        }

        private int FindInitialImageIndex()
        {
            var list = _gv.imageFileList1?.finfoList;
            if (list == null || list.Count == 0 || _deckImages.Count == 0)
                return 0;

            int globalIndex = Math.Clamp(_gv.nextIdx, 0, list.Count - 1);
            string currentPath = list[globalIndex]?.fpath;

            int deckIndex = _deckImages.FindIndex(path =>
                string.Equals(
                    Path.GetFullPath(path),
                    Path.GetFullPath(currentPath ?? string.Empty),
                    StringComparison.OrdinalIgnoreCase));

            return deckIndex < 0 ? 0 : deckIndex;
        }
        private void NavigateSuit(int direction)
        {
            if (_deckImages.Count == 0)
                return;

            int[] suitStartIndices =
            {
        0,
        _deck.WandsIndex,
        _deck.CupsIndex,
        _deck.SwordsIndex,
        _deck.PentaclesIndex
    };

            int[] validSuitStartIndices = suitStartIndices
                .Where(index => index >= 0 && index < _deckImages.Count)
                .Distinct()
                .OrderBy(index => index)
                .ToArray();

            int currentSuitPosition = Array.FindLastIndex(
                validSuitStartIndices,
                index => index <= _currentIndex);

            int targetSuitPosition = currentSuitPosition + direction;

            if (targetSuitPosition < 0)
            {
                StatusTextBlock.Text = "Already at the first suit.";
                return;
            }

            if (targetSuitPosition >= validSuitStartIndices.Length)
            {
                StatusTextBlock.Text = "Already at the last suit.";
                return;
            }

            ShowImage(validSuitStartIndices[targetSuitPosition]);
        }
        private string PromptForDeckVersion()
        {
            var versionTextBox = new TextBox { Text = _deck.Version ?? string.Empty, MinWidth = 300, Margin = new Thickness(0, 4, 0, 12) };

            var typeTextBox = new TextBox
            {
                Text = string.IsNullOrWhiteSpace(_deck.Type) ? "unknown" : _deck.Type,
                Width = 170,
                Margin = new Thickness(0, 4, 12, 12)
            };

            var TypeTarotCheckBox = new CheckBox
            {
                Content = "Tarot",
                IsChecked = string.Equals(typeTextBox.Text, "Tarot", StringComparison.OrdinalIgnoreCase),
                Margin = new Thickness(0, 4, 10, 12)
            };
            var TypeCollectionCheckBox = new CheckBox
            {
                Content = "Collection",
                IsChecked = string.Equals(typeTextBox.Text, "Collection", StringComparison.OrdinalIgnoreCase),
                Margin = new Thickness(0, 4, 10, 12)
            };
            var TypeFolderCheckBox = new CheckBox
            {
                Content = "Folder",
                IsChecked = string.Equals(typeTextBox.Text, "Folder", StringComparison.OrdinalIgnoreCase),
                Margin = new Thickness(0, 4, 0, 12)
            };

            TypeTarotCheckBox.Checked += (_, _) =>
            {
                TypeCollectionCheckBox.IsChecked = false;
                TypeFolderCheckBox.IsChecked = false;
                typeTextBox.Text = "Tarot";
            };
            TypeCollectionCheckBox.Checked += (_, _) =>
            {
                TypeTarotCheckBox.IsChecked = false;
                TypeFolderCheckBox.IsChecked = false;
                typeTextBox.Text = "Collection";
            };
            TypeFolderCheckBox.Checked += (_, _) =>
            {
                TypeTarotCheckBox.IsChecked = false;
                TypeCollectionCheckBox.IsChecked = false;
                typeTextBox.Text = "Folder";
            };

            var typePanel = new StackPanel { Orientation = Orientation.Horizontal };
            typePanel.Children.Add(typeTextBox);
            typePanel.Children.Add(TypeTarotCheckBox);
            typePanel.Children.Add(TypeCollectionCheckBox);
            typePanel.Children.Add(TypeFolderCheckBox);


            var descriptionTextBox = new TextBox { Text = _deck.Description ?? string.Empty, MinWidth = 300, MinHeight = 80, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Margin = new Thickness(0, 4, 0, 12) };
            var okButton = new Button { Content = "Save Deck Details", IsDefault = true, MinWidth = 120 };
            var cancelButton = new Button { Content = "Cancel", IsCancel = true, MinWidth = 80 };

            // Create and keep references to suit index textboxes
            var wandsIndexTextBox = CreateWandsIndexTextBox();
            var cupsIndexTextBox = CreateCupsIndexTextBox();
            var swordsIndexTextBox = CreateSwordsIndexTextBox();
            var pentaclesIndexTextBox = CreatePentaclesIndexTextBox();

            var standardDeckCheckBox = new CheckBox
            {
                Content = "Standard Deck",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 4, 0, 12)
            };

            standardDeckCheckBox.Checked += (_, _) =>
            {
                wandsIndexTextBox.Text = "22";
                cupsIndexTextBox.Text = "36";
                swordsIndexTextBox.Text = "50";
                pentaclesIndexTextBox.Text = "64";
            };

            var wandsPanel = new StackPanel { Orientation = Orientation.Horizontal };
            wandsPanel.Children.Add(wandsIndexTextBox);
            wandsPanel.Children.Add(standardDeckCheckBox);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            var contentPanel = new StackPanel { Margin = new Thickness(18) };
            contentPanel.Children.Add(CreatePromptLabel("Full path"));
            contentPanel.Children.Add(CreatePromptValue(DeckInfoFilePath));
            
            contentPanel.Children.Add(CreatePromptLabel("Tarot deck name"));
            contentPanel.Children.Add(CreatePromptValue(tarotDeckName));
            contentPanel.Children.Add(CreatePromptLabel("Type"));
            contentPanel.Children.Add(typePanel);

            contentPanel.Children.Add(CreatePromptLabel("Deck ID"));
            contentPanel.Children.Add(CreatePromptValue(_deck.DeckId.ToString()));
            contentPanel.Children.Add(CreatePromptLabel("Version (required)"));
            contentPanel.Children.Add(versionTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Description (optional)"));
            contentPanel.Children.Add(descriptionTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Card count"));
            contentPanel.Children.Add(CreatePromptValue(_deck.Count.ToString()));
            contentPanel.Children.Add(CreatePromptLabel("Wands start index"));
            contentPanel.Children.Add(wandsPanel);
            contentPanel.Children.Add(CreatePromptLabel("Cups start index"));
            contentPanel.Children.Add(cupsIndexTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Swords start index"));
            contentPanel.Children.Add(swordsIndexTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Pentacles start index"));
            contentPanel.Children.Add(pentaclesIndexTextBox);
            contentPanel.Children.Add(buttonPanel);

            var dialog = new Window { Title = "Deck Details", Owner = this, Content = contentPanel, SizeToContent = SizeToContent.WidthAndHeight, MinWidth = 540, MaxWidth = 800, ResizeMode = ResizeMode.NoResize, WindowStartupLocation = WindowStartupLocation.CenterOwner, ShowInTaskbar = false };

            okButton.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(versionTextBox.Text))
                {
                    MessageBox.Show(dialog, "A deck version is required.", "Version Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    versionTextBox.Focus();
                    return;
                }

                if (!ValidateSuitStartIndex(wandsIndexTextBox.Text, "Wands start index") |
                    !ValidateSuitStartIndex(cupsIndexTextBox.Text, "Cups start index") |
                    !ValidateSuitStartIndex(swordsIndexTextBox.Text, "Swords start index") |
                    !ValidateSuitStartIndex(pentaclesIndexTextBox.Text, "Pentacles start index"))
                {
                    return;
                }

                _deck.Description = string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? null : descriptionTextBox.Text.Trim();

                _deck.Type = string.IsNullOrWhiteSpace(typeTextBox.Text)
                    ? "unknown"
                    : typeTextBox.Text.Trim();

                _deck.Description = string.IsNullOrWhiteSpace(descriptionTextBox.Text)
                    ? null
                    : descriptionTextBox.Text.Trim();
                // Save validated integer values
                _deck.WandsIndex = int.Parse(wandsIndexTextBox.Text);
                _deck.CupsIndex = int.Parse(cupsIndexTextBox.Text);
                _deck.SwordsIndex = int.Parse(swordsIndexTextBox.Text);
                _deck.PentaclesIndex = int.Parse(pentaclesIndexTextBox.Text);

                dialog.DialogResult = true;
            };

            dialog.ContentRendered += (_, _) => versionTextBox.Focus();

            if (dialog.ShowDialog() != true)
                return null;

            if (string.Equals(_deck.Type, "Tarot", StringComparison.OrdinalIgnoreCase))
            {
                bUseFileStrip = true;
                _useNavigation = true;

                if (_navigationWindow == null)
                    RefreshNavigation();
            }

            return versionTextBox.Text.Trim();
        }
        private string PromptForDeckVersioxxxn()
        {
            var versionTextBox = new TextBox { Text = _deck.Version ?? string.Empty, MinWidth = 300, Margin = new Thickness(0, 4, 0, 12) };
            var descriptionTextBox = new TextBox { Text = _deck.Description ?? string.Empty, MinWidth = 300, MinHeight = 80, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Margin = new Thickness(0, 4, 0, 12) };
            var okButton = new Button { Content = "Save Deck Details", IsDefault = true, MinWidth = 120 };
            var cancelButton = new Button { Content = "Cancel", IsCancel = true, MinWidth = 80 };

            // Create and keep references to suit index textboxes
            var wandsIndexTextBox = CreateWandsIndexTextBox();
            var cupsIndexTextBox = CreateCupsIndexTextBox();
            var swordsIndexTextBox = CreateSwordsIndexTextBox();
            var pentaclesIndexTextBox = CreatePentaclesIndexTextBox();

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            var contentPanel = new StackPanel { Margin = new Thickness(18) };
            contentPanel.Children.Add(CreatePromptLabel("Full path"));
            contentPanel.Children.Add(CreatePromptValue(DeckInfoFilePath));
            contentPanel.Children.Add(CreatePromptLabel("Tarot deck name"));
            contentPanel.Children.Add(CreatePromptValue(tarotDeckName));
            contentPanel.Children.Add(CreatePromptLabel("Deck ID"));
            contentPanel.Children.Add(CreatePromptValue(_deck.DeckId.ToString()));
            contentPanel.Children.Add(CreatePromptLabel("Version (required)"));
            contentPanel.Children.Add(versionTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Description (optional)"));
            contentPanel.Children.Add(descriptionTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Card count"));
            contentPanel.Children.Add(CreatePromptValue(_deck.Count.ToString()));
            contentPanel.Children.Add(CreatePromptLabel("Wands start index"));
            contentPanel.Children.Add(wandsIndexTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Cups start index"));
            contentPanel.Children.Add(cupsIndexTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Swords start index"));
            contentPanel.Children.Add(swordsIndexTextBox);
            contentPanel.Children.Add(CreatePromptLabel("Pentacles start index"));
            contentPanel.Children.Add(pentaclesIndexTextBox);
            contentPanel.Children.Add(buttonPanel);

            var dialog = new Window { Title = "Deck Details", Owner = this, Content = contentPanel, SizeToContent = SizeToContent.WidthAndHeight, MinWidth = 540, MaxWidth = 800, ResizeMode = ResizeMode.NoResize, WindowStartupLocation = WindowStartupLocation.CenterOwner, ShowInTaskbar = false };

            okButton.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(versionTextBox.Text))
                {
                    MessageBox.Show(dialog, "A deck version is required.", "Version Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    versionTextBox.Focus();
                    return;
                }

                if (!ValidateSuitStartIndex(wandsIndexTextBox.Text, "Wands start index") |
                    !ValidateSuitStartIndex(cupsIndexTextBox.Text, "Cups start index") |
                    !ValidateSuitStartIndex(swordsIndexTextBox.Text, "Swords start index") |
                    !ValidateSuitStartIndex(pentaclesIndexTextBox.Text, "Pentacles start index"))
                {
                    return;
                }

                _deck.Description = string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? null : descriptionTextBox.Text.Trim();

                // Save validated integer values
                _deck.WandsIndex = int.Parse(wandsIndexTextBox.Text);
                _deck.CupsIndex = int.Parse(cupsIndexTextBox.Text);
                _deck.SwordsIndex = int.Parse(swordsIndexTextBox.Text);
                _deck.PentaclesIndex = int.Parse(pentaclesIndexTextBox.Text);

                dialog.DialogResult = true;
            };

            dialog.ContentRendered += (_, _) => versionTextBox.Focus();

            return dialog.ShowDialog() == true ? versionTextBox.Text.Trim() : null;
        }

        private bool ValidateSuitStartIndex(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show(
                    "A nonnegative integer value is required for " +
                    fieldName + ".",
                    "Invalid Value",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            if (!int.TryParse(value, out int intValue) || intValue < 0)
            {
                MessageBox.Show(
                    "Please enter a valid nonnegative integer for " +
                    fieldName + ".",
                    "Invalid Value",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        private TextBox CreateWandsIndexTextBox()
        {
            return CreateSuitIndexTextBox(
                _deck.WandsIndex,
                "Wands start index must be a nonnegative integer.");
        }

        private TextBox CreateCupsIndexTextBox()
        {
            return CreateSuitIndexTextBox(
                _deck.CupsIndex,
                "Cups start index must be a nonnegative integer.");
        }

        private TextBox CreateSwordsIndexTextBox()
        {
            return CreateSuitIndexTextBox(
                _deck.SwordsIndex,
                "Swords start index must be a nonnegative integer.");
        }

        private TextBox CreatePentaclesIndexTextBox()
        {
            return CreateSuitIndexTextBox(
                _deck.PentaclesIndex,
                "Pentacles start index must be a nonnegative integer.");
        }
        private TextBox CreateSuitIndexTextBox(int currentValue, string toolTip)
        {
            return new TextBox
            {
                Text = currentValue.ToString(),
                Width = 80,
                MaxLength = 6,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 4, 12, 12),
                ToolTip = toolTip
            };
        }
        private TextBox CreateSuitIndexTexxxxtBox(int currentValue, string toolTip)
        {
            return new TextBox
            {
                Text = currentValue.ToString(),
                MinWidth = 100,
                Margin = new Thickness(0, 4, 0, 12),
                ToolTip = toolTip
            };
        }

        private static TextBlock CreatePromptLabel(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 6, 0, 2)
            };
        }

        private static TextBlock CreatePromptValue(string text)
        {
            return new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(8, 0, 0, 4)
            };
        }
        /*
        private static TextBlock CreatePromptLabel(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 6, 0, 2)
            };
        }

        private static TextBlock CreatePromptValue(string text)
        {
            return new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(8, 0, 0, 4)
            };
        }
        */
        private void ShowImage(int index)
        {
            CancelEdit();

            if (_deckImages.Count == 0)
            {
                _currentIndex = 0;
                _currentBitmap = null;

                MainImage.Source = null;
                AnnotationCanvas.Children.Clear();

                ImageTitleTextBlock.Text = "No images in this deck";
                ImagePositionTextBlock.Text = "0 / 0";
                AnnotationCountTextBlock.Text = "0 annotations";
                StatusTextBlock.Text = "The selected deck folder contains no supported images.";

                RefreshAnnotations();
                return;
            }

            _currentIndex = Math.Clamp(index, 0, _deckImages.Count - 1);
            string imagePath = CurrentImagePath;

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                _currentBitmap = null;
                MainImage.Source = null;
                AnnotationCanvas.Children.Clear();

                ImageTitleTextBlock.Text = "Invalid image";
                ImagePositionTextBlock.Text =
                    $"{_currentIndex + 1:N0} / {_deckImages.Count:N0}";
                AnnotationCountTextBlock.Text = "0 annotations";
                StatusTextBlock.Text = "The selected image path is empty.";

                return;
            }

            try
            {
                BitmapSource loadedBitmap = LoadBitmap(imagePath);

                // Remove the previous source before assigning the new card.
                MainImage.Source = null;
                _currentBitmap = loadedBitmap;
                MainImage.Source = _currentBitmap;
                MainImage.Visibility = Visibility.Visible;

                ImageTitleTextBlock.Text = Path.GetFileName(imagePath);
                ImagePositionTextBlock.Text =
                    $"{_currentIndex + 1:N0} / {_deckImages.Count:N0}";

                RefreshAnnotations();

                StatusTextBlock.Text =
                    $"{_currentBitmap.PixelWidth:N0} × " +
                    $"{_currentBitmap.PixelHeight:N0} pixels — " +
                    CurrentRootRelativePath;

                // Wait until WPF has measured the image surface before converting
                // source-image coordinates into annotation-canvas coordinates.
                Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Loaded,
                    new Action(() =>
                    {
                        MainImage.InvalidateVisual();
                        AnnotationSurface.InvalidateVisual();
                        RenderAnnotations();
                    }));
            }
            catch (Exception ex) when (
                ex is IOException or
                UnauthorizedAccessException or
                ArgumentException or
                NotSupportedException)
            {
                _currentBitmap = null;
                MainImage.Source = null;
                AnnotationCanvas.Children.Clear();

                ImageTitleTextBlock.Text = Path.GetFileName(imagePath);
                ImagePositionTextBlock.Text =
                    $"{_currentIndex + 1:N0} / {_deckImages.Count:N0}";
                AnnotationCountTextBlock.Text = "0 annotations";

                StatusTextBlock.Text =
                    $"Unable to load {Path.GetFileName(imagePath)}: {ex.Message}";
            }
        }
        private void ShowImagex(int index)
        {
            CancelEdit();

            if (_deckImages.Count == 0)
            {
                _currentIndex = 0;
                _currentBitmap = null;
                MainImage.Source = null;
                ImageTitleTextBlock.Text = "No images in this deck";
                ImagePositionTextBlock.Text = "0 / 0";
                AnnotationCountTextBlock.Text = "0 annotations";
                RefreshAnnotations();
                return;
            }

            _currentIndex = Math.Clamp(index, 0, _deckImages.Count - 1);
            string imagePath = CurrentImagePath;

            try
            {
                _currentBitmap = LoadBitmap(imagePath);
                MainImage.Source = _currentBitmap;

                ImageTitleTextBlock.Text = Path.GetFileName(imagePath);
                ImagePositionTextBlock.Text =
                    $"{_currentIndex + 1:N0} / {_deckImages.Count:N0}";

                RefreshAnnotations();
                RenderAnnotations();

                StatusTextBlock.Text =
                    $"{_currentBitmap.PixelWidth:N0} × " +
                    $"{_currentBitmap.PixelHeight:N0} pixels — " +
                    CurrentRootRelativePath;
            }
            catch (Exception ex) when (
                ex is IOException or
                UnauthorizedAccessException or
                ArgumentException or
                NotSupportedException)
            {
                _currentBitmap = null;
                MainImage.Source = null;
                ImageTitleTextBlock.Text = Path.GetFileName(imagePath);
                StatusTextBlock.Text =
                    $"Unable to load {Path.GetFileName(imagePath)}: {ex.Message}";
            }
        }
        private static BitmapSource LoadBitmap(string path)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(
                Path.GetFullPath(path),
                UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
        private static BitmapSource LoadBitmapx(string path)
        {
            using FileStream stream = new(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (_deckImages.Count == 0)
                return;

            if (_currentIndex <= 0)
            {
                StatusTextBlock.Text = "Already at the first card.";
                return;
            }

            ShowImage(_currentIndex - 1);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_deckImages.Count == 0)
                return;

            if (_currentIndex >= _deckImages.Count - 1)
            {
                StatusTextBlock.Text = "Already at the last card.";
                return;
            }

            ShowImage(_currentIndex + 1);
        }

        private void RefreshNavigation()
        {
            List<AnnotationThumbnail> thumbnails = _deckImages
                .Select((path, index) => new AnnotationThumbnail
                {
                    Index = index,
                    Caption = Path.GetFileNameWithoutExtension(path),
                    Thumbnail = LoadThumbnail(path)
                })
                .ToList();

            var navigation = new NavigationWpf(this, ShowImage, thumbnails);
            navigation.Closed += (_, _) => _navigationWindow = null;
            _navigationWindow = navigation;
            navigation.Show();
        }

        private static BitmapSource LoadThumbnail(string path)
        {
            try
            {
                using FileStream stream = new(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = 180;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private void Tool_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton &&
                Enum.TryParse(
                    radioButton.Tag?.ToString(),
                    out AnnotationTool selectedTool))
            {
                _activeTool = selectedTool;
                CancelEdit();
                StatusTextBlock.Text =
                    $"{selectedTool} selection tool active.";
            }
        }

        private void AnnotationCanvas_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (_currentBitmap == null)
                return;

            WpfPoint point = e.GetPosition(AnnotationCanvas);
            Rect imageRect = GetRenderedImageRect();

            if (!imageRect.Contains(point))
                return;

            _dragging = true;
            _dragStart = ClampToImage(point, imageRect);
            _dragCurrent = _dragStart;

            _freehandDisplayPoints.Clear();
            if (_activeTool == AnnotationTool.Freehand)
                _freehandDisplayPoints.Add(_dragStart);

            AnnotationCanvas.CaptureMouse();
            RenderAnnotations();
            e.Handled = true;
        }

        private void AnnotationCanvas_MouseMove(
            object sender,
            System.Windows.Input.MouseEventArgs e)
        {
            if (!_dragging)
                return;

            Rect imageRect = GetRenderedImageRect();
            _dragCurrent = ClampToImage(
                e.GetPosition(AnnotationCanvas),
                imageRect);

            if (_activeTool == AnnotationTool.Freehand)
            {
                if (_freehandDisplayPoints.Count == 0 ||
                    Distance(
                        _freehandDisplayPoints[^1],
                        _dragCurrent) >= 2.0)
                {
                    _freehandDisplayPoints.Add(_dragCurrent);
                }
            }

            RenderAnnotations();
            e.Handled = true;
        }

        private void AnnotationCanvas_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            if (!_dragging)
                return;

            _dragging = false;
            AnnotationCanvas.ReleaseMouseCapture();

            ImageAnnotation annotation =
                CreateAnnotationFromCurrentSelection();

            if (annotation == null)
            {
                RenderAnnotations();
                StatusTextBlock.Text = "The selection was too small.";
                return;
            }

            _editingAnnotation = annotation;
            LoadEditor(annotation);
            SaveAnnotationButton.IsEnabled = true;
            AnnotationTitleTextBox.Focus();

            RenderAnnotations();
            StatusTextBlock.Text =
                "Enter annotation information, then select Save Annotation.";

            e.Handled = true;
        }

        private ImageAnnotation CreateAnnotationFromCurrentSelection()
        {
            if (_currentBitmap == null)
                return null;

            Rect imageRect = GetRenderedImageRect();

            if (_activeTool == AnnotationTool.Freehand)
            {
                if (_freehandDisplayPoints.Count < 2)
                    return null;

                List<AnnotationPointData> points = _freehandDisplayPoints
                    .Select(point => DisplayToSource(point, imageRect))
                    .ToList();

                double minX = points.Min(point => point.X);
                double minY = points.Min(point => point.Y);
                double maxX = points.Max(point => point.X);
                double maxY = points.Max(point => point.Y);

                if (maxX - minX < 2 || maxY - minY < 2)
                    return null;

                return CreateDraftAnnotation(
                    minX,
                    minY,
                    maxX - minX,
                    maxY - minY,
                    points);
            }

            Rect displaySelection = MakeRect(_dragStart, _dragCurrent);
            displaySelection.Intersect(imageRect);

            if (displaySelection.Width < 4 ||
                displaySelection.Height < 4)
            {
                return null;
            }

            AnnotationPointData topLeft =
                DisplayToSource(displaySelection.TopLeft, imageRect);

            AnnotationPointData bottomRight =
                DisplayToSource(displaySelection.BottomRight, imageRect);

            return CreateDraftAnnotation(
                topLeft.X,
                topLeft.Y,
                bottomRight.X - topLeft.X,
                bottomRight.Y - topLeft.Y,
                new List<AnnotationPointData>());
        }
        private ImageAnnotation CreateDraftAnnotation(
        double x,
        double y,
        double width,
        double height,
        List<AnnotationPointData> points)
        {
            if (_currentBitmap == null)
                return null;

            var annotation = new ImageAnnotation
            {
                AnnotationId = Guid.NewGuid(),

                DeckId = _deck.DeckId,
                DeckName = _deck.Name,
                DeckVersion = _deck.Version,
                CardNumber = _currentIndex + 1,

                RootRelativePath = CurrentRootRelativePath,
                Shape = _activeTool.ToString(),
                Color = _activeTool == AnnotationTool.Highlight
                    ? "#E8C84E"
                    : "#4E91E8",
                CreatedUtc = DateTime.UtcNow,
                ModifiedUtc = DateTime.UtcNow
            };

            annotation.SetSourceGeometry(
                _currentBitmap.PixelWidth,
                _currentBitmap.PixelHeight,
                x,
                y,
                width,
                height,
                points);

            return annotation;
        }
        private void DisplayAnnotations_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_deckFolder) ||
                !Directory.Exists(_deckFolder))
            {
                StatusTextBlock.Text =
                    "Open a deck before displaying annotations.";
                return;
            }

            var display = new AnnotationDisplayWpf(
                _deckFolder,
                _deck.Name,
                CurrentRootRelativePath,
                _deck.Annotations ?? Enumerable.Empty<ImageAnnotation>())
            {
                Owner = this
            };

            display.Show();
        }
        private void SaveAnnotation_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_editingAnnotation == null)
                return;

            string title = AnnotationTitleTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                StatusTextBlock.Text =
                    "An annotation title is required.";
                AnnotationTitleTextBox.Focus();
                return;
            }

            _editingAnnotation.Title = title;
            _editingAnnotation.Comment =
                AnnotationCommentTextBox.Text.Trim();
            _editingAnnotation.Keywords =
                AnnotationKeywordsTextBox.Text.Trim();
            _editingAnnotation.CrossReferences =
                AnnotationReferencesTextBox.Text.Trim();

            _editingAnnotation.ModifiedUtc = DateTime.UtcNow;

            _editingAnnotation.SelectionImagePng =
                CreateSelectionImage(_editingAnnotation);

            if (!_deck.Annotations.Any(annotation =>
                    annotation.AnnotationId ==
                    _editingAnnotation.AnnotationId))
            {
                _deck.Annotations.Add(_editingAnnotation);
            }

            try
            {
                SaveAnnotations();
                StatusTextBlock.Text =
                    $"Saved annotation: {_editingAnnotation.Title}";
                CancelEdit();
                RefreshAnnotations();
                RenderAnnotations();
            }
            catch (Exception ex) when (
                ex is IOException or
                UnauthorizedAccessException or
                ArgumentException or
                NotSupportedException)
            {
                StatusTextBlock.Text =
                    $"Unable to save annotation: {ex.Message}";
            }
        }

        private void CancelEdit_Click(
            object sender,
            RoutedEventArgs e)
        {
            CancelEdit();
            RenderAnnotations();
        }

        private void CancelEdit()
        {
            _editingAnnotation = null;
            _dragging = false;
            _freehandDisplayPoints.Clear();

            AnnotationTitleTextBox.Clear();
            AnnotationCommentTextBox.Clear();
            AnnotationKeywordsTextBox.Clear();
            AnnotationReferencesTextBox.Clear();

            SaveAnnotationButton.IsEnabled = false;
        }

        private void DeleteAnnotation_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_editingAnnotation == null)
                return;

            ImageAnnotation storedAnnotation =
                _deck.Annotations.FirstOrDefault(annotation =>
                    annotation.AnnotationId ==
                    _editingAnnotation.AnnotationId);

            if (storedAnnotation == null)
            {
                CancelEdit();
                RenderAnnotations();
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                this,
                $"Delete annotation \"{storedAnnotation.Title}\"?",
                "Delete Annotation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
                return;

            _deck.Annotations.Remove(storedAnnotation);

            try
            {
                SaveAnnotations();
                CancelEdit();
                RefreshAnnotations();
                RenderAnnotations();
                StatusTextBlock.Text = "Annotation deleted.";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text =
                    $"Unable to save deletion: {ex.Message}";
            }
        }

        private void AnnotationCard_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is Button button &&
                button.Tag is ImageAnnotation annotation)
            {
                _editingAnnotation = annotation;
                LoadEditor(annotation);
                SaveAnnotationButton.IsEnabled = true;
                RenderAnnotations();
            }
        }

        private void LoadEditor(ImageAnnotation annotation)
        {
            AnnotationTitleTextBox.Text = annotation.Title ?? string.Empty;
            AnnotationCommentTextBox.Text =
                annotation.Comment ?? string.Empty;
            AnnotationKeywordsTextBox.Text =
                annotation.Keywords ?? string.Empty;
            AnnotationReferencesTextBox.Text =
                annotation.CrossReferences ?? string.Empty;
        }

        private void RefreshAnnotations()
        {
            List<ImageAnnotation> annotations = CurrentAnnotations
                .OrderBy(annotation => annotation.Title)
                .ToList();

            AnnotationItemsControl.ItemsSource = null;
            AnnotationItemsControl.ItemsSource = annotations;

            AnnotationCountTextBlock.Text =
                $"{annotations.Count:N0} annotation" +
                (annotations.Count == 1 ? string.Empty : "s");
        }

        private void AnnotationSurface_SizeChanged(
            object sender,
            SizeChangedEventArgs e)
        {
            RenderAnnotations();
        }

        private void RenderAnnotations()
        {
            AnnotationCanvas.Children.Clear();

            if (_currentBitmap == null)
                return;

            int calloutIndex = 0;

            foreach (ImageAnnotation annotation in CurrentAnnotations)
            {
                bool selected =
                    _editingAnnotation?.AnnotationId ==
                    annotation.AnnotationId;

                DrawAnnotation(
                    annotation,
                    selected,
                    calloutIndex++);
            }

            if (_editingAnnotation != null &&
                !_deck.Annotations.Any(annotation =>
                    annotation.AnnotationId ==
                    _editingAnnotation.AnnotationId))
            {
                DrawAnnotation(
                    _editingAnnotation,
                    true,
                    calloutIndex);
            }

            if (_dragging)
                DrawActiveSelection();
        }

        private void DrawAnnotation(
            ImageAnnotation annotation,
            bool selected,
            int calloutIndex)
        {
            Rect imageRect = GetRenderedImageRect();
            Color color = ParseColor(annotation.Color);
            Brush stroke = new SolidColorBrush(color);

            Rect displayBounds = SourceToDisplay(
                annotation.X,
                annotation.Y,
                annotation.Width,
                annotation.Height,
                imageRect);

            if (string.Equals(
                    annotation.Shape,
                    AnnotationTool.Freehand.ToString(),
                    StringComparison.OrdinalIgnoreCase))
            {
                var polyline = new Polyline
                {
                    Stroke = stroke,
                    StrokeThickness = selected ? 4 : 3,
                    StrokeLineJoin = PenLineJoin.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                    IsHitTestVisible = false
                };

                foreach (AnnotationPointData point in annotation.Points)
                    polyline.Points.Add(SourceToDisplay(point, imageRect));

                AnnotationCanvas.Children.Add(polyline);
            }
            else
            {
                Shape shape = string.Equals(
                        annotation.Shape,
                        AnnotationTool.Ellipse.ToString(),
                        StringComparison.OrdinalIgnoreCase)
                    ? new Ellipse()
                    : new WpfRectangle();

                shape.Width = displayBounds.Width;
                shape.Height = displayBounds.Height;
                shape.Stroke = stroke;
                shape.StrokeThickness = selected ? 4 : 3;
                shape.IsHitTestVisible = false;

                if (string.Equals(
                        annotation.Shape,
                        AnnotationTool.Highlight.ToString(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    shape.Fill = new SolidColorBrush(
                        Color.FromArgb(70, color.R, color.G, color.B));
                }
                else
                {
                    shape.Fill = Brushes.Transparent;
                }

                Canvas.SetLeft(shape, displayBounds.Left);
                Canvas.SetTop(shape, displayBounds.Top);
                AnnotationCanvas.Children.Add(shape);
            }

            DrawCallout(annotation, displayBounds, color, calloutIndex);
        }

        private void DrawCallout(
            ImageAnnotation annotation,
            Rect bounds,
            Color color,
            int calloutIndex)
        {
            double labelWidth = 190;
            double labelX = Math.Min(
                Math.Max(bounds.Right + 24, 10),
                Math.Max(10, AnnotationCanvas.ActualWidth - labelWidth - 10));

            double labelY = Math.Min(
                Math.Max(bounds.Top + calloutIndex * 12, 8),
                Math.Max(8, AnnotationCanvas.ActualHeight - 62));

            var line = new Line
            {
                X1 = bounds.Left + bounds.Width / 2,
                Y1 = bounds.Top + bounds.Height / 2,
                X2 = labelX,
                Y2 = labelY + 20,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2,
                IsHitTestVisible = false
            };

            var text = new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(annotation.Title)
                    ? "New annotation"
                    : annotation.Title,
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = labelWidth - 18
            };

            var label = new Border
            {
                Width = labelWidth,
                MinHeight = 40,
                Padding = new Thickness(8, 5, 8, 5),
                Background = new SolidColorBrush(
                    Color.FromArgb(225, 39, 42, 52)),
                BorderBrush = new SolidColorBrush(color),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Child = text,
                IsHitTestVisible = false
            };

            AnnotationCanvas.Children.Add(line);
            Canvas.SetLeft(label, labelX);
            Canvas.SetTop(label, labelY);
            AnnotationCanvas.Children.Add(label);
        }

        private void DrawActiveSelection()
        {
            if (_activeTool == AnnotationTool.Freehand)
            {
                var polyline = new Polyline
                {
                    Stroke = Brushes.White,
                    StrokeThickness = 3,
                    StrokeDashArray = new DoubleCollection { 4, 2 },
                    IsHitTestVisible = false
                };

                foreach (WpfPoint point in _freehandDisplayPoints)
                    polyline.Points.Add(point);

                AnnotationCanvas.Children.Add(polyline);
                return;
            }

            Rect bounds = MakeRect(_dragStart, _dragCurrent);

            Shape shape = _activeTool == AnnotationTool.Ellipse
                ? new Ellipse()
                : new WpfRectangle();

            shape.Width = bounds.Width;
            shape.Height = bounds.Height;
            shape.Stroke = Brushes.White;
            shape.StrokeThickness = 2;
            shape.StrokeDashArray = new DoubleCollection { 4, 2 };
            shape.Fill = _activeTool == AnnotationTool.Highlight
                ? new SolidColorBrush(Color.FromArgb(60, 255, 220, 70))
                : Brushes.Transparent;
            shape.IsHitTestVisible = false;

            Canvas.SetLeft(shape, bounds.Left);
            Canvas.SetTop(shape, bounds.Top);
            AnnotationCanvas.Children.Add(shape);
        }

        private Rect GetRenderedImageRect()
        {
            if (_currentBitmap == null ||
                AnnotationCanvas.ActualWidth <= 0 ||
                AnnotationCanvas.ActualHeight <= 0)
            {
                return Rect.Empty;
            }

            double imageWidth = _currentBitmap.PixelWidth;
            double imageHeight = _currentBitmap.PixelHeight;
            double canvasWidth = AnnotationCanvas.ActualWidth;
            double canvasHeight = AnnotationCanvas.ActualHeight;

            double scale = Math.Min(
                canvasWidth / imageWidth,
                canvasHeight / imageHeight);

            double renderedWidth = imageWidth * scale;
            double renderedHeight = imageHeight * scale;

            return new Rect(
                (canvasWidth - renderedWidth) / 2,
                (canvasHeight - renderedHeight) / 2,
                renderedWidth,
                renderedHeight);
        }

        private AnnotationPointData DisplayToSource(
            WpfPoint point,
            Rect imageRect)
        {
            double x = (point.X - imageRect.Left) *
                       _currentBitmap.PixelWidth /
                       imageRect.Width;

            double y = (point.Y - imageRect.Top) *
                       _currentBitmap.PixelHeight /
                       imageRect.Height;

            return new AnnotationPointData
            {
                X = Math.Clamp(x, 0, _currentBitmap.PixelWidth),
                Y = Math.Clamp(y, 0, _currentBitmap.PixelHeight)
            };
        }

        private WpfPoint SourceToDisplay(
            AnnotationPointData point,
            Rect imageRect)
        {
            return new WpfPoint(
                imageRect.Left +
                point.X * imageRect.Width / _currentBitmap.PixelWidth,
                imageRect.Top +
                point.Y * imageRect.Height / _currentBitmap.PixelHeight);
        }

        private Rect SourceToDisplay(
            double x,
            double y,
            double width,
            double height,
            Rect imageRect)
        {
            WpfPoint topLeft = SourceToDisplay(
                new AnnotationPointData { X = x, Y = y },
                imageRect);

            WpfPoint bottomRight = SourceToDisplay(
                new AnnotationPointData
                {
                    X = x + width,
                    Y = y + height
                },
                imageRect);

            return new Rect(topLeft, bottomRight);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            RunSearch();
        }

        private void SearchTextBox_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RunSearch();
                e.Handled = true;
            }
        }

        private void RunSearch()
        {
            string[] terms = (SearchTextBox.Text ?? string.Empty)
                .Replace("+", " ")
                .Split(
                    new[] { ' ', ',', ';' },
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

            if (terms.Length == 0)
            {
                SearchResultsListBox.ItemsSource = null;
                StatusTextBlock.Text = "Enter one or more search terms.";
                return;
            }

            List<AnnotationSearchResult> results = _deck.Annotations
                .Where(annotation => terms.All(term =>
                    BuildSearchText(annotation).Contains(
                        term,
                        StringComparison.OrdinalIgnoreCase)))
                .GroupBy(annotation => annotation.RootRelativePath,
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => new AnnotationSearchResult
                {
                    RootRelativePath = group.Key,
                    DisplayText =
                        $"{Path.GetFileName(group.Key)} — " +
                        $"{group.Count():N0} match(es)"
                })
                .OrderBy(result => result.DisplayText)
                .ToList();

            SearchResultsListBox.ItemsSource = results;
            StatusTextBlock.Text =
                $"{results.Count:N0} image(s) matched the search.";
        }

        private static string BuildSearchText(
            ImageAnnotation annotation)
        {
            return string.Join(
                " ",
                annotation.Title,
                annotation.Comment,
                annotation.Keywords,
                annotation.CrossReferences,
                annotation.RootRelativePath);
        }
        private void BrowseAnnotations_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_deckFolder) ||
                !Directory.Exists(_deckFolder))
            {
                StatusTextBlock.Text =
                    "Open a deck before browsing annotations.";
                return;
            }

            var browser = new AnnotationBrowserWpf(
                _deckFolder,
                _deck.Name,
                _deck.Annotations ?? Enumerable.Empty<ImageAnnotation>())
            {
                Owner = this
            };

            browser.Show();
        }

        private void SearchResultsListBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (SearchResultsListBox.SelectedItem is not
                AnnotationSearchResult result)
            {
                return;
            }

            int index = _deckImages.FindIndex(path =>
                string.Equals(
                    ToRootRelativePath(path),
                    result.RootRelativePath,
                    StringComparison.OrdinalIgnoreCase));

            if (index >= 0)
                ShowImage(index);
        }

        private string ToRootRelativePath(string path)
        {
            return Path.GetRelativePath(_deckFolder, path)
                .Replace('\\', '/');
        }

        private static Rect MakeRect(
            WpfPoint first,
            WpfPoint second)
        {
            return new Rect(
                Math.Min(first.X, second.X),
                Math.Min(first.Y, second.Y),
                Math.Abs(second.X - first.X),
                Math.Abs(second.Y - first.Y));
        }

        private static WpfPoint ClampToImage(
            WpfPoint point,
            Rect imageRect)
        {
            return new WpfPoint(
                Math.Clamp(point.X, imageRect.Left, imageRect.Right),
                Math.Clamp(point.Y, imageRect.Top, imageRect.Bottom));
        }

        private static double Distance(
            WpfPoint first,
            WpfPoint second)
        {
            double x = second.X - first.X;
            double y = second.Y - first.Y;
            return Math.Sqrt(x * x + y * y);
        }

        private static Color ParseColor(string value)
        {
            try
            {
                object converted =
                    ColorConverter.ConvertFromString(value ?? "#4E91E8");

                return converted is Color color
                    ? color
                    : Color.FromRgb(78, 145, 232);
            }
            catch
            {
                return Color.FromRgb(78, 145, 232);
            }
        }

        private void Window_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelEdit();
                RenderAnnotations();
                e.Handled = true;
                return;
            }

            // Preserve normal arrow, Home, and End editing behavior inside text boxes.
            if (IsTextEditorFocused())
                return;

            switch (e.Key)
            {
                case Key.Home:
                    if (_deckImages.Count > 0)
                    {
                        if (_currentIndex != 0)
                            ShowImage(0);
                        else
                            StatusTextBlock.Text = "Already at the first card.";
                    }

                    e.Handled = true;
                    break;

                case Key.End:
                    if (_deckImages.Count > 0)
                    {
                        int lastIndex = _deckImages.Count - 1;

                        if (_currentIndex != lastIndex)
                            ShowImage(lastIndex);
                        else
                            StatusTextBlock.Text = "Already at the last card.";
                    }

                    e.Handled = true;
                    break;

                case Key.Left:
                    Previous_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;

                case Key.Right:
                    Next_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;
                case Key.PageDown:
                    NavigateSuit(1);
                    e.Handled = true;
                    break;

                case Key.PageUp:
                    NavigateSuit(-1);
                    e.Handled = true;
                    break;
            }
        }

        private static bool IsTextEditorFocused()
        {
            return Keyboard.FocusedElement is TextBox;
        }

        private byte[] CreateSelectionImage(
        ImageAnnotation annotation)
        {
            if (_currentBitmap == null)
                return null;

            int left = Math.Clamp(
                (int)Math.Floor(annotation.X),
                0,
                _currentBitmap.PixelWidth - 1);

            int top = Math.Clamp(
                (int)Math.Floor(annotation.Y),
                0,
                _currentBitmap.PixelHeight - 1);

            int right = Math.Clamp(
                (int)Math.Ceiling(annotation.X + annotation.Width),
                left + 1,
                _currentBitmap.PixelWidth);

            int bottom = Math.Clamp(
                (int)Math.Ceiling(annotation.Y + annotation.Height),
                top + 1,
                _currentBitmap.PixelHeight);

            int width = right - left;
            int height = bottom - top;

            if (width <= 0 || height <= 0)
                return null;

            var croppedBitmap = new CroppedBitmap(
                _currentBitmap,
                new Int32Rect(left, top, width, height));

            croppedBitmap.Freeze();

            BitmapSource result = croppedBitmap;

            if (string.Equals(
                    annotation.Shape,
                    AnnotationTool.Ellipse.ToString(),
                    StringComparison.OrdinalIgnoreCase))
            {
                result = ApplySelectionClip(
                    croppedBitmap,
                    new EllipseGeometry(
                        new Rect(0, 0, width, height)),
                    width,
                    height);
            }
            else if (string.Equals(
                         annotation.Shape,
                         AnnotationTool.Freehand.ToString(),
                         StringComparison.OrdinalIgnoreCase) &&
                     annotation.Points?.Count >= 3)
            {
                var geometry = new StreamGeometry();

                using (StreamGeometryContext context = geometry.Open())
                {
                    AnnotationPointData first = annotation.Points[0];

                    context.BeginFigure(
                        new WpfPoint(
                            first.X - left,
                            first.Y - top),
                        isFilled: true,
                        isClosed: true);

                    context.PolyLineTo(
                        annotation.Points
                            .Skip(1)
                            .Select(point => new WpfPoint(
                                point.X - left,
                                point.Y - top))
                            .ToList(),
                        isStroked: true,
                        isSmoothJoin: true);
                }

                geometry.Freeze();

                result = ApplySelectionClip(
                    croppedBitmap,
                    geometry,
                    width,
                    height);
            }

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(result));

            using var output = new MemoryStream();
            encoder.Save(output);

            return output.ToArray();
        }

        private static BitmapSource ApplySelectionClip(
            BitmapSource source,
            Geometry clip,
            int pixelWidth,
            int pixelHeight)
        {
            var visual = new DrawingVisual();

            using (DrawingContext context = visual.RenderOpen())
            {
                context.PushClip(clip);

                context.DrawImage(
                    source,
                    new Rect(0, 0, pixelWidth, pixelHeight));

                context.Pop();
            }

            var result = new RenderTargetBitmap(
                pixelWidth,
                pixelHeight,
                96,
                96,
                PixelFormats.Pbgra32);

            result.Render(visual);
            result.Freeze();

            return result;
        }
        private enum AnnotationTool
        {
            Rectangle,
            Ellipse,
            Highlight,
            Freehand
        }
    }

    public sealed class AnnotatedDeckInfo
    {
        public Guid DeckId { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "unknown";

        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedUtc { get; set; } = DateTime.UtcNow;

        public int Count { get; set; }

        public int WandsIndex { get; set; }
        public int CupsIndex { get; set; }
        public int SwordsIndex { get; set; }
        public int PentaclesIndex { get; set; }

        [JsonIgnore]
        public List<ImageAnnotation> Annotations { get; set; } = new();
    }


    public sealed class ImageAnnotation
    {
        /// <summary>
        /// Geometry version 1 used only source-pixel coordinates.
        /// Version 2 includes source dimensions and normalized coordinates.
        /// Stable identifier of the deck containing this annotation.
        /// </summary>
        public int GeometryVersion { get; set; } = 2;

        public Guid AnnotationId { get; set; } = Guid.NewGuid();
        public Guid DeckId { get; set; }

        /// <summary>
        /// Human-readable deck identity captured when the annotation is saved.
        /// </summary>
        public string DeckName { get; set; } = string.Empty;

        public string DeckVersion { get; set; } = string.Empty;

        /// <summary>
        /// One-based card number within the deck.
        /// </summary>
        public int CardNumber { get; set; }

        /// <summary>
        /// Image path relative to the deck's unique root folder.
        /// </summary>
        public string RootRelativePath { get; set; } = string.Empty;

        /// <summary>
        /// Rectangle, Ellipse, Highlight, or Freehand.
        /// </summary>
        public string Shape { get; set; } = "Rectangle";

        /// <summary>
        /// Original source-image dimensions when the annotation was created.
        /// </summary>
        public int SourcePixelWidth { get; set; }

        public int SourcePixelHeight { get; set; }

        /// <summary>
        /// Annotation bounds in original source-image pixels.
        /// These fields are retained for accuracy and compatibility.
        /// </summary>
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        /// <summary>
        /// Freehand points in original source-image pixels.
        /// </summary>
        public List<AnnotationPointData> Points { get; set; } = new();

        /// <summary>
        /// Resolution-independent annotation bounds.
        /// Values normally range from 0.0 through 1.0.
        /// </summary>
        public double NormalizedX { get; set; }

        public double NormalizedY { get; set; }

        public double NormalizedWidth { get; set; }

        public double NormalizedHeight { get; set; }

        /// <summary>
        /// Resolution-independent freehand points.
        /// Each X and Y value normally ranges from 0.0 through 1.0.
        /// </summary>
        public List<AnnotationPointData> NormalizedPoints { get; set; } = new();

        public string Title { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public string Keywords { get; set; } = string.Empty;

        public string CrossReferences { get; set; } = string.Empty;
        /*
         * REPLACE CrossReferences with a list of ImageAnnotationReference objects for better structure and type safety.
         * public List<ImageAnnotationReference> AnnotationReferences { get; set; } = new();
         */
        public string Color { get; set; } = "#4E91E8";

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Sets source-pixel geometry and calculates normalized geometry.
        /// </summary>
        public void SetSourceGeometry(
            int sourcePixelWidth,
            int sourcePixelHeight,
            double x,
            double y,
            double width,
            double height,
            IEnumerable<AnnotationPointData> sourcePoints = null)
        {
            if (sourcePixelWidth <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sourcePixelWidth),
                    "The source image width must be greater than zero.");

            if (sourcePixelHeight <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(sourcePixelHeight),
                    "The source image height must be greater than zero.");

            GeometryVersion = 2;

            SourcePixelWidth = sourcePixelWidth;
            SourcePixelHeight = sourcePixelHeight;

            X = Math.Clamp(x, 0, sourcePixelWidth);
            Y = Math.Clamp(y, 0, sourcePixelHeight);
            Width = Math.Clamp(width, 0, sourcePixelWidth - X);
            Height = Math.Clamp(height, 0, sourcePixelHeight - Y);

            Points = sourcePoints?
                .Select(point => new AnnotationPointData
                {
                    X = Math.Clamp(point.X, 0, sourcePixelWidth),
                    Y = Math.Clamp(point.Y, 0, sourcePixelHeight)
                })
                .ToList()
                ?? new List<AnnotationPointData>();

            RecalculateNormalizedGeometry();
        }

        /// <summary>
        /// Recalculates normalized geometry from the source-pixel geometry.
        /// </summary>
        public void RecalculateNormalizedGeometry()
        {
            if (SourcePixelWidth <= 0 || SourcePixelHeight <= 0)
            {
                NormalizedX = 0;
                NormalizedY = 0;
                NormalizedWidth = 0;
                NormalizedHeight = 0;
                NormalizedPoints = new List<AnnotationPointData>();
                return;
            }

            NormalizedX = Normalize(X, SourcePixelWidth);
            NormalizedY = Normalize(Y, SourcePixelHeight);
            NormalizedWidth = Normalize(Width, SourcePixelWidth);
            NormalizedHeight = Normalize(Height, SourcePixelHeight);

            NormalizedPoints = (Points ?? new List<AnnotationPointData>())
                .Select(point => new AnnotationPointData
                {
                    // NormalizedPointX = SourcePointX / SourcePixelWidth
                    X = Normalize(point.X, SourcePixelWidth),

                    // NormalizedPointY = SourcePointY / SourcePixelHeight
                    Y = Normalize(point.Y, SourcePixelHeight)
                })
                .ToList();
        }

        private static double Normalize(double value, int sourceDimension)
        {
            if (sourceDimension <= 0)
                return 0;

            return Math.Clamp(value / sourceDimension, 0.0, 1.0);
        }
        /// <summary>
        /// PNG image containing the selected annotation region.
        /// System.Text.Json serializes this byte array as Base64.
        /// </summary>
        public byte[] SelectionImagePng { get; set; }

        [JsonIgnore]
        public BitmapSource SelectionImage
        {
            get
            {
                if (SelectionImagePng == null ||
                    SelectionImagePng.Length == 0)
                {
                    return null;
                }

                try
                {
                    using var stream =
                        new MemoryStream(SelectionImagePng, writable: false);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }
        }
    } // End of ImageAnnotation class

    public sealed class AnnotationPointData
    {
        public double X { get; set; }

        public double Y { get; set; }
    }
    public sealed class ImageAnnotationReference  // For use in search results and other contexts where only a reference to an annotation is needed.
    {
        public Guid DeckId { get; set; }

        public string DeckName { get; set; } = string.Empty;

        public string DeckVersion { get; set; } = string.Empty;

        public int CardNumber { get; set; }

        public Guid AnnotationId { get; set; }
    }
    public sealed class AnnotationThumbnail
    {
        public int Index { get; set; }

        public string Caption { get; set; } = string.Empty;

        public BitmapSource Thumbnail { get; set; }
    }

    public sealed class AnnotationSearchResult
    {
        public string RootRelativePath { get; set; } = string.Empty;

        public string DisplayText { get; set; } = string.Empty;
    }
}

