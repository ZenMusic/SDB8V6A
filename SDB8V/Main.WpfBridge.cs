using System;

namespace SymbolDB
{
    public partial class Main
    {
        FileFunctions IApplicationShell.FileFunctions => ff;
        AudioPlayer IApplicationShell.Audio => audio;

        void IApplicationShell.SetTargetFolder(string directoryPath) => setTargetFolder(directoryPath);

        void IApplicationShell.ActivateShell() => Activate();

        void IApplicationShell.FocusShell() => Focus();

        void IApplicationShell.SetShellTitle(FileInfoItem fileInfo, string text) => setTitle(fileInfo, text);

        void IApplicationShell.SetImageList(ImageFileList imageFileList) => SetImageList3(imageFileList);

        bool IApplicationShell.DisplayImage(string filePath) => displayThisImage1(filePath);

        void IApplicationShell.SaveInitParameters() => saveInitParms();

        void IApplicationShell.PlayAlert(int soundNumber) => soundAlert(soundNumber);

        int IApplicationShell.CreateDefaultInitParameterFile(string? filePath) =>
            createDefaultInitParm1File(filePath);

        void IApplicationShell.AssignInitParametersFromGlobals() => assignInit1ParmsFromGlobals();

        void IApplicationShell.HideLoadingParameters() => HideLoadingParms();

        void IApplicationShell.MarkForDeletion(bool delete) => markForDeletion(delete);

        void IApplicationShell.ContinueAfterDelete(bool continueOperation) =>
            DelReturnCodeContinue(continueOperation);

        internal string WpfTargetFolder => tbTargetFolder.Text;

        internal bool WpfCanSaveImageList => gv.imageFileList1?.finfoList?.Count > 0;

        internal void WpfSetTargetFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            setTargetFolder(folderPath);
        }

        internal void WpfSetCatalogMode(bool enabled)
        {
            cbCatalogImages.Checked = enabled;
        }

        internal void WpfSetSelectedDisplays(
            bool display1Selected,
            bool display2Selected,
            bool display3Selected,
            bool display4Selected,
            bool displayIndexSelected)
        {
            // Preserve display selection without opening the legacy slideshow display forms.
            bScreen1 = display1Selected;
            bScreen2 = display2Selected;
            bScreen3 = display3Selected;
            bScreen4 = display4Selected;
            cbDisplay1idx3.Checked = displayIndexSelected;
        }

        internal void WpfShowDisplayNames(bool show)
        {
            ShowDisplayNames(show);
        }

        internal void WpfOpenProgramParameters()
        {
            OpenDialogParms();
        }

        internal void WpfOpenNotes()
        {
            btEditNotes_Click(this, EventArgs.Empty);
        }

        internal void WpfSaveImageList()
        {
            btFileCreate_Click(this, EventArgs.Empty);
        }

        internal void WpfOpenTraverser(string targetFolder)
        {
            WpfSetTargetFolder(targetFolder);
            buttonTraverse_Click(this, EventArgs.Empty);
        }
    }
}
