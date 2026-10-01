using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using WinForms = System.Windows.Forms;
using System.Collections.Generic; // <--

namespace SymbolDB
{
    public partial class MainWpf : Window, IApplicationShell
    {
        private readonly ApplicationController _applicationController;
        private readonly GlobalVars _globals;
        private Database? _databaseWindow;
        private bool _isClosing;

        public MainWpf(DatabaseStatus databaseStatus)
        {
            ArgumentNullException.ThrowIfNull(databaseStatus);

            InitializeComponent();

            const string instructions = "Click Traverse Folder to open a multimedia folder";
            InstructionsTextBox.Text = instructions;
            _applicationController = new ApplicationController(this);
            _globals = _applicationController.Globals;

            TargetFolderTextBox.Text = _applicationController.TargetFolder;
            VersionTextBlock.Text = GetVersionText();
            DisplayDatabaseStatus(databaseStatus);
            UpdateWindowSizeText();
            UpdateTargetFolderStatus();

            SizeChanged += (_, _) => UpdateWindowSizeText();
            Closed += MainWpf_Closed;
        }

        private static int GetDisplayNumber(WinForms.Screen screen)
        {
            const string marker = "DISPLAY";

            string deviceName = screen.DeviceName;
            int markerIndex = deviceName.LastIndexOf(
                marker,
                StringComparison.OrdinalIgnoreCase);

            if (markerIndex < 0)
                return 0;

            string numberText = deviceName[(markerIndex + marker.Length)..];
            return int.TryParse(numberText, out int displayNumber)
                ? displayNumber
                : 0;
        }
        public static MainWpf ShowFromWinForms(DatabaseStatus databaseStatus)
        {
            var window = new MainWpf(databaseStatus);

            ElementHost.EnableModelessKeyboardInterop(window);
            window.Show();
            return window;
        }

        private void RefreshDatabaseStatusButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayDatabaseStatus(SqliteDb.GetStatus());
        }

        private void OpenDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_databaseWindow is { IsDisposed: false })
            {
                _databaseWindow.WindowState = WinForms.FormWindowState.Normal;
                _databaseWindow.Activate();
                return;
            }

            _databaseWindow = new Database();
            _databaseWindow.Show();
            _databaseWindow.Activate();
            _databaseWindow.BringToFront();
        }

        private void DisplayDatabaseStatus(DatabaseStatus databaseStatus)
        {
            DatabasePathTextBox.Text = databaseStatus.DatabasePath;
            DatabaseSchemaTextBox.Text = databaseStatus.SchemaSummary;
            DatabaseConnectionTextBlock.Text = databaseStatus.Message;
            DatabaseConnectionTextBlock.Foreground = databaseStatus.IsConnected
                ? System.Windows.Media.Brushes.DarkGreen
                : System.Windows.Media.Brushes.Firebrick;
        }

        private static string GetVersionText()
        {
            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            return version == null ? "SymbolDB" : $"v{version.ToString(3)}";
        }

        private void ShowDisplayNamesCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                bool showDisplayNames = ShowDisplayNamesCheckBox.IsChecked == true;
                ShowDisplayNames(showDisplayNames);
                SetStatus(showDisplayNames
                    ? "Display information shown on each monitor."
                    : "Display information hidden.");
            }
            catch (Exception ex)
            {
                SetStatus($"Unable to show display information: {ex.Message}", true);
                ShowDisplayNamesCheckBox.IsChecked = false;
            }
            bool show  =  ShowDisplayNamesCheckBox.IsChecked == true;
            try { ShowMonitors(show); } catch { }

        }
        private readonly Dictionary<string, ShowDisplay> _monitorLabels =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Shows an identification window on each monitor using that monitor's
        /// device name and exact virtual-desktop bounds.
        /// </summary>
        public void ShowMonitors(bool show)
        {
            CloseMonitorLabels();

            if (!show)
                return;

            foreach (Screen screen in Screen.AllScreens)
            {
                int displayNumber = GetDeviceDisplayNumber(screen.DeviceName);
                var labelWindow = new ShowDisplay(
                    displayNumber,
                    screen.Bounds.Width,
                    screen.Bounds.Height)
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = screen.Bounds.Location
                };

                labelWindow.YourLocation(screen.Bounds.Location);
                labelWindow.FormClosed += (_, _) =>
                    _monitorLabels.Remove(screen.DeviceName);

                _monitorLabels[screen.DeviceName] = labelWindow;
                _globals.debug.w(
                    $"{screen.DeviceName}: bounds={screen.Bounds}, workingArea={screen.WorkingArea}");

                labelWindow.Show();
                labelWindow.Activate();
            }
        }

        private void CloseMonitorLabels()
        {
            foreach (ShowDisplay labelWindow in new List<ShowDisplay>(_monitorLabels.Values))
            {
                if (!labelWindow.IsDisposed)
                    labelWindow.Close();
            }

            _monitorLabels.Clear();
        }

        private static int GetDeviceDisplayNumber(string? deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                return 0;

            const string marker = "DISPLAY";
            int markerIndex = deviceName.LastIndexOf(
                marker,
                StringComparison.OrdinalIgnoreCase);

            return markerIndex >= 0 &&
                   int.TryParse(deviceName[(markerIndex + marker.Length)..], out int displayNumber)
                ? displayNumber
                : 0;
        }
        //

        private void SizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            SizeToContent = SizeToContent.WidthAndHeight;
            SizeToContent = SizeToContent.Manual;
            UpdateWindowSizeText();
        }

        private void Resize1080Button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            Width = 1920;
            Height = 1080;
            UpdateWindowSizeText();
        }

        private void SetTargetFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using var folderDialog = new WinForms.FolderBrowserDialog
            {
                Description = "Select the target folder",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
                SelectedPath = Directory.Exists(TargetFolderTextBox.Text)
                    ? TargetFolderTextBox.Text
                    : string.Empty
            };

            if (folderDialog.ShowDialog() != WinForms.DialogResult.OK)
                return;

            TargetFolderTextBox.Text = folderDialog.SelectedPath;
            ApplyTargetFolder();
        }

        private void TargetFolderTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateTargetFolderStatus();
        }

        private void CatalogImagesCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            _globals.bImageFileList1Loaded = CatalogImagesCheckBox.IsChecked == true;
        }

        private void TraverseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ApplyTargetFolder())
                return;

            try
            {
                OpenTraverser(TargetFolderTextBox.Text);
                SetStatus("Traverser opened.");
            }
            catch (Exception ex)
            {
                SetStatus($"Unable to open the traverser: {ex.Message}", true);
            }
        }

        private void SaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_globals.imageFileList1?.finfoList?.Count <= 0)
            {
                SetStatus("There is no loaded image list to save.", true);
                return;
            }

            try
            {
                SaveImageList();
                SetStatus("Image list save completed or was canceled.");
            }
            catch (Exception ex)
            {
                SetStatus($"Unable to save the image list: {ex.Message}", true);
            }
        }

        private void ProgramParametersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenProgramParameters();
            }
            catch (Exception ex)
            {
                SetStatus($"Unable to open program parameters: {ex.Message}", true);
            }
        }

        private void NotesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenNotes();
            }
            catch (Exception ex)
            {
                SetStatus($"Unable to open notes: {ex.Message}", true);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool ApplyTargetFolder()
        {
            string folderPath = TargetFolderTextBox.Text.Trim();
            if (!Directory.Exists(folderPath))
            {
                SetStatus("Select an existing target folder before continuing.", true);
                return false;
            }

            SetTargetFolder(folderPath);
            SetStatus($"Target folder: {folderPath}");
            return true;
        }

        private void ValidateFolderButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTargetFolderStatus();
        }
        private void UpdateTargetFolderStatus()
        {
            string folderPath = TargetFolderTextBox.Text.Trim();
            bool folderExists = Directory.Exists(folderPath);

            TargetFolderStatus.Text = folderExists
                ? "folder is valid"
                : "folder not found";

            if (string.IsNullOrEmpty(folderPath))
            {
                SetStatus("Choose a target folder, then open the traverser.");
                return;
            }

            SetStatus(
                folderExists
                    ? $"Target folder: {folderPath}"
                    : "The target folder does not exist.",
                !folderExists);
        }
        private void UpdateTargetFolderStatuxxxxs()
        {
            string folderPath = TargetFolderTextBox.Text.Trim();
            if (string.IsNullOrEmpty(folderPath))
            {
                SetStatus("Choose a target folder, then open the traverser.");
                return;
            }

            SetStatus(
                Directory.Exists(folderPath)
                    ? $"Target folder: {folderPath}"
                    : "The target folder does not exist.",
                !Directory.Exists(folderPath));
        }

        private void UpdateWindowSizeText()
        {
            SizeWindowButton.Content = $"{Math.Round(ActualWidth)} × {Math.Round(ActualHeight)}";
        }
        private void SetStatus(string message, bool isError = false)
        {
            StatusTextBlock.Text =
                $"{message}{Environment.NewLine}Application data folder: {_globals.dataFolder}";

            StatusTextBlock.Foreground = isError
                ? System.Windows.Media.Brushes.Firebrick
                : System.Windows.Media.Brushes.DarkSlateGray;

            StatusTextBlock.Foreground = System.Windows.Media.Brushes.White;

        }

        private void SetStatusWas(string message, bool isError = false)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Foreground = isError
                ? System.Windows.Media.Brushes.Firebrick
                : System.Windows.Media.Brushes.DarkSlateGray;
        }

        private void MainWpf_Closed(object? sender, EventArgs e)
        {
            if (_isClosing)
                return;

            _isClosing = true;

            try
            {
                ShowDisplayNames(false);
            }
            finally
            {
                WinForms.Application.ExitThread();
            }
        }
    }
}
