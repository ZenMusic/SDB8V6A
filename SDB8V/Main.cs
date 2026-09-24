//backup v2 restored to main
#nullable disable
using LibVLCSharp.Shared;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using SymbolDB;
using SymbolDB.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Xml.Serialization;
using static SymbolDB.WindowsThumbnailProvider;
using Path = System.IO.Path;

/*
 * The events in the lifecycle of a Form from the time it is launched to the time it is closed are listed below:
•Load: This event occurs before a form is displayed for the first time.
•Move: This event occurs when the form is moved. Although by default, when a form is instantiated and launched, the user does not move it, yet this event is triggered before the Load event occurs.
•VisibleChanged: This event occurs when the Visible property value changes.
•Activated: This event occurs when the form is activated in code or by the user.
•Shown: This event occurs whenever the form is first displayed. 
•Paint: This event occurs when the control is redrawn.
•Deactivate: This event occurs when the form loses focus and is not the active form.
•Closing: This event occurs when the form is closing.
•Closed: This event occurs when the form is being closed.
*/
// 10/03/2025  ratings adding


namespace SymbolDB
{

    public partial class Main : Form
    {
        private readonly FileInfoRepository _repo = new FileInfoRepository();

        public bool bMoveImages = false;

        // ---- SoundPlayer plays a chord.
        //System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chord.wav");
        // ---  SoundPlayer plays TaDa.
        // System.Media.SoundPlayer finishSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav");
        //private string imgFile = null;
        //System.Media.SoundPlayer player; //= new System.Media.SoundPlayer(@"c:\mywavfile.wav");
        GlobalVars gv; //= new GlobalVars();
                       // WebBrowser2 browser2;
                       //Browser1 wb1;

        //Browser2 wb2;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        //---- 
        //--- SLIDESHOW (manual or auto)
        //
        //int nextSlide = 0;
        // int currentSlide = 0;
        Boolean showImage = false;
        public FileInfoItem finfo1 = new FileInfoItem();
        public FileInfoItem iinfo2 = new FileInfoItem();
        public FileInfoItem iinfoNextImage = new FileInfoItem();

        Label label = new Label();
        Display1[] display = new Display1[3]; // display array loadi
        Display1 display1;
        Display1 display2;
        Display1 display3;
        Display1 display4;
        Boolean slideSorter = false;
        DisplayPreviewSet displayPreviewSetForm = null;
        Boolean bScreen1 = false;
        Boolean bScreen2 = false;
        Boolean bScreen3 = false;
        Boolean bScreen4 = false;
        bool bScreen1Focus = false;
        bool bScreen2Focus = false;
        bool bScreen3Focus = false;
        bool bScreen4Focus = false;
        Boolean bAllowScreen2 = true;
        Boolean bAllowScreen3 = true;
        Boolean bAllowScreen4 = true;
        Boolean bScan = false;
        //current image FULLPATH
        string fpathCurrentImage;

        public FileFunctions ff;
        //---------SCREEENS
        ScreensInfo screensInfo;
        System.Drawing.Point listViewLoc;

        ImageFileList deletedItems;

        Color tagBackgroundColor = Color.Gray;
        public void ErrorStop()
        {
            ErrorStop m = new ErrorStop();
            m.ShowDialog();
            var value = m.Text;
            this.Close();
        }
        string appRunningFrom = System.Windows.Forms.Application.ExecutablePath;
        // Add these fields near your other class-level variables (around line 150)
        private readonly Queue<(Bitmap Image, long FileSize, int Index)> _recentImagesCache
            = new Queue<(Bitmap, long, int)>();
        private const int CACHE_SIZE = 8;  // Small groups Check against last 8 images
        //private const int CACHE_SIZE = 20; // Medium batches
        //private const int CACHE_SIZE = 50; // Large file size clusters
        string DBpath = SqliteDb.DbPath;

        public AudioPlayer audio = new AudioPlayer();
        string defaultAudioSound2 = @"C:\Windows\Media\ding.wav";
        string defaultAudioSound = @"C:\Windows\Media\Windows Hardware Fail.wav";

        // ── FindMatchingWindow (lazy, reusable) ──────────────────────────────
        private FindMatchingWindow _findMatchingWindow;

        /// <summary>Returns the image currently displayed in pb1 (used by FindMatchingWindow).</summary>
     //   public Image CurrentMainImage => pb1.Image;

        public Main() //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            //
            InitializeComponent();
            //

            string appName = Application.ProductName;
            gv = new GlobalVars(this);
            gv.appName = appName;
            //gv.bRunningInDevelopment = System.Diagnostics.Debugger.IsAttached;
            // ff.setFoldersForApp(gv.bRunningInDevelopment);


            pbMatch2.SizeMode = PictureBoxSizeMode.Zoom;
            btImageErrors.Click += btImageErrors_Click;
            UpdateImageErrorsCount();
            dgvCompare.Visible = false;
            //System.Windows.Forms.MessageBox.Show(SqliteDb.ConnectionString, "DB in use");
            tbMessage.Text = $"DB = {SqliteDb.DbPath}";
            //tbMessage.Text = $"{SqliteDb.DbPath} {SqliteDb.ConnectionString}";
            tbFpath.Text = "schema version and target " + SqliteDb.GetSchemaVersionInfo().ToString();

            //C:\Users\david\AppData\Local\SDB7\SDB7.db
            DBconnect();

            dgvFileInfo.Size = new System.Drawing.Size(581, 332);
            PreSetupDGV(); //2025
            btCopyImageInfo.Visible = false;
            this.Text = "starting ... init";

            gv.soundVolume = Settings.Default.volumeLevel;


            audio.Volume = gv.soundVolume;

            //MessageBox.Show("start");
            if (string.IsNullOrEmpty(gv.logFilePath))
            {
                gv.logFilePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "_dazen",
                    "SymbolDB.log"
                );
            }
            mainInit();
            //ff.setFoldersForApp(gv.bRunningInDevelopment);

            CreateMostRecentImageList();
            cbPanel1.Checked = true;
            cbPanel1.Checked = false;
            cbPanel2.Checked = true;
            cbPanel2.Checked = false;
            tbFilePath3.Text = appRunningFrom;
            bool safe = VerifySafeDemoImage();
            if (!safe)
                btHide.BackColor = Color.DarkGray;
            this.Text = $"completed init {gv.inifileFullPathName}";

            SoundLevel(50);
        }
        public bool bDebugStartup = false;
        public bool bUseLocalApplicationData = false;
        public bool bMigrateApplicationData = false;
        public string tempInitFileFullPath = "C:\\Users\\david\\OneDrive\\Documents\\_dazen\\sdb8.xml";


        private void tbRowCount_TextChanged(object sender, EventArgs e)
        {
            // Add your implementation here
        }




        // ══════════════════════════════════════════════════════════════════════
        //  Public API – called by FindMatchingWindow and similar child windows
        // ══════════════════════════════════════════════════════════════════════

        // ── expose repository to child windows ──────────────────────────────
        /// <summary>The shared FileInfoRepository used by this form and child windows.</summary>
        public FileInfoRepository Repo => _repo;

        /// <summary>
        /// Sets the max-slide-number textbox (tbMaxSlideNumber) on the Main form.
        /// Typically called with <c>$"{gv.slideCount1 - 1}"</c>.
        /// Thread-safe.
        /// </summary>
        public void SetMaxSlideText222(string text)
        {
            if (tbMaxSlideNumber.InvokeRequired)
                tbMaxSlideNumber.Invoke(() => tbMaxSlideNumber.Text = text);
            else
                tbMaxSlideNumber.Text = text;
        }


        /// <summary>Sets the image-count textbox (tbMaxSlideNumber) on the Main form.</summary>
        public void SetImageCountText222(string text)
        {
            if (tbMaxSlideNumber.InvokeRequired)
                tbMaxSlideNumber.Invoke(() => tbMaxSlideNumber.Text = text);
            else
                tbMaxSlideNumber.Text = text;
        }

        /// <summary>Sets the go-to-slide textbox (tbGoToSlide) on the Main form.</summary>
        public void SetGoToSlideText222(string text)
        {
            if (tbGoToSlide.InvokeRequired)
                tbGoToSlide.Invoke(() => tbGoToSlide.Text = text);
            else
                tbGoToSlide.Text = text;
        }

        /// <summary>
        /// Programmatically triggers slide navigation — equivalent to clicking btGoToSlideNumber.
        /// </summary>
        public void DoGoToSlideNumber222()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(DoGoToSlideNumber);
                return;
            }

            if (string.IsNullOrEmpty(tbGoToSlide.Text))
                tbGoToSlide.Text = "0";

            if (tbGoToSlide.Text.Equals("0"))
                cbFindDuplicates.Checked = false;

            GoToSlideNumber();
            enteringSlideNumber = false;
        }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><


        public void SetAudioVolumeLevel(int volume)
        {
            audio.Volume = volume;
            gv.soundVolume = volume;
            Properties.Settings.Default.volumeLevel = volume;
            Properties.Settings.Default.Save();
        }
        public bool isDebugEnv = System.Diagnostics.Debugger.IsAttached;

        public void mainInit()
        {
            if (bDebugStartup)
                MessageBox.Show($"mainInit");

            // ============================================================================
            // DETERMINE INIT FILE LOCATION BASED ON ENVIRONMENT        NOW DONE IN MAIN
            // ============================================================================




            string MachineName = Environment.MachineName;



            // ============================================================================
            // MIGRATION: Copy old init file to new location if needed
            // ============================================================================
            if (bMigrateApplicationData && !string.IsNullOrWhiteSpace(tempInitFileFullPath))
            {
                try
                {
                    // ==========================================
                    // MIGRATE INIT FILE (sdb8.xml)
                    // ==========================================
                    if (File.Exists(tempInitFileFullPath))
                    {
                        if (!File.Exists(gv.inifileFullPathName))
                        {
                            File.Copy(tempInitFileFullPath, gv.inifileFullPathName, overwrite: false);

                            tbMessage.Text = $"Migrated init file from:\n{tempInitFileFullPath}\nto:\n{gv.inifileFullPathName}";
                            tbMessage.BackColor = Color.LightGreen;

                            if (bDebugStartup)
                            {
                                MessageBox.Show(
                                    $"Successfully migrated init file:\n\nFrom: {tempInitFileFullPath}\n\nTo: {gv.inifileFullPathName}",
                                    "Migration Complete",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }
                        }
                        else
                        {
                            if (bDebugStartup)
                            {
                                MessageBox.Show(
                                    $"Init file already exists at new location:\n{gv.inifileFullPathName}\n\nSkipping migration.",
                                    "Migration Skipped",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }
                        }
                    }
                    else
                    {
                        if (bDebugStartup)
                        {
                            MessageBox.Show(
                                $"Source init file not found:\n{tempInitFileFullPath}\n\nNo migration performed.",
                                "Migration Info",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                    }

                    // ==========================================
                    // MIGRATE FOLDER HISTORY FILE (historyFolderList.xml)
                    // ==========================================

                    // Determine the old history path based on environment
                    string oldHistoryPath;

                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        // DEVELOPMENT MODE: Use Documents\_dazen path
                        oldHistoryPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "_dazen",
                            "historyFolderList.xml"
                        );
                    }
                    else
                    {
                        // PRODUCTION/DISTRIBUTED MODE: Use OneDrive path (user's existing location)
                        oldHistoryPath = @"C:\Users\david\OneDrive\Documents\_dazen\historyFolderList.xml";
                    }

                    if (File.Exists(oldHistoryPath))
                    {
                        // Build destination path using the same directory as the new init file
                        string newHistoryPath = gv.folderHistoryFileFullPathName;

                        if (string.IsNullOrWhiteSpace(newHistoryPath))
                        {
                            // Fallback: construct path based on new init directory
                            string newConfigDir = Path.GetDirectoryName(gv.inifileFullPathName);
                            newHistoryPath = Path.Combine(newConfigDir, "SymbolDB_FolderHistory.xml");
                            gv.folderHistoryFileFullPathName = newHistoryPath;
                        }

                        if (!File.Exists(newHistoryPath))
                        {
                            // Copy the old history file to the new location
                            File.Copy(oldHistoryPath, newHistoryPath, overwrite: false);

                            if (bDebugStartup)
                            {
                                MessageBox.Show(
                                    $"Successfully migrated folder history:\n\nFrom: {oldHistoryPath}\n\nTo: {newHistoryPath}",
                                    "History Migration Complete",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }

                            gv.debug?.w($"Migrated folder history from {oldHistoryPath} to {newHistoryPath}");
                        }
                        else
                        {
                            if (bDebugStartup)
                            {
                                MessageBox.Show(
                                    $"Folder history already exists at:\n{newHistoryPath}\n\nSkipping migration.",
                                    "History Migration Skipped",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }
                        }
                    }
                    else
                    {
                        if (bDebugStartup)
                        {
                            MessageBox.Show(
                                $"Old folder history file not found:\n{oldHistoryPath}\n\nNo migration performed.",
                                "History Migration Info",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }
                        gv.debug?.w($"Old folder history file not found: {oldHistoryPath}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to migrate application data:\n{ex.Message}",
                        "Migration Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    // Continue execution - don't fail the app if migration fails
                    gv.debug?.w($"Migration failed: {ex.Message}");
                }
            }

            if (bDebugStartup)
                MessageBox.Show($"Init file: {gv.inifileFullPathName}");

            // ============================================================================
            // CONTINUE WITH EXISTING INITIALIZATION
            // ============================================================================

            // SCREENS INFO  
            initScreens();
            this.Location = gv.screen[0].Bounds.Location;
            this.CenterToScreen();
            //
            this.WindowState = FormWindowState.Maximized;
            //
            btDisplayImages.Enabled = true;

            // Suppose these are your two columns:
            // Column 0 (Label), Column 1 (Value).

            // 1) Auto-size the Label column so it fits its content.
            dgvFileInfo.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // 2) Make the Value column fill the remaining space.
            dgvFileInfo.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Optionally, set a minimum width or fill weight for finer control:
            dgvFileInfo.Columns[1].MinimumWidth = 100;
            dgvFileInfo.Columns[1].FillWeight = 1; // default is 100, but 1 works fine for a single fill column

            this.Activate();
            this.Focus();
            gv.mainWindow = this;
            InitializeOpenFileDialog();
            //wb2 = new Browser2(gv);
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer2.Tick += new EventHandler(timer2_Tick); // Everytime timer ticks, timer_Tick will be called
            timer2.Interval = (int)1000;              // Timer will tick every 3 seconds
            timer2.Enabled = true;                       // Enable the timer2
                                                         //  if (!bMasterStopSlideShow)
                                                         //    timer2.Start();                              // Start the timer2

            gv.debug.registerMain(this);
            //gv.debug.Hide();
            this.Focus();
            gv.directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            gv.folderHistoryList = new List<FolderHistory>();

            //gv.bRunningInDevelopment = System.Diagnostics.Debugger.IsAttached;
            ff = new FileFunctions(gv);
            ff.setFoldersForApp(gv.bRunningInDevelopment);

            LoadFolderHistoryList();
            //-------------------GET SCREENS INFO----------------------------------------------------------

            // screensInfo = new ScreensInfo(gv);

            //CreateListViewFinfo(); // use this to display Iinfo for each image

            //initListViewFinfo();
            // initLVFinfo();
            tagBackgroundColor = Color.LightGray;

            lvScreenInfo.BringToFront();

            //tbMessage.Text = getProdVersion();
            if (bDebugStartup) MessageBox.Show("Init File");
            // MessageBox.Show("Startup", initFileName);
            int iparmCount = 0;
            if (bUseLocalApplicationData)
                LoadInitFile();
            else
                iparmCount = this.readInitParms1File();
            //
            //
            //new gpt fix 2025
            if (iparmCount <= 0)
            {
                MessageBox.Show("no parms", initFileName);
                // 1) The file doesn't exist or is invalid, so try auto-creating:
                string initPath = gv.inifileFullPathName;  // or wherever you expected the file
                DialogResult dr = System.Windows.Forms.MessageBox.Show(
                    $"No Init File found at:\n{initPath}\n\nAttempting to create a default Init File...",
                    "Init File Missing",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                );
                if (dr == DialogResult.Cancel)
                {
                    // If user clicks Cancel, we could exit or continue in a "limited" mode
                    // For now, let's just exit:
                    System.Windows.Forms.Application.Exit();
                    return; // prevent further init
                }

                iparmCount = createDefaultInitParm1File();
                if (iparmCount <= 0)
                {
                    // 2) If still failing, prompt user to select a folder manually
                    string newFolder = PromptUserForInitDirectory();
                    if (string.IsNullOrEmpty(newFolder))
                    {
                        // user canceled or error
                        System.Windows.Forms.MessageBox.Show("No folder selected. Cannot create Init File. Exiting.");
                        System.Windows.Forms.Application.Exit();
                        return;
                    }

                    // 3) Retry creation, passing in that folder
                    iparmCount = createDefaultInitParm1File(newFolder);
                    if (iparmCount <= 0)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            $"Could not create default Init File in {newFolder}.\nExiting application.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        System.Windows.Forms.Application.Exit();
                        return;
                    }
                }
            } // END OF FAILED TO OPEN INIT FILE

            gv.initParm1List[0].SetGlobalVars(gv);
            if (iparmCount < 1)
                MessageBox.Show($"init {iparmCount}");
            if (string.IsNullOrEmpty(gv.initParm1List[0].sourceDir5))
                gv.initParm1List[0].sourceDir5 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].sourceDir4))
                gv.initParm1List[0].sourceDir4 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].sourceDir3))
                gv.initParm1List[0].sourceDir3 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].sourceDir2))
                gv.initParm1List[0].sourceDir2 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].sourceDir1))
                gv.initParm1List[0].sourceDir1 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].targetDir1))
                gv.initParm1List[0].SetTargetDir1("C:");
            if (string.IsNullOrEmpty(gv.initParm1List[0].targetDir2))
                gv.initParm1List[0].targetDir2 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].targetDir3))
                gv.initParm1List[0].targetDir3 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].watchFolder1))
                gv.initParm1List[0].watchFolder1 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].watchFolder2))
                gv.initParm1List[0].watchFolder2 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].watchFolder3))
                gv.initParm1List[0].watchFolder3 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].history1))
                gv.initParm1List[0].history1 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].history2))
                gv.initParm1List[0].history2 = "C:";
            if (string.IsNullOrEmpty(gv.initParm1List[0].history3))
                gv.initParm1List[0].history3 = "C:";
            if (iparmCount <= 0)
            {
                MessageBox.Show("No Init File");
                iparmCount = createDefaultInitParm1File();
            }
            gv.ratingsImages = gv.initParm1List[0].ratingsImagesFile;
            gv.ratingsVideos = gv.initParm1List[0].ratingsVideosFile;

            if (bDebugStartup) MessageBox.Show($"Init File {iparmCount}");
            if (iparmCount > 0)
            {
                string watchFolder = gv.initParm1List[0].watchFolder1;

                //watchFolder = gv.initParm1List[0].watchFolder2;

                tbWatch.Text = watchFolder;
                gv.watchFolderPath = watchFolder;

                InitParms1 temp = gv.initParm1List[0];
                if (string.IsNullOrEmpty(temp.targetDir1))
                {
                    this.tbTargetFolder.Text = @"C:\share\aaa";// gv.dirTargetFolder;
                    gv.initParm1List[0].SetTargetDir1(tbTargetFolder.Text);
                    temp.SetTargetDir1(tbTargetFolder.Text);
                    temp.dirPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    gv.initParm1List[0] = temp;
                }
                else
                {
                    //gv.inifilePath path =  //System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    this.tbTargetFolder.Text = temp.targetDir1;
                    gv.initParm1List[0].SetTargetDir1(tbTargetFolder.Text);
                    temp.dirPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    gv.initParm1List[0] = temp;
                }
            }
            else //2025 gpt
            {
                // fallback if somehow iparmCount is still 0
                MessageBox.Show("No Init File and cannot create. Exiting.");
                System.Windows.Forms.Application.Exit();
                return;
            }


            pbNext.SizeMode = PictureBoxSizeMode.Zoom;
            // this.cbActualSize.Checked = true;
            //this.cbActualSize.Focus();
            this.Activate();
            this.Focus();

            cbDisplay3.Checked = false;
            //pb1.Load(gv.dirHomeDir86 + "01.jpg");
            deletedItems = new ImageFileList();

            resizeImage();
            pbFourthLast.SizeMode = PictureBoxSizeMode.Zoom;
            pbThirdLast.SizeMode = PictureBoxSizeMode.Zoom;
            pbSecondLast.SizeMode = PictureBoxSizeMode.Zoom;
            pbLastImage.SizeMode = PictureBoxSizeMode.Zoom;
            pbNext.SizeMode = PictureBoxSizeMode.Zoom;
            pb1ThumbNail.SizeMode = PictureBoxSizeMode.Zoom;
            pb2Image.SizeMode = PictureBoxSizeMode.Zoom;
            // loadInitParmsFileXML();
            getSoundList();
            //player = new System.Media.SoundPlayer(soundFiles[30]);
        }



        private void btImageErrors_Click(object sender, EventArgs e)
        {
            SaveImageErrorsList();
        }

        private void UpdateImageErrorsCount()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateImageErrorsCount));
                return;
            }

            int count = 0;
            try
            {
                if (gv?.imageFileErrorList != null)
                {
                    try { count = gv.imageFileErrorList.getImageFileListLength(); }
                    catch
                    {
                        try { count = gv.imageFileErrorList.getImageCount(); }
                        catch { count = gv.imageFileErrorList.finfoList?.Count ?? 0; }
                    }
                }
            }
            catch { /* keep UI safe */ }

            tbImageErrors.Text = count.ToString();
        }

        private void SaveImageErrorsList()
        {
            try
            {
                if (gv?.imageFileErrorList == null)
                {
                    tbMessage.Text = "No image errors to save.";
                    UpdateImageErrorsCount();
                    return;
                }

                int count = 0;
                try { count = gv.imageFileErrorList.getImageFileListLength(); }
                catch
                {
                    try { count = gv.imageFileErrorList.getImageCount(); }
                    catch { count = gv.imageFileErrorList.finfoList?.Count ?? 0; }
                }

                if (count == 0)
                {
                    tbMessage.Text = "No image errors to save.";
                    UpdateImageErrorsCount();
                    return;
                }

                string folder = Path.GetDirectoryName(gv.inifileFullPathName);
                if (string.IsNullOrWhiteSpace(folder))
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                Directory.CreateDirectory(folder);

                string fpath = Path.Combine(folder, "ImageErrors260318.xml");
                var serializer = new XmlSerializer(typeof(List<FileInfoItem>));
                using var writer = new StreamWriter(fpath);
                serializer.Serialize(writer, gv.imageFileErrorList.getFinfo());

                tbMessage.Text = $"Saved image errors: {fpath}";
            }
            catch (Exception ex)
            {
                tbMessage.Text = $"Save image errors failed: {ex.Message}";
            }
        }
        private string PromptUserForInitDirectory()
        {
            // Using a standard FolderBrowserDialog:
            using (var fbd = new FolderBrowserDialog())
            {
                // Provide some instructions in the dialog:
                fbd.Description = "Select a folder where the application can create its default Init File.";

                // Let the user create new folders if desired:
                fbd.ShowNewFolderButton = true;

                // Show the dialog and return the chosen path if the user clicks OK:
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    return fbd.SelectedPath;
                }
            }

            // If the user cancels or closes the dialog without selecting a folder, return null:
            return null;
        }


        /*
                using (var dest = new SqliteConnection($"Data Source={backupFile}"))
                {
                    source.Open();
                    dest.Open();
                    source.BackupDatabase(dest);
                }
        */
        // Open database
        string sourcePath = "";
        public bool DBconnect()
        {
            sourcePath = SqliteDb.DbPath;
            using (var conn = new SqliteConnection($"Data Source={sourcePath}"))
            {
                try
                {
                    conn.Open();
                    tbMessage.Text = $"DB Connection opened successfully!   >> {sourcePath}";
                }
                catch (SqliteException ex)
                {
                    tbMessage.Text = $"SQLite error: {ex.Message}";
                    return false;
                }
                catch (Exception ex)
                {
                    tbMessage.Text = $"General error: {ex.Message}";
                    return false;
                }
                conn.Close();
            }
            btDatabase.BackColor = Color.LightGreen;
            return true;
        }

        public string NormalizePathRemoveDrive(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "";

            // Normalize slashes first
            path = path.Replace('\\', '/').Trim();

            // UNC path? (\\Server\Share\File)
            // After slash-normalization → //Server/Share/File
            if (path.StartsWith("//"))
            {
                // Remove duplicate slashes, but preserve UNC root
                while (path.StartsWith("///"))
                    path = path.Substring(1);

                return path; // leave UNC as "//Server/share/file"
            }

            // Drive letter? (C:/..., D:/..., etc.)
            if (path.Length >= 2 && path[1] == ':')
            {
                // Remove "C:/" → remove first 3 characters
                if (path.Length > 3 && (path[2] == '/' || path[2] == '\\'))
                    path = path.Substring(3);
                else
                    path = path.Substring(2); // handles cases like "C:folder"
            }

            // Clean up:
            // remove leading slashes: "/dmc/123" → "dmc/123"
            path = path.TrimStart('/');

            // Remove accidental double slashes inside path
            while (path.Contains("//"))
                path = path.Replace("//", "/");

            return path;
        }




        //ManagementObject[] objs = new ManagementObject[10];





        private static Screen[] GetScreensSafe()
        {
            var screens = Screen.AllScreens;
            if (screens == null || screens.Length == 0)
            {
                var primary = Screen.PrimaryScreen;
                if (primary != null)
                    return new[] { primary };

                return Array.Empty<Screen>();
            }

            return screens;
        }

        public void initScreens()
        {
            screensInfo = new ScreensInfo(gv);

            var screens = GetScreensSafe();
            gv.screen = screens;
            gv.displayCount = screens.Length;

            btCompareLoad.Text = gv.displayCount.ToString();

            cbDisplay2.Enabled = gv.displayCount >= 2;
            cbDisplay3.Enabled = gv.displayCount >= 3;
            cbDisplay4.Enabled = gv.displayCount >= 4;

            if (gv.displayCount == 0)
            {
                gv.debug.w("No displays detected.");
                return;
            }

            var primaryScreen = screens.FirstOrDefault(s => s.Primary) ?? screens[0];
            var appScreenSnum = Array.IndexOf(screens, primaryScreen);
            if (appScreenSnum < 0)
                appScreenSnum = 0;

            gv.appScreenNum = appScreenSnum;

            this.Location = screens[appScreenSnum].Bounds.Location;

            // Ensure ListView is set up only once, but always refreshed.
            if (lvScreenInfo.Columns.Count == 0)
                CreateLV_ScreensInfo();

            lvScreenInfo.BeginUpdate();
            try
            {
                lvScreenInfo.Items.Clear();

                var virtualScreen = gv.virtualScreenSize;
                if (virtualScreen == Rectangle.Empty)
                    virtualScreen = SystemInformation.VirtualScreen;

                insertRowLV_ScreenInfo(
                    gv.displayCount,
                    virtualScreen.Width,
                    virtualScreen.Height,
                    virtualScreen.X,
                    virtualScreen.Y,
                    "virtual");

                for (int i = 0; i < screens.Length; i++)
                {
                    var s = screens[i];
                    insertRowLV_ScreenInfo(
                        i,
                        s.Bounds.Width,
                        s.Bounds.Height,
                        s.Bounds.X,
                        s.Bounds.Y,
                        s.DeviceName);
                }
            }
            finally
            {
                lvScreenInfo.EndUpdate();
            }
        }
        public int testNumberOfMonitors()
        {
            if (gv.screen.Length < 2)
            {
                gv.debug.w("ERROR 1 display null");
                gv.displayCount = 1;
                btCompareLoad.Text = "1";
                return 1;
            }
            if (gv.screen[1] == null)
            {
                gv.debug.w("ERROR 1 display null");
                tbFpath.Text = "ERROR 1 display 1 null";
                gv.displayCount = 1;
                btCompareLoad.Text = "1";
                return 1;
            }
            return 0;
        }
        public void initScreens2()
        {
            screensInfo = new ScreensInfo(gv);
            int countMonitors = gv.displayCount;
            //this.Location = gv.screen[0].Bounds.Location;
            //int snum = gv.screen.Length;
            btCompareLoad.Text = countMonitors.ToString();
            int xposition = -99999;
            int appScreenSnum = 0;
            int mainScreenNum = 0;

            if (countMonitors < 4)
                cbDisplay4.Enabled = false;
            if (countMonitors < 3)
                cbDisplay3.Enabled = false;
            if (countMonitors < 2)
                cbDisplay2.Enabled = false;

            testNumberOfMonitors();

            int mainScreenXposition = -9999;
            for (int idx = 0; idx < gv.displayCount; ++idx)
            {
                if (gv.screen[idx].Primary)//gv.screen[idx].Bounds.X == 0)
                {
                    mainScreenXposition = gv.screen[idx].Bounds.X;
                    mainScreenNum = idx;
                }
            }

            // gv.masterControlScreenNum = masterControlScreenNum;
            /*
             for (int idx = 0; idx < gv.displayCount; ++idx)
             {
                 if (gv.screen[idx].Bounds.X > mainScreenXposition)
                 {
                     if (xposition > 0) // already found one screen to the right of Main
                     {
                         if (gv.screen[idx].Bounds.X < xposition)
                         {
                             xposition = gv.screen[idx].Bounds.X;
                             appScreenSnum = idx;
                         }
                     }
                     else
                     {
                         xposition = gv.screen[idx].Bounds.X;
                         appScreenSnum = idx;
                     }

                 }
             }
             */
            gv.appScreenNum = appScreenSnum;

            System.Drawing.Point locMain = new System.Drawing.Point(gv.screen[appScreenSnum].Bounds.Location.X, gv.screen[appScreenSnum].Bounds.Location.Y);
            this.Location = locMain;

            int offset = gv.screen[appScreenSnum].WorkingArea.Width - this.Width;

            // this.CenterToScreen();
            //
            //this.WindowState = FormWindowState.Maximized;
            CreateLV_ScreensInfo();
            // pb1.Height = this.height - 20;

            insertRowLV_ScreenInfo(gv.displayCount, gv.virtualScreenSize.Width, gv.virtualScreenSize.Height, gv.virtualScreenSize.Location.X, gv.virtualScreenSize.Location.Y, "virtual");
            // insertRowLV_ScreenInfo(-gv.displayCount, gv.screenSize.Width, gv.screenSize.Height, 0, 0);
            //  insertRowLV_ScreenInfo(0, gv.screen[gv.masterControlScreenNum].Bounds.Width, gv.screen[gv.masterControlScreenNum].Bounds.Height, gv.screen[gv.masterControlScreenNum].Bounds.Location.X, gv.screen[gv.masterControlScreenNum].Bounds.Location.Y);

            for (int idx = 0; idx < gv.displayCount; ++idx)
            {
                if (gv.screen[idx] != null)
                    insertRowLV_ScreenInfo(idx, gv.screen[idx].Bounds.Width, gv.screen[idx].Bounds.Height, gv.screen[idx].Bounds.Location.X, gv.screen[idx].Bounds.Location.Y, gv.screen[idx].DeviceName);
                else
                {
                    //   bAllowScreen2 = false;
                    //   bScreen2 = false;
                    // cbDisplay2.Enabled = false;
                }
            }

        }


        public System.Drawing.Point getLocation()
        {
            return this.Location;
        }

        /*
         * public string fpath; //fullpath
        public string fname; // file name + ext
        public string dpath; //directory
        public long len;
        public string ext;
        public string type;  // (f)ile , (d)irectory 
        public int level;    // nested subfolder level
        public DateTime ts;
        public string stimestamp;*/

        private void updateLvFinfo(FileInfoItem finfo)
        {
            dgvFileInfo.Rows[DeletedRowNumber].Cells[1].Value = finfo.bDelete;   // row 9
        }
        public void updateDgv1(FileInfoItem finfo)
        {

        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        public static byte[] converterDemo(System.Drawing.Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }
        public bool CompareImagesOld()
        {
            var array1 = converterDemo(bmp);
            var array2 = converterDemo(bmpPrevious);

            bool isSame = array1.Length == array2.Length;

            if (isSame)
                for (int index = 0; index < array1.Length; index++)
                    if (array1[index] != array2[index])
                    {
                        isSame = false;
                        break;
                    }
            return isSame;
        }






        /*
        public void updateMRLhistory(ImageInfo iinfo)
        {
            tbPrevious4FileSize.Text = tbPrevious3FileSize.Text;
            tbPrevious3FileSize.Text = tbPrevious2FileSize.Text;
            tbPrevious2FileSize.Text = tbPreviousFileSize.Text;
            tbPreviousFileSize.Text = tbFileSize.Text;
            tbFileSize.Text = iinfo.len.ToString();

            fileSizePrevious = fileSize;
            fileSize = iinfo.len;

            if (fileSizePrevious == fileSize)
            {
                tbPreviousFileSize.BackColor = Color.LightGreen;
                tbFileSize.BackColor = Color.LightGreen;
                btDeleteAndResume.BackColor = Color.LightGreen;
                tbMatchRowNumber.Visible = true;
                tbMatchRowNumber.BackColor = Color.Yellow;
                // bool match = CompareImages();
                // if (match)
                //    tbMatchRowNumber.Text = "MATCH!";  //idxRow.ToString();
                // else
                if (cbFindDuplicates.Checked)
                {
                    tbMatchRowNumber.Text = "MATCH?";  //idxRow.ToString();
                    if (cbAutoDeleteExactMatch.Checked)
                    {
                        tbMatchPercent.Text = "--";

                        if (pbLastImage.Image == null)
                            unmatch = 100;
                        else
                            unmatch = CompareImages2((Bitmap)pb1.Image, (Bitmap)pbLastImage.Image); //pb1 pbLast
                        tbMatchPercent.Text = $"{unmatch:N2}";
                        if (unmatch < 20)
                        {
                            tbMatchPercent.Text = "Match";
                            if (unmatch < 5)
                            {
                                tbMatchPercent.Text = "EXACT";
                                iinfo.bDelete = true;
                                tbMatchPercent.BackColor = Color.LightBlue;
                                if (!gv.bSlideShow)
                                    gv.bSlideShow = true;
                            }
                            else
                            {
                                stopSlideShow("pb1 matches last (prior)");
                                DisplayAutoDeletePrompt();
                                btResume.BackColor = Color.Red;
                            }
                        }
                    }
                    else
                    {
                        stopSlideShow("pb1 matches last (prior)");
                        bAutoStartAllow = false;
                        //DisplayAutoDeletePrompt();
                        btResume.BackColor = Color.Red;
                    }
                }
            }
            else
            {
                tbPreviousFileSize.BackColor = Color.LightGray;
                tbFileSize.BackColor = Color.LightGray;
                btDeleteAndResume.BackColor = Color.LightGray;
                tbMatchRowNumber.Visible = false;
            double unmatch =110;
            }
        }*/
        long fileSize = 0;
        bool bAutoStartAllow = true;
        long fileSizePrevious = 0;
        string lastFolderPath = "none";




        // Add rows in the same order you used in ListView
        string[] labels = {
            "file name",
            "folder",
            "full path",
            "ext",
            "timestamp",
            "length",
            "width",
            "height",
            "rating",
            "bDelete",
            "bInvalid"
        };
        // PRESETUP 
        public void PreSetupDGV()
        {
            // Typically done once, e.g., in Form_Load or a method like initDgvFinfo().
            dgvFileInfo.Columns.Clear();
            dgvFileInfo.Rows.Clear();
            dgvFileInfo.AllowUserToAddRows = false;
            dgvFileInfo.AllowUserToDeleteRows = false;
            dgvFileInfo.RowHeadersVisible = false;
            dgvFileInfo.ReadOnly = true;

            // Add 2 columns: [Label, Value]
            dgvFileInfo.Columns.Add("colLabel", "Label");
            dgvFileInfo.Columns.Add("colValue", "Value");



            foreach (var lbl in labels)
            {
                // We start each Value cell with an empty string
                dgvFileInfo.Rows.Add(lbl, "");
            }
        }

        /// <summary>
        /// /
        /// 
        /// </summary>
        /// <returns></returns>

        public bool initDgvFinfo()
        {
            // Example array of property names. 
            // (Same as your old 'name' array in initLVFinfo)
            string[] propertyNames = new string[]
            {
        "file name",
        "folder",
        "full path",
        "ext",
        "timestamp",
        "length",
        "width",
        "height",
        "rating",
        "bDelete",
        "bInvalid"
            };

            // 1. Clear any existing columns or rows in the DataGridView
            dgvFileInfo.Columns.Clear();
            dgvFileInfo.Rows.Clear();

            // 2. Configure DataGridView (optional customizations as needed)
            dgvFileInfo.AllowUserToAddRows = false;
            dgvFileInfo.AllowUserToDeleteRows = false;
            dgvFileInfo.ReadOnly = true;
            dgvFileInfo.RowHeadersVisible = false;

            // 3. Add two columns: "Property" and "Value"
            dgvFileInfo.Columns.Add("colProperty", "Property");
            dgvFileInfo.Columns.Add("colValue", "Value");

            // 4. Add one row per property
            foreach (var propName in propertyNames)
            {
                // The "Value" column can be initialized empty (or with " ").
                dgvFileInfo.Rows.Add(propName, "");
            }

            return true;
        }
        public bool initDataGridViewFinfo()
        {
            // 1) Clear any existing rows/columns from the DataGridView
            dgvFileInfo.Columns.Clear();
            dgvFileInfo.Rows.Clear();

            // 2) Optionally set up some properties
            dgvFileInfo.AllowUserToAddRows = false;
            dgvFileInfo.AllowUserToDeleteRows = false;
            dgvFileInfo.ReadOnly = true;  // if you want it read-only
            dgvFileInfo.RowHeadersVisible = false; // no row headers on the left

            // 3) Add two columns: 
            //    - colLabel  (the property name/label) 
            //    - colValue  (the actual value to display)
            dgvFileInfo.Columns.Add("colLabel", "Label");
            dgvFileInfo.Columns.Add("colValue", "Value");

            // 4) Prepare the list of labels (like your ListView items)
            string[] labels = new string[]
            {
        "full path",
        "file name",
        "length",
        "timestamp",
        "width",
        "height",
        "rating",
        "bDelete",
        "bInvalid",
        "folder",
        "ext"
            };

            // 5) For each label, add a new row with an empty Value cell
            foreach (var label in labels)
            {
                // Column 0 = label, Column 1 = (initially blank)
                dgvFileInfo.Rows.Add(label, "");
            }

            return true;
        }
        public int DeletedRowNumber = 0;
        public int DeletedColumnNumber = 0;
        public void updateDgvFinfo(FileInfoItem iinfo)
        {
            bAutoStartAllow = true;
            int idx = 0;
            tbImageNumber.Text = gv.nextIdx.ToString("N0"); //<<<<<<<<<<<<<<<<<<<<<<<<
                                                            // 1) Mirror your existing logic
            setTitle(iinfo, iinfo.dpath);
            tbDescription.Text = iinfo.comment;

            // 2) Assign property values to the second column (index 1) in each row
            try
            {
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.fname;        // row 0
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.dpath;        // row 1
                lastFolderPath = iinfo.dpath;
                SetFolder(iinfo.dpath);

                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.fpath;        // row 2
                fpathCurrentImage = iinfo.fpath;

                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.ext;          // row 3
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.stimestamp;   // row 4
                dgvFileInfo.Rows[idx++].Cells[1].Value
                    = string.Format("{0:##,###,##0}", iinfo.len);         // row 5

                // 3) Handle width/height from pb1.Image
                if (pb1.Image != null)
                {
                    dgvFileInfo.Rows[idx++].Cells[1].Value = pb1.Image.Width.ToString();  // row 6
                    dgvFileInfo.Rows[idx++].Cells[1].Value = pb1.Image.Height.ToString(); // row 7
                    iinfo.width = pb1.Image.Width;
                    iinfo.height = pb1.Image.Height;
                }
                else
                {
                    dgvFileInfo.Rows[idx++].Cells[1].Value = "0";  // width  (row 6)
                    dgvFileInfo.Rows[idx++].Cells[1].Value = "0";  // height (row 7)
                }

                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.rating;    // row 8
                DeletedRowNumber = idx;
                DeletedColumnNumber = 1;
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.bDelete;   // row 9

                // ENSURE bInvalid IS DISPLAYED - row 10
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.bInvalid;  // row 10

                // Set background color based on bInvalid status
                if (iinfo.bInvalid)
                {
                    dgvFileInfo.BackColor = Color.LightPink;
                    dgvFileInfo.Rows[10].Cells[1].Style.BackColor = Color.Red; // Highlight the bInvalid row
                    dgvFileInfo.Rows[10].Cells[1].Style.ForeColor = Color.White;
                }
                else
                {
                    dgvFileInfo.BackColor = Color.LightGray;
                    dgvFileInfo.Rows[10].Cells[1].Style.BackColor = Color.White;
                    dgvFileInfo.Rows[10].Cells[1].Style.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                // If your row indexing doesn't match, you might get an IndexOutOfRangeException
                gv.debug.w($"DataGridView row mismatch: {ex.Message}");
                return;
            }

            if (cbFindExceptions.Checked)
            {
                if (iinfo.bInvalid || iinfo.bDelete)
                {
                    stopSlideShow("Exception found - Invalid or Deleted file");
                }
            }

            // 4) bDelete logic
            if (iinfo.bDelete)
                btDelete.BackColor = Color.OrangeRed;
            else
                btDelete.BackColor = Color.LightSalmon;

            // Additional UI updates...
            if (gv.nextIdx >= gv.slideCount1)
            {
                gv.nextIdx = gv.slideCount1 - 1;
                stopSlideShow();
            }

            tbSlideNumber.Text = string.Format("{0:###,###,##0}", gv.nextIdx);
            tbImageNumber.Text = tbSlideNumber.Text;
            tbMaxSlideNumber.Text = string.Format("{0:###,###,###}", gv.slideCount1 - 1);

            tbFpath.Text = iinfo.fpath;

            try
            {
                dgvFileInfo.Refresh();   // Force redraw if needed
            }
            catch (Exception e)
            {
                gv.debug.w("update error dgvFinfo: " + e.Message);
            }

            // Update match display and other logic...
            if (cbFindMatch.Checked && pb1.Image != null)
            {
                CompareImageWithTraverser2((Bitmap)pb1.Image);
                if (unmatchPercent < 20)
                    stopSlideShow("pb1 match T2?");
                if (pbNext.Image != null)
                {
                    CompareImageWithTraverser2((Bitmap)pbNext.Image);
                    if (unmatchPercent < 20)
                        stopSlideShow("pbNext match T2?");
                }
            }

            UpdateMatchDisplay();
        }


        // Add inside the Main class (e.g. near other public helper methods)

        // Purpose: Ensure BrowserForm is open/active, then load a local HTML/HTM/MHTML file.
        public bool ShowBrowserAndDisplayLocalFile(string filePath)
        {
            return false;
        }


        private volatile bool _cancelCopyOperation = false;
        bool bCopyAll2 = false;
        string tbTargetBatchCopy = "1";

        public void CopyAllExec()
        {
            idxCopyHighestValue = 0;  // Track highest index reached
            gv.maxCountCopy = (int)numCopyBatchNumber.Value;
            if (string.IsNullOrEmpty(numTargetFolderCopyAll.Value.ToString()))
                tbTargetBatchCopy = "V";

            char targetDir = char.Parse(tbTargetBatchCopy.Substring(0, 1));
            idxCopyHighestValue = 0;

            // Reset the cancel flag
            _cancelCopyOperation = false;

            while (cbCopyAll.Checked && !_cancelCopyOperation)
            {
                // Process Windows messages to handle keyboard input
                Application.DoEvents();

                bool rcContinue = CopyAll2(targetDir);
                if (!rcContinue)
                    return; // Exit on failure

                // Small delay to prevent excessive CPU usage and allow UI updates
                System.Threading.Thread.Sleep(50);
            }

            // Clean up when exiting
            if (_cancelCopyOperation)
            {
                cbCopyAll.Checked = false;
                tbMessage.Text = "Copy operation cancelled by user";
                tbMessage.BackColor = Color.Orange;
                stopSlideShow("Copy operation cancelled");
            }
        }

        public void CopyAllExecwas22()
        {
            idxCopyHighestValue = 0;  // Track highest index reached
            gv.maxCountCopy = (int)numCopyBatchNumber.Value;
            if (string.IsNullOrEmpty(numTargetFolderCopyAll.Value.ToString()))
                tbTargetBatchCopy = "V";

            char targetDir = char.Parse(tbTargetBatchCopy.Substring(0, 1));
            idxCopyHighestValue = 0;
            while (cbCopyAll.Checked)
            {
                bool rcContinue = CopyAll2(targetDir);
                if (!rcContinue)
                    return; // Exit on failure
            }
        }
        // Keep track of last index we copied for preview management
        int idxCopyHighestValue = 0;
        bool firstMatch = true;
        int copyallLimitForTest = 1000;
        public bool CopyAll2(char targetd)  //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< COPY ALL 2
        {
            bool shouldSkip = false;

            // Step 1: Preview and duplicate check
            if (cbSkipDuplicates.Checked)
            {
                if (gv.nextIdx + 1 < gv.slideCount1)
                {
                    previewNextImage(1);
                }
                /*
                if (gv.nextIdx > copyallLimitForTest)
                {
                    cbCopyAll.Checked = false;
                    return false;
                }
                */
                idxCopyHighestValue = gv.nextIdx;
                status.UpdateStatusHighest(idxCopyHighestValue);

                // **NEW: Check against recent images cache instead of just next image**
                if (pb1.Image != null && finfo1.len > 0)
                {
                    bool foundDuplicate = false;
                    int matchedIndex = -1;
                    double bestMatch = 0;

                    // Check against all cached images with same file size
                    foreach (var cached in _recentImagesCache.Where(c => c.FileSize == finfo1.len))
                    {
                        try
                        {
                            double match = CompareImages2((Bitmap)pb1.Image, cached.Image);

                            if (match > 90)
                            {
                                foundDuplicate = true;
                                matchedIndex = cached.Index;
                                bestMatch = match;
                                break; // Found a duplicate
                            }
                        }
                        catch (Exception ex)
                        {
                            gv.debug.w($"Cache comparison error: {ex.Message}");
                        }
                    }

                    if (foundDuplicate)
                    {
                        shouldSkip = true;
                        tbMatchPercent.Text = $"{bestMatch:N2}% (vs #{matchedIndex})";
                        tbMessage.Text = $"Duplicate of image #{matchedIndex}: {finfo1.fname}";
                        tbMessage.BackColor = Color.Yellow;

                        ++countMarkedForDeletion;
                        tbMarkedForDeletion.Text = countMarkedForDeletion.ToString();
                        if (firstMatch && cbStopOnMatch.Checked)
                        {
                            firstMatch = false;
                            MessageBox.Show($"First duplicate found at image #{gv.nextIdx} matching #{gv.nextIdx + 1}.\n" +
                                $"Subsequent duplicates will be skipped automatically.", "Duplicate Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (cbStopOnMatch.Checked)
                                stopSlideShow("Duplicate found");

                        }

                        if (cbAutoDeleteExactMatch.Checked)
                        {
                            finfo1.bDelete = true;
                            gv.imageFileList1.updateDelete(true, gv.nextIdx);
                        }
                    }
                    else
                    {
                        // No duplicate found - add current image to cache
                        AddToRecentCache((Bitmap)pb1.Image, finfo1.len, gv.nextIdx);
                    }
                }
            }

            // Step 2: Copy file (rest of existing code)
            if (!cbScanOnly.Checked)
            {
                if (!shouldSkip)
                {
                    bool rc = copyOrMoveImage(targetd);
                    if (!rc)
                    {
                        // ... existing error handling ...
                        return false;
                    }

                    tbMessage.BackColor = Color.LightGreen;
                    tbMessage.Text = $"Copied: {finfo1.fname}";
                }
            }
            // Step 3: Advance to next image
            int nextIdx = computeNextSlideNumber(gv.nextIdx, 1);
            showNextSlideImageFileList1(nextIdx, 1);

            idxCopyHighestValue = Math.Max(idxCopyHighestValue, gv.nextIdx);
            status.UpdateStatusHighest(idxCopyHighestValue);

            // ... rest of existing code ...
            return true;
        }
        int CountCopies = 0;

        public string validateDirectory(string sdir)
        {
            string stext = sdir;

            if (sdir.Contains("\\"))
            {
                stext = sdir.Replace("\\", "/");
            }
            int ilen = stext.Length - 1;
            string slast = stext.Substring(ilen, 1);

            if (!slast.Equals("/"))
            {
                stext += "/";
            }
            return stext;
        }
        public bool setCopyMode(bool mode1)
        {
            //cbCatalogImages.Checked = mode1;
            if (mode1)
            {
                string targetdir = validateDirectory(tbTargetFolder.Text);
                tbTargetFolder.Text = targetdir;
            }
            return rbCopy.Checked;
        }
        private void CreateLV_ScreensInfo()
        {
            // Created a new ListView control at top:
            // ListView listView1 = new ListView();

            //  listView1.Bounds = new Rectangle(new Point(0, 0), new Size(1200, 400));

            // Set the view to show details.
            lvScreenInfo.View = View.Details;
            lvScreenInfo.Scrollable = true;
            // Allow the user to edit item text.
            lvScreenInfo.LabelEdit = false;
            // Allow the user to rearrange columns.
            lvScreenInfo.AllowColumnReorder = true;
            // Display check boxes.
            lvScreenInfo.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            lvScreenInfo.FullRowSelect = true;
            // Display grid lines.
            lvScreenInfo.GridLines = true;
            // Sort the items in the list in ascending order.
            lvScreenInfo.Sorting = System.Windows.Forms.SortOrder.None;

            System.Drawing.Rectangle wndBounds = this.Bounds;

            listViewLoc = lvScreenInfo.Location;
            /* wndBounds.Height -= 250;
             wndBounds.Width -= 50;
             wndBounds.X = listViewLoc.X;
             wndBounds.Y = listViewLoc.Y;
             this.listView1.Bounds = wndBounds;
             */
            Boolean test = true;
            // Create columns for the items and subitems.
            lvScreenInfo.Columns.Add("screen", 80, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("width", 60, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("height", 60, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("x", 60, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("y", 60, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("orientation", 100, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("name", 150, System.Windows.Forms.HorizontalAlignment.Center);



            //listviewX.ListView.Columns[ColumnToSort].Tag = "Text";
            //ColumnHeader header1 = listView1.InsertColumn(0, "Name", 10 * listView1.Font.SizeInPoints, HorizontalAlignment.Center);
            //Add the items to the ListView.
            // listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

            // Create two ImageList objects.
            // ImageList imageListSmall = new ImageList();
            //ImageList imageListLarge = new ImageList();

            // Initialize the ImageList objects with bitmaps.
            // imageListSmall.Images.Add(Bitmap.FromFile("C:\\01.jpg"));
            // imageListSmall.Images.Add(Bitmap.FromFile("C:\\01.bmp"));
            // imageListLarge.Images.Add(Bitmap.FromFile("C:\\01.jpg"));
            // imageListLarge.Images.Add(Bitmap.FromFile("C:\\01.bmp"));

            //Assign the ImageList objects to the ListView.
            //    listView1.LargeImageList = imageListLarge;
            //    listView1.SmallImageList = imageListSmall;

            // Add the ListView to the control collection.
            // this.Controls.Add(listView1);

            lvScreenInfo.Columns[0].Tag = "Integer";
            lvScreenInfo.Columns[1].Tag = "Integer";
            lvScreenInfo.Columns[2].Tag = "Integer";
            lvScreenInfo.Columns[3].Tag = "Integer";
            lvScreenInfo.Columns[4].Tag = "Integer";
        }

        private void CreateLV_ScreensInfoWAS()
        {
            // Created a new ListView control at top:
            // ListView listView1 = new ListView();

            //  listView1.Bounds = new Rectangle(new Point(0, 0), new Size(1200, 400));

            // Set the view to show details.
            lvScreenInfo.View = View.Details;
            lvScreenInfo.Scrollable = true;
            // Allow the user to edit item text.
            lvScreenInfo.LabelEdit = false;
            // Allow the user to rearrange columns.
            lvScreenInfo.AllowColumnReorder = true;
            // Display check boxes.
            lvScreenInfo.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            lvScreenInfo.FullRowSelect = true;
            // Display grid lines.
            lvScreenInfo.GridLines = true;
            // Sort the items in the list in ascending order.
            lvScreenInfo.Sorting = System.Windows.Forms.SortOrder.None;

            System.Drawing.Rectangle wndBounds = this.Bounds;

            listViewLoc = lvScreenInfo.Location;
            /* wndBounds.Height -= 250;
             wndBounds.Width -= 50;
             wndBounds.X = listViewLoc.X;
             wndBounds.Y = listViewLoc.Y;
             this.listView1.Bounds = wndBounds;
             */
            Boolean test = true;
            // Create columns for the items and subitems.
            lvScreenInfo.Columns.Add("screen", 80, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("width", 60, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("height", 60, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("x", 60, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("y", 60, System.Windows.Forms.HorizontalAlignment.Center);
            lvScreenInfo.Columns.Add("orientation", 150, System.Windows.Forms.HorizontalAlignment.Center);



            //listviewX.ListView.Columns[ColumnToSort].Tag = "Text";
            //ColumnHeader header1 = listView1.InsertColumn(0, "Name", 10 * listView1.Font.SizeInPoints, HorizontalAlignment.Center);
            //Add the items to the ListView.
            // listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

            // Create two ImageList objects.
            // ImageList imageListSmall = new ImageList();
            //ImageList imageListLarge = new ImageList();

            // Initialize the ImageList objects with bitmaps.
            // imageListSmall.Images.Add(Bitmap.FromFile("C:\\01.jpg"));
            // imageListSmall.Images.Add(Bitmap.FromFile("C:\\01.bmp"));
            // imageListLarge.Images.Add(Bitmap.FromFile("C:\\01.jpg"));
            // imageListLarge.Images.Add(Bitmap.FromFile("C:\\01.bmp"));

            //Assign the ImageList objects to the ListView.
            //    listView1.LargeImageList = imageListLarge;
            //    listView1.SmallImageList = imageListSmall;

            // Add the ListView to the control collection.
            // this.Controls.Add(listView1);

            lvScreenInfo.Columns[0].Tag = "Integer";
            lvScreenInfo.Columns[1].Tag = "Integer";
            lvScreenInfo.Columns[2].Tag = "Integer";
            lvScreenInfo.Columns[3].Tag = "Integer";
            lvScreenInfo.Columns[4].Tag = "Integer";
        }
        int rowCount = 0;

        public Boolean insertRowLV_ScreenInfo(int iScreen, int iWidth, int iHeight, int x, int y, string sname)
        {
            //-- NEW ITEM
            string screennumber = iScreen.ToString();
            if (x == 0)
                screennumber = "main";
            else if (iScreen == gv.displayCount)
                screennumber = "virtual";
            else
                screennumber = (iScreen + 1).ToString();
            string iwidth = iWidth.ToString();
            string iheight = iHeight.ToString();

            string xxx = x.ToString();
            string yyy = y.ToString();
            string imode = "";
            Screen tmp;
            if (iScreen >= 0 && iScreen < gv.displayCount)
            {
                tmp = Screen.AllScreens[iScreen];
                if (screensInfo.landscapeMode(tmp))
                    imode = "Landscape";
                else
                    imode = "Portrait";
            }

            ListViewItem item1 = new ListViewItem(screennumber, 0);
            // Place a check mark next to the item.
            if (iScreen == gv.appScreenNum)
                item1.Checked = true;
            //-- SUB-ITEMS
            item1.SubItems.Add(iwidth);
            item1.SubItems.Add(iheight);
            item1.SubItems.Add(xxx);
            item1.SubItems.Add(yyy);
            item1.SubItems.Add(imode);
            item1.SubItems.Add(sname);
            //counter

            ++rowCount;
            lvScreenInfo.Items.Add(item1);
            return true;
        }
        // C#
        // Replace any use of System.Windows.Forms.Design.FolderNameEditor or FileNameEditor with:
        private static string PickFileAtRuntime(IWin32Window owner = null, string filter = "All files (*.*)|*.*")
        {
            using var ofd = new OpenFileDialog { Filter = filter, CheckFileExists = true, Multiselect = false };
            return ofd.ShowDialog(owner) == DialogResult.OK ? ofd.FileName : null;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // If the user selects the Resize check box, 
            // change the PictureBox's SizeMode property to "Zoom", resize to dimensions,
            // otherwise set it to "Normal", Actual Size.
            resizeImage();
        }

        public Bitmap ResizeBitmapFromFile(string filepath, int width, int height)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(filepath);
            Bitmap result = null;
            try
            {
                result = new Bitmap(width, height);
            }
            catch (Exception)
            {

                //throw;
            }

            using (Graphics g = Graphics.FromImage(result))
            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                // Draw the originam image in a new size.
                g.DrawImage(originalImage, 0, 0, width, height);

                // Fill a part of the result image with a specific color.
                System.Drawing.Rectangle toFill = new System.Drawing.Rectangle(0, 0, width / 2, height / 2);
                g.FillRectangle(brush, toFill);
            }

            return result;
        }
        public Bitmap ResizeBitmap(System.Drawing.Image originalImage, int width, int height)
        {
            //Image originalImage = Image.FromFile(filepath);

            Bitmap result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                // Draw the originam image in a new size.
                g.DrawImage(originalImage, 0, 0, width, height);

                // Fill a part of the result image with a specific color.
                // Rectangle toFill = new Rectangle(0, 0, width / 2, height / 2);
                // g.FillRectangle(brush, toFill);
            }

            return result;
        }
        private void resizeImage()
        {
            if (this.rbResize.Checked)
            {
                pb1.SizeMode = PictureBoxSizeMode.Zoom; //keeps aspect ratio

            }
            else if (this.rbCenter.Checked)
            {
                pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else if (this.rbActual.Checked)
            {
                pb1.SizeMode = PictureBoxSizeMode.Normal;
            }
            // pb1.SizeMode = PictureBoxSizeMode.AutoSize; ---- not useful here .. would resize the PB to the miag
            // pb1.SizeMode = PictureBoxSizeMode.StretchImage; // --not useful -- streachs image to fill the PB does not maintain aspect ratio

            pb1.BorderStyle = BorderStyle.None; // BorderStyle.Fixed3D;

            setDisplay1Mode(pb1.SizeMode);

            pb1.Refresh();
        }

        public void SetFileType(string ftype)
        {
            gv.FILE_TYPE = ftype;
            tbMessage.Text = gv.FILE_TYPE;
            tbVideoType.Text = gv.FILE_TYPE;
            btDisplayImages.BackColor = Color.LightGreen;
        }



        //not used
        private void MoveToStart()
        {
            SoundLevel(gv.soundVolume);
            // Point startingPoint = panel1.Location;
            // startingPoint.Offset(10, 10);
            // Cursor.Position = PointToScreen(startingPoint);
        }
        public void directoryPath(string path)
        {
            if (cbAutoAdvance.Checked)
                this.Text = "copy mode -- " + path;
            else
                this.Text = path;
        }

        public void ClearAllPictureBoxesInUsewas()
        {
            pbNext.Image?.Dispose();
            // pb1.Image = null;
            pb1ThumbNail.Image?.Dispose();
            pbLastImage.Image?.Dispose();
            pbSecondLast.Image?.Dispose();
            pbThirdLast.Image?.Dispose();
            pbFourthLast.Image?.Dispose();
            pbNext.Image = null;
            pb1ThumbNail.Image = null;
            pbLastImage.Image = null;
            pbSecondLast.Image = null;
            pbThirdLast.Image = null;
            pbFourthLast.Image = null;
        }

        public void ResetPictureBoxes()
        {
            pbNext.Image = null;
            pb1.Image = null;
            pb1ThumbNail.Image = null;
            pbLastImage.Image = null;
            pbSecondLast.Image = null;
            pbThirdLast.Image = null;
            pbFourthLast.Image = null;
            tbFpath.Text = "";
            //  lvFinfo.Items.Clear();
            mtbCount.Text = "00";
        }
        List<FolderHistory> folderHistoryList1;
        List<FolderHistory> folderHistoryList2;
        private void SortHistory()
        {
            folderHistoryList1 = gv.folderHistoryList.OrderBy(file => file.folderPath).ToList();
            folderHistoryList2 = folderHistoryList1.OrderBy(file => file.folderPath).ToList();
            // loadSourceCmboList();
            gv.folderHistoryList = folderHistoryList2.Distinct().ToList();
        }
        //TraverserDialog gv.dialogTraverser;
        private void buttonTraverse_Click(object sender, EventArgs e)
        {
            btDisplayImages.BackColor = Color.LightGray;
            SortHistory();
            cbShowDisplayNames.Checked = false;
            lvScreenInfo.SendToBack();
            ResetPictureBoxes();
            // gv.slideCount = 0;
            gv.nextIdx = 0;
            //this.Refresh();
            if (gv.dialogTraverser1 == null || gv.dialogTraverser1.IsDisposed)
            {
                gv.dialogTraverser1 = new DialogTraverser(gv, this, 0, ff);
                gv.dialogTraverser1.Visible = true;
            }
            else
            {
                gv.dialogTraverser1.WindowState = FormWindowState.Normal;
                gv.dialogTraverser1.Activate();
            }
            //gv.setCursorHourGlass();
            loadedPreviewListNumber = -1;
            //gv.imageFileList
            gv.dialogTraverser1.SettargetDirToThis(tbTargetFolder.Text);
        }

        //this form contains System.Windows.Forms.PictureBox pb;
        // pb displays image

        ColorDialog colorDialog1;

        private void backgroundButton_Click(object sender, EventArgs e)
        {
            // Show the color dialog box. If the user clicks OK, change the
            // PictureBox control's background to the color the user chose.
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                pb1.BackColor = colorDialog1.Color;

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pb_Click(object sender, EventArgs e)
        {
            this.resizeImage();
        }

        OpenFileDialog openFileDialog1;

        private void InitializeOpenFileDialog()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;// .SupportMultiDottedExtensions = true; //.Multiselect = true;
            this.openFileDialog1.Title = "My Image Browser";


            // openFileDialog1.InitialDirectory = Default_Path;

        }

        // This method handles the FileOK event.  It opens each file 
        // selected and loads the image from a stream into pictureBox1.
        /*
         * // Make sure that you have added the System.IO namespace.
            using System.IO;

        // Specify a valid picture file path on your computer.
            FileStream fs;
            fs = new FileStream("C:\\WINNT\\Web\\Wallpaper\\Fly Away.jpg", FileMode.Open, FileAccess.Read);
            pictureBox1.Image = System.Drawing.Image.FromStream(fs);
            fs.Close();
         * 
        public void openFileDialogStreamLoad(string files)
        {

            //this.tb1.Text = "test";

            // Open each file and display the image in pictureBox1.
            // Call Application.DoEvents to force a repaint after each
            // file is read.        
            foreach (string file in files)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
                System.IO.FileStream fileStream = fileInfo.OpenRead();
                if (this.showImage)
                    pb1.Image = System.Drawing.Image.FromStream(fileStream);
                Application.DoEvents();
                fileStream.Close();

                // Call Sleep so the picture is briefly displayed, 
                //which will create a slide-show effect.
            }
            //pictureBox1.Image = null;
        }*/

        public void sleepSeconds()
        {
            System.Threading.Thread.Sleep(2000);
        }

        public void displayThisImage(System.Drawing.Image image)
        {
            //  if (this.showImage)
            this.pb1.Image = image;
            this.Refresh();

        }
        public void ClearPB1Image()
        {
            if (this.showImage)
                this.pb1.Image = null;
            //this.pb.InitialImage

        }
        public void ReloadImage()
        {
            pbSecondLast.Image = pb1.Image;
            System.Drawing.Image oldImage = pbNext.Image;
            pbNext.Image = null;
            //reset?
            pbNext.Image = pb1.Image;
            if (oldImage != null)
                oldImage.Dispose();
            pbNext.BringToFront();
        }

        // <summary>
        /// Skips the next image in the current SlideShow or CopyAll operation
        /// </summary>
        public int SkipNextImage()
        {
            // Stop any active slideshow temporarily
            bool wasSlideShowActive = gv.bSlideShow;
            bool wasCopyAllActive = cbCopyAll?.Checked ?? false;

            if (wasSlideShowActive)
            {
                timer.Stop();
            }

            // Log the skip action
            gv.debug.w($"Skipping image at index {gv.nextIdx}: {finfo1?.fpath ?? "unknown"}");

            // Move to next image
            if (gv.nextIdx < gv.slideCount1 - 1) //dmccheckthis
            {
                gv.nextIdx++;

                // Display the next image
                showNextSlideImageFileList1(gv.nextIdx, 1);

                // Resume slideshow if it was active
                if (wasSlideShowActive)
                {
                    timer.Start();
                }

                // Continue CopyAll if it was active
                if (wasCopyAllActive && cbCopyAll != null)
                {
                    // The CopyAll loop will pick up from the new gv.nextIdx
                    // No need to restart it, just let it continue
                }

                tbMessage.Text = $"Skipped to image {gv.nextIdx} of {gv.slideCount1 - 1}";
                tbMessage.BackColor = Color.Yellow;
            }
            else
            {
                // We're at the end of the list
                tbMessage.Text = "Cannot skip: at end of image list";
                tbMessage.BackColor = Color.OrangeRed;
                stopSlideShow();
            }
            return gv.nextIdx;
        }

        /// <summary>
        /// Skips the current image and marks it for later review (optional rating)
        /// </summary>
        /// <param name="markAsSkipped">If true, marks the image with a special rating</param>
        public void SkipAndMarkImage(bool markAsSkipped = false)
        {
            if (markAsSkipped && finfo1 != null)
            {
                // Mark with 's' for skipped
                finfo1.rating = 's';
                gv.imageFileList1.updateRating('s', gv.nextIdx);
                updateDgvFinfo(finfo1);
            }

            SkipNextImage();
        }

        /// <summary>
        /// Skips multiple images forward
        /// </summary>
        /// <param name="count">Number of images to skip</param>
        public void SkipMultipleImages(int count)
        {
            if (count <= 0) return;

            bool wasSlideShowActive = gv.bSlideShow;
            if (wasSlideShowActive)
            {
                timer.Stop();
            }

            int targetIdx = gv.nextIdx + count;
            if (targetIdx >= gv.slideCount1)
            {
                targetIdx = gv.slideCount1 - 1;
            }

            gv.debug.w($"Skipping {count} images from {gv.nextIdx} to {targetIdx}");

            showNextSlideImageFileList1(targetIdx, 1);

            if (wasSlideShowActive)
            {
                timer.Start();
            }

            tbMessage.Text = $"Skipped {count} images to {targetIdx}";
            tbMessage.BackColor = Color.LightBlue;
        }
        public void displayMRLinListBox()
        {
            FileInfoItem fi;
            mylistBox.Items.Clear();

            for (int idx = 0; idx < mostRecentImages.getImageCount(); ++idx)
            {
                fi = mostRecentImages.getIndexed(idx);
                mylistBox.Items.Add($"{fi.level} {fi.ndx:0000} {fi.len} {fi.dpath} ");
            }

        }
        int invalidImageCount = 0;
        string lastImage;
        long p1Size = 0;
        Bitmap mainImage = null;
        Bitmap bmp;
        Bitmap bmpPrevious;
        bool bFindDupByFileSize = false;
        public bool LoadImageFromFinfo(string fpath, int pbNumber, FileInfoItem fi = null)
        {
            bool loadedOK = true;
            lastImage = fpath;
            switch (pbNumber)
            {
                case 1: //current pb1
                    if (cbDisplayOnThisDisplay.Checked)
                    {
                        mainImage = LoadImageBmap(fpath);// LoadImageBmap

                        pb1.Image = mainImage;
                        //  bmp = new Bitmap(fpath);
                        //  bmpPrevious = bmp;
                        //   pb1.Image = bmp;
                        //MRL will save if it is next in order
                        tbFileSize.Text = $"{fi.len}";
                        tbP1Size.Text = $"{fi.len}";
                        p1Size = fi.len;
                        tbImageNumber.Text = $"{fi.ndx:N0}";
                        tbImageNumber.Refresh();
                    }
                    mostRecentImages.addItemMRL(fi, gv.nextIdx);
                    displayPreviousImagesInfo();
                    displayMRLinListBox();
                    if (bFindDupByFileSize)
                        CompareByFileSize();
                    //2024 Compare3Images();
                    break;
                case 0: //next (preview) <<<<<<<<<<<<<<<<<<<<<<<<<<<<<< PREVIEW IMAGE
                    System.Drawing.Image old = pbNext.Image;

                    int nextIdx = fi?.ndx ?? idxOfNextImage;
                    FileInfoItem nextFi = fi;
                    string nextPath = fpath;

                    while (true)
                    {
                        if (string.IsNullOrWhiteSpace(nextPath))
                        {
                            tbMessage.Text = "Preview path is empty. Skipping.";
                            tbMessage.BackColor = Color.OrangeRed;
                            return false;
                        }

                        try
                        {
                            bmp = new Bitmap(nextPath);
                            break; // success
                        }
                        catch (Exception ex)
                        {
                            ++invalidImageCount;
                            tbInvalidCount.Text = $"Invalid Images: {invalidImageCount}";

                            if (gv.imageFileErrorList == null)
                                gv.imageFileErrorList = new ImageFileList();

                            if (nextFi == null)
                                nextFi = new FileInfoItem(nextPath);

                            nextFi.bInvalid = true;
                            if (string.IsNullOrWhiteSpace(nextFi.comment))
                                nextFi.comment = $"Invalid image: {ex.Message}";

                            gv.imageFileErrorList.addItem(nextFi);
                            UpdateImageErrorsCount();

                            // Advance to next image in list
                            nextIdx++;
                            if (nextIdx >= gv.slideCount1)
                            {
                                tbMessage.Text = "Reached end of list while searching for a valid preview image.";
                                tbMessage.BackColor = Color.OrangeRed;
                                return false;
                            }

                            nextFi = gv.imageFileList1.getIndexed(nextIdx);
                            nextFi.ndx = nextIdx;
                            nextPath = nextFi.fpath;

                            // keep state in sync for preview
                            idxOfNextImage = nextIdx;
                            fi = nextFi;
                            fpath = nextPath;
                        }
                    }

                    idxOfNextImage = nextIdx;

                    if (idxOfNextImage != fi.ndx)
                    {
                        System.Windows.Forms.MessageBox.Show($"idx {idxOfNextImage} != {fi.ndx}", "preview image error");
                    }

                    pbNext.Image = bmp;
                    tbPbNextSize.Text = $"{fi.len}";
                    nextFileSize = fi.len;                //2024
                    if (old != null)
                        old.Dispose();
                    break;
                case 2: //comparison pb2
                    bmp = new Bitmap(fpath);
                    pb2Image.Image = bmp;
                    break;
                case 9: //comparison 
                    bmp = new Bitmap(fpath);
                    break;
                default:
                    tbMessage.Text = "ERROR LOAD IMAGE";
                    loadedOK = false;
                    break;
            }
            return loadedOK;
        }

        public long nextFileSize = 0;
        public void activateImageButtons()
        {
            btDisplayImages.Enabled = true;
            btSlideShow.Enabled = true;
            btFileCreate.Enabled = true;
            btPreviewImages.Enabled = true;
        }
        private void btnDisplay1_click(object sender, EventArgs e)
        {
            Display1 d1 = new Display1(gv);
            //d1.setMonitorDisplay
            d1.Visible = true;
            System.Drawing.Image image1 = pb1.Image;
            d1.displayThisImage(image1);

        }


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }


        private void catalogImages()
        {
            gv.setCursorDefault();

            // System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chord.wav");
            gv.nextIdx = 0;

            startSlideShow(0, false, 1); // NO TIMER .. start at idx = 0 

        }
        // bool bMasterStopSlideShow = false;
        public bool setSlideShowOn(bool bOn)
        {
            if (cbMasterOFF.Checked)
                bOn = false;
            gv.bSlideShow = bOn;
            cbSlideShow.Checked = bOn;
            return gv.bSlideShow;
        }
        public void stopSlideShow(string msg = null)
        {
            tbMessageFromMatching.Text = msg;
            gv.bSlideShow = false;
            SetTimerOn(false);
            timer.Stop();
            Cursor.Show();
            bScan = false;
            setSlideShowOn(false);
            //StopAll(); //dmc26
            // **NEW: Clear image cache to free memory**
            ClearRecentCache();
            ForceGarbageCollection();
        }
        /// <summary>
        /// Adds current image to the recent images cache for duplicate detection.
        /// Maintains a sliding window of the last N images.
        /// </summary>
        private void AddToRecentCache(Bitmap image, long fileSize, int index)
        {
            if (image == null) return;

            try
            {
                // Create a copy so the original can be disposed safely
                var cacheCopy = new Bitmap(image);

                _recentImagesCache.Enqueue((cacheCopy, fileSize, index));

                // Maintain cache size limit
                while (_recentImagesCache.Count > CACHE_SIZE)
                {
                    var oldest = _recentImagesCache.Dequeue();
                    oldest.Image?.Dispose(); // Free memory
                }
            }
            catch (Exception ex)
            {
                gv.debug.w($"Cache add error: {ex.Message}");
            }
        }
        /// <summary>
        /// Clears and disposes all cached images to prevent memory leaks
        /// </summary>
        private void ClearRecentCache()
        {
            while (_recentImagesCache.Count > 0)
            {
                var item = _recentImagesCache.Dequeue();
                item.Image?.Dispose();
            }
        }
        public void initTimer()
        {
            //stopSlideShow();
            timer.Interval = ((int)numSlideShowSpeed.Value) * (1000) + 1;              // Timer will tick every 3 seconds
            if (timer.Interval == 0)
                timer.Interval = 1;
            if (bScan)
                timer.Interval = 1;             // Timer fast
            if (gv.copyOnly)
                timer.Interval = 200;
            timer.Enabled = true;                       // Enable the timer
            if (!cbMasterOFF.Checked)
                timer.Start();                              // Start the timer

            //  label.Location = new Point(100, 100);
            //  label.AutoSize = true;
            //  label.Text = String.Empty;

            //   this.Controls.Add(label);
        }
        public void MasterStopSlideShow()
        {
            //bMasterStopSlideShow = true;
            timer.Stop();
        }
        public void initTimer(decimal isec)
        {
            int iseconds = Convert.ToInt32(isec);
            if (iseconds > 9)
                iseconds = 9;
            if (iseconds < 1)
                timer.Interval = 200;
            else
                timer.Interval = (1000) * (iseconds);              // Timer will tick every 3 seconds
        }

        public void initTimer(int isec)
        {
            int iseconds = isec - 96;
            if (iseconds > 9)
                iseconds = 9;
            if (iseconds < 1)
                timer.Interval = 200;
            else
                timer.Interval = (1000) * (iseconds);              // Timer will tick every 3 seconds

            //    timer.Enabled = true;                       // Enable the timer
            //    timer.Start();                              // Start the timer

            //  label.Location = new Point(100, 100);
            //  label.AutoSize = true;
            //  label.Text = String.Empty;

            //   this.Controls.Add(label);
        }
        public void initFastTimer(int speed)
        {
            this.stopSlideShow();
            if (speed < 1000)
            {
                // speed = 1;
                bScan = true;
            }
            timer.Interval = speed;             // Timer will tick every (speed /100) seconds
            timer.Enabled = true;                       // Enable the timer
            if (!cbMasterOFF.Checked)
                timer.Start();                              // Start the timer
            gv.debug.w("initFastTimer", "scan=", bScan.ToString());
            //  label.Location = new Point(100, 100);
            //  label.AutoSize = true;
            //  label.Text = String.Empty;

            //   this.Controls.Add(label);
        }

        public int removeSlide()
        {
            gv.nextIdx--;
            gv.slideCount1--;
            if (gv.nextIdx < 0)
                gv.nextIdx = 0;
            if (gv.slideCount1 < 0)
                gv.slideCount1 = 0;
            return gv.nextIdx;
        }
        public int showFirstSlide()
        {
            showNextSlideImageFileList1(0, 1);
            tbMaxSlideNumber.Text = $"{gv.slideCount1 - 1}";
            return gv.slideCount1;
        }
        int currentSlideNumber = 0;
        public int showPreviousSlide()
        {
            currentSlideNumber = gv.nextIdx;
            int currentSlide = gv.nextIdx - 1;
            int slideIdx = 0;
            Boolean rc = false;
            this.stopSlideShow();
            if (gv.nextIdx > 0)
            {
                //++gv.nextIdx;//test
                //dmc9 gv.nextIdx--;
                if (gv.nextIdx + 1 < currentSlideNumber)
                    gv.nextIdx = currentSlideNumber - 1;
                slideIdx = showNextSlide3(gv.nextIdx, -1);
                //slideIdx = showNextSlide3(gv.nextIdx, -1);
            }
            if (slideIdx < currentSlide)
            {
                System.Windows.Forms.MessageBox.Show("ERROR", $"slide < too far {currentSlide} but {slideIdx}");
                return slideIdx;
            }
            return 0;
        }
        DateTime dt;
        string fileExt = null;
        public bool copyOrMoveImage(char ch)
        {
            string targetDir;
            bool test1 = tbTargetFolder.Text.EndsWith("/");
            if (!test1)
                test1 = tbTargetFolder.Text.EndsWith("\\");
            if (!test1)
                targetDir = @tbTargetFolder.Text + "\\" + ch;
            else
                targetDir = @tbTargetFolder.Text + ch;
            this.stopSlideShow();
            bool bcopied = false;
            bool toggleColor = true;

            if (cbCatalogImages.Checked)
            {
                gv.debug.w($"--->> COPIED image file {fpathCurrentImage} to {targetDir}");
                btCopyImageInfo.Visible = true;
                btCopyImageInfo.Text = $"File Copy to >>{targetDir} \n\n {fpathCurrentImage}";
                tbMessage.Text = $"File Copy to >>{targetDir} \n\n {fpathCurrentImage}";
                //2024
                fileExt = System.IO.Path.GetExtension(fpathCurrentImage);
                //fileExt fpathCurrentImage
                this.Refresh();
                if (!Directory.Exists(@tbTargetFolder.Text))
                {
                    Directory.CreateDirectory(@tbTargetFolder.Text);
                }

                if (!Directory.Exists(@targetDir))
                {
                    Directory.CreateDirectory(@targetDir);
                }
                if (fpathCurrentImage != null)
                    if (!cbRename.Checked)
                    {
                        if (rbMove.Checked)
                        {
                            string combined = System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(fpathCurrentImage));
                            if (ff.verifyFileExists(combined))
                            {
                                System.Windows.Forms.MessageBox.Show($"{combined}", "File Already Exists");
                                return true; //dmc26 AI 
                            }

                            gv.debug.w("--->> attempt to MOVE image file >> ", fpathCurrentImage);

                            //-- 
                            bcopied = ff.MoveFile(fpathCurrentImage, targetDir); //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<MOVE FILE
                            //pb1.Image.Save()
                        }
                        else
                        {
                            gv.debug.w("--->> attempt to COPY image file >> ", fpathCurrentImage);
                            bcopied = ff.CopyFile(fpathCurrentImage, targetDir); //////////////////////////////////////// COPY or MOVE 
                        }
                        gv.debug.w("--->> COPIED/MOVED image file >> ", targetDir, fpathCurrentImage);

                        if (bcopied)
                        {
                            tbFpath.Text = $">> {targetDir} {fpathCurrentImage}";

                            bool targetFileExists = File.Exists(System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(fpathCurrentImage)));

                            if (!targetFileExists)
                            {
                                tbFpath.Text = "file copy error - target file not found after copy";
                                tbFpath.BackColor = Color.LightPink;
                                System.Windows.Forms.MessageBox.Show($"File copy/move error to {targetDir} {fpathCurrentImage}", fpathCurrentImage);
                                return false;
                            }
                            toggleColor = !toggleColor;
                            if (toggleColor)
                                tbMessage.BackColor = Color.LightGreen;
                            else
                                tbMessage.BackColor = Color.LightBlue;
                        }
                        else
                        {
                            tbFpath.Text = "file copy error";
                            tbFpath.BackColor = Color.LightPink;
                            System.Windows.Forms.MessageBox.Show($"File copy/move error to {targetDir} {fpathCurrentImage}", fpathCurrentImage);
                        }
                    }
                    else //rename
                    {
                        dt = DateTime.Now;
                        string fname = String.Format("{0:yyMMddHHmmssfff}", dt);
                        string ftype = ".jpg";
                        //
                        //2024

                        //fname = fname + ftype;
                        fname = fname + fileExt;
                        if (rbMove.Checked)
                        {
                            gv.debug.w("--->> attempt to MOVE image file >> ", fpathCurrentImage);
                            bcopied = ff.MoveFileRename(fpathCurrentImage, targetDir, fname);
                        }
                        else
                        {
                            gv.debug.w("--->> attempt to COPY image file >> ", fpathCurrentImage);
                            bcopied = ff.CopyFileRename(fpathCurrentImage, targetDir, fname); //////////////////////////////////////// COPY or MOVE 
                        }
                        gv.debug.w("--->> COPIED/MOVED and RENAMED image file >> ", targetDir, fpathCurrentImage, fname);
                        tbMessage.Text = $"--->> COPIED/MOVED and RENAMED image file >>  {targetDir}, {fpathCurrentImage}, {fname}";
                        /*
                        string testSource = fpathCurrentImage;
                        string testDest = Path.Combine(targetDir, "TEST_" + Path.GetFileName(fpathCurrentImage));
                        File.Copy(testSource, testDest, true);
                        bool testExists = File.Exists(testDest);
                        MessageBox.Show($"Test copy exists: {testExists}\nPath: {testDest}");
                        */

                        if (bToggleMessageColor)
                            tbMessage.BackColor = Color.LightBlue;
                        else
                            tbMessage.BackColor = Color.LightGreen;
                        bToggleMessageColor = !bToggleMessageColor;
                        if (bcopied)
                        {
                            tbFpath.Text = $">> {targetDir} {fname}";
                            toggleColor = !toggleColor;
                            if (toggleColor)
                                tbMessage.BackColor = Color.LightGreen;
                            else
                                tbMessage.BackColor = Color.LightBlue;
                        }
                        else
                        {
                            tbFpath.Text = "file copy error";
                            System.Windows.Forms.MessageBox.Show("ERROR on copy/move", fpathCurrentImage);
                        }
                    }
                if (!cbShowCopyStatus.Checked)
                    btCopyImageInfo.Visible = false;
            }
            return bcopied;
        }
        //
        // Safe cloning and assignment helpers + updated keepPreviousImage to avoid passing disposed/shared Image instances into PictureBoxes.

        private static Image CloneImageSafe(Image src)
        {
            if (src == null)
                return null;
            try
            {
                // create an independent copy so disposing one PictureBox's image won't affect another
                return new Bitmap(src);
            }
            catch
            {
                return null;
            }
        }

        private static void SafeAssignPictureBoxImage(PictureBox pb, Image newImage)
        {
            // detach and dispose previous image safely, then assign the new image (already cloned)
            var previous = pb.Image;
            try
            {
                pb.Image = newImage;
            }
            catch
            {
                // if assignment fails, ensure pb has no image
                pb.Image = null;
                newImage?.Dispose();
            }
            finally
            {
                if (previous != null && !ReferenceEquals(previous, pb.Image))
                {
                    try { previous.Dispose(); } catch { }
                }
            }
        }

        public void keepPreviousImage(int iDirection)
        {
            // Clone sources first to avoid referencing images that may be disposed below.
            var cloneThird = CloneImageSafe(pbThirdLast.Image);
            var cloneSecond = CloneImageSafe(pbSecondLast.Image);
            var cloneLast = CloneImageSafe(pbLastImage.Image);
            var cloneCurrent = CloneImageSafe(pb1.Image);

            // Assign clones into the shift order, disposing the replaced images inside SafeAssign.
            SafeAssignPictureBoxImage(pbFourthLast, cloneThird);
            SafeAssignPictureBoxImage(pbThirdLast, cloneSecond);
            SafeAssignPictureBoxImage(pbSecondLast, cloneLast);
            SafeAssignPictureBoxImage(pbLastImage, cloneCurrent);
        }

        //
        //





        bool bToggleMessageColor = true;
        MRListImages mostRecentImages;
        public void CreateMostRecentImageList()
        {
            mostRecentImages = new MRListImages();
            mostRecentImages.SetUpMRL();
        }
        public void processImageList(int idx)
        {
            finfo1 = gv.imageFileList1.getIndexed(idx);
            string fpath = finfo1.fpath;

            if (!File.Exists(fpath))
            {
                gv.debug.w("XXXXXXXXXXXXXXXX  DOES NOT EXIST fname: ", fpath, " ", gv.imageFileList1.getIndexed(idx).fname);
                return;
            }
            else // file does exists so load and send it to Display1
            {
                try
                {
                    LoadImageFromFinfo(fpath, 1, finfo1); /////exception thrown if invalid/corrupted IMAGE ///////////////////////
                }
                catch (Exception)
                {
                    StopAll();
                    gv.debug.w("XXXXXXXXXXXXXXXX  FILE LOAD ERR fname: ", fpath, " ", gv.imageFileList1.getIndexed(idx).fname);
                    return;
                }
                //gv.debug.w("+++++++++++++++++  FILE FOUND LOAD fname: ",  fpath, " ",  gv.imageFileList.getIndexed(idx).fname);
                //pb1.Refresh();
            }
        }
        public void SetImageList3(ImageFileList list)
        {
            gv.imageFileListCompare = new ImageFileList();
            if (gv.imageFileListCompare != null)
                gv.imageFileListCompare.clearList();
            gv.imageFileListCompare = list;
            btIdx3Show.BackColor = Color.LightGreen;
        }
        FileInfoItem finfo3;
        int idx3 = 0;
        public int ShowNextList3AndCompare() //was double return before 25
        {
            if (gv.imageFileListCompare == null)
                gv.imageFileListCompare = gv.imageFileList2;
            if (idx3 >= gv.imageFileListCompare.getImageCount())
            {
                idx3 = 0;
                return -1;
            }
            btIdx3Next.Text = idx3.ToString();
            btIdx3Next.Refresh();
            finfo3 = gv.imageFileList2.getIndexed(idx3);
            tbP2Size.Text = $"{finfo3.len}";
            tbMatchPercent.Text = "0";
            tbFilePath3.Text = "";
            //idx3++;
            if (p1Size != finfo3.len)  // CHECK FILE SIZE FIRST
                return 0;
            try
            {
                LoadImageFromFinfo(finfo3.fpath, 2, finfo3); //   2 = pb2         ///exception thrown if invalid/corrupted IMAGE ///////////////////////
            }
            catch (Exception)
            {
                StopAll();
                gv.debug.w("XXXXXXXXXXXXXXXX  FILE LOAD ERR fname: ", finfo3.fpath, " ", gv.imageFileList1.getIndexed(idx).fname);
                return -1;
            }
            double match = CompareImagesOnList(); //compare pb1 and pb2                   //<<<<<<<<<<<<<< actual compare 
            tbMatchPercent.Text = ((int)Math.Round(match)).ToString();

            if (match > 95)
            {
                lastFoundIdx3 = idx3;
                tbFilePath3.Text = finfo3.fpath;
                return (int)match;
            }
            return 0;
        }


        private void SlideShow_Click_1(object sender, EventArgs e)
        {
            gv.setCursorHourGlass();

            // System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chord.wav");
            gv.nextIdx = 0;

            if (startSlideShow(-100, true, 1))
            {
                initTimer();
            }

        }
        public void ContinueInitialization()
        {
            getSoundList();
        }
        // in a button click handler in Main.cs:
        public void btnSoundList_Click(object sender, EventArgs e)
        {
            ShowSoundListForm();
        }
        private async void btPlaySoundList_Click(object sender, EventArgs e)
        {
            ShowSoundListForm();
        }

        public void SoundDing()
        {
            SoundLevel(100, defaultAudioSound);
        }
        public void SoundLevel(int level, string soundFilePath = null)
        {
            if (level < 0)
                level = 50;
            SetAudioVolumeLevel(level);
            audio.Load(soundFilePath ?? defaultAudioSound);
            if (File.Exists(soundFilePath ?? defaultAudioSound))
            {
                audio.Play();
            }
            else
            {
                MessageBox.Show($"Sound file not found: {soundFilePath ?? defaultAudioSound}", "Error");
            }
        }
        public async Task PlaySoundListAsync(int pauseMilliseconds = 1000)
        {
            // Use local variable instead of class field
            for (int i = 0; i < soundList.Count; i++)
            {
                try
                {
                    // Play the sound
                    soundLocation = soundFiles[i];
                    SoundLevel(gv.soundVolume, soundLocation);

                    // Non-blocking pause to let sound finish/pause between sounds
                    await Task.Delay(pauseMilliseconds);
                }
                catch (Exception ex)
                {
                    gv.debug.w($"Failed to play sound {i}: {ex.Message}");
                }
            }
        }

        // Alternative: Wait for each sound to complete before playing next
        public async Task PlaySoundListSequentialAsync()
        {
            for (int i = 0; i < soundList.Count; i++)
            {
                try
                {
                    soundLocation = soundFiles[i];

                    // PlaySync blocks, so run it on a background thread
                    await Task.Run(() => SoundLevel(gv.soundVolume, soundLocation));

                    // Optional: small pause between sounds
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    gv.debug.w($"Failed to play sound {i}: {ex.Message}");
                }
            }
        }
        //public SoundItems sounds;

        public void getSoundList()
        {
            string path = @"C:\Windows\Media\";
            try
            {
                soundFiles = Directory.GetFiles(path, "*.wav");
            }
            catch (Exception)
            {

                //no directories /// or no ACCESS ...
            }
            createSoundList();
        }
        /// <summary>Opens the Sound List browser window.</summary>
        public void ShowSoundListForm()
        {
            getSoundList();   // refreshes soundFiles + soundList
            var frm = new SoundListForm(gv, soundList);
            frm.Show(this);
        }
        public List<SoundItems> soundList;
        public void createSoundList()
        {
            SoundItems sitems;
            soundList = new List<SoundItems>();

            for (int idx = 0; idx < soundFiles.Length; ++idx)
            {

                sitems = new SoundItems(idx, System.IO.Path.GetFileName(soundFiles[idx]), soundFiles[idx]);
                soundList.Add(sitems);
            }
        }
        bool soundOff = false;
        string soundLocation = null;
        public void soundAlert(int soundNumberDmc) //bool no sound #
        {
            if (soundOff)
                return;

            switch (soundNumberDmc)
            {
                case 1:
                    {
                        soundLocation = gv.soundHighGongPath;
                        break;
                    }
                case 2:
                    {
                        soundLocation = gv.soundHighAlertPath;
                        break;
                    }
                case 3:
                    {
                        soundLocation = gv.soundLowAlertPath;
                        break;
                    }
                case 4:
                    {
                        soundLocation = gv.soundLowGongPath;
                        break;
                    }
                case 5://ding
                    {
                        soundLocation = gv.soundDefault;
                        break;
                    }
                case 6://alert
                    {
                        soundLocation = gv.soundAlert;
                        break;
                    }
                case 35:
                    {
                        soundLocation = @"C:\Windows\Media\Windows Hardware Fail.wav";
                        break;
                    }
                case 46:
                    {
                        soundLocation = @"C:\Windows\Media\Windows Hardware Fail.wav";
                        break;
                    }
                case 49://alert
                    {
                        soundLocation = @"C:\Windows\Media\Windows Information Bar.wav";
                        break;
                    }
                case 55://alert
                    {
                        soundLocation = @"C:\Windows\Media\Windows Navigation Start.wav";
                        break;
                    }

                // 30   =  C:\Windows\Media\Speech Misrecognition.wav
                default:
                    {
                        soundLocation = gv.soundDefault;
                        break;
                    }
            }
            SoundLevel(gv.soundVolume, soundLocation);
        }
        //-----REV3D------------------MAIN ENTRY------------ STARTSLIDESHOW ----------boolean autoStart   --------slide 0 = start (manual or auto) ------
        // int startSlideNumber 
        // = -1 == NEXT SLIDE in sequence
        // = -2 == NEXT = 0  
        // -100 == reset to 0 
        // = out of range == reset to 0 
        //boolean autoStart <<< controls timer (re)start 
        // RETURN TRUE = okay continue
        // FALSE = error stop
        // does NOT display the image, only handles setup before 1st displayed image  //gv  showDirection  lastDisplayedIdx   lastDisplayedIdx = nextIdx      slideCount

        public Boolean startSlideShow(int startSlideNumber, Boolean autoStart, int direction)
        {
            int idx = 0;
            bool bLoadFirstImage = false;
            //Boolean startTimer = autoStart;
            //gv.bSlideShow
            if (direction == -1)
                gv.iShowDirection = -1;
            else
                gv.iShowDirection = 1; //FORWARD

            if (startSlideNumber == -100)//RESET
            {
                gv.nextIdx = 0;
                gv.lastDisplayedIdx = -1; //none
                bLoadFirstImage = true;
            }
            else if (startSlideNumber >= 0 && startSlideNumber < gv.slideCount1)
            {
                gv.nextIdx = startSlideNumber;
            }

            if (gv.nextIdx < 0 || gv.nextIdx >= gv.slideCount1)
            {
                gv.nextIdx = 0;
                gv.lastDisplayedIdx = -1; //none
            }

            activateDisplays();

            // MAIN SECTION  SHOW NEXT IMAGE nextIdx
            if (gv.nextIdx >= 0 && gv.nextIdx < gv.slideCount1)
            {
                Boolean rc = false;
                //current no image has been displayed ... so next goest to FIRST image
                gv.debug.w("ShowNextSlide2 in Main ....  StartSlideShow");
                this.showNextSlideImageFileList1(gv.nextIdx, 1, bLoadFirstImage);
                if (autoStart)
                {
                    setSlideShowOn(true);
                    cbSlideShow.Checked = true;
                    initTimer();
                }
                else
                {
                    timer.Enabled = false;
                    setSlideShowOn(false);
                    cbSlideShow.Checked = false;
                }
                return true;
            }//end nextside 
            return false;
        }
        public bool SetTimerOn(bool bFlag)
        {
            if (bFlag)
            {
                timer.Enabled = bFlag;
                timer.Start();
                btSlideShow.BackColor = Color.LightGreen;
            }
            else
                timer.Stop();
            timer.Enabled = bFlag;
            btSlideShow.BackColor = Color.LightGray;
            return bFlag;

        }
#pragma warning disable IDE1006 // Naming Styles

        // Activate and display the screens based on the boolean flags for each screen. <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        public void activateDisplays()
#pragma warning restore IDE1006 // Naming Styles
        {
            if (bScreen1)
                if (display1 == null || display1.IsDisposed) //////////// CREATE DISPLAY1
                {
                    display1 = new Display1(gv);
                    // if (gv.copyOnly)
                    //   screen2 = false;

                }

            if (bScreen2)
            {
                if (display2 == null || display2.IsDisposed)
                    display2 = new Display1(gv);
            }
            if (bScreen3)
            {
                if (display3 == null || display3.IsDisposed)
                    display3 = new Display1(gv);

            }
            if (bScreen4)
            {
                if (display3 == null || display3.IsDisposed)
                    display3 = new Display1(gv);

            }
            //nsertRow(1, gv.screen[1].Bounds.Width, gv.screen[1].Bounds.Height, gv.screen[1]x, gv.screen[1].Bounds.Width);
            if (bScreen1)
                display1.setDisplayMonitor(gv.screen[0], 1);

            if (bScreen2)
                display2.setDisplayMonitor(gv.screen[1], 2);
            if (bScreen3)
            {
                display3.setDisplayMonitor(gv.screen[2], 3);

            }
            if (bScreen4)
            {
                display4.setDisplayMonitor(gv.screen[3], 4);

            }
            // DISPLAY -- display1.Bounds = new Rectangle(gv.screen[1]x, 200, gv.screen[1].Bounds.Width - 100, gv.screen[1].Bounds.Height - 100);


            this.Focus();

            if (bScreen1)
            {
                display1.WindowState = FormWindowState.Maximized;
                display1.Visible = true;
                display1.setFullScreenMode(true); // set    display1  fullscreen mode
                display1.Activate();
                display1.Focus();
            }
            if (bScreen2)
            {
                display2.WindowState = FormWindowState.Maximized;
                display2.setFullScreenMode(true); // set    display2  fullscreen mode
                display2.Visible = true;
                display2.Activate();
                display2.Focus();
            }
            if (bScreen3)
            {
                display3.WindowState = FormWindowState.Maximized;
                display3.setFullScreenMode(true); // set    display2  fullscreen mode
                display3.Visible = true;
                display3.Activate();
                display3.Focus();
            }
            if (bScreen4)
            {
                display4.WindowState = FormWindowState.Maximized;
                display4.setFullScreenMode(true); // set    display2  fullscreen mode
                display4.Visible = true;
                display4.Activate();
                display4.Focus();
            }
        }
        //----------------------------------  // Manual Mode Next Slide <<<<<<<<<<<<<<
        void manual_NextSlide()
        {
        }
        //----------------------------------
        bool bShowDisplays = false;

        public bool setPreviewSetSize(int isize)
        {
            pageSizeForSlideShow = isize;
            runningPreviewSet = 1;
            return gv.bSlideShow;
        }
        public int pageSizeForSlideShow = 0;
        public int runningPreviewSet = 0;
        void timer_Tick(object sender, EventArgs e) // TIMER EVENT <<<<<<<<<<<<<<<<<<<< gv.nextSlide 
        {
            if (cbMasterOFF.Checked)
            {
                timer.Stop();
                return;
            }
            //label.Text = DateTime.Now.ToString();
            if (runningPreviewSet == 1)
            {
                if (gv.nextIdx >= 0 && gv.nextIdx < gv.slideCount1)
                {
                    gv.debug.w("showNextSlide2 in Main timer_Tick");
                    runningPreviewSet = showNextSlide3(gv.nextIdx, 1);
                }

                else
                {
                    stopSlideShow();
                    runningPreviewSet = 0;
                }
            }
            else
            {
                gv.debug.w("ShowNextSlide2 in Main .... timer_Tick");

                showNextSlide3(gv.nextIdx, 1);// -1 means increment nextIdx
            }
        }
        int timer2Counter = 0;
        bool bTimer2Running = true;

        void timer2_Tick(object sender, EventArgs e) // TIMER EVENT <<<<<<<<<<<<<<<<<<<< gv.nextSlide 
        {
            timer2.Stop();
            if (cbMasterOFF.Checked)
            {
                timer2.Stop();
                timer.Stop();
                return;
            }
            if (bTimer2Running)
                tbCountDown.Text = timer2Counter.ToString();
            ++timer2Counter;
            if (timer2Counter > 50)
            {

            }
            else if (timer2Counter > 5)
            {
                timer2Counter = 0;
                if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                {
                    gv.dialogTraverser2.call_from_timer_main();
                }

            }
        }

        public bool bSyncPreviewDisplay = false;

        public int showNextSlideNow() /////////////////////// force
        {
            if (gv.bSlideShow)
            {
                this.stopSlideShow();
                this.initTimer();
            }
            if (bSyncPreviewDisplay)
                syncPreviewDisplay();

            return showNextSlide3(gv.nextIdx, 1);
        }
        public int showNextNoTimer() /////////////////////// force
        {
            if (gv.bSlideShow)
            {
                this.stopSlideShow();
                // this.initTimer();
            }
            gv.debug.w("ShowNextSlide2 in Main ... showNextNoTimer");
            return showNextSlide3(gv.nextIdx, 1);
        }
        public int scanForward(int speed) /////////////////////// force
        {

            this.stopSlideShow();
            bScan = true;
            timer.Enabled = false;
            // timer.Dispose();
            //  timer = new System.Windows.Forms.Timer();
            //  timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            this.initFastTimer(speed);
            setSlideShowOn(true);

            return showNextSlide3(gv.nextIdx, 1);
        }

        bool bMoveInvalidfiles = false;
        bool bValidateImageFile = true;



        public bool displayThisImage2Screens()  //finfoItem
        {
            bool rc = false;
            if (bScreen1)
            {
                if (display1 == null)
                    return false;
                rc = display1.displayThisImage(finfo1);// (fpath);
            }
            else
                rc = true;

            if (!bScan)
            {

                if (bScreen2)
                    display2.displayThisImage(finfo1);// (fpath);
                if (bScreen3)
                {
                    display3.displayThisImage(finfo1);// (fpath);
                }
                if (bScreen4)
                {
                    display4.displayThisImage(finfo1);// (fpath);
                }
            }
            return rc;
        }
        public bool SetFullSizeMode()  //finfoItem
        {
            bool rc = false;
            if (bScreen1)
            {
                if (display1 == null)
                    return false;
                display1.SetFullScreenModeOn();
            }
            else
                rc = true;

            if (!bScan)
            {

                if (bScreen2)
                    display2.SetFullScreenModeOn();
                if (bScreen3)
                {
                    display3.SetFullScreenModeOn();
                }
                if (bScreen4)
                {
                    display4.SetFullScreenModeOn();
                }
            }
            return rc;
        }
        int THUMB_SIZE = 48; // 256;
        int THUMB_SIZE2 = 108; // 256;
        //Remember that you need to Dispose() the bitmap after using it.
        public int computeNextSlideNumber(int idx, int direction)
        {
            if (idx < 0)
            {
                gv.nextIdx = 0;
                gv.lastDisplayedIdx = -1;
            }
            else if (idx >= gv.slideCount1)
            {
                gv.nextIdx = gv.slideCount1 - 1;
            }
            else
            {
                gv.nextIdx = idx + direction;
            }
            return gv.nextIdx;
        }
        int testLastIdx = 0;

        public int showNextSlide3(int idx, int iDirection)
        {
            tbResultIdx3Compare.Text = "";
            pb2Image.Image = null;
            int idx2 = computeNextSlideNumber(idx, iDirection);
            showNextSlideImageFileList1(idx2, iDirection);
            return idx2;
        }
        int idx2 = 0;
        public int ScanImagesInList()
        {
            while (idx2 < gv.slideCount1)
            {
                bool match = CompareAndStopOnMatch();
                if (match)
                    return idx2;
                idx2 = showNextSlideImageFileList1(idx2, 1);
                idx2++; // = computeNextSlideNumber(idx, 1);
            }
            idx2 = 0;
            return idx2;
        }
        const int MaxX = 2440;
        const int MaxY = 1040;
        public void reposition()
        {
            foreach (Control c in this.Controls)
            {
                // get current position
                var loc = c.Location;

                // clamp it
                if (loc.X > MaxX) loc.X = MaxX;
                if (loc.Y > MaxY) loc.Y = MaxY;

                // re-assign
                c.Location = loc;
            }
        }
        ///////////////////////// NEXT IMAGE  is THIS IMAGE CURRENT IDX gv.nextSlide  ///////////////////////// -1 use gv.nextSlide++
        public int showNextSlideImageFileList1(int idx, int iDirection, bool bLoadFirstImage = false)
        {
            int idxRC = 1;

            List<FileInfoItem> ilist = gv.imageFileList1.finfoList;

            gv.debug.w("NEXT SLIDE2 request ", idx.ToString());
            if (iDirection < 0)
            {
                if (testLastIdx > idx + 1)
                    this.Text = "ERROR";
                testLastIdx = idx;
            }

            if (idx < 0)
            {
                gv.nextIdx = 0;
                gv.lastDisplayedIdx = -1;
            }
            else if (idx >= gv.slideCount1)
            {
                if (gv.nextIdx == gv.slideCount1 - 1) // already was at the end
                {
                    return gv.nextIdx;
                }
                gv.nextIdx = gv.slideCount1 - 1; // check 2025
            }
            else
            {
                gv.nextIdx = idx;
            }
            if (gv.nextIdx >= gv.slideCount1)
            {
                stopSlideShow();
                return 0;
            }
            gv.debug.w($">>>>>>>>>>>>>>>>>>>>>showNextSlide>>>{gv.nextIdx} << of MAX {gv.slideCount1}");
            //
            // dmcReadImage
            // dmcMetadata
            //
            idxRC = showNextImageFromList(gv.nextIdx, iDirection);
            //
            //updateLvFinfo(finfo1, idxRC); //show next
            tbFpath.Text = finfo1.fpath;
            updateDgvFinfo(finfo1);
            //
            if (bLoadFirstImage)
            {
                try
                {
                    LoadImageFromFinfo(finfo1.fpath, 1, finfo1);
                }
                catch (Exception ex)
                {
                    this.Text = $"Exception pb1.Load {ex.Message}";
                }
            }

            string fileName = finfo1.fpath;
            Bitmap thumbnail;

            if (!String.IsNullOrEmpty(fileName))
            {
                thumbnail = WindowsThumbnailProvider.GetThumbnail(fileName, THUMB_SIZE, THUMB_SIZE2, ThumbnailOptions.None);
                pb1ThumbNail.Image = thumbnail;
            }
            else
            {
                gv.debug.w("invalid image ", fileName);
                return -1;
            }

            pb1.Refresh();
            gv.debug.w("------ pb1.Refresh ");
            // NEW: update tbMatch when enabled after pb1 loaded
            UpdateMatchDisplay();

            gv.lastDisplayedIdx = idxRC;
            displayThisImage2Screens();
            if (cbPreview.Checked)
            {
                try
                {
                    previewNextImage(iDirection);

                }
                catch (Exception)
                {
                    StopAll();
                    MessageBox.Show("previousNextImage", "showNextSlideImageFileList1");
                }
            }
            //this.Refresh();

            if (bSyncPreviewDisplay)
            {
                if (iDirection == 1 || iDirection == -1)
                {
                    if (idx != 0)
                        syncPreviewDisplay();
                }
            }
            long prevFileSize = mostRecentImages.getIndexed(1).len;

            if (finfo1.len == nextFileSize && finfo1.len > 0)
            {
                if (cbFindDuplicates.Checked)
                {
                    double match = CompareImages2((Bitmap)pb1.Image, (Bitmap)pbNext.Image); //pb1 pbNext
                    tbMatchPercent.Text = ((int)Math.Round(match)).ToString();
                    if (cbSkipDuplicates.Checked)
                    {
                        if (match > 90)
                        {
                            SkipNextImage();
                        }
                    }
                    else if (match > 90)
                    {
                        DialogResult result;
                        if (cbContinueSearchDuplicates.Checked)
                        {
                            result = DialogResult.Yes;
                        }
                        else
                        {
                            stopSlideShow();

                            result = System.Windows.Forms.MessageBox.Show(
                                $"Mark next #{gv.nextIdx + 1} for Deletion?",
                                $"This image #{gv.nextIdx} and next image match",
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question);
                        }
                        if (result == DialogResult.Yes)
                        {
                            MarkNextForDeletion();
                            if (!cbContinueSearchDuplicates.Checked)
                                ResumeSlideShow();
                        }
                        else if (result == DialogResult.No)
                        {
                            ResumeSlideShow();
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            cbFindDuplicates.Checked = false;
                            stopSlideShow();
                            MasterOff();
                        }
                    }
                }
            }
            return idxRC;
        }
        // Add this helper and the small calls to update match display.

        private void UpdateMatchDisplay()
        {
            // Ensure we run on UI thread
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(UpdateMatchDisplay));
                return;
            }

            // Only act when enabled
            if (gv == null || !gv.showMatchPercentage)
            {
                try { tbMatch.Text = ""; } catch { }
                return;
            }

            // Need a source image to compare
            if (pb1?.Image == null)
            {
                tbMatch.Text = "";
                return;
            }

            double matchPercentage = 0;
            bool haveComparison = false;

            // Prefer pb2Image as the comparison target (existing compare helpers expect pb2Image)
            if (pb2Image?.Image is Bitmap bmp2 && pb1.Image is Bitmap bmp1)
            {
                try
                {
                    matchPercentage = CompareImages2(bmp1, bmp2);
                    haveComparison = true;
                }
                catch
                {
                    haveComparison = false;
                }
            }
            // Fallback: if pb2Image not present, try pbMatch


            if (haveComparison)
                tbMatch.Text = $"{matchPercentage:N2}%";
            else
                tbMatch.Text = "N/A";
        }
        //dmc9
        public int showNextImageFromList(int idx, int iDirection)
        {
            int rcIDX = 1;
            Boolean rc = false;
            bool bBadImage = false;

            if (idx < 0)
            {
                gv.nextIdx = 0;
                return 0;
            }
            if (idx >= gv.slideCount1)
            {
                setSlideShowOn(false);
                stopSlideShow();
                pb1.Image = pbNext.Image;
                return gv.slideCount1 - 1;
            }
            gv.nextIdx = idx;
            //gv.debug.w(String.Format(">>>>>>>>>>>>>>>>>>>>>----------------------------showNextSlide>>>{0} << of MAX {1}   last displayed{2} direction{3}", gv.nextIdx, (gv.slideCount1 - 1), gv.lastDisplayedIdx, iDirection.ToString()));

            rcIDX = getImageInfoByIndex(gv.nextIdx, iDirection, false); //bool getPreview .. so false = this is a main display not a preview
            if (rcIDX < 0)
                return rcIDX;
            FileInfoItem fi;
            fi = gv.imageFileList1.getIndexed(rcIDX);
            if (fi.fpath == null)
            {
                gv.nextIdx -= 1;
                return 0;
            }
            if (fi.fpath.Contains("mp4"))
                return 0;
            //
            fi.ndx = rcIDX;
            keepPreviousImage(iDirection);
            //
            if (rcIDX >= gv.slideCount1 - 1)
            {
                setSlideShowOn(false);
                stopSlideShow();
            }
            //
            //  LOAD IMAGE -=-=-=-=-------------------------------------------------
            //
            bool loadedImage = false;
            try
            {
                loadedImage = LoadImageFromFinfo(fi.fpath, 1, fi);   //dmc 2025
            }
            catch (Exception ex)
            {
                this.Text = $"Exception pb1.Load {ex.Message}";
            }
            return rcIDX;
            //
            //
        }
        public void keepPreviousImagewas(int iDirection)
        {
            pbFourthLast.Image = pbThirdLast.Image;
            pbThirdLast.Image = pbSecondLast.Image;
            pbSecondLast.Image = pbLastImage.Image;
            pbLastImage.Image = pb1.Image;
        }
        public void displayPreviousImagesInfo()
        {

            tbFourthLastSlideNumber.Text = mostRecentImages.getIndexed(4).ndx.ToString();
            tbThirdLastSlideNumber.Text = mostRecentImages.getIndexed(3).ndx.ToString();
            tbSecondlastSlideNumber.Text = mostRecentImages.getIndexed(2).ndx.ToString();

            tbSlideNumberOfLast.Text = mostRecentImages.getIndexed(1).ndx.ToString();

            tbPrevious4FileSize.Text = mostRecentImages.getIndexed(4).len.ToString();
            tbPrevious3FileSize.Text = mostRecentImages.getIndexed(3).len.ToString();
            tbPrevious2FileSize.Text = mostRecentImages.getIndexed(2).len.ToString();
            tbPreviousFileSize.Text = mostRecentImages.getIndexed(1).len.ToString();
        }
        public void CompareByFileSize()
        {

            if (mostRecentImages.getIndexed(0).len == mostRecentImages.getIndexed(1).len)
            {
                if (cbFindDuplicates.Checked)
                {
                    tbMatchRowNumber.Text = "MATCH?";  //idxRow.ToString();
                    stopSlideShow();
                }

            }
        }
        //
        //
        //----------- manage ImageFileList    MAIN DISPLAY IMAGE from file
        //
        //

        public int getImageInfoByIndex(int idx, int iDirection, bool previewGet) //--- 0 = no direction IDX only, 1 up -1 down /////////////////////// NEXT IMAGE  is THIS IMAGE CURRENT IDX gv.nextSlide  ///////////////////////// -1 use gv.nextSlide++
        {
            gv.debug.w(String.Format(">>>>>>>>>>>>>>>>>>>>>-------------------------- get NextSlide>>>{0} << of MAX {1}", idx, (gv.slideCount1 - 1)));
            // gv.lastDisplayedIdx
            //return rec number
            //iSlideNumber = gv.nextIdx;
            bool bGotImage = false;
            FileInfoItem tempFI = null;
            bool bContinue = true;

            while (!bGotImage && bContinue)
            {
                if (idx >= 0 && idx < gv.slideCount1)
                {

                    tempFI = gv.imageFileList1.getIndexed(idx);
                    if (!previewGet)
                        finfo1 = tempFI;
                    else
                    {
                        // if (iinfo2)
                    }
                    gv.debug.w($" got >> MAIN.getImageByIndex fname: {tempFI.fname}, rating:{tempFI.rating}, bInvalid={tempFI.bInvalid} preview={previewGet}");

                    //iinfo2 = getImageTest(tempFI);
                    return idx;

                    if (!iinfo2.bInvalid) //--- got image
                    {
                        return idx;
                    }
                    if (false && iinfo2.bDelete) //-------------------------- was invalid
                    {
                        deletedItems.addItem(iinfo2);
                        gv.debug.w("MAIN.getImageByIndex DELETE ITEM", iinfo2.fpath);

                        gv.imageFileList1.removeItemAt(idx);
                    }
                }
                else if (idx >= gv.slideCount1)
                {
                    return -1;
                }
                else if (idx < 0)
                {
                    return -1;
                }

                if (finfo1 == null || finfo1.fpath == null || tempFI == null)
                {
                    bContinue = DisplayContinueOrAbort(idx);

                }
                //idx += iDirection;
            }
            return idx;
        }
        bool bCorrectedLastNbr = false;

        public bool DisplayContinueOrAbort(int ndx)
        {
            stopSlideShow();
            if (cbMasterOFF.Checked == true)
            {
                StopAll();
                return false;
            }
            var result = DialogResult.Yes;
            //if (!cbCopyAll.Checked)
            result = System.Windows.Forms.MessageBox.Show($"ERR {ndx} Do you want to continue (YES) 1 or stop (NO)?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                // User chose “Continue”
                gv.nextIdx = ndx + 1;
                if (gv.nextIdx >= gv.slideCount1)
                {
                    MessageBox.Show("Reached end of list, resetting to 0");
                    gv.nextIdx = 0;
                    stopSlideShow();
                }
                return true;
            }
            else
            {
                // User chose “Abort”
                stopSlideShow();
                StopAll();
                //
                //
                cbCopyAll.Checked = false;
                // error step over the image and continue 
                // gv.nextIdx++;


                // old code to fix last nbr      changing to simple skip the bad image
                /*
                if (!bCorrectedLastNbr)
                {
                    bCorrectedLastNbr = true;
                    gv.iMaxFileCount = ndx;
                    gv.slideCount1 = gv.iMaxFileCount;
                    int lastNbr = gv.iMaxFileCount - 1;
                    tbMaxSlideNumber.Text = lastNbr.ToString();
                    gv.nextIdx = ndx;
                    showNextImageFromList(gv.nextIdx, gv.iMaxFileCount);
                    return false;
                }
                */
            }
            return false;
        }
        StatusOfProcessing status = null;
        private int _lastStatusHighest = -1;
        public void SendProgressStatus(int currentIdx, int nextIdx, int highestIdx, string lastImage)
        {
            EnsureStatus();
            status.UpdateStatus(currentIdx, nextIdx, highestIdx, lastImage);

            // gv.debug.w($"Progress Status - CurrentIdx: {currentIdx}, NextIdx: {nextIdx}, HighestIdx: {highestIdx}, LastImage: {lastImage}");
        }

        public FileInfoItem getImageInfo(int idx)
        {
            FileInfoItem finfo = gv.imageFileList1.getIndexed(idx);
            return finfo;
        }


        int loadedPreviewListNumber = -1;
        //dmc9
        bool bUseBitmapLoad = false;

        public int idxOfNextImage;

        string lastImageLoadedInPreview = "test0";
        private void EnsureStatus()
        {
            if (status == null || status.IsDisposed)
                status = new StatusOfProcessing(this.Location, this.Size);
        }

        private void UpdateStatusHighestIfChanged(int value)
        {
            EnsureStatus();
            if (value == _lastStatusHighest)
                return;

            _lastStatusHighest = value;
            status.UpdateStatusHighest(value);
        }

        public void previewNextImage(int iDirection)  // <<<<<<<<<<<<<<<<<<<<<<< PREVIEW NEXT IMAGE
        {
            string fpath2;
            EnsureStatus();
            bool loadedValidImage = false;
            while (!loadedValidImage)
            {
                //compute IDX
                if (iDirection > 0)
                {
                    if (gv.nextIdx < gv.slideCount1 - 1) //dmc check this
                        idxOfNextImage = gv.nextIdx + 1; //next
                    else
                        return;
                }
                else
                {
                    if (gv.nextIdx <= 0)
                        return;
                    idxOfNextImage = gv.nextIdx - 1; //previous
                }
                idxOfNextImage = getImageInfoByIndex(idxOfNextImage, 1, true); //true = preview  result in finfo1
                if (idxOfNextImage < 0)
                    return;
                //
                //
                iinfoNextImage = gv.imageFileList1.getIndexed(idxOfNextImage);
                iinfoNextImage.ndx = idxOfNextImage;
                //
                //dmc26
                if (iinfoNextImage.fpath == lastImageLoadedInPreview)
                {
                    gv.nextIdx++;
                    idxCopyHighestValue = gv.nextIdx;
                    UpdateStatusHighestIfChanged(idxCopyHighestValue);

                    continue;
                }
                lastImageLoadedInPreview = iinfoNextImage.fpath; //<<<<<<<<<<<< 
                //
                tbPreviewIndex.Text = idxOfNextImage.ToString();
                fpath2 = iinfoNextImage.fpath;
                loadedPreviewListNumber = idxOfNextImage;
                tagNextImageName.Text = iinfoNextImage.fname;

                //status.UpdateStatus(idxOfNextImage, gv.nextIdx, idxCopyHighestValue, lastImageLoadedInPreview);
                if (false && !cbDisplayPreview.Checked)
                {
                    _ = LoadPbNextFromThumbnail(fpath2);        //getWindowThumbnail(fpath2);
                    return;
                }

                tagNextImageName.BackColor = tagBackgroundColor;
                if (iinfoNextImage.bInvalid) //INVALID FILE FORMAT FOR IMAGE 
                {
                    gv.debug.w("__PREVIEW FOUND >>> INVALID IMAGE FILE HEADER in previewNextImage:", fpath2);
                    tagNextImageName.BackColor = Color.Red;

                    if (gv.imageFileErrorList == null)
                        gv.imageFileErrorList = new ImageFileList();

                    gv.imageFileErrorList.addItem(iinfoNextImage);
                    UpdateImageErrorsCount();
                    gv.imageFileList1.markRating("@", idxOfNextImage);
                    loadedPreviewListNumber = -1;
                    gv.nextIdx = idxOfNextImage;
                    idxCopyHighestValue = gv.nextIdx;
                    UpdateStatusHighestIfChanged(idxCopyHighestValue);
                    continue;
                }

                //pbNext.Image = pbTest.Image;
                //pbNext.Refresh();

                lastImageLoadedInPreview = iinfo2.fpath;

                bool loadedOk = false;
                try
                {
                    loadedOk = LoadImageFromFinfo(fpath2, 0, iinfoNextImage); /////exception thrown if invalid/corrupted IMAGE ///////////////////////
                    //   pb2.SizeMode = PictureBoxSizeMode.Zoom;
                    //   pb2.Update();
                    if (loadedOk)
                        loadedValidImage = true;
                }
                catch (Exception ex)
                {
                    if (gv.imageFileErrorList == null)
                        gv.imageFileErrorList = new ImageFileList();

                    var errItem = iinfoNextImage ?? new FileInfoItem(fpath2);
                    errItem.bInvalid = true;
                    if (string.IsNullOrWhiteSpace(errItem.comment))
                        errItem.comment = $"Preview load error: {ex.Message}";
                    gv.imageFileErrorList.addItem(errItem);
                    UpdateImageErrorsCount();
                    MarkNextInvalid();
                    gv.debug.w("---LOAD FAILED! >> INVALID FILE in PreviewNextImage pbNext.Load:", fpath2, ex.Message);
                    if (gv.debugLevel > 0)
                        MessageBox.Show($"LOAD FAILED! >> INVALID FILE in PreviewNextImage pbNext.Load: {fpath2}\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    loadedOk = false;


                    if (dialog == null || dialog.IsDisposed)
                        dialog = new DialogInvalidImage(gv);
                    dialog.Setup(fpath2, gv.nextIdx, gv.slideCount1, gv.nextIdx + 1); //continue cancel or STOP

                    dialog.ShowDialog();

                    if (gv.todoAction == "cancel")
                    {
                        // User chose to cancel the entire process
                        StopAll();
                        return;
                    }
                    else if (gv.todoAction == "stop")
                    {
                        stopSlideShow();
                        if (gv.nextIdx > lastImageError)
                        {
                            return; // Already moved to next image, so just return
                        }
                        if (gv.nextIdx < gv.slideCount1)
                        {
                            ++gv.nextIdx;
                            return;
                        }
                    }
                    else //"continue"
                    {
                        // Just continue without stopping
                    }
                    dialog.Dispose();
                } //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< LOOP TO LOAD VALID IMAGE

                if (!loadedOk)
                {
                    tagNextImageName.BackColor = Color.Red;
                    pbNext.Image = null;
                    gv.imageFileList1.markRating("@", idxOfNextImage);
                    loadedPreviewListNumber = -1;
                    gv.nextIdx = idxOfNextImage;
                    idxCopyHighestValue = gv.nextIdx;
                    UpdateStatusHighestIfChanged(idxCopyHighestValue);
                    continue;
                }

                if (bUseBitmapLoad)
                {
                    if (pbSecondLast.Image != null)
                    {
                        pbSecondLast.Image.Dispose();
                        pbSecondLast.Image = null;
                    }
                    Bitmap bmp = new Bitmap(fpath2);
                    pbSecondLast.Image = bmp;
                }

                return;
            } // LOOOP WHILE SEACHING FOR THE NEXT VALID IMAGE TO PREVIEW
        }
        public void previewNextImagexx22(int iDirection)  // <<<<<<<<<<<<<<<<<<<<<<< PREVIEW NEXT IMAGE
        {
            string fpath2;

            bool bOk = false;

            EnsureStatus();

        INC1MORE:

            //compute IDX
            if (iDirection > 0)
            {
                if (gv.nextIdx < gv.slideCount1 - 1) //dmc check this
                    idxOfNextImage = gv.nextIdx + 1; //next
                else
                    return;
            }
            else
            {
                if (gv.nextIdx <= 0)
                    return;
                idxOfNextImage = gv.nextIdx - 1; //previous
            }
            idxOfNextImage = getImageInfoByIndex(idxOfNextImage, 1, true); //true = preview  result in finfo1
            if (idxOfNextImage < 0)
                return;
            //
            //
            iinfoNextImage = gv.imageFileList1.getIndexed(idxOfNextImage);
            iinfoNextImage.ndx = idxOfNextImage;
            //
            //dmc26
            if (iinfoNextImage.fpath == lastImageLoadedInPreview)
            {
                gv.nextIdx++;
                idxCopyHighestValue = gv.nextIdx;
                UpdateStatusHighestIfChanged(idxCopyHighestValue);

                goto INC1MORE;
            }
            lastImageLoadedInPreview = iinfoNextImage.fpath; //<<<<<<<<<<<< 
            //
            tbPreviewIndex.Text = idxOfNextImage.ToString();
            fpath2 = iinfoNextImage.fpath;
            loadedPreviewListNumber = idxOfNextImage;
            tagNextImageName.Text = iinfoNextImage.fname;
            bool ok = false;

            //status.UpdateStatus(idxOfNextImage, gv.nextIdx, idxCopyHighestValue, lastImageLoadedInPreview);
            if (false && !cbDisplayPreview.Checked)
            {
                ok = LoadPbNextFromThumbnail(fpath2);        //getWindowThumbnail(fpath2);
                return;
            }

            tagNextImageName.BackColor = tagBackgroundColor;
            if (iinfoNextImage.bInvalid) //INVALID FILE FORMAT FOR IMAGE 
            {
                gv.debug.w("__PREVIEW FOUND >>> INVALID IMAGE FILE HEADER in previewNextImage:", fpath2);
                stopSlideShow();
                //gv.imageFileList.markInvalid(true, idx);
                //MessageBox.Show("FILE Error:" + fpath2);
                tagNextImageName.BackColor = Color.Red;
            }

            //pbNext.Image = pbTest.Image;
            //pbNext.Refresh();

            lastImageLoadedInPreview = iinfo2.fpath;
            try
            {
                LoadImageFromFinfo(fpath2, 0, iinfoNextImage); /////exception thrown if invalid/corrupted IMAGE ///////////////////////
                //   pb2.SizeMode = PictureBoxSizeMode.Zoom;
                //   pb2.Update();
            }
            catch (Exception)
            {
                StopAll();
                tagNextImageName.BackColor = Color.Red;
                gv.debug.w("---LOAD FAILED! >> INVALID FILE in PreviewNextImage pbNext.Load:", fpath2);
                pbNext.Image = null;
                gv.imageFileList1.markRating("@", idxOfNextImage);
                //gv.imageFileList.markInvalid(true, idx);
                loadedPreviewListNumber = -1;
            }
            if (!bUseBitmapLoad)
                LoadImageFromFinfo(fpath2, 0, iinfoNextImage);
            else
            {
                if (pbSecondLast.Image != null)
                    pbSecondLast.Dispose();
                Bitmap bmp = new Bitmap(fpath2);
                pbSecondLast.Image = bmp;
            }
            if (!String.IsNullOrEmpty(fpath2))
            {
                //pbNext.Image = thumbnail;

            }
        }
        public Bitmap CreateThumbnailForFile(string fpath2)
        {
            Bitmap thumbnail = null;
            if (!String.IsNullOrEmpty(fpath2))
            {
                try
                {
                    thumbnail = WindowsThumbnailProvider.GetThumbnail(fpath2, THUMB_SIZE, THUMB_SIZE2, ThumbnailOptions.None);
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("NO THUMBNAIL FOR" + fpath2);
                    return null;
                }
                return thumbnail;
            }
            return null;
        }
        public bool LoadPbNextFromThumbnail(string fpathname)
        {
            Bitmap thumbnail;
            thumbnail = WindowsThumbnailProvider.GetThumbnail(fpathname, THUMB_SIZE, THUMB_SIZE2, ThumbnailOptions.None);
            //pbThumbNail2.Image = thumbnail;
            if (pbNext.Image != null)
                pbNext.Dispose();
            pbNext.Image = thumbnail;
            //LoadImage(fpathname, 0);
            return (thumbnail != null);
        }
        public Bitmap getWindowThumbnail(string fpath2)
        {
            Bitmap thumbnail;
            if (!String.IsNullOrEmpty(fpath2))
            {
                thumbnail = WindowsThumbnailProvider.GetThumbnail(fpath2, THUMB_SIZE, THUMB_SIZE2, ThumbnailOptions.None);
                return thumbnail;
            }
            return null;
        }
        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        private bool IsValidImage(string filename) //////////////////////////////////// VALIDATE IMAGE FILE format  
        {
            Stream imageStream = null;
            try
            {
                imageStream = new FileStream(filename, FileMode.Open);

                if (imageStream.Length > 0)
                {

                    //testdmc2018

                    //
                    byte[] header = new byte[30]; // Change size if needed.
                    string[] imageHeaders = new[]
            {
                "BM",       // BMP
                "GIF",      // GIF
                Encoding.ASCII.GetString(new byte[]{137, 80, 78, 71}),// PNG
                "MM\x00\x2a", // TIFF
                "II\x2a\x00" // TIFF
            };

                    imageStream.Read(header, 0, header.Length);

                    bool isImageHeader = imageHeaders.Count(str => Encoding.ASCII.GetString(header).StartsWith(str)) > 0;
                    if (imageStream != null)
                    {
                        imageStream.Close();
                        //  imageStream.Dispose();
                        imageStream = null;
                    }

                    if (isImageHeader == false)
                    {
                        //Verify if is jpeg
                        using (BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open)))
                        {
                            UInt16 soi = br.ReadUInt16();  // Start of Image (SOI) marker (FFD8)
                            UInt16 jfif = br.ReadUInt16(); // JFIF marker

                            return soi == 0xd8ff && (jfif == 0xe0ff || jfif == 57855);
                        }
                    }

                    return isImageHeader;
                }

                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (imageStream != null)
                {
                    imageStream.Close();
                    imageStream.Dispose();
                }
            }
        }

        ///----------------------------------    END OF SLIDESHOW NEXT SLIDE (auto and manual modes) -----------------------------
        ///

        public void setTitle(FileInfoItem fi, string txt)
        {
            this.Text = $"file {buildPictureLabel(finfo1)}         {txt}";
        }

        public string buildPictureLabel(FileInfoItem fi)
        {
            //            string title = String.Format("{0:###############} {1:###,###,###}      fpath = {2}",       ">>"+fi.fname+"<<",    fi.len,   fi.fpath);
            //            title += "  image# " + gv.nextIdx.ToString() + " of " + (gv.slideCount1 -1).ToString();
            //string title = "image# " + gv.nextIdx.ToString() + " of " + (gv.slideCount1 - 1).ToString();
            string title = String.Format("image# {0:#,###,###} of {1:#,###,###} size={2:##,###,###} KB   >>{3}     {4}", gv.nextIdx + 1, gv.slideCount1, (fi.len / 1024) + 1, fi.fname, "       ", fi.fpath);
            return title;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        bool browser2;

        private void button1_Click_2(object sender, EventArgs e)
        {
            if (browser2 == null)//|| browser2.IsDisposed)
            {
                //  wb2 = new Browser2(gv);
                // browser2.goHome();
            }

            // wb2.Activate();
            //wb2.Visible = true;


        }




        private void buttonSmaller_Click(object sender, EventArgs e)
        {
            System.Drawing.Size size = pb1.Image.Size;

            gv.debug.w(size.Width.ToString() + " " + size.Height.ToString());
            double Width = (pb1.Image.Width * .95);

            double Height = (pb1.Image.Size.Height * .95);

            pb1.Image = ResizeBitmap(pb1.Image, (int)Width, (int)Height);  //(int) size.Width * 1.1, (int)size.Height * 1.1);
            ///
            size = pb1.Image.Size;
            gv.debug.w(size.Width.ToString() + " " + size.Height.ToString());

            pb1.Update();
        }

        private void btFileFunctions_Click(object sender, EventArgs e)
        {
            FileFunctionsDialog ffd = new FileFunctionsDialog(gv, null);
            ffd.Visible = true;
        }

        private void btCatalogImages_Click(object sender, EventArgs e)
        {

            catalogImages();

        }//slideShow

        /*
        public string fname;
        public string type;
        public int level;
        public string fpath;
         */
        public void moveImage(char ch)
        {
            string targetDir = gv.targetFolder + ch;
            this.stopSlideShow();
            if (fpathCurrentImage != null)
            {
                gv.debug.w("DISABLED MOVE OF FILES >>>>>>>> attempt to move image file >> ", fpathCurrentImage);
                if (fpathCurrentImage != null)
                    return;
                ff.MoveFile(fpathCurrentImage, targetDir);

                //gv.imageFileList.getIndexed(nextSlide).fname;

                gv.imageFileList1.updatePath(System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(fpathCurrentImage)), gv.nextIdx);

                gv.debug.w("ShowNextSlide2 in Main moveImage");
                this.showNextSlideImageFileList1(gv.nextIdx, 1);
            }
        }



        private void btSetRootDir_Click(object sender, EventArgs e)
        {
            // RootSetterDialog rd = new RootSetterDialog(gv, this);

            //  rd.Visible = true;

        }

        public void renameImage(string fname)
        {
            origFname = finfo1.fname;
            this.finfo1.fname = mtbCount.Text + "_" + btCompareLoad.Text + iinfo2.ext;
            updateLvFinfo(finfo1); //rename
            showNewName(finfo1.fname);
            // tbFilePath.Text =  finfoItem.dpath + "/" + finfoItem.fname;
        }

        string sNewName = null;

        public void showNewName(string postName)
        {
            // renameImage(
            sNewName = postName;
        }

        public string getNewName()
        {
            return sNewName;

        }


        ///insertRowLvParms(string pname, string pvalue)
        public int assignInit1ParmsFromGlobals()
        {
            if (gv.initParm1List.Count <= 0)
                gv.initParm1List.Add(new InitParms1());
            if (gv.initParm1List.Count > 0)
            {
                gv.initParm1List[0].dirPath = gv.dataFolder;
                gv.initParm1List[0].fpath = gv.inifileFullPathName;
                if (string.IsNullOrEmpty(gv.initParm1List[0].sourceDir1))
                    gv.initParm1List[0].sourceDir1 = gv.lastTraversedFolder;
                gv.initParm1List[0].SetTargetDir1(gv.initParm1List[0].targetDir2);
            }
            return gv.initParm1List.Count;
        }
        public InitParms1 assignGlobalInitVars(InitParms1 initvars)
        {
            gv.dataFolder = initvars.dirPath;
            gv.inifileFullPathName = initvars.fpath;
            gv.lastTraversedFolder = initvars.mostRecent;
            //gv.dirTarget4Copy = initvars.targetDir;
            return initvars;
        }
        //
        // 2025
        //
        string initFileName = "sdb8.xml";
        public void SaveInitFile()
        {
            // 1) Build a path under LocalApplicationData:
            string folder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MyCompanyName",     // or any folder name you prefer
                "MyAppName"         // your application name
            );

            // 2) Ensure the folder exists
            Directory.CreateDirectory(folder);

            // 3) Construct the init file’s full path
            string initFilePath = System.IO.Path.Combine(folder, initFileName);

            // 4) Write your init file here 
            // (e.g., XML serialization, plain text, JSON, etc.)
            File.WriteAllText(initFilePath, "<init>Some settings</init>");

            // Optional: Show the path for debugging
            System.Windows.Forms.MessageBox.Show($"Init file saved to:\n{initFilePath}");
        }

        public void LoadInitFile()
        {
            // Recreate the path
            string folder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MyCompanyName",
                "MyAppName"
            );
            string initFilePath = System.IO.Path.Combine(folder, initFileName);

            if (!File.Exists(initFilePath))
            {
                System.Windows.Forms.MessageBox.Show("No Init File found. Please create or handle defaults.");
                return;
            }

            // Load the content (XML, JSON, etc.)
            string fileContent = File.ReadAllText(initFilePath);
            // Do something with fileContent...
        }
        //
        //
        //
        public int readInitParms1File()
        {
            int count = DialogSearchExtensionsEditor.LoadFromFile(gv);
            if (count < 1)
            {
                MessageBox.Show("error no x file", "error no x file");
            }
            gv.debug.w("read ini file :");
            int iParmCount = ff.readInitParms1File(gv.inifileFullPathName);
            //if (iParmCount == -2) //recreated
            // return (int) gv.initParm1ItemList.Count();
            //  iParmCount = ff.readIniFile(gv.inifile);
            if (iParmCount < 1)
                iParmCount = createDefaultInitParm1File();
            return iParmCount;
            // gv.dirFullpath = gv.initParms[0].pvalue;
        }

        public int createDefaultInitParm1File(string filepath = null)
        {
            InitParms1 ip = new InitParms1();
            if (filepath != null)
            {
                gv.inifileFullPathName = filepath;
            }
            if (gv.initParm1List.Count <= 0)
            {
                ip.dirPath = gv.dataFolder;
                ip.fpath = gv.inifileFullPathName;
                ip.SetTargetDir1("C:\\");   // safe fallback — Count is 0, can't read [0]
                if (gv.initParm1List == null)
                    gv.initParm1List = new List<InitParms1>();
                gv.initParm1List.Add(ip);
            }
            /*
            if (gv.initParm1List.Count <= 0)
            {
                ip.dirPath = gv.default_init_Path;//System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                ip.fpath = gv.inifileFullPathName;
                //= tbTargetFolder.Text;
                ip.SetTargetDir1(gv.initParm1List[0].targetDir1);
                if (gv.initParm1List == null)
                    gv.initParm1List = new List<InitParms1>();
                gv.initParm1List.Add(ip);
            }
            */
            saveInitParms();// gv.initParm1List);
            return gv.initParm1List.Count;
        }
        private void listView1_SelectedIndexChanged_2(object sender, EventArgs e)
        {

        }
        private void cbCopyOnly_CheckedChanged_2(object sender, EventArgs e)
        {
            setCopyOnly();
            //bCopyMode = true;
        }

        private void cbCopyOnly_CheckedChanged(object sender, EventArgs e)
        {
            setCopyOnly();
        }

        private void setCopyOnly()
        {
            if (cbAutoAdvance.Checked)
            {
                gv.copyOnly = true;
                gv.mainWindow.initFastTimer(600);
                SlideShow_Click_1(null, null);
            }
            else
            {
                gv.copyOnly = false;
            }
        }

        private void lvInitParms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonTraverse_Click_1(object sender, EventArgs e)
        {
            gv.setCursorHourGlass();

            DialogTraverser test = new DialogTraverser(gv, this, 0, ff);
            test.Visible = true;
            //  test.traverse("C:\\testdir");
            //test.traverse("D:\\Documents and Settings\\mcclandx\\My Documents\\DavidsPictures");

        }
        /// <summary>
        /// Clears and disposes of images in the slideshow history PictureBoxes
        /// to prevent memory leaks and red X errors when restarting the slideshow.
        /// </summary>
        private void ClearSlideShowHistoryImages()
        {
            // Helper to safely clear a PictureBox
            void SafeClear(PictureBox pb)
            {
                if (pb?.Image != null)
                {
                    var img = pb.Image;
                    pb.Image = null;   // Detach first
                    img.Dispose();     // Then dispose
                }
            }

            SafeClear(pbFourthLast);
            SafeClear(pbThirdLast);
            SafeClear(pbSecondLast);
            SafeClear(pbLastImage);

            // Optional: Also clear the current thumbnail
            SafeClear(pb1ThumbNail);
        }

        //startSlideShow(int startSlideNumber, Boolean autoStart, int direction)
        private void btSlideShow_Click(object sender, EventArgs e)
        {
            //soundAlert(58);
            if (!gv.FILE_TYPE.Equals("images"))
                return;

            // Check current state BEFORE toggling
            bool wasRunning = cbSlideShow.Checked;

            if (wasRunning)
            {
                // Currently running - STOP it
                cbSlideShow.Checked = false;
                gv.bSlideShow = false;
                stopSlideShow();
                return;
            }
            cbMasterOFF.Checked = false;
            // Currently stopped - START it
            cbSlideShow.Checked = true;
            ClearSlideShowHistoryImages();
            bStartedSlideShowScan = true;
            // cbMasterOFF.Checked = false;

            dgvFileInfo.Show();

            if ((ModifierKeys & Keys.Control) == Keys.Control) //Control Key
                this.startSlideShow(gv.nextIdx, true, 1);
            else
                this.startSlideShow(-100, true, 1);

            setSlideShowOn(true);
        }
        private void btSlideShow_Clickxx(object sender, EventArgs e)
        {
            soundAlert(58);
            if (!gv.FILE_TYPE.Equals("images"))
                return;
            //
            cbSlideShow.Checked = !cbSlideShow.Checked;
            //
            if (!cbSlideShow.Checked)
            {
                gv.bSlideShow = false;
                stopSlideShow();
                return;
            }
            ClearSlideShowHistoryImages();
            bStartedSlideShowScan = true;
            //  cbMasterOFF.Checked = false;

            //cbSlideShow.Checked = !cbSlideShow.Checked;

            dgvFileInfo.Show();
            //this.pb1.Image = null;
            //this.Refresh();

            if ((ModifierKeys & Keys.Control) == Keys.Control) //Control Key
                this.startSlideShow(gv.nextIdx, true, 1);
            else
                this.startSlideShow(-100, true, 1);
            setSlideShowOn(true);
        }
        string lastFolder = "startFolder";

        public void ContinueSlideShow()
        {
            this.startSlideShow(gv.nextIdx, true, 1);
            setSlideShowOn(true);
            initTimer();
        }
        public void FindNextFolder()
        {
            int rcIDX = 0;
            string dir = finfo1.dpath;
            stopSlideShow();
            while (rcIDX >= 0)
            {
                rcIDX = getImageInfoByIndex(gv.nextIdx, 1, false); //bool getPreview .. so false = this is a main display not a preview
                if (!dir.Equals(finfo1.dpath))
                {
                    tbMessage.Text = finfo1.fpath;
                    //ContinueSlideShow();
                    updateDgvFinfo(finfo1);
                    showNextImageFromList(gv.nextIdx, 1);

                    return;
                }
                else
                {
                    ++gv.nextIdx;
                    if (gv.nextIdx >= gv.slideCount0)
                    {
                        stopSlideShow();
                        gv.nextIdx--;
                        return;
                    }
                }
            }
            stopSlideShow();

        }
        public void SearchNextFolder2()
        {
            int rcIDX = 0;
            string dir = finfo1.dpath;
            while (rcIDX >= 0)
            {
                rcIDX = getImageInfoByIndex(gv.nextIdx, 1, false); //bool getPreview .. so false = this is a main display not a preview
                if (SetFolder(finfo1.dpath))
                {
                    tbMessage.Text = finfo1.fpath;
                    ContinueSlideShow();
                    return;
                }
                else
                {
                    ++gv.nextIdx;
                    if (gv.nextIdx >= gv.slideCount0)
                    {
                        stopSlideShow();
                        return;
                    }
                }
            }
        }
        public bool SetFolder(string dir)
        {
            bool bNext = false;
            if (lastFolder != null)
            {
                if (!lastFolder.Equals(dir))
                {
                    if (cbStopNextFolder.Checked)
                        stopSlideShow();
                    lastFolder = dir;

                }
                //tbLastFolder.Text = lastFolder;
                lastFolder = dir; //next
                bNext = true;

            }
            else
            {
                lastFolder = dir;
                // tbLastFolder.Text = lastFolder;
            }
            return bNext;
        }
        public void RunSlideShowToNextFolder()
        {
            startSlideShow(gv.nextIdx, true, 1);
            setSlideShowOn(true);
        }
        bool bStartedSlideShowScan = false;

        public void StartSlideShowScan()
        {
            if (!gv.FILE_TYPE.Equals("images"))
                return;
            stopSlideShow();
            cbFindDuplicates.Checked = true;
            activateImageButtons();
            if (!bStartedSlideShowScan)
            {
                btResume.Text = "Resume F11";
                bStartedSlideShowScan = true;
                numSlideShowSpeed.Value = 0;
                this.startSlideShow(gv.nextIdx, true, 1);
                //this.startSlideShow(-100, true, 1);
                setSlideShowOn(true);
            }
        }
        //start Display SlideShow in MANUAL MODE (int startSlideNumber, Boolean autoStart, int direction)

        public bool bShowImagesEnabled = false;
        private void btDisplay1_Click(object sender, EventArgs e)
        {
            DisplayImages();
        }
        public void DisplayImages()
        {
            if (gv.FILE_TYPE.Equals("html"))
            {
                DisplayHTMLlist();
                return;
            }

            if (!gv.FILE_TYPE.Equals("images"))
                return;

            // Initialize error list
            if (gv.imageFileErrorList == null)
                gv.imageFileErrorList = new ImageFileList();
            else
                gv.imageFileErrorList?.clearList();

            UpdateImageErrorsCount();

            // Verify we have images to display
            if (gv.imageFileList1 == null || gv.slideCount1 <= 0)
            {
                tbMessage.Text = "No images loaded. Please load an image list first.";
                tbMessage.BackColor = Color.Orange;
                return;
            }

            bShowImagesEnabled = true;
            lastCountLoaded = gv.slideCount1;
            activateImageButtons();
            cbDisplayInfomation.Checked = false;
            dgvFileInfo.Show();

            // Reset to beginning
            gv.nextIdx = 0;
            gv.lastDisplayedIdx = -1;

            // **FIX: Force initial image load and display**
            try
            {
                // Get first image info
                finfo1 = gv.imageFileList1.getIndexed(0);

                // Load the image into pb1
                LoadImageFromFinfo(finfo1.fpath, 1, finfo1);

                // Update UI to show the image info
                updateDgvFinfo(finfo1);
                tbFpath.Text = finfo1.fpath;

                // Load thumbnail
                if (!string.IsNullOrEmpty(finfo1.fpath) && File.Exists(finfo1.fpath))
                {
                    var thumbnail = WindowsThumbnailProvider.GetThumbnail(
                        finfo1.fpath,
                        THUMB_SIZE,
                        THUMB_SIZE2,
                        ThumbnailOptions.None);
                    pb1ThumbNail.Image = thumbnail;
                }

                // Display on screens if active
                displayThisImage2Screens();

                // Load preview of next image
                if (cbPreview.Checked && gv.slideCount1 > 1)
                {
                    previewNextImage(1);
                }

                pb1.Refresh();
                this.Refresh();

                tbMessage.Text = $"Loaded {gv.slideCount1} images";
                tbMessage.BackColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                tbMessage.Text = $"Error displaying first image: {ex.Message}";
                tbMessage.BackColor = Color.OrangeRed;
                gv.debug.w($"DisplayImages error: {ex.Message}");
            }
        }
        private void btDebugDisplayState_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"FILE_TYPE: {gv.FILE_TYPE}");
            sb.AppendLine($"slideCount1: {gv.slideCount1}");
            sb.AppendLine($"nextIdx: {gv.nextIdx}");
            sb.AppendLine($"imageFileList1 null? {gv.imageFileList1 == null}");
            sb.AppendLine($"pb1.Image null? {pb1.Image == null}");
            sb.AppendLine($"finfo1.fpath: {finfo1?.fpath ?? "null"}");

            if (gv.imageFileList1 != null)
            {
                sb.AppendLine($"imageFileList1.Count: {gv.imageFileList1.getImageCount()}");
                if (gv.imageFileList1.getImageCount() > 0)
                {
                    var first = gv.imageFileList1.getIndexed(0);
                    sb.AppendLine($"First image: {first?.fpath ?? "null"}");
                    sb.AppendLine($"File exists? {File.Exists(first?.fpath ?? "")}");
                }
            }

            MessageBox.Show(sb.ToString(), "Display State Debug");
        }
        public void DisplayImagesxxxxxx()
        {
            if (gv.FILE_TYPE.Equals("html"))
                DisplayHTMLlist();
            if (!gv.FILE_TYPE.Equals("images"))
                return;
            if (gv.imageFileErrorList == null)
                gv.imageFileErrorList = new ImageFileList();
            else
                gv.imageFileErrorList?.clearList();
            UpdateImageErrorsCount();
            bShowImagesEnabled = true;
            lastCountLoaded = gv.slideCount1;
            activateImageButtons();
            cbDisplayInfomation.Checked = false;
            dgvFileInfo.Show();
            //
            gv.nextIdx = 0;
            this.showNextSlideImageFileList1(gv.nextIdx, 1, true);
            /*
            if (gv.nextIdx < 0 || gv.nextIdx >= gv.slideCount1)
                this.startSlideShow(-100, false, 1); //manual timer=false
            else
                this.startSlideShow(gv.nextIdx, false, 1); //manual timer=false
            */
        }
        //start Display SlideShow in MANUAL MODE (int startSlideNumber, Boolean autoStart, int direction)
        private void DisplayHTMLlist()
        {
            lastCountLoaded = gv.slideCount1;
            //activateImageButtons();
            cbDisplayInfomation.Checked = false;
            gv.nextIdx = 0;
        }
        private void btSlideShow_Click1(object sender, EventArgs e)
        {
            if (bScreen1)
            {
                // if (display1 != null)
                //  if (display1.IsAccessible)
                // display1.Focus();
            }
            gv.mainWindow.showNextSlideNow();

        }
        private void closeButton_Click_1(object sender, EventArgs e)
        {

        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            //  if (gv.debugLevel > 1)
            //    gv.debug.w("main  Key  Up Captured", e.ToString());

            //this.Close();
        }

        bool enteringSlideNumber = false;

        private void Main_KeyPress(object sender, KeyPressEventArgs e) //////
        {
            if (enteringSlideNumber)
            {
                return;
            }
            char key_char = e.KeyChar;
            string key = e.ToString();
            if (gv.debugLevel > 9)
                gv.debug.w("Main_KeyPress in >>>>>>>>>> main window");
            else
                return;
            switch (key_char)
            {
                case (char)Keys.Escape: ////////////////////////////////////////  CLOSE D1 Window //////////
                    Cursor.Show();
                    // Cursor.Position = new Point(111, 111);
                    if (cbEscToCloseApp.Checked)
                        this.Close();
                    break;
                case (char)Keys.Right:
                    gv.debug.w("MAIN keypress right");
                    if (bScreen1Focus) display1.Focus();
                    else
                    {
                        gv.debug.w("ShowNextSlide2 in Main  Main_KeyPress RIGHT");
                        showNextSlide3(gv.nextIdx, 1);
                    }
                    break;
                case (char)Keys.Left:
                    if (bScreen1Focus) display1.Focus();
                    else
                        showPreviousSlide();
                    break;
                case (char)Keys.Decimal:
                    markDeleted();
                    break;
                default:
                    break;
            }


        }
        /*
            * through overriding the ProcessCmdKey() method of the form. That way, 
             * your key handling logic gets executed no matter what control 
            * has focus at the time of keypress. 
            * Beside that, you even get to choose whether the focused control gets the key 
            * after you processed it (return false) or not (return true).
         *
         * When you "return true" you are handling the key press
            * */
        bool controlKey = false;
        //
        //
        bool deleteInsteadOfMove = false;
        //
        //
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) /////// override dmcdmc123
        {
            if (enteringSlideNumber)
            {
                return false;
            }

            if (keyData == Keys.Left)
            {
                stopSlideShow();
                gv.debug.w("mainWindow  Key   Left");
                ////if (!screen1)
                //   gv.mainWindow.showPreviousSlide();
                showPreviousSlide();
                return true; //for the active control to see the keypress, return false
            }
            else if (keyData == (Keys.Left | Keys.Control))
            {
                gv.debug.w("mainWindow Key Cntl + Left");
                int idx = gv.nextIdx - 100;
                if (idx < 0)
                    idx = 0;
                showNextSlide3(idx, -1);
            }
            else if (keyData == Keys.F1) ///// HELP
            {
                gv.showHelp(this.Location, "main");
            }
            else if (keyData == Keys.F3) ///// just to have F3 assigned
            {
                gv.showHelp(this.Location, "main");
            }
            else if (keyData == Keys.F6)
            {
                if (display1 != null)
                    display1.Activate();
            }
            else if (keyData == Keys.F12)
            {
                if (cbFindDuplicates.Checked)
                {
                    if (fileSizePrevious == fileSize)
                    {
                        markDeleted();
                    }
                    ResumeDupSearch();
                }
            }
            else if (keyData == Keys.F11)
            {
                if (cbFindDuplicates.Checked)
                {
                    ResumeDupSearch();
                }
            }
            else if (keyData == Keys.Enter)
            {
                return false;
            }
            else if (keyData == (Keys.Alt | Keys.PageDown))
            {
                gv.mainWindow.scanForward(10);
                gv.debug.w("------------------------alt pagedown");
                return true;
            }
            else if (keyData == (Keys.Control | Keys.PageDown))
            {
                gv.mainWindow.scanForward(500);
                gv.debug.w("--------------------------cntl pagedown");
                return true;
            }
            else if (keyData == (Keys.PageDown))
            {
                showNextSlide3(gv.nextIdx + 100, 1);
                gv.debug.w("-----------------------pagedown");
                return true;
            }
            else if (keyData == (Keys.PageUp))
            {
                showNextSlide3(gv.nextIdx - 100, 1);
                gv.debug.w("-----------------------pageup");
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Right))
            {
                showNextSlideImageFileList1(gv.nextIdx + 100, 1); //skip
            }
            else if (keyData == Keys.Right)
            {
                gv.debug.w("mainWindow RIGHT");
                //if (!screen1)
                gv.debug.w("ShowNextSlide2 in Main  ProcessCmdKey RIGHT");
                showNextSlide3(gv.nextIdx, 1);
                return true; //for the active control to see the keypress, return false
            }
            else if (keyData == Keys.Down)
            {
                gv.debug.w("mainWindow  Key   DOWN");
                scanForward(3000);
                return true; //for the active control to see the keypress, return false
            }
            //else if (keyData == Keys.Up)
            // {
            //  gv.debug.w("mainWindow  Key   UP");

            // return true; //for the active control to see the keypress, return false
            //}
            else if (keyData == Keys.Escape)
            {
                // If copy operation is running, cancel it
                if (cbCopyAll.Checked)
                {
                    _cancelCopyOperation = true;
                    return true;
                }

                if (cbEscToCloseApp.Checked)
                    System.Windows.Forms.Application.Exit();
                return true;
            }
            else if (keyData == (Keys.Delete))
            {
                markDeleted();
                if (cbAdvanceOnDelete.Checked)
                    showNextSlide3(gv.nextIdx, 1);
                return true;
            }
            else if (keyData == Keys.Decimal)
            {
                markDeleted();
                if (cbAdvanceOnDelete.Checked)
                    showNextSlide3(gv.nextIdx, 1);
                return true;
            }  //  can also mark delete = false

            //
            // Note: if bCopyMode (set with checkbox copy)
            //  keystrokes  A-Z and 0-9 will copy and file the image under subfolder with keyname 
            //  C:\\aImages\a\imagexx.jpg
            //  C:\\aImages\b\imagexx.jpg
            //
            else if (cbCatalogImages.Checked) ////////////////// Main.cs  COPY IMAGE FILE  //////// ------------- CATALOG MODE ------------------------- a-z 0-9 
            {
                if ((keyData >= Keys.A && keyData <= Keys.Z) || (keyData >= Keys.D0 && keyData <= Keys.D9)) //update rating or //COPY image to C:\\aImages\x  folder //////
                {
                    char cData;
                    //this.Text = cData.ToString();
                    if (keyData >= Keys.D0 && keyData <= Keys.D9)
                    {
                        cData = Convert.ToChar(msg.WParam.ToInt32()); ///////////////// convert 0-9 
                    }
                    else
                    {
                        string s = keyData.ToString();
                        cData = s[0];
                    }
                    if (true)
                    {
                        if (rbMove.Checked) //dmc2022 fix this  deleteInsteadOfMove
                        {
                            gv.imageFileList1.updateRating(cData, gv.nextIdx);
                            updateLvFinfo(gv.imageFileList1.getIndexed(gv.nextIdx)); //catalog move
                        }
                        if (rbCopy.Checked || rbMove.Checked)
                        {
                            bool rc = copyOrMoveImage(cData);
                            if (bMoveImages && rc)
                            {
                                gv.imageFileList1.updateDelete(true, gv.nextIdx);
                                updateLvFinfo(gv.imageFileList1.getIndexed(gv.nextIdx)); //catalog update del
                            }
                        }
                        this.Focus();
                        this.Activate();
                    }
                    else // DO ACTUAL MOVE OF FILE -------------------------------2022
                    {
                        if (rbCopy.Checked) //dmc2022   deleteInsteadOfMove
                        {
                            gv.imageFileList1.updateRating(cData, gv.nextIdx);
                            updateLvFinfo(gv.imageFileList1.getIndexed(gv.nextIdx)); //catalog move
                            bool rc = copyOrMoveImage(cData);
                        }
                        if (rbMove.Checked)
                        {
                            ff.MoveFile(finfo1.fpath, gv.dirDeletedFolder);
                        }
                    }
                    if (bAutoAdvance)
                        showNextSlideNow();
                    return true;
                }
                else if (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9) ////////////// keypad numbers 
                {
                    if (rbCopy.Checked)
                    {
                        bool rc = copyOrMoveImage((char)keyData);
                        if (bMoveImages && rc)
                        {
                            gv.imageFileList1.updateDelete(true, gv.nextIdx);
                            updateLvFinfo(gv.imageFileList1.getIndexed(gv.nextIdx)); //keydata move
                        }
                        showNextSlideNow();
                    }
                    else
                    {
                        if (gv.bSlideShow)
                        {
                            int i;
                            i = (int)keyData;
                            initTimer(i);
                        }
                    }
                    return true;

                }
                else if (keyData == Keys.Space)
                {
                    showNextSlide3(gv.nextIdx, 1);
                }
            }
            else //// !bCopyMode////////////////// rate image 
            {
                string txt = keyData.ToString();
                int num = (int)msg.WParam - 48;
                string txt2 = num.ToString();
                gv.debug.w("key ", txt2);
                gv.debug.w(keyData.ToString());
            }
            return false; // base.ProcessCmdKey(ref msg, keyData);
        }

        private void updateRating(char rating)
        {
            gv.imageFileList1.updateRating(rating, gv.nextIdx);
            finfo1 = gv.imageFileList1.getIndexed(gv.nextIdx);
            updateLvFinfo(finfo1); //rating
        }
        private void ListArray_Click(object sender, EventArgs e)
        {
            listFinfo();
        }





        /// <summary>
        /// GROUP BOX for RADIO BUTTONS for image preview resizing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbResize_CheckedChanged(object sender, EventArgs e)
        {
            // resize checked or unchecked
            resizeImage();
        }

        private void rbCenter_CheckedChanged(object sender, EventArgs e)
        {
            //center
            resizeImage();

        }

        private void rbActual_CheckedChanged(object sender, EventArgs e)
        {
            //actual
            resizeImage();
        }


        //setDisplay1Mode(PictureBoxSizeMode.AutoSize); --- this would resize the PB1 itself to the size of the image -- not useful here



        int startImageIdx = 0;
        bool bPreviewSet = false;

        int previewNumberOfRows = 4;
        public void displayPreviewSet(int numRowsStart = 4)
        {
            previewNumberOfRows = numRowsStart;
            if (displayPreviewSetForm == null || displayPreviewSetForm.IsDisposed)
                displayPreviewSetForm = new DisplayPreviewSet(gv, gv.nextIdx, previewNumberOfRows);
            else
                displayPreviewSetForm.Activate();
            // ds.WindowState = FormWindowState.Normal;
            //  ds.Location = new Point(gv.screen[1].Bounds.Location.X, gv.screen[1].Bounds.Location.Y);
        }
        private void buttonDisplaySet_Click(object sender, EventArgs e)
        {
            if (!bShowImagesEnabled)
                DisplayImages();
            cbSlideShow.Checked = false;
            stopSlideShow();
            displayPreviewSet((int)numNumberOfRowsInPreview.Value);
        }
        public void ResizePreviewSet()
        {
            if (displayPreviewSetForm == null || displayPreviewSetForm.IsDisposed)
                return;
            displayPreviewSetForm.ResizeDisplaySet(true);
        }
        public void syncPreviewDisplay()
        {
            if (displayPreviewSetForm != null)
            {
                if (displayPreviewSetForm.Visible)
                {
                    displayPreviewSetForm.syncImageIdx();
                }
            }
        }
        // Returns the index in gv.screen[] whose DeviceName ends with the given GDI index.
        public Screen GetScreenByGdiIndex(int gdiIndex)
        {
            foreach (var s in gv.screen)
            {
                if (s == null) continue;
                var m = System.Text.RegularExpressions.Regex.Match(s.DeviceName ?? "", @"\d+$");
                if (m.Success && int.Parse(m.Value) == gdiIndex)
                    return s;
            }
            return null;
        }
        /// Returns the index in gv.screen[] whose DeviceName ends with the given GDI index.
        /// e.g. argument 2 matches "\\.\DISPLAY2"
        /// Returns -1 if not found.
        /// </summary>
        public int GetScreenIndexByGdiIndex(int gdiIndex)
        {
            for (int i = 0; i < gv.screen.Length; i++)
            {
                var s = gv.screen[i];
                if (s == null) continue;
                var m = System.Text.RegularExpressions.Regex.Match(s.DeviceName ?? "", @"\d+$");
                if (m.Success && int.Parse(m.Value) == gdiIndex)
                    return i;
            }
            return -1;
        }
        public void closingThisDisplay(int dnumber)
        {
            switch (dnumber)
            {
                case 1:
                    cbDisplay1.Checked = false;
                    break;
                case 2:
                    cbDisplay2.Checked = false;
                    break;

                case 3:
                    cbDisplay3.Checked = false;
                    break;
            }
        }

        private void cbDisplay1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplay1.Checked)
            {
                bScreen1 = true;
                var screen = GetScreenByGdiIndex(1);
                if (display1 == null || display1.IsDisposed)
                    display1 = new Display1(gv);
                display1.setDisplayMonitor(screen, 1);
                display1.Show();
                display1.Location = screen.Bounds.Location;   // force after Show()
                display1.displayThisImage(bmp);
            }
            else
            {
                bScreen1 = false;
                display1.Dispose();
            }
        }

        private void cbDisplay2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplay2.Checked)
            {
                if (bAllowScreen2)
                {
                    bScreen2 = true;
                    var screen = GetScreenByGdiIndex(2);
                    if (display2 == null || display2.IsDisposed)
                        display2 = new Display1(gv);
                    display2.setDisplayMonitor(screen, 2);
                    display2.Show();
                    display2.Location = screen.Bounds.Location;   // force after Show()
                }
                else
                    cbDisplay2.Enabled = false;
                display2.displayThisImage(bmp);
            }
            else
            {
                bScreen2 = false;
                display2.Dispose();
            }
        }

        private void cbDisplay3_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplay3.Checked)
            {
                if (bAllowScreen3)
                {
                    bScreen3 = true;
                    var screen = GetScreenByGdiIndex(3);
                    if (display3 == null || display3.IsDisposed)
                        display3 = new Display1(gv);
                    display3.setDisplayMonitor(screen, 3);
                    display3.Show();
                    //     display3.Location = screen.Bounds.Location;   // force after Show()
                }
                else
                    cbDisplay3.Enabled = false;
                display3.displayThisImage(bmp);
            }
            else
            {
                bScreen3 = false;
                display3.Dispose();
            }
        }

        private void cbDisplay4_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplay4.Checked)
            {
                if (bAllowScreen4)
                {
                    bScreen4 = true;
                    var screen = GetScreenByGdiIndex(4);
                    if (display4 == null || display4.IsDisposed)
                        display4 = new Display1(gv);
                    display4.setDisplayMonitor(screen, 4);
                    display4.Show();
                    display4.Location = screen.Bounds.Location;   // force after Show()
                }
                else
                    cbDisplay4.Enabled = false;
                display4.displayThisImage(bmp);
            }
            else
            {
                bScreen4 = false;
                display4.Dispose();
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            gv.debug.w("Main_KeyDown", e.KeyValue);
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            string filename = @"C:\dmc\test.bmp";
            int width = 640;
            int height = 480;
            Bitmap bitm1 = new Bitmap(width, height);

            // Draw myBitmap to the screen.
            //e.Graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width,
            //    myBitmap.Height);

            for (int i = 0; i < width; i++)
            {
                int y = (int)((Math.Sin((double)i * 2.0 * Math.PI / width) + 1.0) * (height - 1) / 2.0);
                bitm1.SetPixel(i, y, Color.Black);
            }
            bitm1.Save(filename);

            this.pbNext.Image = System.Drawing.Image.FromFile(filename);
            pbNext.Refresh();

            /*for use with Windows Forms, and it requires PaintEventArgs e, which is a parameter of the Paint event handler. The code performs the following actions
             * // Draw myBitmap to the screen again.
                e.Graphics.DrawImage(myBitmap, myBitmap.Width, 0,
                myBitmap.Width, myBitmap.Height);
}
*/

        }
        Bitmap bgraph;
        int idx = 0;
        int width = 640;
        int height = 480;

        private void drawPoint()
        {
            // Draw myBitmap to the screen.
            //e.Graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width,
            //    myBitmap.Height);
            int y = (int)((Math.Sin((double)idx * 2.0 * Math.PI / width) + 1.0) * (height - 1) / 2.0);
            bgraph.SetPixel(idx, y, Color.Black);
            ++idx;
            if (idx >= width)
                idx = 0;

        }
        private void initGraph()
        {
            int width = 640;
            int height = 480;
            bgraph = new Bitmap(width, height);
            pbNext.Image = bgraph;
            idx = 0;
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            if (idx == 0)
            {
                initGraph();
            }
            drawPoint();
            drawPoint();
            drawPoint();
            drawPoint();
            drawPoint();
            pbNext.Refresh();
        }

        public void btShowDecription(object sender, EventArgs e)
        {
            cbShowDisplayNames.Checked = false;
            lvScreenInfo.SendToBack();
            parms = new DialogParms(gv);
            parms.Visible = true;
            parms.Activate();
        }
        public void HideLoadingParms()
        {
            btLoadingParameters.Visible = false;
            btLoadingParameters.SendToBack();
        }
        DialogParms parms;
        private void btShowParms_Click(object sender, EventArgs e)
        {
            btLoadingParameters.Visible = true;
            btLoadingParameters.BringToFront();
            this.Refresh();
            OpenDialogParms();
        }
        public void OpenDialogParms()
        {
            cbShowDisplayNames.Checked = false;
            lvScreenInfo.SendToBack();
            parms = new DialogParms(gv);
            parms.Visible = true;
            parms.Activate();
        }
        DisplayParms displayParms;
        public void OpenDisplayParms()
        {
            cbShowDisplayNames.Checked = false;
            lvScreenInfo.SendToBack();
            displayParms = new DisplayParms(gv);
            displayParms.Visible = true;
            displayParms.Activate();
        }
        bool bAutoAdvance = false;

        private void cbCopyOnly_CheckedChanged_1(object sender, EventArgs e)
        {
            bAutoAdvance = cbAutoAdvance.Checked;
        }

        bool bTarotWidth = true;

        private void cbPreviewWidth_CheckedChanged(object sender, EventArgs e)
        {
            bTarotWidth = this.cbTarotWidth.Checked;
        }
        private void bDeleteMarked_Click(object sender, EventArgs e)
        {
            ResetPictureBoxes();
            ClearAllPictureBoxesInUse();
            deleteMarkedImages();
            showFirstSlide();
            countMarkedForDeletion = 0;
            tbMarkedForDeletion.Text = countMarkedForDeletion.ToString();
        }

        private void listFinfo()
        {
            FileInfoItem ii;

            for (int idx = 0; idx < gv.slideCount1; ++idx)
            {
                ii = gv.imageFileList1.getIndexed(idx);
                gv.debug.w("IINFO", idx.ToString(), ii.fpath, ii.bDelete.ToString());

                if (idx == 1)
                    ii.bDelete = true;
            }
        }
        public int countMarkedForDeletion = 0;
        public void markForDeletion(bool delete = true)
        {
            btDeleteMarked.BackColor = Color.OrangeRed;
            if (delete)
                finfo1.bDelete = true;
            else
                finfo1.bDelete = false;
            updateLvFinfo(finfo1); //mark del
            ++countMarkedForDeletion;
            tbMarkedForDeletion.Text = countMarkedForDeletion.ToString();
        }

        public int MarkForDeletionThisId(int index)
        {
            int rcIDX = getImageInfoByIndex(index, 0, false); //bool getPreview .. so false = this is a main display not a preview
            if (rcIDX < 0 || rcIDX >= gv.imageFileList1.getImageCount())
                return rcIDX;
            FileInfoItem fi;
            fi = gv.imageFileList1.getIndexed(rcIDX);
            if (fi.fpath == null)
            {
                return 0;
            }
            fi.bDelete = true;
            return 1;
        }
        public void InsertSourceComment(string src)
        {
            finfo1.comment = src;
            updateLvFinfo(finfo1); //comment
        }
        private void btMarkForDeletion_Click(object sender, EventArgs e)
        {
            MarkForDeletion();
        }
        public void MarkForDeletion()
        {
            markDeleted();
            if (cbAdvanceOnDelete.Checked)
                showNextSlide3(gv.nextIdx, 1);
        }
        public void MarkNextForDeletion()
        {
            markNextForDeletion2();
            if (cbAdvanceOnDelete.Checked)
                showNextSlide3(gv.nextIdx, 1);
        }
        public void MarkNextInvalid()
        {
            markNextInvalidImage();
            showNextSlide3(gv.nextIdx, 1);
        }

        int batchCountMarkedForDeletion = 0;
        public void markNextForDeletion2(bool delete = true)
        {
            btDeleteMarked.BackColor = Color.OrangeRed;
            // gv.nextIdxxxxx
            iinfoNextImage.bDelete = true;
            // updateLvFinfo(finfo1); //mark del
            ++countMarkedForDeletion;
            ++batchCountMarkedForDeletion;
            tbMarkedForDeletion.Text = countMarkedForDeletion.ToString();
            if (batchCountMarkedForDeletion > 999)
            {
                batchCountMarkedForDeletion = 0;

                // Pause to let display catch up without blocking
                tbMarkedForDeletion.Refresh();
                Application.DoEvents();

                // Optional: add a brief delay if needed
                System.Threading.Thread.Sleep(4000); // 1 second pause
                Application.DoEvents(); // Process UI updates again

                DialogResult result = System.Windows.Forms.MessageBox.Show($"delete count {countMarkedForDeletion}you want to continue (YES) 1 or stop (NO)?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                if (result == DialogResult.No)
                {
                    StopAll();
                }
            }
        }
        public void markNextInvalidImage(bool delete = true)
        {
            iinfoNextImage.bInvalid = true;
            updateLvFinfo(finfo1); //mark Invalid
        }
        public bool markDeleted(bool bSet = true)
        {
            btDeleteMarked.BackColor = Color.Red;
            //gv.imageFileList.updateDelete(bSet, gv.nextIdx, gv); //mark for deletion
            //this.Text = gv.nextIdx.ToString();
            // gv.debug.w("MARKED TO DELETE", gv.nextIdx.ToString(), gv.imageFileList.getIndexed(gv.nextIdx).fpath);
            //string fp = gv.imageFileList.getIndexed(gv.nextIdx).fpath;
            //finfoItem.bDelete = bSet;
            //updateLvFinfo(finfoItem);

            //lvFinfo.Items[9].SubItems[1].Text = bSet.ToString();
            sound(30);
            //gv.imageFileList
            btDeleteMarked.Enabled = true;
            markForDeletion(bSet);
            dgvFileInfo.Refresh();
            return bSet;

        }
        public string[] soundFiles = null;
        // @"Data Source=C:\Users\david\Documents\dzenSDB\SDB01.sdf;Persist Security Info=True";


        static int idxSound = 0;

        private void sound(int idxSound)
        {
            // player = new System.Media.SoundPlayer(gv.main.soundFiles[idx]);
            soundLocation = soundFiles[idxSound];
            //tbSoundFile.Text = soundFiles[idxSound];
            SoundLevel(gv.soundVolume, soundLocation);
        }

        bool bOnlyMoveDeleted = false; //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< delete or move 

        private void deleteMarkedImages()
        {

            int row = 0;
            for (int idx = 0; idx < gv.slideCount1; ++idx)
            {
                finfo1 = gv.imageFileList1.getIndexed(idx);
                fpathCurrentImage = gv.imageFileList1.getIndexed(idx).fpath;

                if (finfo1.bDelete)
                {
                    gv.debug.w("DELETE  ENABLED .... fname: " + fpathCurrentImage);
                    // pb1.Image.Dispose();
                    if (bOnlyMoveDeleted)
                    {
                        if (!ff.directoryExists(gv.dirDeletedFolder))
                        {
                            ff.createDirectory(gv.dirDeletedFolder);
                        }
                        ff.MoveFile(finfo1.fpath, gv.dirDeletedFolder);
                    }
                    else
                    {
                        bool flag = ff.FileDelete(finfo1.fpath);
                        if (flag)
                            this.setTitle(finfo1, "deleted");
                    }
                    gv.imageFileList1.updatePath(null, idx);

                }
            }
            //remove from collection
            bool bRemoveDeleted = true;
            int rc = 0;
            if (bRemoveDeleted)
                for (int idx = gv.slideCount1 - 1; idx >= 0; --idx)
                {
                    finfo1 = gv.imageFileList1.getIndexed(idx);
                    fpathCurrentImage = gv.imageFileList1.getIndexed(idx).fpath;
                    if (finfo1.bDelete && string.IsNullOrEmpty(fpathCurrentImage))
                    {
                        gv.debug.w("------ before attempt to remove entry:", idx.ToString(), " total entries:", gv.slideCount1.ToString());
                        rc = gv.imageFileList1.removeItemAt(idx); ///////////addItem((string)key, (string)all[key], level)
                        gv.debug.w("------ removed Item entry for deleted file gv.ImageFileList idx  and total now", idx.ToString(), rc.ToString());
                    }
                }
            gv.slideCount1 = gv.imageFileList1.getImageFileListLength();
            tbMaxSlideNumber.Text = $"{gv.slideCount1 - 1}";
            //next decrement position
            //this.Text = this.Text + " DELETED";
            //removeSlide();
            //showNextSlide(-2);
        }

        //  IHTMLDocument2 doc;
        //  IHTMLControlRange imgRange;
        // Add this field to keep a modeless instance alive
        // Keep a single modeless instance alive



        private void btSaveInitParms_Click(object sender, EventArgs e)
        {
            //ff.saveInitParmsList();
            this.Text = gv.initParm1List[0].mostRecent;
            ff.saveInitParmsListToDefault(gv.initParm1List);
            SaveHistoryList();
            System.Windows.Forms.Application.Exit();
        }

        // ShowDisplay[] win = new ShowDisplay[4];
        public void ShowMonitors(bool bShow)
        {
            if (gv.screen.Length < 2)
            {
                gv.displayCount = 1;
                btCompareLoad.Text = "1";
                if (bShow)
                    System.Windows.Forms.MessageBox.Show($"single display {gv.screen[0].Bounds.Width} {gv.screen[0].Bounds.Height} Please note this application is designed for a system with 2 or more displays ");
                else
                    cbShowDisplayNames.Checked = false;
                return;
            }
            if (gv.screen[1] == null)
            {
                gv.displayCount = 1;
                btCompareLoad.Text = "1";
                if (bShow)
                    System.Windows.Forms.MessageBox.Show($"single display {gv.screen[0].Bounds.Width} {gv.screen[0].Bounds.Height} Please note this application is designed for a system with 2 or more displays ");
                else
                    cbShowDisplayNames.Checked = false;
                return;
            }

            System.Drawing.Rectangle recDisplay1 = gv.screen[1].Bounds;
            System.Drawing.Point p = new System.Drawing.Point(recDisplay1.X, recDisplay1.Y);
            int winnumber = 0;
            //int idx = winnumber - 1;
            for (int idx = 0; idx < gv.displayCount; ++idx)
            {

                if (bShow)
                {
                    winnumber = idx + 1;
                    int displayNumber = GetWindowsDisplayNumber(gv.screen[idx]);
                    win[idx] = new ShowDisplay(displayNumber, gv.screen[idx].Bounds.Width, gv.screen[idx].Bounds.Height);

                    // win[idx].Text = " WINDOW " + winnumber.ToString();
                    // win[idx].setText(winnumber.ToString());

                    gv.debug.w(gv.screen[idx].WorkingArea.ToString());
                    // NOTE HAD TO SET FORM default START POSITION to manual
                    win[idx].Location = new System.Drawing.Point(gv.screen[idx].Bounds.X, gv.screen[idx].Bounds.Y);
                    win[idx].YourLocation(win[idx].Location);
                    //win.DesktopLocation = new Point(gv.screen[idx].Bounds.X, gv.screen[idx].Bounds.Y);
                    win[idx].Show();
                    win[idx].Activate();
                }
                else
                    win[idx].Dispose();
            }


        }
        ShowDisplay[] win = new ShowDisplay[4];
        /// <summary>
        /// Returns the Windows Display Settings number for a screen, matching
        /// what Windows shows in System > Display and the Identify button.
        /// Primary is always 1; non-primary are numbered 2+ by GDI device order.
        /// </summary>
        private static int GetWindowsDisplayNumber(Screen s)
        {
            if (s.Primary) return 1;

            static int GdiIndex(Screen scr)
            {
                var m = System.Text.RegularExpressions.Regex.Match(scr.DeviceName ?? "", @"\d+$");
                return m.Success ? int.Parse(m.Value) : 0;
            }

            var nonPrimary = Screen.AllScreens
                .Where(x => !x.Primary)
                .OrderBy(x => GdiIndex(x))
                .ToList();

            int idx = nonPrimary.FindIndex(x => x.DeviceName == s.DeviceName);
            return idx + 2;
        }

        private static Screen GetScreenByWindowsNumber(int winDisplayNum)
        {
            return Screen.AllScreens
                .FirstOrDefault(s => GetWindowsDisplayNumber(s) == winDisplayNum);
        }


        private void cbCatalogImages_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbRename.Checked)
                rbCopy.Checked = true;
            if (cbCatalogImages.Checked)
                cbAutoAdvance.Checked = true;
            setCopyMode(cbCatalogImages.Enabled);
            /*
            if (display1 != null)
            if (display1.IsAccessible)
                display1.setCopyMode(cbCopyToFolder.Enabled);

            if (display2 != null)
                if (display2.IsAccessible)
                display2.setCopyMode(cbCopyToFolder.Enabled);

            if (display3 != null)
                if (display3.IsAccessible)
                display3.setCopyMode(cbCopyToFolder.Enabled);
             */

        }

        private void pb1_Resize(object sender, EventArgs e)
        {
        }

        public void setDisplay1Mode(PictureBoxSizeMode sm)
        {
            if (display1 != null)
                display1.setPictureBoxSize(sm);
            if (display2 != null)
                display2.setPictureBoxSize(sm);
            if (display3 != null)
                display3.setPictureBoxSize(sm);
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            btSizeWindow.Text = $"{this.Bounds.Width} {this.Bounds.Height}";
            int margin = 10;


            // Apply only if height is reasonable
            //  if (newHeight > 50)
            // dgvFileInfo.Height = newHeight;
        }

        private void tbTargetFolder_Enter(object sender, EventArgs e)
        {
            tbTargetFolder.Text = validateDirectory(tbTargetFolder.Text);
        }

        private void btTagFile_Click_1(object sender, EventArgs e)
        {
            finfo1.rating = '@';
            finfo1.bInvalid = true;
            //finfoItem.bDelete = true;
            //gv.imageFileList.markRating("@", gv.nextIdx);
            updateLvFinfo(finfo1); //tag

        }

        private void btUntag_Click(object sender, EventArgs e)
        {
            finfo1.rating = 'g';
            finfo1.bInvalid = false;
            //gv.imageFileList.markRating("g", gv.nextIdx);
            updateLvFinfo(finfo1); //untag
        }

        private void dbCatalogImages_CheckedChanged(object sender, EventArgs e)
        {
            // bCatalogImagesMode =  cbCatalogImages.Checked;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public AllDisplaysInfoForm _allDisplaysInfoForm;
        public void ShowDisplayNames(bool bShow)
        {
            if (!bShow)
            {
                bShowDisplays = false;
                try { ShowMonitors(false); } catch { }

                _allDisplaysInfoForm?.Close();
                //_allDisplaysInfoForm = null;

                lvScreenInfo.SendToBack();
            }
            else
            {
                bShowDisplays = true;
                DisplayAllScreensInfo();
                try { ShowMonitors(true); } catch { }



                lvScreenInfo.BringToFront();
            }
        }
        private void cbShowDisplayNames_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbShowDisplayNames.Checked)
            {
                bShowDisplays = false;
                try { ShowMonitors(false); } catch { }

                _allDisplaysInfoForm?.Close();
                //_allDisplaysInfoForm = null;

                lvScreenInfo.SendToBack();
            }
            else
            {
                bShowDisplays = true;
                DisplayAllScreensInfo();
                try { ShowMonitors(true); } catch { }



                lvScreenInfo.BringToFront();
            }
        }
        public void DisplayAllScreensInfo()
        {
            if (_allDisplaysInfoForm == null || _allDisplaysInfoForm.IsDisposed)
            {
                _allDisplaysInfoForm = new AllDisplaysInfoForm(gv);
                _allDisplaysInfoForm.PopulateScreens();
                _allDisplaysInfoForm.Show(this);
            }
            else
            {
                _allDisplaysInfoForm.BringToFront();
            }
        }
        private void cbShowDisplayNames_CheckedChangedxx(object sender, EventArgs e)
        {
            if (!cbShowDisplayNames.Checked)
            {
                bShowDisplays = false;
                try
                {
                    ShowMonitors(false);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Windows Display settings is incorrect .. show a ghost monitor");
                }
                lvScreenInfo.SendToBack();
            }
            else // display 
            {
                bShowDisplays = true;
                try
                {
                    ShowMonitors(true);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Windows Display settings is incorrect");
                }
                lvScreenInfo.BringToFront();
            }
        }
        /// <summary>
        /// Loads gv.folderHistoryList from gv.folderHistoryFileFullPathName.
        /// Uses the same plain XmlSerializer(typeof(List<FolderHistory>)) that
        /// SaveHistoryList() uses — no custom XmlRootAttribute — so the root
        /// element "<ArrayOfFolderHistory>" round-trips correctly.
        /// </summary>
        public int LoadHistoryList()
        {
            if (gv.folderHistoryList == null)
                gv.folderHistoryList = new List<FolderHistory>();

            if (string.IsNullOrEmpty(gv.folderHistoryFileFullPathName))
                gv.folderHistoryFileFullPathName = Path.Combine(gv.dataFolder, gv.historyFileNameIs);

            if (!File.Exists(gv.folderHistoryFileFullPathName))
            {
                MessageBox.Show($"NO History File {gv.folderHistoryFileFullPathName}");

                bool folderExists = ff.directoryExists(gv.dataFolder);
                if (!folderExists)
                    ff.createDirectory(gv.dataFolder);

                if (!File.Exists(gv.folderHistoryFileFullPathName))
                {
                    MessageBox.Show($"NO History File {gv.folderHistoryFileFullPathName}");
                    return 0;
                }
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<FolderHistory>));
                using (TextReader reader = new StreamReader(gv.folderHistoryFileFullPathName))
                {
                    gv.folderHistoryList = (List<FolderHistory>)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                gv.debug.w("LoadHistoryList Deserialize Exception", ex.ToString(), ex.Message);
                gv.folderHistoryList = new List<FolderHistory>();
                return 0;
            }

            PathHelpers.NormalizeFolderHistoryPaths(gv);

            return gv.folderHistoryList.Count;
        }

        SaveFileDialog saveFileDialog1 = new SaveFileDialog();

        public void SaveHistoryList()
        {
            TextWriter textWriter;
            if (true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<FolderHistory>));
                try
                {
                    textWriter = new StreamWriter(gv.folderHistoryFileFullPathName);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("gv.folderHistoryFileFullPathName", "Can not access");
                    return;
                }
                try
                {
                    serializer.Serialize(textWriter, gv.folderHistoryList);
                }
                catch (Exception ex)
                {

                    gv.debug.w("Serilize Exception ", ex.ToString(), ex.Message);
                }
                textWriter.Close();

            }
        }
        private static readonly XmlSerializer _folderHistorySerializer =
            new XmlSerializer(typeof(List<FolderHistory>), new XmlRootAttribute("TargetHistory"));

        List<FolderHistory> DeserializeFromXML2xxxxxxx(string fpath)
        {
            XmlSerializer deserializer = _folderHistorySerializer;
            TextReader textReader = new StreamReader(fpath);
            List<FolderHistory> folderHistory;
            folderHistory = (List<FolderHistory>)deserializer.Deserialize(textReader);
            textReader.Close();

            return folderHistory;
        }
        List<FolderHistory> DeserializeFromXML2(string fpath)
        {
            XmlSerializer deserializer = _folderHistorySerializer;
            try
            {
                using (TextReader textReader = new StreamReader(fpath))
                {
                    return (List<FolderHistory>)deserializer.Deserialize(textReader);
                }
            }
            catch (InvalidOperationException)
            {
                // File is corrupt or incompatible format — start fresh
                File.Delete(fpath);
                return new List<FolderHistory>();
            }
        }
        List<FolderAssignment> DeserializeFolderAssignmentFromXML(string fpath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<FolderAssignment>));
            TextReader textReader = new StreamReader(fpath);
            List<FolderAssignment> list;
            list = (List<FolderAssignment>)deserializer.Deserialize(textReader);
            textReader.Close();

            return list;
        }
        public int LoadFolderHistoryList()
        {
            if (true)
            {
                if (gv.folderHistoryList == null)
                    gv.folderHistoryList = new List<FolderHistory>();

                // gv.folderHistoryFileFullPathName = ff.NormalizeFileFullName(gv.dazenFolderName);
                // gv.folderHistoryFileFullPathName = gv.default_init_Path + "\\TargetHistory.xml";

                if (gv.folderHistoryFileFullPathName == null)
                {
                    gv.folderHistoryFileFullPathName = Path.Combine(gv.dataFolder, gv.historyFileNameIs); // "TargetHistory.xml");
                }
                bool brc = System.IO.File.Exists(gv.folderHistoryFileFullPathName);
                bool bCreateFolder = false;
                if (!brc)
                {
                    System.Windows.Forms.MessageBox.Show($"NO History File {gv.folderHistoryFileFullPathName}");
                    bCreateFolder = true;

                    brc = ff.directoryExists(gv.dataFolder);
                    if (!brc)
                        ff.createDirectory(gv.dataFolder);
                    if (!brc)
                    {
                        System.Windows.Forms.MessageBox.Show($"Create the Folder {gv.dataFolder} and rerun the SDB4");
                        return 0;
                    }
                    if (bCreateFolder)
                        System.Windows.Forms.MessageBox.Show($"SDB4 Created folder {gv.dataFolder}");
                    //SaveHistoryList();
                    brc = System.IO.File.Exists(gv.folderHistoryFileFullPathName);
                    if (!brc)
                    {
                        System.Windows.Forms.MessageBox.Show($"NO History File {gv.folderHistoryFileFullPathName}");
                        return 0;
                    }
                }
                //gv.folderHistoryList = DeserializeFromXML2(gv.folderHistoryFileFullPathName);
                LoadHistoryList();
                PathHelpers.NormalizeFolderHistoryPaths(gv);

                return gv.folderHistoryList.Count;
            }

        }


        // -------- INIT PARMS now in XML file format -----------------------------------
        public void InitializeSaveFileDialogXML()
        {
            if (this.saveFileDialog1 == null)
                this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            // Set the file dialog to filter for graphics files.
            this.saveFileDialog1.Filter =
                "XML Files (*.XML)|*.XML";

            //this.saveFileDialog1.Multiselect = false;// .SupportMultiDottedExtensions = true; //.Multiselect = true;
            this.saveFileDialog1.Title = "Save INIT PARMS List File as (.XML)";

        }





        public void InitializeOpenFileDialogXML()
        {
            if (this.openFileDialog1 == null)
                this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "XML Files (*.XML)|*.XML";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = false;// .SupportMultiDottedExtensions = true; //.Multiselect = true;
            this.openFileDialog1.Title = "Select SDB Init Parms File (.XML)";

        }



        // ------------ end of INIT PARMS in XML file

        private void btFileCreate_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fpath = System.IO.Path.ChangeExtension(saveFileDialog1.FileName, "xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoItem>));
                TextWriter textWriter = new StreamWriter(fpath);
                try
                {
                    serializer.Serialize(textWriter, gv.imageFileList1.getFinfo());
                }
                catch (Exception ex)
                {

                    gv.debug.w("Serilize Exception ", ex.ToString(), ex.Message);
                }
                textWriter.Close();

            }
        }
        private void SaveDeleteFileToXML()
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fpath = System.IO.Path.ChangeExtension(saveFileDialog1.FileName, "xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoItem>));
                TextWriter textWriter = new StreamWriter(fpath);
                try
                {
                    serializer.Serialize(textWriter, GetDeleteList().getFinfo());
                }
                catch (Exception ex)
                {

                    gv.debug.w("Serilize Exception ", ex.ToString(), ex.Message);
                }
                textWriter.Close();

            }
        }




        List<FileInfoItem> DeserializeImageFileInfoListFromXML(string fpath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<FileInfoItem>));
            TextReader textReader = new StreamReader(fpath);
            List<FileInfoItem> imageInfoList;
            imageInfoList = (List<FileInfoItem>)deserializer.Deserialize(textReader);
            textReader.Close();

            return imageInfoList;
        }


        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void btSaveURLs_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<URLinfoList>));
                TextWriter textWriter = new StreamWriter(saveFileDialog1.FileName);
                try
                {
                    serializer.Serialize(textWriter, gv.urlList.getURLinfo());                  //
                    //gv.imageFileList.getFinfo());
                }
                catch (Exception ex)
                {

                    gv.debug.w("Serilize Exception URL info", ex.ToString(), ex.Message);
                }
                textWriter.Close();

            }

        }

        private void tbTargetFolder_DoubleClick(object sender, EventArgs e)
        {
            // open (gv.directoryPromptDialog)
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            DialogFolderSelection getFolder = new DialogFolderSelection(gv, null);
            getFolder.Show();
            getFolder.Activate();
        }

        public void setTargetFolder(string dir)
        {
            tbTargetFolder.Text = dir;

            gv.initParm1List[0].SetTargetDir1(dir);
        }
        public string GetTargetFolder()
        {
            return tbTargetFolder.Text;
        }
        public void message(string txt)
        {
            tbMessage.Text = txt;
            //sound(55);
        }
        private void btPromptFolder(object sender, EventArgs e)
        {
            DialogFolderSelection getFolder = new DialogFolderSelection(gv, tbTargetFolder);
            getFolder.Show();
            getFolder.Activate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (timer.Enabled)
                initTimer(numSlideShowSpeed.Value);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pb1.Image = null;
            pb1.Refresh();
            System.Drawing.Point loc = getLocation();
            loc.X += 100;
            loc.Y += 100;
            gv.debug.Location = loc;
            gv.debug.Show();
        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            if (e != null)
                return;
            for (int idx = 0; idx < gv.slideCount1; ++idx)
            {
                iinfo2 = gv.imageFileList1.getIndexed(idx);
                if (iinfo2.rating.Equals('g'))
                {
                    gv.debug.w("ShowNextSlide2 in Main btSearch_Click");
                    showNextSlideImageFileList1(idx, 1);
                    return;
                }
            }
            System.Windows.Forms.MessageBox.Show("No gv.imageFilelist item with 'g' rating found");
        }
        int counterValidate = 0;
        PromptContinueWindow continueDialog;

        DialogDeleteDup deleteDup;

        public bool DisplayContinuePrompt()
        {
            if (continueDialog == null || continueDialog.IsDisposed)
                continueDialog = new PromptContinueWindow(gv);
            if (continueDialog.ShowDialog(this) == DialogResult.OK)
            {
                return true;
            }
            StopAll();
            stopSlideShow();
            return false;
        }

        public void DisplayAutoDeletePrompt()
        {
            stopSlideShow();
            if (deleteDup == null || deleteDup.IsDisposed)
                deleteDup = new DialogDeleteDup(gv);
            deleteDup.Show();
            deleteDup.Activate();
            deleteDup.Restart();
        }
        public bool delReturnCode = false;

        public void DelReturnCodeContinue(bool flag)
        {
            delReturnCode = flag;
            //deleteDup.Close();
            ResumeDupSearch();
        }
        private void btValidate_Click(object sender, EventArgs e)
        {
            int idx = 0;
            counterValidate = 0;
            gv.debug.w("XXXXXXXXXXXXXXXX  Validate Image File list XXXXXXXXXXXXXX");

            for (idx = 0; idx < gv.slideCount1; ++idx)
            {
                tbSlideNumber.Text = idx.ToString();
                processImageList(idx);
                pb1.Refresh();
                tbSlideNumber.Refresh();
                if (++counterValidate >= 100)
                {
                    if (DisplayContinuePrompt())
                    {
                        continueDialog.Dispose();
                        counterValidate = 0;
                    }
                    else
                    {
                        continueDialog.Dispose();
                        return;
                    }
                }
            }
            gv.debug.w("XXXXXXXXXXXXXXXX  ------end of validation --- XXXXXXXXXXXXXX");
        }



        private void tbFpath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetData(System.Windows.Forms.DataFormats.Text, (Object)tbFpath.Text);
            }
            catch (Exception)
            {

                gv.debug.w("clipboard error tbFpath_TextChanged");
            }
        }
        public void SetNextSlideNumber(int number)
        {
            tbGoToSlide.Text = number.ToString();
        }

        private void tbSlideNumber_Enter(object sender, EventArgs e)
        {
            // GoToSlideNumber();
            // tbMaxSlideNumber.Focus();
        }
        public void GoToSlideNumber()
        {
            int next = 0;
            {
                enteringSlideNumber = false;
                if (string.IsNullOrEmpty(tbGoToSlide.Text))
                    return;
                try
                {
                    next = Convert.ToInt32(tbGoToSlide.Text);
                }
                catch (Exception)
                {

                    gv.debug.w("INVALID SLIDE NUMBER");
                }
                if (next >= gv.slideCount1)
                    next = gv.slideCount1 - 1;
                if (next >= 0)
                    if (next < gv.slideCount1)
                    {
                        gv.debug.w("ShowNextSlide2 in Main ....  tbSlideNumber_KeyUp");
                        showNextSlideImageFileList1(next, 1);
                    }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XML_Editor x = new XML_Editor(gv);
            x.Activate();
            x.Show();
        }

        private void cbMove_CheckedChanged(object sender, EventArgs e)
        {
            bMoveImages = rbMove.Checked;
            btDeleteMarked.Enabled = true;
        }

        int numberState = 0;

        private void btResetNum_Click(object sender, EventArgs e)
        {
            ResetPictureBoxes();
            if (String.IsNullOrEmpty(iinfo2.fpath))
                return;
            DirectoryInfo dinfo = new DirectoryInfo(iinfo2.fpath);

            string folderName = dinfo.Parent.Name;
            btCompareLoad.Text = folderName;
        }

        public int currentNumber = 0;
        private void mtbCount_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            currentNumber = Convert.ToInt32(mtbCount.Text);

        }

        string origFname;
        public Boolean VerifyFolderExists(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                tbTargetFolder.BackColor = Color.LightGreen;
                return true;
            }
            else
            {
                MessageBox.Show($"Folder does not exist {folderPath}", "ERROR TARGET FOLDER");
                tbTargetFolder.BackColor = Color.OrangeRed;
                return false;
            }
        }
        private void tbTargetFolder_TextChanged(object sender, EventArgs e)
        {
            VerifyFolderExists(tbTargetFolder.Text);
        }

        private void btGetTargetPath_Click(object sender, EventArgs e)
        {
            string path = gv.dirDialog(InitFolder.MyComputer);
            path = @"C:\share\aaa";
            this.tbTargetFolder.Text = path;
        }

        public void saveInitParms()
        {
            if (bUseLocalApplicationData)
            {
                SaveInitFile();
            }
            else
                ff.saveInitParmsListToDefault(gv.initParm1List);
        }

        private void btDebug_Click_1(object sender, EventArgs e)
        {
            if (gv.debug == null || gv.debug.IsDisposed)
            {
                gv.debug = new DebugWindow(gv);
            }
            gv.debug.Activate();
            gv.debug.Show();
        }

        private void b_db_Click(object sender, EventArgs e)
        {
            /*
            string sqlServerLocaldb = gv.getDbConnectionString();
            string cs1, cs2;
            // XPS-9000\\DMC_SDB4A
            // .\\DMC_SDB4A
            // Server=.\\SQLEXPRESS  
            SqlConnection conn = new SqlConnection(sqlServerLocaldb);

            try
            {
                conn.Open();
            }
            catch (Exception)
            {

                gv.debug.w("DB FAILED TO CONNECT:", gv.getDbConnectionString());
                return;
            }
            gv.debug.w("DB  CONNECTED:", gv.getDbConnectionString());

            SqlCommand cmd = new SqlCommand("SELECT DealDescription, StartDate FROM [Sales].[SpecialDeals]", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cs1 = reader.GetString(0);
                //cs2 = reader.GetString(1);
                gv.debug.w(cs1);
            }
            reader.Close();
            conn.Close();
            */
        }

        private void cbDisplayInfomation_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplayInfomation.Checked)
                lvScreenInfo.BringToFront();
            else
                lvScreenInfo.SendToBack();
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            clearScreen();
        }
        void clearScreen()
        {
            ResetPictureBoxes();
        }
        private void cbLoadFromStreamPreview_CheckedChanged(object sender, EventArgs e)
        {
            gv.bLoadFromStreamPreview = cbLoadFromStreamPreview.Checked;
        }

        string getProdVersion()
        {
            // TODO Although ClickOnce is supported on .NET 5+, apps do not have access to the System.Deployment.Application namespace. For more details see https://github.com/dotnet/deployment-tools/issues/27 and https://github.com/dotnet/deployment-tools/issues/53.
            /*
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment cd =
                System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                string publishVersion = cd.CurrentVersion.ToString();
                return publishVersion;
            }
            */
            return "unregistered test version";
        }
        private void cbWatch1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWatch1.Checked)
            {
                if (!cbDisplay1.Checked)
                    cbDisplay1.Checked = true;
                display1.startWatcher();
            }


        }
        Watcher fileWatcher = null;

        public void stopWatcher()
        {
            tbMessage.Text = "not watching";
            if (fileWatcher == null)
                return;

            try
            {
                // Try common stop/close/dispose methods (safe reflection fallback,
                // because `Watcher` is a custom type in your codebase).
                var t = fileWatcher.GetType();
                var stopMethod = t.GetMethod("Stop") ??
                                 t.GetMethod("StopWatching") ??
                                 t.GetMethod("Close") ??
                                 t.GetMethod("Dispose");
                stopMethod?.Invoke(fileWatcher, null);

                // If the custom Watcher exposes an inner FileSystemWatcher property/field,
                // attempt to dispose that too to release OS handles immediately.
                var fswProp = t.GetProperty("Watcher") ??
                              t.GetProperty("FileSystemWatcher") ??
                              t.GetProperty("FsWatcher");
                if (fswProp != null)
                {
                    if (fswProp.GetValue(fileWatcher) is IDisposable inner)
                        inner.Dispose();
                }

                // Also attempt to find a field with a FileSystemWatcher instance.
                var fswField = t.GetField("watcher") ??
                               t.GetField("_watcher") ??
                               t.GetField("fileSystemWatcher");
                if (fswField != null)
                {
                    if (fswField.GetValue(fileWatcher) is IDisposable innerField)
                        innerField.Dispose();
                }
            }
            catch (Exception ex)
            {
                // keep app responsive and log the issue to your debug window
                gv.debug.w("stopWatcher error:", ex.Message);
            }
            finally
            {
                // Dispose the Watcher if it implements IDisposable
                try
                {
                    if (fileWatcher is IDisposable d)
                        d.Dispose();
                }
                catch { /* ignore disposal errors */ }

                // remove reference so it can be collected
                fileWatcher = null;

                // optional: release unmanaged resources promptly
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void startWatcher()
        {
            tbMessage.Text = "watching";

            // If already created, attempt to start it (if it exposes a Start method).
            if (fileWatcher != null)
            {
                try
                {
                    var start = fileWatcher.GetType().GetMethod("Start") ??
                                fileWatcher.GetType().GetMethod("StartWatching");
                    if (start != null)
                    {
                        start.Invoke(fileWatcher, null);
                        return;
                    }

                    // If it's already active do nothing.
                    return;
                }
                catch (Exception ex)
                {
                    gv.debug.w("startWatcher error:", ex.Message);
                    // fall through and recreate below
                }
            }

            // Create a new watcher instance
            fileWatcher = new Watcher(gv, null, null);
        }
        // C#
        public Boolean displayThisImage1(string fpath)
        {
            if (this.InvokeRequired)
            {
                // Use BeginInvoke to avoid blocking the caller thread
                var result = (IAsyncResult)this.BeginInvoke(new Func<string, Boolean>(displayThisImage1), fpath);
                return (Boolean)this.EndInvoke(result);
            }

            // == existing UI-thread-only logic below ==
            tbMessage.Text = fpath;
            this.Refresh();
            if (!String.IsNullOrEmpty(fpath))
            {
                string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
                if (fpath == null || !formats.Any(fpath.Contains))
                    return false;
            }
            // Replace the single-call line:
            //     LoadImageFromFinfo(fpath, 1);
            // with the following robust load logic that assigns the image to pb1.Image
            try
            {
                // Dispose previous image safely to avoid file locks and memory leaks.
                var previous = pb1.Image;
                using (var fs = new FileStream(fpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using var img = System.Drawing.Image.FromStream(fs);
                    // Create a copy so the stream can be closed and the file unlocked immediately.
                    pb1.Image = new Bitmap(img);
                }
                // Only dispose the old image if it's different from the newly assigned one.
                if (previous != null && previous != pb1.Image)
                    previous.Dispose();

                // Optional UI updates
                pb1.Refresh();
            }
            catch (Exception ex)
            {
                // keep UI responsive and log failure
                pb1.Image = null;
                gv.debug.w($"Failed to load image '{fpath}': {ex.Message}");
                StopAll();
            }
            return true;
        }
        public Boolean displayThisImage1xxx(string fpath) //// DISPLAY IMAGE after LOADING <<<<<<<<<<<<<<<<<< MAIN CALL TO DISPLAY THIS IMAGE << LOAD AND DISPLAY FPATH image
        {
            //this.Text = fpath;
            tbMessage.Text = fpath;
            this.Refresh();
            if (!String.IsNullOrEmpty(fpath))
            {
                var file = fpath;
                string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
                if (file == null || !formats.Any(fpath.Contains))
                {
                    return false;
                }
            }
            try
            {
                LoadImageFromFinfo(fpath, 1);//openFileDialog1.FileName);
            }
            catch (Exception)
            {
                StopAll();
                pb1.Image = null;
                gv.debug.w("Error File not found ", fpath);
                return false;

            }
            //    resizeZoom();
            return true;
        }
        private void btVerifyTargetFolder_Click(object sender, EventArgs e)
        {
            tbTargetFolder.BackColor = Color.LightGray;
            bool test = ff.directoryExists(tbTargetFolder.Text);
            if (!test)
                System.Windows.Forms.MessageBox.Show("target folder invalid", "ERROR");
            else
            {
                tbTargetFolder.BackColor = Color.LightGreen;
                //gv.initParm1List[0].watchFolder1 = tbTargetFolder.Text;
            }

        }

        private void btStopSlideShow_Click(object sender, EventArgs e)
        {
            stopSlideShow();
        }
        int myScreen = 0;
        public void moveToNextScreen()
        {
            if (cbDisplay1.Checked)
                cbDisplay1.Checked = false;
            if (cbDisplay2.Checked)
                cbDisplay2.Checked = false;
            if (cbDisplay3.Checked)
                cbDisplay3.Checked = false;
            if (cbDisplay4.Checked)
                cbDisplay4.Checked = false;

            ///MessageBox.Show(gv.displayCount.ToString() );
           // gv.debug.w($"{myScreen} of {gv.displayCount}");
            if (gv == null)
                gv.debug.w("{gv is null");
            if (gv.screen == null)
                gv.debug.w("{gv.screen is null");

            if (gv.displayCount > 1)
                ++myScreen;
            if (myScreen >= gv.screen.Length)
                myScreen = 0;
            if (myScreen >= gv.displayCount)
                myScreen = 0;
            if (gv.screen[myScreen] == null)
            {
                gv.debug.w($"gv.screen[{myScreen}] is null");
                return;
            }
            this.Location = new System.Drawing.Point(gv.screen[myScreen].Bounds.X, gv.screen[myScreen].Bounds.Y);

            // Set location
            this.Location = new System.Drawing.Point(gv.screen[myScreen].Bounds.X, gv.screen[myScreen].Bounds.Y);

            // Force form to resize to new screen
            this.WindowState = FormWindowState.Normal;
            this.WindowState = FormWindowState.Maximized;

            // Reposition anchored controls after screen change
            Application.DoEvents(); // Let Windows process the resize
            //RepositionControlsForCurrentScreen();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            moveToNextScreen();
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            moveToNextScreen();
            this.WindowState = FormWindowState.Maximized;
        }

        private void btAddSourceNote_Click(object sender, EventArgs e)
        {
            InsertSourceComment(tbDescription.Text);
        }

        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            Control win = (Control)sender;
            System.Drawing.Size x = this.MaximumSize;

            // if (win.Width != 640 && win.Height != 480)
            lblResizeDims.Text = $"{win.Width} x {win.Height}";
        }

        private void numWatchFolder_ValueChanged(object sender, EventArgs e)
        {
            int nFolder = (int)numWatchFolder.Value;
            switch (nFolder)
            {
                case 0:
                    gv.watchFolderPath = tbWatch.Text;
                    break;
                case 1:
                    if (Directory.Exists(gv.initParm1List[0].watchFolder1))
                        gv.watchFolderPath = gv.initParm1List[0].watchFolder1;
                    break;
                case 2:
                    if (Directory.Exists(gv.initParm1List[0].watchFolder2))
                        gv.watchFolderPath = gv.initParm1List[0].watchFolder2;
                    break;
                case 3:
                    if (Directory.Exists(gv.initParm1List[0].watchFolder3))
                        gv.watchFolderPath = gv.initParm1List[0].watchFolder3;
                    break;

            }
            tbWatch.Text = gv.watchFolderPath;
        }

        private void cbSlideShow_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSlideShow.Checked)
            {
                activateImageButtons();
                btSlideShow.BackColor = Color.LightGreen;
            }
            else
            {
                btSlideShow.BackColor = Color.LightGray;
            }

        }

        private void btTestCopy_Click(object sender, EventArgs e)
        {
            btCopyImageInfo.Visible = false;
        }

        private void btEnterSlideNumber_Click(object sender, EventArgs e)
        {
            enteringSlideNumber = true;
            tbSlideNumber.Focus();
            tbSlideNumber.SelectAll();
        }

        private void Main_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            cbCatalogImages.Checked = false;
            cbCopyAll.Checked = false;
            cbSlideShow.Checked = false;
            stopSlideShow();
        }
        public void hold()
        {
            //   p.X = scrObj.screen.WorkingArea.Left;
            //   p.Y = scrObj.screen.WorkingArea.Top;

            //  this.Location = p;
        }
        private void cbStatusLocation_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStatusLocation.Checked)
            {
                btCopyImageInfo.Location = new System.Drawing.Point(12, 89);
            }
            else
            {
                btCopyImageInfo.Location = new System.Drawing.Point(99, 89);
            }
        }

        private void btResize_Click(object sender, EventArgs e)
        {
            ForceGarbageCollection();
        }
        public void ForceGarbageCollection()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void cbCopyAll_CheckedChanged(object sender, EventArgs e)
        {
            EnsureStatus();
            if (cbCopyAll.Checked)
            {
                firstMatch = true;
                countMarkedForDeletion = 0;
                lastImageLoadedInPreview = "test0";
                cbCopyAll.BackColor = Color.LightGreen;
                if (gv.imageFileErrorList == null)
                    gv.imageFileErrorList = new ImageFileList();
                else
                    gv.imageFileErrorList?.clearList();
                UpdateImageErrorsCount();

                //if (cbCopyAll.Checked)
                CopyAllExec();
            }
            else
            {
                cbCopyAll.BackColor = Color.LightGray;
            }

        }

        private void btGoToSlideNumber_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbGoToSlide.Text))
                tbGoToSlide.Text = "0";
            if (tbGoToSlide.Text.Equals("0"))
            {
                cbFindDuplicates.Checked = false;
            }
            GoToSlideNumber();
            enteringSlideNumber = false;
        }

        private void btSetCopyNumber_Click(object sender, EventArgs e)
        {
            gv.maxCountCopy = (int)numCopyBatchNumber.Value;
        }
        int goToNextBatch = 0;
        private void cbCopy1Batch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCopy1Batch.Checked)
            {
                goToNextBatch += 1000;
                numCopyBatchNumber.Value = 1000;
                tbGoToSlide.Text = goToNextBatch.ToString();
                gv.bCopyOnlyOneBatch = true;
            }
            gv.maxCountCopy = (int)numCopyBatchNumber.Value;
        }

        private void btFindDuplicates_Click(object sender, EventArgs e)
        {

        }

        private void btResume_Click(object sender, EventArgs e)
        {
            ResumeSlideShow();
        }
        public void ResumeSlideShow()
        {
            if (gv.nextIdx >= gv.slideCount1 - 1)
                return;
            if (cbMasterOFF.Checked)
            {
                // Emergency stop - user has enabled Master OFF
                cbCopyAll.Checked = false;
                cbFindDuplicates.Checked = false;
                cbSlideShow.Checked = false;

                stopSlideShow("Stopped by Master OFF");
                timer.Stop();
                timer.Enabled = false;

                gv.bSlideShow = false;

                tbMessage.Text = "All operations stopped (Master OFF is checked)";
                tbMessage.BackColor = Color.Orange;

                return; // Exit immediately - don't resume
            }
            btResume.Text = "Resume F11";
            btResume.BackColor = Color.LightGray;
            if (!bStartedSlideShowScan)
                StartSlideShowScan();
            else
                ResumeDupSearch();
        }
        public void ResumeDupSearch(int addCount = 1)
        {
            if (!gv.FILE_TYPE.Equals("images"))
                return;
            gv.nextIdx += addCount;
            if (gv.nextIdx < (gv.slideCount1))
            {
                this.startSlideShow(gv.nextIdx, bAutoStartAllow, 1); //false autostart
                if (bAutoStartAllow)
                    setSlideShowOn(true);
                bAutoStartAllow = true; //next time
            }
        }

        public void btDeleteAndResume_Click(object sender, EventArgs e)
        {
            markDeleted();
            ResumeDupSearch();
        }
        private void btDeleteAndResume_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (fileSizePrevious == fileSize)
                {
                    markDeleted();
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                //do something
            }
            ResumeDupSearch();
        }

        private void cbFindDuplicates_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFindDuplicates.Checked)
            {
                // btDeleteAndResume.BackColor = Color.LightGray;
                // btDeleteAndResume.Refresh();
                // cbAllowDelete.Checked = true;
            }
        }

        private void cbThumbNailONLY_CheckedChanged(object sender, EventArgs e)
        {
            pb1.Visible = !cbThumbNailONLY.Checked;
            //pbNext.Visible = !cbThumbNailONLY.Checked;
        }

        private void rbMove_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbShowCopyStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbShowCopyStatus.Checked)
                btCopyImageInfo.Visible = false;
        }
        int lastCountLoaded = 0;
        private void btClearImageList_Click_1(object sender, EventArgs e)
        {
            gv.slideCount1 = 0;
        }

        private void btSetLastCount_Click(object sender, EventArgs e)
        {
            if (gv.imageFileList1 == null)
            {
                tbImageNumber.Text = "0";
            }
            else
            {
                gv.slideCount1 = gv.imageFileList1.getImageFileListLength();
                tbImageNumber.Text = gv.slideCount1.ToString("N0");
            }
        }

        private void cbShowTravWin_CheckedChanged(object sender, EventArgs e)
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                gv.dialogTraverser2.Activate();
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            if (gv != null)
                if (gv.bExitProgram)
                    this.Close();
        }
        int idxToStart = 0;
        private void btTest_Click(object sender, EventArgs e)
        {
            //if (cbFindMatch.Checked)
            // idxToStart = ScanImagesInList();
            cbThumbNailONLY.Checked = true;
        }

        private void tbGoToSlide_MouseClick(object sender, MouseEventArgs e)
        {
            enteringSlideNumber = true;
        }

        private void btToNextFolder_Click(object sender, EventArgs e)
        {
            RunSlideShowToNextFolder();
        }

        //
        // 2023
        //
        public void GetBmp2()
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                pb2Image.Image = gv.dialogTraverser2.img1;
            }

        }
        public void GetNextBmp2FromTransverser2()
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                gv.dialogTraverser2.GoToNextImageDataRow();
            }

        }

        //
        double CompareImagesOriginal(Bitmap InputImage1, Bitmap InputImage2, int Tollerance = 10)
        {
            if (InputImage1 == null || InputImage2 == null)
                return -1;
            Bitmap Image1 = new Bitmap(InputImage1, new System.Drawing.Size(128, 128));
            Bitmap Image2 = new Bitmap(InputImage2, new System.Drawing.Size(128, 128));
            int Image1Size = Image1.Width * Image1.Height;
            int Image2Size = Image2.Width * Image2.Height;
            Bitmap Image3;
            if (Image1Size > Image2Size)
            {
                Image1 = new Bitmap(Image1, Image2.Size);
                Image3 = new Bitmap(Image2.Width, Image2.Height);
            }
            else
            {
                Image1 = new Bitmap(Image1, Image2.Size);
                Image3 = new Bitmap(Image2.Width, Image2.Height);
            }
            for (int x = 0; x < Image1.Width; x++)
            {
                for (int y = 0; y < Image1.Height; y++)
                {
                    Color Color1 = Image1.GetPixel(x, y);
                    Color Color2 = Image2.GetPixel(x, y);
                    int r = Color1.R > Color2.R ? Color1.R - Color2.R : Color2.R - Color1.R;
                    int g = Color1.G > Color2.G ? Color1.G - Color2.G : Color2.G - Color1.G;
                    int b = Color1.B > Color2.B ? Color1.B - Color2.B : Color2.B - Color1.B;
                    Image3.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            int Difference = 0;
            for (int x = 0; x < Image1.Width; x++)
            {
                for (int y = 0; y < Image1.Height; y++)
                {
                    Color Color1 = Image3.GetPixel(x, y);
                    int Media = (Color1.R + Color1.G + Color1.B) / 3;
                    if (Media > Tollerance)
                        Difference++;
                }
            }
            double UsedSize = Image1Size > Image2Size ? Image2Size : Image1Size;
            double result = Difference * 100 / UsedSize;
            return Difference * 100 / UsedSize;
        }
        double unmatchPercent = 100;

        public double CompareImages2(Bitmap image, Bitmap image2, int range = 10)
        {
            unmatchPercent = ImageCompare.CompareImages((Bitmap)image, (Bitmap)image2, 10);
            double matchPercentage = 100 - (Math.Round(unmatchPercent, 2));

            tbWatch.Text = unmatchPercent.ToString();
            tbMatchPercent.Text = $"{matchPercentage:N2}";
            if (matchPercentage > 90) //a match STOP
            {
                tbWatch.BackColor = Color.LightGreen;
                /*
                if (cbDeleteAndResume.Checked)
                {
                    DisplayAutoDeletePrompt();
                }
                else
                {
                    if (!cbContinueAfterDelete.Checked)
                        stopSlideShow();
                }
                */
            }
            else
            {
                tbWatch.BackColor = Color.LightGray;
            }
            return matchPercentage;
        }
        public double CompareImageWithTraverser2(Bitmap image)
        {

            unmatchPercent = CompareImages2((Bitmap)image, (Bitmap)pb2Image.Image, 10); // image pb2Image
            tbWatch.Text = unmatchPercent.ToString();
            if (unmatchPercent < 20) //a match STOP
            {
                tbWatch.BackColor = Color.LightGreen;

                if (cbDeleteAndResume.Checked)
                {
                    markDeleted();
                    //ResumeDupSearch();
                    this.startSlideShow(gv.nextIdx++, true, 1);
                    setSlideShowOn(true);
                }
                else
                {
                    stopSlideShow();
                }
            }
            else
            {
                tbWatch.BackColor = Color.LightGray;
            }
            double match = 100 - (Math.Round(unmatchPercent, 2));
            return match;
        }
        public double CompareImagesOnList()
        {
            unmatchPercent = CompareImages2((Bitmap)pb1.Image, (Bitmap)pb2Image.Image, 10); // image pb2Image
            tbWatch.Text = unmatchPercent.ToString();
            double match = 100 - (Math.Round(unmatchPercent, 2));
            if (unmatchPercent < 0) //a  STOP
            {
            }
            else if (match > 89)
            {
                // System.Windows.Forms.MessageBox.Show("High match Percent", $"match");
            }
            return match;
        }
        bool bUseTransverser2 = false;
        private void cbFindCompare_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFindMatch.Checked)
            {
                if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                {
                    idx2 = 0;
                    pb2Image.BringToFront();
                    GetBmp2();
                    bUseTransverser2 = true;
                }
                else
                {
                    bUseTransverser2 = false;
                    cbFindMatch.Checked = false;
                }
            }
            else
            {
                pb2Image.SendToBack();
            }
        }

        private void btCompare_Click(object sender, EventArgs e)
        {
            idx2 = gv.nextIdx;
            CompareAndStopOnMatch();
        }
        bool bMatchPreview = false;

        public bool CompareAndStopOnMatch()
        {
            double match = 0;
            if (!cbFindMatch.Checked)
                return false;
            if (pb1.Image != null)
                match = CompareImageWithTraverser2((Bitmap)pb1.Image);
            if (match < 20)
                return true;
            if (bMatchPreview)
            {
                if (pbNext.Image != null)
                    match = CompareImageWithTraverser2((Bitmap)pbNext.Image);
                if (match < 20)
                    return true;
            }
            return false;
        }

        private void btNextImageForCompare_Click(object sender, EventArgs e)
        {
            GetNextBmp2FromTransverser2();
        }

        private void button2_Click_3(object sender, EventArgs e)
        {
            cbFindMatch.Checked = true;
            GetBmp2();
        }

        private void cbDeleteAndResume_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbAutoDeleteExactMatch_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btUndelete_Click(object sender, EventArgs e)
        {
            markDeleted(false);
        }
        private void DeleteItemNumber(int idx)
        {
            if (idx < 0 || idx >= gv.imageFileList1.getImageCount() - 1)
            {
                return;
            }
            FileInfoItem fi = gv.imageFileList1.getIndexed(idx);
            fi.bDelete = true;
        }

        private void tbPrevious3FileSize_DoubleClick(object sender, EventArgs e)
        {
            int idx = Convert.ToInt32(tbSlideNumber.Text.Replace(",", ""));
            idx -= 3;
            if (idx < 0 || idx >= gv.imageFileList1.getImageCount() - 1)
            {
                return;
            }
            FileInfoItem fi = gv.imageFileList1.getIndexed(idx);
            fi.bDelete = true;
        }

        private void tbPrevious3FileSize_TextChanged(object sender, EventArgs e)
        {

        }
        bool b4Color = true;
        private void pbFourthLast_Click(object sender, EventArgs e)
        {
            if (!cbAllowDelete.Checked)
                return;
            int idx = Convert.ToInt32(tbSlideNumber.Text.Replace(",", ""));
            idx -= 4;
            if (idx < 0 || idx >= gv.imageFileList1.getImageCount() - 1)
            {
                return;
            }
            btDeleteMarked.Enabled = true;
            FileInfoItem fi = gv.imageFileList1.getIndexed(idx);
            fi.bDelete = true;
            if (b4Color)
                tbPrevious4FileSize.BackColor = Color.LightPink;
            else
                tbPrevious4FileSize.BackColor = Color.LightBlue;
            b4Color = !b4Color;
            if (cbContinueAfterDelete2.Checked)
                ResumeDupSearch();
        }
        bool b3Color = true;
        private void pbThirdLast_Click(object sender, EventArgs e)
        {
            if (!cbAllowDelete.Checked)
                return;
            int idx = Convert.ToInt32(tbSlideNumber.Text.Replace(",", ""));
            idx -= 3;
            if (idx < 0 || idx >= gv.imageFileList1.getImageCount() - 1)
            {
                return;
            }
            btDeleteMarked.Enabled = true;
            FileInfoItem fi = gv.imageFileList1.getIndexed(idx);
            fi.bDelete = true;
            if (b3Color)
                tbPrevious3FileSize.BackColor = Color.LightPink;
            else
                tbPrevious3FileSize.BackColor = Color.LightBlue;
            b3Color = !b3Color;
            if (cbContinueAfterDelete2.Checked)
                ResumeDupSearch();
        }
        bool b2Color = true;
        private void pbSecondLast_Click(object sender, EventArgs e)
        {
            if (!cbAllowDelete.Checked)
                return;
            int idx = Convert.ToInt32(tbSlideNumber.Text.Replace(",", ""));
            idx -= 2;
            if (idx < 0 || idx >= gv.imageFileList1.getImageCount() - 1)
            {
                return;
            }
            btDeleteMarked.Enabled = true;
            FileInfoItem fi = gv.imageFileList1.getIndexed(idx);
            fi.bDelete = true;
            if (b2Color)
                tbPrevious2FileSize.BackColor = Color.LightPink;
            else
                tbPrevious2FileSize.BackColor = Color.LightBlue;
            b2Color = !b2Color;
            if (cbContinueAfterDelete2.Checked)
                ResumeDupSearch();
        }
        ImageFileList imageFileListForDelete;
        public ImageFileList GetDeleteList()
        {
            imageFileListForDelete = new ImageFileList();

            for (int idx = 0; idx < gv.imageFileList1.getImageCount(); idx++)
            {
                FileInfoItem fi = gv.imageFileList1.getFinfoItem(idx);
                if (fi.bDelete)
                {
                    imageFileListForDelete.addItem(fi);
                }
            }
            return imageFileListForDelete;

        }
        bool bLastColor = true;
        private void pbThumbNailLast_Click(object sender, EventArgs e)
        {
            if (!cbAllowDelete.Checked)
                return;
            int idx = Convert.ToInt32(tbSlideNumber.Text.Replace(",", ""));
            idx -= 1;
            if (idx < 0 || idx >= gv.imageFileList1.getImageCount() - 1)
            {
                return;
            }
            btDeleteMarked.Enabled = true;
            FileInfoItem fi = gv.imageFileList1.getIndexed(idx);
            fi.bDelete = true;
            if (bLastColor)
                tbPreviousFileSize.BackColor = Color.LightPink;
            else
                tbPreviousFileSize.BackColor = Color.LightBlue;
            bLastColor = !bLastColor;
            if (cbContinueAfterDelete2.Checked)
                ResumeDupSearch();
        }

        private void pbThumbNail_Click(object sender, EventArgs e)
        {

        }

        private void tbPreviousFileSize_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbPrevious2FileSize_TextChanged(object sender, EventArgs e)
        {

        }
        private void btSaveDeleteList_Click(object sender, EventArgs e)
        {
            SaveDeleteFileToXML();
        }
        public void SetAllowDelete(bool allow)
        {
            gv.bAllowDeleteWithControlClick = allow;
        }

        private void cbAllowDelete_CheckedChanged(object sender, EventArgs e)
        {
            btDeleteMarked.Enabled = true;
        }

        private void btResizePreview_Click(object sender, EventArgs e)
        {
            //ResizePreviewSet();
            pb2Image.Image = null;
        }

        private void btDisplayFolderPath_Click(object sender, EventArgs e)
        {
            tbMessage.Text = fpathCurrentImage;
        }

        private void btIdx3Next_Click(object sender, EventArgs e)
        {
            ResetIdx3Search();
        }
        public void ResetIdx3Search()
        {
            idx3 = 0;
            btIdx3Next.Text = "0";
            ShowNextList3AndCompare();
        }
        public void NextIdx3()
        {
            ++idx3;
            if (idx >= gv.imageFileListCompare.getImageCount())
                idx3 = -1;
            btIdx3Next.Text = idx3.ToString();
        }
        /*
         * It’s the click handler that runs a “find next match in list 3” workflow and, if requested, shows the matched image on a fourth display.
                What it does:
                •	Calls CompareNext3() to iterate gv.imageFileListCompare (defaults to gv.imageFileList2) from the current idx3, loading each candidate into pb2Image and comparing it to the current main image (pb1.Image) via ShowNextList3AndCompare() → CompareImagesOnList().
                •	If a candidate passes the threshold (result ≤ 20), CompareNext3() returns true.
                •	If the “display on display 4” checkbox (cbDisplay1idx3) is checked and a match was found, DisplayIdx3OnDisplay4() opens/uses display4 and shows the matched pb2Image there.
                Side effects and gotchas:
                •	UI updates: tbResultIdx3Compare shows the last compare score; pb2Image shows the current candidate.
                •	Uses & instead of && in the if condition, so it won’t short‑circuit. Prefer &&.
                •	ShowNextList3AndCompare() pulls items from gv.imageFileList2 even if gv.imageFileListCompare was set to a different list; align those if you intend to compare another list.

        */
        private void btIdx3Show_Click(object sender, EventArgs e)
        {
            lastFoundIdx3 = 0;
            bool rc = CompareImage1WithIdx3();
            if (rc && cbDisplay1idx3.Checked)
                DisplayIdx3OnDisplay4();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ContinueSearch(-2);
        }
        public void ContinueSearch(int startIdx)
        {
            CompareImage1WithIdx3(startIdx);
        }
        int lastFoundIdx3 = 0;
        public bool CompareImage1WithIdx3(int startIdx = 0)
        {
            if (startIdx == -2)
                idx3 = lastFoundIdx3 + 1;
            else
                idx3 = 0;

            bool match = false;
            tbResultIdx3Compare.Text = "";
            double result = 0;
            do
            {
                tbMessageFromMatching.Text = $"{idx3}";
                result = ShowNextList3AndCompare();
                tbResultIdx3Compare.Text = result.ToString();
                if (result > 94)
                {
                    System.Windows.Forms.MessageBox.Show("High match Percent", $"match");
                    return true;
                }
                if (result > 89)
                {
                    System.Windows.Forms.MessageBox.Show("Very high match value, check images", "TEST");
                    return true;
                }
                if (result < 0)
                    return false;  // result set to -1  at CompareImagesOnList()  no images left to compare
                /* if (result > 20)
                 {
                     if (idx3 <= 0)
                     {
                         ResetIdx3Search();
                         tbResultIdx3Compare.Text = "no";
                         return false;
                     }


                 }*/
                NextIdx3();
            } while (result < 95);

            return true;
        }

        private void btNextAndCompare_Click(object sender, EventArgs e)
        {
            ResetIdx3Search();
            showNextSlideNow();
            bool rc = CompareImage1WithIdx3();
            if (rc & cbDisplay1idx3.Checked)
                DisplayIdx3OnDisplay4();
        }

        private void cbDisplay1idx3_CheckedChanged(object sender, EventArgs e)
        {
            DisplayIdx3OnDisplay4();
        }

        public void DisplayIdx3OnDisplay4()
        {
            if (cbDisplay4.Checked)
            {
                if (display4 == null || display4.IsDisposed)
                {
                    display4 = new Display1(gv);
                    display4.setDisplayMonitor(gv.screen[3], 4);
                }
                display4.Show();
                display4.displayThisImage((Bitmap)pb2Image.Image);
            }
        }

        public bool copyInvalidImages = true;
        System.Drawing.Bitmap bitmap1;
        public System.Drawing.Bitmap LoadImageBmap(string imageFilePath)
        {
            bitmap1 = new Bitmap(imageFilePath);

            return bitmap1;
        }
        System.Drawing.Bitmap bitmapSource;

        string fileNameBase = "SDB";
        // string fileExt = ".txt";

        public string CreateNotesFileName(int numFileNumber)
        {
            string fileName = fileNameBase + gv.notesBaseName + numFileNumber.ToString() + fileExt;
            return fileName;
        }
        private void btResetAll_Click(object sender, EventArgs e)
        {
            cbDisplayOnThisDisplay.Checked = true;
        }
        EditNotes notes;
        private void btEditNotes_Click(object sender, EventArgs e)
        {
            if (notes == null || notes.IsDisposed)
            {
                notes = new EditNotes(gv);
            }
            notes.Show();
            notes.Activate();
        }

        private void btContinue_Click(object sender, EventArgs e)
        {
            ContinueSlideShow();
        }

        private void btNextFolder_Click(object sender, EventArgs e)
        {
            FindNextFolder();
        }

        private void btShowListWindow_Click(object sender, EventArgs e)
        {
            if (gv.fileListWin != null && !gv.fileListWin.IsDisposed)
            {
                gv.fileListWin.WindowState = FormWindowState.Normal;
                gv.fileListWin.Activate();
            }
        }

        private void b_Click(object sender, EventArgs e)
        {
            // cbThumbNailONLY.Checked = true;
            ShowSafeImage();
        }

        public string directoryPathSafeImage = "no";
        public string safeImageFP = "no";

        public bool VerifySafeDemoImage()
        {
            directoryPathSafeImage = Path.GetDirectoryName(appRunningFrom);
            safeImageFP = Path.Combine(directoryPathSafeImage, "safe.jpg");
            if (!File.Exists(safeImageFP))
            {
                tbMessage.Text = $"File demo image not found: {safeImageFP}";
                return false;
            }
            return true;
        }
        public void ShowSafeImage()
        {
            // quick sanity checks
            if (string.IsNullOrWhiteSpace(safeImageFP))
            {
                tbMessage.Text = "No application path specified.";
                return;
            }

            if (!File.Exists(safeImageFP))
            {
                tbMessage.Text = $"File demo image not found: {safeImageFP}";
                return;
            }

            // optional: validate header bytes to ensure it's an image
            if (!IsValidImage(safeImageFP))
            {
                tbMessage.Text = $"Not a valid image file: {safeImageFP}";
                return;
            }

            // keep reference to previous image so we can dispose it safely after load
            var previous = pb1.Image;
            try
            {
                // Open read-only with shared read so other processes (or the app) can access the file
                using (var fs = new FileStream(safeImageFP, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using var loaded = System.Drawing.Image.FromStream(fs);
                    // create a copy so the stream can be closed and file unlocked immediately
                    pb1.Image = new Bitmap(loaded);
                }

                // update other pictureboxes if desired (following original ShowSafeImage behavior)
                pb2Image.Image = pb1.Image;
                pb1ThumbNail.Image = pb1.Image;
                pbSecondLast.Image = pb1.Image;
                pbThirdLast.Image = pb1.Image;
                pbFourthLast.Image = pb1.Image;
                pbLastImage.Image = pb1.Image;
                pbNext.Image = pb1.Image;

                tbMessage.Text = $"Loaded image: {safeImageFP}";
                pb1.Refresh();
            }
            catch (Exception ex)
            {
                tbMessage.Text = $"Failed to load image: {ex.Message}";
            }
            finally
            {
                // dispose old image if it is not the same as the newly assigned one
                if (previous != null && previous != pb1.Image)
                    previous.Dispose();
            }
        }
        private void cbCompareABforDups_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbMaxPBsize_CheckedChanged(object sender, EventArgs e)
        {
            int width = this.width;
            if (cbMaxPBsize.Checked)
            {
                width = Math.Abs(tbMessageFromMatching.Location.X);
                pb1.Width = 2400;
            }
            else
            {
                pb1.Width = 1200;
            }
            this.Refresh();
            pb1.Refresh();

            // tbComment.Text = pb1.Width.ToString();
        }
        int pbwidth = 1200;
        private void btExpandPb1_Click(object sender, EventArgs e)
        {
            int newPb1Width = Int32.Parse(tbPb1Width.Text); //rowIdMovieList
            pbwidth = (int)newPb1Width;
            tbPb1Width.Text = newPb1Width.ToString();
            pb1.Width = newPb1Width;
            pb1.Refresh();
        }

        private void tbPb1Width_TextChanged(object sender, EventArgs e)
        {

        }

        private void btResize1080_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Size = new System.Drawing.Size(1920, 1080);
        }

        private void btResize1080_Resize(object sender, EventArgs e)
        {
            System.Drawing.Rectangle currentScreen = GetScreen2();
            btSizeWindow.Text = $"{this.Bounds.Width} {this.Bounds.Height}";
        }
        public System.Drawing.Rectangle GetScreen2()
        {
            return Screen.FromControl(this).Bounds;
        }

        private void tbSlideNumber_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void tbSlideNumber_Click(object sender, EventArgs e)
        {
            cbCatalogImages.Checked = false;
        }

        private void tbGoToSlide_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbGoToSlide_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GoToSlideNumber();
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            tbScaling.Text = $"Form DvcDpi = {this.DeviceDpi}";
            tbScaling.Text += $"Form AutoScaleDimensions = {this.AutoScaleDimensions}";
        }

        private void cbSlideShowOff_CheckedChanged(object sender, EventArgs e)
        {
            //bMasterStopSlideShow = !cbSlideShowOff.Checked;
            if (!cbSlideShowOff.Checked)
                setSlideShowOn(false);
        }

        private void cbTopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = cbTopMost.Checked;
        }

        private void btFullScreenAll_Click(object sender, EventArgs e)
        {
            SetFullSizeMode();
        }

        private void tbVideoType_Click(object sender, EventArgs e)
        {

        }

        private void btDeleteNextAndContinue_Click(object sender, EventArgs e)
        {
            MarkDeleteAndContinue();
        }
        public void MarkDeleteAndContinue()
        {
            ++countMarkedForDeletion;
            tbMarkedForDeletion.Text = countMarkedForDeletion.ToString();
            showNextSlideNow();
            markForDeletion();
            ResumeSlideShow();
        }

        private void tbTargetFolder_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // tbTargetFolder_TextChanged(sender, e);
        }
        int idxSource = 0;
        int numberMatching = 0;
        Bitmap otherImage = null;

        bool bOptimizedSearch = true;

        double matchMinmatchMin = 0.9; //90%
        //
        private CancellationTokenSource _cts;
        private ImageMatcher _matcher = new ImageMatcher();
        private List<string> _matches = new List<string>();

        private string _lastSearchFolder;



        public int BuildMatchingImagesList(List<string> matches)
        {
            // Create or clear the matches ImageFileList, then populate with metadata.
            if (matches == null || matches.Count == 0)
                return 0;

            try
            {
                if (gv.imageFileListMatches == null)
                    gv.imageFileListMatches = new ImageFileList();
                else
                    gv.imageFileListMatches.clearList();
            }
            catch (Exception ex)
            {
                gv.debug.w("BuildMatchingImagesList: failed to initialize imageFileListMatches", ex.Message);
                return 0;
            }

            var added = 0;
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var m in matches)
            {
                if (string.IsNullOrWhiteSpace(m))
                    continue;

                string full;
                try
                {
                    full = Path.GetFullPath(m);
                }
                catch
                {
                    full = m;
                }

                if (seen.Contains(full))
                    continue;

                // Attempt to load metadata from repository first.
                FileInfoItem itemFromDb = null;
                try
                {
                    var repoType = _repo.GetType();
                    var byFpath = repoType.GetMethod("GetByFpath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    if (byFpath != null)
                    {
                        var res = byFpath.Invoke(_repo, new object[] { full });
                        if (res is FileInfoItem fii) itemFromDb = fii;
                    }
                    else
                    {
                        var byKey = repoType.GetMethod("GetByKeyPath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        if (byKey != null)
                        {
                            var key = full.ToUpperInvariant();
                            var res2 = byKey.Invoke(_repo, new object[] { key });
                            if (res2 is FileInfoItem fii2) itemFromDb = fii2;
                        }
                    }
                }
                catch
                {
                    itemFromDb = null;
                }

                if (itemFromDb != null)
                {
                    itemFromDb.fpath = full;
                    try
                    {
                        gv.imageFileListMatches.addItem(itemFromDb);
                        ++added;
                        seen.Add(full);
                    }
                    catch { /* ignore add failures */ }
                    continue;
                }

                // Build metadata from filesystem
                var newItem = new FileInfoItem();
                try
                {
                    var fi = new FileInfo(full);
                    newItem.fpath = full;
                    newItem.fname = Path.GetFileName(full);
                    newItem.dpath = Path.GetDirectoryName(full) ?? "";
                    newItem.ext = Path.GetExtension(full) ?? "";
                    newItem.len = fi.Exists ? fi.Length : 0;
                    newItem.stimestamp = fi.Exists ? fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
                }
                catch
                {
                    // best-effort; continue even if FileInfo fails
                }

                // Try to read image dimensions safely.
                try
                {
                    if (File.Exists(full))
                    {
                        using var fs = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var img = System.Drawing.Image.FromStream(fs, false, false);
                        newItem.width = img.Width;
                        newItem.height = img.Height;
                    }
                }
                catch
                {
                    // ignore image read errors (corrupt, locked, unsupported)
                }

                try
                {
                    gv.imageFileListMatches.addItem(newItem);
                    ++added;
                    seen.Add(full);
                }
                catch (Exception ex)
                {
                    gv.debug.w("BuildMatchingImagesList: addItem failed", ex.Message);
                }
            }

            // Return resulting count (try a couple common APIs)
            int count = 0;
            try
            {
                count = gv.imageFileListMatches.getImageFileListLength();
            }
            catch
            {
                try { count = gv.imageFileListMatches.getImageCount(); }
                catch
                {
                    try { count = gv.imageFileListMatches.finfoList?.Count ?? added; } catch { count = added; }
                }
            }

            // Keep UI/status in sync if desired
            try { tbImageNumber.Text = count.ToString("N0"); } catch { }
            gv.debug.w($"BuildMatchingImagesList: added {added} items, total {count}");
            return count;
        }
        // Added helper and checkbox handler to support swapping match results into the main list.
        private void CopyImageFileList(ImageFileList src, ImageFileList dst)
        {
            if (src == null) return;
            if (dst == null) return;

            try
            {
                dst.clearList();
            }
            catch
            {
                // best-effort: if no clear method, try to replace internal list via reflection
                try
                {
                    var t = dst.GetType();
                    var finfoField = t.GetField("finfoList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    if (finfoField != null)
                    {
                        var list = src.getFinfo();
                        finfoField.SetValue(dst, new List<FileInfoItem>(list));
                        return;
                    }
                }
                catch { }
            }

            try
            {
                // prefer getFinfo() if available
                var items = src.getFinfo();
                if (items != null)
                {
                    foreach (var fi in items)
                        dst.addItem(fi);
                    return;
                }
            }
            catch { }

            // fallback: iterate by index
            try
            {
                int cnt = src.getImageFileListLength();
                for (int i = 0; i < cnt; ++i)
                {
                    var fi = src.getIndexed(i);
                    dst.addItem(fi);
                }
            }
            catch { }
        }




        private void PrependMatchesAndEnsureMetadata(List<string> matches)
        {
            if (matches == null || matches.Count == 0)
                return;

            try
            {
                if (gv.imageFileList1 == null)
                    gv.imageFileList1 = new ImageFileList();
            }
            catch
            {
                // If ImageFileList type isn't available for some reason, bail safely.
                return;
            }

            var existing = gv.imageFileList1.finfoList ?? new List<FileInfoItem>();
            var existingPaths = new HashSet<string>(existing
                .Where(f => !string.IsNullOrEmpty(f?.fpath))
                .Select(f =>
                {
                    try { return Path.GetFullPath(f.fpath); } catch { return f.fpath ?? string.Empty; }
                }),
                StringComparer.OrdinalIgnoreCase);

            var prepended = new List<FileInfoItem>(matches.Count);

            foreach (var m in matches)
            {
                if (string.IsNullOrWhiteSpace(m))
                    continue;

                string full;
                try { full = Path.GetFullPath(m); } catch { full = m; }

                if (existingPaths.Contains(full))
                    continue; // already present

                // Try to find metadata from DB repository
                FileInfoItem dbItem = null;
                try
                {
                    var repoType = _repo.GetType();
                    var byFpath = repoType.GetMethod("GetByFpath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    if (byFpath != null)
                    {
                        var res = byFpath.Invoke(_repo, new object[] { full });
                        if (res is FileInfoItem fii) dbItem = fii;
                    }
                    else
                    {
                        var byKey = repoType.GetMethod("GetByKeyPath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        if (byKey != null)
                        {
                            var key = full.ToUpperInvariant();
                            var res2 = byKey.Invoke(_repo, new object[] { key });
                            if (res2 is FileInfoItem fii2) dbItem = fii2;
                        }
                    }
                }
                catch { dbItem = null; }

                if (dbItem != null)
                {
                    dbItem.fpath = full;
                    prepended.Add(dbItem);
                    existingPaths.Add(full);
                    continue;
                }

                // build FileInfoItem from filesystem
                var newItem = new FileInfoItem();
                try
                {
                    var fi = new FileInfo(full);
                    newItem.fpath = full;
                    newItem.fname = Path.GetFileName(full);
                    newItem.dpath = Path.GetDirectoryName(full) ?? "";
                    newItem.ext = Path.GetExtension(full) ?? "";
                    newItem.len = fi.Exists ? fi.Length : 0;
                    newItem.stimestamp = fi.Exists ? fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
                }
                catch { /* ignore */ }

                try
                {
                    using var fs = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var img = System.Drawing.Image.FromStream(fs, false, false);
                    newItem.width = img.Width;
                    newItem.height = img.Height;
                }
                catch { /* ignore image read errors */ }

                prepended.Add(newItem);
                existingPaths.Add(full);
            }

            if (prepended.Count == 0)
            {
                gv.debug.w("PrependMatches: no new unique matches to add");
                return;
            }

            // Attempt to insert using ImageFileList public API (if available).
            var listObj = gv.imageFileList1;
            var listType = listObj.GetType();
            bool insertedViaApi = false;

            // Look for an "Insert" or "Add" style method (case-insensitive search).
            var candidateMethods = listType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                .Where(m =>
                {
                    var name = m.Name.ToLowerInvariant();
                    return name.Contains("insert") || name.Contains("add") || name.Contains("prepend");
                })
                .ToArray();

            // Prefer method accepting FileInfoItem and an index (InsertAt-like)
            System.Reflection.MethodInfo insertAt = candidateMethods
                .FirstOrDefault(m =>
                {
                    var ps = m.GetParameters();
                    return ps.Length == 2 && (ps[0].ParameterType == typeof(int) || ps[0].ParameterType == typeof(long)) &&
                           (ps[1].ParameterType == typeof(FileInfoItem) || ps[1].ParameterType == typeof(object));
                });

            if (insertAt != null)
            {
                // Insert items at index 0 in original order (first match should be first in list)
                for (int i = 0; i < prepended.Count; i++)
                {
                    try
                    {
                        insertAt.Invoke(listObj, new object[] { i, prepended[i] });
                    }
                    catch { /* ignore per-item failures */ }
                }
                insertedViaApi = true;
            }
            else
            {
                // Look for single-parameter add/append method that takes FileInfoItem
                var addMethod = candidateMethods
                    .FirstOrDefault(m =>
                    {
                        var ps = m.GetParameters();
                        return ps.Length == 1 && (ps[0].ParameterType == typeof(FileInfoItem) || ps[0].ParameterType == typeof(object));
                    });

                if (addMethod != null)
                {
                    // If only append is available, to get prepended semantics we append existing after prepended:
                    try
                    {
                        // build merged list and try to set internal list if a setter exists
                        var merged = prepended.Concat(existing).ToList();

                        var finfoField = listType.GetField("finfoList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        if (finfoField != null)
                        {
                            finfoField.SetValue(listObj, merged);
                            insertedViaApi = true;
                        }
                        else
                        {
                            // fallback: clear and add using addMethod
                            // attempt to find a Clear method
                            var clearMethod = candidateMethods.FirstOrDefault(m => m.Name.ToLowerInvariant().Contains("clear"));
                            clearMethod?.Invoke(listObj, null);
                            foreach (var it in merged)
                            {
                                addMethod.Invoke(listObj, new object[] { it });
                            }
                            insertedViaApi = true;
                        }
                    }
                    catch
                    {
                        insertedViaApi = false;
                    }
                }
            }

            // Final fallback: set finfoList directly and attempt to update internal count fields via reflection
            if (!insertedViaApi)
            {
                var merged = prepended.Concat(existing).ToList();
                try
                {
                    var finfoField = listType.GetField("finfoList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    if (finfoField != null)
                    {
                        finfoField.SetValue(listObj, merged);
                    }
                    else
                    {
                        // As last resort, try public property
                        var finfoProp = listType.GetProperty("finfoList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        if (finfoProp != null && finfoProp.CanWrite)
                            finfoProp.SetValue(listObj, merged);
                    }

                    // Try to update common internal counters if present
                    var possibleNames = new[] { "iMaxFileCount", "imageCount", "_count", "count", "iMaxFile", "iCount" };
                    foreach (var name in possibleNames)
                    {
                        var f = listType.GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        if (f != null && f.FieldType == typeof(int))
                        {
                            f.SetValue(listObj, merged.Count);
                        }
                        var p = listType.GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        if (p != null && p.PropertyType == typeof(int) && p.CanWrite)
                        {
                            p.SetValue(listObj, merged.Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    gv.debug.w("PrependMatches fallback failed: ", ex.Message);
                }
            }

            // Refresh slide counts and UI
            try
            {
                gv.slideCount1 = gv.imageFileList1.getImageFileListLength();
            }
            catch
            {
                // fallback to finfoList count
                try { gv.slideCount1 = gv.imageFileList1.finfoList?.Count ?? 0; } catch { }
            }

            try { tbMaxSlideNumber.Text = $"{gv.slideCount1 - 1}"; } catch { }
            try { tbImageNumber.Text = gv.slideCount1.ToString("N0"); } catch { }

            gv.debug.w($"PrependMatches: added {prepended.Count} items; total now {gv.slideCount1}");
        }
        // --- new helper method to add to the Main class (place near other helpers)
        private void PrependMatchesAndEnsureMetadataxxxxx(List<string> matches)
        {
            if (matches == null || matches.Count == 0)
                return;

            try
            {
                // ensure image list exists
                if (gv.imageFileList1 == null)
                    gv.imageFileList1 = new ImageFileList();

                var existing = gv.imageFileList1.finfoList ?? new List<FileInfoItem>();
                var existingPaths = new HashSet<string>(existing
                    .Where(f => !string.IsNullOrEmpty(f?.fpath))
                    .Select(f => Path.GetFullPath(f.fpath)), StringComparer.OrdinalIgnoreCase);

                var prepended = new List<FileInfoItem>(matches.Count);

                foreach (var m in matches)
                {
                    if (string.IsNullOrWhiteSpace(m))
                        continue;

                    string full = null;
                    try { full = Path.GetFullPath(m); } catch { full = m; }

                    if (existingPaths.Contains(full))
                        continue; // skip duplicates

                    // Try to get DB metadata using repository if available (reflective, safe)
                    FileInfoItem dbItem = null;
                    try
                    {
                        var repoType = _repo.GetType();
                        var byFpath = repoType.GetMethod("GetByFpath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        if (byFpath != null)
                        {
                            var res = byFpath.Invoke(_repo, new object[] { full });
                            if (res is FileInfoItem fii) dbItem = fii;
                        }
                        else
                        {
                            // fallback to key lookup
                            var byKey = repoType.GetMethod("GetByKeyPath", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                            if (byKey != null)
                            {
                                var key = full.ToUpperInvariant();
                                var res2 = byKey.Invoke(_repo, new object[] { key });
                                if (res2 is FileInfoItem fii2) dbItem = fii2;
                            }
                        }
                    }
                    catch
                    {
                        dbItem = null;
                    }

                    if (dbItem != null)
                    {
                        // Ensure fpath normalized
                        dbItem.fpath = full;
                        prepended.Add(dbItem);
                        existingPaths.Add(full);
                        continue;
                    }

                    // Build metadata from filesystem when DB not found
                    var newItem = new FileInfoItem();
                    try
                    {
                        var fi = new FileInfo(full);
                        newItem.fpath = full;
                        newItem.fname = Path.GetFileName(full);
                        newItem.dpath = Path.GetDirectoryName(full) ?? "";
                        newItem.ext = Path.GetExtension(full) ?? "";
                        newItem.len = fi.Exists ? fi.Length : 0;
                        newItem.stimestamp = fi.Exists ? fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
                    }
                    catch { /* ignore fileinfo errors */ }

                    // try to read image dimensions safely (no lock)
                    try
                    {
                        using var fs = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var img = System.Drawing.Image.FromStream(fs, false, false);
                        newItem.width = img.Width;
                        newItem.height = img.Height;
                    }
                    catch { /* ignore image read errors */ }

                    prepended.Add(newItem);
                    existingPaths.Add(full);
                }

                // Prepend: keep new unique items first, then existing list
                var merged = prepended.Concat(existing).ToList();
                gv.imageFileList1.finfoList = merged;

                // refresh counts/UI fields used elsewhere
                gv.slideCount1 = gv.imageFileList1.getImageFileListLength();
                try { tbMaxSlideNumber.Text = $"{gv.slideCount1 - 1}"; } catch { }
                try { tbImageNumber.Text = gv.slideCount1.ToString("N0"); } catch { }

                // optional: if you want the UI list refreshed here, uncomment one of the calls below
                // resetDGV(); // if Main has a method to rebind a grid
                // showNextSlideImageFileList1(0, 1); // to force display update
            }
            catch (Exception ex)
            {
                try { gv.debug.w("PrependMatchesAndEnsureMetadata error", ex.Message); } catch { }
            }
        }




        private void btClearAllPictureBoxesUse_Click(object sender, EventArgs e)
        {
            ClearAllPictureBoxesInUse();
            reposition();
        }
        private static Image LoadImageNoLock(string path)
        {
            // open for read with shared read so other processes can access file
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var img = Image.FromStream(fs);
            // return a new Bitmap copy so we can close the stream immediately
            return new Bitmap(img);
        }


        public void ClearAllPictureBoxesInUse() //new
        {
            // Make sure changes happen on UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ClearAllPictureBoxesInUse));
                return;
            }

            void SafeClear(PictureBox pb)
            {
                if (pb?.Image != null)
                {
                    var img = pb.Image;
                    pb.Image = null;   // detach from control first so Paint won't access disposed image
                    img.Dispose();
                }
            }

            SafeClear(pbNext);
            SafeClear(pb1ThumbNail);
            SafeClear(pbLastImage);
            SafeClear(pbSecondLast);
            SafeClear(pbThirdLast);
            SafeClear(pbFourthLast);
        }

        //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< NEW FIND MATCHING IMAGE WINDOW CODE
        // ── FindMatchingWindow bridge members ────────────────────────────────

        /// <summary>Exposes the repository so FindMatchingWindow can do metadata look-ups.</summary>
      //  public FileInfoRepository Repo => _repo;

        /// <summary>Exposes the current main picture-box image (read-only).</summary>
        public Image CurrentMainImage { get; private set; }

        /// <summary>Hides the file-info panel (cbFileInfoFront) if it is visible.</summary>
        public void HideFileInfoPanel()
        {
            if (cbFileInfoFront.Checked)
                cbFileInfoFront.Checked = false;
        }

        /// <summary>Sets tbImageNumber (total image count label).</summary>
        public void SetImageCountText(string text)
        {
            if (tbImageNumber.InvokeRequired)
                tbImageNumber.BeginInvoke(new Action(() => tbImageNumber.Text = text));
            else
                tbImageNumber.Text = text;
        }

        /// <summary>Sets tbMaxSlideNumber.</summary>
        public void SetMaxSlideText(string text)
        {
            if (tbMaxSlideNumber.InvokeRequired)
                tbMaxSlideNumber.BeginInvoke(new Action(() => tbMaxSlideNumber.Text = text));
            else
                tbMaxSlideNumber.Text = text;
        }

        /// <summary>Sets tbGoToSlide.</summary>
        public void SetGoToSlideText(string text)
        {
            if (tbGoToSlide.InvokeRequired)
                tbGoToSlide.BeginInvoke(new Action(() => tbGoToSlide.Text = text));
            else
                tbGoToSlide.Text = text;
        }

        /// <summary>Triggers the go-to-slide logic (reads tbGoToSlide and navigates).</summary>
        ///         /// <summary>Triggers the go-to-slide logic (reads tbGoToSlide and navigates).</summary>
        public void DoGoToSlideNumber()
        {
            if (int.TryParse(tbGoToSlide.Text, out int slideNum))
                showNextSlideImageFileList1(slideNum - 1, 1);
            else
                showNextSlideImageFileList1(0, 1);
        }

        public Image GetCurrentImage()
        {
            return pb1.Image;
        }
        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<  END NEW FIND MATCHING IMAGE WINDOW CODE



        private void btThumbnailImages_Click(object sender, EventArgs e)
        {
            // from your main form, e.g. on a menu or button:



        }

        private void btSizeWindow_Click(object sender, EventArgs e)
        {

        }

        private void cbStopNextFolder_CheckedChanged(object sender, EventArgs e)
        {

        }



        private void cbPanel1_CheckedChanged(object sender, EventArgs e)
        {
            panelMisc.Enabled = cbPanel1.Checked;
            panelMisc.Visible = cbPanel1.Checked;
            if (cbPanel1.Checked)
                panelMisc.BringToFront();
        }

        private void cbPanel2_CheckedChanged(object sender, EventArgs e)
        {
            panelOther.Enabled = cbPanel2.Checked;
            panelOther.Visible = cbPanel2.Checked;
            if (cbPanel2.Checked)
                panelOther.BringToFront();
        }

        //

        public void MasterOff()
        {
            //bMasterStopSlideShow = true;
            timer.Stop();
            timer.Enabled = false;
            cbMasterOFF.Checked = true;
        }
        public void MasterOn()
        {
            //bMasterStopSlideShow = false;
            timer.Enabled = true;
            //    cbMasterOFF.Checked = false;
        }
        private void btResetTimerSlideShow_Click(object sender, EventArgs e)
        {
            MasterOn();
        }

        private void btExit_Click(object sender, EventArgs e)
        {

        }

        private void btMarkedForDeletion_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbMarkedForDeletion_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            countMarkedForDeletion = 0;
            tbMarkedForDeletion.Text = countMarkedForDeletion.ToString();
        }

        private void cbContinueAfterDelete2_CheckedChanged(object sender, EventArgs e)
        {

        }
        private ImageCache _imageCache;

        private void btLoadImageCache_Click(object sender, EventArgs e)
        {
            _imageCache = new ImageCache(gv.imageFileList1, batchSize: 132);
        }
        int nextBatchStart = 0;
        private void btImageCache2_Click(object sender, EventArgs e)
        {
            nextBatchStart += 132;
        }
        public void loadNextSet(int startingImageFileIdx)
        {
            nextBatchStart = ((startingImageFileIdx + _imageCache._batchSize) / _imageCache._batchSize)
                      * _imageCache._batchSize;
            if (nextBatchStart < gv.imageFileList1.getImageCount())
                _ = _imageCache.PreloadBatchAsync(nextBatchStart);
        }
        bool listTopMost = false;
        private void dgvFinfo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            listTopMost = !listTopMost;
            if (listTopMost)
                dgvFileInfo.BringToFront();
            else
                dgvFileInfo.SendToBack();
        }
        bool bHaveMouse = false;
        public void HaveMouse(bool bHave)
        {
            bHaveMouse = bHave;
            if (bHave)
                btHaveMouse.BackColor = Color.Red;
            else
                btHaveMouse.BackColor = Color.LightGray;
        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            btHaveMouse.BackColor = Color.LightGray;
        }

        private void cbFileInfoFront_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFileInfoFront.Checked)
            {
                dgvFileInfo.BringToFront();
            }
            else
            {
                dgvFileInfo.SendToBack();
            }

        }



        private void tbScaling_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tbScaling.Text = $"Form DvcDpi = {this.DeviceDpi}";
            tbScaling.Text += $"Form AutoScaleDimensions = {this.AutoScaleDimensions}";
        }

        private void pb2Image_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            pb2Image.Image = pb1.Image;
        }

        private void pb1ThumbNail_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
        public string NormalizeFolderHistoryFileFullName()
        {
            if (string.IsNullOrEmpty(gv.folderHistoryFileFullPathName))
            {
                throw new InvalidOperationException("The folder history file path is null or empty.");
            }

            // Normalize the path to ensure it uses consistent directory separators and is absolute.  
            string normalizedPath = System.IO.Path.GetFullPath(gv.folderHistoryFileFullPathName)
                .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            gv.folderHistoryFileFullPathName = normalizedPath;
            return normalizedPath;
        }



        private void btImportImageFileList_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select ImageFileList XML",
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true
            };

            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            string path = ofd.FileName;
            int ok = 0, errors = 0;

            try
            {
                var doc = XDocument.Load(path);
                var items = doc.Descendants("FileInfoItem");

                foreach (var node in items)
                {
                    try
                    {
                        var f = new FileInfoItem
                        {
                            fname = Get(node, "fname"),
                            ext = Get(node, "ext"),
                            len = TryLong(Get(node, "len")),
                            type = Get(node, "type") ?? "f",
                            level = TryInt(Get(node, "level")),
                            stimestamp = Get(node, "stimestamp"),
                            rating = TryChar(Get(node, "rating")),
                            bDelete = TryBool(Get(node, "bDelete")),
                            bInvalid = TryBool(Get(node, "bInvalid")),
                            width = TryInt(Get(node, "width")),
                            height = TryInt(Get(node, "height")),
                            source = Get(node, "source"),
                            playTime = TryDouble(Get(node, "playTime")),
                            minutes = TryInt(Get(node, "minutes")),
                            seconds = TryInt(Get(node, "seconds")),
                            dpath = Get(node, "dpath"),
                            fpath = Get(node, "fpath"),
                            ndx = TryInt(Get(node, "index")),
                            comment = Get(node, "desc")
                        };

                        // Normalize missing fpath
                        if (string.IsNullOrWhiteSpace(f.fpath))
                            f.fpath = Path.Combine(f.dpath ?? "", f.fname ?? "");

                        //  _repo.Upsert(f);
                        ok++;
                    }
                    catch
                    {
                        errors++;
                    }
                }

                System.Windows.Forms.MessageBox.Show($"XML import complete.\n\nInserted/Updated: {ok}\nErrors: {errors}",
                    "Import XML", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Import failed:\n{ex.Message}", "Import XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---- helper functions ----
        private static string Get(XElement node, string name)
            => node.Element(name)?.Value?.Trim();

        private static int TryInt(string s, int def = 0)
            => int.TryParse(s, out var v) ? v : def;

        private static long TryLong(string s, long def = 0)
            => long.TryParse(s, out var v) ? v : def;

        private static double TryDouble(string s, double def = 0)
            => double.TryParse(s, out var v) ? v : def;

        private static bool TryBool(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            if (bool.TryParse(s, out var b)) return b;
            if (int.TryParse(s, out var i)) return i != 0;
            return s.Equals("y", StringComparison.OrdinalIgnoreCase) || s.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("true", StringComparison.OrdinalIgnoreCase) || s.Equals("t", StringComparison.OrdinalIgnoreCase);
        }

        private static char TryChar(string s)
            => string.IsNullOrWhiteSpace(s) ? ' ' : s.Trim()[0];


        // ----- helpers -----
        private static Dictionary<string, int> BuildHeaderMap(string[] header)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < header.Length; i++)
            {
                var name = header[i]?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                dict[name] = i;

                // allow common aliases
                if (name.Equals("description", StringComparison.OrdinalIgnoreCase)) dict["desc"] = i;
                if (name.Equals("filepath", StringComparison.OrdinalIgnoreCase)) dict["fpath"] = i;
                if (name.Equals("directory", StringComparison.OrdinalIgnoreCase)) dict["dpath"] = i;
            }
            return dict;
        }

        private static string Get(Dictionary<string, int> map, string[] row, string col)
            => map.TryGetValue(col, out var i) && i >= 0 && i < row.Length ? row[i]?.Trim() : null;

        private void btBackupDB_Click(object sender, EventArgs e)
        {
            try
            {
                string sourcePath = SqliteDb.DbPath;  // your app’s live DB
                string backupFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SDB7Backups");

                Directory.CreateDirectory(backupFolder);

                // Build timestamped backup filename
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string backupFile = Path.Combine(backupFolder, $"SDB7_backup_{timeStamp}.db");

                // Use SQLite backup API for a consistent copy
                using (var source = new SqliteConnection($"Data Source={sourcePath}"))
                using (var dest = new SqliteConnection($"Data Source={backupFile}"))
                {
                    source.Open();
                    dest.Open();
                    source.BackupDatabase(dest);
                }

                System.Windows.Forms.MessageBox.Show(
                    $"Database backup created successfully:\n{backupFile}",
                    "Backup Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Backup failed:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        Database dbWin = null;
        private void btDatabase_Click(object sender, EventArgs e)
        {
            if (dbWin != null && !dbWin.IsDisposed)
            {
                dbWin.WindowState = FormWindowState.Normal;
                dbWin.Activate();
                return;
            }
            dbWin = new Database();
            dbWin.Show();
            dbWin.Activate();
            dbWin.BringToFront();


        }

        private void btDisplayHTML_Click(object sender, EventArgs e)
        {
            if (gv.FILE_TYPE == "html" || gv.FILE_TYPE == "htm")
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = finfo1.fpath,
                    UseShellExecute = true,
                });
            }
        }

        private void btShowFolder_Click(object sender, EventArgs e)
        {

        }

        public void FileExplorerOpenAppFolder()
        {
            if (System.IO.File.Exists(appRunningFrom))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{appRunningFrom}\"") { UseShellExecute = true });
            }
        }
        private void btOpenExecFolder_Click(object sender, EventArgs e)
        {
            FileExplorerOpenAppFolder();
            return; // Skip rest of click logic if Ctrl was held
        }

        private void cbWatch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWatch.Checked)
            {
                startWatcher();
            }
            else
            {
                stopWatcher();
            }
        }

        private void cbMasterOFF_CheckedChanged(object sender, EventArgs e)
        {
            cbCopyAll.Checked = false;
            if (cbMasterOFF.Checked)
            {
                stopSlideShow();
            }
        }


        //26

        public FileInfoItem updatedFileInfo = null;
        public FileInfoItem previousRecord = null;

        private void btEditFileInfo_Click(object sender, EventArgs e)
        {
            if (dgvFileInfo.CurrentRow == null)
            {
                MessageBox.Show("No row selected. Please select a row to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Retrieve the current row's data
            int rowIndex = dgvFileInfo.CurrentRow.Index;
            FileInfoItem currentFileInfo = new FileInfoItem
            {
                keyPath = dgvFileInfo.Rows[rowIndex].Cells["keyPath"].Value?.ToString() ?? "",
                fname = dgvFileInfo.Rows[rowIndex].Cells["fname"].Value?.ToString() ?? "",
                ext = dgvFileInfo.Rows[rowIndex].Cells["ext"].Value?.ToString() ?? "",
                dpath = dgvFileInfo.Rows[rowIndex].Cells["dpath"].Value?.ToString() ?? "",
                fpath = dgvFileInfo.Rows[rowIndex].Cells["fpath"].Value?.ToString() ?? "",
                type = dgvFileInfo.Rows[rowIndex].Cells["type"].Value?.ToString() ?? "",
                level = dgvFileInfo.Rows[rowIndex].Cells["level"].Value != null ? Convert.ToInt32(dgvFileInfo.Rows[rowIndex].Cells["level"].Value) : 0,
                len = dgvFileInfo.Rows[rowIndex].Cells["len"].Value != null ? Convert.ToInt64(dgvFileInfo.Rows[rowIndex].Cells["len"].Value) : 0,
                stimestamp = dgvFileInfo.Rows[rowIndex].Cells["stimestamp"].Value?.ToString() ?? "",
                bDelete = dgvFileInfo.Rows[rowIndex].Cells["bDelete"].Value != null && Convert.ToBoolean(dgvFileInfo.Rows[rowIndex].Cells["bDelete"].Value),
                bInvalid = dgvFileInfo.Rows[rowIndex].Cells["bInvalid"].Value != null && Convert.ToBoolean(dgvFileInfo.Rows[rowIndex].Cells["bInvalid"].Value),
                rating = dgvFileInfo.Rows[rowIndex].Cells["rating"].Value != null ? Convert.ToChar(dgvFileInfo.Rows[rowIndex].Cells["rating"].Value) : ' ',
                comment = dgvFileInfo.Rows[rowIndex].Cells["comment"].Value?.ToString() ?? "",
                source = dgvFileInfo.Rows[rowIndex].Cells["source"].Value?.ToString() ?? "",
                playTime = dgvFileInfo.Rows[rowIndex].Cells["playTime"].Value != null ? Convert.ToDouble(dgvFileInfo.Rows[rowIndex].Cells["playTime"].Value) : 0,
                minutes = dgvFileInfo.Rows[rowIndex].Cells["minutes"].Value != null ? Convert.ToInt32(dgvFileInfo.Rows[rowIndex].Cells["minutes"].Value) : 0,
                seconds = dgvFileInfo.Rows[rowIndex].Cells["seconds"].Value != null ? Convert.ToInt32(dgvFileInfo.Rows[rowIndex].Cells["seconds"].Value) : 0,
                width = dgvFileInfo.Rows[rowIndex].Cells["width"].Value != null ? Convert.ToInt32(dgvFileInfo.Rows[rowIndex].Cells["width"].Value) : 0,
                height = dgvFileInfo.Rows[rowIndex].Cells["height"].Value != null ? Convert.ToInt32(dgvFileInfo.Rows[rowIndex].Cells["height"].Value) : 0
            };



            // Open the EditFileInfo form
            using (var editForm = new EditFileInfo(currentFileInfo, this))
            {
                editForm.PopulateFields(currentFileInfo);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Update the method call to pass the required parameter 'currentFileInfo' to the UpdatedFileInfo method.
                    updatedFileInfo = editForm.UpdatedFileInfo();

                    dgvFileInfo.Rows[rowIndex].Cells["fname"].Value = updatedFileInfo.fname;
                    dgvFileInfo.Rows[rowIndex].Cells["ext"].Value = updatedFileInfo.ext;
                    dgvFileInfo.Rows[rowIndex].Cells["dpath"].Value = updatedFileInfo.dpath;
                    dgvFileInfo.Rows[rowIndex].Cells["fpath"].Value = updatedFileInfo.fpath;
                    dgvFileInfo.Rows[rowIndex].Cells["type"].Value = updatedFileInfo.type;
                    dgvFileInfo.Rows[rowIndex].Cells["level"].Value = updatedFileInfo.level;
                    dgvFileInfo.Rows[rowIndex].Cells["len"].Value = updatedFileInfo.len;
                    dgvFileInfo.Rows[rowIndex].Cells["stimestamp"].Value = updatedFileInfo.stimestamp;
                    dgvFileInfo.Rows[rowIndex].Cells["bDelete"].Value = updatedFileInfo.bDelete;
                    dgvFileInfo.Rows[rowIndex].Cells["bInvalid"].Value = updatedFileInfo.bInvalid;
                    dgvFileInfo.Rows[rowIndex].Cells["rating"].Value = updatedFileInfo.rating;
                    dgvFileInfo.Rows[rowIndex].Cells["source"].Value = updatedFileInfo.source;
                    dgvFileInfo.Rows[rowIndex].Cells["playTime"].Value = updatedFileInfo.playTime;
                    dgvFileInfo.Rows[rowIndex].Cells["minutes"].Value = updatedFileInfo.minutes;
                    dgvFileInfo.Rows[rowIndex].Cells["seconds"].Value = updatedFileInfo.seconds;
                    dgvFileInfo.Rows[rowIndex].Cells["width"].Value = updatedFileInfo.width;
                    dgvFileInfo.Rows[rowIndex].Cells["height"].Value = updatedFileInfo.height;
                    dgvFileInfo.Rows[rowIndex].Cells["comment"].Value = updatedFileInfo.comment;

                    MessageBox.Show("File information updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btEditRating_Click(object sender, EventArgs e)
        {
            btEditFileInfo_Click(sender, e);
        }

        private void tbTargetBatchCopy_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbTargetBatchCopy_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Pass the base target folder to the dialog
            using (var dialog = new DialogSelectTargetFolder(tbTargetFolder.Text))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(dialog.SelectedFolder))
                    {
                        tbTargetBatchCopy = dialog.SelectedFolder;

                        // Build and display the full path
                        string fullPath = Path.Combine(
                            tbTargetFolder.Text.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                            dialog.SelectedFolder);

                        //tbMessage.Text = $"Target folder set to: {fullPath}";
                        // tbMessage.BackColor = Color.LightGreen;
                    }
                }
            }
        }
        private void tbTargetBatchCopy_MouseDoubleClickxxxx(object sender, MouseEventArgs e)
        {
            using (var dialog = new DialogSelectTargetFolder())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(dialog.SelectedFolder))
                    {
                        tbTargetBatchCopy = dialog.SelectedFolder;
                        tbMessage.Text = $"Target folder set to: {dialog.SelectedFolder}";
                    }
                }
            }
        }

        private void cbSkipDuplicates_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbStatusProcessing_CheckedChanged(object sender, EventArgs e)
        {
            EnsureStatus();
            if (cbStatusProcessing.Checked)
            {
                status.Show();
            }
            else
            {
                status.Hide();
            }
        }
        DisplayImageList displayIL = null;
        private void cbDisplayImageList_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplayImageList.Checked)
            {
                if (displayIL == null || displayIL.IsDisposed)
                    displayIL = new DisplayImageList(gv);
                displayIL.Show();
            }
            else
            {
                displayIL.Hide();
            }
        }

        private void btMasterStopSlideShow_Click(object sender, EventArgs e)
        {
            StopAll();
        }
        public void StopAll()
        {
            stopSlideShow();
            cbCopyAll.Checked = false;
            cbMasterOFF.Checked = true;
            // ShowSafeImage();
        }
        List<FileInfoItem> DeserializeFromXML(string fpath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<FileInfoItem>));
            TextReader textReader = new StreamReader(fpath);
            List<FileInfoItem> imageInfo;
            imageInfo = (List<FileInfoItem>)deserializer.Deserialize(textReader);
            textReader.Close();

            return imageInfo;
        }
        string imageFileName = "";
        ImageFileList imageFileList1;
        public void LoadImageListFile1()
        {
            //
            imageFileList1 = new ImageFileList();
            //
            //gv.imageFileList1.finfoList = imageFileList1.finfoList;
            imageFileName = Path.GetFileName(gv.lastLoadedImageFile);


            //
            //imageFileList1.finfoList = DeserializeFromXML(openFileDialog1.FileName);
            //




            imageFileList1.finfoList = DeserializeFromXML(gv.lastLoadedImageFile);
            //
            gv.imageFileList1 = new ImageFileList();
            gv.imageFileList1.finfoList = imageFileList1.finfoList;


            gv.nextIdx = 0;
            gv.bImageFileList1Loaded = true;
            gv.slideCount1 = imageFileList1.getImageFileListLength();
            //imageFileList2 = imageFileList1;
            //this.loadListViewRange();
            //SortName();
            //keep whatever order is present in the file 
        }
        private void btReloadLastImageFileList_Click(object sender, EventArgs e)
        {
            gv.lastLoadedImageFile = "unknown;unknown";
            if (gv.lastLoadedImageFile != null)
            {
                if (!File.Exists(gv.lastLoadedImageFile))
                {
                    MessageBox.Show($"File not found:\n{gv.lastLoadedImageFile}");
                    return;
                }
                LoadImageListFile1();
                gv.slideCount1 = gv.imageFileList1.getImageFileListLength();
                tbImageNumber.Text = gv.slideCount1.ToString();
                tbMaxSlideNumber.Text = (gv.slideCount1 - 1).ToString("N0");
                SetFileType("images");
                MessageBox.Show($"Loaded ImageFileList:\n{imageFileName}\nTotal images: {gv.slideCount1}");
            }

        }

        private void cbScanOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (cbScanOnly.Checked)
            {
                cbAutoDeleteExactMatch.Checked = true;
                cbSkipDuplicates.Checked = true;
                this.Refresh();
                cbCopyAll.Checked = true;
            }
            else
            {
                cbAutoDeleteExactMatch.Checked = false;
            }
        }

        private void btHoldImage_Click(object sender, EventArgs e)
        {
            if (pb1.Image == null)
            {
                System.Windows.Forms.MessageBox.Show("Please load a source image first.");
                return;
            }
            //
            pbMatch2.Image = pb1.Image;
            CopyToDgvCompare();

        }
        private void CopyToDgvCompare()
        {
            CopyDgvFileInfoToDgvCompare();
            // dgvCompare.Visible = true;
            // dgvCompare.BringToFront();
        }

        private void btCompareThese2Images_Click(object sender, EventArgs e)
        {
            if (pbMatch2.Image == null)
                return;
            dgvCompare.Visible = true;
            dgvCompare.BringToFront();
            double match = CompareImages2((Bitmap)pb1.Image, (Bitmap)pbMatch2.Image);
            if (match > 90) // 90% similar
            {
                // Mark as duplicate

            }
            tbMatchPercent.Text = match.ToString("F2") + "%";
        }
        /// <summary>
        /// Copies all data from dgvFileInfo to dgvCompare, preserving structure and values.
        /// </summary>
        public void CopyDgvFileInfoToDgvCompare()
        {
            try
            {
                // Clear existing data in dgvCompare
                dgvCompare.Columns.Clear();
                dgvCompare.Rows.Clear();

                // Set up dgvCompare with same properties as dgvFileInfo
                dgvCompare.AllowUserToAddRows = false;
                dgvCompare.AllowUserToDeleteRows = false;
                dgvCompare.RowHeadersVisible = false;
                dgvCompare.ReadOnly = true;

                // Copy column structure
                foreach (DataGridViewColumn col in dgvFileInfo.Columns)
                {
                    var newCol = (DataGridViewColumn)col.Clone();
                    dgvCompare.Columns.Add(newCol);
                }

                // Copy all rows and their cell values
                foreach (DataGridViewRow row in dgvFileInfo.Rows)
                {
                    if (row.IsNewRow) continue; // Skip the new row placeholder

                    var newRow = new DataGridViewRow();
                    newRow.CreateCells(dgvCompare);

                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        newRow.Cells[i].Value = row.Cells[i].Value;
                    }

                    dgvCompare.Rows.Add(newRow);
                }

                // Match column sizing modes
                if (dgvFileInfo.Columns.Count > 0 && dgvCompare.Columns.Count > 0)
                {
                    dgvCompare.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    dgvCompare.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvCompare.Columns[1].MinimumWidth = 100;
                    dgvCompare.Columns[1].FillWeight = 1;
                }

                dgvCompare.Refresh();

                gv.debug.w($"Copied {dgvCompare.Rows.Count} rows from dgvFileInfo to dgvCompare");
            }
            catch (Exception ex)
            {
                gv.debug.w($"Error copying dgvFileInfo to dgvCompare: {ex.Message}");
                MessageBox.Show($"Failed to copy data:\n{ex.Message}", "Copy Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Alternative: Copy only the current image's data to dgvCompare for side-by-side comparison.
        /// </summary>
        public void CopyCurrentImageInfoToDgvCompare(FileInfoItem finfo)
        {
            try
            {
                // Initialize dgvCompare with same structure as dgvFileInfo
                if (dgvCompare.Columns.Count == 0)
                {
                    dgvCompare.Columns.Clear();
                    dgvCompare.Rows.Clear();
                    dgvCompare.AllowUserToAddRows = false;
                    dgvCompare.AllowUserToDeleteRows = false;
                    dgvCompare.RowHeadersVisible = false;
                    dgvCompare.ReadOnly = true;

                    dgvCompare.Columns.Add("colLabel", "Label");
                    dgvCompare.Columns.Add("colValue", "Value");

                    foreach (var lbl in labels)
                    {
                        dgvCompare.Rows.Add(lbl, "");
                    }
                }

                // Populate with current image data
                int idx = 0;
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.fname;
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.dpath;
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.fpath;
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.ext;
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.stimestamp;
                dgvCompare.Rows[idx++].Cells[1].Value = string.Format("{0:##,###,##0}", finfo.len);
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.width.ToString();
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.height.ToString();
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.rating;
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.bDelete;
                dgvCompare.Rows[idx++].Cells[1].Value = finfo.bInvalid;

                dgvCompare.Refresh();
            }
            catch (Exception ex)
            {
                gv.debug.w($"Error copying current image info to dgvCompare: {ex.Message}");
            }
        }

        private void dgvCompare_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            dgvCompare.Visible = false;
        }

        private void btValidateTargetFolder_Click(object sender, EventArgs e)
        {
            bool folderExists = VerifyFolderExists(tbTargetFolder.Text);
            if (folderExists)
            {
                string invalidFolderPath = Path.Combine(tbTargetFolder.Text, "zInvalidImages");
                Directory.CreateDirectory(invalidFolderPath);
                //tbInvalidFolder.Text = invalidFolderPath;
                tbInvalidFolder.Text = new DirectoryInfo(invalidFolderPath).Name;
            }

            btExitApplication.Visible = true;
        }

        private void btForceRun_Click(object sender, EventArgs e)
        {
            SetTimerOn(true);
            initTimer();

            timer.Start();
        }

        private void cbContinueSearchDuplicates_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void numTargetFolderCopyAll_ValueChanged(object sender, EventArgs e)
        {
            tbTargetBatchCopy = numTargetFolderCopyAll.Value.ToString();
        }

        private void Main_LocationChanged(object sender, EventArgs e)
        {
            // Small delay to let Windows finish positioning
            System.Windows.Forms.Timer repositionTimer = new System.Windows.Forms.Timer();
            repositionTimer.Interval = 100;
            repositionTimer.Tick += (s, args) =>
            {
                repositionTimer.Stop();
                repositionTimer.Dispose();

            };
            repositionTimer.Start();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gv.useTestSourceAndTargetFolder == false) // Don't save parms if using test folders to avoid overwriting real settings
                ff.saveInitParms1List(gv.inifileFullPathName, gv.initParm1List);

            Properties.Settings.Default.Save();
        }

        private void btExit_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbInvalidCount_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            invalidImageCount = 0;
            tbMarkedForDeletion.Text = $"Marked for Deletion: {invalidImageCount}";
        }
        DialogInvalidImage dialog;
        int lastImageError = 0;
        public void InvalidImage(string fullpath) //, FileInfoItem fi = null)
        {
            stopSlideShow();
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(InvalidImage), fullpath);
                return;
            }

            if (string.IsNullOrWhiteSpace(fullpath))
                return;

            lastImageError = gv.nextIdx;

            if (dialog == null || dialog.IsDisposed)
                dialog = new DialogInvalidImage(gv);
            dialog.Setup(fullpath, gv.nextIdx, gv.slideCount1, gv.nextIdx + 1); //continue cancel or STOP

            dialog.ShowDialog();

            if (gv.todoAction == "cancel")
            {
                // User chose to cancel the entire process
                StopAll();
                return;
            }
            else if (gv.todoAction == "stop")
            {
                stopSlideShow();
                if (gv.nextIdx > lastImageError)
                {
                    return; // Already moved to next image, so just return
                }
                if (gv.nextIdx < gv.slideCount1)
                {
                    ++gv.nextIdx;
                    return;
                }
            }
            else //"continue"
            {
                // Just continue without stopping
            }
            dialog.Dispose();
            if (gv.bCaptureAllImageInWb2)
            {
                try
                {
                    if (!File.Exists(fullpath))
                    {
                        MessageBox.Show($"File not found:\n{fullpath}", "InvalidImage",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string invalidFolder = tbInvalidFolder.Text?.Trim();
                    if (string.IsNullOrWhiteSpace(invalidFolder))
                    {
                        MessageBox.Show("Invalid folder path is empty.", "InvalidImage",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!Path.IsPathRooted(invalidFolder))
                        invalidFolder = Path.Combine(tbTargetFolder.Text, invalidFolder);

                    Directory.CreateDirectory(invalidFolder);

                    string destFile = Path.Combine(invalidFolder, Path.GetFileName(fullpath));
                    if (File.Exists(destFile))
                    {
                        string name = Path.GetFileNameWithoutExtension(destFile);
                        string ext = Path.GetExtension(destFile);
                        destFile = Path.Combine(invalidFolder, $"{name}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}");
                    }

                    //NOTE ADD THE invalid image file to the list   gv.imageFileErrorList
                    if (true)
                    {
                        if (gv.imageFileErrorList == null)
                            gv.imageFileErrorList = new ImageFileList();

                        var invalidItem = new FileInfoItem(fullpath)
                        {
                            bInvalid = true,
                            rating = 'i',
                            comment = "Invalid image copied to invalid folder"
                        };

                        gv.imageFileErrorList.addItem(invalidItem);
                        UpdateImageErrorsCount();
                    }

                    bool copiedOk = false;

                    try
                    {
                        File.Copy(fullpath, destFile, overwrite: false);
                        copiedOk = File.Exists(destFile);
                    }
                    catch
                    {
                        copiedOk = false;
                    }

                    if (!copiedOk)
                    {
                        // Fallback: stream copy in case the file is locked by another handle
                        using var src = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var dst = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None);
                        src.CopyTo(dst);
                        copiedOk = File.Exists(destFile);
                    }

                    if (!copiedOk)
                    {
                        MessageBox.Show($"Copy failed:\n{fullpath}", "InvalidImage",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (cbDeleteAndResume.Checked)
                        File.Delete(fullpath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Copy failed:\n{ex.Message}", "InvalidImage",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tbGoToSlide_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!int.TryParse(tbGoToSlide.Text, out var value))
                value = 0;

            value++;
            tbGoToSlide.Text = value.ToString();
        }

        private void btTestFolders_Click(object sender, EventArgs e)
        {
            displayParms = new DisplayParms(gv);
            displayParms.Visible = true;
            displayParms.Activate();
        }

        private void btDisplayDebugForImages_Click(object sender, EventArgs e)
        {
            btDebugDisplayState_Click(sender, e);
        }

        private void numSoundLevel_ValueChanged(object sender, EventArgs e)
        {
            SoundLevel(gv.soundVolume);
        }

        private void btSoundTest_Click(object sender, EventArgs e)
        {
            SoundDing();
        }

        private void btResizeImage_Click(object sender, EventArgs e)
        {
            // Verify there is an image loaded in pb1
            if (pb1.Image == null)
            {
                MessageBox.Show("No image is currently loaded in pb1.", "Resize Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(fpathCurrentImage) || !File.Exists(fpathCurrentImage))
            {
                MessageBox.Show("Cannot determine the source file path for the current image.", "Resize Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prompt for minimum width
            string widthInput = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter minimum width in pixels:\n(Current: {pb1.Image.Width}px)",
                "Resize Image – Minimum Width",
                pb1.Image.Width.ToString());

            if (string.IsNullOrWhiteSpace(widthInput))
                return;

            if (!int.TryParse(widthInput, out int minWidth) || minWidth <= 0)
            {
                MessageBox.Show("Please enter a valid positive integer for width.", "Invalid Width", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Prompt for minimum height
            string heightInput = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter minimum height in pixels:\n(Current: {pb1.Image.Height}px)",
                "Resize Image – Minimum Height",
                pb1.Image.Height.ToString());

            if (string.IsNullOrWhiteSpace(heightInput))
                return;

            if (!int.TryParse(heightInput, out int minHeight) || minHeight <= 0)
            {
                MessageBox.Show("Please enter a valid positive integer for height.", "Invalid Height", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Passing null output path causes ImageResizer to prompt the user for the destination folder
                ImageResizer.ResizeToMinimum(fpathCurrentImage, null, minWidth, minHeight);
                MessageBox.Show("Image resized successfully.", "Resize Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resizing image:\n{ex.Message}", "Resize Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>Opens (or shows) the FindMatchingWindow, passing the current pb1 image.</summary>
        public void OpenFindMatchingWindow()
        {
            if (pb1.Image == null)
            {
                MessageBox.Show("Please load an image first.", "Find Matching Image",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_findMatchingWindow == null || _findMatchingWindow.IsDisposed)
                _findMatchingWindow = new FindMatchingWindow(this, gv);

            _findMatchingWindow.Show();
            _findMatchingWindow.BringToFront();
        }

        private void btOpenFindMatching_Click(object sender, EventArgs e)
        {
            CurrentMainImage = pb1?.Image;
            OpenFindMatchingWindow();
            _findMatchingWindow?.RefreshSourceImage();  // update image on re-show
        }

        private void cbDisplayOnThisDisplay_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}




//}// CLASS MAIN

/*
dominated by a boxy old standby: 
2020 1280X1024
FHD  1920X1080p   
 * 1024x768, which held a whopping 41.8 percent of the market in that initial March 2009 report. The giant has fallen to just 18.6 percent today, however; meanwhile,
 * 1366x768 has skyrocketed from a 0.68 percent share to a 19.28 percent share in the same timeframe. 
 * 1280x800 sits in third place with 13 percent.
 * 1280x1024
 * now          ----------
 *              1920x1080p
 *              1280x720p
 *              2460x1440p
 *              3840x2160   4K
 * 
 * 
 
Csharp

NOT !

*/