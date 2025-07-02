using Microsoft.Win32;
using SymbolDB.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
//using static System.Net.Mime.MediaTypeNames;

namespace SymbolDB
{
    public struct FileDialogInput2
    {
        public string fpath;
        public string fname;

        public FileDialogInput2(string fpathIn)
        {
            this.fpath = fpathIn;
            this.fname = fpathIn;
        }
    }
    public partial class TraverserDialog : Form
    {
#pragma warning disable IDE0044 // Add readonly modifier
        GlobalVars gv;
#pragma warning restore IDE0044 // Add readonly modifier
        //set root folder
        InitFolder startFolder = InitFolder.MyComputer; //starting Folder is set my radiobutton choice or by parm
        string targetFolder;
        FileFunctions ff;

        public enum Action
        {
            TRAVERSING, LOADING_DISPLAY, NONE, COMPLETED
        }

        Action action = Action.NONE;
        TraverserBG traverser;
        public string fullpath;
        //const int gv.iMaxFileCount = 9999;

        Point listViewLoc;


        Main main = null;
        TraverserDialog parentWin;
        bool bThisIsSubWindow = false;

        public int rowCount = 0; // row count in listView1

        private ListViewColumnSorter lvwColumnSorter;


        public TraverserDialog(GlobalVars g, Main mainParent, int imode, FileFunctions f, TraverserDialog parent = null) /////////////////////////////////////////////////////////////////////////
        {
            InitializeComponent();
            // to scale everything by 25%.
            float scaleFactor = 1.5f;

            // Scale the form and all child controls.
            if (parent == null)
                this.Scale(new SizeF(scaleFactor, scaleFactor));

            gv = g;
            //gv.dialogTraverser = this;
            gv.fileListWin = new FileList(gv, f);
            gv.fileListWin.Show();
            gv.fileListWin.Activate();
            gv.fileListWin.WindowState = FormWindowState.Minimized;

            cbPlayBackControls.Checked = true;
            tbMainOrSubWindow.Text = "Main Window";

            startFolder = InitFolder.Special; // tbDirectoryPath.Text;

            startFolder = InitFolder.MyDesktop;
            btTestCopy.Visible = false;
            pbThumbNail.SizeMode = PictureBoxSizeMode.Zoom;

            tbTargetFolder.Text = gv.initParm1List[0].targetDir1;

            //btCopyFileInProgress.Visible = false;
            if (!string.IsNullOrEmpty(gv.initParm1List[0].targetDir1))  // 2nd Traverser Window List 2 so set to TargetDir1
            {
                int idx = cmboSourceFolder.FindString(gv.initParm1List[0].targetDir1);
                cmboSourceFolder.SelectedItem = idx;
                targetFolder = gv.initParm1List[0].targetDir1;
            }
            ff = f;
            main = mainParent;
            if (parent != null)
            {
                bThisIsSubWindow = true;
                //  cbPositionSearchStatus.Checked = false;
                tbMainOrSubWindow.Text = "Sub Window";
                parentWin = parent;
                btSearchWin2.Text = "Search Main";
                btClose.Text = "Close List2";
                tbDirectoryPath.Text = gv.initParm1List[0].targetDir1;
                gv.dialogTraverser2 = this;
                RepositionTraverser2();
                this.Activate();
                this.Show();

            }
            else
            {
                gv.dialogTraverser = this;
                cbCopyFileListMain.Checked = true;
            }

            tbMax.Text = gv.iMaxFileCount.ToString();
            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.

            if (!bThisIsSubWindow)
            {
                this.CenterToScreen();
                Point xy = this.Location;
                xy.Y = 0;
                this.Location = xy;
            }
            traverser = new TraverserBG(gv, this);
            //CreateListView1();
            this.tbInfo.Text = "create ListView1";

            if (!bThisIsSubWindow)
            {
                if (gv.initParm1List.Count > 0)//   .initParms[0] != null)
                {
                    // gv.dirFullpath = gv.initParm1List[0].lastDir1;
                    tbDirectoryPath.Text = gv.initParm1List[0].mostRecent; //lastPath    in init

                    gv.lastTraversedFolder = gv.initParm1List[0].sourceDir1;
                }
                else
                    tbDirectoryPath.Text = gv.lastTraversedFolder; //in init
            }
            else
            {
                if (!string.IsNullOrEmpty(gv.initParm1List[0].targetDir1))
                    tbDirectoryPath.Text = gv.initParm1List[0].targetDir1;
            }
            loadSourceCmboList();
            loadTargetCmboList();
            addHistoryToSourceCmboList();
            if (bThisIsSubWindow)
                tbDirectoryPath.Text = gv.initParm1List[0].mostRecentSubfolder;
            LoadMostRecentFolders();

            dgv1.MultiSelect = false;
        }
        public void LoadMostRecentFolders()
        {
            tbHistory1.Text = gv.initParm1List[0].mostRecent1;
            tbHistory2.Text = gv.initParm1List[0].mostRecent2;
            tbHistory3.Text = gv.initParm1List[0].mostRecent3;
            tbHistory4.Text = gv.initParm1List[0].mostRecent4;
            tbHistory5.Text = gv.initParm1List[0].mostRecent5;
            tbHistory6.Text = gv.initParm1List[0].mostRecent6;
        }

        public void SaveImageFileListDefault()
        {
            string localAppData =
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string myAppFolder = Path.Combine(localAppData, "SymbolDB");    // or your app’s name
            Directory.CreateDirectory(myAppFolder);

            string listFile = Path.Combine(myAppFolder, "ImageFileList.xml");

        }


        // CAPTURE FOCUS ON THIS WINDOW
        int timerCount = 0;
        public void call_from_timer_main()
        {
            SetForegroundWindow(this.Handle);
            timerCount++;
            tbMainTimer.Text = timerCount.ToString();
            if (Form.ActiveForm == this)
            {
                btFocus.BackColor = Color.Yellow;
            }
            else
            {
                btFocus.BackColor = Color.Orange;
            }
            this.Focus();

        }

        private void btRepositionWindows_Click(object sender, EventArgs e)
        {
            RestorePosition();
        }
        public void RestorePosition()
        {
            if (!bThisIsSubWindow)
            {
                this.CenterToScreen();
                Point xy = this.Location;
                xy.Y = 0;
                this.Location = xy;
                if (gv.dialogTraverser2 != null)
                    gv.dialogTraverser2.RepositionTraverser2();
            }
            else
            {
                RepositionTraverser2();
            }
        }
        public bool bDoingBackup = false;
        public void GoToNextRow()
        {
            if (rowIdMovieList < dgv1.RowCount - 1)
            {
                //++rowIdMovieList;
                // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                // dgv1.Rows[rowIdMovieList + 1].Selected = true;
                AdvanceSelectionDGV1();
            }
        }
        public void GoToPreviousRow()
        {
            if (rowIdMovieList > 0)
            {
                //++rowIdMovieList;
                // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                // dgv1.Rows[rowIdMovieList + 1].Selected = true;
                PreviousSelectionDGV1();
            }
        }
        public bool PreviousSelectionDGV1()
        {
            bool bAdvanced = true;
            if (rowIdMovieList > 0)
            {
                rowIdMovieList--;
                //dgv1.ClearSelection();
                dgv1.CurrentCell = dgv1.Rows[rowIdMovieList].Cells[0]; // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList].Cells[0];  correct
                dgv1.Rows[rowIdMovieList].Selected = true; //SELECTED WILL SET rowIdMovieList 

                if (dgv1.SelectedRows.Count > 1)
                    MessageBox.Show("multiselect", "error");
                dgv1.Refresh();
            }
            bAdvanced = false;

            return bAdvanced;
        }
        TopMost continueWindow;
        int countProcessed = 0;
        int countCopied = 0;
        private void btCopyNewFiles_Click(object sender, EventArgs e)
        {
            btScanBackup.Text = "Backup!";
            gv.maxCountCopy = (int)numCopyBatchNumber.Value;
            numCopyCount.Value = 0;
            countCopied = 0;
            rowIdMovieList = 0;
            bool rc = true;
            bDoingBackup = true;
            while (rowIdMovieList < dgv1.RowCount - 1)
            {
                ScanFileListNextItem();
                CheckAndCopyIfNewFile();
                ++countProcessed;
                if (countProcessed > gv.maxCountCopy)
                {
                    countProcessed = 0;
                    rc = DisplayContinuePrompt();
                }
                if (!gv.continueCopy)
                {
                    bDoingBackup = false;
                    return;
                }
                if (!rc)
                {
                    bDoingBackup = false;
                    return;
                }
            }
            btScanBackup.Text = "Backup>";
            bDoingBackup = false;

        }
        bool CheckContinue()
        {
            bool rc = DisplayContinuePrompt();
            return rc;
        }
        public void ScanFileListNextItem()
        {
            if (rowIdMovieList < dgv1.RowCount - 1)
            {
                int oldRowId = rowIdMovieList;
                //bring row into view 
                //dataGridView1.FirstDisplayedScrollingRowIndex = index;
                //dataGridView1.Refresh();
                // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                // dgv1.Rows[rowIdMovieList + 1].Selected = true; //SELECTED WILL SET rowIdMovieList 
                AdvanceSelectionDGV1();
                //
                //  DgvFileList1_SelectionChanged(this, new EventArgs());
                if (oldRowId == rowIdMovieList)
                {
                    MessageBox.Show("rowId did not increment", $"{rowIdMovieList}");
                    return;
                }
                dgv1.Refresh();
                tbRowIdTemp.Refresh();
            }
            else
            {
                cbScanMovies.Checked = false;
                cbSearchUntilNotFoundIn2.Checked = false;
                return;
            }
        }
        DisplayMessage displayMessage;
        public void DisplayCopyMessageText(String targetDir2, string txt2, string targetDirRoot)
        {
            if (cbHidePlayer.Checked)
                MinimizePlayer();
            if (displayMessage == null || displayMessage.IsDisposed)
            {
                displayMessage = new DisplayMessage(gv, this);
                displayMessage.Visible = true;
                displayMessage.SetTopMost(true);
                displayMessage.Location = new Point(0, 0);
            }
            else
            {
                displayMessage.WindowState = FormWindowState.Normal;
                displayMessage.Activate();
                displayMessage.SetTopMost(true);
            }
            if (targetDirRoot.Contains("/AAAA") || targetDirRoot.Contains("\\AAAA"))
            {
                bUsingSpecialDirectory = true;
            }
            else
            {
                bUsingSpecialDirectory = false;
            }
            displayMessage.DisplayCopyMessageText(bUsingSpecialDirectory, targetDir2, txt2, targetDirRoot);
        }
        public void HideCopyMessageWindow()
        {
            if (displayMessage == null || displayMessage.IsDisposed)
            {
            }
            else
            {
                displayMessage.SetTopMost(false);
            }
            if (cbHidePlayer.Checked)
                RestorePlayer();
        }
        public void CheckAndCopyIfNewFile()
        {
            bool foundMatch = false;
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                foundMatch = ClearAndSearch2();
            if (!foundMatch)
            {
                CopyNewFile();
                countCopied++;
                tbCountBackupCopied.Text = countCopied.ToString();
            }
        }
        int countCopiedMovies = 0;
        public void CopyNewFile()
        {
            char cData = 'd';
            cData = tbCurrentFileFolder.Text[0];
            ++countCopiedMovies;

            this.Text = $"copy {cData} {tbTargetFolder.Text}";
            this.Refresh();
            this.Text = $"copy completed {countCopied}";
            bool rcc = copyFile(cData);  //ProcessCmdKey
        }


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
        private void btPositionLeftDown_Click(object sender, EventArgs e)
        {
            RepositionTraverser2();
        }
        public void RepositionTraverser2()
        {
            if (parentWin == null)
                return;
            this.Size = new Size(1900, 600);
            int screenHeight = gv.screen[0].WorkingArea.Height;
            Point myLocation = parentWin.Location;
            if (bThisIsSubWindow)
            {
                myLocation.X = 0;
                myLocation.Y = screenHeight - this.Height;
                this.Location = myLocation;
            }
        }

        // Enable 
        public void loadSourceCmboList()
        {
            cmboSourceFolder.Items.Add(gv.initParm1List[0].sourceDir1);
            cmboSourceFolder.Items.Add(gv.initParm1List[0].sourceDir2);
            cmboSourceFolder.Items.Add(gv.initParm1List[0].sourceDir3);
            cmboSourceFolder.Items.Add(gv.initParm1List[0].sourceDir4);
            cmboSourceFolder.Items.Add(gv.initParm1List[0].sourceDir5);
            if (gv.initParm1List[0].sourceDir6 == null)
                gv.initParm1List[0].sourceDir6 = "";
            cmboSourceFolder.Items.Add(gv.initParm1List[0].sourceDir6);
            cmboSourceFolder.SelectedIndex = 0;
        }
        public void loadTargetCmboList()
        {
            cmboSetTarget.Items.Add(gv.initParm1List[0].targetDir1);
            cmboSetTarget.Items.Add(gv.initParm1List[0].targetDir2);
            cmboSetTarget.Items.Add(gv.initParm1List[0].targetDir3);
            if (!string.IsNullOrEmpty(gv.initParm1List[0].mostRecentTarget))
                cmboSetTarget.Items.Add(gv.initParm1List[0].mostRecentTarget);
            cmboSetTarget.SelectedIndex = 0;
        }
        public int FindItemInComboBox(string txt)
        {
            int idx = cmboSetTarget.FindStringExact(txt);
            return idx;
        }
        public int FindItemInHistoryList(string txt)
        {
            int idx = cmboSetTarget.FindStringExact(txt);
            return idx;
        }
        public void addHistoryToSourceCmboList()
        {
            for (int idx = 0; idx < gv.folderHistoryList.Count; ++idx)
            {
                //if (FindItemInComboBox(gv.folderHistoryList[idx].folderPath) < 0)
                cmboSourceFolder.Items.Add(gv.folderHistoryList[idx].folderPath);
            }
        }

        public void loadSourceCmboListFromParmWindow() //NOT USED
        {
            for (int idx = 0; idx < gv.folderHistoryList.Count; ++idx)
            {
                cmboSourceFolder.Items.Add(gv.folderHistoryList[idx].folderPath);
            }
            cmboSourceFolder.SelectedIndex = 0;
        }

        ImageFileList imageFileFolderList;
        ImageFileList imageFileList1;
        ImageFileList imageFileList2;
        ImageFileList imageFileListOriginal1; //in the scope of this Traverser
        ImageFileList imageFileListOriginal2; //in the scope of this Traverser
                                              //   ImageFileList imageFileListOriginalHold;

        public Boolean insertRowsInto_LV_ImageList(ImageFileList imageFileList, int count)
        {

            return true;
        }
        public void resetOriginalList()
        {
            resetDGV();
        }
        public void resetDGV()
        {
            dgv1.DataSource = null;
            List<FileInfoItem> orderedByFileName;
            if (bThisIsSubWindow)
                orderedByFileName = imageFileList2.finfoList.OrderBy(file => file.fname).ToList();
            else
                orderedByFileName = imageFileList1.finfoList.OrderBy(file => file.fname).ToList();
            dgv1.DataSource = orderedByFileName;
            tbCount.Text = dgv1.RowCount.ToString();
        }
        public void createFileListDataGrid()
        {
            dgv1.Columns[0].Width = 400;
            dgv1.Columns[1].Width = 500;
            dgv1.Columns[2].Width = 100;
            dgv1.Columns[3].Width = 100;
            dgv1.Columns[4].Width = 1200;
            dgv1.Columns[7].Width = 400;
        }
        public void resizeFileListDataGrid()
        {
            dgv1.Columns[0].Width = 400;
            dgv1.Columns[1].Width = 350;
            dgv1.Columns[2].Width = 50;
            dgv1.Columns[3].Width = 100;
            dgv1.Columns[4].Width = 50;
            dgv1.Columns[5].Width = 30;
            dgv1.Columns[6].Width = 30;
            dgv1.Columns[7].Width = 200;
            setReadOnly();
        }
        public void setReadOnly()
        {
            foreach (DataGridViewColumn dc in dgv1.Columns)
            {
                dc.ReadOnly = true;
                //dc.Index.Equals(0);
                break;
            }
        }
        bool bDidZoom = false;
        bool bReceivedTimerMessage = false;

        public void CallFromMainTimer()
        {
            if (cbFocusIsOnThisWindow.Checked)
                SetForegroundWindow(this.Handle);
        }

        private void SetForegroundWindow(IntPtr handle)
        {
            throw new NotImplementedException();
        }

        public void takeFocusIfYouWant(double pos, bool bForceFocus = false)
        {
            if (!bFocusIsHere)
            {
                if (cbCatalog.Checked)
                    SoundAlertFocus();

                btFocus.BackColor = Color.Orange;
            }
            bFocusIsHere = true;
            cbFocusIsOnThisWindow.Checked = bFocusIsHere;
            if (bForceFocus)
                Console.Beep();
            if (cbFocusHere.Checked || bForceFocus)
                this.Focus();
        }
        int messageCount = 0;
        bool delayOnce = true;
        int counttest = 0;
        //
        // MESSAGE STATUS
        //
        int timerTicksWas = 1;
        public void SetScanPause(bool suspend)
        {
            if (suspend)
            {
                timerTicksWas = (int)numTimerTicks.Value;
                numTimerTicks.Value = 222;
            }
            else
            {
                numTimerTicks.Value = timerTicksWas;
            }
        }
        public bool RETRIGGER_DGV1 = false;

        public void movieTimerNotice(double pos)
        {
            if (!tbPlayStateEnding.Text.Equals("ENDING"))
            {
                ++counttest;
                tbCounterDelay.Text = $"{counttest}";
                if (counttest >= (int)numTimerTicks.Value)
                {
                    counttest = 0;
                }
                else
                    return;
            }
            if (!bReceivedTimerMessage)
            {
                bReceivedTimerMessage = true;
                //cbReceivedMessage.Checked = true;
                //mp.zoomFullScreen(this);
                //getMovieStatus();
                //bReceivedTimerMessage = false;
                if (!getMoviePlayerState().Contains("Play"))
                {
                    mp.play();
                }
            }
            updateMovieStatus(pos);
            if (cbMore.Checked && messageCount > 2)
            {
                if (delayOnce)
                {
                    delayOnce = false;
                    return;
                }
                delayOnce = true;
            }

            tbMessageIn.Text = messageCount++.ToString();
            if (messageCount == 1)
                cbReceivedFirstMessage.Checked = true;
            if (cbScanMovies.Checked && cbAutoPlay.Checked)
            {
                if (bPlayingEnding)
                    return;
                if (cbSoundScan.Checked)
                    gv.mainWindow.soundAlert(49);
                if (messageCount > 0 && messageCount < numSampleCount.Value)
                {
                    //longer segments option
                    mp.skipForwardPercent((int)numSampleCount.Value);
                    /*
                    if (cbMore.Checked)
                        mp.skipForward();
                    else
                        mp.skipForwardBig();
                    */
                    getMovieStatus();
                }
                else if (messageCount >= numSampleCount.Value) //set ENDING
                {
                    if (cbScanAndPlay.Checked)
                    {
                        if (cbAutoCopyScan.Checked) //tbFoundFileFolder)
                        {
                            if (!string.IsNullOrEmpty(tbFoundFileFolder.Text))
                            {
                                if (cbGoToNext.Checked)
                                    if (rowIdMovieList < dgv1.RowCount - 1)
                                    {
                                        AdvanceSelectionDGV1();
                                    }
                                return;
                            }
                            if (!string.IsNullOrEmpty(tbCurrentFolder.Text))
                            {
                                char cData = tbCurrentFolder.Text[0];
                                bool rcc = copyFile(cData);
                                if (!rcc)
                                    return;
                            }
                        }
                        return;
                    }
                    counttest = (int)numTimerTicks.Value;
                    if (cbMinScan.Checked)
                    {
                        playNextMovie(true);
                    }
                    else
                    {
                        if (!bPlayingEnding)
                            GoToEnd(endPosition);
                        btGoToEnd15.BackColor = Color.LightBlue;
                        bPlayingEnding = true;
                    }
                }
                if (messageCount < numSampleCount.Value && !bPlayingEnding)
                    btGoToEnd15.BackColor = Color.LightGray;
                else if (!bPlayingEnding)
                {
                    GoToEnd(endPosition);
                    btGoToEnd15.BackColor = Color.LightBlue;
                    bPlayingEnding = true;
                }
                tbCountM.Text = messageCount.ToString();

            }
            if (retriggerDGV1)
            {
                // retriggerDGV1 = false;
                //  dgv1_SelectionChanged(null, null);
            }
        }
        bool bPlayingEnding = false;
        //bool playLongerSegments = false;
        // bool playLongerToggle = true;



        public void changeState()
        {
            DisplayMovieResolution();
        }

        public void resetMessageToZero()
        {
            cbReceivedFirstMessage.Checked = false;
            messageCount = 0;
            delayOnce = true;
        }
        public Boolean insertRow(FileInfoItem finfo)
        {

            return true;
        }

        bool cbRename = true;
        bool rbMove = true;

        public bool CopyAndRenameFile(string fpathCurrentImage, string targetDir)
        {
            bool bcopied = false;
            DateTime dt = DateTime.Now;
            string fname = String.Format("{0:yyMMddHHmmssfff}", dt);
            string ftype = gv.FILE_EXT;
            //
            //2024

            //fname = fname + ftype;
            fname = fname + ftype;
            if (false)
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
            // MessageBox.Show($"--->> COPIED/MOVED and RENAMED image file >>  {targetDir}, {fpathCurrentImage}, {fname}", "RENAMED image file ");

            return bcopied;
        }
        bool bSearch2Again = true;
        string descPlayBackControls = "1 Play 2 Back 3 Forward 4 Next ||  5 Zoom 6 BACK 7 SK2min 8 ToEnd || 9 Search 10 Pause";
        /*
           * through overriding the ProcessCmdKey() method of the form. That way, 
            * your key handling logic gets executed no matter what control 
           * has focus at the time of keypress. 
           * Beside that, you even get to choose whether the focused control gets the key 
           * after you processed it (return false) or not (return true).
        *
        * When you call "return true" you are handling the key press
           * */
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) /////// override dmcdmc123
        {
            bool rc = false;

            tbCopyFileTargetFolder.Text = DateTime.Now.ToShortTimeString();
            if (!cbPlayBackControls.Checked && keyData == Keys.F1)
            {
                // dgv1_SelectionChanged(null, null);
                gv.showHelp(this.Location, "Dialog Traverser");
                this.Text = descPlayBackControls;
                return true; //for the active control to see the keypress, return false
            }
            else if (keyData == Keys.Back)
                return false;
            if (cbCatalog.Checked && bLoadedList)
            {
                // IF Cataloging and this is a KEY a-z 0-9
                btFocus.Text = "Focus";
                if ((keyData >= Keys.A && keyData <= Keys.Z) || (keyData >= Keys.D0 && keyData <= Keys.D9)) //update rating or //COPY image to C:\\aImages\x  folder //////
                {
                    rc = true;
                    char cData;
                    if (keyData >= Keys.D0 && keyData <= Keys.D9)
                    {
                        cData = Convert.ToChar(msg.WParam.ToInt32()); ///////////////// convert 0-9 
                    }
                    else
                    {
                        string s = keyData.ToString();
                        cData = s[0];
                    }
                    if (cbCatalog.Checked && bLoadedList)
                    {
                        fpath = tbMovieFpath.Text;

                        if (mp == null || mp.IsDisposed)
                        {
                            if (mpVersion1)
                                mp = new WmPlayer(gv, this);
                            else
                                mp2 = /*P2*/ new WmPlayer(gv, this);
                        }
                        else
                        {
                            //   mp.loadMovie("");
                            //MessageBox.Show("ERROR ProcessCmdKey cbCatalog");
                        }
                        bool rcc = copyFile(cData);  //ProcessCmdKey
                        this.Text = $"copy {cData} {keyData}";
                        if (!rcc)
                        {
                            GoToEnd(5);
                            //return true; //handled KEY but copy failed
                        }
                    }
                    //if (bAutoAdvance)
                    //   showNextSlideNow();
                    // rc = true;
                }
                else if (keyData == Keys.Space)
                {
                    if (rowIdMovieList < dgv1.RowCount - 1)
                    {
                        //++rowIdMovieList;
                        // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                        // dgv1.Rows[rowIdMovieList + 1].Selected = true;
                        AdvanceSelectionDGV1();
                        //this.Text = rowIdMovieList.ToString();
                    }
                    return true;
                }

            }
            else if (cbRating.Checked)
            {
                if ((keyData >= Keys.A && keyData <= Keys.Z) || (keyData >= Keys.D0 && keyData <= Keys.D9)) //update rating or //COPY image to C:\\aImages\x  folder //////
                {
                    rc = true;
                    string charIn = keyData.ToString();
                    char cData = charIn[0];
                    if (charIn.Length > 1)
                        cData = charIn[1];
                    this.Text = $"{cData}";
                    dgv1.Rows[rowIdMovieList].Cells["rating"].Value = cData;
                    tbUpdatedRow.Text = rowIdMovieList.ToString();
                    btFocus.BackColor = Color.LightGreen;
                    btFocus2.BackColor = Color.LightGreen;
                    focusColor = Color.LightGreen;
                    if (cbRateAndNext.Checked)
                        GoToEnd(5);
                }
            }
            if (cbPlayBackControls.Checked)
            {
                if (keyData == Keys.F1)
                {
                    //PlayButtonGo();
                    //  dgv1_SelectionChanged(null, null);
                }
                else if (keyData == Keys.F2)
                {
                    if (cbPicture.Checked)
                    {
                        GoToPreviousRow();
                        return true;
                    }
                    SkipBack();
                }
                else if (keyData == Keys.F3)
                {

                    if (cbPicture.Checked)
                    {
                        GoToNextRow();
                        return true;
                    }
                    SkipForward1min();
                }
                else if (keyData == Keys.F4)
                {
                    cbPause2.Checked = false;
                    if (cbPicture.Checked)
                    {
                        GoToNextRow();
                        return true;
                    }
                    //AdvanceSelectionDGV1();
                    // playNextMovie(true);
                    btReselectSelectCurrentRow_Click(null, null);
                }
                else if (keyData == Keys.F5)
                {
                    SkipForward2min();
                }
                else if (keyData == Keys.F6)
                {
                    SkipBackupLarger();
                }
                else if (keyData == Keys.F7)
                {
                    SkipForward2min();
                }
                else if (keyData == Keys.F8)
                {
                    GoToEnd(cbShortEndSample.Checked ? 5 : 15);
                }
                /* else if (keyData == Keys.F10)
                 {
                     cbPause2.Checked = !cbPause2.Checked;
                 }*/
                else if (keyData == Keys.F9)
                {
                    bSearch2Again = !bSearch2Again;
                    if (bSearch2Again)
                    {
                        tbFoundSeachList2.Text = "search2";
                        tbFoundSeachList2.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        tbFoundSeachList2.Text = "search";
                        tbFoundSeachList2.BackColor = Color.LightCoral;
                    }
                    SearchTrav2DialogFileList();
                }
                else if (keyData == Keys.F12)
                {
                    cbSpecialFolder.Checked = true;
                }
                else if (keyData == Keys.F11)
                {
                    cbSpecialFolder.Checked = false;
                }
                else if (keyData == Keys.Escape)
                {
                    CloseTrav();
                    //Application.Exit();
                }
            }
            bool bSecondaryControls = false;

            if (bSecondaryControls)
            {
                if (cbAllowMovieSelection.Checked)
                {
                    if (keyData == Keys.F9)
                    {
                        gotoNextSelectedRow();
                        return true;
                    }
                    else if (keyData == Keys.F8)
                    {
                        if (rowIdMovieList < dgv1.RowCount - 1)
                        {
                            //++rowIdMovieList;
                            // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                            //  dgv1.Rows[rowIdMovieList + 1].Selected = true;
                            AdvanceSelectionDGV1();
                            //this.Text = rowIdMovieList.ToString();
                        }
                        return true;
                    }
                    else if (keyData == Keys.F10)
                    {
                        if (rowIdMovieList < dgv1.RowCount - 1)
                        {
                            dgv1.Rows[rowIdMovieList].Cells["bInvalid"].Value = true;
                        }
                        return true;
                    }
                }

            }
            if (cbAllowMovieSelection.Checked && mp != null && !mp.IsDisposed)
            {
                if (keyData == Keys.OemMinus)
                    mp.backup3();
                else if (keyData == Keys.Left)
                    mp.backup();
                else if (keyData == Keys.Right)
                    mp.skipForward();
                else if (keyData == Keys.Divide)
                    mp.ScanForward();
                else if (keyData == Keys.Escape)
                    System.Windows.Forms.Application.Exit();
                // else if (keyData == Keys.Down)
                //playNextMovie(true); //key DOWN
                else if (keyData == Keys.PageDown)
                    mp.skipForwardBig();
                else if (keyData == Keys.PageUp)
                    mp.backup3();
                else if (keyData == Keys.Home)
                    restart();
                else if (keyData == Keys.Insert)
                    mp.mute(false);
                else if (keyData == Keys.Add)
                    mp.playNormalSpeed();
                else if (keyData == Keys.Delete)
                {
                    mp.mute(true);
                    browser();
                }
                else if (keyData == Keys.Back)
                    mp.skipBack();
                else if (keyData == Keys.Subtract)
                    mp.slowMotion(true);
                //else if (keyData == Keys.Up)
                //    mp.pause();
                else if (keyData == Keys.Divide)
                    mp.slowMotion();
                else if (keyData == Keys.Pause)
                    mp.pause();
                else if (keyData == Keys.End)
                {
                    GoToEnd(cbShortEndSample.Checked ? 5 : 15);
                }
                else if (keyData == Keys.Oemtilde)
                {
                    mp.stop();
                    this.TopMost = false;
                }
                else
                    return false;
                getMovieStatus();
                return true;
            }
            this.Focus();
            return rc;
        }
        public void MarkAsInvalid()
        {
            if (rowIdMovieList < dgv1.RowCount - 1)
            {
                dgv1.Rows[rowIdMovieList].Cells["bInvalid"].Value = true;
                // dgvFileList1.Rows[rowIdMovieList].Cells["rating"].Value = tbPosition3.Text;
                dgv1.Rows[rowIdMovieList].Cells["source"].Value = tbPosition3.Text;
            }

        }

        public void GoToEnd(int secondsRemaining)
        {
            if (!bPlayingEnding)
                mp.gotoEnd(secondsRemaining);
            bPlayingEnding = true;
        }
        public void browser()
        {
            //System.Diagnostics.Process.Start("http://www.microsoft.com/");
        }
        public void restart()
        {
            resetMpPlay();
            if (mp == null || mp.IsDisposed)
            {
                if (mpVersion1)
                    mp = new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                else
                    mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                mp.Activate();
                mp.Show();
                gv.SetMovies(mp);
            }
            else
            {
                if (CheckFileTypeWEBM(fpath))
                    return;

                mp.restart();
                mp.loadMovie(tbMovieFpath.Text, cbZoom.Checked); //in restart
                TrackFromSourceLoad("restart");
            }
        }
        public void Message1TakeFocus()
        {
            tbPlayBackError.Text = "MESSAGE 1 MESSAGE 1 MESSAGE 1 MESSAGE 1";
            //GoToEnd(5);
            // this.Text = "MESSAGE 1 MESSAGE 1 MESSAGE 1 MESSAGE 1";
        }
        public void showCopyFileInfo(string targetDir, string filename)
        {
            filename = filename.Replace('/', '\\');
            tbCopyFileName.Text = Path.GetFileName(filename);
            tbCopyFileTargetFolder.Text = targetDir;
            this.Refresh();
        }

        public void TrackFromSourceLoad(string lsource)
        {
            tbPreviousPlayNextSource.Text = tbPlayNextSource.Text;
            tbPlayNextSource.Text = lsource;
        }
        public void nextMovie()
        {
            if (gv.errWindow != null)
            {
                this.Text = "ERR";
                // return;
            }

            if (rowIdMovieList < dgv1.RowCount - 1)
            {
                //++rowIdMovieList;
                // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                // dgv1.Rows[rowIdMovieList + 1].Selected = true;
                AdvanceSelectionDGV1();
                // this.Text = rowIdMovieList.ToString();
            }
        }
        public string GetCurrentMoviePath()
        {
            if (rowIdMovieList < dgv1.RowCount - 1)
            {
                //dgv1.ClearSelection();
                //++rowIdMovieList;
                dgv1.CurrentCell = dgv1.Rows[rowIdMovieList].Cells[0];
                dgv1.Rows[rowIdMovieList].Selected = true;
                //return rowIdMovieList.ToString();
            }
            return tbMovieFpath.Text;
        }
        public void front()
        {
            this.Focus();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void sorter()
        {

        }










        //0 fname 1=rating 2=deleted 3=timestamp 4=size 5=fullpath 6=order
        public int reloadListView()
        {
            return 0;

        }

        //0 fname 1=rating 2=deleted 3=timestamp 4=size 5=fullpath 6=order
        public int rebuildImageList() /////////////////////////////////////////// LOAD LIST VIEW ////////////////
        {

            gv.imageFileList2 = new ImageFileList();
            //replication LIST
            for (int idx = 0; idx < gv.imageFileList1.getImageFileListLength(); ++idx)
            {
                gv.imageFileList2.addItem(gv.imageFileList1.getIndexed(idx));
            }
            gv.debug.w("rebuildImageList: size" + gv.imageFileList1.getImageFileListLength().ToString());

            // reloadListView();
            //   MessageBox.Show("sort completed");
            return gv.imageFileList2.getImageFileListLength();
        }

        private void insertTestRow()
        {


        }


        // This event handler updates the progress bar.
        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            //this.progressBar1.Value = e.ProgressPercentage;
        }

        // This is the method that does the actual work. For this
        // example, it computes a Fibonacci number and
        // reports progress as it does its work.

        private void OnCalculate(object sender, EventArgs e)
        {
            //this.btTraverse.Enabled = true;
            this.tbDirectoryPath.Text = String.Empty;
            this.btCancel.Enabled = true;
            // this.progressBar.Value = 0;


        }

        //private int Add(int x, int y)
        //{
        //   Thread.Sleep(5000);
        //   return x + y;
        //}

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        FolderBrowserDialog selectDirectory = new FolderBrowserDialog();
        string folderName;
        private void dirDialog()
        {

            DialogResult result = selectDirectory.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                folderName = selectDirectory.SelectedPath;
                this.Text = folderName;

            }
        }
        /*
         * if(!fileOpened)
          {
              // No file is opened, bring up openFileDialog in selected path.
              openFileDialog1.InitialDirectory = folderName;
              openFileDialog1.FileName = null;
              openMenuItem.PerformClick();
          } */

        private void buttonTraverse_Click(System.Object sender,
        System.EventArgs e)
        {
            // Reset the text in the result label.
            //this.tbResult.Text = String.Empty;

            //this.btTraverse.Enabled = false;
            this.btCancel.Enabled = true;
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            //  listView1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Bottom;

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FormTraverser_Resize(object sender, EventArgs e)
        {

        }

        private void tbLength_TextChanged(object sender, EventArgs e)
        {

        }
        public void CopyFileList2To1()
        {
            gv.imageFileList1.finfoList.Clear();
            if (bThisIsSubWindow)
            {
                for (int idx = 0; idx < imageFileList2.finfoList.Count(); ++idx)
                {
                    gv.imageFileList2.finfoList.Add(imageFileList2.finfoList[idx]);
                }
            }
            else
            {
                for (int idx = 0; idx < imageFileList1.finfoList.Count(); ++idx)
                {
                    gv.imageFileList1.finfoList.Add(imageFileList1.finfoList[idx]);
                }
            }
            return;
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            CloseTrav();
        }
        public void CloseTrav()
        {
            if (cbCopyFileListMain.Checked)
            {
            }
            if (!bThisIsSubWindow)
            {
                if (mp != null && !mp.IsDisposed)
                    mp.Close();
                if (!bThisIsSubWindow && cbCloseWin2Also.Checked)
                    if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                    {
                        gv.dialogTraverser2.Close();
                    }
                main.Activate();
            }
            this.Close();
        }

        private void buttonCancel_Click_1(object sender, EventArgs e)
        {
            //this.btTraverse.Enabled = true;
            //  this.buttonCancel.Enabled = false;
            this.tbDirectoryPath.Text = ".... cancelling operation ... ";

            if (action == Action.TRAVERSING)
            {
                try
                {
                    this.traverser.cancel();
                }
                catch (Exception)
                {
                    MessageBox.Show("Exception trying backgroundWorker1.CancelAsync()");
                    throw;
                }
            }
            else if (action == Action.LOADING_DISPLAY)
            {

            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxResult_TextChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {

        }

        private void tbType_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbLevel_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbFullPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbCount_TextChanged(object sender, EventArgs e)
        {

        }

        public bool doEnumerateFiles(string root)
        {
            var dirs = Directory.GetDirectories(root);

            return true;
        }

        public void rememberLastFolders(string fpath)
        {
            if (string.IsNullOrEmpty(fpath))
                return;
            AddHistoryItem(fpath);
            if (gv.initParm1List[0].sourceDir1.Equals(fpath))
                return;
            if (gv.initParm1List[0].sourceDir2.Equals(fpath))
                return;
            if (gv.initParm1List[0].sourceDir3.Equals(fpath))
                return;
            if (gv.initParm1List[0].sourceDir4.Equals(fpath))
                return;
            if (gv.initParm1List[0].sourceDir5.Equals(fpath))
                return;
            //if (gv.initParm1List[0].sourceDir6.Equals(fpath))
            //  return;
            if (gv.initParm1List.Count > 0)
            {
                switch (numLastFolder.Value) // + 1)
                {
                    case 1:
                        gv.initParm1List[0].sourceDir1 = fpath;
                        break;
                    case 2:
                        gv.initParm1List[0].sourceDir2 = fpath;
                        break;
                    case 3:
                        gv.initParm1List[0].sourceDir3 = fpath;
                        break;
                    case 4:
                        gv.initParm1List[0].sourceDir4 = fpath;
                        break;
                    case 5:
                        gv.initParm1List[0].sourceDir5 = fpath;
                        break;
                    case 6:
                        gv.initParm1List[0].sourceDir6 = fpath;
                        break;
                    default:
                        // if 0 don't store it
                        break;
                }
            }
        }
        public void assignParmsFromTextBoxes(string d1, string d2, string d3, string d4, string d5, string d6)
        {
            gv.initParm1List[0].sourceDir1 = d1;
            gv.initParm1List[0].sourceDir2 = d2;
            gv.initParm1List[0].sourceDir3 = d3;
            gv.initParm1List[0].sourceDir4 = d4;
            gv.initParm1List[0].sourceDir5 = d5;
            gv.initParm1List[0].sourceDir6 = d6;
        }
        //endstream
        public void endOfSteamMessage()
        {
            tbPlayStateEnding.Text = "ENDING";
            if (!cbCatalog.Checked)
            {
                btFocus.BackColor = Color.Red;
            }
            else
            {
                btFocus.BackColor = Color.LightGreen;
                btFocus2.BackColor = Color.LightGreen;
            }
            if (cbAutoNext.Checked) /////////////////////////////////////////////                if (gv.FILE_EXT != ".AVIF")
                playNextMovie(true); //on EOS
        }
        bool bRestartAfterError = false;
        public void resumeNextPlay()
        {
            if (bRestartAfterError)
            {
                bRestartAfterError = false;
                playFirstVideo();
            }
        }
        public void startStreamMessage()
        {
            tbPlayStateEnding.Text = "Beginning";
        }
        public bool CheckWithParent() //FIRST MESSAGE
        {
            //cbFocusHere.Checked = true;
            if (cbSetAllPlayerModes.Checked)
            {
                cbFocusHere.Checked = true;
                //if (!cbSmallSize.Checked)
                //  cbZoom.Checked = true;
                cbAutoPlay.Checked = true;
            }
            return cbSetAllPlayerModes.Checked;
        }

        public void setLoadedMovieTrue()
        {
            cbAutoAdv.Enabled = true;
            cbZoom.Enabled = true;
            cbCatalog.Enabled = true;
        }

        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
        }
        public bool TraverseGo()
        {
            dgv1.DataSource = null;
            ResetSortButtonColor(null);
            if (false)  // normalize
            {
                if (tbDirectoryPath.Text.Contains(@"\\"))
                {
                    string text = tbDirectoryPath.Text.Replace(@"\\", @"/");
                    tbDirectoryPath.Text = text;
                }
            }
            if (!Directory.Exists(tbDirectoryPath.Text))
            {
                return false;
            }
            //AddHistoryItem(tbDirectoryPath.Text);
            bAllowMovieSelection = false;
            gv.debug.w("traverse");
            setLoadedMovieTrue();
            doTraversal(tbDirectoryPath.Text);
            return true;
        }
        public void FindLastFolder(string fpath, string dialog) // "main" "sub"
        {
            FolderHistory result = gv.folderHistoryList.Where(f => f.action.Equals(dialog)).First();
            // int result = gv.folderHistoryList.FindIndex(a => a.parmName.Equals(path));
            if (result != null)
            {
                result.action = dialog;
                return;
            }
            // gv.folderHistoryList.Add(fpath);
        }
        public void ClearFolderListAction(string dialog)
        {
            for (int idx = 0; idx < gv.folderHistoryList.Count; ++idx)
            {
                if (gv.folderHistoryList[idx].action.Equals("dialog"))
                {
                    gv.folderHistoryList[idx].action = "";
                    return;
                }
            }
        }
        public void AddHistoryItem(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            FolderHistory fpath = new FolderHistory();
            fpath.folderPath = path;
            //var result = gv.folderHistoryList.Exists(parmName => parmName.Equals(path));
            // Where(f => f.fname.ToUpper().Contains(tbSearchItem.Text.ToUpper())).ToList();

            List<FolderHistory> result = gv.folderHistoryList.Where(f => f.folderPath.Equals(path)).ToList();
            // int result = gv.folderHistoryList.FindIndex(a => a.parmName.Equals(path));
            if (result.Count > 0)
                return;
            gv.folderHistoryList.Add(fpath);
        }
        private void saveFolderName() //dirname
        {

        }
        bool bAllowMovieSelection = false;
        public void TraverseFolder(string dpath)
        {
            if (!string.IsNullOrEmpty(dpath))
            {
                btTraversing.Visible = true;
                if (Directory.Exists(dpath))
                {
                    if (cbAllowMovieSelection.Checked)
                    {
                        setLoadedMovieTrue();
                    }
                    doTraversal(dpath);
                    gv.lastTraversedFolder = dpath;

                    //foreach (var mc in gv.initParmItemList.Where(x => x.lastDir == "lastDir"))
                    //  mc.Value = dpath;

                    rememberLastFolders(dpath);
                    //main.saveInitParms();
                }
                else
                    dpath = "no directory";
            }
            else
                btTraversing.Visible = false;
            this.Text = dpath;
        }
        //
        // returns gv.imageFileList
        //
        private void doTraversal(string dpath)
        {
            if (string.IsNullOrEmpty(dpath))
            {
                MessageBox.Show($"fpath fail {dpath}", "ERROR fpath");
                return;
            }
            if (bThisIsSubWindow)
            {
                gv.initParm1List[0].mostRecentSubfolder = dpath;
            }
            else
            {
                gv.initParm1List[0].mostRecent = dpath;
                gv.lastTraversedFolder = dpath;
            }
            bLoadedList = true;
            rememberLastFolders(dpath);
            btTraversing.Visible = true;
            //2021
            //gv.initParm1List[0].sourceDir1 = dpath;
            //tbDirectoryPath.Text = gv.initParm1List[0].sourceDir1;
            tbDirectoryPath.Text = dpath;
            tbCopyFileName.Text = "";

            // registryWriteDirectory(dpath);

            btCancel.Enabled = true;
            tbCount.BackColor = Color.White;
            gv.setCursorHourGlass();
            action = Action.TRAVERSING;
            TraverserBG traverser = new TraverserBG(gv, this);
            //uses gv.imageFileList
            if (!bThisIsSubWindow)
            {
                if (gv.bImageFileList1Loaded)
                {
                    gv.imageFileList1.clearList();
                    gv.bImageFileList1Loaded = false;

                }
            }
            if (bThisIsSubWindow)
            {
                gv.imageFileList2 = new ImageFileList();
                gv.bLoadingTraverser2 = true;
            }
            else
            {
                gv.imageFileList1 = new ImageFileList();
                gv.deleteFileList1 = new ImageFileList();
                gv.bLoadingTraverser2 = false;
            }
            //get 2 levels of subfolders for  %completion estimate   doEnumerateFiles(gv.dirFullpath);
            ARGS args = new ARGS();
            //args.dirpath = gv.initParm1List[0].sourceDir1;
            args.dirpath = dpath;
            args.bMovies = cbMovie.Checked;
            args.bPictures = cbPicture.Checked;
            args.bWEBM = cbWEBM.Checked;
            // args.bWEBP = cbWEBM.Checked;
            args.bMIDI = cbMIDI.Checked;
            args.bALL = cbFilesAll.Checked;

            traverser.doTraverseFolders(args);
            saveFolderName();
        }
        bool bLoadedList = false;
        //
        // COMPLETED 
        //
        public void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e) //on work completed findlist
        {
            gv.setCursorDefault();
            btCancel.Enabled = false;
            btTraversing.Visible = false;
            bAllowMovieSelection = true;

            Cursor.Current = Cursors.WaitCursor;

            if (e.Cancelled)
            {
                this.Text = "Cancelled";
                action = Action.NONE;
                btTraversing.Hide();
            }
            else
            {
                ////  main.directoryPath(tbDirectoryPath.Text + " " + gv.imageFileList.getIndexed(0));

                gv.mainWindow.setTitle(gv.imageFileList1.getIndexed(0), tbDirectoryPath.Text);

                action = Action.COMPLETED;

                this.tbDirectoryPath.Text = e.Result.ToString();
                if (gv.messageCount > 0)
                    this.Text = gv.getMessage();
                else
                    this.Text = e.Result.ToString();
                btClose.BackColor = Color.Aqua;
                setReadOnly();
            }
            if (bThisIsSubWindow)
            {
                gv.slideCount2 = gv.slideCount0;
                tbCount.Text = gv.slideCount2.ToString();
                cbLoadedAndReady.Checked = true;

            }
            else
            {
                gv.slideCount1 = gv.slideCount0;
                tbCount.Text = gv.slideCount1.ToString();
                cbLoadedAndReady.Checked = true;

            }
            tbCount.BackColor = Color.Yellow;

            if (bThisIsSubWindow)
            {
                imageFileListOriginal2 = gv.imageFileList2;
                imageFileList2 = gv.imageFileList2; //initial backup of original <<<<<<<<<<<<<<<<<<<<<<<<<<<<
            }
            else
            {
                imageFileListOriginal1 = gv.imageFileList1;
                imageFileList1 = gv.imageFileList1; //initial backup of original <<<<<<<<<<<<<<<<<<<<<<<<<<<<
            }
            //this.btTraverse.Enabled = true;
            //   this.buttonCancel.Enabled = false;
            this.progressBar1.Value = 100;

            //dmc2022fixthis

            Cursor.Current = Cursors.Default;
            //this.dataGridView1.Sort(this.dataGridView1.Columns["Name"], ListSortDirection.Ascending);
            //dgvFileList1.Sort(dgvFileList1.Columns[0], ListSortDirection.Ascending);
            formatDataGridViewFileList(); //??????

            SortName();
        }

        public void OnCancel(string stat)
        {
            //backgroundWorker1.CancelAsync();
            btTraversing.Visible = false;
            this.tbDirectoryPath.Text = "Cancelled.";
            action = Action.NONE;
        }
        int progressFactor = 1;
        public void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 99)
                progressFactor += 1;
            this.progressBar1.Value = e.ProgressPercentage / progressFactor;
        }

        private void FormTraverser2_Resize(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void FormTraverser2_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (gv.imageFileList != null)
            //if (gv.imageFileList.getImageCount() > 0)
            // gv.mainWindow.displayThisImage(gv.imageFileList.getIndexed(0));
        }

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int index = e.ItemIndex;
            //   gv.debug.w("item changed", index);
            ListViewItem lvItem = e.Item;


            if (bThisIsSubWindow)
                return;
            if (e.IsSelected)
            {
                //     gv.debug.winfo(e.Item);
                //gv.debug.w(e.Item.GetSubItemAt(0, 1).ToString());
                gv.nextIdx = index;
            }

        }

        private void btTraverser1_Click(object sender, EventArgs e)
        {

        }

        public bool registryWriteDirectory(string dpath)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);

            RegistryKey myKey = key.CreateSubKey("DZEN");

            myKey.SetValue("dpath", dpath);/////////////, RegistryValueKind.String);
            return true;

        }

        public string registryReadDirectory()
        {
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\DZEN", false);

            string myValue = (string)myKey.GetValue("dpath");
            return myValue;
        }

        private void AcceptSort_Click_1(object sender, EventArgs e)
        {
            tbLoadedCount.Text = "";
            this.rebuildImageList();
            //   FileFunctions ff = new FileFunctions(gv);
            //   ff.createIniFile(gv.dirFullpath);
            this.Close();
        }

        public void setNoSort()
        {
            lvwColumnSorter.Order = SortOrder.None;
        }



        Boolean insertRangeB = true;

        private void btDisplayList_Click(object sender, EventArgs e)
        {
            gv.setCursorHourGlass();
        }







        private delegate void UpdateListViewDelegate(int idx, int count);//string newText);





        private void cbDone_CheckStateChanged(object sender, EventArgs e)
        {

        }

        private void cbDone_CheckedChanged(object sender, EventArgs e)
        {

        }



        System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
        System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

        public void InitializeOpenFileDialogXML()
        {
            if (this.openFileDialog1 == null)
                this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "XML Files (*.XML)|*.XML";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = false;// .SupportMultiDottedExtensions = true; //.Multiselect = true;
            this.openFileDialog1.Title = "Select Image List File (.XML)";

        }

        public void InitializeSaveFileDialogXML()
        {
            if (this.saveFileDialog1 == null)
                this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            // Set the file dialog to filter for graphics files.
            this.saveFileDialog1.Filter =
                "XML Files (*.XML)|*.XML";

            // Allow the user to select multiple images.
            //this.saveFileDialog1.Multiselect = false;// .SupportMultiDottedExtensions = true; //.Multiselect = true;
            this.saveFileDialog1.Title = "Save Image List File as (.XML)";

        }
        string imageFileName = "none";
        private void btSave_Click(object sender, EventArgs e) // XML save file 
        {
            if (!gv.bImageFileList1Loaded)
                return;
            InitializeSaveFileDialogXML();
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fpath = saveFileDialog1.FileName;
                imageFileName = Path.ChangeExtension(fpath, "xml");
                tbFileName.Text = imageFileName;
                SaveFile1();
            }
        }
        public void SaveFile1()
        {
            tbFoundRowNumber.Text = tbRowIdTemp.Text;
            //XML Serializer 
            if (imageFileList1 == null && !bThisIsSubWindow)
            {
                MessageBox.Show("NULL ImageFileListOriginal");
                return;
            }
            if (imageFileList2 == null && bThisIsSubWindow)
            {
                MessageBox.Show("NULL ImageFile2ListOriginal");
                return;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoItem>));
            TextWriter textWriter = new StreamWriter(imageFileName);
            try
            {
                if (bThisIsSubWindow)
                    serializer.Serialize(textWriter, imageFileList2.finfoList);  // this saves List<FileInfoItem> 
                else
                    serializer.Serialize(textWriter, imageFileList1.finfoList);  // this saves List<FileInfoItem> 
            }
            catch (Exception ex)
            {

                gv.debug.w("Serilize Exception ", ex.ToString(), ex.Message);
            }
            textWriter.Close();
            bSavedFile = true;
        }
        int countUpdates = 1;
        bool bSavedFile = false;

        public void UpdateXmlFileList()
        {
            if (string.IsNullOrEmpty(imageFileName) || imageFileName.Equals("none"))
                return;
            if (bSavedFile)
            {
                SaveFile1();
            }
        }

        private void resaveImageFileList()
        {
            if (!bSavedFile)
                return;
            SaveFile1();
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

        private void btLoadImageFileList_Click(object sender, EventArgs e)
        {
            if (bThisIsSubWindow)
                LoadImageListFile2();
            else
                LoadImageListFile1();
            gv.mainWindow.SetFileType("images");
        }

        private void btLoadVideoFile_Click(object sender, EventArgs e)
        {
            if (bThisIsSubWindow)
                LoadImageListFile2();
            else
                LoadImageListFile1();
            gv.mainWindow.SetFileType("videos");
        }
        public void LoadImageListFile1()
        {
            InitializeOpenFileDialogXML();
            cbAutoAdv.Enabled = true;
            cbCatalog.Enabled = true;
            bLoadedList = true;
            //2022
            gv.imageFileList1 = new ImageFileList();
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (imageFileList1 == null)
                    imageFileList1 = new ImageFileList();
                gv.setCursorHourGlass();
                //
                imageFileList1.finfoList = DeserializeFromXML(openFileDialog1.FileName);
                //
                gv.imageFileList1.finfoList = imageFileList1.finfoList;
                imageFileName = openFileDialog1.FileName;
                tbFileName.Text = openFileDialog1.FileName;
                gv.nextIdx = 0;
                gv.bImageFileList1Loaded = true;
                gv.slideCount1 = imageFileList1.getImageFileListLength();
                imageFileList2 = imageFileList1;
                //this.loadListViewRange();
                SortName();
                bSavedFile = true;
            }
            gv.setCursorDefault();
            formatDataGridViewFileList();
        }

        public void LoadImageListFile2()
        {
            InitializeOpenFileDialogXML();
            cbAutoAdv.Enabled = true;
            cbCatalog.Enabled = true;
            bLoadedList = true;
            //2022
            gv.imageFileList1 = new ImageFileList();
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (imageFileList2 == null)
                    imageFileList2 = new ImageFileList();
                gv.setCursorHourGlass();
                //
                imageFileList2.finfoList = DeserializeFromXML(openFileDialog1.FileName);
                //
                gv.imageFileList1.finfoList = imageFileList2.finfoList;
                imageFileName = openFileDialog1.FileName;
                tbFileName.Text = openFileDialog1.FileName;
                gv.nextIdx = 0;
                gv.bImageFileList1Loaded = true;
                gv.slideCount2 = imageFileList2.getImageFileListLength();
                // imageFileList2 = imageFileList2;
                //this.loadListViewRange();
                SortName();
                bSavedFile = true;
            }
            gv.setCursorDefault();
            formatDataGridViewFileList();
        }


        private void writeToTextFile()
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {



            }
        }
        private void writeListView2File(string targetFile)
        {

        }

        public void readImageFIle()
        {


        }
        private void btDBwrite_Click(object sender, EventArgs e)
        {

        }

        public void setRootDirDefault()
        {
            // RootSetterDialog rd = new RootSetterDialog(gv, null);

            // rd.Visible = true;
        }
        private void btSetRootDir_Click(object sender, EventArgs e)
        {
            //  RootSetterDialog rd = new RootSetterDialog(gv, null);

            // rd.Visible = true;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {

        }

        private void button1_Click_3(object sender, EventArgs e)
        {

        }



        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }
        private void rbProfile_CheckedChanged_1(object sender, EventArgs e)
        {
            bUseSpecial = true;
            setFolder();
        }
        private void setFolder()
        {
            if (this.radioButton1.Checked)
            {
                startFolder = InitFolder.MyComputer;
            }
            else if (this.radioButton3.Checked)
            {
                startFolder = InitFolder.MyDocs;

            }
            else if (this.radioButton2.Checked)
            {
                startFolder = InitFolder.MyPics;
            }
            else if (this.radioButton4.Checked)
                startFolder = InitFolder.C_Drive;
            else if (this.rbDesktop.Checked)
                startFolder = InitFolder.MyDesktop;
            else if (this.rbProfile.Checked)
            {

                startFolder = InitFolder.Special;/// tbDirectoryPath.Text;
                //tbDirectoryPath.Text = @"C:";


            }
        }

        private void cbProgress_CheckedChanged(object sender, EventArgs e)
        {
            gv.bShowProgress = cbProgress.Checked;
        }

        private void cbTraverse_CheckedChanged(object sender, EventArgs e)
        {

        }
        public FolderBrowserDialog directoryPromptDialog2;
        public string promptedFolderName;


        bool bUseSpecial = false;
        public string GetStartingFolder()
        {
            // RootSetter setRootDirectory = new RootSetter();

            //  this.LVcs_ImageList.Clear();
            //  CreateListView1();
            //tbDirectoryPath.Text = " ";
            //btTraversing.Visible = true;
            gv.directoryPromptDialog1 = new FolderBrowserDialog();
            var x = gv.directoryPromptDialog1.RootFolder;
            string dpath = null;
            if (cbTraverseThisFolder.Checked) // 7/6/2019
            {
                dpath = gv.dirDialog(startFolder, null, tbDirectoryPath.Text);// (startFolder may have to be set to Special or Desktop??
            }
            else
            {
                if (bUseSpecial)
                    dpath = gv.dirDialog(InitFolder.Special);///, dpath);// + "/");
                else
                    dpath = gv.dirDialog(startFolder);// (startFolder);  //dirDialog dmcdmc123
            }
            tbDirectoryPath.Text = dpath;
            this.Refresh();
            tbDirectoryPath.Refresh();
            gv.debug.w("got folder ", dpath);
            return dpath;
        }

        private void btDirectoryDialog_Click(object sender, EventArgs e)
        {
            GetStartingFolder();
        }

        private void tbDirectoryPath_Enter(object sender, EventArgs e)
        {
            //gv.initParm1List[0].lastDir1 = tbDirectoryPath.Text;
        }

        private void tbDirectoryPath_TextChanged(object sender, EventArgs e)
        {
            //gv.initParm1List[0].lastDir1 = tbDirectoryPath.Text;
        }
        int readMax = 0;
        private void tbMax_TextChanged(object sender, EventArgs e)
        {
            try
            {
                readMax = Convert.ToInt32(this.tbMax.Text);
            }
            catch (Exception)
            {

                gv.debug.w("INVALID MAX NUMBER");
                return;
            }
            gv.iMaxFileCount = readMax;
        }

        private void btHigherFolder_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = System.IO.Directory.GetParent(tbDirectoryPath.Text).ToString();
            startFolder = InitFolder.Special;
        }

        private void btParentFolder_Click(object sender, EventArgs e)
        {
            //tbDirectoryPath.Text
        }


        WmPlayer mp;
        WmPlayer mp2;
        public void resetTimerRequestToMovies()
        {
            this.Text = "";
            bReceivedTimerMessage = false;
            if (cbZoom.Checked)
            {
                bDidZoom = false;
            }
            resetMessageToZero();
        }
        private void btPlay_Click(object sender, EventArgs e)
        {
            cbLoadedAndReady.Checked = true;
            dgv1_SelectionChanged(null, null);
            PlayButtonGo();
            OpenTransparentWin();

            if (cbSearchWin2EachSelection.Checked && (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed))
                ClearAndSearch2();
        }
        public bool mpActive()
        {
            return (mp != null && !mp.IsDisposed);
        }

        int durationOfSampleSeconds = 3;
        public void PlayButtonGo()
        {
            tbCountM.Text = "1";
            messageCount = 1;
            counttest = 0;
            if (mp != null && mp.IsDisposed)
                mp.Close();
            ClearError();
            if (cbFilesAll.Checked)
                return;
            playFirstVideo();
            if (cbLongerSegments.Checked)
                mp.setSlowerDelay(true, durationOfSampleSeconds);
            if (cbHidePlayer.Checked)
                mp.HidePlayer();
            // if (cbSlideShow.Checked)
            //mp.SetTimerSeconds(3);
        }

        public void ShowInBrowser(string fpath)
        {
            if (!File.Exists(fpath))
                return;
            System.Diagnostics.Process.Start("Chrome", Uri.EscapeDataString(fpath));
            //System.Diagnostics.Process.Start(fpath);
        }
        public bool CheckFileTypeWEBM(string fpath)
        {
            string ext = Path.GetExtension(fpath);
            ext = ext.ToUpper();
            if (ext.Contains("WEBM"))
            {
                this.Text = fpath;
                ShowInBrowser(fpath);
                return true;
            }
            return false;
        }
        public void resetMpPlay()
        {
            resetingCbSlow = true;
            cbSlow.Checked = false;
            cbSlower2.Checked = false;
            cbPause2.Checked = false;
            resetingCbSlow = false;
            bPlayingEnding = false;
            btGoToEnd15.BackColor = Color.LightGray;
        }
        public void Resume()
        {
            if (mp == null || mp.IsDisposed)
            {
                if (mpVersion1)
                    mp = new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                else
                    mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                mp.Activate();
                mp.Show();
                gv.SetMovies(mp);
            }
        }
        public void MovieWindowCreated()
        {
            cbReceivedFirstMessage.Checked = false;
        }
        public void playFirstVideo()
        {
            resetMpPlay();
            if (CheckFileTypeWEBM(tbMovieFpath.Text))
                return;
            bPlayedFirstVideo = true;
            if (mp == null || mp.IsDisposed)
            {
                if (mpVersion1)
                    mp = new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                else
                    mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                mp.Activate();
                mp.Show();
                gv.SetMovies(mp);
                if (cbMute.Checked)
                    mp.mute(cbMute.Checked);
            }
            else
            {
                if (CheckFileTypeWEBM(fpath))
                    return;
                try
                {
                    LoadMovie(tbMovieFpath.Text, cbZoom.Checked); // playFirstVideo
                    TrackFromSourceLoad("playFirstVideo");

                }
                catch
                {
                    MessageBox.Show("TEST");
                }
            }
            DisplayMovieResolution();
            if (cbBeep.Checked)
            {
                cbFocusHere.Checked = true;
                //cbZoom.Checked = true;
            }
            mp.zoomFullScreen(cbZoom.Checked);
            resetTimerRequestToMovies(); // send after mp.loadMovie
            resetMessageToZero();
            mp.SetCallParent(true);
        }
        bool bUseOverlay = false;
        TransparentForm formTransparent;
        public void OpenTransparentWin()
        {
            if (mp == null || mp.IsDisposed)
                return;
            if (bUseOverlay)
            {
                if (formTransparent == null || formTransparent.IsDisposed)
                {
                    formTransparent = new TransparentForm(gv, mp.Location);
                }
                formTransparent.Activate();
                formTransparent.Show();
                // formTransparent.TopMost = true;
            }
        }
        public void OpenOverlay()
        {
            if (formTransparent == null || formTransparent.IsDisposed)
            {
                if (mp == null)
                    formTransparent = new TransparentForm(gv, this.Location);
                else
                    formTransparent = new TransparentForm(gv, mp.Location);
            }
            formTransparent.Activate();
            formTransparent.Show();
            //formTransparent.Parent = this;
            //formTransparent.TopMost = true;
        }
        public void UpdateFinfoMovieResolution(FileInfoItem fi)
        {
            if (mp == null || mp.IsDisposed)
                return;
            Point xy = mp.getResolution();
            if (xy.X == 0)
                return;
            dur = mp.getDuration();
            fi.SetResolution(xy.X, xy.Y);
            fi.playTime = (double)dur;
            fi.minutes = (int)dur / 60;
            fi.seconds = (int)dur - ((int)(dur / 60) * 60);
        }
        public bool bDisplayedResolution = false;
        public void DisplayMovieResolution()
        {
            if (mp == null || mp.IsDisposed)
                return;
            Point xy = mp.getResolution();
            if (xy.X == 0)
                return;
            dur = mp.getDuration();
            bDisplayedResolution = true;
            tbResolution.Text = $"{xy.Y} x {xy.X}";
            if (xy.X >= 1080)
            {
                tbResolution.BackColor = Color.LightGreen;
            }
            else if (xy.X >= 720)
            {
                tbResolution.BackColor = Color.LightBlue;
            }
            else
                tbResolution.BackColor = Color.LightGray;
            this.Refresh();

            if (rowIdMovieList >= 0)
            {
                if (dgv1.RowCount < rowIdMovieList)
                {
                    return;
                }
                if (xy.X > 0)
                {
                    dgv1.Rows[rowIdMovieList].Cells["height"].Value = xy.X;
                    dgv1.Rows[rowIdMovieList].Cells["width"].Value = xy.Y;

                    dgv1.Rows[rowIdMovieList].Cells["playTime"].Value = dur;
                    dgv1.Rows[rowIdMovieList].Cells["minutes"].Value = (int)dur / 60;
                    dgv1.Rows[rowIdMovieList].Cells["seconds"].Value = (int)dur - ((int)(dur / 60) * 60);

                    // if (cbGetMetaDataOnly.Checked) //DisplayMovieResolution
                    //   GoToEnd(cbShortEndSample.Checked ? 5 : 15);

                }
            }
            if (cbGetMetaDataOnly.Checked)
                playNextMovie();
        }
        void formatDataGridViewFileList(int column0Width = 0) //2020
        {
            for (int idx = 0; idx < dgv1.Columns.Count; idx++)
            {

                dgv1.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                int colw = dgv1.Columns[idx].Width;
                dgv1.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgv1.Columns[idx].Width = colw;
                if (idx == 0 && column0Width > 0)
                    dgv1.Columns[idx].Width = column0Width;
                else if (idx == 4)
                    dgv1.Columns[idx].Width = 6;
                else if (idx <= 6)
                    dgv1.Columns[idx].ReadOnly = true;
            }
            return;
            //dgvFileList.Columns[0].Width = 350;
            //dgvFileList.Columns[1].Width = 600;
            int colCount2 = dgv1.Columns.Count;
            int colCount = this.dgv1.Columns.Count; // this returns the total number of columns (=6)
            //colCount = 3;                                                //MessageBox.Show(colCount.ToString());
            //colCount = colCount - 1; // =5
            for (int idx = 0; idx < colCount; idx++)
            {
                DataGridViewColumn column = dgv1.Columns[idx]; // column[1] selects the required column 
                if (idx != 4)
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; // sets the AutoSizeMode of column defined in previous line
                else
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                int colWidth = column.Width; // store columns width after auto resize
                                             //MessageBox.Show(colWidth.ToString()); // show me the autoresize width (used as a visual check really)
                if (idx == 4)
                    colWidth = 22;
                //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // set the column resize mode to 'none' to allow manual/program changes
                //colWidth += 20; // add 20 pixels to what 'colWidth' already is
                this.dgv1.Columns[idx].Width = colWidth; // set the columns width to the value stored in 'colWidth'

                //if ((idx == colCount - 1))
                //  column.DefaultCellStyle.Format = "yyyy.MM.dd HH:mm:ss";
            }
            this.Refresh();
        }
        private void btClear_Click(object sender, EventArgs e)
        {
            dgv1.DataSource = null;
            if (imageFileList2 != null && imageFileList2.getImageCount() > 0)
                imageFileList2.clearList();
        }
        int rowIdMovieList;
        bool bPlayedFirstVideo = false;
        string oldName;
        //
        int countChanges = 0;
        int previousRowIdWas = -1;
        int newRowIdShouldBe = 0;
        bool doingRetrigger = false;
        private void dgv1_SelectionChanged(object sender, EventArgs evtArgs)
        {
            if (!cbLoadedAndReady.Checked)
                return;

            if (cbCompareToPreviousImage.Checked)
            {
                pb2.Image = pbThumbNail.Image;
                CompareImagesInSequence();
            }


            try
            {
                rowIdMovieList = (int)dgv1.CurrentCell.RowIndex;

                if (rowIdMovieList == previousRowIdWas)
                {

                }
                tbRowIdTemp.Text = rowIdMovieList.ToString();
                tbRowId.Text = rowIdMovieList.ToString();
            }
            catch
            {
                //this.debug.w("marketDataGrid No selected rows or empty set");
                rowIdMovieList = -1;
                tbMovieFpath.Text = "ERRORbbb";
                tbRowIdTemp.Text = "err";
                return;
            }

            tbMovieFpath.Text = dgv1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
            tbCurrentFolder.Text = ff.getFolder(tbMovieFpath.Text);
            tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
            tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
            fpath = tbMovieFpath.Text;
            tbFileNameSelected.Text = dgv1.Rows[rowIdMovieList].Cells["fname"].Value.ToString();
            gv.FILE_EXT = Path.GetExtension(tbFileNameSelected.Text);
            if (cbThumbNail.Checked)
            {
                pbThumbNail.Image = gv.mainWindow.CreateThumbnailForFile(tbMovieFpath.Text);
            }



            if (!bThisIsSubWindow)
            {
                if (previousRowIdWas == rowIdMovieList)
                {
                    if (bPlayedFirstVideo && rowIdMovieList >= 0)
                        if (retriggerForRowId > rowIdMovieList) //&& retriggerForRowId < dgv1.RowCount)
                        {
                            tbMovieFpath.Text = dgv1.Rows[rowIdMovieList + 1].Cells["fpath"].Value.ToString();
                            tbCurrentFolder.Text = ff.getFolder(tbMovieFpath.Text);
                            tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
                            tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
                            fpath = tbMovieFpath.Text;
                            tbFileNameSelected.Text = dgv1.Rows[rowIdMovieList + 1].Cells["fname"].Value.ToString();
                            rowIdMovieList = retriggerForRowId;
                            retriggerForRowId = -1;
                        }
                        else
                            retriggerForRowId = -1;
                }
                previousRowIdWas = rowIdMovieList;
                DGV1_PERFORM_SelectionChanged();

                if (cbSearchWin2EachSelection.Checked && !bThisIsSubWindow)
                {
                    string fpath = tbMovieFpath.Text;
                    if (fpath.Length > 0)
                        SearchWin2();
                    /*
                    lblSEARCH2state.Text = "s";
                    if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                        if (foundRowIdIn2ndWindow < 0)
                            SearchIn2ndWindowWas(fpath);
                    */
                }

                if (false)
                {
                    retriggerDGV1 = false;
                    retriggerForRowId = rowIdMovieList;
                    //  dgv1_SelectionChanged(null, null);
                }
            }
        }
        int retriggerForRowId = 0;
        bool retriggerDGV1 = false;
        private void xxDg1_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv1.CurrentCell == null)
            {
                return;
            }
            if (!dgv1.Focused)
            {
                //  return;
            }
            if (previousRowIdWas >= 0)
            {
                //  if (dgv1.SelectedRows.Count < 1)
                //  return;
            }
            try
            {
                rowIdMovieList = (int)dgv1.CurrentCell.RowIndex;
                tbRowIdTemp.Text = rowIdMovieList.ToString();
            }
            catch
            {
                //this.debug.w("marketDataGrid No selected rows or empty set");
                rowIdMovieList = -1;
                tbMovieFpath.Text = "ERRORbbb";
                tbRowIdTemp.Text = "err";
                return;
            }
            if (previousRowIdWas == rowIdMovieList)
            {
                if (!bThisIsSubWindow)
                    return;
            }
            previousRowIdWas = rowIdMovieList;
            if (rowIdMovieList >= dgv1.RowCount - 1)
            {
                StopPlaying();
            }
            DGV1_EFFECT_SelectionChanged(rowIdMovieList); //img this also displays thumbnail
            if (cbCompareToPreviousImage.Checked)
            {
                CompareImageWithTraverser2();
            }
            else if (cbCompare.Checked)
            {
                CompareImageWithTraverser2();
            }
            else if (bComparisionModeInTrav1 && bThisIsSubWindow) //  in Traverser2
            {
                // gv.dialogTraverser.CompareImageWithTraverser2();
            }
        }
        public bool bComparisionModeInTrav1 = false;
        bool bErrReset = false;
        int lastRowIdSet = 0;
        int previousRowId = -1;
        bool foundInWin2 = false;

        public void StopPlaying()
        {
            cbAutoAdv.Checked = false;
            cbAutoNext.Checked = false;
            cbAutoPlay.Checked = false;
            cbGetMetaDataOnly.Checked = false;
        }
        public void GetBmp2()
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                pb2.Image = gv.dialogTraverser2.img1;
            }

        }
        public String GetFilePath()
        {
            return dgv1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
        }
        public Bitmap img1;
        bool bInMainWindow = false;
        public void DGV1_PERFORM_SelectionChanged()
        {
            if (cbAutoPlay.Checked) //cbAutoAdv.Checked && 
            {
                if (!bPlayedFirstVideo)
                    return;
                if (mp == null || mp.IsDisposed)
                {
                    if (mpVersion1)
                        mp = new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                    else
                        mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                    mp.Activate();
                    mp.Show();
                    gv.SetMovies(mp);
                    this.Focus();
                }
                else
                {
                    if (string.IsNullOrEmpty(fpath) || string.IsNullOrWhiteSpace(fpath))
                    {
                        this.Text = rowIdMovieList.ToString();
                        if (rowIdMovieList < 0)
                            MessageBox.Show("ERROR DialogTraverser rowIdMovieList");
                        return;
                    }
                    if (CheckFileTypeWEBM(fpath))
                        return;
                    resetMpPlay();
                    if (!cbRefreshForceOnScan.Checked)
                    {
                        bool loadNext = LoadMovie(fpath, cbZoom.Checked); //DVG1 SELECTION_CHANGED   IF autoplay
                        if (loadNext)
                            playNextMovie(true);
                    }
                    TrackFromSourceLoad("DVG1 SELECTION_CHANGED");

                    resetTimerRequestToMovies();// send after mp.loadMovie
                    resetMessageToZero();
                }
                mp.zoomFullScreen(cbZoom.Checked);
                bReceivedTimerMessage = false;
                resetMessageToZero();
                if (cbCompareToPreviousImage.Checked)
                {
                    // pb2.Image = pbThumbNail.Image;
                }
            }
        }
        /*
         * if (cbSearchWin2EachSelection.Checked && !bThisIsSubWindow)
                    {
                        //btSearchWin2_Click(null, null);
                        SearchWin2();
                        /*
                        lblSEARCH2state.Text = "s";
                        if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                            if (foundRowIdIn2ndWindow < 0)
                                SearchIn2ndWindowWas(fpath);
                        
                    }
        */

        public void DGV1_EFFECT_SelectionChanged(int rowIdMovieListNOW)
        {
            //tbResult.Text = rowIdMovieList.ToString();
            //  if (!cbAllowMovieSelection.Checked)
            //     return;

            if (dgv1.SelectedRows.Count > 1)
                MessageBox.Show("multiselect", "error");

            // if (rowIdMovieListNOW != rowIdMovieList)
            //    rowIdMovieList = rowIdMovieListNOW;

            tbFileSelected.Text = dgv1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
            filelen = (long)dgv1.Rows[rowIdMovieList].Cells["len"].Value;
            tbCurrentFileSize.Text = $"{dgv1.Rows[rowIdMovieList].Cells["len"].Value:N0}";

            if (cbThumbNail.Checked)
            {
                //pbThumbNail.Image = gv.mainWindow.CreateThumbnailForFile(tbFileSelected.Text);
                pbThumbNail.Image = gv.mainWindow.CreateThumbnailForFile(tbMovieFpath.Text);
                img1 = gv.mainWindow.CreateThumbnailForFile(tbFileSelected.Text);
                if (bThisIsSubWindow)
                {
                    if (bComparisionModeInTrav1)
                        gv.dialogTraverser.SendUpdatePb2FromSub(img1);
                }
            }
            oldName = dgv1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
            //
            if (bErrReset)
            {
                bErrReset = false;   //2021 after err may need to increment rowIdMovieList   //note removed Movies.close on err dmcdmc
            }
            tbRowId.Text = rowIdMovieList.ToString();
            //
            tbMovieFpath.Text = dgv1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
            tbCurrentFolder.Text = ff.getFolder(tbMovieFpath.Text);
            tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
            tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
            fpath = tbMovieFpath.Text;
            tbFileNameSelected.Text = dgv1.Rows[rowIdMovieList].Cells[0].Value.ToString();
            if (formTransparent != null && !formTransparent.IsDisposed)
                formTransparent.FileName(tbFileNameSelected.Text);

            btSearchWin2.BackColor = Color.LightGray;
            if (!bThisIsSubWindow && cbSearchWin2EachSelection.Checked)
            {
                foundInWin2 = SearchTrav2DialogFileList(); // tbFileNameSelected.Text);
            }
            else if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                if (cbRefreshForceOnScan.Checked)
                    this.Refresh();
                if (previousRowId == rowIdMovieList)
                {

                }
                else
                {
                    if (cbSearchWin2EachSelection.Checked)
                    {
                        //found = SearchOtherTMain("2"); // tbFileNameSelected.Text);
                        foundInWin2 = ClearAndSearch2();
                        if (!bThisIsSubWindow && cbSearchUntilNotFoundIn2.Checked) //DGV1_EFFECT_SelectionChanged
                            if (foundInWin2)
                            {
                                SetFlagScanningGoToNextMovie(true);
                                tbFoundRowNumber.Text = foundRowIdIn2ndWindow.ToString();
                            }
                            else
                            {
                                SetFlagScanningGoToNextMovie(false);
                            }

                    }
                }
            }
            else
                previousRowId = rowIdMovieList;

            if (cbAutoPlay.Checked) //cbAutoAdv.Checked && 
            {
                if (!bPlayedFirstVideo)
                    return;
                if (mp == null || mp.IsDisposed)
                {
                    if (mpVersion1)
                        mp = new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                    else
                        mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                    mp.Activate();
                    mp.Show();
                    gv.SetMovies(mp);
                    this.Focus();
                }
                else
                {
                    if (string.IsNullOrEmpty(fpath) || string.IsNullOrWhiteSpace(fpath))
                    {
                        this.Text = rowIdMovieList.ToString();
                        if (rowIdMovieList < 0)
                            MessageBox.Show("ERROR DialogTraverser rowIdMovieList");
                        return;
                    }
                    if (CheckFileTypeWEBM(fpath))
                        return;
                    resetMpPlay();
                    if (!cbRefreshForceOnScan.Checked)
                    {
                        bool loadNext = LoadMovie(fpath, cbZoom.Checked); //DVG1 SELECTION_CHANGED   IF autoplay
                        if (loadNext)
                            playNextMovie(true);
                    }
                    TrackFromSourceLoad("DVG1 SELECTION_CHANGED");

                    resetTimerRequestToMovies();// send after mp.loadMovie
                    resetMessageToZero();
                }
                mp.zoomFullScreen(cbZoom.Checked);
                bReceivedTimerMessage = false;
                resetMessageToZero();
                if (cbSearchWin2EachSelection.Checked)
                {
                    lblSEARCH2state.Text = "s";
                    if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                        if (foundRowIdIn2ndWindow < 0)
                            SearchIn2ndWindowWas(fpath);
                }
            }
        }
        string lastLoadedMovie = "none";
        //
        //
        //
        public bool LoadMovie(string fpath2, bool bZoom)
        {
            bool rcPlayNext = false;
            tbPlayNextInListTitle.Text = fpath2;
            tbRowIdNextToPlay.Text = $"{rowIdMovieList}";
            if (lastLoadedMovie.Equals(fpath2))
            {
                // gv.mainWindow.ErrorStop();
                tbMovieFpath.Text = $"need NEXT";// GetFilePath();
                rcPlayNext = true;
                retriggerDGV1 = false;
                return false;
            }
            else
                mp.loadMovie(fpath2, bZoom); //DVG1 SELECTION_CHANGED   IF autopla
            lastLoadedMovie = fpath2;
            return rcPlayNext;

        }
        //
        //
        //

        bool bScanToNextMovie = false;
        public void SetFlagScanningGoToNextMovie(bool flag)
        {
            bScanToNextMovie = flag;
        }
        int errCountErrorDisplay = 0;
        public void ErrorDisplay(string ex)
        {
            if (++errCountErrorDisplay > 90)
                errCountErrorDisplay = 1;
            btFocus.Text = ex;
            countChanges = 0;
            btFocus.Focus();
            btErrorCount.Text = errCountErrorDisplay.ToString();
        }
        private void cbMovies_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMovies.Checked)
            {
                cbMovie.Checked = true;
                cbPicture.Checked = false;
                cbAllowMovieSelection.Checked = cbMovie.Checked;
            }
        }

        private void cbMovie_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPicture.Checked)
            {
                cbMovie.Checked = false;
                cbMovies.Checked = false;
                cbAllowMovieSelection.Checked = cbMovie.Checked;
            }
        }

        private void cbAutoPlay_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAutoPlay.Checked)
            {
                cbPicture.Checked = false;
                cbMovie.Checked = true;
                gv.bAutoPlayMovies = true;
                cbAllowMovieSelection.Checked = true;
            }
            else
                gv.bAutoPlayMovies = false;
        }
        Color focusColor = Color.LightGray;

        bool bCatalogModeIntended = false;

        private void cbCatalog_CheckedChanged(object sender, EventArgs e)
        {
            tbSearchItem.Enabled = true;
            if (cbCatalog.Checked)
            {
                cbPlayBackControls.Checked = true;
                if (cbOnErrContinue.Checked)
                {
                    bCatalogModeIntended = true;
                    cbCatalog.BackColor = Color.LightCoral;
                }
                cbRating.Checked = false;
                cbRateAndNext.Checked = false;
                btFocus.BackColor = Color.LightGreen;
                btFocus2.BackColor = Color.LightGreen;
                focusColor = Color.LightGreen;
            }
            else
            {
                if (cbOnErrContinue.Checked)
                {
                    if (cbPlayBackControls.Checked || bCatalogModeIntended)
                    {
                        //cbCatalog.Checked = true;
                        if (cbFocusHere.Checked)
                        {
                            btFocus.BackColor = Color.LightCoral;
                            btFocus2.BackColor = Color.LightCoral;
                        }
                    }
                }
                else
                {
                    bCatalogModeIntended = false;
                    cbCatalog.BackColor = Color.LightGray;
                    btFocus.BackColor = Color.Yellow;
                    btFocus2.BackColor = Color.Yellow;
                }
            }
        }
        string fpath = "test"; //previous file
        string dmcFolder = "zz";
        bool copyToggle = false;
        bool copyFailed = false;

        bool bcopied = false;
        public bool copyFile(char ch)
        {
            Console.Beep();
            string targetDir = gv.initParm1List[0].targetDir1 + "\\" + ch;
            if (!bDoingBackup)
                DisplayCopyMessageText(targetDir, fpath, gv.initParm1List[0].targetDir1);
            targetDir = ff.validateDirectoryFormat(targetDir);
            //
            string targetDir2Base = gv.initParm1List[0].targetDir1 + "\\" + dmcFolder;
            targetDir2Base = ff.validateDirectoryFormat(targetDir2Base);
            bcopied = false;

            if (!Directory.Exists(gv.initParm1List[0].targetDir1))
            {
                HideCopyMessageWindow();
                btTestCopy.Visible = false;
                return false;
            }

            if (!Directory.Exists(targetDir2Base))
            {
                Directory.CreateDirectory(targetDir2Base);
            }

            //  targetDir = targetDir2Base + ch;


            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            //btCopyFileInProgress.SendToBack();
            //btCopyFileInProgress.Text = fpath;
            btTestCopy.Visible = false;
            btTestCopy.Text = $"File Copy to >>{targetDir}";
            tbCopyFileName.BackColor = Color.AntiqueWhite;
            copyToggle = !copyToggle;
            if (copyToggle)
                tbCopyFileName.BackColor = Color.LightCoral;
            this.Refresh();

            numCopyCount.Value = numCopyCount.Value + 1;
            numCopyCount.BackColor = Color.LightGreen;
            tbCopyFileName.BackColor = Color.LightGreen;
            numCopyCount.Refresh();
            showCopyFileInfo(targetDir, fpath);

            //btCopyFileInProgress.Text = fpath;
            btTestCopy.Visible = true;
            btTestCopy.Text = $"File Copy to >>{targetDir}";
            //btCopyFileInProgress.Visible = true;
            this.Refresh();
            if (cbMOVE_not_copy.Checked)
            {
                bcopied = ff.MoveFile(fpath, targetDir);
            }
            else
            {
                if (fpath.Contains("need"))
                    fpath = tbFileNameSelected.Text;
                if (fpath != null)
                {
                    if (cbRN.Checked)
                    {
                        string fpathCurrentImage = fpath;
                        bcopied = CopyAndRenameFile(fpathCurrentImage, targetDir);
                    }
                    else
                    {
                        CopyAsync(fpath, targetDir);
                        //bcopied
                        //ff.CopyFile(fpath, targetDir); ///////<<<<<<<<<<<<//////////////// COPY or MOVE <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                    }
                }
            }
            return true; //return bool in copyFilePart2
        }
        public bool copyFilePart2(bool bcopied, string targetDir)
        { 
            if (bcopied && cbAddFInfoToSub.Checked)
            {
                AddFInfoEntryToSubWindowUpdatedPath(targetDir);
            }
            else if (!bcopied)
            {
                //MessageBox.Show("ERROR DID NOT COPY", "COPY ERROR"); //   COPY ERROR COPY ERROR
                HideCopyMessageWindow();
                btTestCopy.Visible = false;
                HideCopyMessageWindow();
                ErrorDialog frm = new ErrorDialog(gv);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return false;
                }
                else
                {
                    return false;
                }
                //return bcopied;
                //this.Close();
            }
            //btCopyFileInProgress.SendToBack();
            btTestCopy.Visible = false;
            //btCopyFileInProgress.Visible = false;
            if (bcopied)
            {
                gv.debug.w("--->> COPIED /or MOVED movie file >> ", targetDir, fpath);
                tbError.Text = "Okay";
            }
            else
            {
                tbError.Text = "Copy ERR";
                gv.debug.w("copy failed ", targetDir, fpath);
                return false;
            }
            numCopyCount.BackColor = Color.LightGray;
            tbCopyFileName.BackColor = Color.LightGray;
            if (cbAutoAdv.Checked)
            {
                if (rowIdMovieList < dgv1.RowCount - 1)
                {
                    //++rowIdMovieList;
                    //  dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                    // dgv1.Rows[rowIdMovieList + 1].Selected = true;
                    // AdvanceSelectionDGV1();
                    playNextMovie();
                }
            }
            HideCopyMessageWindow();
            return bcopied;
        }

        private async void CopyAsync(string sourceFilePath, string destFilePath)
        {
            string fname = Path.GetFileName(sourceFilePath);
            //  string path2 = path + "temp";

            // Copy the file.
            gv.debug.w("try to copy ", fpath, Path.Combine(destFilePath, fname));
            bool f1 = false;
            f1 = ff.verifyFileExists(fname);
            bool d1 = false;
            d1 = ff.directoryExists(destFilePath);
            string targetFile = Path.Combine(destFilePath, fname);

            var progress = new Progress<double>(p =>
            {
                progressBar1.Value = (int)(p * 100);
            });

            try
            {
                await AsyncFileCopier.CopyFileAsync(
                    sourceFilePath,
                    targetFile,
                    progress);

                // MessageBox.Show("Copy completed!", "Done",
                //               MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Copy failed:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
            }
            bool copied = ff.verifyFileExists(targetFile);
            if (copied)
            {
                btCopySuccess.BackColor = Color.LightGreen;
                bcopied = true;
            }
            else
            {
                btCopySuccess.BackColor = Color.DeepPink;
                bcopied = false;
            }
            copyFilePart2(bcopied, targetFile);
                return;
        }

        public void ShowMyDialogBox()
        {
            ErrorDialog testDialog = new ErrorDialog(gv);

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (testDialog.ShowDialog(this) == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                // this.txtResult.Text = testDialog.TextBox1.Text;
            }
            else
            {
                // this.txtResult.Text = "Cancelled";
            }
            testDialog.Dispose();
        }


        public void removeSelectedRowFromDGVfilelist()
        {
            try
            {
                int selectedIndex = dgv1.CurrentCell.RowIndex;
                if (true) //selectedIndex > -1)
                {
                    dgv1.Rows.RemoveAt(rowIdMovieList + 1);
                    dgv1.Refresh(); // if needed
                }
            }

            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Unable to remove selected row at this time");
            }
        }
        public bool deleteFile()
        {
            if (mp != null && !mp.IsDisposed)
            {
                if (bPlayedFirstVideo)
                    mp.stop();
            }
            string deleteFile = fpath;
            bool bDeleted = false;

            dgv1.Rows[rowIdMovieList].Cells["len"].Value = "0";
            dgv1.Rows[rowIdMovieList].Cells["rating"].Value = "F";

            if (cbAutoAdv.Checked)
            {
                if (rowIdMovieList < dgv1.RowCount - 1)
                {
                    //++rowIdMovieList;
                    // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                    //  dgv1.Rows[rowIdMovieList + 1].Selected = true;
                    AdvanceSelectionDGV1();
                }
            }
            if (fpath != null)
                bDeleted = ff.FileDelete(deleteFile);// .CopyFile(fpath, targetDir); //////////////////////////////////////// COPY or MOVE 

            if (bDeleted)
            {
                gv.debug.w("--->> DELETED movie file >> ", deleteFile);
                tbError.Text = "DEL";
                //removeSelectedRowFromDGVfilelist();
            }
            else
            {
                tbError.Text = "Del ERR";
                gv.debug.w("copy failed ", deleteFile);
            }

            return bDeleted;
        }
        public void setFocusHere(double pos)
        {
            tbInfo.Text = pos.ToString();
        }
        //My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Asterisk);
        //My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
        //My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
        //My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Hand)
        //My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Question)
        private void TraverserDialog_Activated(object sender, EventArgs e)
        {
            SoundAlertFocus2();
            bFocusIsHere = true;
            cbFocusIsOnThisWindow.Checked = bFocusIsHere;
            tbTime.Text = DateTime.Now.ToLongTimeString();
            if (!cbCatalog.Checked)
            {
                btFocus.BackColor = Color.Red;
                btFocus2.BackColor = Color.Red;
            }
            else
            {
                btFocus.BackColor = Color.LightGreen;
                btFocus2.BackColor = Color.LightGreen;
            }
        }
        public void SetSmallPlayback()
        {
            cbZoom.Checked = false;
            //this.Focus();

            if (mp == null || mp.IsDisposed)
                return;
            /*
            if (movieLocAndSize.Width > 700)
                movieLocAndSize.Width = 700;
            if (movieLocAndSize.Height > 600)
                movieLocAndSize.Height = 600;
            */
            mp.SetSmallScreen(movieLocAndSize);
            //mp.Location = new Point(movieLocAndSize.X, movieLocAndSize.Y);
            //mp.Width = movieLocAndSize.Width;
            // mp.Height = movieLocAndSize.Height;
        }
        private void btZoomMovieFullScreen_Click(object sender, EventArgs e)
        {
            ZoomMovieFullScreen();
        }
        public void ZoomMovieFullScreen()
        {
            SoundAlertFocus();
            cbZoom.Checked = true;
            //this.Focus();
            getMovieStatus();
            bReceivedTimerMessage = false;
            resetMessageToZero();
            if (mp == null || mp.IsDisposed)
                return;
            mp.zoomFullScreen(true);
        }
        public void completedMovieZoomMessage()
        {
        }
        private void cbFocusHere_CheckedChanged(object sender, EventArgs e)
        {
            if (mp == null || mp.IsDisposed)
                return;
            if (cbFocusHere.Checked)
                resetMessageToZero();
            mp.setFocusOnParent(cbFocusHere.Checked);
        }

        private void btDeleteFile_Click(object sender, EventArgs e)
        {
            numCopyCount.BackColor = Color.Red;
            deleteFile();
            numCopyCount.BackColor = Color.LightGray;
        }
        string tbStatus1;
        string tbStatus2;
        string tbStatus3;
        string tbStatus4;
        string tbStatus5;

        public void updatePlayerState(string newState)
        {
            bool bSearching2 = cbSearchUntilNotFoundIn2.Checked;
            if (!tbPlayerState.Text.Equals(newState))
            {
                tbStatus5 = tbStatus4;
                tbStatus4 = tbStatus3;
                tbStatus3 = tbStatus2;
                tbStatus2 = tbStatus1;
                tbPlayerState.Text = newState;
                if (newState.Equals("Playing"))
                {
                    tbPlayStateEnding.Text = "play";
                    if (cbSearchUntilNotFoundIn2.Checked)
                    {
                        if (foundInWin2)
                            playNextMovie(true);
                        //  else
                        //  cbSearchUntilNotFoundIn2.Checked = false;
                    }
                    else if (cbPicture.Checked)
                    {
                        if (!cbSlideShow.Checked)
                            StopSlideShow();
                    }
                }
                else if (newState.Contains("Stop"))
                {
                    //dmc 2025 gv.mainWindow.displayThisImage()
                }
                if (newState.Equals("Media Ending"))
                    tbPlayStateEnding.Text = "END";
            }
            if (cbRating.Checked)
            {
                cbZoom.Checked = true;
                cbFocusHere.Checked = true;
                //cbPlayNext.Checked = true;
                cbCatalog.Checked = false;
            }
        }
        Rectangle movieLocAndSize = new Rectangle(800, 200, 400, 400);

        public void movieWindowSize(Rectangle size)
        {
            movieLocAndSize = size;
        }
        public void MovieStatusException(string errMsg)
        {
            if (errMsg.Contains("DMC"))
            {
                if (cbOnErrContinue.Checked)
                {
                    tbStatusFoundInWin2.Text = "DMC ERR";
                }
                else
                {
                    MessageBox.Show("DMC ERROR STOP");
                    StopPlaying();
                    return;
                }
            }
            this.Text = errMsg;
            MarkAsInvalid();
            ResetAfterError();
            bRestartAfterError = true;
            playNextMovie(true); //Exception
            //DO NEED THE PREVIOUS TO MOVE TO NEXT LIST  .. but can not playFirstVideo();
        }
        bool bUpdateSearchByTimer = false;
        int countTimer = 0;

        public void IsFileAVIForWEBP()
        {
            string ext = Path.GetExtension(tbFileNameSelected.Text.ToUpper());
            if (ext == ".AVIF" || ext == ".WEBP")
            {
                gv.FILE_EXT = ext;
                tbExt.Text = ext;
            }
        }
        public void updateMovieStatus(double pos)
        {
            tbExt.Text = "Name";
            if (cbFocusHere.Checked)
                btFocus.Focus();
            if (pos < 0)
                return;
            if (mp == null || mp.IsDisposed)
                return;
            IsFileAVIForWEBP();

            numPosDecimal.Value = (decimal)pos;
            int mind = (int)pos / 60;
            if (gv.FILE_EXT == ".AVIF" || gv.FILE_EXT == ".WEBP")
            {
               // mind = 9999;
               if (pos > 0)
                {
                    if (cbAVIF.Checked)
                    if (!cbPause2.Checked)
                    {
                        cbPause2.Checked = true;
                    }
                    fpath = tbMovieFpath.Text;
                    mind = 9999;
                }
            }
            int sec = (int)pos % 60;
            tbPosition3.Text = $"{mind}:" + sec.ToString("D2");
            
            if (pos < 5)
                this.Focus();
            //
            if (!bScanToNextMovie)
            {
                if (bUpdateSearchByTimer && countTimer++ > 1)
                {
                    bUpdateSearchByTimer = false;
                    countTimer = 0;
                    ClearSearch2Result();
                    bool found = SearchTrav2DialogFileList();
                }
            }
            //
            dur = mp.getDuration();
            mind = (int)dur / 60;
            sec = (int)dur % 60;

            tbTotalDuration3.Text = $"{mind}:" + sec.ToString("D2");
            // ScanMovies();
            DisplayMovieResolution();

            if (formTransparent != null && !formTransparent.IsDisposed)
                formTransparent.UpdateStatus(tbTotalDuration3.Text, tbPosition3.Text);

            if (cbGetMetaDataOnly.Checked && pos > 0)
            {
                GoToEnd(5);
            }
            if (!bThisIsSubWindow && bScanToNextMovie)
            {
                SetFlagScanningGoToNextMovie(false);
                playNextMovie();
            }

        }
        public string getMoviePlayerState()
        {
            if (mp == null || mp.IsDisposed)
                return null;
            return mp.getState();
        }
        double dur;
        public void getMovieStatus()
        {
            if (mp == null || mp.IsDisposed)
                return;
            double pos = mp.getPosition();
            int mind = (int)pos / 60;
            int sec = (int)pos % 60;
            tbPosition3.Text = $"{mind}:{sec:N2}";
            dur = mp.getDuration();
            mind = (int)dur / 60;
            sec = (int)dur % 60;
            tbTotalDuration3.Text = $"{mind}:{sec}";

            DisplayMovieResolution();

        }
        private void btStatus_Click(object sender, EventArgs e)
        {
            getMovieStatus();
            DisplayMovieResolution();
            if (tbFileSelected.Text.Contains(tbFileNameSelected.Text))
            {
                tbError.Text = "MISMATCHED FILE NAME";
                tbSearchItem.Text = ff.getFileNameFromPath(tbFileNameSelected.Text);
                StartSearchTitle();
            }
        }
        public void SoundAlertFocus()
        {
            gv.mainWindow.soundAlert(40);

        }
        public void SoundAlertFocus2()
        {
            gv.mainWindow.soundAlert(5);

        }
        private void btFocus_Click(object sender, EventArgs e)
        {
            FocusHere();
            btFocus.Text = "Focus";
        }
        public void FocusHere()
        {
            SoundAlertFocus2();
            //cbZoom.Checked = true;
            if (mp != null && !mp.IsDisposed)
                mp.requestTimerCallToParent(this);
            resetMessageToZero();
            //My.Computer.Audio.PlaySystemSound(System.Media.SystemSounds.Question);
            //resizeFileListDataGrid();
        }
        bool bFocusIsHere = false;
        private void TraverserDialog_Deactivate(object sender, EventArgs e)
        {
            btFocus.BackColor = Color.LightGray;
            btFocus2.BackColor = Color.LightGray;
            bFocusIsHere = false;
            gv.mainWindow.soundAlert(6);

            cbFocusIsOnThisWindow.Checked = bFocusIsHere;
        }

        private void cbZoom_CheckedChanged(object sender, EventArgs e)
        {
            if (mp == null || mp.IsDisposed)
                return;
            mp.zoomFullScreen(cbZoom.Checked);
        }

        private void cbFaster_CheckedChanged(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
                mp.setFasterDelay(cbFaster.Checked);
            this.TopMost = cbFaster.Checked;
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            gv.bExitProgram = true;
            CloseTrav();
        }

        List<FileInfoItem> orderedByName;
       

        List<FileInfoItem> orderedByFolder;
        private void btSortFolder_Click(object sender, EventArgs e)
        {
            if (!bLoadedList)
                return;
            btTraversing.Visible = true;
            btTraversing.Update();
            ResetSortButtonColor(sender);
            dgv1.DataSource = null;

            if (bThisIsSubWindow)
                orderedByFolder = imageFileList2.finfoList.Where(file => file.len > 10000).OrderBy(file => file.dpath).ToList();
            else
                orderedByFolder = imageFileList1.finfoList.Where(file => file.len > 10000).OrderBy(file => file.dpath).ToList();
            dgv1.DataSource = orderedByFolder;
            //resizeFileListDataGrid();
            if (bThisIsSubWindow)
                imageFileList2.copyFileList(orderedByFolder); //ifl 2022 replaced
            else
                imageFileList1.copyFileList(orderedByFolder); //ifl 2022 replaced
            formatDataGridViewFileList();
            btFindDir.BackColor = Color.LightGreen;
            btTraversing.Visible = false;
        }
        bool bSorted = false;

        List<FileInfoItem> orderedByDate;
        private void btSortDate_Click(object sender, EventArgs e)
        {
            if (!bLoadedList)
                return;
            btTraversing.Visible = true;
            btTraversing.Update();
            ResetSortButtonColor(sender);
            dgv1.DataSource = null;

            if (bThisIsSubWindow)
            {
                if (cbDateDescending.Checked)
                    orderedByDate = imageFileList2.finfoList.OrderByDescending(file => file.stimestamp).ToList();
                else
                    orderedByDate = imageFileList2.finfoList.OrderBy(file => file.stimestamp).ToList();
            }
            else
            {
                if (cbDateDescending.Checked)
                    orderedByDate = imageFileList1.finfoList.OrderByDescending(file => file.stimestamp).ToList();
                else
                    orderedByDate = imageFileList1.finfoList.OrderBy(file => file.stimestamp).ToList();
            }
            dgv1.DataSource = orderedByDate;
            //resizeFileListDataGrid();
            if (bThisIsSubWindow)
                imageFileList2.copyFileList(orderedByDate); //ifl 2022 replaced
            else
                imageFileList1.copyFileList(orderedByDate); //ifl 2022 replaced
            formatDataGridViewFileList();
            btTraversing.Visible = false;
        }

        private void btSortName_Click(object sender, EventArgs e)
        {
            if (!bLoadedList)
                return;
            btTraversing.Visible = true;
            btTraversing.Update();
            ResetSortButtonColor(sender);
            SortName();
            btTraversing.Visible = false;
        }
        public void SortName()
        {
            if (!bLoadedList)
                return;

            dgv1.DataSource = null;
            List<FileInfoItem> orderedByName;
            if (bThisIsSubWindow)
            {
                if (imageFileList2 == null)
                    return;
                orderedByName = imageFileList2.finfoList.OrderBy(file => file.fname).ToList();
            }
            else
            {
                if (imageFileList1 == null)
                    return;
                orderedByName = imageFileList1.finfoList.OrderBy(file => file.fname).ToList();
            }
            dgv1.DataSource = orderedByName;
            //resizeFileListDataGrid();
            if (bThisIsSubWindow)
                imageFileList2.copyFileList(orderedByName); //ifl 2022 replaced
            else
                imageFileList1.copyFileList(orderedByName); //ifl 2022 replaced
            formatDataGridViewFileList();
        }
        private void btSortByFileLength_Click(object sender, EventArgs e)
        {
            btSortBySize.BackColor = Color.White;
            if (!bLoadedList)
                return;
            btTraversing.Visible = true;
            btTraversing.Update();
            ResetSortButtonColor(sender);
            dgv1.DataSource = null;
            List<FileInfoItem> orderedByLen;
            if (bThisIsSubWindow)
            {
                if (imageFileList2 == null | imageFileList2.getImageCount() < 1)
                    return;
                orderedByLen = imageFileList2.finfoList.OrderByDescending(file => file.len).ToList();
            }
            else
            {
                if (imageFileList1 == null | imageFileList1.getImageCount() < 1)
                    return;
                if (cbDateDescending.Checked)
                    orderedByLen = imageFileList1.finfoList.OrderByDescending(file => file.len).ToList();
                else
                    orderedByLen = imageFileList1.finfoList.OrderBy(file => file.len).ToList();
            }
            dgv1.DataSource = orderedByLen;
            //resizeFileListDataGrid();
            if (bThisIsSubWindow)
                imageFileList2.copyFileList(orderedByLen); //ifl 2022 replaced
            else
                imageFileList1.copyFileList(orderedByLen); //ifl 2022 replaced
            formatDataGridViewFileList();
            btTraversing.Visible = false;
        }

        public void ResetSortButtonColor(object sender)
        {
            btFindDir.BackColor = Color.LightGray;
            btSortName.BackColor = Color.LightGray;
            btSortBySize.BackColor = Color.LightGray;
            btSortFolder.BackColor = Color.LightGray;
            btSortOnDate.BackColor = Color.LightGray;
            if (sender != null)
                ((Button)sender).BackColor = Color.LightGreen;
        }

        private void btMoveMovie_Click(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
            {
                cbZoom.Checked = false;
                mp.moveToNextScreen();
            }
        }

        /*
         * the easy way is to use ToLower() method

                var lists = rec.Where(p => p.Name.ToLower().Contains(records.Name.ToLower())).ToList();
                            a better solution (based on this post: Case insensitive 'Contains(string)')

                var lists = rec.Where(p => 
                         CultureInfo.CurrentCulture.CompareInfo.IndexOf
                 (p.Name, records.Name, CompareOptions.IgnoreCase) >= 0).ToList();
             */

        List<FileInfoItem> searchResult;
        public void searchTitleNewList()
        {
            //searchResult = imageFileList.finfoList.OrderBy(person => person.fname).ToList();

            // searchResult = imageFileListOriginal.Where(p => p.  // .Name.ToLower().Contains(records.Name.ToLower())).ToList();
            //imageFileListOriginal.getFinfo.
            //searchResult = imageFileList.finfoList.Where( => s.ToUpper().Contains(tbSearchItem.Text.ToUpper()));
            //.OrderBy(person => person.fname).ToList();
            // searchResult = imageFileList.finfoList.FindAll(s => s.ToUpper().Contains(tbSearchItem.Text.ToUpper()));
        }
        public void searchInvalid()
        {
            string seachtxt = tbSearchItem.Text;
            bool selectFirst = true;
            for (int idx = 0; idx < dgv1.Rows.Count; ++idx)
            {
                bool isCellChecked = (bool)dgv1.Rows[idx].Cells["bInvalid"].Value;
                if (isCellChecked)
                {
                    dgv1.Rows[idx].Visible = true;
                    if (selectFirst)
                    {
                        // dgv1.ClearSelection();
                        dgv1.Rows[idx].Selected = true;
                        selectFirst = false;
                    }
                }
                else
                {
                    dgv1.CurrentCell = null;
                    dgv1.Rows[idx].Visible = false;
                }
            }
            tbError.Text = seachtxt;
        }
        public void searchClear()
        {
            foreach (System.Windows.Forms.DataGridViewRow r in dgv1.Rows)
            {
                dgv1.Rows[r.Index].Visible = true;
            }
        }
        int searchResultCount = 0;
        int rowSelection = 0;
        int currentRow = 0;
        int currentRowIndex = 0;
        DataGridViewSelectedCellCollection searchSelected;
        int[] goIndex = new int[2000];

        public void gotoNextSelectedRow(bool withoutSearch = false)
        {
            if (withoutSearch)
            {
                searchSelectedCount = dgv1.RowCount;
            }
            if (currentRowIndex < searchSelectedCount)
            {
                currentRow = goIndex[currentRowIndex];
                dgv1.FirstDisplayedScrollingRowIndex = currentRow;
                dgv1.CurrentCell = dgv1.Rows[currentRow].Cells[0];
                dgv1.Rows[currentRow].Selected = true;
                ++currentRowIndex;

                tbRowId.Text = currentRowIndex.ToString(); //dmc2021??

                tbMovieFpath.Text = dgv1.Rows[currentRow].Cells["fpath"].Value.ToString();
                tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
                tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
                fpath = tbMovieFpath.Text;
            }
        }
        public void GoToNextImageDataRow()
        {
            if (rowIdMovieList < dgv1.RowCount - 1)
            {
                //testdmc
                int oldRowId = rowIdMovieList;
                ++countPlayedMovies;
                if (cbResaveFileEachTime.Checked && bSavedFile && countPlayedMovies > 20)
                {
                    countPlayedMovies = 0;
                    resaveImageFileList();
                }
                //bring row into view 
                //dataGridView1.FirstDisplayedScrollingRowIndex = index;
                //dataGridView1.Refresh();
                //  dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                //  dgv1.Rows[rowIdMovieList + 1].Selected = true; //SELECTED WILL SET rowIdMovieList 
                AdvanceSelectionDGV1();
                //
                // DgvFileList1_SelectionChanged(this, new EventArgs());
                //
                //
            }
        }
        public void unselectAll()
        {
            if (currentRowIndex < searchSelectedCount)
            {
                currentRow = goIndex[currentRowIndex];
                dgv1.FirstDisplayedScrollingRowIndex = currentRow;
                dgv1.CurrentCell = dgv1.Rows[currentRow].Cells[0];
                dgv1.Rows[currentRow].Selected = true;
                ++currentRowIndex;
            }
        }
        public void AddFileInfoRow(FileInfoItem finfo)
        {
            if (bThisIsSubWindow)
            {
                if (imageFileList2 == null)
                    imageFileList2 = new ImageFileList();
                imageFileList2.addItem(finfo);
                dgv1.DataSource = null;
                dgv1.DataSource = imageFileList2;
            }
            else
            {
                if (imageFileList1 == null)
                    imageFileList1 = new ImageFileList();
                imageFileList1.addItem(finfo);
                dgv1.DataSource = null;
                dgv1.DataSource = imageFileList1;
            }
        }
        public void AddFolderInfoRow(FileInfoItem finfo)
        {
            if (imageFileFolderList == null)
                imageFileFolderList = new ImageFileList();
            imageFileFolderList.addItem(finfo);
        }
        public FileInfoItem GetSelectedFileInfoItem()
        {
            string fname = dgv1.Rows[rowIdMovieList].Cells["fname"].Value.ToString();
            string ext = dgv1.Rows[rowIdMovieList].Cells["ext"].Value.ToString();
            string dpath = dgv1.Rows[rowIdMovieList].Cells["dpath"].Value.ToString();
            string fpath = dgv1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();

            string type = dgv1.Rows[rowIdMovieList].Cells["type"].Value.ToString();
            int level = Convert.ToInt32(dgv1.Rows[rowIdMovieList].Cells["level"].Value);
            long len = (long)Convert.ToUInt64(dgv1.Rows[rowIdMovieList].Cells["len"].Value);

            var dtime = dgv1.Rows[rowIdMovieList].Cells["stimestamp"].Value.ToString();
            // DateTime ts = Convert.ToDateTime(dtime);// "yyyy/mm/dd mm:ss:ii");
            //  u :2000-08-17 23:32:32Z
            string stimestamp = dtime.ToString();
            DateTime test = ff.ConvertStringToDateTime(stimestamp);

            bool bDelete = Convert.ToBoolean(dgv1.Rows[rowIdMovieList].Cells["bDelete"].Value);
            bool bInvalid = Convert.ToBoolean(dgv1.Rows[rowIdMovieList].Cells["bInvalid"].Value);
            char rating = Convert.ToChar(dgv1.Rows[rowIdMovieList].Cells["minutes"].Value);
            string source = "";
            string desc = Convert.ToString(dgv1.Rows[rowIdMovieList].Cells["desc"].Value);

            //double playTime = DateTime.ParseExact(dtime, "yyyy-MM-dd HH:mm"); //, CultureInfo.InvariantCulture);
            double playTime = Convert.ToDouble(dgv1.Rows[rowIdMovieList].Cells["playTime"].Value);
            int minutes = Convert.ToInt32(dgv1.Rows[rowIdMovieList].Cells["minutes"].Value);
            int seconds = Convert.ToInt32(dgv1.Rows[rowIdMovieList].Cells["seconds"].Value);

            int width = Convert.ToInt32(dgv1.Rows[rowIdMovieList].Cells["width"].Value);
            int height = Convert.ToInt32(dgv1.Rows[rowIdMovieList].Cells["height"].Value);

            FileInfoItem fi = new FileInfoItem(fname, ext, dpath, fpath, type, level, len, stimestamp, bDelete, bInvalid,
                rating, source, playTime, minutes, seconds, width, height, desc);

            return fi;

        }
        int searchSelectedCount = 0;
        public int getSelectedRowCount()
        {
            IEnumerable<DataGridViewRow> selectedRows = dgv1.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.OwningRow).Distinct();
            //int count = selectedRows.Count();
            int scount = dgv1.SelectedRows.Count;
            searchSelected = dgv1.SelectedCells;
            //selectedRows
            scount = searchSelected.Count;
            --scount;
            int jdx = 0;
            int lastValue = -1;
            for (int idx = scount; idx >= 0; --idx)
            {
                if (searchSelected[idx].RowIndex != lastValue)
                {
                    goIndex[jdx++] = searchSelected[idx].RowIndex;
                    searchSelectedCount = jdx;
                    lastValue = searchSelected[idx].RowIndex;
                    tbLoadedCount.Text = jdx.ToString();
                }
            }
            // selectedRows.ElementAt
            return scount;
        }
        private void dgvFileList1_DoubleClick(object sender, EventArgs e)
        {
            if (cbAllowMovieSelection.Checked && bLoadedList)
            {
                bPlayedFirstVideo = true;
                if (mp == null || mp.IsDisposed)
                {
                    if (mpVersion1)
                        mp = new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                    else
                        mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                    mp.Activate();
                    mp.Show();
                    gv.SetMovies(mp);
                }
                else
                {
                    if (CheckFileTypeWEBM(fpath))
                        return;
                    resetMpPlay();
                    mp.loadMovie(tbMovieFpath.Text, cbZoom.Checked); //DGV1 double-clicked
                    TrackFromSourceLoad("DGV1 double-clicked");

                }
                if (cbBeep.Checked)
                {
                    cbFocusHere.Checked = true;
                    // cbZoom.Checked = true;
                }
                mp.zoomFullScreen(cbZoom.Checked);
                resetTimerRequestToMovies(); // send after mp.loadMovie
                resetMessageToZero();
            }
        }
        public string getFileName()
        {
            // return ff.getFileName(tbFileName.Text); // fpath);
            return tbFileNameSelected.Text; // fpath);
        }
        public string getFileLength()
        {
            // return ff.getFileName(tbFileName.Text); // fpath);
            return tbCurrentFileSize.Text; // fpath);
        }
        public string getFolder()
        {
            if (string.IsNullOrEmpty(tbFileSelected.Text))
                return null;
            string dir = ff.getFolder(tbFileSelected.Text); // fpath);
            if (getSpecialFolder(dir))
                return "AAA" + "\\" + dir;
            return dir;
        }
        public bool getSpecialFolder(string dir)
        {
            string dirFolder = tbFileSelected.Text;
            if (dirFolder.Contains($"AAA\\{dir}"))
                return true;
            if (dirFolder.Contains($"AAA/{dir}"))
                return true;
            return false;
        }
        private void btSearch_Click(object sender, EventArgs e)
        {
            StartSearchTitle();
        }
        public int StartSearchTitle(string searchTitle = null)
        {
            searchRow = -1;
            if (!string.IsNullOrEmpty(searchTitle))
                tbSearchItem.Text = ff.getFileNameFromPath(searchTitle);
            if (string.IsNullOrEmpty(tbSearchItem.Text))
                return -1;
            bFoundText = false;
            dgv1.MultiSelect = false;
            lastSearchTitle = searchTitle;
            dgv1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            return SearchTitle(tbSearchItem.Text);
        }
        public string GetSubFolderFound()
        {
            return tbCurrentFileFolder.Text;
        }
        string lastSearchTitle;
        public int StartSearchWin2Title(string searchTitle) // SEARCH SUBWINDOW SUBSEARCH SUB
        {
            tbFound2Folder.Text = "";
            string searchForThis = "";
            tbFoundSeachList2.Text = "";
            if (formTransparent != null && !formTransparent.IsDisposed)
                formTransparent.UpdateStatus2("");
            //searchRow = -1;
            if (!string.IsNullOrEmpty(searchTitle))
                searchForThis = ff.getFileNameFromPath(searchTitle);
            else
                return -1;
            if (string.IsNullOrEmpty(searchForThis))
                return -1;
            bFoundText = false;
            lastSearchTitle = searchTitle;
            if (cbSearchPartial.Checked)
                searchForThis = GetPartialFileNameForSearch(searchForThis);
            if (bThisIsSubWindow)
                rowIdMovieList = -1;
            int rowId = -1;
            // if (bThisIsSubWindow)
            rowId = gv.dialogTraverser2.SearchWin2TitleAndDisplaySubset(searchForThis);  //Search Window 2
            if (rowId > -1)
            {
                string test = gv.dialogTraverser2.getFileName();
                tbFound2Folder.Text = gv.dialogTraverser2.getFolder();
                tbFoundFileFolder.Text = tbFound2Folder.Text;
                if (formTransparent != null && !formTransparent.IsDisposed)
                    formTransparent.FoundFile(tbFound2Folder.Text);
                tbFoundSeachList2.Text = test;
                tbFoundSeachList2.BackColor = Color.LightGreen;
                if (formTransparent != null && !formTransparent.IsDisposed)
                    formTransparent.UpdateStatus2(test);

                tbFound2Folder.BackColor = Color.LightGreen;
                bFoundText = true;
                tbWin2FileSizeMatchSearch.Text = gv.dialogTraverser2.getFileLength();
            }
            else
            {
                tbFoundSeachList2.BackColor = Color.LightGray;
                tbFound2Folder.BackColor = Color.LightGray;
                bFoundText = true;
                if (formTransparent != null && !formTransparent.IsDisposed)
                    formTransparent.FoundFile("");
                //cbSearchUntilNotFoundIn2.Checked = false;
            }
            return rowId;
        }


        public void ClearIfNotFound()
        {
            dgv1.Rows.Clear();
            dgv1.Refresh();
        }
        public int StartSearchWin2Size(long len) // SEARCH SUBWINDOW SUBSEARCH SUB
        {
            tbFound2Folder.Text = "";
            string searchForThis = "";
            tbFoundSeachList2.Text = "";
            //searchRow = -1;
            bFoundText = false;

            int rowId = -1;
            // if (bThisIsSubWindow)
            rowId = gv.dialogTraverser2.SearchWin2Size(filelen);
            if (rowId > -1)
            {
                string test = gv.dialogTraverser2.getFileName();
                tbFound2Folder.Text = gv.dialogTraverser2.getFolder();
                tbFoundFileFolder.Text = tbFound2Folder.Text;
                tbFoundSeachList2.Text = test;
                tbFoundSeachList2.BackColor = Color.LightGreen;
                tbFound2Folder.BackColor = Color.LightGreen;
                bFoundText = true;
                tbWin2FileSizeMatchSearch.Text = gv.dialogTraverser2.getFileLength();
            }
            else
            {
                tbFoundSeachList2.BackColor = Color.LightGray;
                tbFound2Folder.BackColor = Color.LightGray;
                bFoundText = true;
            }
            return rowId;
        }
        bool bFoundText = false;
        int searchRow = -1;

        public int SearchTitle(string searchTitle, bool bNewSearch = false) // THIS IS THE MAIN SEARCH OF THE DGV LIST for a certain (or partial) FILE NAME 
        {
            tbSearchState.Text = "searching ..";
            if (bThisIsSubWindow)
            {
                tbSearchState.Text = "searching s..";
                resetDGV();
            }
            DisplaySearchingSubFor(searchTitle); // bThisIsSubWindow
            bFoundText = false;
            if (bNewSearch)
            {
                searchRow = -1;
                dgv1.MultiSelect = false;
                dgv1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            if (searchRow >= dgv1.RowCount)
                searchRow = 0;
            else
                searchRow++;
            string searchtxtUpper = searchTitle.ToUpper();
            for (; searchRow < dgv1.RowCount - 1; ++searchRow)
            {
                if (dgv1.Rows[searchRow].Cells[0].Value.ToString().ToUpper().Contains(searchtxtUpper))
                {
                    if (true)
                    {
                        //rowIdMovieList = searchRow;
                        //Select the found item
                        dgv1.CurrentCell = dgv1.Rows[searchRow].Cells[0];
                        dgv1.Rows[searchRow].Selected = true;

                        dgv1.FirstDisplayedScrollingRowIndex = searchRow;

                        dgv1.Refresh();
                        bFoundText = true;
                        tbSearchState.Text = "Found";
                        DisplaySearchingSubFound();
                        /*
                         * 
                         DataGridViewCell cell = row.Cells[0];
                         cell.Selected = true;
                         */

                        /* this should be set in SelectionChanged\
                         */
                        tbFileSelected.Text = dgv1.Rows[searchRow].Cells["fpath"].Value.ToString();
                        //oldName = dgvFileList1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
                        //
                        tbRowId.Text = searchRow.ToString();
                        lastRowIdSet = searchRow;
                        //
                        tbMovieFpath.Text = dgv1.Rows[searchRow].Cells["fpath"].Value.ToString();
                        tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
                        tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
                        fpath = tbMovieFpath.Text;
                        tbFileNameSelected.Text = dgv1.Rows[searchRow].Cells[0].Value.ToString();
                        // rowIdMovieList = searchRow++;
                        rowIdMovieList = (int)dgv1.CurrentCell.RowIndex;
                        tbRowIdTemp.Text = rowIdMovieList.ToString();
                        break;
                    }
                } //match found
            }
            tbError.Text = rowIdMovieList.ToString();
            if (bFoundText)
                return searchRow;
            else
            {
                tbSearchState.Text = "NOT found title";
                DisplaySearchingSubNotFound();
                return -1;
            }
        }
        public int searchTitle2(string seachText)
        {
            int rc = -1;
            string seachtxtUpper = seachText.ToUpper();
            foreach (System.Windows.Forms.DataGridViewRow rowDataFileInfo in dgv1.Rows)
            {
                if ((rowDataFileInfo.Cells[0].Value).ToString().ToUpper().Contains(seachtxtUpper))
                {

                    if (rowIdMovieList < rowDataFileInfo.Index && rowDataFileInfo.Index < dgv1.RowCount)
                    {
                        //  rowIdMovieList = rowDataFileInfo.Index;

                        //dgvFileList1.CurrentCell = dgvFileList1.Rows[rowIdMovieList].Cells[0];
                        dgv1.CurrentCell = dgv1.Rows[rowIdMovieList].Cells[0];
                        dgv1.Rows[rowIdMovieList].Selected = true;

                        dgv1.FirstDisplayedScrollingRowIndex = rowIdMovieList;

                        dgv1.Refresh();
                        bFoundText = true;
                        /*
                         * 
                         DataGridViewCell cell = row.Cells[0];
                         cell.Selected = true;
                         */
                        /* following set in SelectionChanged
                        tbFileSelected.Text = dgvFileList1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
                        //oldName = dgvFileList1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
                        //
                        tbRowId.Text = rowIdMovieList.ToString();
                        //
                        tbMovieFpath.Text = dgvFileList1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
                        fpath = tbMovieFpath.Text;
                        tbFileNameSelected.Text = dgvFileList1.Rows[rowIdMovieList].Cells[0].Value.ToString();

                        rc = rowIdMovieList;
                        */
                        break;
                    }
                }
            }
            tbError.Text = rowIdMovieList.ToString();
            return rc;
        }
        private void tbSearchItem_Enter(object sender, EventArgs e)
        {
            if (searchResultCount > 0 && cbFind.Checked)
            {
                //     dgv1.ClearSelection();
                gotoNextSelectedRow();
            }
        }
        private void btCompare_Clicked(object sender, EventArgs e)
        {
            /*
            if (unmatchPercent < 20)
            if (rowIdMovieList < dgvFileList1.RowCount - 2)
            {
                //++rowIdMovieList;
                dgvFileList1.CurrentCell = dgvFileList1.Rows[rowIdMovieList + 1].Cells[0];
                dgvFileList1.Rows[rowIdMovieList + 1].Selected = true;
            }
            */
            CompareImageWithTraverser2();
        }
        public void CompareImagesInSequence()
        {
            if (bThisIsSubWindow)
                return;
            double unmatchPct = CompareImages((Bitmap)pbThumbNail.Image, (Bitmap)pb2.Image, 10);
            if (unmatchPct < 20)
            {

            }
        }
        bool bSlowDown = false;
        public void CompareImageWithTraverser2()
        {
            if (bThisIsSubWindow)
                return;
            GetBmp2();
            unmatchPercent = CompareImages((Bitmap)pbThumbNail.Image, (Bitmap)pb2.Image, 10);
            tbCompareUnmatch.Text = unmatchPercent.ToString();
            if (unmatchPercent < 20) //a match STOP
            {
                tbCompareUnmatch.BackColor = Color.LightGreen;
                btRefreshCompare2.BackColor = Color.Red;

            }
            else
            {
                btRefreshCompare2.BackColor = Color.LightBlue;
                tbCompareUnmatch.BackColor = Color.LightGray;
                if (bSlowDown)
                {
                    this.Refresh();
                    pbThumbNail.Refresh();
                    tbCompareUnmatch.Refresh();
                }
                if (cbAutoAdv.Checked)
                {
                    if (rowIdMovieList < dgv1.RowCount - 2)
                    {
                        //++rowIdMovieList;
                        // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells[0];
                        // dgv1.Rows[rowIdMovieList + 1].Selected = true;
                        AdvanceSelectionDGV1();
                    }
                }
            }
        }
        double unmatchPercent = 100;
        //
        //
        // 2023
        double CompareImages(Bitmap InputImage1, Bitmap InputImage2, int Tollerance = 10)
        {
            if (InputImage1 == null || InputImage2 == null)
                return 0;
            Bitmap Image1 = new Bitmap(InputImage1, new Size(128, 128));
            Bitmap Image2 = new Bitmap(InputImage2, new Size(128, 128));
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
        //
        //
        //
        private void cbBeep_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBeep.Checked && bPlayedFirstVideo)
            {
                cbFocusHere.Checked = true;
                // cbZoom.Checked = true;
            }
        }

        private void tbSearchItem_Click(object sender, EventArgs e)
        {
            cbCatalog.Checked = false;
            cbAutoAdv.Checked = false;
            btSearch.Enabled = true;
        }

        private void tbSearchItem_MouseEnter(object sender, EventArgs e)
        {
            cbCatalog.Checked = false;
            cbAutoAdv.Checked = false;
        }

        private void tbSearchItem_TextChanged(object sender, EventArgs e)
        {
            cbCatalog.Checked = false;
            cbAutoAdv.Checked = false;
            searchRow = 0;
        }

        private void tbSearchItem_MouseClick(object sender, MouseEventArgs e)
        {
            cbFocusHere.Checked = false;
            cbCatalog.Checked = false;
            cbAutoAdv.Checked = false;
            btSearch.Enabled = true;
            tbSearchItem.Select(0, 0);
        }

        private void cbAutoAdvance_CheckedChanged(object sender, EventArgs e)
        {
            tbSearchItem.Enabled = true;

        }

        private void btGoToEnding_Click_1(object sender, EventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                GoToEnd(5);
            }
            else
                GoToEnd(10);
            messageCount = 99;
        }
        bool mpVersion1 = true;
        public void PlayNextVideo(bool play = false)
        {
            if (mp == null || mp.IsDisposed)
            {
                if (mpVersion1)
                    mp = new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                else
                    mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                mp.Activate();
                mp.Show();
                gv.SetMovies(mp);
                this.Focus();
                resetMpPlay();
                mp.loadMovie(tbMovieFpath.Text, cbZoom.Checked); //playNext2
                TrackFromSourceLoad("playNextVideo");

                if (CheckFileTypeWEBM(fpath))
                    return;
                mp.play();
            }
            else
            {
                if (string.IsNullOrEmpty(fpath) || string.IsNullOrWhiteSpace(fpath))
                {
                    this.Text = rowIdMovieList.ToString();
                    if (rowIdMovieList < 0)
                        MessageBox.Show("ERROR DialogTraverser rowIdMovieList");
                    return;
                }
                resetMpPlay();
                mp.loadMovie(fpath, cbZoom.Checked); //playNext2
                TrackFromSourceLoad("playNext2");

                resetTimerRequestToMovies();// send after mp.loadMovie
                resetMessageToZero();
                if (play)
                    mp.play();
            }
        }

        private void btCloseMovie_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                if (mp != null && !mp.IsDisposed)
                    mp.Hide();
            }
            if (mp != null && !mp.IsDisposed)
            {
                mp.Close();
                mp.Dispose();
            }
        }
        public void StopSlideShow()
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.Close();
                mp.Dispose();
            }
        }
        private void btGoToEnd15_Click(object sender, EventArgs e)
        {
            if (cbShortEndSample.Checked)
                GoToEnd(5);
            else
                GoToEnd(5);
        }

        private void cmboSourceFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = cmboSourceFolder.SelectedItem.ToString();
            string sanitizedFilename = new string(cmboSourceFolder.SelectedItem.ToString().Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());

            int idx = gv.folderHistoryList.FindIndex(fh => fh.folderPath.Equals(tbDirectoryPath.Text, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0)
                tbCopyFileName.Text = gv.folderHistoryList[idx].desc;
            //   tbCopyFileName.Text = gv.folderHistoryList[idx].desc;
            // tbDirectoryPath.Text = sanitizedFilename;
            // numLastFolder.Value = cmboSourceFolder.SelectedIndex;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void cbFind_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFind.Checked)
            {

            }
            else
            {
                searchClear();
            }
        }

        private void BtRename_Click(object sender, EventArgs e)
        {
            cbRenameResult.Checked = false;
            bool rc = ff.renameFile(oldName, tbFileSelected.Text);
            cbRenameResult.Checked = rc;
        }

        public void ScanMovies()
        {
            if (cbScanMovies.Checked)
            {
                playNextMovie(true); //ScanMovies
            }
        }


        private void CbScanMovies_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbScanMovies.Checked)
                cbGetMetaDataOnly.Checked = false;
        }
        private void BtScanThruClick(object sender, EventArgs e)
        {
            cbMute.Checked = false;
            if (bPlayedFirstVideo)
                Resume();
            StartScanThru();
        }
        public void StartScanThru()
        {
            if (!bPlayedFirstVideo)
                playFirstVideo();
            if (!cbFocusHere.Checked)
                cbFocusHere.Checked = true;
            // if (!cbZoom.Checked)
            //   cbZoom.Checked = true;
            cbScanMovies.Checked = true;
            if (cbLongerSegments.Checked)
                mp.setSlowerDelay(true, durationOfSampleSeconds);
        }
        private void CbInvalid_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInvalid.Checked)
                searchInvalid();
        }

        private void BtMove_Click(object sender, EventArgs e)
        {
            string fname;
            foreach (DataGridViewColumn dc in dgv1.Columns)
            {
                fname = dgv1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
                //ff.MoveFile(fname, targetFolder + "\\E");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            GoToEnd(cbShortEndSample.Checked ? 5 : 15);
        }

        private void BtGotoNext_Click(object sender, EventArgs e)
        {
            playNextMovie(); //btGoToNext
        }
        public bool OnErrContinue()
        {
            if (cbOnErrContinue.Checked)
            {
                UpdateXmlFileList();
                PlayButtonGo();
                ClearError();
            }
            return cbOnErrContinue.Checked;
        }
        public void PlaybackErrorInMovies(string title)
        {
            tbPlayBackError.Text = $"{title}";
        }
        public void ClearError()
        {
            gv.ERROR_STOP = false;
            btCloseMovie.BackColor = Color.LightBlue;
        }
        public bool AdvanceSelectionDGV1() //works 3/17/2024 !!!!!!!!!!!!!!!!!!!
        {
            if (!cbLoadedAndReady.Checked)
                return false;

            bool bAdvanced = true;
            if (rowIdMovieList < dgv1.RowCount - 1)
            {
                // rowIdMovieList++;
                //dgv1.ClearSelection();
                retriggerForRowId = -1; /// temp  rowIdMovieList + 1;
                dgv1.CurrentCell = dgv1.Rows[rowIdMovieList + 1].Cells["fname"]; // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList+].Cells[0];  correct
                dgv1.Rows[rowIdMovieList + 1].Selected = true; //SELECTED WILL SET rowIdMovieList 

                if (dgv1.SelectedRows.Count > 1)
                    MessageBox.Show("multiselect", "error");
                dgv1.Refresh();
            }
            bAdvanced = false;

            return bAdvanced;
        }
        int countPlayedMovies = 0;
        public void playNextMovie(bool autoNext = false)
        {
            ClearPlayInfoFields();
            bDisplayedResolution = false;
            if (cbDEBUG.Checked)
                gv.ERROR_STOP = false;
            if (gv.ERROR_STOP)
                btCloseMovie.BackColor = Color.Red;
            if (mp == null || mp.IsDisposed)
                return;
            if (rowIdMovieList < dgv1.RowCount - 1)
            {
                //testdmc
                int oldRowId = rowIdMovieList;
                ++countPlayedMovies;
                if (cbResaveFileEachTime.Checked && bSavedFile && countPlayedMovies > 20)
                {
                    countPlayedMovies = 0;
                    resaveImageFileList();
                }
                btReselectSelectCurrentRow_Click(null, null);
                // AdvanceSelectionDGV1();

                if (!cbRefreshForceOnScan.Checked)
                {

                    resetMpPlay();
                    mp.loadMovie(fpath, cbZoom.Checked);//playNextMovie
                    TrackFromSourceLoad("playNextMove");
                }
                resetTimerRequestToMovies();// send after mp.loadMovie
                resetMessageToZero();
            }
            else
            {
                cbScanMovies.Checked = false;
                cbSearchUntilNotFoundIn2.Checked = false;
                return;
            }
            return;
        }
        private int DeleteMarkedFiles()
        {
            int count = 0;
            for (int i = gv.imageFileList1.getImageFileListLength() - 1; i >= 0; i--)
            {
                FileInfoItem finfo = gv.imageFileList1.getIndexed(i);

                if (gv.imageFileList1 != null && finfo.bDelete)
                {
                    try
                    {
                        File.Delete(finfo.fpath);
                        //dgv1.Rows.RemoveAt(i);
                        ++count;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete {finfo.fpath}: {ex.Message}");
                    }
                }
            }
            btDeleteMarkedFiles.BackColor = Color.OrangeRed;
            return count;
        }
        //
        // temp 2020
        //

        public void junk()
        {

            if (cbAutoNext.Checked)
            {
                if (CheckFileTypeWEBM(fpath))
                    return;

                mp.play();
                if (!mp.getState().Contains("Play"))
                {
                    mp.play();
                }
            }
            // first timer message back will mp.play();
            if (cbScanMovies.Checked)
            {
                cbMute.Checked = true;
            }
            DisplayMovieResolution();
        }

        private void btSkipAhead_Click(object sender, EventArgs e)
        {
            SkipForward();
        }
        public void SkipForward()
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.skipForward((double)numAhead.Value);
                getMovieStatus();
            }
        }
        public void ClearPlayInfoFields()
        {
            tbTotalDuration3.Text = "";
            tbPosition3.Text = "";
            tbResolution.Text = "";

        }
        private void btPlayNext_Click(object sender, EventArgs e)
        {
            ClearPlayInfoFields();
            playNextMovie(true);
        }

        private void cbMore_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMore.Checked)
            {
                numSampleCount.Value = 20;
            }
            else
            {
                numSampleCount.Value = 10;
                if (messageCount > 8)
                    messageCount = 8;
            }
        }

        private void btBackup_Click(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
                mp.ScanBackward();
        }

        private void numAhead_DoubleClick(object sender, EventArgs e)
        {
            numAhead.Value = 0;
        }

        private void btSkipForward1min_Click(object sender, EventArgs e)
        {
            SkipForward1min();
        }
        public void SkipForward1min()
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.skipForward((double)60);
                getMovieStatus();
            }
        }
        public void SkipForward2min()
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.skipForward((double)120);
                getMovieStatus();
            }
        }

        private void btSkipBack_Click_5(object sender, EventArgs e)
        {
            SkipBack();
        }
        public void SkipBack()
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.skipBack((double)20);
                getMovieStatus();
            }
        }

        private void TraverserDialog_Leave(object sender, EventArgs e)
        {
            cbFocusHere.BackColor = Color.LightGray;
            if (cbFocusHere.Checked)
                if (mp != null && !mp.IsDisposed)
                    mp.setFocusOnParent2();
            bFocusIsHere = false;
            gv.mainWindow.soundAlert(6);
            cbFocusIsOnThisWindow.Checked = bFocusIsHere;
        }

        private void btVideos_Click(object sender, EventArgs e)
        {
            cbLoadedAndReady.Checked = false;
            tbMovieFpath.Text = "";
            cbMIDI.Checked = false;
            cbAutoPlay.Checked = true;
            cbMovies.Checked = true;
            bSavedFile = false;
            btVideos.BackColor = Color.LightGreen;
            if (!TraverseGo())
            {
                tbMovieFpath.Text = "!! ERROR Invalid directory !!";
                MessageBox.Show("!! ERROR Invalid directory !!");
                return;
            }
            if (bThisIsSubWindow)
            {
                cbLoadedAndReady.Checked = true;
                // gv.mainWindow.setTargetFolder(tbDirectoryPath.Text);   //dmcdmc24
            }
            else
            {
                btDeleteMarkedFiles.BackColor = Color.LightYellow;
            }
            gv.mainWindow.SetFileType("videos");
            bLoadedList = true;
        }
        public void ResetMoviesConnection(WmPlayer newMP)
        {
            mp = newMP;
        }
        private void btSortOnNameAgain_Click(object sender, EventArgs e)
        {
            FindFile(tbSearchItem.Text);
        }
        public int FindFile(string fname)
        {
            return StartSearchTitle(tbSearchItem.Text);
        }
        private void tbSearchItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchTitle(tbSearchItem.Text);
            }
            else if (e.KeyCode == Keys.Escape)
            {
            }
        }

        private void btImages_Click(object sender, EventArgs e)
        {
            cbLoadedAndReady.Checked = false;
            tbMovieFpath.Text = "";
            cbMIDI.Checked = false;
            bSavedFile = false;
            cbPicture.Checked = true;
            btImages.BackColor = Color.LightGreen;
            if (!TraverseGo())
            {
                tbMovieFpath.Text = "!! ERROR Invalid directory !!";
                MessageBox.Show("!! ERROR Invalid directory !!");
                return;
            }

            gv.mainWindow.SetFileType("images");
            this.BringToFront();
            this.Activate();
        }

        private void btPlayNormalSpeed_Click(object sender, EventArgs e)
        {
            if (mp != null)
            {
                mp.playNormalSpeed();
                resetMpPlay(); //cbSlow.checked = false;
            }
        }
        bool resetingCbSlow = false;

        private void cbSlower_CheckedChanged(object sender, EventArgs e)
        {
            if (!resetingCbSlow)
            {
                if (mp != null)
                {
                    if (cbSlow.Checked)
                        mp.slowMotion(cbSlow.Checked);
                    else
                        mp.playNormalSpeed();
                }
            }
        }

        private void btSkipBackupLarger_Click(object sender, EventArgs e)
        {
            SkipBackupLarger();
        }
        public void SkipBackupLarger()
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.skipBack((double)40);
                getMovieStatus();
            }
        }
        private void btPause_Click(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
            {
                string st = getMoviePlayerState();
                if (!st.Equals("PAUSED"))
                    mp.pause();
                else
                    mp.play();
                getMovieStatus();
            }
        }
        private void cbPause2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPause2.Checked)
                mp.pause();
            else
                mp.play();
        }

        private void btSlower_Click(object sender, EventArgs e)
        {
            if (mp != null)
            {
                cbSlow.Checked = true;
                mp.slowMotion(true);
            }
        }

        private void cbMute_CheckedChanged(object sender, EventArgs e)
        {
            if (mp != null)
                mp.mute(cbMute.Checked);
        }

        private void cbSlower2_CheckedChanged(object sender, EventArgs e)
        {
            if (!resetingCbSlow)
            {
                if (mp != null)
                {
                    if (cbSlower2.Checked)
                    {
                        if (!cbSlow.Checked)
                        {
                            cbSlow.Checked = true;
                        }
                        mp.slowMotion(true);
                    }
                    else
                    {
                        if (cbSlow.Checked)
                            mp.slowMotion(false); //slower
                        else
                            mp.playNormalSpeed();

                    }
                }
            }
        }

        private void cbLess_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLess.Checked)
            {
                numSampleCount.Value = 4;
                messageCount = 0;
            }
            else
            {
                numSampleCount.Value = 10;
                if (messageCount > 8)
                    messageCount = 8;
            }
        }

        private void tbCountM_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbSetAllPlayerModes_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSetAllPlayerModes.Checked)
            {
                cbMute.Checked = false;
                cbAutoAdv.Checked = true;
                cbAutoPlay.Checked = true;
                cbMute.Checked = true;
                cbOnErrContinue.Checked = true;
                cbFocusHere.Checked = true;
                cbAutoNext.Checked = true;
            }
            // btGetWindow2FolderTarget_Click(sender, e);
            tbTargetFolder.BackColor = Color.LightGreen;
        }

        int endPosition = 15;
        private void cbShortEndSample_CheckedChanged(object sender, EventArgs e)
        {
            if (cbShortEndSample.Checked)
                endPosition = 6;
            else
                endPosition = 15;
        }

        private void btRestoreList_Click(object sender, EventArgs e)
        {

        }

        private void cbLongerSegments_CheckedChanged(object sender, EventArgs e)
        {
            if (mp != null)
                mp.setSlowerDelay(cbLongerSegments.Checked, durationOfSampleSeconds);
        }

        private void btSmallPlayback_Click(object sender, EventArgs e)
        {
            SetSmallPlayback();
        }

        private void btGetMovieSize_Click(object sender, EventArgs e)
        {
            if (mp != null)
            {
                movieLocAndSize = mp.GetSize();
                tbInfo.Text = $"{movieLocAndSize.Width} {movieLocAndSize.Height}";
            }
        }

        private void btOpenMovieWin_Click(object sender, EventArgs e)
        {
            if (mp == null || mp.IsDisposed)
            {
                if (mpVersion1)
                    mp = new WmPlayer(gv, this);
                else
                    mp2 = /*P2*/ new WmPlayer(gv, this);
                mp.Activate();
                mp.Show();
            }
        }

        private void btScanForward_Click(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
                mp.fasterForward();
        }

        private void cbGetMetaDataOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (cbGetMetaDataOnly.Checked)
            {
                cbShortEndSample.Checked = true;
                cbLess.Checked = true;
                //StartScanThru();
            }
            if (cbGetMetaDataOnly.Checked)
            {
                numSampleCount.Value = 2;
                messageCount = 0;
            }
            else
            {
                numSampleCount.Value = 10;
                if (messageCount > 8)
                    messageCount = 8;
            }
        }

        private void btDisplayMainFullScreen_Click(object sender, EventArgs e)
        {
            main.Activate();
            main.WindowState = FormWindowState.Maximized;
        }

        private void btResume_Click(object sender, EventArgs e)
        {
            Resume();
        }

        private void cbRateAndNext_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRateAndNext.Checked)
            {
                cbRating.Checked = true;
                //cbPlayNext.Checked = true;
                cbFocusHere.Checked = true;
                cbAutoPlay.Checked = true;
            }
        }

        private void btResumeAfterException_Click(object sender, EventArgs e)
        {
            ResetAfterError();
        }
        public void ResetAfterError()
        {
            cbFocusHere.Checked = false;
            cbZoom.Checked = false;
            //cbPlayNext.Checked = false;
            cbRating.Checked = false;

            cbRateAndNext.Checked = true;

            cbRating.Checked = true;
            cbFocusHere.Checked = true;
            // cbZoom.Checked = true;
            //cbPlayNext.Checked = true;
            FocusHere();
            if (mp != null && !mp.IsDisposed)
                mp.FocusOnParent();
        }

        private void btClearERRORSTOP_Click(object sender, EventArgs e)
        {
            gv.ERROR_STOP = false;
            btCloseMovie.BackColor = Color.LightBlue;
        }

        private void btErrorDisplay_Click(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
                mp.errorDisplay();
        }

        //ImageFileList imageFileListSearch;
        public List<FileInfoItem> imageFileListSearch;

        private void btSearchNewList_Click(object sender, EventArgs e)
        {
            SearchWin2TitleAndDisplaySubset(tbSearchItem.Text);
        }
        public int SearchWin2TitleAndDisplaySubset(string searchTitle) //dmc searchInWindow2 actual search routine   //Search Window 2
        {
            if (bThisIsSubWindow)
            {
                if (imageFileList2 == null)
                {
                    return -1;
                }
            }
            else //main
            {
                if (imageFileList2 == null)
                    imageFileList2 = imageFileList1;
            }
            DisplaySearchingSubFor();
            //24 rowIdMovieList = 0;
            if (bThisIsSubWindow)
                imageFileListSearch = imageFileList2.finfoList.Where(f => f.fname.ToUpper().Contains(searchTitle.ToUpper())).ToList();
            else
                imageFileListSearch = imageFileList1.finfoList.Where(f => f.fname.ToUpper().Contains(searchTitle.ToUpper())).ToList();
            // List<FileInfoItem> imageFileListSearch = found.ToList();
            dgv1.DataSource = null;
            dgv1.DataSource = imageFileListSearch;

            tbCount.Text = dgv1.RowCount.ToString();
            cbLoadedAndReady.Checked = true;
            formatDataGridViewFileList();
            if (imageFileListSearch.Count > 0)
            {
                DisplaySearchingSubFound(searchTitle);
                return imageFileListSearch.Count;
            }
            else
            {
                DisplaySearchingSubNotFound(searchTitle);
                if (dgv1.DisplayedRowCount(true) > 9)
                {
                    dgv1.DataSource = null;
                    dgv1.Refresh();
                    tbRowId.Text = "x";
                }
                return -1;
            }
        }
        public int SearchWin2Size(long len) //dmc searchInWindow2 actual search routine
        {
            // if (!bThisIsSubWindow)
            //  return -1;
            DisplaySearchingSubFor();
            //24 rowIdMovieList = 0;
            if (bThisIsSubWindow)
                imageFileListSearch = imageFileList2.finfoList.Where(f => f.len.Equals(len)).ToList();
            else
                imageFileListSearch = imageFileList1.finfoList.Where(f => f.len.Equals(len)).ToList();
            // List<FileInfoItem> imageFileListSearch = found.ToList();
            dgv1.DataSource = null;
            dgv1.DataSource = imageFileListSearch;

            tbCount.Text = dgv1.RowCount.ToString();
            cbLoadedAndReady.Checked = true;
            formatDataGridViewFileList();
            if (imageFileListSearch.Count > 0)
            {
                DisplaySearchingSubFound("match len");
                return imageFileListSearch.Count;
            }
            else
            {
                DisplaySearchingSubNotFound("match len");
                return -1;
            }
        }
        private void btRestoreListOriginal_Click(object sender, EventArgs e)
        {
            if (bThisIsSubWindow)
            {
                imageFileList2 = imageFileListOriginal2;
            }
            else
            {
                imageFileList1 = imageFileListOriginal1;
            }
            resetDGV();

            formatDataGridViewFileList();
        }

        private void btUpdateImageFile_Click(object sender, EventArgs e)
        {
            UpdateXmlFileList();
        }

        private void cbOnErrContinue_CheckedChanged(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
                mp.SetOnErrContinue(cbOnErrContinue.Checked);
        }
        public void IsSpecialFolderSet()
        {
            cbSpecialFolder.Checked = UsingSpecialFolder(null); // does  main.setTargetFolder(fullPath);
            cbSetSpecialFolder.Checked = cbSpecialFolder.Checked;
        }
        private void btOpenFileListWindow_Click(object sender, EventArgs e)
        {
            IsSpecialFolderSet();
            cbFocusHere.Checked = false;
            btSearchWin2.BackColor = Color.LightGreen;
            if (gv.dialogTraverser2 == null || gv.dialogTraverser2.IsDisposed)
            {
                gv.dialogTraverser2 = new TraverserDialog(gv, main, 0, ff, this);
                gv.dialogTraverser2.Visible = true;
            }
            else
            {
                gv.dialogTraverser2.WindowState = FormWindowState.Normal;
                gv.dialogTraverser2.Activate();
            }
        }

        private void btSetTargetAsBase_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = gv.mainWindow.GetTargetFolder(); // whatever is set in Main Window tbTarget
        }
        public string GetFolderFullPath()
        {
            return tbDirectoryPath.Text;
        }

        public string GetNextHigherFolderName(string filePath)
        {
            var splitResult = filePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var newFilePath = filePath.Take(splitResult.Length - 1).ToArray();
            return newFilePath.ToString();
        }
        //
        //
        //

        private void btSearchWin2_Click(object sender, EventArgs e)
        {
            SearchWin2();
        }
        public bool SearchWin2() //call this
        {
            gv.foundInWin2 = false;
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                bool found = ClearAndSearch2();
                if (!found)
                {
                    int countRows = dgv1.DisplayedRowCount(true);
                    if (countRows > 0)
                    {

                    }
                }
                else
                {
                    tbFoundSeachList2.Text = tbFileNameSelected.Text;
                    tbFound2Folder.Text = gv.dialogTraverser2.GetSubFolderFound();
                }
                gv.foundInWin2 = found;
            }
            return gv.foundInWin2;
        }
        bool bUseSubWinSubsetSearch = true;
        long filelen = 0;
        public bool ClearAndSearch2()
        {
            bool found = false;
            if (bThisIsSubWindow)
                return found;
            else
                tbFoundSeachList2.Text = "";
            ClearSearch2Result();
            // bool rc = SearchOtherTMain();
            // StartSearchWin2Title();

            int foundRow = -1;
            if (!cbSearchForLenMatch.Checked)
                foundRow = StartSearchWin2Title(tbFileNameSelected.Text); //Search Window 2
            else
                foundRow = StartSearchWin2Size(filelen);
            if (foundRow >= 0)
                found = true;

            // SearchOtherTMain(); //New Search 2
            if (!found)
            {
                foundInWin2 = false;

            }
            else
                foundInWin2 = true;
            return found;
        }
        public void SearchOtherResultFound()
        {
            tbFoundSeachList2.Text = gv.dialogTraverser2.getFileName();
            tbFoundSeachList2.BackColor = Color.LightGreen;
            btSearchWin2.BackColor = Color.LightGreen;
            tbWin2FileSizeMatchSearch.Text = gv.dialogTraverser2.getFileLength();
        }
        public void ClearSearch2Result()
        {
            tbFoundSeachList2.Text = "";
            tbFoundSeachList2.BackColor = Color.White;
            tbFoundFileFolder.Text = "";
            btSearchWin2.BackColor = Color.LightGray;
            tbWin2FileSizeMatchSearch.Text = "";
        }
        public void ClearSearchOtherResult(string startSymbol)
        {
            //if (tbFoundSeachList2.Text.mi
            if (tbFoundSeachList2.Text.Contains($"{startSymbol} {startSymbol}"))
            {
                tbFoundSeachList2.Text = "";
                tbFoundFileFolder.Text = "";
            }
            else
            {
                tbFoundSeachList2.Text = $"{startSymbol} {tbFoundSeachList2.Text}";
            }
            tbFoundSeachList2.BackColor = Color.White;
            lblSEARCH2state.Text = currentRowIndex.ToString();
            lblSEARCH2state.BackColor = Color.LightGray;
        }
        public string GetPartialFileNameForSearch(string searchText)
        {
            int posDot = searchText.LastIndexOf(".");
            if (posDot > 4)
            {
                posDot -= 4;
                searchText = tbFileNameSelected.Text.Substring(0, posDot);
            }
            return searchText;
        }
        int foundRowIdIn2ndWindow = -1;
        public bool SearchTrav2DialogFileList(bool reSearch = false)  // in bSubWindow Search DialogTraverser2  2nd 
        {
            bool found = false;
            bool bMainWindow = false;
            tbFoundRowNumber.Text = "";
            if (bThisIsSubWindow)
            {
                String searchText = tbFileNameSelected.Text;
                if (cbSearchPartial.Checked)
                {
                    searchText = GetPartialFileNameForSearch(searchText);
                }
                foundRowIdIn2ndWindow = parentWin.StartSearchTitle(searchText);
                tbFoundRowNumber.Text = foundRowIdIn2ndWindow.ToString();
                if (foundRowIdIn2ndWindow >= 0)
                    found = true;
            }
            else
            {
                if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                {
                    bMainWindow = true;
                    String searchText = tbFileNameSelected.Text;
                    if (cbSearchPartial.Checked)
                    {
                        searchText = GetPartialFileNameForSearch(searchText);
                    }
                    found = SearchIn2ndWindowWas(searchText);
                }
            }
            btSearchWin2.BackColor = Color.LightGray;
            if (found && bMainWindow)
            {
                tbFound2Folder.Text = gv.dialogTraverser2.getFolder();
                tbFoundFileFolder.Text = tbFound2Folder.Text;
                SearchOtherResultFound();
            }

            return found;
        }
        bool SearchIn2ndWindowWas(string searchTitle)
        {
            bool rc = false;
            lblSEARCH2state.Text = "Search2";

            foundRowIdIn2ndWindow = gv.dialogTraverser2.StartSearchTitle(searchTitle);
            lblSEARCH2state.Text = foundRowIdIn2ndWindow.ToString();
            // lblSEARCH2state.Text = tbFoundRowNumber.Text;
            if (foundRowIdIn2ndWindow >= 0)
                rc = true;
            return rc;
        }

        private void btReSync_Click(object sender, EventArgs e)
        {
            tbFoundSeachList2.Text = "";
            tbFoundFileFolder.Text = "";
            if (mp != null && !mp.IsDisposed)
            {
                tbSearchItem.Text = ff.getFolder(fpath);
                tbPlayBackError.Text = "checking play title...";
                tbFoundSeachList2.Text = ff.getFileNameFromPath(mp.getTitle());

                if (tbFoundSeachList2.Text != tbFileNameSelected.Text)
                {
                    tbPlayBackError.Text = $"wrong title in play";
                    PlayNextVideo(true);
                }
                else
                {
                    tbPlayBackError.Text = $"checking play title {tbFoundSeachList2.Text}";
                }
            }
        }

        private void btMute_Click(object sender, EventArgs e)
        {
            cbMute.Checked = false;
            cbMute.Checked = true;
        }

        private void cbSurpressErrDialog_CheckedChanged(object sender, EventArgs e)
        {
            gv.bSurpressErrorDialog = cbSurpressErrDialog.Checked;
        }

        private void btFormatDGV1_Click(object sender, EventArgs e)
        {
            formatDataGridViewFileList((int)numColumnWidth.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rowIdMovieList = lastRowIdSet;
            // DGV1_EFFECT_SelectionChanged(rowIdMovieList);
        }

        private void btRefreshWindow2_Click(object sender, EventArgs e)
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                gv.dialogTraverser2.TraverseGo();
                gv.dialogTraverser2.ResetSortButtonColor(sender);
                gv.dialogTraverser2.SortName();
            }
        }
        int addWidth = 0;

        private void btOverlay_Click(object sender, EventArgs e)
        {
            OpenOverlay();
        }

        private void cbMinimize_CheckedChanged(object sender, EventArgs e)
        {
        }
        public void MinimizePlayer()
        {
            if (mp != null && !mp.IsDisposed)
                mp.MinimizeMovieWin(cbHidePlayer.Checked);
        }
        public void RestorePlayer()
        {
            if (mp != null && !mp.IsDisposed)
                mp.MinimizeMovieWin(false);
        }
        private void tbGoToSubFolder_TextChanged(object sender, EventArgs e)
        {
            SearchForFolder();
        }
        public void SearchForSubfolder(string folder) //searchforfolder old version don't use this one
        {
            string dir;

            for (; searchRow < dgv1.RowCount; ++searchRow)
            {
                dir = ff.getFolder(dgv1.Rows[searchRow].Cells[1].Value.ToString().ToUpper());
                if (dir == folder)
                {

                    if (true)
                    {
                        dgv1.CurrentCell = dgv1.Rows[searchRow].Cells[0];
                        dgv1.Rows[searchRow].Selected = true;

                        dgv1.FirstDisplayedScrollingRowIndex = searchRow;

                        dgv1.Refresh();
                        bFoundText = true;
                        tbFileSelected.Text = dgv1.Rows[searchRow].Cells["fpath"].Value.ToString();
                        tbRowId.Text = searchRow.ToString();
                        lastRowIdSet = searchRow;
                        //
                        tbMovieFpath.Text = dgv1.Rows[searchRow].Cells["fpath"].Value.ToString();
                        fpath = tbMovieFpath.Text;
                        tbFileNameSelected.Text = dgv1.Rows[searchRow].Cells[0].Value.ToString();
                        // rowIdMovieList = searchRow++;
                        rowIdMovieList = (int)dgv1.CurrentCell.RowIndex;
                        tbRowIdTemp.Text = rowIdMovieList.ToString();
                        break;
                    }
                }
            }
        }

        private void btFindDir_Click(object sender, EventArgs e)
        {
            tbGoToSubFolder.SelectAll();
            tbGoToSubFolder.Focus();
            SearchForFolder();
        }
        public int SearchForFolder() // THIS IS THE MAIN SEARCH OF THE DGV LIST for a certain (or partial) FILE NAME 
        {
            bool bFoundSubFolder = false;
            int searchRow = 0;
            dgv1.MultiSelect = false;
            dgv1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            string searchtxtUpper = tbGoToSubFolder.Text.ToUpper();
            if (searchtxtUpper.Length < 1)
                return -1;
            if (!searchtxtUpper.Substring(0, 1).Equals('/'))
                searchtxtUpper = '/' + searchtxtUpper;

            for (searchRow = 0; searchRow < dgv1.RowCount - 1; ++searchRow)
            {
                if (dgv1.Rows[searchRow].Cells["dpath"].Value.ToString().ToUpper().Contains(searchtxtUpper))
                {
                    dgv1.CurrentCell = dgv1.Rows[searchRow].Cells[0];
                    dgv1.Rows[searchRow].Selected = true;

                    dgv1.FirstDisplayedScrollingRowIndex = searchRow;

                    dgv1.Refresh();
                    bFoundSubFolder = true;
                    tbFileSelected.Text = dgv1.Rows[searchRow].Cells["fpath"].Value.ToString();

                    tbRowId.Text = searchRow.ToString();
                    lastRowIdSet = searchRow;
                    //
                    tbMovieFpath.Text = dgv1.Rows[searchRow].Cells["fpath"].Value.ToString();
                    fpath = tbMovieFpath.Text;
                    tbFileNameSelected.Text = dgv1.Rows[searchRow].Cells[0].Value.ToString();
                    // rowIdMovieList = searchRow++;
                    rowIdMovieList = (int)dgv1.CurrentCell.RowIndex;
                    tbRowIdTemp.Text = rowIdMovieList.ToString();
                    break;

                } //match found
            }
            tbError.Text = rowIdMovieList.ToString();
            if (bFoundSubFolder)
                return searchRow;
            else
            {
                tbSearchState.Text = "NOT found";
                return -1;
            }
        }
        public int SearchForNextMetaDataPosition() // find the next uncompleted entry for metadata
        {
            bool bFoundSubFolder = false;
            int searchRow = 0;
            dgv1.MultiSelect = false;
            dgv1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            for (searchRow = 0; searchRow < dgv1.RowCount - 1; ++searchRow)
            {
                int len = (int)dgv1.Rows[searchRow].Cells["width"].Value;
                if (len == 0)
                {
                    bool invalid = (bool)dgv1.Rows[searchRow].Cells["bInvalid"].Value;
                    if (!invalid)
                    {
                        dgv1.CurrentCell = dgv1.Rows[searchRow].Cells[0];
                        dgv1.Rows[searchRow].Selected = true;

                        dgv1.FirstDisplayedScrollingRowIndex = searchRow;

                        dgv1.Refresh();
                        bFoundSubFolder = true;
                        tbFileSelected.Text = dgv1.Rows[searchRow].Cells["fpath"].Value.ToString();

                        tbRowId.Text = searchRow.ToString();
                        lastRowIdSet = searchRow;
                        //
                        tbMovieFpath.Text = dgv1.Rows[searchRow].Cells["fpath"].Value.ToString();
                        tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
                        tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
                        fpath = tbMovieFpath.Text;
                        tbFileNameSelected.Text = dgv1.Rows[searchRow].Cells[0].Value.ToString();
                        // rowIdMovieList = searchRow++;
                        rowIdMovieList = (int)dgv1.CurrentCell.RowIndex;
                        tbRowIdTemp.Text = rowIdMovieList.ToString();
                        break;
                    }
                } //match found
            }
            tbError.Text = rowIdMovieList.ToString();
            if (bFoundSubFolder)
                return searchRow;
            else
                return -1;
        }
        private void tbGoToSubFolder_DoubleClick(object sender, EventArgs e)
        {
            tbGoToSubFolder.SelectAll();
        }

        private void numCopyCount_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cbPlayBackControls_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPlayBackControls.Checked)
                tbPlayBackError.Text = descPlayBackControls;
            else
                tbPlayBackError.Text = "";
            cbFocusHere.Checked = true;
        }

        public string GetNextMovieFullpath()
        {
            return tbMovieFpath.Text;
        }

        private void btErrReset_Click(object sender, EventArgs e)
        {
            mp.Close();
            bErrReset = true;
            tbRowId.Text = rowIdMovieList.ToString();
            playFirstVideo();
            //mp.ResetAndPlay(tbMovieFpath.Text);
        }

        private void btResetRID_Click_1(object sender, EventArgs e)
        {
            //rowIdMovieList
            int nextRID = Int32.Parse(tbRowId.Text); //rowIdMovieList
            rowIdMovieList = nextRID;
            mp.stop();
            mp.ResetAndPlay(tbMovieFpath.Text);


            //   DGV1_EFFECT_SelectionChanged(rowIdMovieList);
        }

        private void tbMovieFpath_TextChanged(object sender, EventArgs e)
        {

        }

        private void btGetMovieNameFromMoviesWindow_Click(object sender, EventArgs e)
        {
            tbMovieFpath.Text = "query movie win";
            mp.stop();
            tbMovieFpath.Text = mp.GetMovieFPath();
        }

        private void btMovies2_Click(object sender, EventArgs e)
        {
            mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
            mp2.Activate();
            mp2.Show();
            //gv.SetMovies(mp);mp.loadMovie(tbMovieFpath.Text, cbZoom.Checked); //playNext2
            TrackFromSourceLoad("playNext2");

            mp2.loadMovie(tbMovieFpath.Text, cbZoom.Checked); //playNext2
            mp2.play(); //playFirstVideo();
        }

        private void btSetSource_Click(object sender, EventArgs e)
        {
            rememberLastFolders(tbDirectoryPath.Text);
        }
        int startRowSearchNotFound = 0;

        private void btSearchUntilNotFound_Click(object sender, EventArgs e)
        {
            OpenPlayer();
            cbSearchUntilNotFoundIn2.Checked = true;
            playNextMovie(true);
        }
        public void OpenPlayer()
        {
            if (mp == null || mp.IsDisposed)
            {
                if (mpVersion1)
                    mp = new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                else
                    mp2 = /*P2*/ new WmPlayer(gv, this, cbOnErrContinue.Checked, tbMovieFpath.Text);
                mp.Activate();
                mp.Show();
                gv.SetMovies(mp);
                if (cbMute.Checked)
                    mp.mute(cbMute.Checked);
            }
        }
        public void SearchUntilNotFoundIn2()
        {
            cbSearchUntilNotFoundIn2.Checked = true;
        }
        private void btSetTarget2_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = gv.initParm1List[0].targetDir2;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = gv.initParm1List[0].targetDir2;
        }

        private void btSaveHistory_Click(object sender, EventArgs e)
        {
            gv.mainWindow.SaveHistoryList();
        }

        private void btAddFolder_Click(object sender, EventArgs e)
        {

        }

        private void btMostRecentDirUse_Click(object sender, EventArgs e)
        {
            if (bThisIsSubWindow)
                tbDirectoryPath.Text = gv.initParm1List[0].mostRecentSubfolder;
            else
                tbDirectoryPath.Text = gv.initParm1List[0].mostRecent;
        }

        private void cmboSetTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbTargetFolder.Text = cmboSetTarget.SelectedItem.ToString();
            gv.mainWindow.setTargetFolder(tbTargetFolder.Text);
        }

        private void btSetTargetDirToThis_Click(object sender, EventArgs e)
        {
            bool foundDir = ff.directoryExists(tbTargetFolder.Text);
            if (foundDir)
            {
                btSetTargetDirToThis.BackColor = Color.LightGreen;
                IsSpecialFolderSet();
            }
            else
            {
                btSetTargetDirToThis.BackColor = Color.LightGray;
            }
        }

        private void btSetupCopy_Click(object sender, EventArgs e)
        {
            cbSearchWin2EachSelection.Checked = true;
            cbSearchPartial.Checked = true;
            cbSetAllPlayerModes.Checked = true;
            cbCatalog.Checked = true;
            cbPlayBackControls.Checked = true;
        }

        private void btGetWindow2FolderTarget_Click(object sender, EventArgs e)
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                tbTargetFolder.Text = gv.dialogTraverser2.GetFolderFullPath();
                gv.mainWindow.setTargetFolder(tbTargetFolder.Text);
            }
        }

        private void cbSmallSize_CheckedChanged(object sender, EventArgs e)
        {
            if (mp == null || mp.IsDisposed)
                return;
            if (cbSmallSize.Checked)
            {
                cbZoom.Checked = false;
            }
            mp.SetSmallScreenLock(cbSmallSize.Checked);

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            counttest = (int)numTimerTicks.Value;
        }

        private void cbSetAllPlayer_CheckedChanged(object sender, EventArgs e)
        {
            PlayButtonGo();
            OpenTransparentWin();
            cbScanMovies.Checked = true;
            cbSetAllPlayerModes.Checked = true;
            ZoomMovieFullScreen();
        }

        private void tbRowIdTemp_TextChanged(object sender, EventArgs e)
        {

        }

        private void btFindNextMetaEntry_Click(object sender, EventArgs e)
        {
            SearchForNextMetaDataPosition();
        }

        private void btVerifyFolderExists_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(tbDirectoryPath.Text))
            {
                tbGoToSubFolder.Text = "err";
            }
            else
                tbGoToSubFolder.Text = "OK";
        }

        private void cbTN2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTN2.Checked)
                pbThumbNail.SizeMode = PictureBoxSizeMode.StretchImage;
            else if (cbTN3.Checked)
                pbThumbNail.SizeMode = PictureBoxSizeMode.AutoSize;
            else
                pbThumbNail.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void cbTN3_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTN3.Checked)
                pbThumbNail.SizeMode = PictureBoxSizeMode.AutoSize;
            else if (cbTN2.Checked)
                pbThumbNail.SizeMode = PictureBoxSizeMode.StretchImage;
            else
                pbThumbNail.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void cbTN4_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTN4.Checked)
                pbThumbNail.SizeMode = PictureBoxSizeMode.CenterImage;
            else
                pbThumbNail.SizeMode = PictureBoxSizeMode.Normal;
        }

        private void cbThumbNail_CheckedChanged(object sender, EventArgs e)
        {
            pb2.BringToFront();
            pbThumbNail.BringToFront();
            if (cbThumbNail.Checked)
            {
                pbThumbNail.SizeMode = PictureBoxSizeMode.Zoom;
                pbThumbNail.Image = gv.mainWindow.CreateThumbnailForFile(tbFileSelected.Text);
                pbThumbNail.Image = gv.mainWindow.CreateThumbnailForFile(tbMovieFpath.Text);
               // gv.mainWindow.displayThisImage(pbThumbNail.Image);
            }
            else
            {
                pbThumbNail.Image = null;
                pbThumbNail.Refresh();
                pb2.SendToBack();
                pbThumbNail.SendToBack();
            }
            pbThumbNail.Refresh();
        }

        private void cbMinScan_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMinScan.Checked)
            {
                numSampleCount.Value = 2;
                messageCount = 0;
            }
            else
            {
                numSampleCount.Value = 10;
                if (messageCount > 8)
                    messageCount = 8;
            }
        }
        string searchString;
        public void DisplaySearchingSubFor(string txtTitle = null)
        {
            searchString = txtTitle;
            tbStatusFoundInWin2.Text = "searching...";
            tbStatusFoundInWin2.BackColor = Color.LightGray;
        }
        public void DisplaySearchingSubFound(string title = null)
        {
            tbStatusFoundInWin2.Text = $"{title}";
            tbStatusFoundInWin2.BackColor = Color.LightGreen;
        }
        public void DisplaySearchingSubNotFound(string title = null)
        {
            tbStatusFoundInWin2.Text = $"NOT FOUND {title}";
            tbStatusFoundInWin2.BackColor = Color.LightPink;
        }

        private void cbSearchUntilNotFoundIn2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSearchUntilNotFoundIn2.Checked)
            {
                this.Refresh();
                cbSearchUntilNotFoundIn2.Refresh();
                SearchUntilNotFoundIn2();
                if (cbPicture.Checked)
                    cbSlideShow.Checked = true;
            }
            else
            {
                if (cbPicture.Checked)
                    cbSlideShow.Checked = false;
            }
        }

        private void cbPositionSearchStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (!bThisIsSubWindow)
                return;
        }

        private void tbTargetFolder_TextChanged(object sender, EventArgs e)
        {
            //   cbSpecialFolder.Checked = UsingSpecialFolder(tbTargetFolder.Text);
        }

        /*
        DriveInfo di = ff.GetDriveInfo(tbTargetFolder.Text);
        if (di != null)
            tbTargetFreeSpace.Text = $"free space {di.AvailableFreeSpace}";
        */

        private void TraverserDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
                mp.Close();
        }

        private void btTestAddRowToSubWindow_Click(object sender, EventArgs e)
        {
            AddFInfoEntryToSubWindow();
        }
        public void AddFInfoEntryToSubWindow() // when copied 
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                FileInfoItem fi = GetSelectedFileInfoItem();
                UpdateFinfoMovieResolution(fi);
                gv.dialogTraverser2.AddFileInfoRow(fi);
            }
        }
        public void AddFInfoEntryToSubWindowUpdatedPath(string target) // when copied 
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                FileInfoItem fi = GetSelectedFileInfoItem();
                fi.dpath = target;
                fi.fpath = target + fi.fname;
                gv.dialogTraverser2.AddFileInfoRow(fi);
            }
        }
        private void btInitMovieWindow_Click(object sender, EventArgs e)
        {

        }

        private void cbDMC_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbResaveFileEachTime_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btUseImageList_Click(object sender, EventArgs e)
        {
             //ImageFileList
            //class ImageFileList
            if (gv.imageFileList1 == null)
            {
                gv.imageFileList1 = new ImageFileList();
                if (bThisIsSubWindow)
                    gv.imageFileList2.finfoList = imageFileList2.finfoList;
                else
                    gv.imageFileList1.finfoList = imageFileList1.finfoList;
            }
            else
            {
                if (bThisIsSubWindow)
                    imageFileList2 = gv.imageFileList2; //initial backup of original <<<<<<<<<<<<<<<<<<<<<<<<<<<<
                else
                    imageFileList1 = gv.imageFileList1; //initial backup of original <<<<<<<<<<<<<<<<<<<<<<<<<<<<
            }

            dgv1.DataSource = null;
            dgv1.DataSource = gv.imageFileList1.finfoList;
            //   dgv1.DataBindings.Add("DataSource", gv, "imageFileList1");

            gv.mainWindow.SetFileType("images");
        }

        private void btGoToOtherTravWin_Click(object sender, EventArgs e)
        {
            cbFocusHere.Checked = false;
            if (bThisIsSubWindow)
            {
                parentWin.Activate();
            }
            else
            {
                gv.dialogTraverser2.Activate();
            }
        }

        private void btUnset_Click(object sender, EventArgs e)
        {
            StopPlaying();
            cbSetAllPlayerModes.Checked = false;
            cbCatalog.Checked = false;
            cbPlayBackControls.Checked = false;
        }

        private void cbCatalog2_CheckedChanged(object sender, EventArgs e)
        {
            cbCatalog.Checked = true;
            cbSearchWin2EachSelection.Checked = true;
            cbSearchPartial.Checked = true;
        }
        private void btAllFiles_Click(object sender, EventArgs e)
        {
            cbAutoPlay.Checked = true;
            cbMovies.Checked = false;
            cbMovie.Checked = false;
            cbPicture.Checked = false;
            cbMIDI.Checked = false;
            cbFilesAll.Checked = true;
            bSavedFile = false;
            btVideos.BackColor = Color.LightGreen;
            if (!TraverseGo())
            {
                tbMovieFpath.Text = "!! ERROR Invalid directory !!";
                MessageBox.Show("!! ERROR Invalid directory !!");
                return;
            }

            gv.mainWindow.SetFileType("ALL");
            bLoadedList = true;
        }
        private void btMIDI_Click(object sender, EventArgs e)
        {
            cbAutoPlay.Checked = true;
            cbMovies.Checked = false;
            cbMovie.Checked = false;
            cbPicture.Checked = false;
            cbMIDI.Checked = true;
            bSavedFile = false;
            btVideos.BackColor = Color.LightGreen;
            if (!TraverseGo())
            {
                tbMovieFpath.Text = "!! ERROR Invalid directory !!";
                MessageBox.Show("!! ERROR Invalid directory !!");
                return;
            }

            gv.mainWindow.SetFileType("MIDI");
            bLoadedList = true;
        }

        private void cbCompare_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCompare.Checked)
            {
                if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
                {
                    btCompare.Text = "compare";
                    gv.dialogTraverser2.bComparisionModeInTrav1 = cbCompare.Checked;
                }
                else
                    cbCompare.Checked = false;
            }
            pb2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void cbFindFileName_CheckedChanged(object sender, EventArgs e)
        {
            gv.bFindFileName = cbFindFileName.Checked;
            if (cbFindFileName.Checked)
                gv.findFileName = tbFindFileName.Text;
        }

        private void btCloseThisOneOnly_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btCheckFileList_Click(object sender, EventArgs e)
        {
            if (bThisIsSubWindow)
                tbRowId.Text = imageFileList2.getImageCount().ToString();
            else
                tbRowId.Text = imageFileList1.getImageCount().ToString();
        }

        private void btSendList3_Click(object sender, EventArgs e)
        {
            gv.mainWindow.SetImageList3(imageFileList2); //imageFileList2
        }

        private void btMoveToDeleteList_Click(object sender, EventArgs e)
        {
            FileInfoItem fi = GetSelectedFileInfoItem();
            gv.fileListWin.AddItem(fi);
            gv.fileListWin.Activate();
        }

        private void btShowWinList_Click(object sender, EventArgs e)
        {
            gv.fileListWin.WindowState = FormWindowState.Normal;
            gv.fileListWin.Activate();
        }
        string specialDirectory;
        string originalTargetDirectory;
        bool bUsingSpecialDirectory = false;
        private void btSetTarget2Special_Click(object sender, EventArgs e)
        {
        }
        private void cbSpecialFolder_CheckedChanged(object sender, EventArgs e)
        {
        }
        public bool UsingSpecialFolder(string fullPath)
        {
            if (fullPath == null)
                fullPath = tbTargetFolder.Text;
            // Remove trailing '/' or '\' if present
            fullPath = fullPath.TrimEnd('/', '\\');
            if (fullPath.EndsWith("/AAAA") || fullPath.EndsWith("\\AAAA"))
            {
                main.setTargetFolder(fullPath);
                return true;
            }
            else
            {
                main.setTargetFolder(fullPath);
                return false;
            }
        }
        public string SetSpecialFolder(bool bOn, string folderName = null)
        {
            if (folderName == null)
                folderName = tbTargetFolder.Text;
            folderName = folderName.TrimEnd('/', '\\');
            bool rc = UsingSpecialFolder(folderName);

            if (bOn)
            {
                if (!rc)
                {
                    folderName = folderName + "/AAAA";
                    main.setTargetFolder(folderName);
                }
            }
            else //else REMOVE specialFolder from TARGET
            {
                // If using the special folder, remove "/AAAA" or "\AAAA"
                if (folderName.EndsWith("/AAAA", StringComparison.OrdinalIgnoreCase))
                {
                    folderName = folderName.Substring(0, folderName.Length - "/AAAA".Length);
                }
                else if (folderName.EndsWith("\\AAAA", StringComparison.OrdinalIgnoreCase))
                {
                    folderName = folderName.Substring(0, folderName.Length - "\\AAAA".Length);
                }
            }
            tbTargetFolder.Text = folderName;
            main.setTargetFolder(folderName);
            return folderName;
        }
        public void SetSpecialFolder(bool bOn, bool off)  //btSetTarget2Special
        {
            int idx = 0;
            while (++idx < 3)
            {
                btSetTargetDirToThis.BackColor = Color.LightGray;
                if (tbTargetFolder.Text.Contains("AAAA"))
                    return;
                bool foundDir = ff.directoryExists(tbTargetFolder.Text);
                if (foundDir)
                {
                    originalTargetDirectory = tbTargetFolder.Text;
                    specialDirectory = tbTargetFolder.Text + "\\AAAA";
                    bool foundDir2 = ff.directoryExists(specialDirectory);
                    if (!foundDir2)
                    {
                        Directory.CreateDirectory(specialDirectory);
                    }
                    gv.mainWindow.setTargetFolder(specialDirectory);
                    btSetTarget2Special.BackColor = Color.LightGreen;
                    btSetTarget2Special.Text = "AAAA";
                    tbTargetFolder.Text = specialDirectory;
                }
                if (tbTargetFolder.Text.Contains("AAAA"))
                    idx = 9;
            }
            bUsingSpecialDirectory = true;
        }
        public void ResetOriginalFolder()
        {
            btSetTarget2Special.BackColor = Color.LightGray;
            btSetTarget2Special.Text = "base";
            btSetTargetDirToThis_Click(null, null);
            bUsingSpecialDirectory = false;
            tbTargetFolder.Text = originalTargetDirectory;
            gv.mainWindow.setTargetFolder(tbTargetFolder.Text);
        }

        private void btResetTarget1_Click(object sender, EventArgs e)
        {
        }

        public void btHideMovie_Click(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.HidePlayer();
            }
        }
        public void UnHidePlayer()
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.UnHidePlayer();
            }

        }

        private void btUnHidePlayer_Click(object sender, EventArgs e)
        {
            UnHidePlayer();
        }

        private void btResizeHider_Click(object sender, EventArgs e)
        {
            if (mp != null && !mp.IsDisposed)
            {
                mp.ResizeWindow();
            }
        }

        private void tbDirectoryPath_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddHistoryItem(tbDirectoryPath.Text);
        }

        private void cbPicTopMost_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPicTopMost.Checked)
            {
                pbThumbNail.BringToFront();
                pb2.BringToFront();
            }
            else
            {
                pbThumbNail.SendToBack();
                pb2.SendToBack();
            }

        }

        private void cb1_CheckedChanged(object sender, EventArgs e)
        {
            tbHistory1.Text = tbDirectoryPath.Text;
            if (checkBox1.Checked)
                tbHistory1.Text = tbTargetFolder.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            tbHistory2.Text = tbDirectoryPath.Text;
            if (checkBox2.Checked)
                tbHistory2.Text = tbTargetFolder.Text;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            tbHistory3.Text = tbDirectoryPath.Text;
            if (checkBox3.Checked)
                tbHistory3.Text = tbTargetFolder.Text;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            tbHistory4.Text = tbDirectoryPath.Text;
            if (checkBox4.Checked)
                tbHistory4.Text = tbTargetFolder.Text;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            tbHistory5.Text = tbDirectoryPath.Text;
            if (checkBox5.Checked)
                tbHistory5.Text = tbTargetFolder.Text;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            tbHistory6.Text = tbDirectoryPath.Text;
            if (checkBox6.Checked)
                tbHistory6.Text = tbTargetFolder.Text;
        }

        private void cbSave_CheckedChanged(object sender, EventArgs e)
        {
            gv.initParm1List[0].mostRecent1 = tbHistory1.Text;
            gv.initParm1List[0].mostRecent2 = tbHistory2.Text;
            gv.initParm1List[0].mostRecent3 = tbHistory3.Text;
            gv.initParm1List[0].mostRecent4 = tbHistory4.Text;
            gv.initParm1List[0].mostRecent5 = tbHistory5.Text;
            gv.initParm1List[0].mostRecent6 = tbHistory6.Text;
        }

        private void tbHistory6_TextChanged(object sender, EventArgs e)
        {

        }



        private void btSendT1_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = tbHistory1.Text;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = tbHistory2.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = tbHistory3.Text;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = tbHistory4.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = tbHistory5.Text;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = tbHistory6.Text;
        }

        private void btPlayNextInList_Click(object sender, EventArgs e)
        {
            if (bThisIsSubWindow)
                StartSearchWin2Title(lastSearchTitle);

        }

        private void btSend1T_Click(object sender, EventArgs e)
        {
            tbTargetFolder.Text = tbHistory1.Text;
        }

        private void btSend2T_Click(object sender, EventArgs e)
        {
            tbTargetFolder.Text = tbHistory2.Text;
        }

        private void btSend3T_Click(object sender, EventArgs e)
        {
            tbTargetFolder.Text = tbHistory3.Text;
        }

        private void btSend4T_Click(object sender, EventArgs e)
        {
            tbTargetFolder.Text = tbHistory4.Text;
        }

        private void btSend5T_Click(object sender, EventArgs e)
        {
            tbTargetFolder.Text = tbHistory5.Text;
        }

        private void btSend6T_Click(object sender, EventArgs e)
        {
            tbTargetFolder.Text = tbHistory6.Text;
        }

        private void btFastForward_Click(object sender, EventArgs e)
        {
            mp.fastForward();
        }

        private void btReverse_Click(object sender, EventArgs e)
        {
            mp.Reverse();
        }

        private void btRefreshWin2_Click(object sender, EventArgs e)
        {
            if (gv.dialogTraverser2 != null && !gv.dialogTraverser2.IsDisposed)
            {
                tbTargetFolder.Text = gv.dialogTraverser2.GetFolderFullPath();
                gv.dialogTraverser2.btVideos_Click(sender, e);
            }
        }

        private void cbDateDescending_CheckedChanged(object sender, EventArgs e)
        {
            //  ResetSortButtonColor(sender);
        }

        private void cbDocsOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFilesAll.Checked)
            {
                cbFilesAll.BackColor = Color.Red;
            }
            else
            {
                cbFilesAll.BackColor = Color.LightGray;
            }
        }

        private void btFoldersList_Click(object sender, EventArgs e)
        {
            for (int idx = 0; idx < gv.imageFileList1.getImageCount(); ++idx)
            {

                string fpath = gv.imageFileList1.getIndexed(idx).dpath;


            }
            //AddFolderInfoRow
        }

        private void btTestNormalizeFilePath_Click(object sender, EventArgs e)
        {
            tbCopyFileName.Text = NormalizePath(tbDirectoryPath.Text);
        }

        private void cbScanAndPlay_CheckedChanged(object sender, EventArgs e)
        {
            if (cbScanAndPlay.Checked)
                numSampleCount.Value = 5;
        }

        private void cbOverLayTransparentWin_CheckedChanged(object sender, EventArgs e)
        {
            bUseOverlay = cbOverLayTransparentWin.Checked;
        }

        private void btSetScanFolderToTarget_Click(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = tbTargetFolder.Text;
        }
        public void SendUpdatePb2FromSub(Bitmap pic)
        {
            pb2.Image = pic;
            CompareImageWithTraverser2();
        }
        private void btRefreshCompare2_Click(object sender, EventArgs e)
        {
            GetBmp2();
        }

        private void btReselectSelectCurrentRow_Click(object sender, EventArgs e)
        {
            if (cbLoadedAndReady.Checked)
                AdvanceSelectionDGV1();
            dgv1_SelectionChanged(null, null);
        }

        private void cbSuspendScan_CheckedChanged(object sender, EventArgs e)
        {
            SetScanPause(cbSuspendScan.Checked);
        }

        private void cbSlideShow_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSlideShow.Checked)
            {
                if (cbPicture.Checked)
                {
                    cbSearchUntilNotFoundIn2.Checked = true;
                    btPlay_Click(null, null);
                }
            }
            else
            {
                if (cbSearchUntilNotFoundIn2.Checked)
                {
                    cbSearchUntilNotFoundIn2.Checked = false;
                    foundInWin2 = true;
                }
            }
        }

        private void btShowTargetDir_Click(object sender, EventArgs e)
        {
            tbTargetFolder.Text = gv.initParm1List[0].targetDir1;
        }

        private void btDeleteMarkedFiles_Click(object sender, EventArgs e)
        {
            int count = DeleteMarkedFiles();
            if (count > 0)
            {
                MessageBox.Show($"deleted file count {count}", "deleted files REFRESH List ");
            }
            else
                MessageBox.Show($"No file were marked for deletion", "Did not delete files");
        }

        private void cbSetSpecialFolder_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSetSpecialFolder.Checked)
                SetSpecialFolder(true);
            else
                SetSpecialFolder(false);
        }

        // This event handler is triggered when the user clicks the copy button.
        private void CopyWithProgress(string sourceFile, string destinationFile)
        {
            // Define your source and destination file paths.
           // string sourceFile = @"C:\Path\To\Your\Source\File.mp4";
          // string destinationFile = @"C:\Path\To\Your\Destination\File.mp4";

            // Call the helper method to copy the file while updating the progress bar.
            FileCopyWithProgress.CopyFileWithProgressBar(sourceFile, destinationFile, progressBar2);
        }

        private void cbAVIF_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAVIF.Checked)
            {
                cbThumbNail.Checked = true;
            }
            else
            {
                cbPause2.Checked = false;
            }
        }

        private void cbCompareToPreviousImage_CheckedChanged(object sender, EventArgs e)
        {
            pb2.BringToFront();
            pbThumbNail.BringToFront();
            pb2.Size = pbThumbNail.Size;
            pb2.SizeMode = PictureBoxSizeMode.Zoom;
        }
        
        public int copyFileList2Main2(List<FileInfoItem> list)
        {
            gv.imageFileListCompare.finfoList.Clear();
            for (int idx = 0; idx < list.Count(); ++idx)
            {
                gv.imageFileListCompare.finfoList.Add(list[idx]);
            }
            return gv.imageFileListCompare.finfoList.Count;
        }
        private void cbCopyFileListMain_CheckedChanged(object sender, EventArgs e)
        {
            if (bThisIsSubWindow && cbCopyFileListMain.Checked)
            {
                gv.imageFileListCompare = new ImageFileList();
                copyFileList2Main2(gv.imageFileList2.finfoList);
            }

        }
    }

}






/////////////////////////////////////////////////////////
