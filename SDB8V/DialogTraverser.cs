#nullable disable
//broke
using LibVLCSharp.Shared;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using SymbolDB;
using SymbolDB.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
//using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using static SymbolDB.GlobalVars;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using static System.Net.Mime.MediaTypeNames;
//using static System.Net.Mime.MediaTypeNames;


namespace SymbolDB  //dmc26
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


    public partial class DialogTraverser : Form  //----------- FORM 
    {
#pragma warning disable IDE0044 // Add readonly modifier
        GlobalVars gv;
#pragma warning restore IDE0044 // Add readonly modifier
        //set root folder
        InitFolder startFolder = InitFolder.MyComputer; //starting Folder is set my radiobutton choice or by parm
        string targetFolder;
        FileFunctions ff;

        private Player vlcPlayer;
        private Player vlcPlayer2;


        private bool bAlsoLoadMetadata = false;
        public void EnableMetadataLoading(bool enable) => bAlsoLoadMetadata = enable;


        public enum TraversalActionState
        {
            TRAVERSING, LOADING_DISPLAY, NONE, COMPLETED
        }

        TraversalActionState action = TraversalActionState.NONE;
        //
        //
        //
        //TraverserBG traverser;
        private dynamic? traverser = null;
        public int usingBGnumber = 4;
        //
        //
        public string fullpath;
        //const int gv.iMaxFileCount = 9999;

        //Point listViewLoc;


        Main? main = null;
        DialogTraverser? parentWin;
        public bool bThisIsSubWindow = false;

        //public int rowCount = 0; // row count in listView1

        private ListViewColumnSorter lvwColumnSorter;

        private readonly FileInfoRepository _repo = new FileInfoRepository();
        // HashSet to track changed rows
        private HashSet<int> changedRows = new HashSet<int>();
        // Add these fields to the DialogTraverser class (after existing field declarations)
        private bool _metadataUpdatedAfterTraversal = false;
        private string _lastTraversalType = ""; // "images" or "videos"

        public DialogTraverser(GlobalVars g, Main mainParent, int imode, FileFunctions f, DialogTraverser parent = null) /////////////////////////////////////////////////////////////////////////
        {
            gv = g;
            InitializeComponent();

            // Restore last selected extension category from previous session
            string lastExt = Properties.Settings.Default.LastSearchExtension;
            if (!string.IsNullOrEmpty(lastExt))
            {
                gv.searchExtensionCategoryInUse = lastExt;
                selectedSearchExtensionType = lastExt;   // ✅ set local field too
                setDefaultSearchExtension = true;
            }
            // Populate the search extensions ComboBox
            PopulateSearchExtensionsComboBox();
            // to scale everything by 25%.
            /*
            float scaleFactor = 1.5f;
            // Scale the form and all child controls.
            if (parent == null)
                this.Scale(new SizeF(scaleFactor, scaleFactor));

            float scaleFactor = 0.67f; // 2/3 size
            ScaleFormAndControls(scaleFactor);
            */
            //gv.dialogTraverser = this;
            // Subscribe to the CellValueChanged event
            dgvFileInfo.CellValueChanged += Dgv1_CellValueChanged;



            // Initialize previously unassigned fields:
            lvwColumnSorter = new ListViewColumnSorter();
            tbStatus1 = string.Empty;
            tbStatus2 = string.Empty;

            //numBGversion.Value = 5;

            CheckLibVLCRuntimeAndLogMethods();


            // Ensure edits are committed when the user leaves a cell
            dgvFileInfo.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvFileInfo.IsCurrentCellDirty)
                {
                    dgvFileInfo.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            gv.fileListWin = new FileList(gv, f);
            gv.fileListWin.Show();
            gv.fileListWin.Activate();
            gv.fileListWin.WindowState = FormWindowState.Minimized;

            cbPlayBackControls.Checked = true;
            tbMainOrSubWindow.Text = "Main Window";

            startFolder = InitFolder.Special; // tbDirectoryPath.Text;

            startFolder = InitFolder.MyComputer; // MyDesktop;
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
                gv.dialogTraverser1 = this;
                cbCopyFileListMain.Checked = true;
            }
            // Subscribe to the CellValueChanged event
            dgvFileInfo.CellValueChanged += Dgv1_CellValueChanged;
            dgvFileInfo.KeyDown += DgvFileInfo_KeyDown;
            dgvFileInfo.DataBindingComplete += DgvFileInfo_DataBindingComplete;

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
            //traverser = new TraverserBG(gv, this);
            // traverser will be created on demand by EnsureTraverserInstance()
            traverser = null;
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
            addHistoryToSourceCmboList();
            if (bThisIsSubWindow)
                tbDirectoryPath.Text = gv.initParm1List[0].mostRecentSubfolder;
            LoadMostRecentFolders();

            dgvFileInfo.MultiSelect = false;
            btDisplayHTML.BackColor = Color.LightGray;

            if (gv.useTestSourceAndTargetFolder)
            {
                tbDirectoryPath.Text = gv.testSourceFolder;
            }
        } ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Add to the existing partial class
        //private PlaybackControlForm? _playbackForm;


        private void DgvFileInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F3)
                return;

            if (cbPlayBackControls.Checked)
            {
                if (_usePictures)
                    GoToNextRow();
                else
                    SkipForward1min();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void DgvFileInfo_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DisableDgvSorting(dgvFileInfo);
        }

        private static void DisableDgvSorting(DataGridView dgv)
        {
            if (dgv == null)
                return;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }



        //csharp SymbolDB\DialogTraverser.cs
        //
        // LibVLC player related properties
        //
        // Seconds to skip with the small / large buttons
        private const int SkipSmallSeconds = 5;
        private const int SkipLargeSeconds = 60;

        // For convenience – actual LibVLC media player
        private LibVLCSharp.Shared.MediaPlayer? Player =>
            vlcPlayer != null ? vlcPlayer.MediaPlayer : null;

        private bool IsPlayerReady =>
            vlcPlayer != null &&
            !vlcPlayer.IsDisposed &&
            Player != null &&
            Player.Media != null;
        //
        //
        //

        private void EnsureTraverserInstance()
        {

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
        // Event handler for CellValueChanged
        private void Dgv1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure it's not a header row
            {
                // Add the row index to the HashSet
                changedRows.Add(e.RowIndex);
            }
        }
        // Method to process changed rows
        // Call this from your "Apply Updates" button
        private int ProcessChangedRows()
        {
            // Nothing to do?
            if (dgvFileInfo == null || dgvFileInfo.Rows.Count == 0 || changedRows.Count == 0)
                return 0;

            int updated = 0, skipped = 0, errors = 0;

            using (var conn = new SqliteConnection(SqliteDb.ConnectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    // Work on a snapshot so we can safely Clear() later
                    foreach (int rowIndex in changedRows.ToList())
                    {
                        if (rowIndex < 0 || rowIndex >= dgvFileInfo.Rows.Count)
                        {
                            skipped++;
                            continue;
                        }

                        var row = dgvFileInfo.Rows[rowIndex];
                        if (row.IsNewRow)
                        {
                            skipped++;
                            continue;
                        }

                        // *** KEY CHANGE: use DataBoundItem instead of reading cells ***
                        if (row.DataBoundItem is not FileInfoItem item)
                        {
                            skipped++;
                            continue;
                        }

                        try
                        {
                            // fpath is the only thing we absolutely need
                            var fpath = item.fpath?.Trim();
                            if (string.IsNullOrWhiteSpace(fpath))
                            {
                                skipped++;
                                continue;
                            }

                            // bDelete – for now, skip deleted rows (you could change this to a DeleteByFpath)
                            if (item.bDelete)
                            {
                                skipped++;
                                continue;
                            }

                            // Ensure keyPath is set (and matches your DB key convention)
                            // If you’ve changed the primary key to keyPath, this is critical.
                            item.keyPath = PathHelpers
                                .ToNormalizedKey(fpath)
                                ?.ToUpperInvariant() ?? "";

                            // Optionally: ensure fname/ext/dpath are consistent with fpath
                            if (string.IsNullOrWhiteSpace(item.fname))
                                item.fname = Path.GetFileName(fpath);

                            if (string.IsNullOrWhiteSpace(item.ext) && !string.IsNullOrWhiteSpace(item.fname))
                                item.ext = Path.GetExtension(item.fname).TrimStart('.', ' ');

                            if (string.IsNullOrWhiteSpace(item.dpath))
                                item.dpath = Path.GetDirectoryName(fpath) ?? "";

                            // Let the repository handle INSERT/UPDATE
                            _repo.Upsert(item, conn, tx);
                            updated++;
                        }
                        catch
                        {
                            errors++;
                        }
                    }

                    tx.Commit(); //commit COMMIT 
                }
            }

            changedRows.Clear();

            DBstatus($"Processed changed rows:\nUpdated: {updated}\nSkipped: {skipped}\nErrors: {errors}");

            return updated;
        }


        public void DBstatus(string msg)
        {
            tbDbMessage.Text = msg;
            tbDbMessage.BackColor = Color.LightYellow;
        }

        // ---- Helper functions for ProcessChangedRows ----

        // Get the string value of a cell (case-insensitive column lookup)
        private static string GetCellString(DataGridViewRow row, string colName)
        {
            var col = row.DataGridView.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c =>
                    string.Equals(c.Name, colName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c.DataPropertyName, colName, StringComparison.OrdinalIgnoreCase));

            if (col == null) return null;

            var val = row.Cells[col.Index].Value;
            return val?.ToString()?.Trim();
        }

        // Convert strings like "true", "1", "yes", "y" into bools
        private static bool TryBool(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            if (bool.TryParse(s, out var b)) return b;
            if (int.TryParse(s, out var i)) return i != 0;
            return s.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("t", StringComparison.OrdinalIgnoreCase);
        }

        // Build a FileInfoItem from the DataGridViewRow
        private FileInfoItem BuildItemFromRow(DataGridViewRow row)
        {
            if (row == null || row.IsNewRow)
                throw new ArgumentException("Invalid row passed to BuildItemFromRow.");

            string? fpath = GetCellString(row, "fpath");
            if (string.IsNullOrWhiteSpace(fpath))
                throw new Exception("Row has no fpath and cannot be saved to DB.");

            var item = new FileInfoItem
            {
                fpath = fpath,
                keyPath = NormalizeFullPathForDb(fpath),

                fname = GetCellString(row, "fname")
                             ?? Path.GetFileName(fpath),

                ext = GetCellString(row, "ext")
                             ?? Path.GetExtension(fpath).TrimStart('.', ' '),

                len = TryLong(GetCellString(row, "len")),
                stimestamp = GetCellString(row, "stimestamp")
                             ?? GetCellString(row, "stimestamp")
                             ?? "",

                rating = TryChar(GetCellString(row, "rating")),
                comment = GetCellString(row, "comment")
                             ?? GetCellString(row, "comment")
                             ?? "",

                bDelete = TryBool(GetCellString(row, "bDelete")),
                bInvalid = TryBool(GetCellString(row, "bInvalid")),
                width = TryInt(GetCellString(row, "width")),
                height = TryInt(GetCellString(row, "height")),
                source = GetCellString(row, "source") ?? "",
                playTime = TryDouble(GetCellString(row, "playTime")),
                minutes = TryInt(GetCellString(row, "minutes")),
                seconds = TryInt(GetCellString(row, "seconds")),

                dpath = GetCellString(row, "dpath")
                             ?? Path.GetDirectoryName(fpath)
                             ?? "",

                ndx = TryInt(GetCellString(row, "ndx")
                             ?? GetCellString(row, "index")),

                type = GetCellString(row, "type") ?? "f",
                level = TryInt(GetCellString(row, "level"))
            };

            // Final fallbacks
            if (string.IsNullOrWhiteSpace(item.fname))
                item.fname = Path.GetFileName(fpath);

            if (string.IsNullOrWhiteSpace(item.ext))
                item.ext = Path.GetExtension(item.fname).TrimStart('.', ' ');

            return item;
        }


        // Parsing helpers
        private static int TryInt(string? s, int def = 0) => int.TryParse(s, out var v) ? v : def;
        private static long TryLong(string? s, long def = 0) => long.TryParse(s, out var v) ? v : def;
        private static double TryDouble(string? s, double def = 0) => double.TryParse(s, out var v) ? v : def;
        private static char TryChar(string? s) => string.IsNullOrWhiteSpace(s) ? ' ' : s.Trim()[0];





        private void ProcessChangedRowsToFile()
        {
            foreach (int rowIndex in changedRows)
            {
                // Access the row using the rowIndex
                DataGridViewRow row = dgvFileInfo.Rows[rowIndex];

            }

            // Clear the HashSet after processing
            changedRows.Clear();
        }
        // Example: Retrieve values from the row
        //string filePath = row.Cells["fpath"].Value?.ToString();
        //char rating = row.Cells["rating"].Value != null ? Convert.ToChar(row.Cells["rating"].Value) : ' ';
        //string description = row.Cells["desc"].Value?.ToString();

        // Process the row (e.g., save to a database, update a file, etc.)
        //MessageBox.Show($"Processing Row {rowIndex}: fpath={filePath}, rating={rating}, comment={comment}");

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
            if (cbFocusHere.Checked) this.Focus();

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
            if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
                dgvFileInfo.CurrentCell = dgvFileInfo.Rows[rowIdMovieList].Cells[0]; // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList].Cells[0];  correct
                dgvFileInfo.Rows[rowIdMovieList].Selected = true; //SELECTED WILL SET rowIdMovieList 

                if (dgvFileInfo.SelectedRows.Count > 1)
                    MessageBox.Show("multiselect", "error");
                dgvFileInfo.Refresh();
            }
            bAdvanced = false;

            return bAdvanced;
        }
        //  TopMost? continueWindow;
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
            while (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
            if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
                dgvFileInfo.Refresh();
                tbRowIdTemp.Refresh();
            }
            else
            {
                cbScanMovies.Checked = false;
                cbSearchUntilNotFoundIn2.Checked = false;
                return;
            }
        }
        DisplayMessage? displayMessage;
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


        PromptContinueWindow? continueDialog;

        DialogDeleteDup? deleteDup;

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
        public void loadSourceCmboList2()
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

        public void loadSourceCmboList()
        {
            cmboSourceFolder.Items.Clear();
            var sources = new[]
            {
                gv.initParm1List[0].sourceDir1 ?? "",
                gv.initParm1List[0].sourceDir2 ?? "",
                gv.initParm1List[0].sourceDir3 ?? "",
                gv.initParm1List[0].sourceDir4 ?? "",
                gv.initParm1List[0].sourceDir5 ?? "",
                gv.initParm1List[0].sourceDir6 ?? ""
            };

            foreach (var path in sources)
            {
                if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                    cmboSourceFolder.Items.Add(path);
            }

            // Also add folderHistoryList entries (newest first), skipping duplicates
            for (int idx = gv.folderHistoryList.Count - 1; idx >= 0; --idx)
            {
                var folderPath = gv.folderHistoryList[idx].folderPath;
                if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
                {
                    if (!cmboSourceFolder.Items.Contains(folderPath))
                        cmboSourceFolder.Items.Add(folderPath);
                }
                else
                    gv.folderHistoryList.RemoveAt(idx); // Remove unavailable
            }

            if (cmboSourceFolder.Items.Count > 0)
                cmboSourceFolder.SelectedIndex = 0;
        }

        public void addHistoryToSourceCmboList()
        {
            for (int idx = 0; idx < gv.folderHistoryList.Count; ++idx)
            {
                var folderPath = gv.folderHistoryList[idx].folderPath;
                if (!cmboSourceFolder.Items.Contains(folderPath))
                    cmboSourceFolder.Items.Add(folderPath);
            }
            cmboSourceFolder.SelectedIndex = 0;
        }

        public void loadSourceCmboListFromParmWindow() //NOT USED
        {
            for (int idx = 0; idx < gv.folderHistoryList.Count; ++idx)
            {
                cmboSourceFolder.Items.Add(gv.folderHistoryList[idx].folderPath);
            }
            cmboSourceFolder.SelectedIndex = 0;
        }

        ImageFileList? imageFileFolderList;
        ImageFileList? imageFileList1;
        ImageFileList? imageFileList2;
        ImageFileList? imageFileListOriginal1; //in the scope of this Traverser
        ImageFileList? imageFileListOriginal2; //in the scope of this Traverser
                                               //   ImageFileList imageFileListOriginalHold;

        public Boolean insertRowsInto_LV_ImageList(ImageFileList imageFileList, int count)
        {

            return true;
        }
        public void resetOriginalList()
        {
            resetDGV();
        }

        public void AutoSizeDGVColumns()
        {
            dgvFileInfo.SuspendLayout();
            try
            {
                // Disable continuous auto-sizing (it re-measures on every paint otherwise)
                dgvFileInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                // Only measure visible rows — dramatically faster than AllCells on large lists
                dgvFileInfo.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

                //SetMaxColumnWidth("keyPath", 200);
                SetMaxColumnWidth("fname", 200);
                SetMaxColumnWidth("dpath", 300);
            }
            finally
            {
                dgvFileInfo.ResumeLayout(false);
            }
        }
        public void AutoSizeDGVColumns2()
        {
            dgvFileInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvFileInfo.AutoResizeColumns();
            SetMaxColumnWidth("keyPath", 200);
            SetMaxColumnWidth("fname", 200);
            SetMaxColumnWidth("dpath", 300);
        }
        /// <summary>
        /// Enforces a max width on a specific column *after* autosizing.
        /// </summary>
        private void SetMaxColumnWidth(string columnName, int maxWidth)
        {
            if (!dgvFileInfo.Columns.Contains(columnName))
                return;

            var col = dgvFileInfo.Columns[columnName];

            if (col.Width < maxWidth)
                col.Width = maxWidth + 100;

            // Optional: Prevent user from stretching past max
            col.Resizable = DataGridViewTriState.True;
            col.MinimumWidth = Math.Min(col.Width, maxWidth);
            col.FillWeight = 1;
        }
        public void resetDGV()
        {
            dgvFileInfo.DataSource = null;
            List<FileInfoItem> orderedByFileName;
            if (bThisIsSubWindow)
                orderedByFileName = imageFileList2.finfoList.OrderBy(file => file.fname).ToList();
            else
                orderedByFileName = imageFileList1.finfoList.OrderBy(file => file.fname).ToList();
            dgvFileInfo.DataSource = orderedByFileName;
            AutoSizeDGVColumns(); //reset
            tbCount.Text = dgvFileInfo.RowCount.ToString();
        }
        public void createFileListDataGrid()
        {
            dgvFileInfo.Columns[0].Width = 400;
            dgvFileInfo.Columns[1].Width = 500;
            dgvFileInfo.Columns[2].Width = 100;
            dgvFileInfo.Columns[3].Width = 100;
            dgvFileInfo.Columns[4].Width = 1200;
            dgvFileInfo.Columns[7].Width = 400;
        }
        public void resizeFileListDataGrid()
        {
            dgvFileInfo.Columns[0].Width = 400;
            dgvFileInfo.Columns[1].Width = 350;
            dgvFileInfo.Columns[2].Width = 50;
            dgvFileInfo.Columns[3].Width = 100;
            dgvFileInfo.Columns[4].Width = 50;
            dgvFileInfo.Columns[5].Width = 30;
            dgvFileInfo.Columns[6].Width = 30;
            dgvFileInfo.Columns[7].Width = 200;
            setReadOnly();
        }
        public void setReadOnly()
        {
            foreach (DataGridViewColumn dc in dgvFileInfo.Columns)
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
                if (cbFocusHere.Checked) this.Focus();
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
            if (!tbPlayStateStatus.Text.Equals("ENDING"))
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
                if (!getMoviePlayerState().Contains("Play"))
                {
                    vlcPlayer.Play();
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
                    vlcPlayer.skipForwardPercent((int)numSampleCount.Value);
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
                                    if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
                //dmc26// if (messageCount < numSampleCount.Value && !bPlayingEnding)
                // btGoToEnd15.BackColor = Color.LightGray;
                // else 
                if (!bPlayingEnding)
                {
                    GoToEnd(endPosition);
                    //btGoToEnd15.BackColor = Color.LightBlue;
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

                        if (vlcPlayer == null || vlcPlayer.IsDisposed)
                        {
                            if (mpVersion1)
                                vlcPlayer = new Player(gv, this);
                            else
                                vlcPlayer2 = new Player(gv, this);
                        }
                        else
                        {
                            //   mp.loadMovie("");
                            //MessageBox.Show("ERROR ProcessCmdKey cbCatalog");
                        }
                        bool rcc = false;
                        if (_useHTML)
                        {
                            rcc = copyFileHTML(cData);
                        }
                        else
                        {
                            rcc = copyFile(cData);  //ProcessCmdKey
                        }
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
                    if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
                    dgvFileInfo.Rows[rowIdMovieList].Cells["rating"].Value = cData;
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
                    if (_usePictures)
                    {
                        GoToPreviousRow();
                        return true;
                    }
                    SkipBack();
                }
                else if (keyData == Keys.F3)
                {

                    if (_usePictures)
                    {
                        GoToNextRow();
                        return true;
                    }
                    SkipForward1min();
                }
                else if (keyData == Keys.F4)
                {
                    //cbPause2.Checked = false;
                    if (_usePictures)
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
                    GoToPreviousRow();
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
                        tbFoundSeachInWin3.Text = "search2";
                        tbFoundSeachInWin3.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        tbFoundSeachInWin3.Text = "search";
                        tbFoundSeachInWin3.BackColor = Color.LightCoral;
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
                        if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
                        if (rowIdMovieList < dgvFileInfo.RowCount - 1)
                        {
                            dgvFileInfo.Rows[rowIdMovieList].Cells["bInvalid"].Value = true;
                        }
                        return true;
                    }
                }

            }
            if (cbAllowMovieSelection.Checked && vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                if (keyData == Keys.OemMinus)
                    vlcPlayer.SkipBackwards();
                else if (keyData == Keys.Left)
                    vlcPlayer.SkipBackward();
                else if (keyData == Keys.Right)
                    vlcPlayer.SkipForward(10);
                else if (keyData == Keys.Divide)
                    vlcPlayer.ScanForward();
                else if (keyData == Keys.Escape)
                    System.Windows.Forms.Application.Exit();
                // else if (keyData == Keys.Down)
                //playNextMovie(true); //key DOWN
                else if (keyData == Keys.PageDown)
                    vlcPlayer.SkipForwardBig();
                else if (keyData == Keys.PageUp)
                    vlcPlayer.SkipBackwards3();
                else if (keyData == Keys.Home)
                    restart();
                else if (keyData == Keys.Insert)
                    vlcPlayer.ToggleMute(false);
                else if (keyData == Keys.Add)
                    vlcPlayer.SetNormalSpeed();
                else if (keyData == Keys.Delete)
                {
                    vlcPlayer.ToggleMute(true);
                    browser();
                }
                else if (keyData == Keys.Back)
                    vlcPlayer.SkipBackwards();
                else if (keyData == Keys.Subtract)
                    vlcPlayer.SlowMotion(2);
                //else if (keyData == Keys.Up)
                //    mp.pause();
                else if (keyData == Keys.Divide)
                    vlcPlayer.SlowMotion(5);
                else if (keyData == Keys.Pause)
                    vlcPlayer.Pause();
                else if (keyData == Keys.End)
                {
                    GoToEnd(cbShortEndSample.Checked ? 5 : 15);
                }
                else if (keyData == Keys.Oemtilde)
                {
                    vlcPlayer.Stop();
                    this.TopMost = false;
                }
                else
                    return false;
                getMovieStatus();
                return true;
            }
            if (cbFocusHere.Checked) this.Focus();
            return rc;
        }

        public void SkipBackupLarger()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.SkipBackwards3();   // 30 sec
                getMovieStatus();
            }
        }

        public void MarkAsInvalid()
        {
            if (rowIdMovieList < dgvFileInfo.RowCount - 1)
            {
                dgvFileInfo.Rows[rowIdMovieList].Cells["bInvalid"].Value = true;
                // dgvFileList1.Rows[rowIdMovieList].Cells["rating"].Value = tbPosition3.Text;
                dgvFileInfo.Rows[rowIdMovieList].Cells["source"].Value = tbPosition3.Text;
            }

        }
        public void GoToPosition(int seconds)
        {
            System.TimeSpan ts = TimeSpan.FromSeconds(seconds);
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.SeekTo(ts);
        }
        public void GoToEnd(int secondsRemaining)
        {
            if (!bPlayingEnding)
                vlcPlayer.gotoEnd(secondsRemaining);
            bPlayingEnding = true;
        }
        public void browser()
        {
            //System.Diagnostics.Process.Start("http://www.microsoft.com/");
        }
        public void restart()
        {
            resetMpPlay();
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
            {
                if (mpVersion1)
                    vlcPlayer = new Player(gv, this);
                else
                    vlcPlayer2 = new Player(gv, this);
                vlcPlayer.SeekTo(TimeSpan.Zero);
                vlcPlayer.Activate();
                vlcPlayer.Show();
                // gv.SetMovies(mp);
            }
            else
            {
                if (CheckFileTypeWEBM(fpath))
                    return;

                vlcPlayer.Restart();
                vlcPlayer.LoadMedia(tbMovieFpath.Text, true); //in restart
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

            if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
            if (rowIdMovieList < dgvFileInfo.RowCount - 1)
            {
                //dgv1.ClearSelection();
                //++rowIdMovieList;
                dgvFileInfo.CurrentCell = dgvFileInfo.Rows[rowIdMovieList].Cells[0];
                dgvFileInfo.Rows[rowIdMovieList].Selected = true;
                //return rowIdMovieList.ToString();
            }
            return tbMovieFpath.Text;
        }
        public void front()
        {
            if (cbFocusHere.Checked) this.Focus();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void ShowMediaForm()
        {
            // Example inside TraverserDialog.OpenLibVLCPlayer or wherever you create the player:
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
            {
                vlcPlayer = new Player(gv, this);
                SetSmallPlayback();
            }
            vlcPlayer.Show();                 // <-- make it visible
            vlcPlayer.BringToFront();         // optional if it might be behind other windows
        }


        public void OpenLibVLCPlayer(string mediaPath = null, bool bringToFront = true)
        {
            // Default to currently selected movie path when none provided
            if (string.IsNullOrWhiteSpace(mediaPath))
                mediaPath = tbMovieFpath.Text;

            // Keep same pre-playback state as other players
            resetMpPlay();

            // Early exit for WEBM - handled by external browser in this app
            if (CheckFileTypeWEBM(mediaPath))
                return;

            try
            {
                // Create player if we don't have one or it was disposed
                if (vlcPlayer == null || vlcPlayer.IsDisposed)
                {
                    try
                    {
                        // Preferred ctor that accepts GlobalVars and parent
                        vlcPlayer = new Player(gv, this, mediaPath);
                    }
                    catch (LibVLCSharp.Shared.VLCException)
                    {
                        // Native runtime might be missing; attempt a parameterless fallback
                        try
                        {
                            vlcPlayer = (Player)Activator.CreateInstance(typeof(Player))!;
                        }
                        catch
                        {
                            MessageBox.Show(
                                "Unable to create LibVLC player. Ensure native LibVLC is available for win-x64.\n\n" +
                                "Add the VideoLAN.LibVLC.Windows NuGet package or install matching VLC and rebuild.",
                                "VLC Initialization Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Final fallback if something unexpected happens during construction
                        MessageBox.Show($"Failed to initialize LibVLC player:\n{ex.Message}", "VLC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Bring up the player window and configure common flags if methods exist
                    TryInvokeIfExists(vlcPlayer, "Show");
                    TryInvokeIfExists(vlcPlayer, "Activate");
                    TryInvokeIfExists(vlcPlayer, "SetCallParent", true);
                    TryInvokeIfExists(vlcPlayer, "ToggleMute", cbMute.Checked);
                    // TryInvokeIfExists(vlcPlayer, "zoomFullScreen", cbZoom.Checked);

                    // If the constructed player supports loading media, request load/play
                    TryInvokeIfExists(vlcPlayer, "LoadMedia", mediaPath, true);
                }
                else
                {
                    // Reuse existing instance: load media and optionally bring to front
                    TryInvokeIfExists(vlcPlayer, "LoadMedia", mediaPath, true);
                    if (bringToFront)
                    {
                        TryInvokeIfExists(vlcPlayer, "Show");
                        TryInvokeIfExists(vlcPlayer, "Activate");
                    }
                    // TryInvokeIfExists(vlcPlayer, "zoomFullScreen", cbZoom.Checked);
                    TryInvokeIfExists(vlcPlayer, "ToggleMute", cbMute.Checked);
                }

                // Keep UI in sync
                bPlayedFirstVideo = true;
                DisplayMovieResolution();
                resetTimerRequestToMovies();
                resetMessageToZero();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"VLC player failed to open:\n{ex.Message}", "VLC Playback Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        string? folderName;
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
        // Add this helper inside DialogTraverser class
        private void ResizeDgv1ToFillForm()
        {
            if (dgvFileInfo == null)
                return;

            const int leftMargin = 0;   // flush to left edge
            const int rightMargin = 0;  // flush to right edge
            const int bottomMargin = 0; // flush to bottom edge

            int top = dgvFileInfo.Top; // keep current top position (below your other controls)

            int newWidth = Math.Max(100, ClientSize.Width - leftMargin - rightMargin);
            int newHeight = Math.Max(100, ClientSize.Height - top - bottomMargin);

            dgvFileInfo.Location = new Point(leftMargin, top);
            dgvFileInfo.Size = new Size(newWidth, newHeight);
            dgvFileInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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
        private void buttonClose_Click(object sender, EventArgs e)  //then TraverserDialog_FormClosing
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
                if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                    vlcPlayer.Close();
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
            this.Text = ".... canceled operation ... ";

            if (action == TraversalActionState.TRAVERSING)
            {
                if (this.traverser != null)
                {
                    try { this.traverser.cancel(); } catch { /* swallow - UI initiated cancel */ }
                }
            }
            else if (action == TraversalActionState.LOADING_DISPLAY)
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
            
            if (gv.initParm1List.Count > 0)
            {
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
            }
            AddHistoryItem(fpath);
            main.SaveHistoryList(); // ← ADD THIS: persist the updated history to disk
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
            tbPlayStateStatus.Text = "ENDING";
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
            tbPlayStateStatus.Text = "Beginning";
        }
        public void stoppedStreamMessage()
        {
            tbPlayStateStatus.Text = "Stopped";
        }
        public void pausedStreamMessage()
        {
            tbPlayStateStatus.Text = "Paused";
        }
        public void playingStreamMessage()
        {
            tbPlayStateStatus.Text = "Playing";
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
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            try
            {
                // Accept both file:// URIs and regular file-system paths
                if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
                {
                    if (uri.IsFile)
                    {
                        // Uri.LocalPath handles percent-encoding and UNC correctly
                        string local = uri.LocalPath;
                        string full = Path.GetFullPath(local);
                        return full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
                    }

                    // If it's an absolute non-file URI, fall back to original string
                    // but attempt to normalize separators
                    path = uri.ToString();
                }

                // Treat as filesystem path (handles relative paths)
                string fullPath = Path.GetFullPath(path);
                return fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
            }
            catch
            {
                // Best-effort resilient fallback: normalize separators and trim
                try
                {
                    var safe = (path ?? string.Empty)
                        .Replace('/', Path.DirectorySeparatorChar)
                        .Replace('\\', Path.DirectorySeparatorChar)
                        .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    return safe.ToUpperInvariant();
                }
                catch
                {
                    return path ?? string.Empty;
                }
            }
        }
        public static string NormalizePathxxxx(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
        }


        // Call this method at the end of TraverseGo() for the main window, after doTraversal and before returning true
        public bool TraverseGo()
        {
            dgvFileInfo.DataSource = null;
            ResetSortButtonColor(null);
            tbDirectoryPath.Text = NormalizePath(tbDirectoryPath.Text);
            //rememberLastFolders(tbDirectoryPath.Text);

            if (!Directory.Exists(tbDirectoryPath.Text))
            {
                return false;
            }
            bAllowMovieSelection = false;
            gv.debug.w("traverse");
            setLoadedMovieTrue();
            doTraversal(tbDirectoryPath.Text, bThisIsSubWindow);


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

            PathHelpers.NormalizeFolderHistoryPaths(gv);
            //gv.folderHistoryList  was updated
            // Find existing entry
            var existing = gv.folderHistoryList
                .FirstOrDefault(f => f.folderPath.Equals(path, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                // Update the datetime for existing entry
                existing.lastAccessDate = DateTime.Now;
                return;
            }

            // Add new entry
            FolderHistory fpath = new FolderHistory();
            fpath.folderPath = path;
            fpath.lastAccessDate = DateTime.Now;
            gv.folderHistoryList.Add(fpath);
        }
        
        bool bAllowMovieSelection = false;
        public void TraverseFolder(string dpath) //NOT USED
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
                    doTraversal(dpath, bThisIsSubWindow);
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

        private bool doTraversal(string dpath, bool bSubWindow)
        {
            if (string.IsNullOrEmpty(dpath))
            {
                MessageBox.Show($"fpath fail {dpath}", "ERROR fpath");
                return false;
            }

            if (!bSubWindow)
            {
                btImageListFile.BackColor = Color.LightGreen;
                btImageListFile.Text = "imageFile 1";
            }
            else
            {
                btImageListFile.BackColor = Color.LightCoral;
                btImageListFile.Text = "imageFile 2";
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
            //rememberLastFolders(dpath);
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
            action = TraversalActionState.TRAVERSING;
            //TraverserBG traverser = new TraverserBG(gv, this);

            // Use the implementation selected by `usingBGnumber`      //dmc26
            //EnsureTraverserInstance();


            //uses gv.imageFileList
            if (!bThisIsSubWindow) // main window
            {
                if (gv.bImageFileList1Loaded)
                {
                    //dmcfixthis26
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
            ARGS args = BuildTraversalArgsFromUi();

            // Add this debug check:
            if (args == null)
            {
                MessageBox.Show("BuildTraversalArgsFromUi returned null!");
                return false;
            }


            args.dirpath = dpath;
            // DisplayTraversalArgs(args);

            //CORRECT 26
            var traverser = new TraverserBG5(gv, this);
            traverser.StartTraversal(args, bThisIsSubWindow, dpath);

            rememberLastFolders(dpath);
            //saveFolderName();
            return true;
        }
        public int UpdateMetadatVersionToUse = 2; //1=old, 2=new
        private async Task UpdateMetadataAfterScanAsync_Fast()
        {
            btTraversing.Visible = true;
            tbStatus.Text = "Updating metadata...";
            tbExt.Text = "Starting...";
            this.Refresh();

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var fileList = bThisIsSubWindow ? gv.imageFileList2?.finfoList : gv.imageFileList1?.finfoList;
            if (fileList == null || fileList.Count == 0)
            {
                tbStatus.Text = "No files to update.";
                btTraversing.Visible = false;
                return;
            }

            int total = fileList.Count;

            int processed = 0, updated = 0, alreadyLoaded = 0;
            int lastReported = 0;

            dgvFileInfo.SuspendLayout();
            var oldDataSource = dgvFileInfo.DataSource;
            dgvFileInfo.DataSource = null;

            using var uiTimer = new System.Windows.Forms.Timer { Interval = 500 };
            uiTimer.Tick += (_, __) =>
            {
                int p = Volatile.Read(ref processed);
                if (p - lastReported >= 5000 || p == total)
                {
                    lastReported = p;
                    double pct = (p / (double)total) * 100.0;
                    tbStatus.Text = $"Metadata: {p:N0}/{total:N0} ({pct:F1}%)  Updated={Volatile.Read(ref updated):N0}  Loaded={Volatile.Read(ref alreadyLoaded):N0}";
                    tbExt.Text = $"{sw.Elapsed.TotalSeconds:F0}s";
                }
            };
            uiTimer.Start();

            try
            {
                // External HDD sweet spot: 2–4
                int maxDegree = Math.Clamp(Environment.ProcessorCount / 2, 2, 4);
                var opts = new ParallelOptions { MaxDegreeOfParallelism = maxDegree };

                await Task.Run(() =>
                {
                    Parallel.ForEach(
                        fileList,
                        opts,
                        () => (proc: 0, upd: 0, loaded: 0),
                        (item, state, local) =>
                        {
                            local.proc++;

                            if (item == null) return local;

                            var fpath = item.fpath;
                            if (string.IsNullOrWhiteSpace(fpath)) return local;

                            // If you want “complete means skip”, expand this condition.
                            bool hasCoreMeta = item.len > 0 && !string.IsNullOrWhiteSpace(item.stimestamp);
                            if (hasCoreMeta)
                            {
                                local.loaded++;
                                return local;
                            }

                            try
                            {
                                var fi = new FileInfo(fpath);
                                fi.Refresh();
                                if (!fi.Exists) return local;

                                item.len = fi.Length;
                                item.stimestamp = fi.LastWriteTimeUtc.ToString("yyyy-MM-dd HH:mm:ss");

                                if (string.IsNullOrWhiteSpace(item.fname))
                                    item.fname = fi.Name;

                                if (string.IsNullOrWhiteSpace(item.ext))
                                    item.ext = fi.Extension.TrimStart('.', ' ');

                                if (string.IsNullOrWhiteSpace(item.dpath))
                                    item.dpath = fi.DirectoryName ?? "";

                                if (string.IsNullOrWhiteSpace(item.keyPath))
                                    item.keyPath = NormalizeFullPathForDb(fpath);

                                local.upd++;
                            }
                            catch
                            {
                                // swallow inaccessible files
                            }

                            return local;
                        },
                        local =>
                        {
                            Interlocked.Add(ref processed, local.proc);
                            Interlocked.Add(ref updated, local.upd);
                            Interlocked.Add(ref alreadyLoaded, local.loaded);
                        });
                });
            }
            finally
            {
                uiTimer.Stop();

                dgvFileInfo.DataSource = oldDataSource;
                dgvFileInfo.ResumeLayout();

                if (bAutoSizeDGVcolumns)
                    AutoSizeDGVColumns();

                sw.Stop();
                tbStatus.Text = $"Finished: {processed:N0}/{total:N0}  Updated={updated:N0}  Loaded={alreadyLoaded:N0}";
                tbExt.Text = $"{sw.Elapsed.TotalSeconds:F1}s";
                btTraversing.Visible = false;
            }
        }
        bool bAutoSizeDGVcolumns = true;
        private async Task UpdateMetadataAfterScanAsync2()
        {
            btTraversing.Visible = true;
            tbStatus.Text = "Updating metadata...";
            tbExt.Text = "Starting...";
            this.Refresh();

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var fileList = bThisIsSubWindow ? gv.imageFileList2?.finfoList : gv.imageFileList1?.finfoList;
            if (fileList == null || fileList.Count == 0)
            {
                tbStatus.Text = "No files to update.";
                btTraversing.Visible = false;
                return;
            }

            int total = fileList.Count;
            int processed = 0, updated = 0, alreadyLoaded = 0;
            int _lastReported = 0;

            // Suspend DGV updates
            dgvFileInfo.SuspendLayout();
            var oldDataSource = dgvFileInfo.DataSource;
            dgvFileInfo.DataSource = null;

            // UI timer updates status (no BeginInvoke spam from worker threads)
            using var uiTimer = new System.Windows.Forms.Timer { Interval = 500 };
            uiTimer.Tick += (_, __) =>
            {
                int p = Volatile.Read(ref processed);
                if (p - _lastReported >= 5000 || p == total)
                {
                    _lastReported = p;
                    double pct = (p / (double)total) * 100.0;
                    tbStatus.Text = $"Metadata: {p:N0}/{total:N0} ({pct:F1}%)  Updated={updated:N0}  Loaded={alreadyLoaded:N0}";
                    tbExt.Text = $"{sw.Elapsed.TotalSeconds:F0}s";
                }
            };
            uiTimer.Start();

            try
            {
                // **KEY OPTIMIZATION 1**: Limit parallelism for disk I/O
                // Environment.ProcessorCount is too high for file operations
                int maxDegree = Math.Min(Environment.ProcessorCount / 2, 4);

                var opts = new ParallelOptions { MaxDegreeOfParallelism = maxDegree };

                await Task.Run(() =>
                {
                    Parallel.ForEach(fileList, opts, item =>
                    {
                        try
                        {
                            if (item == null) return;

                            var fpath = item.fpath;
                            if (string.IsNullOrWhiteSpace(fpath)) return;

                            // **KEY OPTIMIZATION 2**: Early exit with volatile read
                            if (item.len > 0 && !string.IsNullOrWhiteSpace(item.stimestamp))
                            {
                                Interlocked.Increment(ref alreadyLoaded);
                                Interlocked.Increment(ref processed);
                                return;
                            }

                            // **KEY OPTIMIZATION 3**: Check file existence before creating FileInfo
                            if (!File.Exists(fpath))
                            {
                                Interlocked.Increment(ref processed);
                                return;
                            }

                            // FileInfo constructor does syscall - keep minimal
                            var fi = new FileInfo(fpath);

                            // **KEY OPTIMIZATION 4**: Batch all updates together
                            item.len = fi.Length;
                            item.stimestamp = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

                            if (string.IsNullOrWhiteSpace(item.fname))
                                item.fname = fi.Name; // Use fi.Name instead of Path.GetFileName

                            if (string.IsNullOrWhiteSpace(item.ext))
                                item.ext = fi.Extension.TrimStart('.', ' ');

                            if (string.IsNullOrWhiteSpace(item.dpath))
                                item.dpath = fi.DirectoryName ?? "";

                            if (string.IsNullOrWhiteSpace(item.keyPath))
                                item.keyPath = NormalizeFullPathForDb(fpath);

                            Interlocked.Increment(ref updated);
                        }
                        catch
                        {
                            // swallow inaccessible files
                        }
                        finally
                        {
                            Interlocked.Increment(ref processed);
                        }
                    });
                });
            }
            finally
            {
                uiTimer.Stop();

                // Restore DGV binding once (one refresh)
                dgvFileInfo.DataSource = oldDataSource;
                dgvFileInfo.ResumeLayout();

                if (bAutoSizeDGVcolumns)
                    AutoSizeDGVColumns();

                sw.Stop();
                tbStatus.Text = $"Finished: {processed:N0}/{total:N0}  Updated={updated:N0}  Loaded={alreadyLoaded:N0}";
                tbExt.Text = $"{sw.Elapsed.TotalSeconds:F1}s";
                btTraversing.Visible = false;
            }
        }
        private async Task UpdateMetadataAfterScanAsync()  //old
        {
            btTraversing.Visible = true;
            tbStatus.Text = "Updating metadata...";
            tbExt.Text = "Starting...";
            this.Refresh();

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var fileList = bThisIsSubWindow ? gv.imageFileList2?.finfoList : gv.imageFileList1?.finfoList;
            if (fileList == null || fileList.Count == 0)
            {
                tbStatus.Text = "No files to update.";
                btTraversing.Visible = false;
                return;
            }

            int total = fileList.Count;
            int processed = 0, updated = 0, alreadyLoaded = 0;
            int _lastReported = 0;
            // Suspend DGV updates
            dgvFileInfo.SuspendLayout();
            var oldDataSource = dgvFileInfo.DataSource;
            dgvFileInfo.DataSource = null;

            // UI timer updates status (no BeginInvoke spam from worker threads)
            using var uiTimer = new System.Windows.Forms.Timer { Interval = 500 }; // 0.5s feels good
            uiTimer.Tick += (_, __) =>
            {
                int p = processed;
                if (p - _lastReported >= 5000 || p == total)
                {
                    _lastReported = p;
                    double pct = (p / (double)total) * 100.0;
                    tbStatus.Text = $"Metadata: {p:N0}/{total:N0} ({pct:F1}%)  Updated={updated:N0}  Loaded={alreadyLoaded:N0}";
                    tbExt.Text = $"{sw.Elapsed.TotalSeconds:F0}s";
                }

            };
            uiTimer.Start();

            try
            {
                await Task.Run(() =>
                {
                    var opts = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

                    Parallel.ForEach(fileList, opts, item =>
                    {
                        try
                        {
                            if (item == null) return;

                            var fpath = item.fpath;
                            if (string.IsNullOrWhiteSpace(fpath)) return;

                            // skip if already has metadata
                            if (item.len > 0 && !string.IsNullOrWhiteSpace(item.stimestamp))
                            {
                                Interlocked.Increment(ref alreadyLoaded);
                                return;
                            }

                            // FileInfo can be slow on some paths; keep it minimal
                            var fi = new FileInfo(fpath);
                            if (!fi.Exists) return;

                            item.len = fi.Length;
                            item.stimestamp = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

                            if (string.IsNullOrWhiteSpace(item.fname))
                                item.fname = Path.GetFileName(fpath);

                            if (string.IsNullOrWhiteSpace(item.ext) && !string.IsNullOrWhiteSpace(item.fname))
                                item.ext = Path.GetExtension(item.fname).TrimStart('.', ' ');

                            if (string.IsNullOrWhiteSpace(item.dpath))
                                item.dpath = Path.GetDirectoryName(fpath) ?? "";

                            if (string.IsNullOrWhiteSpace(item.keyPath))
                                item.keyPath = NormalizeFullPathForDb(fpath); // your key builder

                            Interlocked.Increment(ref updated);
                        }
                        catch
                        {
                            // swallow inaccessible files
                        }
                        finally
                        {
                            Interlocked.Increment(ref processed);
                        }
                    });
                });
            }
            finally
            {
                uiTimer.Stop();

                // Restore DGV binding once (one refresh)
                dgvFileInfo.DataSource = oldDataSource;
                dgvFileInfo.ResumeLayout();

                if (bAutoSizeDGVcolumns)
                    AutoSizeDGVColumns();

                sw.Stop();
                tbStatus.Text = $"Finished: {processed:N0}/{total:N0}  Updated={updated:N0}  Loaded={alreadyLoaded:N0}";
                tbExt.Text = $"{sw.Elapsed.TotalSeconds:F1}s";
                btTraversing.Visible = false;
            }
        }

        private async Task UpdateMetadataAfterScanAsyncwas()
        {
            btTraversing.Visible = true;
            tbStatus.Text = "Updating metadata...";
            tbExt.Text = "Starting...";
            Refresh();

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var fileList = bThisIsSubWindow
                ? gv.imageFileList2?.finfoList
                : gv.imageFileList1?.finfoList;

            if (fileList == null || fileList.Count == 0)
            {
                tbStatus.Text = "No files to update.";
                btTraversing.Visible = false;
                return;
            }

            int total = fileList.Count;
            int processed = 0;
            int updated = 0;
            int alreadyLoaded = 0;

            // ---- Suspend grid updates (your approach) ----
            dgvFileInfo.SuspendLayout();
            object oldDataSource = dgvFileInfo.DataSource;
            dgvFileInfo.DataSource = null;

            try
            {
                // NOTE: Don't use Environment.ProcessorCount here. That's usually too high for disk I/O.
                // Tune: SSD local -> 6-10, HDD/network -> 2-4
                int maxConc = Math.Clamp(Environment.ProcessorCount / 2, 2, 8);

                using var sem = new System.Threading.SemaphoreSlim(maxConc);

                // We’ll create tasks in a streaming way to avoid allocating 42k tasks at once.
                var tasks = new List<Task>(maxConc * 4);

                // UI progress throttling
                long lastUiTick = 0;

                foreach (var item in fileList)
                {
                    if (item == null) continue;

                    // Quick skip (no IO)
                    if (!string.IsNullOrWhiteSpace(item.stimestamp) && item.len > 0)
                    {
                        alreadyLoaded++;
                        continue;
                    }

                    await sem.WaitAsync().ConfigureAwait(false);

                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(item.fpath))
                                return;

                            // If file doesn't exist, skip quickly
                            // (File.Exists is a syscall but cheaper than throwing)
                            if (!File.Exists(item.fpath))
                                return;

                            // FileInfo is still the practical managed way to get Length.
                            // Keep it minimal.
                            var fi = new FileInfo(item.fpath);

                            // If race: file deleted after Exists
                            if (!fi.Exists)
                                return;

                            item.len = fi.Length;

                            // Use UTC to be consistent; format once.
                            // You can switch to local if you prefer.
                            var ts = fi.LastWriteTimeUtc;
                            item.stimestamp = ts.ToString("yyyy-MM-dd HH:mm:ss");

                            if (string.IsNullOrWhiteSpace(item.fname))
                                item.fname = fi.Name;

                            if (string.IsNullOrWhiteSpace(item.ext))
                                item.ext = fi.Extension.TrimStart('.', ' ');

                            if (string.IsNullOrWhiteSpace(item.dpath))
                                item.dpath = fi.DirectoryName ?? "";

                            // IMPORTANT: no case manipulation.
                            // Also use backslashes and remove drive letter per your new convention.
                            item.keyPath = NormalizeFullPathForDb(item.fpath);

                            System.Threading.Interlocked.Increment(ref updated);
                        }
                        catch
                        {
                            // ignore inaccessible/bad paths
                        }
                        finally
                        {
                            int p = System.Threading.Interlocked.Increment(ref processed);

                            // Throttle UI updates by time (~4 per second) or every 5000.
                            long now = Environment.TickCount64;
                            /*
                            if (p % 5000 == 0 || now - System.Threading.Interlocked.Read(ref lastUiTick) > 250)
                            {
                                System.Threading.Interlocked.Exchange(ref lastUiTick, now);

                                try
                                {
                                    if (p % 5000 == 0)
                                    {
                                        BeginInvoke(new Action(() =>
                                        {
                                            double pct = (p / (double)total) * 100.0;
                                            tbStatus.Text = $"Metadata: {p:N0}/{total:N0} ({pct:F1}%)  Updated={updated:N0}  Loaded={alreadyLoaded:N0}";
                                            tbExt.Text = $"{sw.Elapsed.TotalSeconds:F0}s";
                                        }));
                                    }
                                }
                                catch { }
                            }
                            */
                            sem.Release();
                        }
                    }));

                    // Keep task list bounded
                    if (tasks.Count >= maxConc * 8)
                    {
                        var done = await Task.WhenAny(tasks).ConfigureAwait(false);
                        tasks.Remove(done);
                    }
                }

                // Wait remaining work
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            finally
            {
                // Restore grid once
                dgvFileInfo.DataSource = oldDataSource;
                dgvFileInfo.ResumeLayout();

                if (bAutoSizeDGVcolumns)
                    AutoSizeDGVColumns();

                sw.Stop();
                tbStatus.Text = $"Metadata complete. Updated {updated:N0}, already had {alreadyLoaded:N0}.";
                tbExt.Text = $"Time: {sw.Elapsed.TotalSeconds:F1}s";
                btTraversing.Visible = false;
            }
            BeginInvoke(new Action(() =>
            {
                tbStatus.Text =
                    $"Metadata complete: {total:N0}/{total:N0} (100%)  Updated={updated:N0}  Loaded={alreadyLoaded:N0}";
                tbExt.Text = $"{sw.Elapsed.TotalSeconds:F0}s";
            }));
            tbStatus.Text = "Metadata update finished.";
            tbStatus.Hide();
        }


        //
        //
        // optimized
        //
        //
        /// <summary>
        /// Reads metadata from the file's alternate data stream.
        /// Returns true if metadata was found and loaded.
        /// </summary>
        private bool ReadMetadataFromFile(FileInfoItem item)
        {
            if (string.IsNullOrWhiteSpace(item.fpath))
                return false;

            try
            {
                string adsPath = $"{item.fpath}:metadata";

                if (!File.Exists(adsPath))
                    return false;

                string json = File.ReadAllText(adsPath);

                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Load metadata from JSON
                if (root.TryGetProperty("len", out var lenProp))
                    item.len = lenProp.GetInt64();

                if (root.TryGetProperty("stimestamp", out var tsProp))
                    item.stimestamp = tsProp.GetString();

                if (root.TryGetProperty("fname", out var fnameProp))
                    item.fname = fnameProp.GetString();

                if (root.TryGetProperty("ext", out var extProp))
                    item.ext = extProp.GetString();

                if (root.TryGetProperty("rating", out var ratingProp))
                {
                    string? rStr = ratingProp.GetString();
                    item.rating = !string.IsNullOrEmpty(rStr) ? rStr[0] : ' ';
                }

                if (root.TryGetProperty("comment", out var commentProp))
                    item.comment = commentProp.GetString();

                if (root.TryGetProperty("width", out var widthProp))
                    item.width = widthProp.GetInt32();

                if (root.TryGetProperty("height", out var heightProp))
                    item.height = heightProp.GetInt32();

                if (root.TryGetProperty("playTime", out var playTimeProp))
                    item.playTime = playTimeProp.GetDouble();

                if (root.TryGetProperty("minutes", out var minProp))
                    item.minutes = minProp.GetInt32();

                if (root.TryGetProperty("seconds", out var secProp))
                    item.seconds = secProp.GetInt32();

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Writes metadata to the file's alternate data stream (NTFS ADS).
        /// This stores metadata directly in the file without needing a database.
        /// </summary>
        private void WriteMetadataToFile(FileInfoItem item)
        {
            if (string.IsNullOrWhiteSpace(item.fpath))
                return;

            try
            {
                // Use alternate data stream (hidden from normal file operations)
                string adsPath = $"{item.fpath}:metadata";

                // Create JSON metadata
                var metadata = new
                {
                    len = item.len,
                    stimestamp = item.stimestamp,
                    fname = item.fname,
                    ext = item.ext,
                    dpath = item.dpath,
                    rating = item.rating,
                    comment = item.comment,
                    width = item.width,
                    height = item.height,
                    playTime = item.playTime,
                    minutes = item.minutes,
                    seconds = item.seconds
                };

                string json = System.Text.Json.JsonSerializer.Serialize(metadata);

                // Write to alternate data stream
                File.WriteAllText(adsPath, json);
            }
            catch
            {
                // Silently fail - file might be read-only or on non-NTFS drive
            }
        }

        //from TraverserBG
        public void DisplayTraversalArgs(string[] args)
        {
            tbStatus.Text = string.Join(Environment.NewLine, args);
        }


        //
        //
        //  TraverserBG5  communication methods // progress, cancel, completed <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        public void OnCancel(string stat)
        {
            //backgroundWorker1.CancelAsync();
            btTraversing.Visible = false;
            ShowTransientMessage("Cancelled", 3000);
            action = TraversalActionState.NONE;
        }
        public void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 99)
                progressFactor += 1;
            this.progressBar1.Value = e.ProgressPercentage / progressFactor;

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
                action = TraversalActionState.NONE;
                btTraversing.Hide();
            }
            else
            {
                ////  main.directoryPath(tbDirectoryPath.Text + " " + gv.imageFileList.getIndexed(0));
                if (bThisIsSubWindow)
                    gv.mainWindow.setTitle(gv.imageFileList2.getIndexed(0), tbDirectoryPath.Text);
                else
                    gv.mainWindow.setTitle(gv.imageFileList1.getIndexed(0), tbDirectoryPath.Text);

                action = TraversalActionState.COMPLETED;

                // ... existing code ...

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

                // ADDED: Track traversal type and reset metadata flag
                _metadataUpdatedAfterTraversal = false;
                if (_useMovie)
                    _lastTraversalType = "videos";
                else if (_usePictures || _useHTML)
                    _lastTraversalType = "images";
                else
                    _lastTraversalType = "other";

                if (bThisIsSubWindow)
                {
                    imageFileListOriginal2 = gv.imageFileList2;
                    imageFileList2 = gv.imageFileList2;
                }
                else
                {
                    imageFileListOriginal1 = gv.imageFileList1;
                    imageFileList1 = gv.imageFileList1;
                }

                this.progressBar1.Value = 100;

                Cursor.Current = Cursors.Default;

                dgvFileInfo.SuspendLayout();
                dgvFileInfo.DataSource = null;
                if (bThisIsSubWindow)
                    dgvFileInfo.DataSource = gv.imageFileList2?.finfoList;
                else
                    dgvFileInfo.DataSource = gv.imageFileList1?.finfoList;
                dgvFileInfo.ResumeLayout();

                previousRowIdWas = -1;
                dgv1_SelectionChanged(null, null);

                btUpdateMetadata.Enabled = true;
            }
        }
        //
        //
        //
        public void BeginBatchUpdateDGV1()
        {
            dgvFileInfo.SuspendLayout();
            dgvFileInfo.Visible = false;   // even faster
        }

        public void EndBatchUpdateDGV1()
        {
            dgvFileInfo.Visible = true;
            dgvFileInfo.ResumeLayout();
        }
        //  public void BeginBatchUpdate() => listView.BeginUpdate();
        //   public void EndBatchUpdate() => listView.EndUpdate();

        //
        //
        //

        private void ShowTransientMessage(string message, int milliseconds = 3000)
        {
            // lightweight borderless form used as a transient message/toast
            var toast = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                BackColor = Color.FromArgb(255, 255, 225),
                Size = new Size(460, 160)
            };

            var lbl = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = message,
                Font = new Font(this.Font.FontFamily, 10F, FontStyle.Bold),
                ForeColor = Color.Black
            };

            toast.Controls.Add(lbl);

            // center toast over parent window (falls back to screen if needed)
            try
            {
                var parentRect = this.Bounds;
                //var location = this.PointToScreen(new Point((parentRect.Width - toast.Width) / 2, (parentRect.Height - toast.Height) / 2));
                var location = this.PointToScreen(new Point(parentRect.X, parentRect.Y));
                toast.Location = location;
            }
            catch
            {
                toast.StartPosition = FormStartPosition.CenterScreen;
            }

            var timer = new System.Windows.Forms.Timer { Interval = Math.Max(100, milliseconds) };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                try { toast.Close(); toast.Dispose(); } catch { }
            };

            toast.Show();
            timer.Start();
        }
        int progressFactor = 1;


        private void FormTraverser2_Resize(object sender, EventArgs e)
        {
            ResizeDgv1ToFillForm();
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
            // lvwColumnSorter.Order = SortOrder.None;
        }



        Boolean insertRangeB = true;

        private void btDisplayListwas_Click(object sender, EventArgs e)
        {
            gv.setCursorHourGlass();
        }


        private void ScrollToMiddle(DataGridView dgv)
        {
            if (dgv.CurrentRow == null) return;

            int rowIndex = dgv.CurrentRow.Index;
            int visibleRows = dgv.DisplayedRowCount(false);

            // Calculate the first row to display so that the current row is roughly centered
            int firstRow = rowIndex - (visibleRows / 2);

            if (firstRow < 0)
                firstRow = 0;

            dgv.FirstDisplayedScrollingRowIndex = firstRow;
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
            // The path is already stored in gv.lastLoadedImageFile (line 3104)
            // No need for additional settings persistence here
            // *** SAVE TO APPLICATION SETTINGS ***
            // Properties.Settings.lastLoadedImageFileList = imageFileName;
            // Properties.Settings.Save();
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
            fileType = "images";
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
                tbCount.Text = gv.slideCount1.ToString();
                //imageFileList2 = imageFileList1;
                gv.lastLoadedImageFile = openFileDialog1.FileName;

                // *** SAVE TO APPLICATION SETTINGS ***
                // The path is already stored in gv.lastLoadedImageFile (line 3104)
                // No need for additional settings persistence here
                //Properties.Settings.Default.lastLoadedImageFileList = openFileDialog1.FileName;
                //Properties.Settings.Default.Save();

                //this.loadListViewRange();
                //SortName();
                //keep whatever order is present in the file 
                bSavedFile = true;
                dgvFileInfo.DataSource = imageFileList1.finfoList;
                //dmc26 AutoSizeDGVColumns(); // load imagefile1
            }
            gv.setCursorDefault();
            //formatDataGridViewFileList();
        }

        public void LoadImageListFile2()
        {
            InitializeOpenFileDialogXML();
            cbAutoAdv.Enabled = true;
            cbCatalog.Enabled = true;
            bLoadedList = true;
            //2022
            gv.imageFileList2 = new ImageFileList();
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (imageFileList2 == null)
                    imageFileList2 = new ImageFileList();
                gv.setCursorHourGlass();
                //
                imageFileList2.finfoList = DeserializeFromXML(openFileDialog1.FileName);
                //
                gv.imageFileList2.finfoList = imageFileList2.finfoList;
                imageFileName = openFileDialog1.FileName;
                tbFileName.Text = openFileDialog1.FileName;
                gv.nextIdx = 0;
                gv.bImageFileList1Loaded = true;
                gv.slideCount2 = imageFileList2.getImageFileListLength();
                tbCount.Text = gv.slideCount2.ToString();
                // imageFileList2 = imageFileList2;
                //this.loadListViewRange();
                //SortName();
                bSavedFile = true;
                dgvFileInfo.DataSource = imageFileList2.finfoList;
                //dmc26 AutoSizeDGVColumns();//load imagefile2
            }
            gv.setCursorDefault();
            //formatDataGridViewFileList();
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
                //startFolder
                dpath = gv.dirDialog(InitFolder.MyComputer, null, tbDirectoryPath.Text);// (startFolder may have to be set to Special or Desktop??
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
        private void tbDirectoryPath2_TextChanged(object sender, EventArgs e)
        {
            // put a breakpoint on the next line
            var newValue = tbDirectoryPath.Text;
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
            // put a breakpoint on the next line
            var newValue = tbDirectoryPath.Text;
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
        public bool bUsingLibVLCPlayer = true;



        public bool mpActive()
        {
            return (vlcPlayer != null && !vlcPlayer.IsDisposed);
        }

        int durationOfSampleSeconds = 3;
        public void PlayButtonGo()
        {
            tbCountM.Text = "1";
            messageCount = 1;
            counttest = 0;
            if (vlcPlayer != null && vlcPlayer.IsDisposed)
                vlcPlayer.Close();
            ClearError();
            if (cbFilesAll.Checked)
                return;
            playFirstVideo();
            if (cbLongerSegments.Checked)
                vlcPlayer.SetSlowerTimer();
            if (cbHidePlayer.Checked)
                vlcPlayer.Hide();
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
            _isSlowMode = false;   // was: cbSlow.Checked = false
            _isSlowMode2 = false;   // was: cbSlower2.Checked = false
            _isPaused = false;   // was: cbPause2.Checked = false
            resetingCbSlow = false;
            bPlayingEnding = false;
            // btGoToEnd15.BackColor removed (control deleted)
        }
        public void Resume()
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
            {
                if (mpVersion1)
                    vlcPlayer = new Player(gv, this, tbMovieFpath.Text);
                vlcPlayer.Activate();
                vlcPlayer.Show();
                //gv.SetMovies(vlcPlayer);
            }
        }
        public void MovieWindowCreated()
        {
            cbReceivedFirstMessage.Checked = false;
        }

        /// <summary>
        /// Try to invoke a method on an object by name if it exists (silent on failure).
        /// Used to call common player API methods without hard compile dependency on exact signatures.
        /// </summary>
        private void TryInvokeIfExists(object? target, string methodName, params object[] args)
        {
            if (target == null) return;
            try
            {
                var mi = target.GetType().GetMethod(methodName,
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (mi != null)
                    mi.Invoke(target, args);
            }
            catch
            {
                // ignore reflection invocation errors
            }
        }

        /// <summary>
        /// Play first video using LibVLCPlayer. Mirrors the behavior of playFirstVideo() but targets VLC player.
        /// New VLC-specific helpers/methods are invoked by name when present on the LibVLCPlayer instance.
        /// </summary>
        public void playFirstVideoVLC()
        {
            // reset same playback state as the WmPlayer path
            resetMpPlay();

            if (string.IsNullOrWhiteSpace(tbMovieFpath.Text))
            {
                MessageBox.Show("No media path provided.", "VLC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // normalize path and use file:// URI
            string filePath = tbMovieFpath.Text.Trim();
            if (!File.Exists(filePath))
            {
                // try to expand relative path
                try { filePath = Path.GetFullPath(filePath); } catch { }
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"Media file not found:\n{tbMovieFpath.Text}", "VLC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string mediaUri = new Uri(filePath).AbsoluteUri; // file:///C:/...

            // handle webm special-case (existing behavior)
            if (CheckFileTypeWEBM(mediaUri))
                return;

            bPlayedFirstVideo = true;

            try
            {
                // create instance if required
                if (vlcPlayer == null || vlcPlayer.IsDisposed)
                {
                    try
                    {
                        vlcPlayer = new Player(gv, this, mediaUri);
                    }
                    catch
                    {
                        // fallback to parameterless ctor
                        try { vlcPlayer = (Player)Activator.CreateInstance(typeof(Player))!; } catch { vlcPlayer = null; }
                    }

                    TryInvokeIfExists(vlcPlayer, "Show");
                    TryInvokeIfExists(vlcPlayer, "Activate");
                }

                // Prefer direct call to the known API
                try
                {
                    // LibVLCPlayer.LoadMedia accepts (string path, bool autoPlay = false)
                    //vlcPlayer?.LoadMedia(mediaUri, true);
                    vlcPlayer?.LoadMedia(filePath, true);
                }
                catch
                {
                    // reflection fallback: try common method names and then Play()
                    TryInvokeIfExists(vlcPlayer, "LoadMedia", mediaUri, true);
                    TryInvokeIfExists(vlcPlayer, "loadMovie", mediaUri, cbZoom.Checked);
                    TryInvokeIfExists(vlcPlayer, "Load", mediaUri);
                }

                // Ensure visible / parent callbacks
                TryInvokeIfExists(vlcPlayer, "SetCallParent", true);
                TryInvokeIfExists(vlcPlayer, "zoomFullScreen", cbZoom.Checked);

                // If LoadMedia didn't auto-start, try Play
                TryInvokeIfExists(vlcPlayer, "Play");
                TryInvokeIfExists(vlcPlayer, "play");

                // Update UI/resolution similar to the WmPlayer path
                DisplayMovieResolution();
                if (cbBeep.Checked) cbFocusHere.Checked = true;
                resetTimerRequestToMovies();
                resetMessageToZero();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"VLC player failed to start:\n{ex.Message}", "VLC Playback Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void CheckLibVLCRuntimeAndLogMethods()
        {
            // 1) Try to initialize LibVLCSharp core (safe to call multiple times)
            try
            {
                Core.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"LibVLCSharp Core.Initialize() failed:\n{ex.Message}", "LibVLC Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Core.Initialize() failed: {ex}");
                return;
            }

            // 2) Try to create a minimal LibVLC instance - this verifies native libs are loadable
            try
            {
                using var lib = new LibVLC(new string[] { "--verbose=2" });
                Debug.WriteLine("LibVLC native runtime initialized successfully.");
                // MessageBox.Show("LibVLC native runtime initialized successfully.", "LibVLC Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create LibVLC instance (native runtime missing or mismatched bitness):\n{ex.Message}",
                                "LibVLC Native Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"LibVLC creation failed: {ex}");
                return;
            }

            // 3) Inspect the runtime type/constructors/methods of the LibVLCPlayer instance (if any)
            try
            {
                var sb = new StringBuilder();
                if (vlcPlayer == null)
                {
                    sb.AppendLine("vlcPlayer is null.");
                }
                else
                {
                    var t = vlcPlayer.GetType();
                    sb.AppendLine($"vlcPlayer Type: {t.FullName}");
                    sb.AppendLine("Constructors:");
                    foreach (var c in t.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                        sb.AppendLine("  " + c.ToString());

                    sb.AppendLine();
                    sb.AppendLine("Public & non-public methods (name + signature) - truncated to 200 entries:");
                    var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                   .OrderBy(m => m.Name)
                                   .Select(m => m.ToString())
                                   .Take(200);
                    foreach (var m in methods)
                        sb.AppendLine("  " + m);
                }

                // write to temp file for easier inspection
                string outFile = Path.Combine(Path.GetTempPath(), "vlcPlayer_methods.txt");
                File.WriteAllText(outFile, sb.ToString());
                //MessageBox.Show($"LibVLC check complete. Method/ctor list written to:\n{outFile}", "LibVLC Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Debug.WriteLine(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while introspecting vlcPlayer: {ex.Message}", "LibVLC Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Introspection failed: {ex}");
            }
        }
        bool bUseVLC = true;
        public void playFirstVideo()
        {
            if (CheckFileTypeWEBM(tbMovieFpath.Text))
                return;
            ShowMediaForm();
            if (!bPlayedFirstVideo)
                vlcPlayer.SetSmallScreen();

            resetMpPlay();
            playFirstVideoVLC();
            bPlayedFirstVideo = true;
            // gv.SetMovies(mp);
            if (cbMute.Checked)
                vlcPlayer.ToggleMute(cbMute.Checked);

            try
            {
                LoadMovie(tbMovieFpath.Text, cbZoom.Checked); // playFirstVideo
                TrackFromSourceLoad("playFirstVideo");

            }
            catch
            {
                MessageBox.Show("TEST");
            }

            DisplayMovieResolution();
            if (cbBeep.Checked)
            {
                cbFocusHere.Checked = true;
                //cbZoom.Checked = true;
            }
            //vlcPlayer.SetFullScreen(true);
            resetTimerRequestToMovies(); // send after mp.loadMovie
            resetMessageToZero();
            vlcPlayer.SetCallParent(true);
        }
        //bool gv.bUseOverlay = true;
        public TransparentForm formTransparent;

        public void SetTransparentForm(TransparentForm tf)
        {
            formTransparent = tf;
        }
        public void CloseTransparentWin()
        {
            if (formTransparent != null && !formTransparent.IsDisposed)
            {
                formTransparent.Close();
                formTransparent.Dispose();
                formTransparent = null;
            }
            gv.bUseOverlay = false;
        }
        public void OpenTransparentWin()
        {
            if (!gv.bUseOverlay)
                return;

            // Default placement: slightly inset from the vlcPlayer window
            Point startPosition = new Point(0, 0);
            if (vlcPlayer != null)
                startPosition = vlcPlayer.Location;
            if (startPosition.X > 5) startPosition.X -= 5;
            if (startPosition.Y > 5) startPosition.Y -= 5;

            // Create the overlay form if it doesn't exist yet
            if (formTransparent == null || formTransparent.IsDisposed)
                formTransparent = new TransparentForm(gv, startPosition);

            // ── PlaybackControlForm placement ─────────────────────────────────
            // When the PlaybackControlForm is visible, dock formTransparent
            // directly above it with left edges aligned.
            // If there is not enough vertical room above it, push the
            // PlaybackControlForm down until there is.
            if (_playbackControlForm != null &&
                !_playbackControlForm.IsDisposed &&
                _playbackControlForm.Visible)
            {
                int overlayH = formTransparent.Height;

                if (overlayH > 0)   // guard against un-laid-out form
                {
                    int pcfLeft = _playbackControlForm.Left;
                    int pcfTop = _playbackControlForm.Top;

                    // Not enough room above? slide PlaybackControlForm down
                    if (pcfTop < overlayH)
                    {
                        _playbackControlForm.Top = overlayH;
                        pcfTop = overlayH;
                    }

                    // Left-align overlay with PlaybackControlForm, flush above it
                    formTransparent.Location = new Point(pcfLeft, pcfTop - overlayH);
                }
            }
            // ─────────────────────────────────────────────────────────────────

            formTransparent.Activate();
            formTransparent.Show();
            formTransparent.TopMost = true;
            formTransparent.UpdateFilename(tbMovieFpath.Text);
            gv.bUseOverlay = true;
        }
        public void OpenOverlay()
        {
            if (formTransparent == null || formTransparent.IsDisposed)
            {
                if (vlcPlayer == null)
                    formTransparent = new TransparentForm(gv, this.Location);
                else
                    formTransparent = new TransparentForm(gv, vlcPlayer.Location);
            }
            formTransparent.Activate();
            formTransparent.Show();
            gv.bUseOverlay = true;
        }
        public void SetOverlay(bool bOn)
        {
            cbOverLayTransparentWin.Checked = bOn;
        }
        public void UpdateFinfoMovieResolution(FileInfoItem fi)
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            Point xy = vlcPlayer.GetResolution();
            if (xy.X == 0)
                return;
            dur = vlcPlayer.GetDuration();
            fi.SetResolution(xy.X, xy.Y);
            fi.playTime = (double)dur;
            fi.minutes = (int)dur / 60;
            fi.seconds = (int)dur - ((int)(dur / 60) * 60);
        }
        public bool bDisplayedResolution = false;
        public void DisplayMovieResolution()
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            Point xy = vlcPlayer.GetResolution();
            if (xy.X == 0)
                return;
            dur = vlcPlayer.GetDuration();
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
                if (dgvFileInfo.RowCount < rowIdMovieList)
                {
                    return;
                }
                if (xy.X > 0)
                {
                    // Temporarily disable sorting to prevent the exception
                    var oldSortMode = dgvFileInfo.Columns.Cast<DataGridViewColumn>()
                        .ToDictionary(col => col.Name, col => col.SortMode);

                    try
                    {
                        // Disable sorting on all columns
                        foreach (DataGridViewColumn col in dgvFileInfo.Columns)
                        {
                            col.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }

                        dgvFileInfo.Rows[rowIdMovieList].Cells["height"].Value = xy.X;
                        dgvFileInfo.Rows[rowIdMovieList].Cells["width"].Value = xy.Y;

                        var ratingCell = dgvFileInfo.Rows[rowIdMovieList].Cells["rating"];
                        if (ratingCell.Value != null && ratingCell.Value.ToString() == ratingDefault)
                        {
                            ratingCell.Value = "z";
                        }

                        dgvFileInfo.Rows[rowIdMovieList].Cells["playTime"].Value = dur;
                        dgvFileInfo.Rows[rowIdMovieList].Cells["minutes"].Value = (int)dur / 60;
                        dgvFileInfo.Rows[rowIdMovieList].Cells["seconds"].Value = (int)dur - ((int)(dur / 60) * 60);
                    }
                    finally
                    {
                        // Restore original sort modes
                        foreach (var kvp in oldSortMode)
                        {
                            if (dgvFileInfo.Columns.Contains(kvp.Key))
                            {
                                dgvFileInfo.Columns[kvp.Key].SortMode = kvp.Value;
                            }
                        }
                    }
                }
            }
            if (cbGetMetaDataOnly.Checked)
                playNextMovie();
        }
        public void DisplayMovieResolutionwas()
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            Point xy = vlcPlayer.GetResolution();
            if (xy.X == 0)
                return;
            dur = vlcPlayer.GetDuration();
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
                if (dgvFileInfo.RowCount < rowIdMovieList)
                {
                    return;
                }
                if (xy.X > 0)
                {
                    dgvFileInfo.Rows[rowIdMovieList].Cells["height"].Value = xy.X;
                    dgvFileInfo.Rows[rowIdMovieList].Cells["width"].Value = xy.Y;

                    var ratingCell = dgvFileInfo.Rows[rowIdMovieList].Cells["rating"];
                    if (ratingCell.Value != null && ratingCell.Value.ToString() == ratingDefault)
                    {
                        ratingCell.Value = "z";
                    }

                    dgvFileInfo.Rows[rowIdMovieList].Cells["playTime"].Value = dur;
                    dgvFileInfo.Rows[rowIdMovieList].Cells["minutes"].Value = (int)dur / 60;
                    dgvFileInfo.Rows[rowIdMovieList].Cells["seconds"].Value = (int)dur - ((int)(dur / 60) * 60);

                    // if (cbGetMetaDataOnly.Checked) //DisplayMovieResolution
                    //   GoToEnd(cbShortEndSample.Checked ? 5 : 15);

                }
            }
            if (cbGetMetaDataOnly.Checked)
                playNextMovie();
        }
        void formatDataGridViewFileListSpecial(int column0Width = 0) //2020
        {
            for (int idx = 0; idx < dgvFileInfo.Columns.Count; idx++)
            {

                dgvFileInfo.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                int colw = dgvFileInfo.Columns[idx].Width;
                dgvFileInfo.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvFileInfo.Columns[idx].Width = colw;
                if (idx == 0 && column0Width > 0)
                    dgvFileInfo.Columns[idx].Width = column0Width;
                else if (idx == 4)
                    dgvFileInfo.Columns[idx].Width = 6;
                else if (idx <= 6)
                    dgvFileInfo.Columns[idx].ReadOnly = true;
            }
            return;
        }
        private void btClear_Click(object sender, EventArgs e)
        {
            dgvFileInfo.DataSource = null;
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
        //dmc25xxx

        private void SelectFirstCell(bool rowsExist)
        {
            if (dgvFileInfo.Rows.Count == 0)
                if (!rowsExist)
                    return;

            // Ensure row headers and selection are enabled
            dgvFileInfo.ClearSelection();
            dgvFileInfo.CurrentCell = dgvFileInfo.Rows[0].Cells[0];
            dgvFileInfo.Rows[0].Cells[0].Selected = true;
        }
        private void dgv1_SelectionChanged(object sender, EventArgs evtArgs)
        {
            if (!cbLoadedAndReady.Checked)
                return;
            if (doingSort)
                return;

            EnsureMetadataForCurrentRowAsync();

            bool foundInWin2 = false;
            if (cbCompareToPreviousImage.Checked)
            {
                pb2.Image = pbThumbNail.Image;
                CompareImagesInSequence();
            }

            try
            {
                //currentCell can be null? dmc2025
                if (dgvFileInfo.CurrentCell == null)
                {
                    rowIdMovieList = -1;
                    tbMovieFpath.Text = "ERROR";
                    tbRowIdTemp.Text = "err";
                    MessageBox.Show("No selected rows or empty set", "ERROR dmc33");
                    return;
                }
                rowIdMovieList = (int)dgvFileInfo.CurrentCell.RowIndex;

                if (rowIdMovieList == previousRowIdWas)
                {
                    return;
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
            tbMovieFpath.Text = dgvFileInfo.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
            //
            tbComment.Text = dgvFileInfo.Rows[rowIdMovieList].Cells["comment"].Value?.ToString() ?? "";
            tbRating.Text = dgvFileInfo.Rows[rowIdMovieList].Cells["rating"].Value.ToString() ?? "";
            //
            tbCurrentFolder.Text = ff.getFolder(tbMovieFpath.Text);
            tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
            tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
            fpath = tbMovieFpath.Text;
            tbFileNameOfSelected.Text = dgvFileInfo.Rows[rowIdMovieList].Cells["fname"].Value.ToString();
            gv.FILE_EXT = Path.GetExtension(tbFileNameOfSelected.Text);
            if (bDisplayHTML)
                DisplaySelectedHTMLFile();
            //tbFolderPath.Text = ff.getFolder(tbMovieFpath.Text);
            // ensure folder includes drive letter (fall back to movie path if needed)
            {
                string moviePath = tbMovieFpath.Text ?? "";
                string folder = ff.getFolder(moviePath) ?? "";

                // If ff returned nothing or a non-rooted path, prefer directory from the full movie path
                if (string.IsNullOrWhiteSpace(folder) || !Path.IsPathRooted(folder))
                {
                    var dirFromMovie = !string.IsNullOrWhiteSpace(moviePath) ? Path.GetDirectoryName(moviePath) : null;
                    if (!string.IsNullOrWhiteSpace(dirFromMovie))
                    {
                        folder = dirFromMovie!;
                    }
                    else
                    {
                        // If ff returned a leading-slash path like "\Users\..." try prepending movie drive/root
                        var root = !string.IsNullOrWhiteSpace(moviePath) ? Path.GetPathRoot(moviePath) : null;
                        if (!string.IsNullOrWhiteSpace(root))
                            folder = Path.Combine(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                                                   folder.TrimStart('\\', '/'));
                    }
                }

                // Normalize to a full path when possible
                try { folder = Path.GetFullPath(folder); } catch { /* ignore */ }

                tbFolderPath.Text = folder;
            }
            if (cbThumbNail.Checked)
            {
                pbThumbNail.Image = gv.mainWindow.CreateThumbnailForFile(tbMovieFpath.Text);
            }

            if (!bThisIsSubWindow) //Traverser Main 
            {
                if (previousRowIdWas == rowIdMovieList)
                {
                    if (bPlayedFirstVideo && rowIdMovieList >= 0)
                        if (retriggerForRowId > rowIdMovieList) //&& retriggerForRowId < dgv1.RowCount)
                        {
                            tbMovieFpath.Text = dgvFileInfo.Rows[rowIdMovieList + 1].Cells["fpath"].Value.ToString();
                            tbCurrentFolder.Text = ff.getFolder(tbMovieFpath.Text);
                            tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
                            tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
                            fpath = tbMovieFpath.Text;
                            tbFileNameOfSelected.Text = dgvFileInfo.Rows[rowIdMovieList + 1].Cells["fname"].Value.ToString();
                            rowIdMovieList = retriggerForRowId;
                            retriggerForRowId = -1;
                        }
                        else
                            retriggerForRowId = -1;
                }
                previousRowIdWas = rowIdMovieList;
                DGV1_PERFORM_SelectionChanged();
                if (gv.bUseOverlay)
                {
                    OpenOverlay();
                    formTransparent.UpdateFilename(tbFileNameOfSelected.Text);
                }

                if (bUseSearchWin3) //============================================================WIN3 search
                {
                    string result = searchWin3.SearchForFile(tbFileNameOfSelected.Text);
                    gv.foundInWin2 = !string.IsNullOrEmpty(result);
                }
                else
                    if (cbSearchWin2EachSelection.Checked && !bThisIsSubWindow) //main Traverser
                    {
                        string fpath = tbMovieFpath.Text;
                        if (fpath.Length > 0)
                            foundInWin2 = SearchWin3();
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






        private async void EnsureMetadataForCurrentRowAsync()
        {
            if (dgvFileInfo == null || dgvFileInfo.CurrentRow == null || dgvFileInfo.CurrentRow.IsNewRow)
                return;

            // We expect the DataGridView to be bound to FileInfoItem objects
            if (dgvFileInfo.CurrentRow.DataBoundItem is not FileInfoItem item)
                return;

            // Already have metadata? nothing to do
            if (item.len > 0 && !string.IsNullOrWhiteSpace(item.stimestamp ?? item.stimestamp))
                return;

            // Need a valid file path
            var path = item.fpath;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return;

            try
            {
                // Do filesystem work off the UI thread
                var fi = await Task.Run(() => new FileInfo(path));

                // Update the object
                item.len = fi.Length;

                // Use whatever timestamp format you prefer
                item.stimestamp = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Mark this row as changed so ProcessChangedRows() will upsert it
                if (dgvFileInfo.CurrentRow != null)
                {
                    int rowIndex = dgvFileInfo.CurrentRow.Index;
                    changedRows.Add(rowIndex);
                }

                // Refresh the current item in the grid/binding source
                var bs = dgvFileInfo.DataSource as BindingSource;
                if (bs != null)
                {
                    if (InvokeRequired)
                        BeginInvoke(new Action(bs.ResetCurrentItem));
                    else
                        bs.ResetCurrentItem();
                }
                else
                {
                    // Fallback: repaint the grid
                    if (InvokeRequired)
                        BeginInvoke(new Action(dgvFileInfo.Refresh));
                    else
                        dgvFileInfo.Refresh();
                }
            }
            catch (IOException)
            {
                // File might have been deleted or locked – ignore for now
            }
            catch (UnauthorizedAccessException)
            {
                // No permission – skip
            }
        }


        int retriggerForRowId = 0;
        bool retriggerDGV1 = false;
        private void xxDg1_SelectionChanged(object sender, EventArgs e)
        {
            if (doingSort)
                return;
            if (dgvFileInfo.CurrentCell == null)
            {
                return;
            }
            if (!dgvFileInfo.Focused)
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
                rowIdMovieList = (int)dgvFileInfo.CurrentCell.RowIndex;
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
            if (rowIdMovieList >= dgvFileInfo.RowCount - 1)
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
            return dgvFileInfo.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
        }
        public Bitmap img1;
        bool bInMainWindow = false;
        public void DGV1_PERFORM_SelectionChanged()
        {
            EnsureMetadataForCurrentRowAsync();
            if (cbAutoPlay.Checked)
            {
                // If WebP conversion is OFF and the current file is a .webp, skip to the next file
                if (!gv.webpConversionMode &&
                    !string.IsNullOrEmpty(fpath) &&
                    string.Equals(Path.GetExtension(fpath), ".xxxx", StringComparison.OrdinalIgnoreCase)) //was .webp
                {
                    GoToNextRow();
                    return;
                }

                if (!bPlayedFirstVideo)
                    return;
                if (vlcPlayer == null || vlcPlayer.IsDisposed)
                {
                    if (mpVersion1)
                        vlcPlayer = new Player(gv, this, tbMovieFpath.Text);
                    vlcPlayer.Activate();
                    vlcPlayer.Show();
                    if (cbFocusHere.Checked) this.Focus();
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
                        bool loadNext = LoadMovie(fpath, cbZoom.Checked);
                        if (loadNext)
                            playNextMovie(true);
                    }
                    TrackFromSourceLoad("DVG1 SELECTION_CHANGED");

                    resetTimerRequestToMovies();
                    resetMessageToZero();
                }
                bReceivedTimerMessage = false;
                resetMessageToZero();
                if (cbCompareToPreviousImage.Checked)
                {
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
            EnsureMetadataForCurrentRowAsync();
            if (cbAutoPlay.Checked) //cbAutoAdv.Checked && 
            {
                if (!bPlayedFirstVideo)
                    return;
                if (vlcPlayer == null || vlcPlayer.IsDisposed)
                {
                    if (mpVersion1)
                        vlcPlayer = new Player(gv, this, tbMovieFpath.Text, null);
                    vlcPlayer.Activate();
                    vlcPlayer.Show();
                    //vlcPlayer.SetMovies(mp);
                    if (cbFocusHere.Checked) this.Focus();
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
                vlcPlayer.SetFullScreen(true);
                bReceivedTimerMessage = false;
                resetMessageToZero();
                if (cbCompareToPreviousImage.Checked)
                {
                    // pb2.Image = pbThumbNail.Image;
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
                // tbMovieFpath.Text = $"need NEXT";// GetFilePath();
                // rcPlayNext = true;
                // retriggerDGV1 = false;
                return false;
            }
            else if (!string.IsNullOrEmpty(fpath2) && !string.IsNullOrWhiteSpace(fpath2))
                vlcPlayer.LoadMedia(fpath2, true); //DVG1 SELECTION_CHANGED   IF autopla
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
            if (_useMovies)
            {
                _useMovie = true;
                _usePictures = false;
                cbAllowMovieSelection.Checked = _useMovie;
            }
        }

        private void cbMovie_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (_usePictures)
            {
                _useMovie = false;
                _useMovies = false;
                cbAllowMovieSelection.Checked = _useMovie;
            }
        }

        private void cbAutoPlay_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAutoPlay.Checked)
            {
                _usePictures = false;
                _useMovie = true;
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
            btTestCopy.Text = $"File Copy to >>{targetDir}";
            btTestCopy.Visible = false;
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
                    fpath = tbFileNameOfSelected.Text;
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
        /// <summary>
        /// Copy an HTML (or HTM) file and its companion resource folder(s) (e.g. *_files) to the target folder
        /// under rating/category character subdirectory (same pattern as copyFile(char)).
        /// Typical saved web pages (IE/Edge/Chrome) create a folder beside the html file named:
        ///   <basename>_files   OR   <basename>.files
        /// We also optionally copy a folder with the exact basename if it exists.
        /// </summary>
        /// <param name="ch">Category/rating character used to select/create subfolder inside targetDir1.</param>
        /// <returns>true if the HTML file was copied (resource folders optional); false on failure or not HTML.</returns>
        public bool copyFileHTML(char ch)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fpath))
                    return false;

                string ext = Path.GetExtension(fpath).ToLowerInvariant();
                if (ext != ".html" && ext != ".htm")
                    return false; // Not an HTML file

                // Base target folder for this rating / category.
                string baseTargetRoot = gv.initParm1List[0].targetDir1;
                if (string.IsNullOrWhiteSpace(baseTargetRoot) || !Directory.Exists(baseTargetRoot))
                    return false;

                string targetDir = Path.Combine(baseTargetRoot, ch.ToString());
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                // Display message window (reuse existing UI logic).
                if (!bDoingBackup)
                    DisplayCopyMessageText(targetDir, fpath, baseTargetRoot);

                string sourceFile = fpath;
                string fileName = Path.GetFileName(sourceFile);
                string destFilePath = Path.Combine(targetDir, fileName);

                // Copy the HTML file (overwrite = false; if already exists we can skip).
                if (!File.Exists(destFilePath))
                {
                    File.Copy(sourceFile, destFilePath, false);
                }

                // Identify potential resource folders.
                string sourceFolder = Path.GetDirectoryName(sourceFile)!;
                string baseName = Path.GetFileNameWithoutExtension(sourceFile);

                var candidateFolders = new List<string>
                {
                    Path.Combine(sourceFolder, baseName + "_files"),
                    Path.Combine(sourceFolder, baseName + ".files"),
                    Path.Combine(sourceFolder, baseName) // occasionally resources stored in folder with same basename
                };

                int copiedFolderCount = 0;
                foreach (var srcFolder in candidateFolders.Distinct())
                {
                    if (!Directory.Exists(srcFolder))
                        continue;

                    // Destination folder with same name under targetDir
                    string destFolder = Path.Combine(targetDir, Path.GetFileName(srcFolder));
                    if (!Directory.Exists(destFolder))
                    {
                        try
                        {
                            // Use existing FileFunctions recursive directory copy.
                            ff.CopyDirectory(srcFolder, destFolder);
                            copiedFolderCount++;
                        }
                        catch
                        {
                            // Ignore individual folder copy failures; continue with others.
                        }
                    }
                }

                // Optionally add entry to sub window list if configured (similar to copyFile logic).
                if (cbAddFInfoToSub.Checked)
                {
                    AddFInfoEntryToSubWindowUpdatedPath(targetDir);
                }

                // Hide progress / message window if used.
                if (!bDoingBackup)
                    HideCopyMessageWindow();

                // Update UI fields (reuse existing pattern).
                showCopyFileInfo(targetDir, sourceFile);
                btCopySuccess.BackColor = Color.LightGreen;

                return true;
            }
            catch
            {
                btCopySuccess.BackColor = Color.DeepPink;
                if (!bDoingBackup)
                    HideCopyMessageWindow();
                return false;
            }
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
                if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
                if (displayMessage == null || displayMessage.IsDisposed)
                {

                }
                else
                {
                    displayMessage.DisplayProgress((int)progressBar1.Value);
                }
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
            btTestCopy.Visible = false;
            if (displayMessage != null && !displayMessage.IsDisposed)
            {
                displayMessage.Completed();
            }
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
            this.Activate();
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
                int selectedIndex = dgvFileInfo.CurrentCell.RowIndex;
                if (true) //selectedIndex > -1)
                {
                    dgvFileInfo.Rows.RemoveAt(rowIdMovieList + 1);
                    dgvFileInfo.Refresh(); // if needed
                }
            }

            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Unable to remove selected row at this time");
            }
        }

        private void btDeleteFile_Click(object sender, EventArgs e)
        {
            numCopyCount.BackColor = Color.Red;
            deleteFile();
            numCopyCount.BackColor = Color.LightGray;
        }
        public bool deleteFile()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                //  if (bPlayedFirstVideo)
                //    vlcPlayer.Stop();
            }
            string deleteFile = fpath;
            bool bDeleted = false;

            dgvFileInfo.Rows[rowIdMovieList].Cells["len"].Value = "0";
            dgvFileInfo.Rows[rowIdMovieList].Cells["rating"].Value = "F";

            if (cbAutoAdv.Checked)
            {
                if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
            //bFocusIsHere = true;
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

        private void cbFocusHere_CheckedChanged(object sender, EventArgs e)
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            if (cbFocusHere.Checked)
                resetMessageToZero();
            vlcPlayer.setFocusOnParent(cbFocusHere.Checked);
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
                    tbPlayStateStatus.Text = "play";
                    if (cbSearchUntilNotFoundIn2.Checked)  //        dmc25xxx       this will work when playing next movie
                    {
                        SoundAlertFocus();
                        if (foundInWin2)
                            playNextMovie(true);
                        //  else
                        //  cbSearchUntilNotFoundIn2.Checked = false;
                    }
                    else if (_usePictures)
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
                    tbPlayStateStatus.Text = "END";
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
            string ext = Path.GetExtension(tbFileNameOfSelected.Text.ToUpper());
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
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
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
                        if (!IsPaused)          // was: !cbPause2.Checked
                        {
                            SetPause(true);     // was: cbPause2.Checked = true
                        }
                    fpath = tbMovieFpath.Text;
                    mind = 9999;
                }
            }
            int sec = (int)pos % 60;
            tbPosition3.Text = $"{mind}:" + sec.ToString("D2");

            if (pos < 5 && cbFocusHere.Checked)
                this.Focus();
            //
            if (!bScanToNextMovie)
            {
                if (bUpdateSearchByTimer && countTimer++ > 1)
                {
                    bUpdateSearchByTimer = false;
                    countTimer = 0;
                    ClearSearchWin3Result();
                    bool found = SearchTrav2DialogFileList();
                }
            }
            //
            dur = vlcPlayer.GetDuration();
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
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return null;
            return vlcPlayer.GetState();
        }
        double dur;
        public void getMovieStatus()
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            double pos = vlcPlayer.GetPositionSeconds();
            int mind = (int)pos / 60;
            int sec = (int)pos % 60;
            tbPosition3.Text = $"{mind}:{sec:N2}";
            dur = vlcPlayer.GetDurationSeconds();
            mind = (int)dur / 60;
            sec = (int)dur % 60;
            tbTotalDuration3.Text = $"{mind}:{sec}";

            DisplayMovieResolution();

        }
        private void btStatus_Click(object sender, EventArgs e)
        {
            getMovieStatus();
            DisplayMovieResolution();
            if (tbFileSelected.Text.Contains(tbFileNameOfSelected.Text))
            {
                tbError.Text = "MISMATCHED FILE NAME";
                tbSearchItem.Text = ff.getFileNameFromPath(tbFileNameOfSelected.Text);
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
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.requestTimerCallToParent(this);
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
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            vlcPlayer.SetFullScreen((bool)cbZoom.Checked);
        }

        private void cbFaster_CheckedChanged(object sender, EventArgs e)
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.SetFasterTimer();
            this.TopMost = cbFaster.Checked;
        }
        public void SetTopMost(bool topMost)
        {
            this.TopMost = topMost;
        }
        private void btExit_Click(object sender, EventArgs e)
        {
            gv.bExitProgram = true;
            CloseTrav();
        }
        public void SetTraversingPopup(string message)
        {
            btTraversing.Text = message;
            btTraversing.Visible = true;
            btTraversing.Update();
        }
        // List<FileInfoItem> orderedByName;

        // Add this field near the top of the DialogTraverser class with other fields
        private FolderFileCountDisplay folderCountWindow;

        // Add this method to DialogTraverser class
        private async void ScanAndDisplayFolderCounts()
        {
            if (!bLoadedList)
                return;

            SetTraversingPopup("Scanning folder file counts...");
            btTraversing.Visible = true;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                var fileList = bThisIsSubWindow ? imageFileList2?.finfoList : imageFileList1?.finfoList;

                if (fileList == null || fileList.Count == 0)
                {
                    MessageBox.Show("No files loaded to analyze.", "Folder Counts",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Group files by folder and count them
                var folderCounts = await Task.Run(() =>
                {
                    var groups = fileList
                        .Where(f => !string.IsNullOrWhiteSpace(f.dpath))
                        .GroupBy(f => f.dpath, StringComparer.OrdinalIgnoreCase)
                        .Select(g => new FolderFileCountDisplay.FolderCount
                        {
                            FolderPath = g.Key,
                            FileCount = g.Count(),
                            TotalSize = g.Sum(f => f.len)
                        })
                        .OrderByDescending(f => f.FileCount)
                        .ToList();

                    return groups;
                });

                // Show the results window
                if (folderCountWindow == null || folderCountWindow.IsDisposed)
                {
                    folderCountWindow = new FolderFileCountDisplay();
                }

                folderCountWindow.SetFolderCounts(folderCounts);

                // Show as non-modal so it stays open
                if (!folderCountWindow.Visible)
                {
                    folderCountWindow.Show();
                }
                else
                {
                    folderCountWindow.BringToFront();
                    folderCountWindow.Activate();
                }

                tbStatus.Text = $"Analyzed {folderCounts.Count} folders with {folderCounts.Sum(f => f.FileCount):N0} files";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning folders:\n{ex.Message}", "Scan Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btTraversing.Visible = false;
                this.Cursor = Cursors.Default;
            }
        }

        // Modify the existing btSortFolder_Click to include folder count option
        private async void btSortFolder_Click(object sender, EventArgs e)
        {
            if (!bLoadedList)
                return;

            // ADDED: Ensure metadata is updated before sorting
            await EnsureMetadataUpdatedAsync();

            doingSort = true;
            SetTraversingPopup("Sorting by Folder...");
            tbGoToSubFolder.Enabled = true;

            ResetSortButtonColor(sender);

            var imageList = bThisIsSubWindow ? imageFileList2 : imageFileList1;
            var list = imageList.finfoList;

            list.Sort((a, b) =>
            {
                int cmp = string.Compare(a.dpath, b.dpath, StringComparison.OrdinalIgnoreCase);
                if (cmp != 0)
                {
                    SortCompleted(false);
                    return cmp;
                }
                return string.Compare(a.fname, b.fname, StringComparison.OrdinalIgnoreCase);
            });

            dgvFileInfo.SuspendLayout();
            dgvFileInfo.DataSource = null;
            dgvFileInfo.DataSource = list;
            previousRowIdWas = -1;
            dgv1_SelectionChanged(null, null);
            dgvFileInfo.ResumeLayout();

            btFindDir.BackColor = Color.LightGreen;
            btTraversing.Visible = false;
            SortCompleted();

            // ADDED: Automatically show folder counts after sorting
            ScanAndDisplayFolderCounts();
        }

        List<FileInfoItem> orderedByFolder;
        private async void btSortFolder_Clickxxxx(object sender, EventArgs e) //<<<<<<<<<<<<<<<<<<<<<<<<<< SORT FOLDER
        {
            if (!bLoadedList)
                return;

            // ADDED: Ensure metadata is updated before sorting
            await EnsureMetadataUpdatedAsync();

            doingSort = true;
            SetTraversingPopup("Sorting by Folder...");
            tbGoToSubFolder.Enabled = true;

            ResetSortButtonColor(sender);

            // choose which imageFileList to work with
            var imageList = bThisIsSubWindow ? imageFileList2 : imageFileList1;
            var list = imageList.finfoList;

            // OPTIONAL: if you really want to *drop* files <= 10000 bytes, keep the Where
            // Otherwise I’d remove that filter so small files still appear.
            list.Sort((a, b) =>
            {
                // Order by folder path
                int cmp = string.Compare(a.dpath, b.dpath, StringComparison.OrdinalIgnoreCase);
                if (cmp != 0)
                {
                    SortCompleted(false);
                    return cmp;
                }

                // tie-breaker: file name
                return string.Compare(a.fname, b.fname, StringComparison.OrdinalIgnoreCase);
            });

            // rebind grid with minimal UI churn
            dgvFileInfo.SuspendLayout();

            // if you’re not already using a BindingSource, this is fine:
            dgvFileInfo.DataSource = null;
            dgvFileInfo.DataSource = list;
            previousRowIdWas = -1;
            dgv1_SelectionChanged(null, null);

            //formatDataGridViewFileList();
            dgvFileInfo.ResumeLayout();

            btFindDir.BackColor = Color.LightGreen;
            btTraversing.Visible = false;
            SortCompleted();
        }

        bool bSorted = false;

        List<FileInfoItem> orderedByDate;
        List<FileInfoItem> orderedByFileType;
        private async void btSortDate_Click(object sender, EventArgs e) //<<<<<<<<<<<<<<<<<<<<<<<<<< SORT DATE
        {
            if (!bLoadedList)
                return;

            // ADDED: Ensure metadata is updated before sorting
            await EnsureMetadataUpdatedAsync();

            doingSort = true;
            SetTraversingPopup("Sorting by Date...");
            ResetSortButtonColor(sender);
            dgvFileInfo.DataSource = null;
            if (bThisIsSubWindow)
            {
                if (cbSortDescending.Checked)
                    orderedByDate = imageFileList2.finfoList.OrderByDescending(file => file.stimestamp).ToList();
                else
                    orderedByDate = imageFileList2.finfoList.OrderBy(file => file.stimestamp).ToList();
            }
            else
            {
                if (cbSortDescending.Checked)
                    orderedByDate = imageFileList1.finfoList.OrderByDescending(file => file.stimestamp).ToList();
                else
                    orderedByDate = imageFileList1.finfoList.OrderBy(file => file.stimestamp).ToList();
            }
            dgvFileInfo.DataSource = orderedByDate;
            previousRowIdWas = -1;
            dgv1_SelectionChanged(null, null);

            //resizeFileListDataGrid();
            if (bThisIsSubWindow)
                imageFileList2.copyFileList(orderedByDate); //ifl 2022 replaced
            else
                imageFileList1.copyFileList(orderedByDate); //ifl 2022 replaced
            btTraversing.Visible = false;
            SortCompleted();
        }

        private void btSortName_Click(object sender, EventArgs e)
        {
            if (!bLoadedList)
                return;



            ResetSortButtonColor(sender);
            SortName();

            previousRowIdWas = -1;

            // Optional: select first row after sort rather than calling SelectionChanged manually
            if (dgvFileInfo.Rows.Count > 0)
            {
                dgvFileInfo.ClearSelection();
                dgvFileInfo.CurrentCell = dgvFileInfo.Rows[0].Cells[0];
                dgvFileInfo.Rows[0].Selected = true;
            }

            btTraversing.Visible = false;
        }

        bool doingSort = false;
        public async void SortName() //<<<<<<<<<<<<<<<<<<<<<<<<<< SORT NAME
        {
            if (!bLoadedList)
                return;
            // ADDED: Ensure metadata is updated before sorting
            await EnsureMetadataUpdatedAsync();

            doingSort = true;
            SetTraversingPopup("Sorting by File Name...");
            tbGoToSubFolder.Enabled = false;
            dgvFileInfo.DataSource = null;
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
            dgvFileInfo.DataSource = orderedByName;
            //resizeFileListDataGrid();
            if (bThisIsSubWindow)
                imageFileList2.copyFileList(orderedByName); //ifl 2022 replaced
            else
                imageFileList1.copyFileList(orderedByName); //ifl 2022 replaced
            //rowCount = dgv1.RowCount;
            SortCompleted();
        }
        public void SortCompleted(bool completed = true)
        {
            doingSort = false;
            if (!completed)
                return;
            //dmc26 AutoSizeDGVColumns();
            btTraversing.Visible = false;
            EnsureMetadataForCurrentRowAsync();
        }
        bool idx3IsWaiting = false;
        private void btSortByFileLength_Click(object sender, EventArgs e)
        {
            tbGoToSubFolder.Enabled = false;
            SortByFileLength(sender, e);
        }
        public async void SortByFileLength(object sender, EventArgs e)
        {
            if (!bLoadedList)
                return;
            // ADDED: Ensure metadata is updated before sorting
            await EnsureMetadataUpdatedAsync();

            doingSort = true;
            SetTraversingPopup("Sorting by File Size...");
            btSortBySize.BackColor = Color.White;
            tbGoToSubFolder.Enabled = false;
            btTraversing.Visible = true;
            btTraversing.Update();
            ResetSortButtonColor(sender);
            dgvFileInfo.DataSource = null;
            List<FileInfoItem> orderedByLen;
            if (bThisIsSubWindow)
            {
                if (imageFileList2 == null | imageFileList2.getImageCount() < 1)
                    return;
                if (cbSortDescending.Checked)
                    orderedByLen = imageFileList2.finfoList.OrderByDescending(file => file.len).ThenBy(file => file.stimestamp).ToList();
                else
                    orderedByLen = imageFileList2.finfoList.OrderBy(file => file.len).ThenBy(file => file.stimestamp).ToList();
            }
            else
            {
                if (imageFileList1 == null | imageFileList1.getImageCount() < 1)
                    return;
                if (cbSortDescending.Checked)
                    orderedByLen = imageFileList1.finfoList.OrderByDescending(file => file.len).ToList();
                else
                    orderedByLen = imageFileList1.finfoList.OrderBy(file => file.len).ToList();
            }
            dgvFileInfo.DataSource = orderedByLen;
            previousRowIdWas = -1;
            dgv1_SelectionChanged(null, null);
            //resizeFileListDataGrid();
            if (bThisIsSubWindow)
                imageFileList2.copyFileList(orderedByLen); //ifl 2022 replaced
            else
                imageFileList1.copyFileList(orderedByLen); //ifl 2022 replaced
            btTraversing.Visible = false;
            if (idx3IsWaiting)
            {
                idx3IsWaiting = false;
            }
            SortCompleted();
        }

        public void ResetSortButtonColor(object sender)
        {
            btFindDir.BackColor = Color.LightGray;
            btSortName.BackColor = Color.LightGray;
            btSortBySize.BackColor = Color.LightGray;
            btSortFolder.BackColor = Color.LightGray;
            btSortOnDate.BackColor = Color.LightGray;
            btSortByRating.BackColor = Color.LightGray;
            btSortByFileType.BackColor = Color.LightGray;
            if (sender != null)
                ((System.Windows.Forms.Button)sender).BackColor = Color.LightGreen;
        }

        private void btMoveMovie_Click(object sender, EventArgs e)
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.MoveToNextScreen();
            return;
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                cbZoom.Checked = false;
                //mp.moveToNextScreen();
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

        // List<FileInfoItem> searchResult;
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
            for (int idx = 0; idx < dgvFileInfo.Rows.Count; ++idx)
            {
                bool isCellChecked = (bool)dgvFileInfo.Rows[idx].Cells["bInvalid"].Value;
                if (isCellChecked)
                {
                    dgvFileInfo.Rows[idx].Visible = true;
                    if (selectFirst)
                    {
                        // dgv1.ClearSelection();
                        dgvFileInfo.Rows[idx].Selected = true;
                        selectFirst = false;
                    }
                }
                else
                {
                    dgvFileInfo.CurrentCell = null;
                    dgvFileInfo.Rows[idx].Visible = false;
                }
            }
            tbError.Text = seachtxt;
        }
        public void searchClear()
        {
            foreach (System.Windows.Forms.DataGridViewRow r in dgvFileInfo.Rows)
            {
                dgvFileInfo.Rows[r.Index].Visible = true;
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
                searchSelectedCount = dgvFileInfo.RowCount;
            }
            if (currentRowIndex < searchSelectedCount)
            {
                currentRow = goIndex[currentRowIndex];
                dgvFileInfo.FirstDisplayedScrollingRowIndex = currentRow;
                dgvFileInfo.CurrentCell = dgvFileInfo.Rows[currentRow].Cells[0];
                dgvFileInfo.Rows[currentRow].Selected = true;
                ++currentRowIndex;

                tbRowId.Text = currentRowIndex.ToString(); //dmc2021??

                tbMovieFpath.Text = dgvFileInfo.Rows[currentRow].Cells["fpath"].Value.ToString();
                tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
                tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
                fpath = tbMovieFpath.Text;
            }
        }
        public void GoToNextImageDataRow()
        {
            if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
                dgvFileInfo.FirstDisplayedScrollingRowIndex = currentRow;
                dgvFileInfo.CurrentCell = dgvFileInfo.Rows[currentRow].Cells[0];
                dgvFileInfo.Rows[currentRow].Selected = true;
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
                dgvFileInfo.DataSource = null;
                dgvFileInfo.DataSource = imageFileList2;
            }
            else
            {
                if (imageFileList1 == null)
                    imageFileList1 = new ImageFileList();
                imageFileList1.addItem(finfo);
                dgvFileInfo.DataSource = null;
                dgvFileInfo.DataSource = imageFileList1;
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
            string fname = dgvFileInfo.Rows[rowIdMovieList].Cells["fname"].Value.ToString();
            string ext = dgvFileInfo.Rows[rowIdMovieList].Cells["ext"].Value.ToString();
            string dpath = dgvFileInfo.Rows[rowIdMovieList].Cells["dpath"].Value.ToString();
            string fpath = dgvFileInfo.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();

            string type = dgvFileInfo.Rows[rowIdMovieList].Cells["type"].Value.ToString();
            int level = Convert.ToInt32(dgvFileInfo.Rows[rowIdMovieList].Cells["level"].Value);
            long len = (long)Convert.ToUInt64(dgvFileInfo.Rows[rowIdMovieList].Cells["len"].Value);

            var dtime = dgvFileInfo.Rows[rowIdMovieList].Cells["stimestamp"].Value.ToString();
            // DateTime ts = Convert.ToDateTime(dtime);// "yyyy/mm/dd mm:ss:ii");
            //  u :2000-08-17 23:32:32Z
            string stimestamp = dtime.ToString();
            DateTime test = ff.ConvertStringToDateTime(stimestamp);

            bool bDelete = Convert.ToBoolean(dgvFileInfo.Rows[rowIdMovieList].Cells["bDelete"].Value);
            bool bInvalid = Convert.ToBoolean(dgvFileInfo.Rows[rowIdMovieList].Cells["bInvalid"].Value);
            char rating = Convert.ToChar(dgvFileInfo.Rows[rowIdMovieList].Cells["minutes"].Value);
            string source = "";
            string comment = Convert.ToString(dgvFileInfo.Rows[rowIdMovieList].Cells["comment"].Value);

            //double playTime = DateTime.ParseExact(dtime, "yyyy-MM-dd HH:mm"); //, CultureInfo.InvariantCulture);
            double playTime = Convert.ToDouble(dgvFileInfo.Rows[rowIdMovieList].Cells["playTime"].Value);
            int minutes = Convert.ToInt32(dgvFileInfo.Rows[rowIdMovieList].Cells["minutes"].Value);
            int seconds = Convert.ToInt32(dgvFileInfo.Rows[rowIdMovieList].Cells["seconds"].Value);

            int width = Convert.ToInt32(dgvFileInfo.Rows[rowIdMovieList].Cells["width"].Value);
            int height = Convert.ToInt32(dgvFileInfo.Rows[rowIdMovieList].Cells["height"].Value);

            FileInfoItem fi = new FileInfoItem(fname, ext, dpath, fpath, type, level, len, stimestamp, bDelete, bInvalid,
                rating, source, playTime, minutes, seconds, width, height, comment);

            return fi;

        }
        int searchSelectedCount = 0;
        public int getSelectedRowCount()
        {
            IEnumerable<DataGridViewRow> selectedRows = dgvFileInfo.SelectedCells.Cast<DataGridViewCell>().Select(cell => cell.OwningRow).Distinct();
            //int count = selectedRows.Count();
            int scount = dgvFileInfo.SelectedRows.Count;
            searchSelected = dgvFileInfo.SelectedCells;
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
                if (vlcPlayer == null || vlcPlayer.IsDisposed)
                {
                    if (mpVersion1)
                        vlcPlayer = new Player(gv, this, tbMovieFpath.Text);
                    else
                        vlcPlayer2 = /*P2*/ new Player(gv, this, tbMovieFpath.Text);
                    vlcPlayer.Activate();
                    vlcPlayer.Show();
                    // gv.SetMovies(mp);
                }
                else
                {
                    if (CheckFileTypeWEBM(fpath))
                        return;
                    resetMpPlay();
                    vlcPlayer.LoadMedia(tbMovieFpath.Text, true); //DGV1 double-clicked
                    TrackFromSourceLoad("DGV1 double-clicked");

                }
                if (cbBeep.Checked)
                {
                    cbFocusHere.Checked = true;
                    // cbZoom.Checked = true;
                }
                vlcPlayer.SetFullScreen(cbZoom.Checked);
                resetTimerRequestToMovies(); // send after mp.loadMovie
                resetMessageToZero();
            }
        }
        public string getFileName()
        {
            // return ff.getFileName(tbFileName.Text); // fpath);
            return tbFileNameOfSelected.Text; // fpath);
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
            dgvFileInfo.MultiSelect = false;
            lastSearchTitle = searchTitle;
            dgvFileInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
            tbFoundSeachInWin3.Text = "";
            if (formTransparent != null && !formTransparent.IsDisposed)
                formTransparent.UpdateFilename("");
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
                string fileName = gv.dialogTraverser2.getFileName();
                tbFound2Folder.Text = gv.dialogTraverser2.getFolder();
                tbFoundFileFolder.Text = tbFound2Folder.Text;
                tbFoundSeachInWin3.Text = fileName;
                tbFoundSeachInWin3.BackColor = Color.LightGreen;
                if (formTransparent != null && !formTransparent.IsDisposed)
                    formTransparent.UpdateFilename2(fileName);

                tbFound2Folder.BackColor = Color.LightGreen;
                bFoundText = true;
                tbWin2FileSizeMatchSearch.Text = gv.dialogTraverser2.getFileLength();
            }
            else
            {
                tbFoundSeachInWin3.BackColor = Color.LightGray;
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
            dgvFileInfo.Rows.Clear();
            dgvFileInfo.Refresh();
        }
        public int StartSearchWin2Size(long len) // SEARCH SUBWINDOW SUBSEARCH SUB
        {
            tbFound2Folder.Text = "";
            string searchForThis = "";
            tbFoundSeachInWin3.Text = "";
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
                tbFoundSeachInWin3.Text = test;
                tbFoundSeachInWin3.BackColor = Color.LightGreen;
                tbFound2Folder.BackColor = Color.LightGreen;
                bFoundText = true;
                tbWin2FileSizeMatchSearch.Text = gv.dialogTraverser2.getFileLength();
            }
            else
            {
                tbFoundSeachInWin3.BackColor = Color.LightGray;
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
                dgvFileInfo.MultiSelect = false;
                dgvFileInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            if (searchRow >= dgvFileInfo.RowCount)
                searchRow = 0;
            else
                searchRow++;
            string searchtxtUpper = searchTitle.ToUpper();
            for (; searchRow < dgvFileInfo.RowCount - 1; ++searchRow)
            {
                if (dgvFileInfo.Rows[searchRow].Cells[0].Value.ToString().ToUpper().Contains(searchtxtUpper))
                {
                    if (true)
                    {
                        //rowIdMovieList = searchRow;
                        //Select the found item
                        dgvFileInfo.CurrentCell = dgvFileInfo.Rows[searchRow].Cells[0];
                        dgvFileInfo.Rows[searchRow].Selected = true;

                        dgvFileInfo.FirstDisplayedScrollingRowIndex = searchRow;

                        dgvFileInfo.Refresh();
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
                        tbFileSelected.Text = dgvFileInfo.Rows[searchRow].Cells["fpath"].Value.ToString();
                        //oldName = dgvFileList1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
                        //
                        tbRowId.Text = searchRow.ToString();
                        lastRowIdSet = searchRow;
                        //
                        tbMovieFpath.Text = dgvFileInfo.Rows[searchRow].Cells["fpath"].Value.ToString();
                        tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
                        tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
                        fpath = tbMovieFpath.Text;
                        tbFileNameOfSelected.Text = dgvFileInfo.Rows[searchRow].Cells[0].Value.ToString();
                        // rowIdMovieList = searchRow++;
                        rowIdMovieList = (int)dgvFileInfo.CurrentCell.RowIndex;
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
            foreach (System.Windows.Forms.DataGridViewRow rowDataFileInfo in dgvFileInfo.Rows)
            {
                if ((rowDataFileInfo.Cells[0].Value).ToString().ToUpper().Contains(seachtxtUpper))
                {

                    if (rowIdMovieList < rowDataFileInfo.Index && rowDataFileInfo.Index < dgvFileInfo.RowCount)
                    {
                        //  rowIdMovieList = rowDataFileInfo.Index;

                        //dgvFileList1.CurrentCell = dgvFileList1.Rows[rowIdMovieList].Cells[0];
                        dgvFileInfo.CurrentCell = dgvFileInfo.Rows[rowIdMovieList].Cells[0];
                        dgvFileInfo.Rows[rowIdMovieList].Selected = true;

                        dgvFileInfo.FirstDisplayedScrollingRowIndex = rowIdMovieList;

                        dgvFileInfo.Refresh();
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
                    if (rowIdMovieList < dgvFileInfo.RowCount - 2)
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
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
            {
                if (mpVersion1)
                    vlcPlayer = new Player(gv, this, tbMovieFpath.Text);
                else
                    vlcPlayer2 = /*P2*/ new Player(gv, this, tbMovieFpath.Text);
                vlcPlayer.Activate();
                vlcPlayer.Show();
                // gv.SetMovies(vlcPlayer);
                if (cbFocusHere.Checked) this.Focus();
                resetMpPlay();
                vlcPlayer.LoadMedia(tbMovieFpath.Text, true); //playNext2
                TrackFromSourceLoad("playNextVideo");

                if (CheckFileTypeWEBM(fpath))
                    return;
                vlcPlayer.Play();
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
                vlcPlayer.LoadMedia(fpath, true); //playNext2
                TrackFromSourceLoad("playNext2");

                resetTimerRequestToMovies();// send after mp.loadMovie
                resetMessageToZero();
                if (play)
                    vlcPlayer.Play();
            }
        }

        private void btCloseMovie_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                    vlcPlayer.Hide();
            }
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.Close();
                vlcPlayer.Dispose();
            }
        }
        public void StopSlideShow()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.Close();
                vlcPlayer.Dispose();
            }
        }


        /* PSEUDOCODE / PLAN:
           - When the source-folder combo selection changes:
             1. Get the selected item as a string; if null/empty, just return.
             2. Validate that the selected string represents an existing directory using Directory.Exists().
             3. If the directory does NOT exist:
                a. Remove the entry from the combo box items (by value or index).
                b. Find any matching entry in gv.folderHistoryList (case-insensitive) and remove it if found.
                c. Do NOT change tbDirectoryPath; simply return.
             4. If the directory DOES exist:
                a. Set tbDirectoryPath.Text to the selected path (as before).
                b. Optionally compute a sanitizedFilename if needed (preserve prior behavior).
                c. Find matching history entry by folderPath (case-insensitive) and, if found, populate tbCopyFileName.Text from its comment.
        */

        private void cmboSourceFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Guard: ensure there is a selected item
            if (cmboSourceFolder.SelectedItem == null)
                return;

            string selected = cmboSourceFolder.SelectedItem.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(selected))
                return;

            // If the selected folder does not exist, remove from combo and history and return
            if (!Directory.Exists(selected))
            {
                try
                {
                    int selIndex = cmboSourceFolder.SelectedIndex;

                    // Remove from folder history list if present (case-insensitive)
                    int idx = gv.folderHistoryList.FindIndex(fh => fh.folderPath.Equals(selected, StringComparison.OrdinalIgnoreCase));
                    if (idx >= 0)
                    {
                        gv.folderHistoryList.RemoveAt(idx);
                    }

                    // Remove item from combo box. Try remove by index if valid, otherwise by value.
                    if (selIndex >= 0 && selIndex < cmboSourceFolder.Items.Count)
                        cmboSourceFolder.Items.RemoveAt(selIndex);
                    else
                        cmboSourceFolder.Items.Remove(selected);
                }
                catch
                {
                    // Swallow exceptions to avoid disrupting UI flow; nothing else to do.
                }

                // Do NOT place the invalid path into tbDirectoryPath
                return;
            }

            // Valid directory -> original behavior
            tbDirectoryPath.Text = selected;

            string sanitizedFilename = new string(selected.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());

            int idx2 = gv.folderHistoryList.FindIndex(fh => fh.folderPath.Equals(tbDirectoryPath.Text, StringComparison.OrdinalIgnoreCase));
            if (idx2 >= 0)
            {
                tbCopyFileName.Text = gv.folderHistoryList[idx2].desc;
                gv.folderHistoryList[idx2].lastAccessDate = DateTime.Now;
            }
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
            bool rc = ff.renameFile(tbMovieFpath.Text, tbRenameTo.Text);
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
            cbMute.Checked = true;
            if (bPlayedFirstVideo)
                Resume();
            StartScanThru();
        }
        public void StartScanThru()
        {
            if (!cbFocusHere.Checked)
                cbFocusHere.Checked = true;
            if (!bPlayedFirstVideo)
                playFirstVideo();
            // if (!cbZoom.Checked)
            //   cbZoom.Checked = true;
            cbScanMovies.Checked = true;
            if (cbLongerSegments.Checked)
                vlcPlayer.SetSlowerTimer();
        }
        private void CbInvalid_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInvalid.Checked)
                searchInvalid();
        }

        private void btSetTarget1_Click(object sender, EventArgs e)
        {
            tbTargetFolder.Text = gv.initParm1List[0].targetDir1;
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
            if (rowIdMovieList < dgvFileInfo.RowCount - 1)
            {
                // rowIdMovieList++;
                //dgv1.ClearSelection();
                retriggerForRowId = -1; /// temp  rowIdMovieList + 1;
                dgvFileInfo.CurrentCell = dgvFileInfo.Rows[rowIdMovieList + 1].Cells["fname"]; // dgv1.CurrentCell = dgv1.Rows[rowIdMovieList+].Cells[0];  correct
                dgvFileInfo.Rows[rowIdMovieList + 1].Selected = true; //SELECTED WILL SET rowIdMovieList 

                if (dgvFileInfo.SelectedRows.Count > 1)
                    MessageBox.Show("multiselect", "error");
                dgvFileInfo.Refresh();
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
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            if (rowIdMovieList < dgvFileInfo.RowCount - 1)
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
                    vlcPlayer.LoadMedia(fpath, cbZoom.Checked);//playNextMovie
                    TrackFromSourceLoad("playNextMove");
                }
                resetTimerRequestToMovies();// send after vlcPlayer.LoadMedia
                resetMessageToZero();
            }
            else //end of list
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

                vlcPlayer.Play();
                if (!vlcPlayer.GetState().Contains("Play"))
                {
                    vlcPlayer.Play();
                }
            }
            // first timer message back will vlcPlayer.Play();
            if (cbScanMovies.Checked)
            {
                cbMute.Checked = true;
            }
            DisplayMovieResolution();
        }


        public void ClearPlayInfoFields()
        {
            tbTotalDuration3.Text = "";
            tbPosition3.Text = "";
            tbResolution.Text = "";

        }
        private void PlayNextWas(object sender, EventArgs e)
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
        // 
        //  PLAYBACK CONTROLS
        //
        private void btBackup_Click(object sender, EventArgs e)
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.SkipBackwards();
        }
        private void btSkipBack_Click_5(object sender, EventArgs e)
        {
            SkipBack();
        }
        public void SkipBack()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.SkipBackwards();   // your Player implementation subtracts 10 seconds
        }



        private void btSlower_Click(object sender, EventArgs e)
        {
            if (vlcPlayer != null)
            {
                SetSlow(true);              // was: cbSlow.Checked = true
                vlcPlayer.SetSlowerTimer();
            }
        }





        public void SkipForward()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.SkipForwards(); // your Player subtracts +10 seconds
                getMovieStatus();
            }
        }

        private void numAhead_DoubleClick(object sender, EventArgs e)
        {
            numAhead.Value = 0;
        }


        public void SkipForward1min()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.SkipForward(60);
                getMovieStatus();
            }
        }

        public void SkipForward2min()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.SkipForward(120);
                getMovieStatus();
            }
        }

        private void btGoToEnd_Click(object sender, EventArgs e)
        {
            GoToEnd(cbShortEndSample.Checked ? 5 : 15);
        }
        private void cbMute_CheckedChanged(object sender, EventArgs e)
        {
            if (vlcPlayer != null)
                vlcPlayer.ToggleMute(cbMute.Checked);
        }


        public void ResumeNormalSpeed()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.ResumeNormalSpeed();
                resetMpPlay();
            }
        }
        bool resetingCbSlow = false;
        //
        //
        public void SetSmallPlayback()
        {
            cbZoom.Checked = false;

            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;

            vlcPlayer.SetSmallScreen(200, 200);
        }


        public void ZoomMovieFullScreen()
        {
            SoundAlertFocus();
            cbZoom.Checked = true;

            getMovieStatus();
            resetMessageToZero();

            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;

            vlcPlayer.SetFullScreen(true);
        }
        public void completedMovieZoomMessage()
        {
        }
        //
        //
        //       END OF  PLAYBACK CONTROLS  
        //
        //
        private void TraverserDialog_Leave(object sender, EventArgs e)
        {
            cbFocusHere.BackColor = Color.LightGray;
            if (cbFocusHere.Checked)
                if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                    vlcPlayer.SetFocusOnParent(true);
            bFocusIsHere = false;
            gv.mainWindow.soundAlert(6);
            cbFocusIsOnThisWindow.Checked = bFocusIsHere;
        }
        private ImageFileList test()
        {
            return gv.imageFileList1;
        }
        //dmc262626
        // returns type ImageFileList    gv.imageFileList1
        // 
        // Transition bool fields - will replace CheckBox.Checked values
        private bool _useHTML;
        private bool _useMIDI;
        private bool _usePictures;
        private bool _useImages;
        private bool _useMovies;
        private bool _useVideo;
        private bool _useMovie;

        bool windowLoaded = false;
        
        private void cmbSearchExtensions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSearchExtensions.SelectedItem == null)
                return;

            string selected = cmbSearchExtensions.SelectedItem.ToString();

            if (selected == "Select File type")
            {
                tbSearchExtensions.Text = "";
                return;
            }

            if (!windowLoaded)
            {
                windowLoaded = true;
                return;
            }

            // ── Custom: prompt the user for extension(s) ──────────────────────────
            if (string.Equals(selected, "custom", StringComparison.OrdinalIgnoreCase))
            {
                string input = PromptCustomExtension();
                if (string.IsNullOrWhiteSpace(input))
                {
                    // User cancelled — revert combo to previous selection
                    int prev = cmbSearchExtensions.FindStringExact(gv.searchExtensionCategoryInUse ?? "videos");
                    if (prev >= 0) cmbSearchExtensions.SelectedIndex = prev;
                    return;
                }

                // Parse "jpg, png, *.mp4" → normalise to "*.jpg", "*.png", "*.mp4"
                string[] exts = input
                    .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim().StartsWith("*") ? x.Trim() : "*." + x.TrimStart('.').Trim())
                    .ToArray();

                gv.SearchExtensions["custom"] = exts;
                gv.customSearchExtensions = string.Join(", ", exts);
                tbSearchExtensions.Text = gv.customSearchExtensions;
            }
            else
            {
                //tbExtensionsInUse.Text = selected;
                tbSearchExtensions.Text = selected;
            }
            // ─────────────────────────────────────────────────────────────────────

            selectedSearchExtensionType = selected;
            gv.searchExtensionCategoryInUse = selected;

            Properties.Settings.Default.LastSearchExtension = selected;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Shows a simple input dialog prompting the user for file extension(s).
        /// Returns the raw input string, or null if cancelled.
        /// </summary>
        private string PromptCustomExtension()
        {
            using var dlg = new Form
            {
                Text = "Custom File Extensions",
                Width = 420,
                Height = 155,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label
            {
                Text = "Enter extension(s) separated by commas:\n(e.g.  avif, tiff, *.cr2)",
                Left = 12,
                Top = 12,
                Width = 380,
                Height = 36
            };

            var tb = new TextBox
            {
                Left = 12,
                Top = 54,
                Width = 380,
                Text = gv.customSearchExtensions ?? "*.*"
            };
            tb.SelectAll();

            var ok = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Left = 220,
                Top = 84,
                Width = 80
            };
            var cancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Left = 312,
                Top = 84,
                Width = 80
            };

            dlg.Controls.AddRange(new Control[] { lbl, tb, ok, cancel });
            dlg.AcceptButton = ok;
            dlg.CancelButton = cancel;

            return dlg.ShowDialog(this) == DialogResult.OK ? tb.Text : null;
        }


        // ── field (add near other string fields at the top of the class) ───────
        private string selectedSearchExtensionType = string.Empty;
        private bool setDefaultSearchExtension = false;

        // ── PopulateSearchExtensionsComboBox ────────────────────────────────────
        /// <summary>
        /// Fills cmbSearchExtensions from gv.SearchExtensions keys,
        /// then restores the previously selected category.
        /// Call once from the constructor, and again after the editor saves.
        /// </summary>
        private void PopulateSearchExtensionsComboBox()
        {
            if (cmbSearchExtensions == null) return;

            // Suppress the SelectedIndexChanged event while we reload
            cmbSearchExtensions.SelectedIndexChanged -= CmbSearchExtensions_SelectedIndexChanged;
            cmbSearchExtensions.BeginUpdate();

            cmbSearchExtensions.Items.Clear();

            if (gv?.SearchExtensions != null && gv.SearchExtensions.Count > 0)
            {
                foreach (string key in gv.SearchExtensions.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
                    cmbSearchExtensions.Items.Add(key);
            }
            else
            {
                // Fallback hard-coded categories so the combo is never empty
                foreach (string key in new[] { "images", "videos", "videos_webm", "midi", "html", "all" })
                    cmbSearchExtensions.Items.Add(key);
            }

            // Restore selection ─ prefer last used, then field, then index 0
            string toSelect = gv?.searchExtensionCategoryInUse
                              ?? selectedSearchExtensionType
                              ?? string.Empty;

            int idx = string.IsNullOrEmpty(toSelect)
                      ? -1
                      : cmbSearchExtensions.FindStringExact(toSelect);

            cmbSearchExtensions.SelectedIndex = idx >= 0 ? idx : (cmbSearchExtensions.Items.Count > 0 ? 0 : -1);

            cmbSearchExtensions.EndUpdate();
            cmbSearchExtensions.SelectedIndexChanged += CmbSearchExtensions_SelectedIndexChanged;

            // Fire once manually so tbSearchExtensionsInUse reflects the restored value
            SyncSearchExtensionFromCombo();
        }

        // ── SelectedIndexChanged handler ────────────────────────────────────────
        private void CmbSearchExtensions_SelectedIndexChanged(object sender, EventArgs e)
        {
            SyncSearchExtensionFromCombo();
        }

        // ── Shared sync logic ───────────────────────────────────────────────────
        private void SyncSearchExtensionFromCombo()
        {
            string selected = cmbSearchExtensions.SelectedItem as string;
            if (string.IsNullOrEmpty(selected)) return;

            // Update state
            selectedSearchExtensionType = selected;
            gv.searchExtensionCategoryInUse = selected;

            // Show the actual extension patterns in the text box
            if (gv.SearchExtensions != null &&
                gv.SearchExtensions.TryGetValue(selected, out string[] exts))
            {
                tbExtensionsInUse.Text = string.Join(", ", exts);
            }
            else
            {
                tbExtensionsInUse.Text = selected;
            }

            // Persist across sessions
            try
            {
                Properties.Settings.Default.LastSearchExtension = selected;
                Properties.Settings.Default.Save();
            }
            catch { /* settings may not exist in all build configs */ }
        }
        private void PopulateSearchExtensionsComboBoxxxxxx()
        {
            if (gv?.SearchExtensions == null)
                return;

            cmbSearchExtensions.BeginUpdate();
            cmbSearchExtensions.Items.Clear();

            // Add all keys from the SearchExtensions dictionary
            foreach (var key in gv.SearchExtensions.Keys.OrderBy(k => k))
            {
                cmbSearchExtensions.Items.Add(key);
            }

            if (setDefaultSearchExtension)
            {

                // Set default selection
                if (cmbSearchExtensions.Items.Count > 0)
                {
                    // Try to restore previously selected category
                    if (!string.IsNullOrEmpty(gv.searchExtensionCategoryInUse))
                    {
                        int index = cmbSearchExtensions.FindStringExact(gv.searchExtensionCategoryInUse);
                        cmbSearchExtensions.SelectedIndex = index >= 0 ? index : 0;
                    }
                    else
                    {
                        // Default to "images" or first item
                        int defaultIndex = cmbSearchExtensions.FindStringExact("images");
                        cmbSearchExtensions.SelectedIndex = defaultIndex >= 0 ? defaultIndex : 0;
                    }
                }
            }

            cmbSearchExtensions.EndUpdate();
        }
        private void btTraverse_Click(object sender, EventArgs e)
        {
            tbGoToSubFolder.Enabled = false;
            cbLoadedAndReady.Checked = false;
            tbMovieFpath.Text = "";

            // Get category from gv.searchExtensionCategoryInUse
            string category = gv.searchExtensionCategoryInUse ?? "images";

            // Set file type for main window
            gv.mainWindow.SetFileType(category);
            fileType = category;

            // Update UI based on category
            if (category == "videos" || category == "videos_webm")
            {
                cbAutoPlay.Checked = true;
                btTraverse.BackColor = Color.LightGreen;
                //btPlay.Enabled = true;
            }
            else // images, html, midi, all
            {
                btTraverse.BackColor = Color.LightBlue;
                //btPlay.Enabled = false;
            }

            bSavedFile = false;
            if (!TraverseGo())
            {
                tbMovieFpath.Text = "!! ERROR Invalid directory !!";
                MessageBox.Show("!! ERROR Invalid directory !!");
                return;
            }

            cbLoadedAndReady.Checked = true;
            btDeleteMarkedFiles.BackColor = Color.LightYellow;

            bLoadedList = true;
            this.BringToFront();
            this.Activate();
        }


        public string fileType = "";
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
        private void Test()
        {
            doTraversal(tbDirectoryPath.Text, bThisIsSubWindow);
            // traverser.StartTraversal(args);
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
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.SetSlowerDelay(cbLongerSegments.Checked, durationOfSampleSeconds);
        }

        private void btSmallPlayback_Click(object sender, EventArgs e)
        {
            SetSmallPlayback();
        }

        private void btGetMovieSize_Click(object sender, EventArgs e)
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                movieLocAndSize = vlcPlayer.GetSize();
                tbInfo.Text = $"{movieLocAndSize.Width} {movieLocAndSize.Height}";
            }
        }

        private void btOpenMovieWin_Click(object sender, EventArgs e)
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
            {
                if (mpVersion1)
                    vlcPlayer = new Player(gv, this);
            }
            vlcPlayer.Activate();
            vlcPlayer.Show();
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
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.SetFocusOnParent();
        }

        private void btClearERRORSTOP_Click(object sender, EventArgs e)
        {
            gv.ERROR_STOP = false;
            btCloseMovie.BackColor = Color.LightBlue;
        }

        private void btErrorDisplay_Click(object sender, EventArgs e)
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.ErrorDisplay();
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
            dgvFileInfo.DataSource = null;
            dgvFileInfo.DataSource = imageFileListSearch;

            tbCount.Text = dgvFileInfo.RowCount.ToString();
            cbLoadedAndReady.Checked = true;
            // AutoSizeDGVColumns();
            if (imageFileListSearch.Count > 0)
            {
                DisplaySearchingSubFound(searchTitle);
                return imageFileListSearch.Count;
            }
            else
            {
                DisplaySearchingSubNotFound(searchTitle);
                if (dgvFileInfo.DisplayedRowCount(true) > 9)
                {
                    dgvFileInfo.DataSource = null;
                    dgvFileInfo.Refresh();
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
            dgvFileInfo.DataSource = null;
            dgvFileInfo.DataSource = imageFileListSearch;
            //AutoSizeDGVColumns(); //search
            tbCount.Text = dgvFileInfo.RowCount.ToString();
            cbLoadedAndReady.Checked = true;
            //formatDataGridViewFileList();
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

            //formatDataGridViewFileList();
        }

        private void btUpdateImageFile_Click(object sender, EventArgs e)
        {
            UpdateXmlFileList();
        }

        private void cbOnErrContinue_CheckedChanged(object sender, EventArgs e)
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.SetOnErrContinue(cbOnErrContinue.Checked);
        }
        public void IsSpecialFolderSet()
        {
            cbSpecialFolder.Checked = IsSpecialFolderSet(null); // does  main.setTargetFolder(fullPath);
            cbSetSpecialFolder.Checked = cbSpecialFolder.Checked;
        }
        private void btOpenFileListWindow_Click(object sender, EventArgs e)
        {
            IsSpecialFolderSet();
            if (bUseSearchWin3)
                return;
            cbFocusHere.Checked = false;
            btSearchWin2.BackColor = Color.LightGreen;
            if (gv.dialogTraverser2 == null || gv.dialogTraverser2.IsDisposed)
            {
                gv.dialogTraverser2 = new DialogTraverser(gv, main, 0, ff, this);
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
            if (bUseSearchWin3)
                searchWin3.SearchForFile(tbFileNameOfSelected.Text);
            else
                SearchWin3();
        }
        public bool SearchWin3() //call this
        {
            gv.foundInWin2 = false;

            if (!EnsureSearchWin3Ready())
                return false;

            string? result = searchWin3.SearchForFile(tbFileNameOfSelected.Text);
            bool found = !string.IsNullOrEmpty(result);

            if (found)
            {
                tbFoundSeachInWin3.Text = result;
                tbFound2Folder.Text = searchWin3.GetSubFolderFound();
            }

            gv.foundInWin2 = found;
            return found;
        }

        public bool SearchWin2() //call this
        {
            gv.foundInWin2 = false;

            if (!EnsureSearchWin3Ready())
                return false;

            string? result = searchWin3.SearchForFile(tbFileNameOfSelected.Text);
            bool found = !string.IsNullOrEmpty(result);

            if (found)
            {
                tbFoundSeachInWin3.Text = result;
                tbFound2Folder.Text = searchWin3.GetSubFolderFound();
            }

            gv.foundInWin2 = found;
            return found;
        }
        bool bUseSubWinSubsetSearch = true;
        long filelen = 0;
        public bool ClearAndSearch2()
        {
            bool found = false;
            if (bThisIsSubWindow)
                return found;
            else
                tbFoundSeachInWin3.Text = "";
            ClearSearchWin3Result();
            // bool rc = SearchOtherTMain();
            // StartSearchWin2Title();

            int foundRow = -1;
            if (!cbSearchForLenMatch.Checked)
                foundRow = StartSearchWin2Title(tbFileNameOfSelected.Text); //Search Window 2
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
        public void SearchOtherResultFound() // NO OLD T2
        {
            tbFoundSeachInWin3.Text = gv.dialogTraverser2.getFileName();
            tbFoundSeachInWin3.BackColor = Color.LightGreen;
            btSearchWin2.BackColor = Color.LightGreen;
            tbWin2FileSizeMatchSearch.Text = gv.dialogTraverser2.getFileLength();
        }
        public void ClearSearchWin3Result()
        {
            if (searchWinUntil != null && !searchWinUntil.IsDisposed)
                searchWinUntil.FileName("");
            tbFoundSeachInWin3.Text = "";
            tbFoundSeachInWin3.BackColor = Color.White;
            tbFoundFileFolder.Text = "";
            btSearchWin2.BackColor = Color.LightGray;
            tbWin2FileSizeMatchSearch.Text = "";
        }
        public void ClearSearchOtherResult(string startSymbol)
        {
            //if (tbFoundSeachList2.Text.mi
            if (tbFoundSeachInWin3.Text.Contains($"{startSymbol} {startSymbol}"))
            {
                tbFoundSeachInWin3.Text = "";
                tbFoundFileFolder.Text = "";
            }
            else
            {
                tbFoundSeachInWin3.Text = $"{startSymbol} {tbFoundSeachInWin3.Text}";
            }
            tbFoundSeachInWin3.BackColor = Color.White;
            lblSEARCH2state.Text = currentRowIndex.ToString();
            lblSEARCH2state.BackColor = Color.LightGray;
        }
        public string GetPartialFileNameForSearch(string searchText)
        {
            int posDot = searchText.LastIndexOf(".");
            if (posDot > 4)
            {
                posDot -= 4;
                searchText = tbFileNameOfSelected.Text.Substring(0, posDot);
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
                String searchText = tbFileNameOfSelected.Text;
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
                    String searchText = tbFileNameOfSelected.Text;
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
            tbFoundSeachInWin3.Text = "";
            tbFoundFileFolder.Text = "";
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                tbSearchItem.Text = ff.getFolder(fpath);
                tbPlayBackError.Text = "checking play title...";
                tbFoundSeachInWin3.Text = ff.getFileNameFromPath(vlcPlayer.GetTitle());

                if (tbFoundSeachInWin3.Text != tbFileNameOfSelected.Text)
                {
                    tbPlayBackError.Text = $"wrong title in play";
                    PlayNextVideo(true);
                }
                else
                {
                    tbPlayBackError.Text = $"checking play title {tbFoundSeachInWin3.Text}";
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
            formatDataGridViewFileListSpecial((int)numColumnWidth.Value);
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

        }


        private void cbMinimize_CheckedChanged(object sender, EventArgs e)
        {
        }
        public void MinimizePlayer()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.MinimizeMovieWin(cbHidePlayer.Checked);
        }
        public void RestorePlayer()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.MinimizeMovieWin(false);
        }
        bool bOneLetterSearch = false;
        private void tbGoToSubFolder_TextChanged(object sender, EventArgs e)
        {
            if (bOneLetterSearch)
                SearchForFolder();
        }
        public void SearchForSubfolder(string folder) //searchforfolder old version don't use this one
        {
            string dir;

            for (; searchRow < dgvFileInfo.RowCount; ++searchRow)
            {
                dir = ff.getFolder(dgvFileInfo.Rows[searchRow].Cells[1].Value.ToString().ToUpper());
                if (dir == folder)
                {

                    if (true)
                    {
                        dgvFileInfo.CurrentCell = dgvFileInfo.Rows[searchRow].Cells[0];
                        dgvFileInfo.Rows[searchRow].Selected = true;

                        dgvFileInfo.FirstDisplayedScrollingRowIndex = searchRow;

                        dgvFileInfo.Refresh();
                        bFoundText = true;
                        tbFileSelected.Text = dgvFileInfo.Rows[searchRow].Cells["fpath"].Value.ToString();
                        tbRowId.Text = searchRow.ToString();
                        lastRowIdSet = searchRow;
                        //
                        tbMovieFpath.Text = dgvFileInfo.Rows[searchRow].Cells["fpath"].Value.ToString();
                        fpath = tbMovieFpath.Text;
                        tbFileNameOfSelected.Text = dgvFileInfo.Rows[searchRow].Cells[0].Value.ToString();
                        // rowIdMovieList = searchRow++;
                        rowIdMovieList = (int)dgvFileInfo.CurrentCell.RowIndex;
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
            cbFocusHere.Checked = true;
        }
        public int SearchForFolder() // THIS IS THE MAIN SEARCH OF THE DGV LIST for a certain (or partial) FILE NAME
        {
            bool bFoundSubFolder = false;
            int searchRow = 0;
            dgvFileInfo.MultiSelect = false;
            dgvFileInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            string searchtxtUpper = tbGoToSubFolder.Text.Trim().ToUpper();
            if (searchtxtUpper.Length < 1)
                return -1;

            // Normalize input to backslash (Windows paths use '\')
            searchtxtUpper = searchtxtUpper.Replace('/', '\\');

            // Ensure search text is preceded by a separator so "4" matches "\4" not "44"
            if (!searchtxtUpper.StartsWith('\\'))
                searchtxtUpper = '\\' + searchtxtUpper;

            for (searchRow = 0; searchRow < dgvFileInfo.RowCount - 1; ++searchRow)
            {
                var dpathRaw = dgvFileInfo.Rows[searchRow].Cells["dpath"].Value?.ToString();
                if (string.IsNullOrEmpty(dpathRaw))
                    continue;

                // Normalize dpath to backslash before comparing
                string dpathUpper = dpathRaw.Replace('/', '\\').ToUpper();

                if (dpathUpper.Contains(searchtxtUpper))
                {
                    dgvFileInfo.CurrentCell = dgvFileInfo.Rows[searchRow].Cells[0];
                    dgvFileInfo.Rows[searchRow].Selected = true;
                    dgvFileInfo.FirstDisplayedScrollingRowIndex = searchRow;
                    dgvFileInfo.Refresh();

                    bFoundSubFolder = true;
                    tbFileSelected.Text = dgvFileInfo.Rows[searchRow].Cells["fpath"].Value.ToString();
                    tbRowId.Text = searchRow.ToString();
                    lastRowIdSet = searchRow;
                    tbMovieFpath.Text = dgvFileInfo.Rows[searchRow].Cells["fpath"].Value.ToString();
                    fpath = tbMovieFpath.Text;
                    tbFileNameOfSelected.Text = dgvFileInfo.Rows[searchRow].Cells[0].Value.ToString();
                    rowIdMovieList = (int)dgvFileInfo.CurrentCell.RowIndex;
                    tbRowIdTemp.Text = rowIdMovieList.ToString();
                    break;
                }
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
            dgvFileInfo.MultiSelect = false;
            dgvFileInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            for (searchRow = 0; searchRow < dgvFileInfo.RowCount - 1; ++searchRow)
            {
                int len = (int)dgvFileInfo.Rows[searchRow].Cells["width"].Value;
                if (len == 0)
                {
                    bool invalid = (bool)dgvFileInfo.Rows[searchRow].Cells["bInvalid"].Value;
                    if (!invalid)
                    {
                        dgvFileInfo.CurrentCell = dgvFileInfo.Rows[searchRow].Cells[0];
                        dgvFileInfo.Rows[searchRow].Selected = true;

                        dgvFileInfo.FirstDisplayedScrollingRowIndex = searchRow;

                        dgvFileInfo.Refresh();
                        bFoundSubFolder = true;
                        tbFileSelected.Text = dgvFileInfo.Rows[searchRow].Cells["fpath"].Value.ToString();

                        tbRowId.Text = searchRow.ToString();
                        lastRowIdSet = searchRow;
                        //
                        tbMovieFpath.Text = dgvFileInfo.Rows[searchRow].Cells["fpath"].Value.ToString();
                        tbCurrentFileFolder.Text = ff.getFolder(tbMovieFpath.Text);
                        tbFoundFileFolder.Text = tbCurrentFileFolder.Text;
                        fpath = tbMovieFpath.Text;
                        tbFileNameOfSelected.Text = dgvFileInfo.Rows[searchRow].Cells[0].Value.ToString();
                        // rowIdMovieList = searchRow++;
                        rowIdMovieList = (int)dgvFileInfo.CurrentCell.RowIndex;
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
            vlcPlayer.Close();
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
            vlcPlayer.Stop();
            vlcPlayer.ResetAndPlay(tbMovieFpath.Text);


            //   DGV1_EFFECT_SelectionChanged(rowIdMovieList);
        }

        private void tbMovieFpath_TextChanged(object sender, EventArgs e)
        {

        }

        private void btGetMovieNameFromMoviesWindow_Click(object sender, EventArgs e)
        {
            tbMovieFpath.Text = "query movie win";
            vlcPlayer.Stop();
            tbMovieFpath.Text = vlcPlayer.GetMovieFPath();
        }

        private void btMovies2_Click(object sender, EventArgs e)
        {
        }

        private void btSetSource_Click(object sender, EventArgs e)
        {
            rememberLastFolders(tbDirectoryPath.Text);
        }
        int startRowSearchNotFound = 0;

        private bool EnsureSearchWin3Ready()
        {
            if (searchWin3 == null || searchWin3.IsDisposed)
            {
                searchWin3 = new SearchForMediaByName(gv, gv.mainWindow, this, ff, tbTargetFolder.Text);
                searchWin3.TraverseFolders();
            }

            return searchWin3 != null && !searchWin3.IsDisposed;
        }

        public bool SearchUntilNotFoundStep()
        {
            OpenPlayer();
            cbSearchUntilNotFoundIn2.Checked = true;

            if (!EnsureSearchWin3Ready())
            {
                cbSearchUntilNotFoundIn2.Checked = false;
                return false;
            }

            string? result = searchWin3.SearchForFile(tbFileNameOfSelected.Text);
            bool found = !string.IsNullOrEmpty(result);

            if (found)
            {
                playNextMovie(true);
                return true;
            }

            cbSearchUntilNotFoundIn2.Checked = false;
            return false;
        }

        public void btSearchUntilNotFound_Click(object sender, EventArgs e)
        {
            // SearchUntilNotFoundStep();

            if (fileType.Equals("videos"))
            {
                searchWinUntil = new SearchUntilNotFound(gv, this);
                searchWinUntil.Show();
                searchWinUntil.BringToFront();
                searchWinUntil.Activate();
            }
        }
        public bool SearchUntilNotFoundStepxx()
        {
            OpenPlayer();
            cbSearchUntilNotFoundIn2.Checked = true;

            bool found = SearchWin3(); //ClearAndSearch2(); 
            if (found)
            {
                playNextMovie(true);
                return true;
            }

            cbSearchUntilNotFoundIn2.Checked = false;
            return false;
        }




        public void btSearchUntilNotFound_Click2(object sender, EventArgs e)
        {
            OpenPlayer();
            cbSearchUntilNotFoundIn2.Checked = true;
            playNextMovie(true);
        }
        public void OpenPlayer()
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
            {
                if (mpVersion1)
                    vlcPlayer = new Player(gv, this, tbMovieFpath.Text);
                vlcPlayer.Activate();
                vlcPlayer.Show();
                // gv.SetMovies(mp);
                if (cbMute.Checked)
                    vlcPlayer.ToggleMute(cbMute.Checked);
            }
        }
        public void SetSearchUntilNotFoundIn2()
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



        private void btSetTargetDirToThis_Click(object sender, EventArgs e)
        {
            SettargetDirToThis();
        }
        public bool SettargetDirToThis(string mainTarget = null)
        {
            if (mainTarget != null)
                tbTargetFolder.Text = mainTarget;
            bool foundDir = ff.directoryExists(tbTargetFolder.Text);
            if (foundDir)
            {
                btSetTargetDirToThis.BackColor = Color.LightGreen;
                IsSpecialFolderSet();
            }
            else
            {
                btSetTargetDirToThis.BackColor = Color.OrangeRed;
            }
            return foundDir;
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
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            if (cbSmallSize.Checked)
            {
                cbZoom.Checked = false;
            }
            // vlcPlayer.SetSmallScreenLock(cbSmallSize.Checked);

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
        bool _useSlideShow = false;
        private void cbSearchUntilNotFoundIn2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSearchUntilNotFoundIn2.Checked)
            {
                this.Refresh();
                cbSearchUntilNotFoundIn2.Refresh();
                SetSearchUntilNotFoundIn2();

                _useSlideShow = true;
            }
            else
            {
                if (_usePictures)
                    _useSlideShow = false;
            }
        }

        private void cbPositionSearchStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (!bThisIsSubWindow)
                return;
        }

        private void tbTargetFolder_TextChanged(object sender, EventArgs e)
        {
            //cbSpecialFolder.Checked = IsSpecialFolderSet(tbTargetFolder.Text);
        }

        /*
        DriveInfo di = ff.GetDriveInfo(tbTargetFolder.Text);
        if (di != null)
            tbTargetFreeSpace.Text = $"free space {di.AvailableFreeSpace}";
        */


        private void TraverserDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (searchWinUntil != null && !searchWinUntil.IsDisposed)
                searchWinUntil.Close();
            // Save folder history before closing
            if (gv?.mainWindow != null)
            {
                try
                {
                    gv.mainWindow.SaveHistoryList();
                }
                catch (Exception ex)
                {
                    // Log but don't block closing
                    gv.debug?.w("Failed to save history on close", ex.Message);
                }
            }

            // Clean up video player
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.Close();
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

            dgvFileInfo.DataSource = null;
            dgvFileInfo.DataSource = gv.imageFileList1.finfoList;
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
                //   gv.dialogTraverser2.Activate();
                return;
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
            _useMovies = false;
            _useMovie = false;
            _usePictures = false;
            _useMIDI = false;
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
            _useMovies = false;
            _useMovie = false;
            _usePictures = false;
            _useMIDI = true;
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

        private void btOpenWorkingFolder_Click(object sender, EventArgs e)
        {
            string path = tbFolderPath.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("No folder specified.", "Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // If the path points to an existing file, open Explorer and select the file.
                if (File.Exists(path))
                {
                    string args = $"/select,\"{path}\"";
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = args,
                        UseShellExecute = true
                    });
                    return;
                }

                // If the path points to an existing directory, open Explorer at that directory.
                if (Directory.Exists(path))
                {
                    string args = $"\"{path}\"";
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = args,
                        UseShellExecute = true
                    });
                    return;
                }

                // Try to normalize a relative path to absolute and re-check
                try
                {
                    string full = Path.GetFullPath(path);
                    if (Directory.Exists(full))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = $"\"{full}\"",
                            UseShellExecute = true
                        });
                        return;
                    }
                }
                catch { /* ignore normalization errors */ }

                MessageBox.Show($"Folder or file not found:\n{path}", "Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open folder:\n{ex.Message}", "Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
            idx3IsWaiting = true;
            SortByFileLength(sender, e);
            gv.mainWindow.SetImageList3(imageFileList2); //imageFileList2
            if (btSendList3.BackColor == Color.LightGreen)
                btSendList3.BackColor = Color.LightGray;
            else
                btSendList3.BackColor = Color.LightGreen;
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
        public bool IsSpecialFolderSet(string fullPath)
        {
            if (main == null)
                return false;
            if (fullPath == null)
                fullPath = tbTargetFolder.Text;
            // Remove trailing '/' or '\' if present
            //fullPath = fullPath.TrimEnd('/', '\\');
            fullPath = NormalizePath(fullPath);
            if (fullPath.EndsWith("/AAAA") || fullPath.EndsWith("\\AAAA"))
            {
                main.setTargetFolder(fullPath);
                cbSpecialFolder.Checked = true;
                return true;
            }
            else
            {
                main.setTargetFolder(fullPath);
                cbSpecialFolder.Checked = false;
                return false;
            }
        }
        public string SetSpecialFolder(bool bOn, string folderName = null)
        {
            if (folderName == null)
                folderName = tbTargetFolder.Text;
            folderName = folderName.TrimEnd('/', '\\');
            bool rc = IsSpecialFolderSet(folderName);

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
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.HidePlayer();
            }
        }
        public void UnHidePlayer()
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                vlcPlayer.UnHidePlayer();
            }

        }

        private void btUnHidePlayer_Click(object sender, EventArgs e)
        {
            UnHidePlayer();
        }

        private void btResizeHider_Click(object sender, EventArgs e)
        {
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
            {
                //  vlcPlayer.ResizeWindow();
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



        private void btRefreshWin2_Click(object sender, EventArgs e)
        {

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

        private void btImageList_Click(object sender, EventArgs e)
        {

        }

        private void cbScanAndPlay_CheckedChanged(object sender, EventArgs e)
        {
            if (cbScanAndPlay.Checked)
                numSampleCount.Value = 5;
        }

        private void cbOverLayTransparentWin_CheckedChanged(object sender, EventArgs e)
        {
            gv.bUseOverlay = cbOverLayTransparentWin.Checked;
            if (gv.bUseOverlay)
            {
                OpenTransparentWin();
            }
            else
            {
                if (formTransparent != null && !formTransparent.IsDisposed)
                    formTransparent.Close();
            }
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
                if (_usePictures)
                {
                    cbSearchUntilNotFoundIn2.Checked = true;
                    //btPlay_Click(null, null);
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

        private void cbSetSpecialFolder_CheckedChanged(object sender, EventArgs e)  // SET Special Folder
        {
            bool set = IsSpecialFolderSet(tbTargetFolder.Text);
            if (cbSetSpecialFolder.Checked)
            {
                if (!set)
                    SetSpecialFolder(true);
            }


            else
            {
                if (set)
                    SetSpecialFolder(false);
            }
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
                //cbPause2.Checked = false;
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

        private void btResize1080_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Size = new System.Drawing.Size(1920, 1080);
        }


        public void ScaleFormAndControls(float scaleFactor)
        {
            // Scale the form itself
            this.Scale(new SizeF(scaleFactor, scaleFactor));
            // Recursively scale all child controls
            ScaleControlsRecursive(this.Controls, scaleFactor);
        }

        private void ScaleControlsRecursive(Control.ControlCollection controls, float scaleFactor)
        {
            foreach (Control ctrl in controls)
            {
                ctrl.Scale(new SizeF(scaleFactor, scaleFactor));
                // Optionally, scale font size
                if (ctrl.Font != null)
                {
                    ctrl.Font = new System.Drawing.Font(ctrl.Font.FontFamily, ctrl.Font.Size * scaleFactor, ctrl.Font.Style);
                }
                // Recursively scale child controls
                if (ctrl.HasChildren)
                {
                    ScaleControlsRecursive(ctrl.Controls, scaleFactor);
                }
            }
        }
        public void SetResult(string fname, string folder = "")  //dmc25xxx
        {
            tbFoundSeachInWin3.Text = fname;
            tbFound2Folder.Text = folder;
            if (searchWinUntil != null && !searchWinUntil.IsDisposed)
                searchWinUntil.FileName(fname);
            if (string.IsNullOrEmpty(fname))
            {
                foundInWin2 = false;
                if (cbSearchUntilNotFoundIn2.Checked)
                {
                    cbSearchUntilNotFoundIn2.Checked = false;
                }
                if (formTransparent != null && !formTransparent.IsDisposed)
                    formTransparent.FoundFile("");
            }
            else
            {
                foundInWin2 = true;
                if (formTransparent != null && !formTransparent.IsDisposed)
                    formTransparent.FoundFile(fname, folder);

            }

        }
        public void ClosedWin3()
        {
            bUseSearchWin3 = false;
        }
        SearchForMediaByName searchWin3;
        private void btSearchMediaByName_Click(object sender, EventArgs e)
        {
            bUseSearchWin3 = true;
            if (searchWin3 == null || searchWin3.IsDisposed)
            {
                searchWin3 = new SearchForMediaByName(gv, gv.mainWindow, this, ff, tbTargetFolder.Text);
                searchWin3.TraverseFolders();
                // searchWin3.SetLoadedTrav();
            }
            else
                searchWin3.SearchForFile(tbFileNameOfSelected.Text);

            searchWin3.Show();
            searchWin3.Activate();
            searchWin3.BringToFront();
        }

        /// <summary>
        /// Called when SearchForMediaByName completes its traversal.
        /// Enables the Display List button to show source/target comparison.
        /// </summary>
        public void SourceTraversalCompleted()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(SourceTraversalCompleted));
                return;
            }

            btDisplayList.Enabled = true;
            btDisplayList.BackColor = Color.LightGreen;

            // Optional: Show status message
            tbStatus.Text = "Search window ready - Display List enabled";

            // Optional: Play sound alert
            if (gv?.mainWindow != null)
            {
                try
                {
                    gv.mainWindow.soundAlert(1);
                }
                catch { }
            }
        }
        public void DGV1IncrementCurrentRow()
        {
            if (dgvFileInfo.CurrentCell != null && dgvFileInfo.CurrentCell.RowIndex < dgvFileInfo.RowCount - 1)
            {
                int nextRowIndex = dgvFileInfo.CurrentCell.RowIndex + 1;
                dgvFileInfo.CurrentCell = dgvFileInfo.Rows[nextRowIndex].Cells[0]; // Set focus to the first cell of the next row
                dgvFileInfo.Rows[nextRowIndex].Selected = true; // Highlight the next row
            }
        }

        private void btShowList2_Click(object sender, EventArgs e)
        {
            gv.fileListWin.Activate();
        }

        private void btScrollToMiddle_Click(object sender, EventArgs e)
        {
            ScrollToMiddle(dgvFileInfo);
        }

        public void ApplyRatingChange(FileInfoItem _fileInfoItem)
        {
            if (dgvFileInfo.CurrentRow != null)
            {
                int rowIndex = dgvFileInfo.CurrentRow.Index;
                dgvFileInfo.Rows[rowIndex].Cells["rating"].Value = _fileInfoItem.rating;
                dgvFileInfo.Rows[rowIndex].Cells["source"].Value = _fileInfoItem.source;
                dgvFileInfo.Rows[rowIndex].Cells["comment"].Value = _fileInfoItem.comment;
            }
        }

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
        public FileInfoItem updatedFileInfo = null;
        public FileInfoItem previousRecord = null;

        private void btSetMainTarget_Click(object sender, EventArgs e)
        {

        }
        // Add this method to TraverserDialog

        private void CopyUpdatedRatingsToRatingsFile()
        {
            if (bThisIsSubWindow || gv.imageFileList1 == null || gv.imageFileList1.getImageFileListLength() <= 1)
                return;

            string ratingsFile = null;
            if (gv.FILE_TYPE == "images")
                ratingsFile = gv.ratingsImages;
            else if (gv.FILE_TYPE == "videos")
                ratingsFile = gv.ratingsVideos;

            if (string.IsNullOrEmpty(ratingsFile))
                return;

            // Create or update the ratings file
            List<FileInfoItem> ratingsList = new List<FileInfoItem>();
            if (File.Exists(ratingsFile))
            {
                try
                {
                    ratingsList = DeserializeFromXML(ratingsFile);
                }
                catch
                {
                    // If deserialization fails, start with an empty list
                    ratingsList = new List<FileInfoItem>();
                }
            }

            // Build a lookup for fast access
            var ratingsDict = ratingsList.ToDictionary(f => f.fpath, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < gv.imageFileList1.getImageFileListLength(); ++i)
            {
                var fileItem = gv.imageFileList1.getIndexed(i);
                if (fileItem == null || string.IsNullOrEmpty(fileItem.fpath))
                    continue;

                // Find the corresponding row in dgv1
                for (int row = 0; row < dgvFileInfo.RowCount; ++row)
                {
                    var rowPath = dgvFileInfo.Rows[row].Cells["fpath"].Value?.ToString();
                    if (string.Equals(rowPath, fileItem.fpath, StringComparison.OrdinalIgnoreCase))
                    {
                        char rating = dgvFileInfo.Rows[row].Cells["rating"].Value != null ? Convert.ToChar(dgvFileInfo.Rows[row].Cells["rating"].Value) : ' ';
                        string source = dgvFileInfo.Rows[row].Cells["source"].Value?.ToString() ?? "";
                        string comment = dgvFileInfo.Rows[row].Cells["comment"].Value?.ToString() ?? "";

                        // Only write rows where the rating is not 'g'
                        if (rating != 'g')
                        {
                            if (ratingsDict.TryGetValue(fileItem.fpath, out var existingItem))
                            {
                                // Update existing entry
                                existingItem.rating = rating;
                                existingItem.source = source;
                                existingItem.comment = comment;
                            }
                            else
                            {
                                // Add new entry
                                ratingsList.Add(new FileInfoItem
                                {
                                    fpath = fileItem.fpath,
                                    rating = rating,
                                    source = source,
                                    comment = comment
                                });
                            }
                        }
                        break;
                    }
                }
            }

            // Serialize the updated ratings list back to the file
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoItem>));
                using (TextWriter writer = new StreamWriter(ratingsFile))
                {
                    serializer.Serialize(writer, ratingsList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save ratings file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool bSetCommentToDefaultIfEmpty = true;
        string commentDefault = "Z";
        /// <summary>
        /// Goes through dgv1 rows (ImageFileList XML loaded), looks up each row's fpath/normalized in DB,
        /// and if found, copies rating, source, comment, and other fields into the grid.
        /// Returns count of rows updated.
        /// </summary>

        private int ApplyRatingsToDGV1FromDB()
        {
            if (dgvFileInfo == null || dgvFileInfo.Rows.Count == 0)
                return 0;

            // Ensure all needed columns exist  IF NOT then create them (case-insensitive).
            var colNormalized = EnsureColumn(dgvFileInfo, "normalized");
            var colFpath = EnsureColumn(dgvFileInfo, "fpath");
            var colRating = EnsureColumn(dgvFileInfo, "rating");
            var colSource = EnsureColumn(dgvFileInfo, "source");
            var colComment = EnsureColumn(dgvFileInfo, "comment");
            var colBInvalid = EnsureColumn(dgvFileInfo, "bInvalid");
            var colWidth = EnsureColumn(dgvFileInfo, "width");
            var colHeight = EnsureColumn(dgvFileInfo, "height");
            var colPlayTime = EnsureColumn(dgvFileInfo, "playTime");
            var colMinutes = EnsureColumn(dgvFileInfo, "minutes");
            var colSeconds = EnsureColumn(dgvFileInfo, "seconds");

            int updated = 0;
            int rowIndex = 0;
            int giveUpdate = 100;
            foreach (DataGridViewRow row in dgvFileInfo.Rows)
            {
                if (row.IsNewRow) continue;

                var fpathObj = row.Cells[colFpath.Index].Value;
                var fpath = fpathObj?.ToString();
                if (string.IsNullOrWhiteSpace(fpath)) continue;

                // Compute normalized value for this row and store it
                var normalized = NormalizeFullPathForDb(fpath);
                row.Cells[colNormalized.Index].Value = normalized;

                // Look up by normalized (drive-independent)
                var rec = _repo.GetByKeyPath(normalized);
                rowIndex++;
                --giveUpdate;
                if (giveUpdate <= 0)
                {
                    giveUpdate = 100;
                    tbDbMessage.Text = $"{rowIndex} of {dgvFileInfo.Rows.Count}...";
                    tbDbMessage.Refresh();
                }

                if (rec == null) continue;

                // Update all columns from DB record
                row.Cells[colRating.Index].Value = rec.rating == '\0' || rec.rating == ' ' ? "" : rec.rating.ToString();
                row.Cells[colSource.Index].Value = rec.source ?? "";
                row.Cells[colComment.Index].Value = rec.comment ?? "";
                row.Cells[colBInvalid.Index].Value = rec.bInvalid;
                row.Cells[colWidth.Index].Value = rec.width;
                row.Cells[colHeight.Index].Value = rec.height;
                row.Cells[colPlayTime.Index].Value = rec.playTime;
                row.Cells[colMinutes.Index].Value = rec.minutes;
                row.Cells[colSeconds.Index].Value = rec.seconds;

                // Optionally set comment to default if empty (using the fields above)
                if (bSetCommentToDefaultIfEmpty)
                {
                    string commentDefault = "z";
                    string? currentComment = row.Cells[colComment.Index].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(currentComment) && !string.IsNullOrWhiteSpace(commentDefault))
                    {
                        row.Cells[colComment.Index].Value = commentDefault;

                        if (row.DataBoundItem is DataRowView drv)
                            SafeSet(drv.Row, "comment", commentDefault);
                        else if (row.DataBoundItem != null)
                            TrySetProperty(row.DataBoundItem, "comment", commentDefault);
                    }
                }

                // Update bound record
                if (row.DataBoundItem is DataRowView drv2)
                {
                    SafeSet(drv2.Row, "rating", row.Cells[colRating.Index].Value);
                    SafeSet(drv2.Row, "source", row.Cells[colSource.Index].Value);
                    SafeSet(drv2.Row, "comment", row.Cells[colComment.Index].Value);
                    SafeSet(drv2.Row, "bInvalid", row.Cells[colBInvalid.Index].Value);
                    SafeSet(drv2.Row, "width", row.Cells[colWidth.Index].Value);
                    SafeSet(drv2.Row, "height", row.Cells[colHeight.Index].Value);
                    SafeSet(drv2.Row, "playTime", row.Cells[colPlayTime.Index].Value);
                    SafeSet(drv2.Row, "minutes", row.Cells[colMinutes.Index].Value);
                    SafeSet(drv2.Row, "seconds", row.Cells[colSeconds.Index].Value);
                }
                else if (row.DataBoundItem != null)
                {
                    TrySetProperty(row.DataBoundItem, "rating", rec.rating);
                    TrySetProperty(row.DataBoundItem, "source", rec.source);
                    TrySetProperty(row.DataBoundItem, "comment", rec.comment);
                    TrySetProperty(row.DataBoundItem, "bInvalid", rec.bInvalid);
                    TrySetProperty(row.DataBoundItem, "width", rec.width);
                    TrySetProperty(row.DataBoundItem, "height", rec.height);
                    TrySetProperty(row.DataBoundItem, "playTime", rec.playTime);
                    TrySetProperty(row.DataBoundItem, "minutes", rec.minutes);
                    TrySetProperty(row.DataBoundItem, "seconds", rec.seconds);
                }
                updated++;
            }

            if (dgvFileInfo.DataSource is DataTable dt)
                dt.AcceptChanges();

            return updated;
        }
        private static string NormalizeFullPathForDbUnix(string? fullpath)
        {
            if (string.IsNullOrWhiteSpace(fullpath))
                return string.Empty;

            // Trim and unify slashes to '/'
            var s = fullpath.Trim().Replace('\\', '/');

            // Handle drive letter like "K:/..." or "K:..."
            if (s.Length >= 2 && s[1] == ':')
            {
                s = s.Substring(2);    // remove "K:"
                if (s.StartsWith("/"))
                    s = s.Substring(1); // remove leading slash after drive
            }

            // Optional: for UNC paths like "//server/share/..." you may choose to keep as-is
            // if (s.StartsWith("//")) { ... }

            return s;
        }

        public static string NormalizeFullPathForDb(string? fullPath)
        {
            if (String.IsNullOrWhiteSpace(fullPath))
                return "";

            // 1. Trim whitespace
            string p = fullPath.Trim();

            // 2. Replace any '/' with '\'
            p = p.Replace('/', '\\');

            // 3. Fix doubled slashes \\ -> \
            while (p.Contains("\\\\"))
                p = p.Replace("\\\\", "\\");

            // 4. Uppercase for case-insensitive identity match
            p = p.ToUpperInvariant();

            return p;
        }


        /// <summary>
        /// Goes through dgv1 rows (ImageFileList XML loaded), looks up each row's fpath in DB,
        /// and if found, copies rating, source, comment into the grid.
        /// Returns count of rows updated.
        /// </summary>

        private int ApplyRatingsToDGV1FromDB(bool bSetCommentToDefaultIfEmpty = false, string? commentDefault = null)
        {
            if (dgvFileInfo == null || dgvFileInfo.Rows.Count == 0)
                return 0;

            // Ensure all needed columns exist  IF NOT then create them (case-insensitive).
            var colNormalized = EnsureColumn(dgvFileInfo, "normalized");
            var colFpath = EnsureColumn(dgvFileInfo, "fpath");
            var colRating = EnsureColumn(dgvFileInfo, "rating");
            var colSource = EnsureColumn(dgvFileInfo, "source");
            var colComment = EnsureColumn(dgvFileInfo, "comment");
            var colBInvalid = EnsureColumn(dgvFileInfo, "bInvalid");
            var colWidth = EnsureColumn(dgvFileInfo, "width");
            var colHeight = EnsureColumn(dgvFileInfo, "height");
            var colPlayTime = EnsureColumn(dgvFileInfo, "playTime");
            var colMinutes = EnsureColumn(dgvFileInfo, "minutes");
            var colSeconds = EnsureColumn(dgvFileInfo, "seconds");

            int updated = 0;

            foreach (DataGridViewRow row in dgvFileInfo.Rows)
            {
                if (row.IsNewRow) continue;

                var fpathObj = row.Cells[colFpath.Index].Value;
                var fpath = fpathObj?.ToString();
                if (string.IsNullOrWhiteSpace(fpath)) continue;

                var rec = _repo.GetByFpath(fpath);
                if (rec == null) continue;

                // Update all columns from DB record
                row.Cells[colRating.Index].Value = rec.rating == '\0' || rec.rating == ' ' ? "" : rec.rating.ToString();
                row.Cells[colSource.Index].Value = rec.source ?? "";
                row.Cells[colComment.Index].Value = rec.comment ?? "";
                row.Cells[colBInvalid.Index].Value = rec.bInvalid;
                row.Cells[colWidth.Index].Value = rec.width;
                row.Cells[colHeight.Index].Value = rec.height;
                row.Cells[colPlayTime.Index].Value = rec.playTime;
                row.Cells[colMinutes.Index].Value = rec.minutes;
                row.Cells[colSeconds.Index].Value = rec.seconds;

                // Optionally set comment to default if empty
                if (bSetCommentToDefaultIfEmpty)
                {
                    string? currentComment = row.Cells[colComment.Index].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(currentComment) && !string.IsNullOrWhiteSpace(commentDefault))
                    {
                        row.Cells[colComment.Index].Value = commentDefault;
                        if (row.DataBoundItem is DataRowView drv)
                            SafeSet(drv.Row, "comment", commentDefault);
                        else if (row.DataBoundItem != null)
                            TrySetProperty(row.DataBoundItem, "comment", commentDefault);
                    }
                }

                // Update bound record
                if (row.DataBoundItem is DataRowView drv2)
                {
                    SafeSet(drv2.Row, "rating", row.Cells[colRating.Index].Value);
                    SafeSet(drv2.Row, "source", row.Cells[colSource.Index].Value);
                    SafeSet(drv2.Row, "comment", row.Cells[colComment.Index].Value);
                    SafeSet(drv2.Row, "bInvalid", row.Cells[colBInvalid.Index].Value);
                    SafeSet(drv2.Row, "width", row.Cells[colWidth.Index].Value);
                    SafeSet(drv2.Row, "height", row.Cells[colHeight.Index].Value);
                    SafeSet(drv2.Row, "playTime", row.Cells[colPlayTime.Index].Value);
                    SafeSet(drv2.Row, "minutes", row.Cells[colMinutes.Index].Value);
                    SafeSet(drv2.Row, "seconds", row.Cells[colSeconds.Index].Value);
                }
                else if (row.DataBoundItem != null)
                {
                    TrySetProperty(row.DataBoundItem, "rating", rec.rating);
                    TrySetProperty(row.DataBoundItem, "source", rec.source);
                    TrySetProperty(row.DataBoundItem, "comment", rec.comment);
                    TrySetProperty(row.DataBoundItem, "bInvalid", rec.bInvalid);
                    TrySetProperty(row.DataBoundItem, "width", rec.width);
                    TrySetProperty(row.DataBoundItem, "height", rec.height);
                    TrySetProperty(row.DataBoundItem, "playTime", rec.playTime);
                    TrySetProperty(row.DataBoundItem, "minutes", rec.minutes);
                    TrySetProperty(row.DataBoundItem, "seconds", rec.seconds);
                }

                updated++;
            }

            if (dgvFileInfo.DataSource is DataTable dt)
                dt.AcceptChanges();

            return updated;
        }



        // ---- helpers ----

        // Find a column (case-insensitive). If missing, create a text column and append.
        private static DataGridViewColumn EnsureColumn(DataGridView dgv, string name)
        {
            var col = dgv.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

            if (col != null) return col;

            // Create column
            var newCol = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = name,
                DataPropertyName = name, // helps if bound to DataTable with same column
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };
            dgv.Columns.Add(newCol);
            return newCol;
        }

        // Safely set a DataRow column if it exists
        private static void SafeSet(DataRow row, string colName, object? value)
        {
            if (row.Table.Columns.Contains(colName))
                row[colName] = value ?? DBNull.Value;
        }

        // Use reflection for POCO bound items (optional)
        private static void TrySetProperty(object obj, string propName, object value)
        {
            var prop = obj.GetType().GetProperty(propName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (prop != null && prop.CanWrite)
            {
                try
                {
                    if (value == null)
                    {
                        prop.SetValue(obj, null);
                    }
                    else
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        var converted = Convert.ChangeType(value, targetType);
                        prop.SetValue(obj, converted);
                    }
                }
                catch { /* ignore conversion issues */ }
            }
        }




























        private int ApplyRatingsToFileList()
        {
            int rcount = 0;
            if (bThisIsSubWindow || gv.imageFileList1 == null || gv.imageFileList1.getImageFileListLength() <= 1)
                return rcount;

            string ratingsFile = null;
            if (gv.FILE_TYPE == "images")
                ratingsFile = gv.ratingsImages;
            else if (gv.FILE_TYPE == "videos")
                ratingsFile = gv.ratingsVideos;

            if (string.IsNullOrEmpty(ratingsFile) || !File.Exists(ratingsFile))
                return rcount;

            // Deserialize ratings file
            List<FileInfoItem> ratingsList;
            try
            {
                ratingsList = DeserializeFromXML(ratingsFile);
            }
            catch
            {
                return rcount;
            }

            if (ratingsList == null || ratingsList.Count == 0)
                return rcount;

            // Build a lookup for fast access
            var ratingsDict = ratingsList.ToDictionary(f => f.fpath, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < gv.imageFileList1.getImageFileListLength(); ++i)
            {
                var fileItem = gv.imageFileList1.getIndexed(i);
                if (fileItem == null || string.IsNullOrEmpty(fileItem.fpath))
                    continue;

                if (ratingsDict.TryGetValue(fileItem.fpath, out var ratingItem))
                {
                    // Only apply if rating is not 'g'
                    if (ratingItem.rating != 'g')
                    {
                        // Find the corresponding row in dgv1
                        for (int row = 0; row < dgvFileInfo.RowCount; ++row)
                        {
                            var rowPath = dgvFileInfo.Rows[row].Cells["fpath"].Value?.ToString();
                            if (string.Equals(rowPath, fileItem.fpath, StringComparison.OrdinalIgnoreCase))
                            {
                                dgvFileInfo.Rows[row].Cells["rating"].Value = ratingItem.rating;
                                dgvFileInfo.Rows[row].Cells["source"].Value = ratingItem.source;
                                dgvFileInfo.Rows[row].Cells["comment"].Value = ratingItem.comment;
                                rcount++;
                                break;
                            }
                        }
                    }
                }
            }
            return rcount;
        }
        private void btSetRatingsFiles_Click(object sender, EventArgs e)
        {
            //CopyUpdatedRatingsToRatingsFile();
            ProcessChangedRows();

            gv.mainWindow.soundAlert(1);
        }
        public void CopyCommentToSourceForAllRows()
        {
            if (dgvFileInfo.Rows.Count == 0)
                return;

            foreach (DataGridViewRow row in dgvFileInfo.Rows)
            {
                if (row.Cells["comment"] != null && row.Cells["source"] != null)
                {
                    row.Cells["source"].Value = row.Cells["comment"].Value;
                }
            }

            //  MessageBox.Show("All 'comment' values have been copied to 'source'.", "Operation Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void CopySourceForAllRows()
        {
            if (dgvFileInfo.Rows.Count == 0)
                return;

            foreach (DataGridViewRow row in dgvFileInfo.Rows)
            {
                if (row.Cells["source"] != null)
                {
                    row.Cells["comment"].Value = row.Cells["source"].Value;
                }
            }

            //  MessageBox.Show("All 'comment' values have been copied to 'source'.", "Operation Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void SetCommentToz()
        {
            if (dgvFileInfo.Rows.Count == 0)
                return;

            foreach (DataGridViewRow row in dgvFileInfo.Rows)
            {
                if (row.Cells["comment"] != null)
                {
                    row.Cells["comment"].Value = "x";
                }
            }
            //  MessageBox.Show("All 'comment' cells have been set to 'x'.", "Operation Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void FindPreviousRatingsxx()
        {
            if (!bThisIsSubWindow && gv.imageFileList1 != null && gv.imageFileList1.getImageFileListLength() > 1)
                ApplyRatingsToFileList();


        }
        string ratingDefault = "Z";
        private void AssignDefaultRatingToEmptyCells(string ratingDefault)
        {
            if (dgvFileInfo == null || dgvFileInfo.Rows.Count == 0)
                return;

            // Ensure the "rating" column exists
            var colRating = dgvFileInfo.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c => string.Equals(c.Name, "rating", StringComparison.OrdinalIgnoreCase));

            if (colRating == null)
            {
                MessageBox.Show("The 'rating' column does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Iterate through rows and assign default rating if empty
            foreach (DataGridViewRow row in dgvFileInfo.Rows)
            {
                if (row.IsNewRow) continue;

                var ratingCell = row.Cells[colRating.Index];
                if (ratingCell.Value == null || string.IsNullOrWhiteSpace(ratingCell.Value.ToString()))
                {
                    ratingCell.Value = ratingDefault;
                }
                else if (ratingCell.Value.ToString() == "~")
                {
                    ratingCell.Value = ratingDefault;
                }
            }

            //MessageBox.Show("Default rating assigned to empty cells.", "Operation Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void DBstatus2(string statusMessage)
        {

        }
        private void btGetRatings_Click(object sender, EventArgs e)
        {
            DBstatus($"get ratings from db");
            Refresh();
            var count = ApplyRatingsToDGV1FromDB();
            DBstatus($"Updated {count} rows from DB.");
            gv.mainWindow.soundAlert(1);
            //
            previousRowIdWas = -1;
            dgv1_SelectionChanged(null, null);

            AssignDefaultRatingToEmptyCells(ratingDefault);
            // Clear the HashSet after processing
            changedRows.Clear();

            return;

            int rc = ApplyRatingsToFileList();
            tbCountRatings.Text = rc.ToString();
            SetCommentToz();
            gv.mainWindow.soundAlert(6);
            previousRowIdWas = -1;
            dgv1_SelectionChanged(null, null);
        }

        private void btSortByRating_Click(object sender, EventArgs e) //<<<<<<<<<<<<<<<<<<<< SORT RATING
        {
            if (!bLoadedList)
                return;

            btTraversing.Visible = true;
            btTraversing.Update();
            ResetSortButtonColor(sender);
            SortByRating();
            previousRowIdWas = -1;
            dgv1_SelectionChanged(null, null);
            btTraversing.Visible = false;
        }
        public async void SortByRating()
        {
            if (!bLoadedList)
                return;

            // ADDED: Ensure metadata is updated before sorting
            await EnsureMetadataUpdatedAsync();

            doingSort = true;
            SetTraversingPopup("Sorting by Rating...");

            tbGoToSubFolder.Enabled = false;

            var sourceList = bThisIsSubWindow ? imageFileList2?.finfoList
                                              : imageFileList1?.finfoList;

            if (sourceList == null || sourceList.Count == 0)
                return;

            IEnumerable<FileInfoItem> query;

            // If cbDesc is checked, you’re using “comment/desc” instead of numeric rating
            if (cbDesc.Checked)
            {
                query = cbSortDescending.Checked
                    ? sourceList.OrderByDescending(f => f.comment)
                    : sourceList.OrderBy(f => f.comment);
            }
            else
            {
                query = cbSortDescending.Checked
                    ? sourceList.OrderByDescending(f => f.rating)
                    : sourceList.OrderBy(f => f.rating);
            }

            var ordered = query.ToList();

            dgvFileInfo.DataSource = null;
            dgvFileInfo.DataSource = ordered;

            if (bThisIsSubWindow)
                imageFileList2.copyFileList(ordered);
            else
                imageFileList1.copyFileList(ordered);
            SortCompleted();
        }

        private void btCopyCommentToSource_Click(object sender, EventArgs e)
        {
            CopyCommentToSourceForAllRows();
            // Clear the HashSet after processing
            changedRows.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CopySourceForAllRows();
        }

        private void cbSearchPartial_CheckedChanged(object sender, EventArgs e)
        {
            // cbSearchPartial
        }

        private void cbFocusIsOnThisWindow_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tbTargetFolder_MouseLeave(object sender, EventArgs e)
        {
            cbSpecialFolder.Checked = IsSpecialFolderSet(tbTargetFolder.Text);
        }
        Display3AI display3;
        private void btDisplay3A_Click(object sender, EventArgs e)
        {
            display3 = new Display3AI(gv);
            display3.Show();
            if (bThisIsSubWindow)
                display3.SetImageList(imageFileList2);
            else
                display3.SetImageList(imageFileList1);
        }

        private void cbHTML_CheckedChanged(object sender, EventArgs e)
        {
            _usePictures = false;
            bDisplayHTML = false;
        }

        private void cbMIDI_CheckedChanged(object sender, EventArgs e)
        {

        }

        bool bDisplayHTML = false;
        private void btDisplayHTML_Click(object sender, EventArgs e)
        {
            bDisplayHTML = false;
            btDisplayHTML.BackColor = Color.LightGray;
            if (!_useHTML)
                return;
            btDisplayHTML.BackColor = Color.LightGreen;
            bDisplayHTML = true;
            DisplaySelectedHTMLFile();
        }
        public void DisplaySelectedHTMLFile()
        {
            // Display the currently selected HTML file from dgv1 in Main's WebBrowser.
            // Preconditions: a row is selected; file has .html/.htm extension; file exists.

            if (dgvFileInfo == null || dgvFileInfo.CurrentRow == null)
                return;

            // Retrieve fpath from the bound column (case-insensitive lookup).
            DataGridViewColumn colFpath = dgvFileInfo.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c => string.Equals(c.Name, "fpath", StringComparison.OrdinalIgnoreCase));

            if (colFpath == null)
                return;

            var rawPath = dgvFileInfo.CurrentRow.Cells[colFpath.Index].Value?.ToString();
            if (string.IsNullOrWhiteSpace(rawPath))
                return;

            string fullPath = rawPath.Trim();
            string ext = Path.GetExtension(fullPath)?.ToLowerInvariant();

            if (ext != ".html" && ext != ".htm")
            {
                // Not an HTML file; ignore.
                return;
            }

            if (!File.Exists(fullPath))
            {
                MessageBox.Show($"File not found:\n{fullPath}", "Display HTML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (bUseWebView2)
            {
                gv.mainWindow.ShowBrowserAndDisplayLocalFile(fullPath);

                return;
            }
            // Prefer main.browser if accessible; fallback to gv.browser; else open externally.
            try
            {
                // Ensure we have a file:// URI (WebBrowser.Navigate can take a local path, but normalizing is safer).
                string uri = new Uri(fullPath).AbsoluteUri;

                if (main != null)
                {
                    var browserField = main.GetType().GetField("browser",
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic);

                    if (browserField?.GetValue(main) is WebBrowser wbMain)
                    {
                        wbMain.Navigate(uri);
                        return;
                    }
                }

                if (gv?.browser != null)
                {
                    gv.browser.Navigate(uri);
                    return;
                }

                // Fallback: launch in default system browser.
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = fullPath,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = uri,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to display HTML:\n{ex.Message}", "Display HTML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        bool bUseWebView2 = true;
        private void cbWebView2_CheckedChanged(object sender, EventArgs e)
        {
            bUseWebView2 = cbWebView2.Checked;
        }

        private async void btSortByFileType_Click(object sender, EventArgs e)
        {
            if (!bLoadedList)
                return;

            // ADDED: Ensure metadata is updated before sorting
            await EnsureMetadataUpdatedAsync();

            doingSort = true;
            SetTraversingPopup("Sorting by Date...");
            ResetSortButtonColor(sender);

            dgvFileInfo.DataSource = null;
            if (cbSortDescending.Checked)
                orderedByFileType = imageFileList1.finfoList.OrderByDescending(file => file.ext).ToList();
            else
                orderedByFileType = imageFileList1.finfoList.OrderBy(file => file.ext).ToList();

            dgvFileInfo.DataSource = orderedByFileType;
            previousRowIdWas = -1;
            dgv1_SelectionChanged(null, null);

            //resizeFileListDataGrid();
            imageFileList1.copyFileList(orderedByFileType); //ifl 2022 replaced
            btTraversing.Visible = false;
            SortCompleted();
        }
        private void xxxxxcbSortByDate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // Preserve current selection
                string previous = cmboSourceFolder.SelectedItem?.ToString();

                // Build distinct folder path list from history
                var paths = gv.folderHistoryList
                              .Select(fh => fh.folderPath)
                              .Where(p => !string.IsNullOrWhiteSpace(p))
                              .Distinct(StringComparer.OrdinalIgnoreCase)
                              .ToList();

                IEnumerable<string> ordered;
                if (true)
                {
                    // Order by folder last-write time (descending). Non-existent folders go last.
                    ordered = paths.OrderByDescending(p =>
                    {
                        try
                        {
                            return Directory.Exists(p)
                                ? File.GetLastWriteTimeUtc(p)
                                : DateTime.MinValue;
                        }
                        catch
                        {
                            return DateTime.MinValue;
                        }
                    });
                }
                else
                {
                    // Alphabetical order by path
                    ordered = paths.OrderBy(p => p, StringComparer.OrdinalIgnoreCase);
                }

                cmboSourceFolder.BeginUpdate();
                cmboSourceFolder.Items.Clear();
                foreach (var p in ordered)
                    cmboSourceFolder.Items.Add(p);

                // Restore previous selection when possible, otherwise select first item
                if (cmboSourceFolder.Items.Count > 0)
                {
                    if (!string.IsNullOrEmpty(previous))
                    {
                        int idx = cmboSourceFolder.FindStringExact(previous);
                        cmboSourceFolder.SelectedIndex = idx >= 0 ? idx : 0;
                    }
                    else
                    {
                        cmboSourceFolder.SelectedIndex = 0;
                    }
                }
                cmboSourceFolder.EndUpdate();
            }
            catch (Exception ex)
            {
                // best-effort: log debug if available
                try { gv?.debug?.w("cbSortByDate_CheckedChanged error", ex.Message); } catch { }
            }
        }

        private void tbGoToSubFolder_MouseEnter(object sender, EventArgs e)
        {
            cbFocusHere.Checked = false;
        }

        private void btShowFolder_Click(object sender, EventArgs e)
        {
            string path = tbFolderPath.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("No folder specified.", "Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(path))
            {
                MessageBox.Show($"Folder not found:\n{path}", "Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Let the shell open the folder (works even if path contains spaces)
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open folder:\n{ex.Message}", "Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbFileNameOfSelected_TextChanged(object sender, EventArgs e)
        {

        }
        public void SetStatusMessage(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetStatusMessage), msg);
                return;
            }

            try
            {
                // Always update main status display
                tbPlayerState.Text = msg;
                if (!msg.Equals(tbStatus1))
                {
                    // Optional: Update status history (rolling display)
                    tbStatus5 = tbStatus4;
                    tbStatus4 = tbStatus3;
                    tbStatus3 = tbStatus2;
                    tbStatus2 = tbStatus1;
                    tbStatus1 = msg;
                }
            }
            catch { }

            // Route to specialized handlers using switch expression (C# 8+)
            switch (msg?.ToUpperInvariant())
            {
                case "ENDING":
                    endOfSteamMessage();
                    break;

                case "OPENING": // STARTING":
                    startStreamMessage();
                    break;

                case "STOPPED":
                    stoppedStreamMessage();
                    break;

                case "PAUSED":
                    pausedStreamMessage();
                    break;

                case "PLAYING":
                    playingStreamMessage();
                    break;
                case "MediaChanged":
                    MessageBox.Show("Media changed event received.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "ERROR":
                    // New handler for errors
                    tbPlayerState.BackColor = Color.Red;
                    break;

                case "BUFFERING":
                    tbPlayerState.BackColor = Color.Yellow;
                    break;

                default:
                    // Generic message - just display it
                    tbPlayerState.BackColor = Color.LightGray;
                    break;
            }
        }
        public void SetStatusMessageWas(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetStatusMessage), msg);
                return;
            }

            try
            {
                tbPlayerState.Text = msg;   // or whatever label you use
            }
            catch { }

            if (msg.Equals("ENDING", StringComparison.OrdinalIgnoreCase))
            {
                endOfSteamMessage();
            }
            else if (msg.Equals("STARTING", StringComparison.OrdinalIgnoreCase))
            {
                startStreamMessage();
            }
            else if (msg.Equals("STOPPED", StringComparison.OrdinalIgnoreCase))
            {
                stoppedStreamMessage();
            }
            else if (msg.Equals("PAUSED", StringComparison.OrdinalIgnoreCase))
            {
                pausedStreamMessage();
            }
            else if (msg.Equals("PLAYING", StringComparison.OrdinalIgnoreCase))
            {
                playingStreamMessage();
            }

        }

        private async void btUpdateMetadata_Click(object sender, EventArgs e)
        {
            await UpdateMetadata();
        }
        // Modify the UpdateMetadata method to set the flag
        public async Task UpdateMetadata()
        {
            SetTraversingPopup("Loading metadata ...");
            await UpdateMetadataAfterScanAsync_Fast();
            dgvFileInfo.Refresh();
            SetTraversingPopup("Metadata updated.");

            // ADDED: Mark metadata as updated
            _metadataUpdatedAfterTraversal = true;
            btUpdateMetadata.BackColor = Color.LightGreen;
            btTraversing.Visible = false;
        }

        // Add a helper method to check and run metadata if needed
        private async Task EnsureMetadataUpdatedAsync()
        {
            if (!_metadataUpdatedAfterTraversal && !bThisIsSubWindow)
            {
                tbStatus.Text = "Updating metadata before sort...";
                await UpdateMetadata();
            }
        }


        public async Task RefreshMetadataAsync()
        {
            // Capture references
            var list = gv.imageFileList1.finfoList;
            var grid = dgvFileInfo;

            if (list == null || list.Count == 0)
                return;

            // Temporarily unbind the grid so changes don't trigger 42k redraws
            object oldDataSource = null;

            if (grid.InvokeRequired)
            {
                grid.Invoke(new Action(() =>
                {
                    oldDataSource = grid.DataSource;
                    grid.DataSource = null;
                }));
            }
            else
            {
                oldDataSource = grid.DataSource;
                grid.DataSource = null;
            }

            // Run file-system work on a background thread
            await Task.Run(() =>
            {
                foreach (var item in list)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(item.fpath))
                            continue;

                        var fi = new FileInfo(item.fpath);
                        if (!fi.Exists)
                            continue;

                        item.len = fi.Length;
                        item.stimestamp = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                        // if you want the hidden ts to be useful, make it public or
                        // add a setter method and update it here too.
                    }
                    catch
                    {
                        // Ignore missing/inaccessible files
                    }
                }
            });

            // Rebind once, on UI thread
            if (grid.InvokeRequired)
            {
                grid.Invoke(new Action(() =>
                {
                    grid.SuspendLayout();
                    grid.DataSource = null;          // force refresh
                    grid.DataSource = oldDataSource; // usually finfoList
                    grid.ResumeLayout();
                }));
            }
            else
            {
                grid.SuspendLayout();
                grid.DataSource = null;
                grid.DataSource = oldDataSource;
                grid.ResumeLayout();
            }
        }
        private void btAutoSizeDGVcolumns_Checked(object sender, EventArgs e)
        {
            AutoSizeDGVColumns();
        }
        private void cbAutoSizeDGVcolumns_CheckedChanged(object sender, EventArgs e)
        {
            if (bAutoSizeDGVcolumns)
            {
                AutoSizeDGVColumns();
            }
            else
            {
                dgvFileInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
        }

        private void sliderSlowMotion_Scroll(object sender, EventArgs e)
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;

            TrackBar tb = (TrackBar)sender;

            int factor = tb.Value;           // 1–10
            float rate = 1.0f / factor;      // convert to playback rate

            vlcPlayer.SetSpeed(rate);
            // tbSlowMotionLabel.Text = $"1/{factor}x"; // optional label
        }

        private void cbAutosize_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAutosize.Checked)
            {
                AutoSizeDGVColumns();
                gv.mainWindow.soundAlert(5);
            }
            else
            {

            }
        }

        private void tbExt_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbSpeedControl_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSpeedControl.Checked)
            {
                sliderSlowMotion.Enabled = true;
                sliderSlowMotion.Visible = true;
                sliderSlowMotion.BringToFront();
            }
            else
            {
                sliderSlowMotion.Value = sliderSlowMotion.Maximum / 2; // reset slider position
                if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                {
                    ResumeNormalSpeed();
                }
                sliderSlowMotion.Enabled = false;
                sliderSlowMotion.SendToBack();
                sliderSlowMotion.Visible = false;
            }
        }

        private void sliderSlowMotion_ValueChanged(object sender, EventArgs e)
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;

            if (true) // cbFastForward.Checked)
            {
                float speed = sliderSlowMotion.Value;
                //sliderFastForward.Visible = true;
                //lblFastForwardSpeed.Visible = true;
                //sliderFastForward.Value = 3;
                vlcPlayer.SetSpeed(speed);
            }
            else
            {
                //vlcPlayer.SetSpeed(1.0f);
                //sliderFastForward.Visible = false;
                // lblFastForwardSpeed.Visible = false;
            }
        }
        private void xxxxxxcbUseSearchWin3_CheckedChanged(object sender, EventArgs e)
        {
            if (bUseSearchWin3)
            {
                if (searchWin3 == null || searchWin3.IsDisposed)
                {
                    searchWin3 = new SearchForMediaByName(gv, gv.mainWindow, this, ff, tbTargetFolder.Text);
                    searchWin3.TraverseFolders(); // This will run synchronously on UI thread
                }
                else
                    searchWin3.SearchForFile(tbFileNameOfSelected.Text);

                searchWin3.Show();
                searchWin3.Activate();
                searchWin3.BringToFront();
            }
        }

        public bool bUseSearchWin3 = false;
        private void cbUseSearchWin3_CheckedChanged(object sender, EventArgs e)
        {
            if (!bUseSearchWin3)
                return;

            try
            {
                // Ensure we're on the UI thread
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() => cbUseSearchWin3_CheckedChanged(sender, e)));
                    return;
                }

                if (searchWin3 == null || searchWin3.IsDisposed)
                {
                    // Create the window on UI thread
                    searchWin3 = new SearchForMediaByName(gv, gv.mainWindow, this, ff, tbTargetFolder.Text);

                    // Show a progress indication
                    SetTraversingPopup("Initializing search window...");

                    // Run the heavy TraverseFolders operation on background thread
                    Task.Run(() =>
                    {
                        try
                        {
                            searchWin3.TraverseFolders();

                            // Marshal back to UI thread for window operations
                            if (!searchWin3.IsDisposed)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    try
                                    {
                                        if (!searchWin3.IsDisposed)
                                        {
                                            searchWin3.Show();
                                            searchWin3.Activate();
                                            searchWin3.BringToFront();
                                        }
                                    }
                                    finally
                                    {
                                        btTraversing.Visible = false; // Hide progress indicator
                                    }
                                }));
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle errors on UI thread
                            BeginInvoke(new Action(() =>
                            {
                                btTraversing.Visible = false;
                                MessageBox.Show($"Error traversing folders: {ex.Message}", "Search Window Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                bUseSearchWin3 = false; // Uncheck if failed
                            }));
                        }
                    });
                }
                else
                {
                    // Window already exists - just search and show it
                    searchWin3.SearchForFile(tbFileNameOfSelected.Text);
                    searchWin3.Show();
                    searchWin3.Activate();
                    searchWin3.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening search window: {ex.Message}", "Search Window Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                bUseSearchWin3 = false;
            }
        }
        private void mmcbUseSearchWin3_CheckedChanged(object sender, EventArgs e)
        {
            if (bUseSearchWin3)
            {
                try
                {
                    // Ensure we're on the UI thread
                    if (InvokeRequired)
                    {
                        BeginInvoke(new Action(() => cbUseSearchWin3_CheckedChanged(sender, e)));
                        return;
                    }

                    if (searchWin3 == null || searchWin3.IsDisposed)
                    {
                        searchWin3 = new SearchForMediaByName(gv, gv.mainWindow, this, ff, tbTargetFolder.Text);

                        // Run TraverseFolders on a background thread to avoid blocking UI
                        Task.Run(() =>
                        {
                            try
                            {
                                searchWin3.TraverseFolders();

                                // Show the window on the UI thread
                                BeginInvoke(new Action(() =>
                                {
                                    if (!searchWin3.IsDisposed)
                                    {
                                        searchWin3.Show();
                                        searchWin3.Activate();
                                        searchWin3.BringToFront();
                                    }
                                }));
                            }
                            catch (Exception ex)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    MessageBox.Show($"Error traversing folders: {ex.Message}", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }));
                            }
                        });
                    }
                    else
                    {
                        searchWin3.SearchForFile(tbFileNameOfSelected.Text);
                        searchWin3.Show();
                        searchWin3.Activate();
                        searchWin3.BringToFront();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening search window: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void xxbUseSearchWin3_CheckedChanged(object sender, EventArgs e)
        {
            if (bUseSearchWin3)
            {
                if (searchWin3 == null || searchWin3.IsDisposed)
                {
                    searchWin3 = new SearchForMediaByName(gv, gv.mainWindow, this, ff, tbTargetFolder.Text);
                    searchWin3.TraverseFolders();
                }
                else
                    searchWin3.SearchForFile(tbFileNameOfSelected.Text);

                searchWin3.Show();
                searchWin3.Activate();
                searchWin3.BringToFront();
            }
        }

        private void btTargetFolder_Click(object sender, EventArgs e)
        {
            bool b = Directory.Exists(tbTargetFolder.Text);
            if (!b)
            {
                MessageBox.Show("Target folder does not exist.");
            }
            else
                MessageBox.Show("Target folder exists.");

        }

        private void button9_Click(object sender, EventArgs e)
        {
            // tbTargetFreeSpace
        }

        private void btDownMoveWindow_Click(object sender, EventArgs e)
        {
            // Move the window down by 10 pixels
            this.Location = new Point(this.Location.X, this.Location.Y + 100);

            // Make the window resizable
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
        }
        SearchUntilNotFound searchWinUntil;
        private void btOpenSearchUntilNotFound_Click(object sender, EventArgs e)
        {
            if (fileType.Equals("videos"))
            {
                searchWinUntil = new SearchUntilNotFound(gv, this);
                searchWinUntil.Show();
                searchWinUntil.BringToFront();
                searchWinUntil.Activate();
            }
        }
        //2026 added for source/target superset display

        private SourceTargetSupersetForm? sourceTargetSupersetForm;

        private void btDisplayList_Click(object sender, EventArgs e)
        {
            if (gv.imageFileList1 == null || gv.imageFileList1.getImageCount() == 0)
            {
                MessageBox.Show("Source list is empty (gv.imageFileList1).", "Superset",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (gv.imageFileList2 == null || gv.imageFileList2.getImageCount() == 0)
            {
                MessageBox.Show("Target list is empty (gv.imageFileList2).", "Superset",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var items = BuildSourceTargetSuperset();

            if (sourceTargetSupersetForm == null || sourceTargetSupersetForm.IsDisposed)
                sourceTargetSupersetForm = new SourceTargetSupersetForm(ff, tbDirectoryPath.Text, tbTargetFolder.Text);

            sourceTargetSupersetForm.SetItems(items);
            sourceTargetSupersetForm.Show();
            sourceTargetSupersetForm.BringToFront();
            sourceTargetSupersetForm.Activate();
        }

        private List<SourceTargetEntry> BuildSourceTargetSuperset()
        {
            string sourceRoot = tbDirectoryPath.Text;
            string targetRoot = tbTargetFolder.Text;

            var sourceItems = gv.imageFileList1?.finfoList ?? new List<FileInfoItem>();
            var targetItems = gv.imageFileList2?.finfoList ?? new List<FileInfoItem>();

            var sourceMap = new Dictionary<string, FileInfoItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in sourceItems)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.fpath)) continue;
                string key = BuildRootRelativePath(sourceRoot, item.fpath);
                if (!string.IsNullOrWhiteSpace(key))
                    sourceMap[key] = item;
            }

            var targetMap = new Dictionary<string, FileInfoItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in targetItems)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.fpath)) continue;
                string key = BuildRootRelativePath(targetRoot, item.fpath);
                if (!string.IsNullOrWhiteSpace(key))
                    targetMap[key] = item;
            }

            var allKeys = new HashSet<string>(sourceMap.Keys, StringComparer.OrdinalIgnoreCase);
            allKeys.UnionWith(targetMap.Keys);

            var results = new List<SourceTargetEntry>(allKeys.Count);
            foreach (var key in allKeys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
            {
                bool inSource = sourceMap.TryGetValue(key, out var sItem);
                bool inTarget = targetMap.TryGetValue(key, out var tItem);

                string status = inSource && inTarget ? "both" : inSource ? "source" : "target";

                results.Add(new SourceTargetEntry
                {
                    RootRelativePath = key,
                    Status = status,
                    SourcePath = inSource ? sItem.fpath : "",
                    TargetPath = inTarget ? tItem.fpath : "",
                    CopyToTarget = false,
                    CopyToSource = false,
                    DeleteSource = false,
                    DeleteTarget = false
                });
            }

            return results;
        }
        //
        // returns gv.imageFileList
        //
        public ARGS BuildTraversalArgsFromUi()
        {
            // Use gv.searchExtensionsInUse to determine file types
            string searchType = gv.searchExtensionCategoryInUse ?? "images";

            bool pictures = searchType.Equals("images", StringComparison.OrdinalIgnoreCase);
            bool movies = searchType.Equals("videos", StringComparison.OrdinalIgnoreCase) ||
                          searchType.Equals("videos_webm", StringComparison.OrdinalIgnoreCase);
            bool webm = searchType.Equals("videos_webm", StringComparison.OrdinalIgnoreCase);
            bool midi = searchType.Equals("midi", StringComparison.OrdinalIgnoreCase);
            bool html = searchType.Equals("html", StringComparison.OrdinalIgnoreCase);
            bool all = searchType.Equals("all", StringComparison.OrdinalIgnoreCase);

            // Default to images if nothing selected
            if (!pictures && !movies && !midi && !all && !html)
            {
                pictures = true;
                gv.searchExtensionCategoryInUse = "images";
                searchType = "images";
            }

            // Get the actual extension patterns from the dictionary
            string[] extensions = null;
            if (gv.SearchExtensions != null && gv.SearchExtensions.ContainsKey(searchType))
            {
                extensions = gv.SearchExtensions[searchType];
            }

            // Fallback to default extensions if not found
            if (extensions == null || extensions.Length == 0)
            {
                if (pictures)
                    extensions = new[] { "*.jpg", "*.png", "*.bmp", "*.jpeg" };
                else if (movies)
                    extensions = new[] { "*.mp4", "*.wmv", "*.mov" };
                else if (midi)
                    extensions = new[] { "*.mid", "*.mpe" };
                else if (html)
                    extensions = new[] { "*.htm*", "*.html", "*.mhtml" };
                else if (all)
                    extensions = new[] { "*.*" };
                else
                    extensions = new[] { "*.jpg", "*.png", "*.bmp", "*.jpeg" };
            }

            return new ARGS
            {
                dirpath = tbDirectoryPath.Text,
                bPictures = pictures,
                bMovies = movies,
                bWEBM = webm,
                bMIDI = midi,
                bALL = all,
                bHTML = html,
                searchExtensionCategory = searchType,  // Category name(dictionary key)
                extensions = extensions  // Actual extension patterns array
            };
        }
        private static string BuildRootRelativePath(string root, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(root))
                    return fullPath;

                string rootFull = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root.Trim()));
                string fileFull = Path.GetFullPath(fullPath.Trim());

                if (fileFull.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
                {
                    return Path.GetRelativePath(rootFull, fileFull);
                }
            }
            catch
            {
                // fall through
            }

            return fullPath;
        }

        private void ShowSearchExtensionsExample()
        {
            // Example: Get the values for "videos"
            string searchExtensionName = "videos";

            // Call the helper function from Globals.cs
            (string joinedExtensions, List<string> extensionList) = GlobalVars.SearchExtensionsHelper.GetSearchExtensionValues(gv, searchExtensionName);

            // Display the results
            MessageBox.Show($"Joined Extensions: {joinedExtensions}", "Search Extensions");

            Console.WriteLine("Extension List:");
            foreach (string ext in extensionList)
            {
                Console.WriteLine(ext);
            }
        }

        public string SelectSearchExtension()
        {
            string selected = SearchExtensionsDialog.ShowSearchExtensionsDialog(gv);
            if (!string.IsNullOrEmpty(selected))
            {
                MessageBox.Show($"You selected: {selected}", "Selected Search Extension");
                return selected;
            }
            else
            {
                MessageBox.Show("No selection made.", "Selected Search Extension");
                return null;
            }
        }
        private void btSelectSearchExtentions_Click(object sender, EventArgs e)
        {
            string selectedExtension = SelectSearchExtension();
            if (selectedExtension != null)
                tbSearchExtensions.Text = selectedExtension ?? string.Empty;
            else
                tbSearchExtensions.Text = "NONE";
            gv.searchExtensionCategoryInUse = selectedExtension;
        }
        string joinedExtensions;
        private void btGetSearchExtentions_Click(object sender, EventArgs e)
        {
            string searchExtensionName = "videos";
            (joinedExtensions, List<string> extensionList) = SearchExtensionsHelper.GetSearchExtensionValues(gv, searchExtensionName);
        }

        private void DialogTraverser_Load(object sender, EventArgs e)
        {
            ResizeDgv1ToFillForm();
        }

        private void cbSearchWin2EachSelection_CheckedChanged(object sender, EventArgs e)
        {
            if (searchWin3 == null || searchWin3.IsDisposed)
                btSearchMediaByName_Click(sender, e);
        }

        private void tbSearchExtensionsInUse_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbRotatePlayback_CheckedChanged(object sender, EventArgs e)
        {
            if (vlcPlayer == null) return;

            if (cbRotatePlayback.Checked)
                vlcPlayer.SetRotation("270");   // -90°
            else
                vlcPlayer.ClearRotation();

            vlcPlayer.ApplyRotationAndResume(); // stop → reinit → reload → seek back
        }

        private void btSaveRotatedMp4_Click(object sender, EventArgs e)
        {
            if (vlcPlayer != null)
                vlcPlayer.Stop();

            // ── 1. Get the source file from the currently selected DGV row ──────────
            string src = GetFilePath();
            if (string.IsNullOrWhiteSpace(src) || !File.Exists(src))
            {
                SetStatusMessage("No file selected in list");
                return;
            }

            // ── 2. Build output path ─────────────────────────────────────────────────
            string dir = Path.GetDirectoryName(src);
            string name = Path.GetFileNameWithoutExtension(src);
            string output = Path.Combine(dir, $"{name}_rot270.mp4");

            // ── 3. Confirm overwrite ─────────────────────────────────────────────────
            if (File.Exists(output))
            {
                if (MessageBox.Show($"Overwrite?\n{output}", "Confirm",
                        MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
                File.Delete(output);
            }

            // ── 4. Stop playback (do not pause) ──────────────────────────────────────
            if (vlcPlayer != null)
                vlcPlayer.Stop();

            SetStatusMessage("Saving rotated video...");

            // ── 5. Transcode ─────────────────────────────────────────────────────────
            vlcPlayer?.SaveRotatedVideo(
                inputPath: src,
                outputPath: output,
                transformType: "270",
                onProgress: pct => this.Invoke(() => SetStatusMessage($"Saving... {pct:F0}%")),
                onComplete: ok => this.Invoke(() =>
                {
                    if (ok)
                        SetStatusMessage($"Saved: {Path.GetFileName(output)}");
                    else
                        SetStatusMessage("Save FAILED — see error dialog");
                })
            );
        }


        public class SearchExtensionsDialog
        {
            public static string ShowSearchExtensionsDialog(GlobalVars globals)
            {
                // Create a new form
                using (Form dialog = new Form())
                {
                    dialog.Text = "Select Search Extension";
                    dialog.Width = 400;
                    dialog.Height = 300;
                    dialog.StartPosition = FormStartPosition.CenterParent;

                    // Create a ListBox to display the options
                    ListBox listBox = new ListBox
                    {
                        Dock = DockStyle.Fill
                    };

                    // Add the keys from SearchExtensions to the ListBox
                    foreach (var key in globals.SearchExtensions.Keys)
                    {
                        listBox.Items.Add(key);
                    }

                    // Add the ListBox to the form
                    dialog.Controls.Add(listBox);

                    // Add OK and Cancel buttons
                    Button okButton = new Button
                    {
                        Text = "OK",
                        DialogResult = DialogResult.OK,
                        Dock = DockStyle.Bottom
                    };
                    dialog.Controls.Add(okButton);

                    Button cancelButton = new Button
                    {
                        Text = "Cancel",
                        DialogResult = DialogResult.Cancel,
                        Dock = DockStyle.Bottom
                    };
                    dialog.Controls.Add(cancelButton);

                    // Show the dialog and return the selected value
                    if (dialog.ShowDialog() == DialogResult.OK && listBox.SelectedItem != null)
                    {
                        return listBox.SelectedItem.ToString();
                    }

                    return null; // Return null if no selection is made
                }
            }
        }
        public static class PlayerMessages
        {
            public const string ENDING = "ENDING";
            public const string STARTING = "STARTING";
            public const string STOPPED = "STOPPED";
            public const string PAUSED = "PAUSED";
            public const string PLAYING = "PLAYING";
            public const string ERROR = "ERROR";
            public const string BUFFERING = "BUFFERING";
        }
        /// <summary>
        /// Called by the Player after a WebP file has been successfully converted to PNG.
        /// </summary>
        /// <param name="originalWebpPath">The source .webp file path.</param>
        /// <param name="savedPngPath">The output .png file saved in targetFolder\Temp\</param>
        /// <summary>
        /// Called by the Player after a WebP file has been successfully converted to PNG.
        /// Always invoked on the UI thread (Player marshals via this.Invoke).
        /// </summary>
        /// <param name="originalWebpPath">The source .webp file path.</param>
        /// <param name="savedPngPath">The output .png file saved in targetFolder\Temp\</param>
        public void OnWebpConverted(string originalWebpPath, string savedPngPath)
        {
            if (string.IsNullOrEmpty(savedPngPath) || !File.Exists(savedPngPath))
            {
                tbStatus.Text = $"WebP conversion failed: no PNG produced for {Path.GetFileName(originalWebpPath)}";
                return;
            }

            try
            {
                // Load the PNG into a new bitmap before disposing the old one
                // to avoid a blank flash if the load fails
                var newBmp = new Bitmap(savedPngPath);
                var old = pbThumbNail.Image;
                pbThumbNail.SizeMode = PictureBoxSizeMode.Zoom;
                pbThumbNail.Image = newBmp;
                pbThumbNail.BringToFront();
                pbThumbNail.Refresh();
                old?.Dispose();

                tbStatus.Text = $"WebP → PNG: {savedPngPath}";
            }
            catch (Exception ex)
            {
                tbStatus.Text = $"WebP display error: {ex.Message}";
            }
        }
        private void cbConvertWebpTemp_CheckedChanged(object sender, EventArgs e)
        {
            gv.webpConversionMode = cbConvertWebpTemp.Checked;
        }

        private void btPlaybackControl_Click(object sender, EventArgs e)
        {
            ShowPlaybackForm();          // ← ADD THIS LINE
        }

        private void btnEditSearchExtensions_Click(object sender, EventArgs e)
        {
            DialogSearchExtensionsEditor.Open(gv, this);

            // Reload combo in case categories were added / removed / renamed
            PopulateSearchExtensionsComboBox();
        }
    }
}