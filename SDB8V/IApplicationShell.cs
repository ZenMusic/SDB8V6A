using System.Drawing;
using System.Windows.Forms;

namespace SymbolDB
{
    /// <summary>
    /// Operations that WinForms child windows require from the application shell.
    /// This contract keeps those windows independent of the legacy <see cref="Main"/> form.
    /// </summary>
    public interface IApplicationShell
    {
        FileFunctions FileFunctions { get; }
        AudioPlayer Audio { get; }

        string GetTargetFolder();
        void SetTargetFolder(string directoryPath);
        void ActivateShell();
        void FocusShell();
        void SetShellTitle(FileInfoItem fileInfo, string text);
        void SetFileType(string fileType);
        void SetImageList(ImageFileList imageFileList);
        bool DisplayImage(string filePath);
        void ShowDisplayNames(bool show);
        void SaveHistoryList();
        void SaveInitParameters();
        void PlayAlert(int soundNumber);
        Bitmap CreateThumbnailForFile(string filePath);
        bool ShowBrowserAndDisplayLocalFile(string filePath);
        bool VerifyFolderExists(string folderPath);

        string CreateNotesFileName(int fileNumber);
        int CreateDefaultInitParameterFile(string? filePath = null);
        void AssignInitParametersFromGlobals();
        void HideLoadingParameters();

        void MarkForDeletion(bool delete);
        void ContinueAfterDelete(bool continueOperation);
    }
}
