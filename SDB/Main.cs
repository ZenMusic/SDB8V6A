using SymbolDB;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
//using System.Management;

//using PVS.AVPlayer;

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
// 11/1/2023 fix display detection //run away slide show 
//
// 1/15/2017    added folder watch to display1 popup menu 
// 11/3/2017   slight changes added DGV in Traverser Dialog .. base of movie player
// 11/9/2017    fix image count  and fixed delete ..by clearing all pictureboxes before delete (so file is not in use)
//
// 3/9/2019  added windowmediaplayer
// 3/11/2019 added remote controls to control wmp
// 5/13/2019  zoom and status and focus on DialogTrav
// 5/29/2019  initParms1 
// 5/29/2019  changed / fixed / much
// 5/31/2019 keep focus work
// 6/7/2019  Search movie list and GOTO selection F5 
// 6/11/2019   pause move movie window   // added close Movie button  to allow play & move 
// 6/12/2019  pictures  monitor folder and added  lastDir5 
// 7/10/2019   activate Traverser Dialog if already open
// create init file if it doesn't exist or is invalid  ..  RENAME file 
// 7/17/2019    RENAME .. better control on movies    preparing for 2nd window with search results matching 
// 1/4/2020 simplify copy/move catalog 
// 4/26/2020  search 
// 6/28/2020 
// 9/26/2020  revision for control panel (for single screen) 
// rating and load and save image file list
//  IF PLAY EXCEPTION .. mark as Invalid , MessageBox then go to next line ready to play next
// 9/28/2020   removed LV and rewrote timer / etc.
// 10/26/2020 Media Error handling
// 11/19/2020 error handling .. resave (update) Movie File List  (reset Scan on <Play>)
// 12/11/2020 two way search Traverser -> Child (sub) -> Traverser
// 12/14/2020 up higher folder 
// 12/15/2020 in Traverser  added to PlayNextMovie   trigger event ..  DgvFileList1_SelectionChanged(this, new EventArgs());
// 2021  NOTES AT THE BOTTOM ABOUT ADDING AND USING Windows WMP
// 12/18/2020   refresh2 button 
// 12/22/2020   respostion Traverser2  and set Folder to Target1
// 1/9/2021   fix search1 search2  searchOtherWin
// 1/15/2021  filewatcher 
// 1/17/2021 
//     ClickOnce applications are stored under the user's profile at %LocalAppData%\Apps\2.0\.
//     or  \AppData\Local
//     use taskmanager to get actual location
//  2/28/   Resync .. when player is behind in list
// 9/26   redid 2nd traverser and search and init check for init file and history file 
// 9/30 fix reverse skipping images and added preview either direction
// 10/11 added scan (ignore errors) and metadata collection .. find FOLDER and find NEXT META ENTRY 
// 11/15 lastFolder 
// 12/21/21 major revisions picbox load  copy  NEED TO FIX traverser for DVD
// 1/7/22 fix SearchForNextNotFound but must be playing movies 
// fix search result position Traverser 2
// fix search in Trav2 to reset after each ... ALLOW add Metadata to SubWindow (FILE)
// 2/24/22 Conversion to VS 2022 and redo movies -> WmPlayer    added MIDI

namespace SymbolDB
{

    public partial class Main : Form
    {
        string sVersion = "02/16/25 AM VS2024";
        public bool bMoveImages = false;

        // ---- SoundPlayer plays a chord.
        System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chord.wav");
        // ---  SoundPlayer plays TaDa.
        System.Media.SoundPlayer finishSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav");
        //private string imgFile = null;
        System.Media.SoundPlayer player; //= new System.Media.SoundPlayer(@"c:\mywavfile.wav");
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
        FileInfoItem finfo1 = new FileInfoItem();
        FileInfoItem iinfo2 = new FileInfoItem();
        FileInfoItem iinfoNextImage = new FileInfoItem();

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

        public Main() //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        {
            InitializeComponent();
            dgvFinfo.Size = new System.Drawing.Size(581, 332);
            PreSetupDGV(); //2025
            btCopyImageInfo.Visible = false;
            lblResizeDims.Text = sVersion;
            this.Text = "starting ... init";
            gv = new GlobalVars(this);
            //MessageBox.Show("start");
            mainInit();
            this.Text = sVersion;
            CreateMostRecentImageList();
            cbPanel1.Checked = true;
            cbPanel1.Checked = false;
            cbPanel2.Checked = true;
            cbPanel2.Checked = false;

        }
        public bool bDebugStartup = false;
        public bool bUseLocalApplicationData = false;
        public void mainInit()
        {
            if (bDebugStartup)
                System.Windows.MessageBox.Show($"mainInit");
            //screensInfo = new ScreensInfo(gv);
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
            dgvFinfo.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // 2) Make the Value column fill the remaining space.
            dgvFinfo.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Optionally, set a minimum width or fill weight for finer control:
            dgvFinfo.Columns[1].MinimumWidth = 100;
            dgvFinfo.Columns[1].FillWeight = 1; // default is 100, but 1 works fine for a single fill column

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

            ff = new FileFunctions(gv);

            LoadFolderHistoryList();
            //-------------------GET SCREENS INFO----------------------------------------------------------

            // screensInfo = new ScreensInfo(gv);

            //CreateListViewFinfo(); // use this to display Iinfo for each image

            //initListViewFinfo();
           // initLVFinfo();
            tagBackgroundColor = Color.LightGray;

            lvScreenInfo.BringToFront();

            tbMessage.Text = getProdVersion();
            if (bDebugStartup) System.Windows.MessageBox.Show("Init File");
           // System.Windows.MessageBox.Show("Startup", "SDB5");
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
                System.Windows.MessageBox.Show("no parms", "SDB5");
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
            //
            //
            //

            if (iparmCount < 1)
                System.Windows.MessageBox.Show($"init {iparmCount}");
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
                System.Windows.MessageBox.Show("No Init File");
                iparmCount = createDefaultInitParm1File();
            }
            if (bDebugStartup) System.Windows.MessageBox.Show($"Init File {iparmCount}");
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
                System.Windows.MessageBox.Show("No Init File and cannot create. Exiting.");
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
            pbMatch.SizeMode = PictureBoxSizeMode.Zoom;
            pb2Image.SizeMode = PictureBoxSizeMode.Zoom;
            // loadInitParmsFileXML();
            getSoundList();
            player = new System.Media.SoundPlayer(soundFiles[30]);
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

        //ManagementObject[] objs = new ManagementObject[10];
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
        public void initScreens()
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
            dgvFinfo.Columns.Clear();
            dgvFinfo.Rows.Clear();
            dgvFinfo.AllowUserToAddRows = false;
            dgvFinfo.AllowUserToDeleteRows = false;
            dgvFinfo.RowHeadersVisible = false;
            dgvFinfo.ReadOnly = true;

            // Add 2 columns: [Label, Value]
            dgvFinfo.Columns.Add("colLabel", "Label");
            dgvFinfo.Columns.Add("colValue", "Value");



            foreach (var lbl in labels)
            {
                // We start each Value cell with an empty string
                dgvFinfo.Rows.Add(lbl, "");
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
            dgvFinfo.Columns.Clear();
            dgvFinfo.Rows.Clear();

            // 2. Configure DataGridView (optional customizations as needed)
            dgvFinfo.AllowUserToAddRows = false;
            dgvFinfo.AllowUserToDeleteRows = false;
            dgvFinfo.ReadOnly = true;
            dgvFinfo.RowHeadersVisible = false;

            // 3. Add two columns: "Property" and "Value"
            dgvFinfo.Columns.Add("colProperty", "Property");
            dgvFinfo.Columns.Add("colValue", "Value");

            // 4. Add one row per property
            foreach (var propName in propertyNames)
            {
                // The "Value" column can be initialized empty (or with " ").
                dgvFinfo.Rows.Add(propName, "");
            }

            return true;
        }
        public bool initDataGridViewFinfo()
        {
            // 1) Clear any existing rows/columns from the DataGridView
            dgvFinfo.Columns.Clear();
            dgvFinfo.Rows.Clear();

            // 2) Optionally set up some properties
            dgvFinfo.AllowUserToAddRows = false;
            dgvFinfo.AllowUserToDeleteRows = false;
            dgvFinfo.ReadOnly = true;  // if you want it read-only
            dgvFinfo.RowHeadersVisible = false; // no row headers on the left

            // 3) Add two columns: 
            //    - colLabel  (the property name/label) 
            //    - colValue  (the actual value to display)
            dgvFinfo.Columns.Add("colLabel", "Label");
            dgvFinfo.Columns.Add("colValue", "Value");

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
                dgvFinfo.Rows.Add(label, "");
            }

            return true;
        }

        public void updateDgvFinfo(FileInfoItem iinfo, int idxRow = 0)
        {
            bAutoStartAllow = true;
            int idx = 0;

            // 1) Mirror your existing logic
            setTitle(iinfo, iinfo.dpath);
            tbDescription.Text = iinfo.desc;

            // 2) Assign property values to the second column (index 1) in each row
            try
            {
                dgvFinfo.Rows[idx++].Cells[1].Value = iinfo.fname;        // row 0
                dgvFinfo.Rows[idx++].Cells[1].Value = iinfo.dpath;        // row 1
                lastFolderPath = iinfo.dpath;
                SetFolder(iinfo.dpath);

                dgvFinfo.Rows[idx++].Cells[1].Value = iinfo.fpath;        // row 2
                fpathCurrentImage = iinfo.fpath;

                dgvFinfo.Rows[idx++].Cells[1].Value = iinfo.ext;          // row 3
                dgvFinfo.Rows[idx++].Cells[1].Value = iinfo.stimestamp;   // row 4
                dgvFinfo.Rows[idx++].Cells[1].Value
                    = string.Format("{0:##,###,##0}", iinfo.len);         // row 5

                // 3) Handle width/height from pb1.Image
                if (pb1.Image != null)
                {
                    dgvFinfo.Rows[idx++].Cells[1].Value = pb1.Image.Width.ToString();  // row 6
                    dgvFinfo.Rows[idx++].Cells[1].Value = pb1.Image.Height.ToString(); // row 7
                    iinfo.width = pb1.Image.Width;
                    iinfo.height = pb1.Image.Height;
                }
                else
                {
                    dgvFinfo.Rows[idx++].Cells[1].Value = "0";  // width  (row 6)
                    dgvFinfo.Rows[idx++].Cells[1].Value = "0";  // height (row 7)
                }

                dgvFinfo.Rows[idx++].Cells[1].Value = iinfo.rating;    // row 8
                dgvFinfo.Rows[idx++].Cells[1].Value = iinfo.bDelete;   // row 9
            }
            catch (Exception ex)
            {
                // If your row indexing doesn't match, you might get an IndexOutOfRangeException
                gv.debug.w($"DataGridView row mismatch: {ex.Message}");
                return;
            }

            // 4) bDelete logic
            if (iinfo.bDelete)
                btDelete.BackColor = Color.OrangeRed;
            else
                btDelete.BackColor = Color.LightSalmon;

            // 5) bInvalid
            try
            {
                dgvFinfo.Rows[idx++].Cells[1].Value = iinfo.bInvalid; // row 10
            }
            catch (Exception ex)
            {
                gv.debug.w($"DataGridView row mismatch (bInvalid): {ex.Message}");
            }

            // 6) Background color if invalid
            if (iinfo.bInvalid)
                dgvFinfo.BackColor = Color.LightPink;
            else
                dgvFinfo.BackColor = Color.LightGray;

            // 7) Slide number / counts
            if (gv.nextIdx >= gv.slideCount1)
                gv.nextIdx = gv.slideCount1 - 1;

            tbSlideNumber.Text = string.Format("{0:###,###,##0}", gv.nextIdx);
            tbImageNumber.Text = tbSlideNumber.Text;
            tbMaxSlideNumber.Text = string.Format("{0:###,###,###}", gv.slideCount1 - 1);

            // 8) Additional fields
            tbFpath.Text = iinfo.fpath;

            try
            {
                dgvFinfo.Refresh();   // Force redraw if needed
            }
            catch (Exception e)
            {
                gv.debug.w("update error dgvFinfo: " + e.Message);
            }

            // 9) Some logic related to matching images
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
        }

       



       
        bool bCopyAll2 = false;
        public void CopyAllExec()
        {
            gv.maxCountCopy = (int)numCopyBatchNumber.Value;
            if (string.IsNullOrEmpty(tbTargetBatchCopy.Text))
                return;
            char targetDir = char.Parse(tbTargetBatchCopy.Text.Substring(0, 1));

            while (cbCopyAll.Checked)
            {
                bool rc = CopyAll2(targetDir);
                if (!rc)
                    return;
            }
        }
        public bool CopyAll2(char targetd)
        {
            bool rc = copyOrMoveImage(targetd);
            if (rc != true)
            {
                return false;
            }
            showNextSlideNow();
            if (gv.nextIdx >= (gv.slideCount1 - 1))
            {
                cbCopyAll.Checked = false;
                cbCatalogImages.Checked = false;
            }
            else
            {
                ++CountCopies;
                if (CountCopies > gv.maxCountCopy)
                {
                    CountCopies = 0;
                    if (DisplayContinuePrompt())
                    {
                        ForceGarbageCollection();
                        gv.debug.Close();
                        gv.debug.Dispose();
                        gv.debug = null;
                        gv.debug = new DebugWindow(gv);
                    }
                    else
                    {
                        ForceGarbageCollection();
                        cbCopyAll.Checked = false;
                        cbCatalogImages.Checked = false;
                        stopSlideShow();
                        return false;
                    }
                }
                //if (tbTargetBatchCopy.Text.Equals("A"))
            }
            return rc;
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

            Bitmap result = new Bitmap(width, height);

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
            startSoundPlayer.Play();
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

        public void ClearAllPictureBoxesInUse()
        {
            pbMatch.Image?.Dispose();
            pbNext.Image?.Dispose();
           // pb1.Image = null;
            pb1ThumbNail.Image?.Dispose();
            pbLastImage.Image?.Dispose();
            pbSecondLast.Image?.Dispose();
            pbThirdLast.Image?.Dispose();
            pbFourthLast.Image?.Dispose();
            pbMatch.Image = null;
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
            if (gv.dialogTraverser == null || gv.dialogTraverser.IsDisposed)
            {
                gv.dialogTraverser = new TraverserDialog(gv, this, 0, ff);
                gv.dialogTraverser.Visible = true;
            }
            else
            {
                gv.dialogTraverser.WindowState = FormWindowState.Normal;
                gv.dialogTraverser.Activate();
            }
            //gv.setCursorHourGlass();
            loadedPreviewListNumber = -1;
            //gv.imageFileList
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


        public void displayMRLinListBox()
        {
            FileInfoItem fi;
            mylistBox.Items.Clear();

            for (int idx = 0; idx < mostRecentImages.getImageCount(); ++idx)
            {
                fi = mostRecentImages.getIndexed(idx);
                mylistBox.Items.Add($"{fi.level} {fi.index:0000} {fi.len} {fi.dpath} ");
            }

        }
        string lastImage;
        Bitmap mainImage = null;
        Bitmap bmp;
        Bitmap bmpPrevious;
        bool bFindDupByFileSize = false;
        public bool LoadImage(string fpath, int pbNumber, FileInfoItem fi = null)
        {
            bool loadedOK = true;
            lastImage = fpath;
            switch (pbNumber)
            {
                case 1: //current pb1
                    if (cbDisplayOnThisDisplay.Checked)
                    {
                        mainImage = BitmapFromSource(LoadImage(fpath));
                        pb1.Image = mainImage;
                        //  bmp = new Bitmap(fpath);
                        //  bmpPrevious = bmp;
                        //   pb1.Image = bmp;
                        //MRL will save if it is next in order
                        tbFileSize.Text = $"{fi.len}";
                        tbP1Size.Text = $"{fi.len}";
                    }
                    mostRecentImages.addItemMRL(fi, gv.nextIdx);
                    displayPreviousImagesInfo();
                    displayMRLinListBox();
                    if (bFindDupByFileSize)
                        CompareByFileSize();
                    //2024 Compare3Images();
                    break;
                case 0: //next (preview)
                    System.Drawing.Image old = pbNext.Image;
                    try
                    {
                        bmp = new Bitmap(fpath);
                    }
                    catch
                    {
                        //System.Windows.MessageBox.Show("Case 0 bmp=", "EXCEPTION");
                        bool bcontinue = DisplayContinueOrAbort(gv.nextIdx);
                        return false;
                    }
                    if (idxOfNextImage != fi.index)
                    {
                        System.Windows.Forms.MessageBox.Show($"idx {idxOfNextImage} != {fi.index}", "preview image error");
                    }
                    int t2 = fi.index;
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

            System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chord.wav");
            gv.nextIdx = 0;

            startSlideShow(0, false, 1); // NO TIMER .. start at idx = 0 

        }
        bool bMasterStopSlideShow = false;
        public bool setSlideShowOn(bool bOn)
        {
            if (bMasterStopSlideShow)
                bOn = false;
            gv.bSlideShow = bOn;
            cbSlideShow.Checked = bOn;
            return gv.bSlideShow;
        }
        public void stopSlideShow(string msg = null)
        {
           // mostRecentImages.clearList();
            tbMessageFromMatching.Text = msg;
            ForceGarbageCollection();
            // sound(30);
            gv.bSlideShow = false;
            SetTimerOn(false);
            timer.Stop();
            Cursor.Show();
            bScan = false;
            setSlideShowOn(false);
            //  this.pb1.Image = null;
            //  this.pb1.Refresh();
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
            if (!bMasterStopSlideShow)
                timer.Start();                              // Start the timer
            
            //  label.Location = new Point(100, 100);
            //  label.AutoSize = true;
            //  label.Text = String.Empty;

            //   this.Controls.Add(label);
        }
        public void MasterStopSlideShow()
        {
            bMasterStopSlideShow = true;
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
            if (!bMasterStopSlideShow)
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
        public int showPreviousSlide()
        {
            int currentSlide = gv.nextIdx - 1;
            int slideIdx = 0;
            Boolean rc = false;
            this.stopSlideShow();
            if (gv.nextIdx > 0)
            {
                //dmc9 gv.nextIdx--;
                slideIdx = showNextSlide3(gv.nextIdx, -1);
            }
            if (slideIdx < currentSlide)
            {
                System.Windows.Forms.MessageBox.Show("ERROR", $"slide < too far {currentSlide} but {slideIdx}");
                return slideIdx;
            }
            return 0;
        }
        DateTime dt;

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
                                return false;
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
                    LoadImage(fpath, 1, finfo1); /////exception thrown if invalid/corrupted IMAGE ///////////////////////
                }
                catch (Exception)
                {
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
        public double ShowNextList3AndCompare()
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
            try
            {
                LoadImage(finfo3.fpath, 2, finfo3); /////exception thrown if invalid/corrupted IMAGE ///////////////////////
            }
            catch (Exception)
            {
                gv.debug.w("XXXXXXXXXXXXXXXX  FILE LOAD ERR fname: ", finfo3.fpath, " ", gv.imageFileList1.getIndexed(idx).fname);
                return -1;
            }
            double rc = CompareImagesOnList();
            tbMatchPercent.Text = $"<rc:N2>";
            if (rc < 20)
                tbFilePath3.Text = finfo3.fpath;
            return rc;
        }


        private void SlideShow_Click_1(object sender, EventArgs e)
        {
            gv.setCursorHourGlass();

            System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chord.wav");
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
        public void soundAlert(int soundNumberDmc)
        {
            if (soundOff)
                return;
            if (player == null)
            {
                player = new System.Media.SoundPlayer(gv.mainWindow.soundFiles[41]);
            }
            switch (soundNumberDmc)
            {
                case 1:
                    {
                        player.SoundLocation = gv.soundHighGongPath;
                        break;
                    }
                case 2:
                    {
                        player.SoundLocation = gv.soundHighAlertPath;
                        break;
                    }
                case 3:
                    {
                        player.SoundLocation = gv.soundLowAlertPath;
                        break;
                    }
                case 4:
                    {
                        player.SoundLocation = gv.soundLowGongPath;
                        break;
                    }
                case 5://ding
                    {
                        player.SoundLocation = gv.soundDefault;
                        break;
                    }
                case 6://alert
                    {
                        player.SoundLocation = gv.soundAlert;
                        break;
                    }
                case 35:
                    {
                        player.SoundLocation = @"C:\Windows\Media\Windows Hardware Fail.wav";
                        break;
                    }
                case 46:
                    {
                        player.SoundLocation = @"C:\Windows\Media\Windows Hardware Fail.wav";
                        break;
                    }
                case 49://alert
                    {
                        player.SoundLocation = @"C:\Windows\Media\Windows Information Bar.wav";
                        break;
                    }
                case 55://alert
                    {
                        player.SoundLocation = @"C:\Windows\Media\Windows Navigation Start.wav";
                        break;
                    }

                // 30   =  C:\Windows\Media\Speech Misrecognition.wav
                default:
                    {
                        player.SoundLocation = gv.soundDefault;
                        break;
                    }
            }
            player.Load();
            player.Play();
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
                display1.setDisplayMonitor(gv.screen[0], 1, display1.Name);

            if (bScreen2)
                display2.setDisplayMonitor(gv.screen[1], 2, Name);
            if (bScreen3)
            {
                display3.setDisplayMonitor(gv.screen[2], 3, Name);

            }
            if (bScreen4)
            {
                display4.setDisplayMonitor(gv.screen[2], 3, Name);

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
            if (bMasterStopSlideShow)
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
            if (bMasterStopSlideShow)
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

            } else if (timer2Counter > 5)
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
                gv.nextIdx = gv.slideCount1 - 1;
            }
            else
            {
                gv.nextIdx = idx;
            }
            if (gv.nextIdx >= gv.slideCount1)
                return 0;
            gv.debug.w($">>>>>>>>>>>>>>>>>>>>>showNextSlide>>>{gv.nextIdx} << of MAX {gv.slideCount1}");
            //
            // dmcReadImage
            // dmcMetadata
            //
            idxRC = showNextImageFromList(gv.nextIdx, iDirection);
            //
            //updateLvFinfo(finfo1, idxRC); //show next
            tbFpath.Text = finfo1.fpath;
            updateDgvFinfo(finfo1, idxRC);
            //
            if (bLoadFirstImage)
            {
                LoadImage(finfo1.fpath, 1, finfo1);
            }
            //xxxxxx
            //Image tmpImg = pb1.Image;
            //pb1.Image = pbTest.Image;
            //picImages.Image = Image.FromFile(dir + "\\" + curitem);

            //thumbnail 
            // finfoItem = gv.imageFileList.getIndexed(idxRC);
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
            //this.updateLvFinfo(iinfo2);
            //setTitle(iinfo2, null);
            gv.lastDisplayedIdx = idxRC;
            displayThisImage2Screens();
            if (cbPreview.Checked)
                previewNextImage(iDirection);
            this.Refresh();

            if (bSyncPreviewDisplay)
            {
                if (iDirection == 1 || iDirection == -1)
                {
                    if (idx != 0)
                        syncPreviewDisplay();
                }
            }
            long prevFileSize = mostRecentImages.getIndexed(1).len;
            //
            //
            /*
            if (finfo1.len == prevFileSize && finfo1.len > 0)
            {
                if (cbFindDuplicates.Checked)
                {
                    double match = CompareImages2((Bitmap)pb1.Image, (Bitmap)pbNext.Image); //pb1 pbNext
                    tbMatchPercent.Text = match.ToString();
                    if (match > 80)
                    {
                        stopSlideShow();
                        System.Windows.Forms.MessageBox.Show("image and prev image match", "MATCH");
                    }
                }
            }
            */
            //
            if (finfo1.len == nextFileSize && finfo1.len > 0)
            {
                if (cbFindDuplicates.Checked)
                {
                    double match = CompareImages2((Bitmap)pb1.Image, (Bitmap)pbNext.Image); //pb1 pbNext
                    tbMatchPercent.Text = match.ToString();
                    if (match > 90)
                    {
                        stopSlideShow();
                        DialogResult result = System.Windows.Forms.MessageBox.Show("Mark next for Deletion?", "This image and next image match", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            MarkNextForDeletion();
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
            fi.index = rcIDX;
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
                loadedImage = LoadImage(fi.fpath, 1, fi);   //dmc 2025
            }
            catch (Exception ex)
            {
                this.Text = $"Exception pb1.Load {ex.Message}";
            }
            return rcIDX;
            //
            //
        }
        public void keepPreviousImage(int iDirection)
        {
            pbFourthLast.Image = pbThirdLast.Image;
            pbThirdLast.Image = pbSecondLast.Image;
            pbSecondLast.Image = pbLastImage.Image;
            pbLastImage.Image = pb1.Image;
        }
        public void displayPreviousImagesInfo()
        {

            tbFourthLastSlideNumber.Text = mostRecentImages.getIndexed(4).index.ToString();
            tbThirdLastSlideNumber.Text = mostRecentImages.getIndexed(3).index.ToString();
            tbSecondlastSlideNumber.Text = mostRecentImages.getIndexed(2).index.ToString();

            tbSlideNumberOfLast.Text = mostRecentImages.getIndexed(1).index.ToString();

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

            while (!bGotImage)
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
                bool bContinue = true;
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
            var result = System.Windows.Forms.MessageBox.Show("Do you want to continue (YES) or stop (NO)?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                // User chose “Continue”
                gv.nextIdx = idx - 2;
                return true;
            }
            else
            {
                // User chose “Abort”
                stopSlideShow();
                if (!bCorrectedLastNbr)
                {
                    bCorrectedLastNbr = true;
                    gv.iMaxFileCount = ndx;
                    gv.slideCount1 = gv.iMaxFileCount;
                    tbMaxSlideNumber.Text = gv.iMaxFileCount.ToString();
                    gv.nextIdx = ndx - 4;
                    showNextImageFromList(gv.nextIdx, gv.iMaxFileCount);
                    return false;
                }
            }
            return false;
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
        public void previewNextImage(int iDirection)
        {
            string fpath2;
            
            bool bOk = false;
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
            iinfoNextImage.index = idxOfNextImage;
            //
            //
            tbPreviewIndex.Text = idxOfNextImage.ToString();
            fpath2 = iinfoNextImage.fpath;
            loadedPreviewListNumber = idxOfNextImage;
            tagNextImageName.Text = iinfoNextImage.fname;
            bool ok = false;
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

            try
            {
                LoadImage(fpath2, 0, iinfoNextImage); /////exception thrown if invalid/corrupted IMAGE ///////////////////////
                //   pb2.SizeMode = PictureBoxSizeMode.Zoom;
                //   pb2.Update();
            }
            catch (Exception)
            {
                tagNextImageName.BackColor = Color.Red;
                gv.debug.w("---LOAD FAILED! >> INVALID FILE in PreviewNextImage pbNext.Load:", fpath2);
                pbNext.Image = null;
                gv.imageFileList1.markRating("@", idxOfNextImage);
                //gv.imageFileList.markInvalid(true, idx);
                loadedPreviewListNumber = -1;
            }
            if (!bUseBitmapLoad)
                LoadImage(fpath2, 0, iinfoNextImage);
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
            string targetDir = gv.dirSaveRootFolder + ch;
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

        private void btText_Click(object sender, EventArgs e)
        {
            DisplayListView test = new DisplayListView(gv);
            test.Visible = true;

            PromptRTF getdir = new PromptRTF();
            getdir.Visible = true;

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
                gv.initParm1List[0].dirPath = gv.default_init_Path;
                gv.initParm1List[0].fpath = gv.inifileFullPathName;
                if (string.IsNullOrEmpty(gv.initParm1List[0].sourceDir1))
                    gv.initParm1List[0].sourceDir1 = gv.lastTraversedFolder;
                gv.initParm1List[0].SetTargetDir1(gv.initParm1List[0].targetDir2);
            }
            return gv.initParm1List.Count;
        }
        public InitParms1 assignGlobalInitVars(InitParms1 initvars)
        {
            gv.default_init_Path = initvars.dirPath;
            gv.inifileFullPathName = initvars.fpath;
            gv.lastTraversedFolder = initvars.mostRecent;
            //gv.dirTarget4Copy = initvars.targetDir;
            return initvars;
        }
        //
        // 2025
        //
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
            string initFilePath = System.IO.Path.Combine(folder, "sdb5.xml");

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
            string initFilePath = System.IO.Path.Combine(folder, "sdb5.xml");

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
                ip.dirPath = gv.default_init_Path;//System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                ip.fpath = gv.inifileFullPathName;
                //= tbTargetFolder.Text;
                ip.SetTargetDir1(gv.initParm1List[0].targetDir1);
                if (gv.initParm1List == null)
                    gv.initParm1List = new List<InitParms1>();
                gv.initParm1List.Add(ip);
            }
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

            TraverserDialog test = new TraverserDialog(gv, this, 0, ff);
            test.Visible = true;
            //  test.traverse("C:\\testdir");
            //test.traverse("D:\\Documents and Settings\\mcclandx\\My Documents\\DavidsPictures");

        }

        //startSlideShow(int startSlideNumber, Boolean autoStart, int direction)
        private void btSlideShow_Click(object sender, EventArgs e)
        {
            soundAlert(58);
            if (!gv.FILE_TYPE.Equals("images"))
                return;
            bStartedSlideShowScan = true;
            //
            cbSlideShow.Checked = !cbSlideShow.Checked;
            //
            if (gv.bSlideShow)
            {
                stopSlideShow();
                return;
            }
            else
            {
                
                
            }
            cbSlideShow.Checked = !cbSlideShow.Checked;

            dgvFinfo.Show();
            this.pb1.Image = null;
            this.Refresh();
            
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
                    updateDgvFinfo(finfo1, rcIDX);
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
        private void btDisplay1_Click(object sender, EventArgs e)
        {
            if (!gv.FILE_TYPE.Equals("images"))
                return;
            lastCountLoaded = gv.slideCount1;
            activateImageButtons();
            cbDisplayInfomation.Checked = false;
            dgvFinfo.Show();
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
            else if (keyData == Keys.Decimal)
            {
                markDeleted();
                gv.debug.w("ShowNextSlide2 in Main  ProcessCmdKey MARK DELETED Keys.Decimal");

                // showNextSlide2(-1, 1);
                return true;
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

        public void displayPreviewSet()
        {
            if (displayPreviewSetForm == null || displayPreviewSetForm.IsDisposed)
                displayPreviewSetForm = new DisplayPreviewSet(gv, gv.nextIdx);
            else
                displayPreviewSetForm.Activate();
            // ds.WindowState = FormWindowState.Normal;
            //  ds.Location = new Point(gv.screen[1].Bounds.Location.X, gv.screen[1].Bounds.Location.Y);
        }
        private void buttonDisplaySet_Click(object sender, EventArgs e)
        {
            displayPreviewSet();
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
        private void cbDisplay1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplay1.Checked)
            {
                bScreen1 = true;
                if (display1 == null || display1.IsDisposed)
                {
                    display1 = new Display1(gv);
                    display1.setDisplayMonitor(gv.screen[0], 1, display1.Name, win[0]);
                }
                display1.Show();
                display1.displayThisImage(bmp);
            }
            else
            {
                bScreen1 = false;
                display1.Dispose();
                // gv.debug.setDisplayMainFocus(true);
            }

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

        private void cbDisplay2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplay2.Checked)
            {
                if (bAllowScreen2)
                {
                    bScreen2 = true;
                    if (display2 == null || display2.IsDisposed)
                    {
                        display2 = new Display1(gv);
                        display2.setDisplayMonitor(gv.screen[1], 2, display2.Name, win[1]);
                       
                       // win[1].YourLocation(win[1].Location);
                    }
                    display2.Show();
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
                    if (display3 == null || display3.IsDisposed)
                    {
                        display3 = new Display1(gv);
                        display3.setDisplayMonitor(gv.screen[2], 3, display3.Name, win[2]);
                    }


                    display3.Show();
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
                    if (display4 == null || display4.IsDisposed)
                    {
                        display4 = new Display1(gv);
                        display4.setDisplayMonitor(gv.screen[3], 4, display4.Name, win[3]);
                    }


                    display4.Show();
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
        public  void btShowDecription(object sender, EventArgs e)
        {
            cbShowDisplayNames.Checked = false;
            lvScreenInfo.SendToBack();
            parms = new DialogParms(gv);
            parms.Visible = true;
            parms.Activate();
        }

        DialogParms parms;
        private void btShowParms_Click(object sender, EventArgs e)
        {
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
            sound(30);
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
            finfo1.bDelete = true;
            updateLvFinfo(finfo1); //mark del
            ++countMarkedForDeletion;
            tbMarkedForDeletion.Text = countMarkedForDeletion.ToString();
        }
        public void InsertSourceComment(string src)
        {
            finfo1.desc = src;
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
        public void markNextForDeletion2(bool delete = true)
        {
            btDeleteMarked.BackColor = Color.OrangeRed;
            // gv.nextIdxxxxx
            iinfoNextImage.bDelete = true;
            updateLvFinfo(finfo1); //mark del
            ++countMarkedForDeletion;
            tbMarkedForDeletion.Text = countMarkedForDeletion.ToString();
        }
        public bool markDeleted()
        {
            bool bSet = !finfo1.bDelete;
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
            return bSet;

        }
        public string[] soundFiles = null;
        // @"Data Source=C:\Users\david\Documents\dzenSDB\SDB01.sdf;Persist Security Info=True";


        static int idxSound = 0;

        private void sound(int idxSound)
        {
            // player = new System.Media.SoundPlayer(gv.main.soundFiles[idx]);
            player.SoundLocation = soundFiles[idxSound];
            //tbSoundFile.Text = soundFiles[idxSound];
            //player.Load();
            player.Play();
            //++idxSound;
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

        private void button3_Click(object sender, EventArgs e)
        {
            WebBrowser wb1 = new WebBrowser();

            wb1.WebBrowser2(gv, this);
            wb1.Visible = true;
            wb1.Activate();
            wb1.Show();


            //    doc = (IHTMLDocument2)wb1.Document.DomDocument;
            //    imgRange = (IHTMLControlRange)((HTMLBody)doc.body).createControlRange();

        }

        private void btSaveInitParms_Click(object sender, EventArgs e)
        {
            //ff.saveInitParmsList();
            this.Text = gv.initParm1List[0].mostRecent;
            ff.saveInitParmsListToDefault(gv.initParm1List);
            SaveHistoryList();
            System.Windows.Forms.Application.Exit();
        }

        ShowDisplay[] win = new ShowDisplay[4];

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
            //if gv.screen[1].ac
            System.Drawing.Rectangle recDisplay1 = gv.screen[1].Bounds;
            System.Drawing.Point p = new System.Drawing.Point(recDisplay1.X, recDisplay1.Y);
            int winnumber = 0;
            //int idx = winnumber - 1;
            for (int idx = 0; idx < gv.displayCount; ++idx)
            {

                if (bShow)
                {
                    winnumber = idx + 1;

                    win[idx] = new ShowDisplay(idx + 1, gv.screen[idx].Bounds.Width, gv.screen[idx].Bounds.Height);

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

        private void cbShowDisplayNames_CheckedChanged(object sender, EventArgs e)
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
        List<FolderHistory> DeserializeFromXML2(string fpath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<FolderHistory>));
            TextReader textReader = new StreamReader(fpath);
            List<FolderHistory> folderHistory;
            folderHistory = (List<FolderHistory>)deserializer.Deserialize(textReader);
            textReader.Close();

            return folderHistory;
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

                bool brc = System.IO.File.Exists(gv.folderHistoryFileFullPathName);
                if (!brc)
                {
                    System.Windows.Forms.MessageBox.Show($"NO History File {gv.folderHistoryFileFullPathName}");

                    ff.createDirectory(gv.dazenMainFolder);
                    brc = ff.directoryExists(gv.dazenMainFolder);
                    if (!brc)
                    {
                        System.Windows.Forms.MessageBox.Show($"Create the Folder {gv.dazenMainFolder} and rerun the SDB4");
                        return 0;
                    }
                    System.Windows.Forms.MessageBox.Show($"SDB4 Created folder {gv.dazenMainFolder}");
                    SaveHistoryList();
                    brc = System.IO.File.Exists(gv.folderHistoryFileFullPathName);
                    if (!brc)
                    {
                        System.Windows.Forms.MessageBox.Show($"NO History File {gv.folderHistoryFileFullPathName}");
                        return 0;
                    }
                }
                gv.folderHistoryList = DeserializeFromXML2(gv.folderHistoryFileFullPathName);
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
                System.Windows.MessageBox.Show("Folder does not exist.", "ERROR TARGET FOLDER");
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
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment cd =
                System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                string publishVersion = cd.CurrentVersion.ToString();
                return publishVersion;
            }
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

        public void startWatcher()
        {
            tbMessage.Text = "watching";

            if (fileWatcher == null)
            {
                fileWatcher = new Watcher(gv, null, null);
            }
            // 2017 in for the folder monitor
            /*
            Point lpoint = new Point(0, 0);
            pb1.Location = lpoint;
            pb1.Height = this.Height;
            pb1.Width = this.Width;
            */
            // end of test code 2017
        }
        public Boolean displayThisImage1(string fpath) //// DISPLAY IMAGE after LOADING <<<<<<<<<<<<<<<<<< MAIN CALL TO DISPLAY THIS IMAGE << LOAD AND DISPLAY FPATH image
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
                LoadImage(fpath, 1);//openFileDialog1.FileName);
            }
            catch (Exception)
            {
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
                    gv.watchFolderPath = gv.initParm1List[0].watchFolder1;
                    break;
                case 2:
                    gv.watchFolderPath = gv.initParm1List[0].watchFolder2;
                    break;
                case 3:
                    gv.watchFolderPath = gv.initParm1List[0].watchFolder3;
                    break;

            }
            tbWatch.Text = gv.watchFolderPath;
        }

        private void cbSlideShow_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSlideShow.Checked)
                activateImageButtons();
            //   else
            //    stopSlideShow();
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
            CopyAllExec();
        }

        private void btGoToSlideNumber_Click(object sender, EventArgs e)
        {
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
                btDeleteAndResume.BackColor = Color.LightGray;
                btDeleteAndResume.Refresh();
                cbAllowDelete.Checked = true;
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
                tbImageNumber.Text = gv.slideCount1.ToString();
            }
        }

        private void cbShowTravWin_CheckedChanged(object sender, EventArgs e)
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                gv.dialogTraverser2.Activate();
        }

        private void Main_Activated(object sender, EventArgs e)
        {
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
            double match = 100 - (Math.Round(unmatchPercent, 2));

            tbWatch.Text = unmatchPercent.ToString();
            if (match > 80) //a match STOP
            {
                tbWatch.BackColor = Color.LightGreen;

                if (cbDeleteAndResume.Checked)
                {
                    DisplayAutoDeletePrompt();
                }
                else
                {
                    if (!cbContinueAfterDelete.Checked)
                        stopSlideShow();
                }
            }
            else
            {
                tbWatch.BackColor = Color.LightGray;
            }
            return match;
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
            if (unmatchPercent < 20) //a match STOP
            {
            }
            double match = 100 - (Math.Round(unmatchPercent, 2));
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
            int idx = Convert.ToInt32(tbSlideNumber.Text.Replace(",", ""));
            if (idx != gv.nextIdx)
            {
            }
            FileInfoItem fi = gv.imageFileList1.getIndexed(idx);
            fi.bDelete = false;
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
                idx3 = 0;
            btIdx3Next.Text = idx3.ToString();
        }

        private void btIdx3Show_Click(object sender, EventArgs e)
        {
            bool rc = CompareNext3();
            if (rc & cbDisplay1idx3.Checked)
                DisplayIdx3OnDisplay4();
        }
        public bool CompareNext3()
        {
            bool match = false;
            tbResultIdx3Compare.Text = "";
            double result = 0;
            do
            {
                result = ShowNextList3AndCompare();
                tbResultIdx3Compare.Text = result.ToString();
                if (result < 0)
                    return false;
                if (result > 20)
                {
                    NextIdx3();
                    if (idx3 <= 0)
                    {
                        ResetIdx3Search();
                        tbResultIdx3Compare.Text = "no";
                        return false;
                    }
                }
            } while (result > 20);
            return true;
        }

        private void btNextAndCompare_Click(object sender, EventArgs e)
        {
            ResetIdx3Search();
            showNextSlideNow();
            bool rc = CompareNext3();
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
                    display4.setDisplayMonitor(gv.screen[3], 4, display4.Name);
                }
                display4.Show();
                display4.displayThisImage((Bitmap)pb2Image.Image);
            }
        }
        public Bitmap BitmapFromSource(System.Windows.Media.Imaging.BitmapSource bitmapsource)
        {
            //convert image format
            var src = new System.Windows.Media.Imaging.FormatConvertedBitmap();
            src.BeginInit();
            src.Source = bitmapsource;
            src.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            src.EndInit();

            //copy to bitmap
            Bitmap bitmap = new Bitmap(src.PixelWidth, src.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var data = bitmap.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            src.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        System.Windows.Media.Imaging.BitmapImage bitmap1;
        public System.Windows.Media.Imaging.BitmapImage LoadImage(string imageFilePath)
        {
            bitmap1 = new BitmapImage();
            var stream = File.OpenRead(imageFilePath);

            bitmap1.BeginInit();
            bitmap1.CacheOption = BitmapCacheOption.OnLoad;
            bitmap1.StreamSource = stream;
            bitmap1.EndInit();
            stream.Close();
            stream.Dispose();
            return bitmap1;
        }
        System.Windows.Media.Imaging.BitmapImage bitmapSource;
        public System.Windows.Media.Imaging.BitmapImage LoadImageSource(string imageFilePath)
        {
            bitmapSource = new BitmapImage();
            var stream = File.OpenRead(imageFilePath);

            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.OnLoad;
            bitmapSource.StreamSource = stream;
            bitmapSource.EndInit();
            stream.Close();
            stream.Dispose();
            return bitmapSource;
        }
        string fileNameBase = "SDB";
        string fileExt = ".txt";

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
            cbThumbNailONLY.Checked = true;
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

        }

        private void cbSlideShowOff_CheckedChanged(object sender, EventArgs e)
        {
            bMasterStopSlideShow = !cbSlideShowOff.Checked; 
            if (!cbSlideShowOff.Checked ) 
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
            tbTargetFolder_TextChanged(sender, e);
        }
        int idxSource = 0;
        int numberMatching = 0;
        Bitmap otherImage = null;
        private void btFindMatchingImage2_Click(object sender, EventArgs e)
        {
            double rc = 0;
            while (idxSource < gv.imageFileListCompare.getImageCount() - 2)
            {
                FileInfoItem fi;
                fi = gv.imageFileListCompare.getIndexed(idxSource);
                pbMatch.Image = BitmapFromSource(LoadImage(fi.fpath));
              //  rc = CompareImages2(otherImage, mainImage);
                rc = CompareImages2((Bitmap)pb1.Image, (Bitmap) pbMatch.Image);
                if (rc > 80)
                {
                    ++numberMatching;
                }
                ++idxSource;
               // tbImage3List.Text = idxSource.ToString();
            }
            return;
        }
        bool bOptimizedSearch = true;
        private void btFindMatchingImageReset_Click(object sender, EventArgs e)
        {
            idxSource = 0;
            numberMatching = 0;
            pbMatch.Image = null;
            btFindMatchingImage_Click(sender, e);
        }

        //
        private CancellationTokenSource _cts;
        private ImageMatcher _matcher = new ImageMatcher();
        private List<string> _matches = new List<string>();

        private string _lastSearchFolder;
        private async void btFindMatchingImage_Click(object sender, EventArgs e)
        {
            if (pb1.Image == null)
            {
                System.Windows.Forms.MessageBox.Show("Please load a source image first.");
                return;
            }

            // ask user for folder to search
            string searchFolder;
            if (cbUseSameFolder.Checked && !string.IsNullOrEmpty(_lastSearchFolder))
            {
                searchFolder = _lastSearchFolder;
            }
            else
            {
                using var dlg = new FolderBrowserDialog();
                if (dlg.ShowDialog() != DialogResult.OK) return;

                searchFolder = dlg.SelectedPath;
                _lastSearchFolder = searchFolder;
                cbUseSameFolder.Checked = true;
            }
            tbSourceImageNumber.Text = searchFolder;
            // ← HERE'S THE FIX: always initialize your CTS before using .Token
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            btFindMatchingImage.Enabled = false;
            btDeleteMatchingImages.Enabled = false;
            lbMatches.Items.Clear();
            lblStatus.Text = "Searching…";

            // marshal back to UI for each progress update
            _matcher.ProgressCallback = (processed, found) =>
            {
                if (lblStatus.InvokeRequired)
                {
                    lblStatus.BeginInvoke(new Action(() =>
                    {
                        lblStatus.Text =
                            $"Compared {processed:N0} files, found {found:N0} matches…";
                    }));
                }
                else
                {
                    lblStatus.Text =
                        $"Compared {processed:N0} files, found {found:N0} matches…";
                }
            };
            List<string> results;
            try
            {
                // kick off the search on a thread-pool thread
                results = await Task.Run(() =>
                    _matcher.FindMatches(
                        mainImage: new Bitmap(pb1.Image),
                        rootDirectory: searchFolder,
                        sourceImagePath: finfo1.fpath,
                        threshold: 0.98,
                        cancellationToken: _cts.Token
                    ), _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // (should never happen now, because FindMatches no longer throws)
                lblStatus.Text = "Cancelled";
                btFindMatchingImage.Enabled = true;
                btCancelFind.Enabled = false;
                return;
            }
            // normalize the source path
            string sourceNormalized = System.IO.Path
                .GetFullPath(finfo1.fpath)
                .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            // remove any result whose canonical path == source
            results.RemoveAll(m =>
            {
                string mnorm = System.IO.Path
                    .GetFullPath(m)
                    .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
                return string.Equals(mnorm, sourceNormalized, StringComparison.OrdinalIgnoreCase);
            });

            // populate UI
            lbMatches.Items.AddRange(results.ToArray());
            lblStatus.Text = $"Done: {results.Count:N0} matches.";

            btFindMatchingImage.Enabled = true;
            btDeleteMatchingImages.Enabled = results.Count > 0;

            // ◦◦◦ THIS LINE ◦◦◦
            _matches = results;

        }


        private void btDeleteMatchingImages_Click(object sender, EventArgs e)
        {
            if (_matches == null || _matches.Count == 0)
            {
                System.Windows.MessageBox.Show("No matches to delete.");
                return;
            }

            var result = System.Windows.Forms.MessageBox.Show(
                $"Are you sure you want to permanently delete {_matches.Count} files?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;
            ClearAllPictureBoxesInUse();

            _matcher.DeleteMatches(_matches);

            // delete items from the  ImageFileList  gv.imageFileList1
            int removedCount = 0;
            // copy them into a string list so we can clear the ListBox as we go
            var toDelete = lbMatches.Items
                .Cast<string>()
                .ToList();

            foreach (var fullpath in toDelete)
            {
                // this will normalize and remove the entry if it exists
                if (gv.imageFileList1.FindItemAndDelete(fullpath))
                    removedCount++;
            }
            string slideNumber = tbImageNumber.Text;
            showNextSlideImageFileList1(0, 1);
            lblStatus.Text = "deleted";
            //System.Windows.MessageBox.Show("Files deleted.");
            _matches.Clear();
            lbMatches.Items.Clear();
            btDeleteMatchingImages.Enabled = false;
            tbGoToSlide.Text = slideNumber;
            gv.slideCount1 = gv.imageFileList1.getImageCount();
        }

        private void btCancelFind_Click(object sender, EventArgs e)
        {
            btCancelFind.Enabled = false;
            _cts?.Cancel();
        }

        private void btClearAllPictureBoxesUse_Click(object sender, EventArgs e)
        {
            ClearAllPictureBoxesInUse();
            reposition();
        }

        private void lbMatches_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // make sure something is selected
            if (lbMatches.SelectedItem is string path && File.Exists(path))
            {
                try
                {
                    // dispose the old image if any
                    pbMatch.Image?.Dispose();

                    // load new
                    pbMatch.Image = new Bitmap(path);

                    // update any UI or state you like
                    
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Unable to load image:\n{ex.Message}",
                                    "Load Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }

        private void lbMatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbMatches_MouseDoubleClick(null, null);
            var lb = (ListBox)sender;
            int idx = lb.SelectedIndex;
        }

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

        private void tbRowCount_TextChanged(object sender, EventArgs e)
        {
            tbSourceImageNumber.Text = tbImageNumber.Text;
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
            bMasterStopSlideShow = true;
            timer.Stop();
            timer.Enabled = false;
            cbMasterOFF.Checked = true;
        }
        public void MasterOn()
        {
            bMasterStopSlideShow = false;
            timer.Enabled = true;
            cbMasterOFF.Checked = false;
        }
        private void btResetTimerSlideShow_Click(object sender, EventArgs e)
        {
            MasterOn();
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            Close();
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
    }
}// CLASS MAIN


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
 Adding the Windows Media Player Control
Before creating a new project, make sure that the latest version of Windows Media Player and the Windows Media Player SDK is installed on your computer.
Start Visual Studio, then create a new project.
In Visual Studio, open the Toolbox.
>>>>>>> If Windows Media Player does not appear in the Components portion of the Toolbox, do the following:
Right-click within the Toolbox, and then select Choose Items. This opens the Customize Toolbox dialog box.
On the COM Components tab, select Windows Media Player.
If Windows Media Player does not appear in the list, click Browse, and then open Wmp.dll, which should be in the Windows\System32 folder.
Click OK. The Windows Media Player control will be placed on the current Toolbox tab.

You can now select Windows Media Player in the Toolbox and add it to a form.
Visual Studio gives the Windows Media Player control a default name such as "axWindowsMediaPlayer1". You may want to change the name to something more easily remembered, such as "Player".
Adding the Windows Media Player control from the Toolbox also adds references to two libraries created by Visual Studio, AxWMPLib and WMPLib. You can find them in the Solution Explorer under References.
To make using the objects in the Player namespace easier, you should include the namespace in the using or imports directives of your files, as follows:

Csharp

Copy
using WMPLib;
*/