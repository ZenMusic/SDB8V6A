using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SymbolDB
{
    /// <summary>
    /// Owns application state that previously existed only because the legacy Main form was constructed.
    /// </summary>
    public sealed class ApplicationController
    {
        public ApplicationController(IApplicationShell applicationShell)
        {
            ArgumentNullException.ThrowIfNull(applicationShell);

            Globals = new GlobalVars(applicationShell)
            {
                appName = Application.ProductName,
                folderHistoryList = new List<FolderHistory>()
            };

            FileFunctions = new FileFunctions(Globals);
            FileFunctions.setFoldersForApp(Globals.bRunningInDevelopment);
            Globals.folderHistoryFileFullPathName = Path.Combine(
                Globals.dataFolder,
                GlobalVars.historyFileName);

            ConfigureScreens();
            LoadFolderHistory();
            LoadInitParameters();
            DialogSearchExtensionsEditor.LoadFromFile(Globals);
        }

        public GlobalVars Globals { get; }

        public FileFunctions FileFunctions { get; }

        public string TargetFolder => Globals.initParm1List[0].targetDir1;

        private void ConfigureScreens()
        {
            Screen[] screens = Screen.AllScreens
                .OrderBy(GetDisplayNumber)
                .ToArray();

            Globals.displayCount = screens.Length;
            for (int index = 0; index < Globals.screen.Length; index++)
                Globals.screen[index] = index < screens.Length ? screens[index] : null;
        }

        private void LoadFolderHistory()
        {
            string path = Globals.folderHistoryFileFullPathName;
            if (!File.Exists(path))
                return;

            try
            {
                var serializer = new XmlSerializer(typeof(List<FolderHistory>));
                using FileStream stream = File.OpenRead(path);
                Globals.folderHistoryList =
                    serializer.Deserialize(stream) as List<FolderHistory> ?? new List<FolderHistory>();
            }
            catch (InvalidOperationException)
            {
                Globals.folderHistoryList = new List<FolderHistory>();
            }
        }

        private void LoadInitParameters()
        {
            if (File.Exists(Globals.inifileFullPathName))
                FileFunctions.readInitParms1File(Globals.inifileFullPathName);

            if (Globals.initParm1List.Count == 0)
                Globals.initParm1List.Add(CreateDefaultInitParameters());

            InitParms1 parameters = Globals.initParm1List[0];
            parameters.SetGlobalVars(Globals);

            string fallbackFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (string.IsNullOrWhiteSpace(fallbackFolder) || !Directory.Exists(fallbackFolder))
                fallbackFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            parameters.sourceDir1 = DefaultFolder(parameters.sourceDir1, fallbackFolder);
            parameters.sourceDir2 = DefaultFolder(parameters.sourceDir2, fallbackFolder);
            parameters.sourceDir3 = DefaultFolder(parameters.sourceDir3, fallbackFolder);
            parameters.sourceDir4 = DefaultFolder(parameters.sourceDir4, fallbackFolder);
            parameters.sourceDir5 = DefaultFolder(parameters.sourceDir5, fallbackFolder);
            parameters.targetDir1 = DefaultFolder(parameters.targetDir1, fallbackFolder);
            parameters.targetDir2 = DefaultFolder(parameters.targetDir2, fallbackFolder);
            parameters.targetDir3 = DefaultFolder(parameters.targetDir3, fallbackFolder);
            parameters.watchFolder1 = DefaultFolder(parameters.watchFolder1, fallbackFolder);

            Globals.targetFolder = parameters.targetDir1;
            Globals.watchFolderPath = parameters.watchFolder1;
            Globals.ratingsImages = parameters.ratingsImagesFile ?? Globals.ratingsImages;
            Globals.ratingsVideos = parameters.ratingsVideosFile ?? Globals.ratingsVideos;
        }

        private InitParms1 CreateDefaultInitParameters()
        {
            var parameters = new InitParms1(Globals)
            {
                ratingsImagesFile = Globals.ratingsImages,
                ratingsVideosFile = Globals.ratingsVideos,
                notesBaseName = Globals.notesBaseName
            };
            return parameters;
        }

        private static string DefaultFolder(string? folderPath, string fallbackFolder) =>
            string.IsNullOrWhiteSpace(folderPath) ? fallbackFolder : folderPath;

        private static int GetDisplayNumber(Screen screen)
        {
            const string marker = "DISPLAY";
            int markerIndex = screen.DeviceName.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
            return markerIndex >= 0 &&
                   int.TryParse(screen.DeviceName[(markerIndex + marker.Length)..], out int number)
                ? number
                : int.MaxValue;
        }
    }
}
