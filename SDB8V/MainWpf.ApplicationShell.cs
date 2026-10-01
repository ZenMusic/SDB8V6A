using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SymbolDB
{
    public partial class MainWpf
    {
        private AllDisplaysInfoForm? _allDisplaysInfoForm;
        private DialogParms? _programParametersWindow;
        private EditNotes? _notesWindow;
        private DialogTraverser? _traverserWindow;

        public FileFunctions FileFunctions => _applicationController.FileFunctions;

        public string GetTargetFolder() => _globals.initParm1List[0].targetDir1;
        // Add near the other fields:
        private readonly AudioPlayer _audio = new();


        public void SetTargetFolder(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                return;

            _globals.targetFolder = directoryPath;
            _globals.initParm1List[0].SetTargetDir1(directoryPath);
            if (!string.Equals(TargetFolderTextBox.Text, directoryPath, StringComparison.Ordinal))
                TargetFolderTextBox.Text = directoryPath;
        }

        public void ActivateShell() => Dispatcher.Invoke(() =>
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
            Activate();
        });

        public void FocusShell() => Dispatcher.Invoke(() => Focus());

        public void SetShellTitle(FileInfoItem fileInfo, string text) =>
            Dispatcher.Invoke(() => Title = string.IsNullOrWhiteSpace(text)
                ? fileInfo?.fpath ?? "SymbolDB"
                : text);

        public void SetFileType(string fileType)
        {
            _globals.FILE_TYPE = fileType;
            _globals.searchExtensionCategoryInUse = fileType;
        }

        public void SetImageList(ImageFileList imageFileList)
        {
            _globals.imageFileList1 = imageFileList;
            _globals.bImageFileList1Loaded = imageFileList?.finfoList?.Count > 0;
        }
        // Add near FileFunctions:
        public AudioPlayer Audio => _audio;

        // Add after SetImageList:
        public bool DisplayImage(string filePath) => OpenLocalFile(filePath);
        public void SaveHistoryList()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_globals.folderHistoryFileFullPathName)!);
            var serializer = new XmlSerializer(typeof(List<FolderHistory>));
            using FileStream stream = File.Create(_globals.folderHistoryFileFullPathName);
            serializer.Serialize(stream, _globals.folderHistoryList.Distinct().ToList());
        }

        public void SaveInitParameters()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_globals.inifileFullPathName)!);
            var serializer = new XmlSerializer(typeof(List<InitParms1>));
            using FileStream stream = File.Create(_globals.inifileFullPathName);
            serializer.Serialize(stream, _globals.initParm1List);
        }

        public void PlayAlert(int soundNumber) => SystemSounds.Asterisk.Play();

        public Bitmap CreateThumbnailForFile(string filePath)
        {
            using Image source = Image.FromFile(filePath);
            var thumbnail = new Bitmap(320, 240);
            using Graphics graphics = Graphics.FromImage(thumbnail);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.Clear(Color.Black);

            float scale = Math.Min(320f / source.Width, 240f / source.Height);
            int width = Math.Max(1, (int)(source.Width * scale));
            int height = Math.Max(1, (int)(source.Height * scale));
            graphics.DrawImage(source, (320 - width) / 2, (240 - height) / 2, width, height);
            return thumbnail;
        }
        // Replace the existing void ShowBrowserAndDisplayLocalFile:
        public bool ShowBrowserAndDisplayLocalFile(string filePath) =>
            OpenLocalFile(filePath);

        private static bool OpenLocalFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            try
            {
                Process.Start(new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                });

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ShowBrowserAndDisplayLocalFilexxxxx(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The selected file was not found.", filePath);

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        public bool VerifyFolderExists(string folderPath) => Directory.Exists(folderPath);

        public string CreateNotesFileName(int fileNumber) =>
            $"SDB{_globals.notesBaseName}{fileNumber}.txt";

        public int CreateDefaultInitParameterFile(string? filePath = null)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
                _globals.inifileFullPathName = Directory.Exists(filePath)
                    ? Path.Combine(filePath, GlobalVars.initFileName)
                    : filePath;

            SaveInitParameters();
            return _globals.initParm1List.Count;
        }

        public void AssignInitParametersFromGlobals()
        {
            foreach (InitParms1 parameters in _globals.initParm1List)
                parameters.SetGlobalVars(_globals);
        }

        public void HideLoadingParameters()
        {
        }

        public void MarkForDeletion(bool delete) =>
            SetStatus(delete ? "The selected item is marked for deletion." : "Deletion mark cleared.");

        public void ContinueAfterDelete(bool continueOperation) =>
            SetStatus(continueOperation ? "Deletion confirmed." : "Deletion canceled.");

        private void OpenTraverser(string targetFolder)
        {
            SetTargetFolder(targetFolder);
            if (_traverserWindow is null || _traverserWindow.IsDisposed)
                _traverserWindow = new DialogTraverser(_globals, this, 0, FileFunctions);

            _traverserWindow.SettargetDirToThis(targetFolder);
            _traverserWindow.Show();
            _traverserWindow.Activate();
        }

        private void OpenProgramParameters()
        {
            if (_programParametersWindow is null || _programParametersWindow.IsDisposed)
                _programParametersWindow = new DialogParms(_globals);
            _programParametersWindow.Show();
            _programParametersWindow.Activate();
        }

        private void OpenNotes()
        {
            if (_notesWindow is null || _notesWindow.IsDisposed)
                _notesWindow = new EditNotes(_globals);
            _notesWindow.Show();
            _notesWindow.Activate();
        }

        private void SaveImageList()
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "XML files (*.xml)|*.xml",
                DefaultExt = "xml",
                AddExtension = true
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var serializer = new XmlSerializer(typeof(List<FileInfoItem>));
            using FileStream stream = File.Create(dialog.FileName);
            serializer.Serialize(stream, _globals.imageFileList1.getFinfo());
        }

        public void ShowDisplayNames(bool show)
        {
            if (!show)
            {
                _allDisplaysInfoForm?.Close();
                _allDisplaysInfoForm = null;
                return;
            }

            if (_allDisplaysInfoForm is null || _allDisplaysInfoForm.IsDisposed)
            {
                _allDisplaysInfoForm = new AllDisplaysInfoForm(_globals);
                _allDisplaysInfoForm.FormClosed += (_, _) => _allDisplaysInfoForm = null;
                _allDisplaysInfoForm.PopulateScreens();
                _allDisplaysInfoForm.Show();
            }
            else
            {
                _allDisplaysInfoForm.BringToFront();
            }
        }
    }
}
