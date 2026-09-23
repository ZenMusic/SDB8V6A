using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SymbolDB.DialogTraverser;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace SymbolDB
{
    public partial class SearchForMediaByName : Form
    {
        // TextBox to display found search list
        private TextBox? tbFoundSeachList2b;
        GlobalVars gv;
        Main? main = null;
        DialogTraverser _parentTraverser;
        public bool bThisIsSubWindow = true;
        FileFunctions ff;
        ImageFileList? imageFileList2;  // result of transversal 
        private System.Windows.Forms.Timer _timer2s;

        public enum ActionTraversal
        {
            TRAVERSING, LOADING_DISPLAY, NONE, COMPLETED
        }

        private ActionTraversal action = ActionTraversal.NONE;
        public string? fullpath;

        public SearchForMediaByName(GlobalVars g, Main? mainParent, DialogTraverser parentTrav, FileFunctions f, string? searchFolder = null)
        {
            gv = g;
            main = mainParent;

            if (parentTrav == null)
            {
                MessageBox.Show("NO T PARENT");
                throw new ArgumentNullException(nameof(parentTrav));
            }
            ff = f;
            InitializeComponent();

            // 🔹 ADD THIS LINE: Configure DataGridView to prevent extra rows
            ConfigureDataGridView();

            // 🔹 Keep dgv1 flush with the bottom of the window
            AdjustGridHeight();
            this.Resize += (_, _) => AdjustGridHeight();

            btTraversing.Visible = false;
            this.StartPosition = FormStartPosition.Manual;

            bThisIsSubWindow = true;
            tbMainOrSubWindow.Text = "Sub Window";
            _parentTraverser = parentTrav;
            btSearchWin2.Text = "Search Main";
            btClose.Text = "Close List2";

            // Set directory path safely
            string dirPath = !string.IsNullOrEmpty(searchFolder)
                ? searchFolder
                : gv.initParm1List?[0]?.mostRecentSubfolder ?? gv.initParm1List?[0]?.targetDir1 ?? @"C:\";

            if (string.IsNullOrEmpty(dirPath) || !Directory.Exists(dirPath))
            {
                // Don't show error immediately - let user set directory manually
                dirPath = @"C:\"; // Default fallback
            }

            tbDirectoryPath.Text = dirPath;
            gv.searchWin3 = this;
            Reposition3(_parentTraverser);
            this.Activate();
            this.Show();
            // At end of constructor, before closing brace (line 82):
            _timer2s = new System.Windows.Forms.Timer { Interval = 2000 };
            _timer2s.Tick += Timer2s_Tick;

            // automatically traverse - don't search immediately, let user adjust path if needed
        }

        private void AdjustGridHeight()
        {
            int newHeight = ClientSize.Height - dgv1.Top;
            if (newHeight < 0)
                newHeight = 0;

            dgv1.Height = newHeight;
        }

        // Check if we're on the UI thread and invoke if necessary
        private void SafeInvoke(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        // SYNCHRONOUS TRAVERSAL METHODS

        public void TraverseFolders()
        {
            SafeInvoke(() =>
            {
                cbLoadedAndReady.Checked = false;
                tbMovieFpath.Text = "";
                btTraversing.Visible = true;
                btTraversing.Text = "Traversing...";
                btTraversing.Refresh();
            });

            if (_parentTraverser == null)
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show("NO tParent TraverserParent");
                    btTraversing.Visible = false;
                });
                return;
            }

            try
            {
                // Use TraverserBG5 instead of synchronous enumeration
                if (!TraverseGoAsync(_parentTraverser))
                {
                    SafeInvoke(() =>
                    {
                        btTraversing.Visible = false;
                        tbMovieFpath.Text = "!! ERROR Invalid directory !!";
                        MessageBox.Show("!! ERROR Invalid directory !!");
                    });
                    return;
                }
            }
            catch (Exception ex)
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show($"Error during traversal: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btTraversing.Visible = false;
                });
                gv.setCursorDefault();
            }
        }

        public bool TraverseGoAsync(DialogTraverser parentTrav)
        {
            SafeInvoke(() =>
            {
                dgv1.DataSource = null;
            });

            if (!Directory.Exists(tbDirectoryPath.Text))
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show("Invalid directory: " + tbDirectoryPath.Text);
                });
                return false;
            }

            _parentTraverser = parentTrav;
            gv.setCursorHourGlass();

            // Use TraverserBG5 for high-performance traversal
            DoTraversalAsync(tbDirectoryPath.Text, _parentTraverser);
            return true;
        }

        private void DoTraversalAsync(string dpath, DialogTraverser dt)
        {
            if (string.IsNullOrEmpty(dpath))
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show($"Invalid path: {dpath}", "ERROR");
                });
                return;
            }

            // Initialize state
            if (bThisIsSubWindow)
            {
                gv.initParm1List[0].mostRecentSubfolder = dpath;
                gv.imageFileList2 = new ImageFileList();
                gv.bLoadingTraverser2 = true;
            }
            else
            {
                gv.initParm1List[0].mostRecent = dpath;
                gv.lastTraversedFolder = dpath;
                gv.imageFileList1 = new ImageFileList();
                gv.deleteFileList1 = new ImageFileList();
                gv.bLoadingTraverser2 = false;
            }

            SafeInvoke(() =>
            {
                bLoadedList = true;
                tbDirectoryPath.Text = dpath;
                tbCopyFileName.Text = "";
                action = ActionTraversal.TRAVERSING;
            });

            // 🔹 FIX: Use BuildTraversalArgsFromParent instead of hardcoding bMovies
            TraverserBG5 traverser = new TraverserBG5(gv, null, this);
            ARGS args = BuildTraversalArgsFromParent();
            args.dirpath = dpath;

            traverser.StartTraversal(args, bThisIsSubWindow, dpath);
        }

        // New synchronous traversal method
        public bool TraverseGoSync(DialogTraverser parentTrav)
        {
            SafeInvoke(() =>
            {
                dgv1.DataSource = null;
            });

            if (!Directory.Exists(tbDirectoryPath.Text))
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show("Invalid directory: " + tbDirectoryPath.Text);
                });
                return false;
            }

            _parentTraverser = parentTrav;
            gv.setCursorHourGlass();

            // Do traversal synchronously
            doTraversalSync(tbDirectoryPath.Text, _parentTraverser);
            return true;
        }

        // New synchronous traversal implementation
        private void doTraversalSync(string dpath, DialogTraverser dt)
        {
            if (string.IsNullOrEmpty(dpath))
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show($"Invalid path: {dpath}", "ERROR");
                });
                return;
            }

            // Initialize state
            if (bThisIsSubWindow)
            {
                gv.initParm1List[0].mostRecentSubfolder = dpath;
            }
            else
            {
                gv.initParm1List[0].mostRecent = dpath;
                gv.lastTraversedFolder = dpath;
            }

            SafeInvoke(() =>
            {
                bLoadedList = true;
                tbDirectoryPath.Text = dpath;
                tbCopyFileName.Text = "";
                action = ActionTraversal.TRAVERSING;
            });

            // Initialize lists
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

            ARGS args = BuildTraversalArgsFromParent();
            args.dirpath = dpath;

            // Do the actual file enumeration synchronously
            PerformSynchronousFileEnumeration(dpath, args);
        }

        private HashSet<string>? BuildValidExtensions(ARGS args)
        {
            if (args.bALL)
                return null;

            HashSet<string> validExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 🔹 FIX: Use the current search extension category from gv.searchExtensionCategoryInUse
            string category = gv.searchExtensionCategoryInUse ?? "videos";

            if (gv.SearchExtensions.TryGetValue(category, out var extensions) && extensions != null)
            {
                validExtensions.UnionWith(extensions);
            }
            else
            {
                // Fallback to videos if category not found
                if (gv.SearchExtensions.TryGetValue("videos", out var videoExt) && videoExt != null)
                {
                    validExtensions.UnionWith(videoExt);
                }
            }

            // If still empty, add default pictures
            if (validExtensions.Count == 0)
            {
                if (gv.SearchExtensions.TryGetValue("pictures", out var pictureExt) && pictureExt != null)
                {
                    validExtensions.UnionWith(pictureExt);
                }
            }

            return validExtensions.Count > 0 ? validExtensions : null;
        }

        private void PerformSynchronousFileEnumeration(string rootPath, ARGS args)
        {
            int count = 0;
            int maxFiles = gv.iMaxFileCount;

            args ??= new ARGS { bPictures = true };
            HashSet<string>? validExtensions = BuildValidExtensions(args);

            try
            {
                // Use EnumerationOptions for better performance
                EnumerationOptions options = new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    AttributesToSkip = FileAttributes.System,
                    ReturnSpecialDirectories = false
                };

                // Pre-allocate list with estimated capacity
                List<FileInfoItem> batch = new List<FileInfoItem>(1000);
                int batchSize = 500;
                int lastUpdateCount = 0;

                foreach (string file in Directory.EnumerateFiles(rootPath, "*.*", options))
                {
                    if (count >= maxFiles)
                        break;

                    // Just get basic info without FileInfo
                    string ext = Path.GetExtension(file);
                    if (validExtensions != null && !validExtensions.Contains(ext)) continue;

                    // Create minimal object - defer expensive operations
                    var item = new FileInfoItem
                    {
                        fname = Path.GetFileName(file),
                        ext = ext.TrimStart('.'),
                        dpath = Path.GetDirectoryName(file) ?? "",
                        fpath = file,
                        // Skip expensive operations initially:
                        // len = fi.Length,  ← Only when needed
                        // stimestamp = ..., ← Only when needed
                    };

                    batch.Add(item);
                    count++;

                    // Batch add to reduce lock contention
                    if (batch.Count >= batchSize)
                    {
                        AddBatchToList(batch, bThisIsSubWindow);
                        batch.Clear();
                    }

                    // Update UI less frequently (every 500 files instead of 50)
                    if (count - lastUpdateCount >= 500)
                    {
                        lastUpdateCount = count;
                        SafeInvoke(() =>
                        {
                            btTraversing.Text = $"Found {count} files...";
                        });
                        // Remove Application.DoEvents() - causes massive slowdown
                    }
                }

                // Add remaining batch
                if (batch.Count > 0)
                {
                    AddBatchToList(batch, bThisIsSubWindow);
                }

                // Update final counts
                if (bThisIsSubWindow)
                {
                    gv.slideCount2 = count;
                    gv.slideCount0 = count;
                    imageFileList2 = gv.imageFileList2;
                    imageFileListOriginal2 = gv.imageFileList2;
                }
                else
                {
                    gv.slideCount1 = count;
                    gv.slideCount0 = count;
                }

                SafeInvoke(() =>
                {
                    btTraversing.Text = $"Completed: {count} files";
                });
            }
            catch (Exception ex)
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show($"Error enumerating files: {ex.Message}", "Enumeration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        // New helper method for batch operations
        private void AddBatchToList(List<FileInfoItem> batch, bool isSubWindow)
        {
            if (isSubWindow)
            {
                foreach (var item in batch)
                    gv.imageFileList2.addItem(item);
            }
            else
            {
                foreach (var item in batch)
                    gv.imageFileList1.addItem(item);
            }
        }

        // Optimized - removed LINQ and unnecessary checks
        private bool IsValidMediaFile(string filePath)
        {
            // This method is now obsolete - validation moved inline
            return true;
        }

        private void CompleteTraversal()
        {
            SafeInvoke(() =>
            {
                btTraversing.Visible = false;
                action = ActionTraversal.COMPLETED;

                // Suspend layout during bulk updates
                dgv1.SuspendLayout();

                try
                {
                    // Update UI
                    tbCount.BackColor = Color.Yellow;
                    btVideos.BackColor = Color.LightGreen;
                    btClose.BackColor = Color.Aqua;
                    bLoadedList = true;

                    // Bind data to grid efficiently
                    if (bThisIsSubWindow && imageFileList2 != null)
                    {
                        dgv1.DataSource = null;

                        // Use BindingList for better performance with large datasets
                        dgv1.DataSource = new BindingList<FileInfoItem>(imageFileList2.finfoList);

                        tbCount.Text = imageFileList2.finfoList.Count.ToString();
                    }

                    // Format and sort
                    formatDataGridViewFileList();
                }
                finally
                {
                    dgv1.ResumeLayout();
                }

                SortName();
            });

            // Set title safely
            if (gv.imageFileList1?.getImageFileListLength() > 0)
            {
                SafeUpdateMainWindow(gv.imageFileList1.getIndexed(0), tbDirectoryPath.Text);
            }
            tbTotalSpace.Text = tbFileLength.Text;
            tbTotalCountFound.Text = tbCount.Text;

            // Notify parent DialogTraverser that traversal is complete
            try
            {
                if (_parentTraverser != null && !_parentTraverser.IsDisposed)
                {
                    _parentTraverser.SourceTraversalCompleted();
                }
            }
            catch (Exception ex)
            {
                // Log but don't crash
                gv?.debug?.w("Failed to notify parent traverser", ex.Message);
            }
        }

        public void Reposition3(DialogTraverser parentT)
        {
            SafeInvoke(() =>
            {
                if (bThisIsSubWindow)
                {
                    var parentLocation = parentT.Location;
                    int x = parentLocation.X;
                    int y = parentLocation.Y + parentT.Height;

                    this.Location = new Point(x, y);
                }
            });
        }

        private Point SafeGetParentLocation()
        {
            if (_parentTraverser.InvokeRequired)
            {
                return (Point)_parentTraverser.Invoke(new Func<Point>(() => _parentTraverser.Location));
            }
            else
            {
                return _parentTraverser.Location;
            }
        }
        public void SetLoadedTrav()
        {
            cbLoadedAndReady.Checked = true;
        }
        public string? SearchForFile(string searchName)
        {
            SafeInvoke(() =>
            {
                tbFileNameSelected.Text = searchName;
            });
            string? test = SearchWin3MediaName(searchName);
            SafeSetParentResult(test, tbFolderName.Text);
            return test;
        }

        private void SafeSetParentResult(string? result, string folderName)
        {
            if (_parentTraverser.InvokeRequired)
            {
                _parentTraverser.Invoke(new Action(() => _parentTraverser.SetResult(result, folderName)));
            }
            else
            {
                _parentTraverser.SetResult(result, folderName);
            }
        }

        public string? SearchWin3MediaName(string searchName) //call this
        {
            string? result = null;
            SafeInvoke(() =>
            {
                tbFileNameSelected.Text = searchName;
                gv.foundInWin2 = false;

                bool found = ClearAndSearchWin3ByMedia();
                if (!found)
                {
                    int countRows = dgv1.DisplayedRowCount(true);
                    if (countRows > 0)
                    {
                        // Handle case where rows exist but search didn't find matches
                    }
                    result = null;
                }
                else
                {
                    tbFoundSeachList2.Text = tbFileNameSelected.Text;
                    tbFound2Folder.Text = gv.searchWin3.GetSubFolderFound();
                    result = tbFoundSeachList2.Text;
                }
                gv.foundInWin2 = found;
            });

            return result;
        }

        long filelen = 0;

        public bool ClearAndSearchWin3ByMedia()
        {
            ClearSearch3Result();

            int foundRow = -1;
            if (!cbSearchForLenMatch.Checked)
                foundRow = StartSearchWin3Title(tbFileNameSelected.Text);
            else
                foundRow = StartSearchWin3Size(filelen);

            return foundRow >= 0;
        }

        public void ClearSearch3Result()
        {
            tbFoundSeachList2.Text = "";
            tbFoundSeachList2.BackColor = Color.White;
            tbFoundFileFolder.Text = "";
            btSearchWin2.BackColor = Color.LightGray;
            tbWin2FileSizeMatchSearch.Text = "";
        }

        string lastSearchTitle = "";

        public int StartSearchWin3Title(string searchTitle) // SEARCH SUBWINDOW SUBSEARCH SUB
        {
            if (string.IsNullOrEmpty(searchTitle))
                return -1;

            tbFound2Folder.Text = "";
            tbFoundSeachList2.Text = "";

            string searchForThis = ff.getFileNameFromPath(searchTitle);
            if (string.IsNullOrEmpty(searchForThis))
                return -1;

            lastSearchTitle = searchTitle;
            if (cbSearchPartial.Checked)
                searchForThis = GetPartialFileNameForSearch(searchForThis);

            int rowCount = SearchWin2TitleAndDisplaySubset(searchForThis);  //Search Window 2
            if (rowCount > 0)
            {
                tbFound2Folder.Text = GetDPathValueFromFirstRow() ?? "";
                tbFoundFileFolder.Text = tbFound2Folder.Text;
                tbFolderName.Text = ExtractFolderName(tbFound2Folder.Text);
                tbFoundSeachList2.Text = searchForThis;
                tbFoundSeachList2.BackColor = Color.LightGreen;

                tbFound2Folder.BackColor = Color.LightGreen;
                string numberString = GetLenValueFromFirstRow() ?? "0";
                if (long.TryParse(numberString, out long number))
                {
                    string formattedNumber = string.Format("{0:N0}", number); // Example: 123,123,123
                    tbFileLength.Text = formattedNumber;
                }
            }
            else
            {
                tbFoundSeachList2.BackColor = Color.LightGray;
                tbFound2Folder.BackColor = Color.LightGray;
            }
            return rowCount;
        }

        public string GetSubFolderFound()
        {
            return tbFound2Folder.Text;
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

        public void ClearIfNotFound()
        {
            SafeInvoke(() =>
            {
                dgv1.Rows.Clear();
                dgv1.Refresh();
            });
        }

        public int StartSearchWin3Size(long len) // SEARCH SUBWINDOW SUBSEARCH SUB
        {
            tbFound2Folder.Text = "";
            tbFoundSeachList2.Text = "";

            int rowId = SafeSearchDialogTraverser2(filelen);
            if (rowId > -1)
            {
                string test = SafeGetDialogTraverser2FileName();
                tbFound2Folder.Text = SafeGetDialogTraverser2Folder();
                tbFoundFileFolder.Text = tbFound2Folder.Text;
                tbFoundSeachList2.Text = test;
                tbFoundSeachList2.BackColor = Color.LightGreen;
                tbFound2Folder.BackColor = Color.LightGreen;
                tbWin2FileSizeMatchSearch.Text = SafeGetDialogTraverser2FileLength();
            }
            else
            {
                tbFoundSeachList2.BackColor = Color.LightGray;
                tbFound2Folder.BackColor = Color.LightGray;
            }
            return rowId;
        }

        private int SafeSearchDialogTraverser2(long fileLength)
        {
            if (gv.dialogTraverser2?.InvokeRequired == true)
            {
                return (int)gv.dialogTraverser2.Invoke(new Func<int>(() => gv.dialogTraverser2.SearchWin2Size(fileLength)));
            }
            else
            {
                return gv.dialogTraverser2?.SearchWin2Size(fileLength) ?? -1;
            }
        }

        private string SafeGetDialogTraverser2FileName()
        {
            if (gv.dialogTraverser2?.InvokeRequired == true)
            {
                return (string)gv.dialogTraverser2.Invoke(new Func<string>(() => gv.dialogTraverser2.getFileName()));
            }
            else
            {
                return gv.dialogTraverser2?.getFileName() ?? "";
            }
        }

        private string SafeGetDialogTraverser2Folder()
        {
            if (gv.dialogTraverser2?.InvokeRequired == true)
            {
                return (string)gv.dialogTraverser2.Invoke(new Func<string>(() => gv.dialogTraverser2.getFolder()));
            }
            else
            {
                return gv.dialogTraverser2?.getFolder() ?? "";
            }
        }

        private string SafeGetDialogTraverser2FileLength()
        {
            if (gv.dialogTraverser2?.InvokeRequired == true)
            {
                return (string)gv.dialogTraverser2.Invoke(new Func<string>(() => gv.dialogTraverser2.getFileLength()));
            }
            else
            {
                return gv.dialogTraverser2?.getFileLength() ?? "";
            }
        }

        public List<FileInfoItem>? imageFileListSearch; //search results  from original load imageFileList2
        ImageFileList? imageFileListOriginal2;

        public int SearchWin2TitleAndDisplaySubset(string searchTitle) //dmc searchInWindow2 actual search routine   //Search Window 2
        {
            if (bThisIsSubWindow)
            {
                if (imageFileList2 == null)
                {
                    imageFileList2 = imageFileListOriginal2;
                }
            }
            DisplaySearchingSubFor();

            if (imageFileList2 == null)
                return -1;
            imageFileListSearch = imageFileList2.finfoList.Where(f => f.fname.ToUpper().Contains(searchTitle.ToUpper())).ToList();

            dgv1.DataSource = null;
            dgv1.DataSource = imageFileListSearch;

            tbCount.Text = dgv1.RowCount.ToString();
            //cbLoadedAndReady.Checked = true;

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

        public void LoadDGV()
        {
            SafeInvoke(() =>
            {
                dgv1.DataSource = null;
                dgv1.AutoGenerateColumns = true;
                dgv1.DataSource = imageFileListOriginal2?.getFinfo();
                dgv1.Refresh();
                tbCount.Text = dgv1.RowCount.ToString();
                cbLoadedAndReady.Checked = true;
            });
        }

        private void btSearchWin2_Click(object sender, EventArgs e)
        {
            ClearAndSearchWin3ByMedia();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            SafeBringParentToFront();
            this.Close();
        }

        private void SafeBringParentToFront()
        {
            if (_parentTraverser.InvokeRequired)
                _parentTraverser.Invoke(new Action(() => _parentTraverser.BringToFront()));
            else
                _parentTraverser.BringToFront();
        }

        bool bLoadedList = false;
        private void btVideos_Click(object sender, EventArgs e)
        {
            TraverseFolders();
        }

        public void TraverseFoldersx()
        {
            SafeInvoke(() =>
            {
                cbLoadedAndReady.Checked = false;
                tbMovieFpath.Text = "";
                btTraversing.Visible = true;
                btTraversing.Text = "Traversing...";
                btTraversing.Refresh();
            });

            if (_parentTraverser == null)
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show("NO tParent TraverserParent");
                });
                return;
            }
            try
            {
                // Run traversal synchronously on UI thread
                if (!TraverseGoSync(_parentTraverser))
                {
                    SafeInvoke(() =>
                    {
                        btTraversing.Visible = false;
                        tbMovieFpath.Text = "!! ERROR Invalid directory !!";
                        MessageBox.Show("!! ERROR Invalid directory !!");
                    });
                    return;
                }

                // Update UI immediately since we're on the same thread
                CompleteTraversal();
            }
            catch (Exception ex)
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show($"Error during traversal: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
            finally
            {
                SafeInvoke(() =>
                {
                    btTraversing.Visible = false;
                });
                gv.setCursorDefault();
            }
        }

        private void DisplaySearchingSubFor(string? txtTitle = null)
        {
            tbStatusFoundInWin2.Text = "searching...";
            tbStatusFoundInWin2.BackColor = Color.LightGray;
        }

        private void DisplaySearchingSubFound(string? title = null)
        {
            tbStatusFoundInWin2.Text = $"{title}";
            tbStatusFoundInWin2.BackColor = Color.LightGreen;
        }

        private void DisplaySearchingSubNotFound(string? title = null)
        {
            tbStatusFoundInWin2.Text = $"NOT FOUND {title}";
            tbStatusFoundInWin2.BackColor = Color.LightPink;
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
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            SafeInvoke(() =>
            {
                dgv1.DataSource = null;
                if (imageFileList2 != null && imageFileList2.getImageCount() > 0)
                    imageFileList2.clearList();
            });
        }

        public string? GetDPathValueFromFirstRow()
        {
            if (dgv1.Rows.Count > 0 && dgv1.Columns.Contains("dpath"))
            {
                return dgv1.Rows[0].Cells["dpath"].Value?.ToString();
            }
            return null;
        }

        public string? GetLenValueFromFirstRow()
        {
            if (dgv1.Rows.Count > 0 && dgv1.Columns.Contains("len"))
            {
                return dgv1.Rows[0].Cells["len"].Value?.ToString();
            }
            return null;
        }

        public string ExtractFolderName(string? fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return string.Empty;

            return Path.GetFileName(fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            base.SetVisibleCore(value);
        }

        public void SetFormLocation(Point location)
        {
            SafeInvoke(() =>
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = location;
            });
        }

        private ARGS BuildTraversalArgsFromParent()
        {
            if (_parentTraverser != null)
                return _parentTraverser.BuildTraversalArgsFromUi();

            // 🔹 FIX: Default fallback should respect current search category
            string category = gv.searchExtensionCategoryInUse ?? "videos";

            ARGS args = new ARGS();

            // Set the appropriate flag based on category
            switch (category.ToLower())
            {
                case "videos":
                    args.bMovies = true;
                    break;
                case "pictures":
                    args.bPictures = true;
                    break;
                case "html":
                    args.bHTML = true;
                    break;
                case "midi":
                    args.bMIDI = true;
                    break;
                case "all":
                    args.bALL = true;
                    break;
                default:
                    args.bMovies = true; // final fallback
                    break;
            }

            return args;
        }
        private void doTraversal(string dpath, DialogTraverser dt)
        {
            _parentTraverser = dt;
            if (string.IsNullOrEmpty(dpath))
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show($"fpath fail {dpath}", "ERROR fpath");
                });
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

            SafeInvoke(() =>
            {
                bLoadedList = true;
                btTraversing.Visible = true;
                tbDirectoryPath.Text = dpath;
                tbCopyFileName.Text = "";
                btCancel.Enabled = true;
                tbCount.BackColor = Color.White;
                action = ActionTraversal.TRAVERSING;
            });

            gv.setCursorHourGlass();

            TraverserBG5 traverser = new TraverserBG5(gv, _parentTraverser, this);

            if (!bThisIsSubWindow)
            {
                if (gv.bImageFileList1Loaded)
                {
                    if (gv.imageFileList1 != null)
                        gv.imageFileList1.clearList();
                    gv.bImageFileList1Loaded = false;
                }
            }
            if (bThisIsSubWindow)
            {
                gv.imageFileList2 = new ImageFileList();
                gv.bLoadingTraverser2 = true;
            }
            else //main window
            {
                gv.imageFileList1 = new ImageFileList();
                gv.deleteFileList1 = new ImageFileList();
                gv.bLoadingTraverser2 = false;
            }

            // Fix: reuse parent settings instead of forcing bMovies=true
            ARGS args = BuildTraversalArgsFromParent();
            args.dirpath = dpath;

            traverser.StartTraversal(args, bThisIsSubWindow, dpath);
        }

        public bool TraverseGo(DialogTraverser parentTrav)
        {
            SafeInvoke(() =>
            {
                dgv1.DataSource = null;
            });

            if (!Directory.Exists(tbDirectoryPath.Text))
            {
                SafeInvoke(() =>
                {
                    MessageBox.Show("Invalid directory: " + tbDirectoryPath.Text);
                });
                return false;
            }
            _parentTraverser = parentTrav;
            gv.debug.w("traverse");
            doTraversal(tbDirectoryPath.Text, _parentTraverser);
            return true;
        }

        public void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e) //on work completed findlist
        {
            SafeInvoke(() =>
            {
                gv.setCursorDefault();

                if (e.Cancelled)
                {
                    this.Text = "Cancelled";
                    action = ActionTraversal.NONE;
                    btTraversing.Visible = false;
                    btVideos.BackColor = Color.DarkGray;
                }
                else
                {
                    // Extract status from result
                    if (e.Result is TraverserBG5.TraversalResult result)
                    {
                        this.Text = result.FinalStatus ?? "Completed";
                    }

                    // Call existing method that does the rest
                    CompleteTraversal();
                }
            });
        }

        public void OnCancel(string stat)
        {
            SafeInvoke(() =>
            {
                btTraversing.Visible = false;
                this.tbDirectoryPath.Text = "Cancelled.";
                action = ActionTraversal.NONE;
            });
        }

        public void SortName()
        {
            if (!bLoadedList || imageFileList2 == null)
                return;

            // Use array sort instead of LINQ OrderBy for better performance
            FileInfoItem[] sortedArray = imageFileList2.finfoList.ToArray();
            Array.Sort(sortedArray, (a, b) => string.Compare(a.fname, b.fname, StringComparison.OrdinalIgnoreCase));

            SafeInvoke(() =>
            {
                dgv1.SuspendLayout();
                try
                {
                    dgv1.DataSource = null;
                    dgv1.DataSource = new BindingList<FileInfoItem>(sortedArray);
                }
                finally
                {
                    dgv1.ResumeLayout();
                }
            });

            if (bThisIsSubWindow)
            {
                imageFileList2.finfoList.Clear();
                imageFileList2.finfoList.AddRange(sortedArray);
            }

            SafeInvoke(() =>
            {
                formatDataGridViewFileList();
            });
        }

        private void SearchForMediaByName_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_timer2s != null)
            {
                _timer2s.Stop();
                _timer2s.Tick -= Timer2s_Tick;
                _timer2s.Dispose();
                _timer2s = null;
            }
            if (vlcPlayer != null && !vlcPlayer.IsDisposed)
                vlcPlayer.Close();
            SafeNotifyParentClosing();
        }

        private void SafeNotifyParentClosing()
        {
            if (_parentTraverser.InvokeRequired)
            {
                _parentTraverser.Invoke(new Action(() => _parentTraverser.ClosedWin3()));
            }
            else
            {
                _parentTraverser.ClosedWin3();
            }
        }

        private void SafeUpdateMainWindow(FileInfoItem fileInfo, string directoryPath)
        {
            if (gv.mainWindow?.InvokeRequired == true)
            {
                gv.mainWindow.Invoke(new Action(() =>
                    gv.mainWindow.setTitle(fileInfo, directoryPath)));
            }
            else
            {
                gv.mainWindow?.setTitle(fileInfo, directoryPath);
            }
        }

        public void ApplyScanResults(List<TraverserBG5.FileResult> results, bool isSubWindow)
        {
            SafeInvoke(() =>
            {
                try
                {
                    dgv1.SuspendLayout();

                    // Clear existing data
                    if (isSubWindow)
                    {
                        if (gv.imageFileList2 == null)
                            gv.imageFileList2 = new ImageFileList();
                        gv.imageFileList2.clearList();
                    }
                    else
                    {
                        if (gv.imageFileList1 == null)
                            gv.imageFileList1 = new ImageFileList();
                        gv.imageFileList1.clearList();
                    }

                    // Convert FileResult to FileInfoItem
                    var fileInfoItems = new List<FileInfoItem>();

                    foreach (var result in results)
                    {
                        var item = new FileInfoItem
                        {
                            fname = Path.GetFileName(result.Path),
                            ext = Path.GetExtension(result.Path).TrimStart('.'),
                            dpath = Path.GetDirectoryName(result.Path) ?? "",
                            fpath = result.Path,
                            len = result.Length ?? 0,
                            stimestamp = result.LastWriteTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                            type = result.Kind,
                            level = result.Level,
                            rating = ' ',
                            comment = "",
                            source = ""
                        };
                        fileInfoItems.Add(item);

                        // Add to appropriate list
                        if (isSubWindow)
                            gv.imageFileList2.addItem(item);
                        else
                            gv.imageFileList1.addItem(item);
                    }

                    // Update UI
                    dgv1.DataSource = null;
                    dgv1.DataSource = fileInfoItems;

                    if (isSubWindow)
                    {
                        imageFileList2 = gv.imageFileList2;
                        imageFileListOriginal2 = gv.imageFileList2;
                    }

                    tbCount.Text = fileInfoItems.Count.ToString();

                    // 🔹 ADD THIS LINE: Update total count found
                    tbTotalCountFound.Text = fileInfoItems.Count.ToString();
                }
                finally
                {
                    dgv1.ResumeLayout();
                }
            });
        }

        public void DisplayTraversalArgs(string[] extensions)
        {
            SafeInvoke(() =>
            {
                // Optional: Display extensions info
                string extInfo = $"Scanned: {string.Join(", ", extensions)}";
                // You can show this somewhere in your UI if needed
            });
        }

        public void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SafeInvoke(() =>
            {
                if (e.UserState is int count)
                {
                    btTraversing.Text = $"Found {count} files...";
                    btTraversing.Refresh();
                }
            });
        }

        public void UpdateStatusRowCount(int rowCount)
        {
            SafeInvoke(() =>
            {
                btTraversing.Text = $"Processing {rowCount} files...";
                btTraversing.Refresh();
            });
        }

        private void btGetCount_Click(object sender, EventArgs e)
        {
            SafeInvoke(() =>
            {
                // Get the total number of rows in the DataGridView
                int totalRows = dgv1.RowCount;

                // Update the total count found textbox
                tbTotalCountFound.Text = totalRows.ToString();

                // Optional: Update tbCount as well for consistency
                tbCount.Text = totalRows.ToString();

                // Optional: Add visual feedback
                tbTotalCountFound.BackColor = Color.LightBlue;

                // Optional: Show additional info if the grid has data
                if (totalRows > 0)
                {
                    tbTotalCountFound.BackColor = Color.LightGreen;
                }
                else
                {
                    tbTotalCountFound.BackColor = Color.LightPink;
                }
            });
        }

        /// <summary>
        /// Updates both count displays with current DataGridView row count
        /// </summary>
        public void UpdateRowCounts()
        {
            SafeInvoke(() =>
            {
                int totalRows = dgv1.RowCount;

                tbCount.Text = totalRows.ToString();
                tbTotalCountFound.Text = totalRows.ToString();

                // Visual feedback based on count
                if (totalRows > 0)
                {
                    tbCount.BackColor = Color.Yellow;
                    tbTotalCountFound.BackColor = Color.LightGreen;
                }
                else
                {
                    tbCount.BackColor = Color.LightGray;
                    tbTotalCountFound.BackColor = Color.LightPink;
                }
            });
        }

        private void dgv1_SelectionChanged(object sender, EventArgs e)
        {
            bHaveDuration = false;
            SafeInvoke(() =>
            {
                try
                {
                    // Check if we have valid selection
                    if (dgv1.SelectedRows.Count > 0)
                    {
                        var selectedRow = dgv1.SelectedRows[0];
                        int rowIndex = selectedRow.Index;

                        // 🔹 FIX: Validate the row index bounds
                        if (rowIndex >= 0 && rowIndex < dgv1.Rows.Count)
                        {
                            // 🔹 FIX: Check if it's not the "new row" (if AllowUserToAddRows is true)
                            if (!selectedRow.IsNewRow)
                            {
                                tbRowId.Text = rowIndex.ToString();

                                // 🔹 OPTIONAL: Show additional info about the selected file
                                if (dgv1.DataSource != null)
                                {
                                    // Get additional info from the selected row if needed
                                    var dataSource = dgv1.DataSource;
                                    if (dataSource is List<FileInfoItem> fileList && rowIndex < fileList.Count)
                                    {
                                        var selectedFile = fileList[rowIndex];
                                        tbFileNameSelected.Text = selectedFile.fname;
                                        tbMovieFpath.Text = selectedFile.fpath;
                                    }
                                    else if (dataSource is BindingList<FileInfoItem> bindingList && rowIndex < bindingList.Count)
                                    {
                                        var selectedFile = bindingList[rowIndex];
                                        tbFileNameSelected.Text = selectedFile.fname;
                                        tbMovieFpath.Text = selectedFile.fpath;
                                    }
                                }
                            }
                            else
                            {
                                // This is the "new row" - clear the row ID
                                tbRowId.Text = "New";
                            }
                        }
                        else
                        {
                            // Row index is out of bounds
                            tbRowId.Text = "Invalid";
                        }
                    }
                    else
                    {
                        // No rows selected
                        tbRowId.Text = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    // 🔹 FIX: Catch any remaining exceptions and handle gracefully
                    tbRowId.Text = "Error";

                    // Optional: Log the exception for debugging
                    System.Diagnostics.Debug.WriteLine($"dgv1_SelectionChanged Exception: {ex.Message}");
                }
            });
        }

        // In your constructor or form setup, add:
        private void ConfigureDataGridView()
        {
            SafeInvoke(() =>
            {
                // 🔹 PREVENTION: Disable adding new rows to avoid the extra row
                dgv1.AllowUserToAddRows = false;

                // 🔹 PREVENTION: Make it read-only if users shouldn't edit
                dgv1.ReadOnly = true;

                // 🔹 PREVENTION: Set selection mode
                dgv1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv1.MultiSelect = false; // Allow only single selection
            });
        }
        Player vlcPlayer;
        private void btView_Click(object sender, EventArgs e)
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
            {
                vlcPlayer = new Player(gv, null, null, this);
            }
            vlcPlayer.Activate();
            vlcPlayer.Show();
            Play2();
            _timer2s.Start();
        }
        bool bPlayedFirstVideo = false;
        double duration = 0;
        public void GetVideoDuration()
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            duration = vlcPlayer.GetDuration();
            var mind = (int)duration / 60;
            var sec = (int)duration % 60;
            tbDuration.Text = $"{mind}:{sec}";
        }
        public double GetCurentPosition()
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return 0;
            double pos = vlcPlayer.GetPositionSeconds();
            int mind = (int)pos / 60;
            int sec = (int)pos % 60;
            tbPosition.Text = $"{mind}:{sec:D2}";
            return pos;
        }
        string playerState = "";
        public string GetPlayerState()
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return null;
            playerState = vlcPlayer.GetState();
            tbPlayerState.Text = playerState;
            return playerState;
        }
        bool bHaveDuration = false;
        private void Timer2s_Tick(object? sender, EventArgs e)
        {
            GetPlayerState();
            if (playerState == "Playing")
            {
                if (!bHaveDuration)
                {
                    GetVideoDuration();
                    bHaveDuration = true;
                }
                GetCurentPosition();
            } else if (playerState == "Ended")
            {
                bHaveDuration = false;
            }
        }

        /// <summary>
        /// Skips the vlcPlayer to 30 seconds before the end of the current video.
        /// </summary>
        public void SkipToNearEnd(int secondsBeforeEnd = 30)
        {
            if (vlcPlayer == null || vlcPlayer.IsDisposed)
                return;
            vlcPlayer.gotoEnd(secondsBeforeEnd);
        }

        public void Play2()
        {
            SafeInvoke(() =>
            {
                string filePath = tbMovieFpath.Text;
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                    return;

                if (vlcPlayer == null || vlcPlayer.IsDisposed)
                {
                    vlcPlayer = new Player(gv, null);
                    vlcPlayer.Show();
                }

                vlcPlayer.LoadMedia(filePath, autoPlay: true);
                vlcPlayer.Activate();
                bPlayedFirstVideo = true;
            });
        }

        private void btViewEnding_Click(object sender, EventArgs e)
        {
            SkipToNearEnd();
        }
    }
}
