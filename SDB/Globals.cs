using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.DirectoryServices;

namespace SymbolDB
{
    public class InitParms
    {
        public string parmName;
        public string parmValue;

        public InitParms(string n, string v)
        {
            parmName = n;
            parmValue = v;
        }
    }

    public class ARGS
    {
        public string dirpath { get; set; }
        public bool bPictures { get; set; }
        public bool bMovies { get; set; }
        public bool bWEBM { get; set; }
        public bool bMIDI { get; set; }
        public bool bALL { get; set; }
    }

    public class FolderHistory
    {
        public string folderPath;
        public string action;
        public string desc;

        public FolderHistory()
        {
            folderPath = null;
            action = null;
        }
        public FolderHistory(string n, string v)
        {
            folderPath = n;
            action = v;
        }
    }
    public class FolderAssignment
    {
        public string name { get; set; }
        public string folderPath { get; set; }
        public string desc { get; set; }

        public FolderAssignment()
        {
            name = null;
            folderPath = null;
            desc = null;
        }
        public FolderAssignment(string n, string fPath, string des)
        {
            name = n;
            folderPath = fPath;
            desc = des;
        }
        public FolderAssignment(string n, string fPath)
        {
            name = n;
            folderPath = fPath;
        }
    }

    public class GlobalVars
    {
       // public Main mainWindow = null;

        public int appScreenNum = 0;

        public Help helpF1 = new Help();
        public int iValue = 0;

        //public List<InitParms> xxxinitParmsList = new List<InitParms>();
        //public List<InitParmsStruct> initParm1ItemList = new List<InitParmsStruct>();

        public List<FolderAssignment> folderAssignments = new List<FolderAssignment>();
        //
        public List<InitParms1> initParm1List = new List<InitParms1>();

        public bool ERROR_STOP = false;
        
        public ERROR errWindow;

        public DebugWindow debug;
        public int debugLevel = 1;
        // <summary>
        //
        public string prg86files = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public Boolean stopOnImageError = false;
        static public string initFolderName = "/_dmc";
        //----------------------INIT FILE ----------------//
        static public string initFileName = "/sdb5.xml";
        static public string dataFolderName = @"/_dazen";
        static public string historyFileName = "/historyFolderList.xml";
        static public string mydocsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string myDataFolder = mydocsFolder + dataFolderName; // + "\\company_info";
        public string mySDB5notes1 = "SDBdmc1.txt"; // personalized
        public string notesBaseName = "dmc"; // personalized
        //public string notes1path = mydocsFolder + dataFolderName + mySDB5notes1;

        public string inifileFullPathName = mydocsFolder + dataFolderName + initFileName;
        public string dazenMainFolder = mydocsFolder + dataFolderName;
        public string dazenFolderName = dataFolderName;
        public string folderHistoryFileFullPathName = mydocsFolder + dataFolderName + historyFileName;
        //----------------------INIT FILE ----------------//
        public string defaultInitFile = mydocsFolder + initFileName;
        public Boolean copyOnly = false;
        public string initFileName2 = initFileName.Substring(1);
        public string default_init_Path = mydocsFolder.Substring(1);

        //  public InitParms[] initParms;
        public int parmsCount = 0;
        public int MAX_PARMS = 10;
        public int SECONDS_TO_CONTINUE = 10; 

        public Boolean copyFiles = false;
        // "C:/Users/david/Documents/111"
        public string watchFolderPath = @"\\XPS-9000\Users\david\Documents\111";

        public string defaultImageFolder = @"C:\Users\david\Documents\111";
        static string dirProgramFilesDir86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        static string dirProgramFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public string dirHomeDir86 = dirProgramFilesDir86 +  "/DmcZenware/";
        public string dirDeletedFolder = @"C:\aimages\deleted\";
        public string dirSaveRootFolder = @"C:\aimages\";
        public string dirTargetFolder = @"I:\aimages\";
        string dirHomeDir = dirProgramFilesDir + "/DmcZenware/";
        public System.Windows.Forms.OpenFileDialog openFileDialog;
        // ---- SoundPlayer plays a chord.
        static string dirWindows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(dirWindows + @"\Media\chord.wav");
        public System.Media.SoundPlayer finishSoundPlayer = new System.Media.SoundPlayer(dirWindows + @"\Media\tada.wav");

        public List<FolderHistory> folderHistoryList;
        public FolderHistory[] lastFolderList;
        public List<FolderAssignment> folderAssignmentList;

        public FolderBrowserDialog directoryPromptDialog1 = new FolderBrowserDialog();
        string folderName;
               
      public WebBrowser browser;
     //   public Browser1 browser1;
        public bool bStopAll = false;
        public bool bFindFileName = false;
        //
        public bool bLoadingTraverser2 = false;
        //
        public string findFileName;
        public bool bFindType = true;
        //public Browser1 browser1 = null;
        // public Browser1 wb2 = null;
        public string website;
        public string parseItem;

        public bool bCaptureAllImageInWb2 = false;
        
     /// <summary>
     /// 
     /// </summary>
   public char charFileName = 'z'; ////////////////// microsecond discriminator

        /// <summary>
        /// 
        /// </summary>
        public string errorImage = "C:/error.jpg";

        //Cursor.Current = Cursors.WaitCursor; // Turn mouse into hourglass 

        //----------- ENVIRONMENT -------------------
        public int primaryScreen = 0;
        public int displayCount = 1;

        public Screen[] screen = new Screen[4];
        //public Screen screen0;
        //public int screen0x;

        //public Screen screen1;
        //public int screen1x;
        
        //public Screen screen2;
        //public int screen2x;

        public Size screenSize;
        public Rectangle virtualScreenSize;
        public Size screenPixels;

        static int SIcolNum = 0;
        public int IDX_FPATH = SIcolNum++; // 1
        public int IDX_DIRPATH = SIcolNum++; // 2
        public int IDX_TARGET_DIR = SIcolNum++; //3
        public int IDX_LASTDIR = SIcolNum++;  //0
        public int IDX_LASTDIR2 = SIcolNum++; //4
        public int IDX_LASTDIR3 = SIcolNum++; //5

        public Main mainWindow;
        public bool bLoadFromStreamPreview = true;
        public bool bAutoPlayMovies = false;
        //
        public bool foundInWin2 = false;
        //
        public bool bSurpressErrorDialog = true;
        public Boolean bExitProgram = false; ///// force close of main window 
        public bool bShowProgress = false;
        //----------- images ------------------- LIST -----LIST of IMAGES ---------------------
        public ImageFileList imageFileList1;
        public ImageFileList imageFileList2;
        public ImageFileList deleteFileList1;

        public ImageFileList imageFileListCompare;
        public Boolean bImageFileList1Loaded = false;

        public URLinfoList urlList;

        public FileList fileListWin;

        public int nsoundHighGong = 1;
        public int nsoundHighAlert = 2;
        public int nsoundLowAlert = 3;
        public int nsoundLowGong = 4;
        public int nsoundDing = 5;
        public int nsoundAlert = 6;
        public int nsoundOpen = 7;
        public int nsoundClose = 8;
        public int nsoundOther = 8;
        //path for alert sounds
        public string soundHighGongPath = @"C:\Windows\Media\Windows Foreground.wav";
        public string soundHighAlertPath = @"C:\Windows\Media\Windows Hardware Insert.wav";
        public string soundLowAlertPath = @"C:\Windows\Media\Windows Hardware Remove.wav";
        public string soundLowGongPath = @"C:\Windows\Media\Windows Notify System Generic.wav";
        public string soundDefault = @"C:\Windows\Media\Windows Ding.wav";
        public string soundAlert = @"C:\Windows\Media\chord.wav";
        public string soundOpen = @"C:\Windows\Media\Windows Foreground.wav";
        public string soundClose = @"C:\Windows\Media\Windows Foreground.wav";
        public string soundOther = @"C:\Windows\Media\Windows Information Bar.wav";

        public string soundHighGongPath2 = @"C:\Windows\Media\Windows Unlock.wav";
        public string soundLowGongPath2 = @"C:\Windows\Media\Speech Off.wav";
        //---
        public string FILE_TYPE = "none"; // "images"; // or "videos"
        public string FILE_EXT = "none"; // "images"; // or "videos"

        public int slideCount0 = 0; //only in BG
        public int slideCount1 = 0; //list1
        public int slideCount2 = 0; //list2
        public int maxCountCopy = 100;
        public bool bCopyOnlyOneBatch = false;
        public bool continueCopy = true;
        public int iMaxFileCount = 999999;
       // THIS IS DEFINED IN TraversBG.cs ------------ public int maxFilesAllowed = 999999;
        public int nextIdx = -100;
        public int lastDisplayedIdx = -1;
        public int iShowDirection = 1; // INCREMENT +1 or -1 (reverse)

        //--------------------
        //public string dirFullpath = "default not set";
        //public string dirTarget4Copy = "default not set";
        public Boolean bResizeBestFit = false;
        public string imgFile = null;

        //-------------- SLIDE SHOW 
        public Boolean bSlideShow = false;

        //----------- Traverse stats -------------------
        public string traverserStatus;
        public string lastTraversedFolder;
        public string lastSubFolder;

        public TraverserDialog dialogTraverser;
        public TraverserDialog dialogTraverser2;

        public bool bPadImageName = false;

        //----------- DATABASE ---------------------------- DB --
       
        public GlobalVars(Main mwin)
        {
            this.mainWindow = mwin;
            debug = new DebugWindow(this);
           // debug.Visible = true;
           // debug.Show();
            directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyDocuments;

            //this doesn't create two InitParms instances in the array. it only allocates two slots in the array to hold two instances. 
            //To complete this further 
            // --------initParms[0] = new InitParms("name", "value")     
            
          /*  initParms = new InitParms[MAX_PARMS];

            for (int idx = 0; idx < MAX_PARMS; idx++)
            {
                initParms[idx] = new InitParms("name", "value");
            }
           * */
            
            debug.Activate();

        }
        public void showHelp(Point xy, string subject)
        {
            showHelp(xy.X, xy.Y,  subject);
        }
        public void showHelp(int x, int y , string subject)
        {
           // h.Location = new Point(x, y);
            if (helpF1.IsDisposed)
                helpF1 = new Help();
            helpF1.setSubject(subject);
            helpF1.Visible = true;
            helpF1.Activate();
            helpF1.Location = new Point(x +10 , y +10);
        }
        public string fileDialog()
        {
            InitializeOpenFileDialog();
            return openFileDialog.FileName;
        }

        string mysqlcs = @"datasource=localhost;username=root;password=dmcdmc1;database=SDB01";
        string mysqlexpress = @"Server=.\DMC_SDB4A;Database=WideWorldImporters; Integrated Security=true";

        string ds3 = @"Data Source=C:\Users\david\Documents\Visual Studio 2010\Projects\SDB3\PictureViewer\SDB01.sdf;Password=dante1770@anthem;Persist Security Info=True";

        string ds2 = @"Data Source=C:\Users\david\Documents\dzenSDB\SDB01.sdf;Password=dante1770@anthem;Persist Security Info=True";

        string ds1 = @"Data Source=C:\Users\david\Documents\dzenSDB\SDB01.sdf;Persist Security Info=True";

        public string getDbConnectionString()
        {
            return mysqlexpress;
        }
        public void resetFileAppendChar()
        {
            charFileName = 'z';
        }
        public char getFileAppendChar()
        {
            char cAppend = charFileName--;
            if (charFileName < 'a')
                charFileName = 'z';
            return cAppend;
        }

        public void setRootFolder(int selectedRootId)
        {
           // FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

         //folderBrowser replaced with   gv.directoryPromptDialog
            directoryPromptDialog1.Description = "Select ----- Folder";

           

         
            switch (selectedRootId)
            {
                case 0:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyDocuments; 
                    break;
                case 1:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyComputer;//Drives = 0x0011, // My Computer 
                    break;
                case 2:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.CommonDocuments;
                    break;
                case 3:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.Desktop; //  DesktopDirectory = 0x0010, // user name\Desktop 
                    break;
                case 4:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.Desktop; //  DesktopDirectory = 0x0010, // user name\Desktop 
                    break;
                case 5:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.ProgramFiles;
                    break;
                case 6:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.CommonPictures; //  MyPictures = 0x0027, // C:\Program Files\My Pictures 
                    break;
                case 7:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.Favorites;  // CommonFavorites = 0x001f,
                    break;
                case 8:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyVideos; //            MyVideo = 0x000e, // "My Videos" folder 
                    break;
                default:
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyDocuments; 
                    break;
            }
            directoryPromptDialog1.ShowNewFolderButton = false;
        }

        /*
         * If the SelectedPath property is set before showing the dialog box, 
         * the folder with this path will be the selected folder 
         * - as long as SelectedPath is set to an absolute path that is a subfolder of RootFolder 
         * (or more accurately, points to a subfolder of the shell namespace represented by RootFolder)."
         * 
         * public static string GetFolderPath(	Environment.SpecialFolder folder)
         * */
        public string dirDialog(InitFolder startFolder, string specialPath = null, string myFilePath = null)
        {
            // directoryPromptDialog.RootFolder = Environment.SpecialFolder.Desktop;
            // directoryPromptDialog.SelectedPath = @"c:\temp\";
            // directoryPromptDialog.SelectedPath = @"C:\";
            if (string.IsNullOrEmpty(myFilePath))
            {
                if (startFolder.Equals(InitFolder.MyDesktop))
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.Desktop;
                else if (startFolder.Equals(InitFolder.MyPics))
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyPictures;
                else if (startFolder.Equals(InitFolder.MyDocs))
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyDocuments;
                else if (startFolder.Equals(InitFolder.MyComputer))
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                else if (startFolder.Equals(InitFolder.C_Drive))
                {
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                    directoryPromptDialog1.SelectedPath = dirSaveRootFolder;
                }
                else if (startFolder.Equals(InitFolder.Special))
                {
                    directoryPromptDialog1.RootFolder = Environment.SpecialFolder.UserProfile; //Environment.SpecialFolder.MyComputer;
                                                                                               //directoryPromptDialog1.SelectedPath = @"C:\users\";
                }

                //directoryPromptDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
                debug.w("openDir Dialog with ", Environment.GetFolderPath(directoryPromptDialog1.RootFolder));

            }
            if (!string.IsNullOrEmpty(myFilePath))
                directoryPromptDialog1.SelectedPath = myFilePath; 

            DialogResult result;
            try
            {
                result = directoryPromptDialog1.ShowDialog(); // this is the dialog to find the initial DIRECTORY for traversal  
            }
            catch (Exception e)
            {

                debug.w("EXCEPTION ", e.ToString());
                return null;
            } 

            if (result == DialogResult.OK)
            {
                folderName = directoryPromptDialog1.SelectedPath;
                return folderName;

            }
            return null;
        }
        public string dirDialog(string initialDir)
        {
            // dialog.SelectedPath = @"C:\Users\david\Documents";
            directoryPromptDialog1.SelectedPath = @initialDir;


            DialogResult result = directoryPromptDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                folderName = directoryPromptDialog1.SelectedPath;
                return folderName;

            }
            return null;
        }
        public void SetOpenFileDialogXML()
        {
            if (this.openFileDialog == null)
                this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            // Set the file dialog to filter for graphics files.
            this.openFileDialog.Filter =
                "XML Files (*.XML)|*.XML";

            // Allow the user to select multiple images.
            this.openFileDialog.Multiselect = false;// .SupportMultiDottedExtensions = true; //.Multiselect = true;
        }
        public void InitializeOpenFileDialog()
        {
            if (this.openFileDialog == null)
                this.openFileDialog = new System.Windows.Forms.OpenFileDialog();

            // Set the file dialog to filter for graphics files.
            this.openFileDialog.Filter =
                "Images (*.BMP;*.JPG;*.GIF; *.JPEG)|*.BMP;*.JPG;*.GIF;*.JPEG|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog.Multiselect = true;// .SupportMultiDottedExtensions = true; //.Multiselect = true;
            this.openFileDialog.Title = "Select Directory (or file)";

            
            // openFileDialog1.InitialDirectory = Default_Path;

        }
        public int messageCount = 0;
        string[] messages = new string[10];

        public Movies mp;

        public void SetMovies(Movies Mov)
        {
            mp = Mov;
        }
        public Movies GetMovies(bool old)
        {
            if (mp == null || mp.IsDisposed)
                return null;

            return mp;
        }
        public void CloseMovies(bool old)
        {
            mp.Close();
        }
        
        //2022

        public WmPlayer wmp;
        public void SetMovies(WmPlayer Mov)
        {
            wmp = Mov;
        }
        public WmPlayer GetMovies()
        {
            if (wmp == null || wmp.IsDisposed)
                return null;
            
            return wmp;
        }
        public void CloseMovies()
        {
            wmp.Close();
        }

        public int message(string text)
        {
            if (messageCount < 9)
            {
                messages[messageCount++] = text;
            }
            return messageCount;

        }
        public string getMessage()
        {
            if (messageCount > 0)
                return messages[--messageCount];
            else
                return null;
        }
        public void setCursor(string name)
        {
            if (name.Equals("hourglass"))
                Cursor.Current = Cursors.WaitCursor;
            else
                Cursor.Current = Cursors.Default;
        }
        public void setCursorDefault()
        {
            Cursor.Current = Cursors.Default;
        }
        public void setCursorHourGlass()
        {
                Cursor.Current = Cursors.WaitCursor;
        }
        
    }
}
