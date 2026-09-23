#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class DialogParms : Form
    {
        GlobalVars gv;
        FileFunctions ff;
        //GetDotNetVersion getDotNetVersion = new GetDotNetVersion();
        // FileFunctions requires several Globals 
        //     gv.bRunningInDevelopment
        //     gv.dataFolder
        //     gv.dazenFolderName
        //     gv.inifileFullPathName
        //     gv.initParm1List
        //     gv.mainWindow

        public DialogParms()
        {
            InitializeComponent();
        }
        public DialogParms(GlobalVars g)
        {
            InitializeComponent();
            this.dgvTargetHistory.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTargetHistory_CellDoubleClick);
            gv = g;
            ff = new FileFunctions(gv);
            //string[] driveinfo = ff.getDrivesInfo();
            ff.GetAllDrivesInfo();
            tbEnvironmnet.Text = gv.bRunningInDevelopment ? "Development ENV" : "Production ENV";
            tbInitFile.Text = gv.inifileFullPathName;

            CreateLvDirectoryList(gv);
            this.Refresh();
            this.Show();
            CompleteInitialization();
            gv.mainWindow.HideLoadingParms();
            this.Activate();
            if (gv.useTestSourceAndTargetFolder) // Don't save parms if using test folders to avoid overwriting real settings
            {
                
                btSaveInitFile.Enabled = false;
            }
        }
        //moved from DialogTraverser

        ComboBox cmboSetTarget;
        public void loadTargetCmboList()
        {
            cmboSetTarget.Items.Add(gv.initParm1List[0].targetDir1);
            cmboSetTarget.Items.Add(gv.initParm1List[0].targetDir2);
            cmboSetTarget.Items.Add(gv.initParm1List[0].targetDir3);
            if (!string.IsNullOrEmpty(gv.initParm1List[0].mostRecentTarget))
                cmboSetTarget.Items.Add(gv.initParm1List[0].mostRecentTarget);
            cmboSetTarget.SelectedIndex = 0;
        }
        public void CompleteInitialization()
        {

            tbDotNetVersion.Text = GetDotNetVersion.Get45PlusVersionFromRegistry();
            //displayAppParms();            gv.debug.w("read ini file :");
            // ff.writeIniFile
            resetDGVParms();
            loadSourceCmboList();
            LoadMostRecentList();

            // tbRatingsVideo.Text = gv.ratingsVideos;
            // tbRatingsImages.Text = gv.ratingsImages;
            //
            // DMCNOTE: The following line sets the My Documents folder path in the text box.
            tbMyDocumentsFolder.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (gv.bRunningInDevelopment)
            {
                // Dev: MyDocuments\_dazen\{appName}\
                tbInitFolderPath.Text = gv.dataFolder;
                tbInitFileName.Text = gv.inifileFullPathName;
            }
            else
            {
                // Production: AppData\Roaming\{appName}\Config\
                tbInitFolderPath.Text = Path.Combine(
                    GlobalVars.appDataFolder,
                    gv.appName,
                    "Config");
                tbInitFileName.Text = Path.Combine(
                    GlobalVars.appDataFolder,
                    gv.appName,
                    "Config",
                    GlobalVars.initFileName);
            }

            string fileName = gv.mainWindow.CreateNotesFileName(1);
            string fpath = Path.Combine(gv.dataFolder, fileName);
            tbNotesFile.Text = fpath;

            VerifyIniFolder();
            VerifyIniParmsFile();




            LoadDgvHistoryList();
            LoadDgvTargetHistory();
        }
        public void CompleteInitializationxxxx()
        { 

            tbDotNetVersion.Text = GetDotNetVersion.Get45PlusVersionFromRegistry();
            //displayAppParms();            gv.debug.w("read ini file :");
            // ff.writeIniFile
            resetDGVParms();
            loadSourceCmboList();
            LoadMostRecentList();

            // tbRatingsVideo.Text = gv.ratingsVideos;
            // tbRatingsImages.Text = gv.ratingsImages;
            //
            // DMCNOTE: The following line sets the My Documents folder path in the text box. It uses the Environment.SpecialFolder enumeration to get the path to the My Documents folder for the current user.
            tbMyDocumentsFolder.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);   // building company directory
                                                                                                                  // tbInitFolderPath.Text = Path.Combine(tbMyDocumentsFolder.Text, gv.dazenFolderName);
            tbInitFolderPath.Text = gv.dataFolder;
            string fileName = gv.mainWindow.CreateNotesFileName(1);
            string fpath = Path.Combine(gv.dataFolder, fileName);
            tbNotesFile.Text = fpath;
            //
            // DMCNOTE: The following line sets the full path of the ini file in the text box. It uses the gv.inifileFullPathName property, which presumably contains the full path to the ini file.
            tbInitFileName.Text = gv.inifileFullPathName;
            LoadDgvHistoryList();
            LoadDgvTargetHistory();
        }

        public List<FolderHistory> folderHistoryListSorted;
        public void LoadDgvHistoryList()
        {
            // Clear existing data
            dgvHistory.DataSource = null;
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();

            // Manually create columns
            dgvHistory.Columns.Add("folderPath", "Folder Path");
            dgvHistory.Columns.Add("action", "Action");
            dgvHistory.Columns.Add("desc", "Description");
            dgvHistory.Columns.Add("lastAccessDate", "Last Access");

            folderHistoryListSorted = gv.folderHistoryList.OrderByDescending(file => file.lastAccessDate).ToList();


            // Manually populate rows
            foreach (var item in folderHistoryListSorted)
            {
                dgvHistory.Rows.Add(
                    item.folderPath,
                    item.action,
                    item.desc,
                    item.lastAccessDate
                );
            }
            foreach (DataGridViewColumn column in dgvHistory.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            this.Refresh();
        }
        public void loadSourceCmboList()
        {
            cmboSourceFolder.Items.Clear();
            int idx = 0;

            if (gv?.folderHistoryList != null)
            {
                foreach (var item in gv.folderHistoryList)
                {
                    if (!string.IsNullOrWhiteSpace(item.folderPath))
                    {
                        cmboSourceFolder.Items.Add(item.folderPath);
                        idx++;
                    }
                }
            }

            // Only set SelectedIndex if items exist
            if (cmboSourceFolder.Items.Count > 0)
            {
                cmboSourceFolder.SelectedIndex = 0;
            }

            tbListCount.Text = idx.ToString();
        }
        //s = s.TrimEnd('/');
        public void resetDGVParms()
        {
            dgvInitParms.DataSource = null;
            this.Refresh();
            dgvInitParms.DataSource = gv.initParm1List;

            tbLastDir1.Text = gv.initParm1List[0].sourceDir1.TrimEnd('\\');
            tbLastDir2.Text = gv.initParm1List[0].sourceDir2.TrimEnd('\\');
            tbLastDir3.Text = gv.initParm1List[0].sourceDir3.TrimEnd('\\');
            tbLastDir4.Text = gv.initParm1List[0].sourceDir4.TrimEnd('\\');
            tbLastDir5.Text = gv.initParm1List[0].sourceDir5.TrimEnd('\\');
            if (gv.initParm1List[0].sourceDir6 != null)
                tbLastDir6.Text = gv.initParm1List[0].sourceDir6.TrimEnd('\\');

            tbTargetFolder1.Text = gv.initParm1List[0].targetDir1;
            tbTargetFolder2.Text = gv.initParm1List[0].targetDir2;
            tbTargetFolder3.Text = gv.initParm1List[0].targetDir3;

            tbWatchFolder1.Text = gv.initParm1List[0].watchFolder1;
            tbWatchFolder2.Text = gv.initParm1List[0].watchFolder2;
            tbWatchFolder3.Text = gv.initParm1List[0].watchFolder3;

            tbHistory1.Text = gv.initParm1List[0].history1;
            tbHistory2.Text = gv.initParm1List[0].history2;
            tbHistory3.Text = gv.initParm1List[0].history3;
            // resetDgvParmsColumns();
            tbMostRecent.Text = gv.initParm1List[0].mostRecent;
            tbMostRecentSub.Text = gv.initParm1List[0].mostRecentSubfolder;
            tbMostRecentTarget.Text = gv.initParm1List[0].mostRecentTarget;
            tbNotesBaseName.Text = gv.initParm1List[0].notesBaseName;

            tbRatingsImages.Text = gv.initParm1List[0].ratingsImagesFile;
            tbRatingsVideo.Text = gv.initParm1List[0].ratingsVideosFile;
        }
        public void LoadMostRecentList()
        {
            tbHold1.Text = gv.initParm1List[0].mostRecent1;
            tbHold2.Text = gv.initParm1List[0].mostRecent2;
            tbHold3.Text = gv.initParm1List[0].mostRecent3;
            tbHold4.Text = gv.initParm1List[0].mostRecent4;
            tbHold5.Text = gv.initParm1List[0].mostRecent5;
            tbHold6.Text = gv.initParm1List[0].mostRecent6;
        }
        public void assignParmsFromTextBoxes()
        {
            gv.initParm1List[0].notesBaseName = tbNotesBaseName.Text;

            gv.initParm1List[0].sourceDir1 = tbLastDir1.Text;
            gv.initParm1List[0].sourceDir2 = tbLastDir2.Text;
            gv.initParm1List[0].sourceDir3 = tbLastDir3.Text;
            gv.initParm1List[0].sourceDir4 = tbLastDir4.Text;
            gv.initParm1List[0].sourceDir5 = tbLastDir5.Text;
            gv.initParm1List[0].sourceDir6 = tbLastDir6.Text;

            gv.initParm1List[0].SetTargetDir1(tbTargetFolder1.Text);
            gv.initParm1List[0].targetDir2 = tbTargetFolder2.Text;
            gv.initParm1List[0].targetDir3 = tbTargetFolder3.Text;

            gv.initParm1List[0].watchFolder1 = tbWatchFolder1.Text;
            gv.initParm1List[0].watchFolder2 = tbWatchFolder2.Text;
            gv.initParm1List[0].watchFolder3 = tbWatchFolder3.Text;
            gv.initParm1List[0].history1 = tbHistory1.Text;
            gv.initParm1List[0].history2 = tbHistory2.Text;
            gv.initParm1List[0].history3 = tbHistory3.Text;
            gv.initParm1List[0].mostRecent = tbMostRecent.Text;
            gv.initParm1List[0].ratingsVideosFile = tbRatingsVideo.Text;
            gv.initParm1List[0].ratingsImagesFile = tbRatingsImages.Text;
        }
        /*
         * public string lastDir; // last transversal
        public string dirPath; //directory with init file
        public string fpath; //fullpath with init file
        public string targetDir; //
        */
        Point listViewLoc;

        private void CreateLvInitParms(GlobalVars gv)
        {
            lvInitParms.View = View.Details;
            lvInitParms.Scrollable = true;
            // Allow the user to edit item text.
            lvInitParms.LabelEdit = false;
            // Allow the user to rearrange columns.
            lvInitParms.AllowColumnReorder = true;
            // Display check boxes.
            lvInitParms.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            lvInitParms.FullRowSelect = true;
            // Display grid lines.
            lvInitParms.GridLines = true;
            // Sort the items in the list in ascending order.
            lvInitParms.Sorting = SortOrder.None;

            Rectangle wndBounds = this.Bounds;

            listViewLoc = lvInitParms.Location;
            Boolean test = true;
            // Create columns for the items and subitems.
            lvInitParms.Columns.Add("parm", 200, HorizontalAlignment.Center);
            lvInitParms.Columns.Add("value", 1000, HorizontalAlignment.Left);

            lvInitParms.Columns[0].Tag = "String";
            lvInitParms.Columns[1].Tag = "String";
        }


        private void CreateLvDirectoryList(GlobalVars gv) //-------------------- set target folders
        {
            lvDirectoryList.View = View.Details;
            lvDirectoryList.Scrollable = true;
            // Allow the user to edit item text.
            lvDirectoryList.LabelEdit = true;
            // Allow the user to rearrange columns.
            lvDirectoryList.AllowColumnReorder = false;
            // Display check boxes.
            lvDirectoryList.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            lvDirectoryList.FullRowSelect = true;
            // Display grid lines.
            lvDirectoryList.GridLines = true;
            // Sort the items in the list in ascending order.
            lvDirectoryList.Sorting = SortOrder.None;

            Rectangle wndBounds = this.Bounds;

            listViewLoc = lvDirectoryList.Location;
            Boolean test = true;
            // Create columns for the items and subitems.
            lvDirectoryList.Columns.Add("keyboard letter", 200, HorizontalAlignment.Center);
            lvDirectoryList.Columns.Add("target directory value", 1000, HorizontalAlignment.Left);

            lvDirectoryList.Columns[0].Tag = "String";
            lvDirectoryList.Columns[1].Tag = "String";
        }


        public Boolean insertRowLvDirectoryList(string pname, string pvalue)
        {
            if (pname == null)
                pname = "null";
            if (pvalue == null)
                pvalue = "null";
            //-- NEW ITEM
            ListViewItem item1 = new ListViewItem(pname, 0);
            // Place a check mark next to the item.
            //-- SUB-ITEMS
            item1.SubItems.Add(pvalue);            //counter

            //++rowCount;
            lvDirectoryList.Items.Add(item1);
            return true;
        }
        public void resetDgvParmsColumns()
        {
            DataGridViewColumn columnCname = dgvInitParms.Columns[gv.IDX_FPATH];
            //columnCname.ReadOnly = true;
            columnCname.Width = 24;
            DataGridViewColumn column2 = dgvInitParms.Columns[gv.IDX_DIRPATH];
            column2.Width = 29;
            DataGridViewColumn column3 = dgvInitParms.Columns[gv.IDX_TARGET_DIR];
            column3.Width = 290;
            DataGridViewColumn column4 = dgvInitParms.Columns[gv.IDX_LASTDIR];
            column4.Width = 290;
            DataGridViewColumn column5 = dgvInitParms.Columns[gv.IDX_LASTDIR2];
            column5.Width = 290;
            DataGridViewColumn column6 = dgvInitParms.Columns[gv.IDX_LASTDIR3];
            column6.Width = 290;
        }
        private void btCreateInitFile_Click(object sender, EventArgs e)
        {
            gv.debug.w("create ini file");
            int rc = gv.mainWindow.createDefaultInitParm1File(); //ff.createInit1File(null, 
            gv.debug.w("rc = " + rc.ToString());
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



        int iSelectedIdx = -1;
        private void lvDirectoryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            gv.debug.w("listView SelectedIndexedChanged ", sender.ToString(), e.ToString());
            if (lvDirectoryList.SelectedIndices.Count > 0)
                iSelectedIdx = lvDirectoryList.SelectedIndices[0];
            else
                return;

            if (lvDirectoryList.FocusedItem != null)
                tb1.Text = lvDirectoryList.FocusedItem.SubItems[1].Text;
            // lvDirectoryList.Items[0].SubItems[1].Text = "edited";
        }

        private void tb1_Enter(object sender, EventArgs e)
        {

        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (iSelectedIdx >= 0)
            {
                lvDirectoryList.Items[iSelectedIdx].SubItems[1].Text = tb1.Text;
                lvDirectoryList.Refresh();
            }
        }

        private void tb1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogFolderSelection getFolder = new DialogFolderSelection(gv, tb1);
            getFolder.Show();
            getFolder.Activate();
            //tb1.Text = gv.dirTarget4Copy;
        }
        public void setTargetFolder(string dir)
        {
            tb1.Text = dir;
        }
        private void tb1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btVerifyIniFile_Click(object sender, EventArgs e)
        {
            bool brc = System.IO.File.Exists(gv.inifileFullPathName);
            if (brc)
            {
                MessageBox.Show($"Ini Parameters file does exist {gv.inifileFullPathName}");
                tbInitFileName.BackColor = Color.LightGreen;
            }
            else
            {
                MessageBox.Show($"Ini Parameters file was not found {gv.inifileFullPathName}");
                tbInitFileName.BackColor = Color.LightCoral;
            }
        }

        private void btLoadInitFile_Click(object sender, EventArgs e)
        {
            dgvInitParms.DataSource = null;
            int count = ff.readInitParms1File(gv.inifileFullPathName);
            if (count > 0)
                resetDGVParms();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btSaveInitFile_Click(object sender, EventArgs e)
        {
            if (gv.useTestSourceAndTargetFolder) // Don't save parms if using test folders to avoid overwriting real settings
                return;

            if (gv.initParm1List.Count > 0)
            {
                assignParmsFromTextBoxes();
                if (String.IsNullOrEmpty(gv.initParm1List[0].targetDir1))
                {
                    //gv.initParmItemList[0].targetDir = gv.dirTarget4Copy;
                    gv.initParm1List[0].SetTargetDir1(gv.initParm1List[0].targetDir2);
                }
                ff.saveInitParms1List(gv.inifileFullPathName, gv.initParm1List);
            }
        }

        private void btSetInitFilePath_Click(object sender, EventArgs e)
        {
            gv.inifileFullPathName = tbInitFile.Text;
            //gv.default_init_Path = Path.GetDirectoryName(gv.inifileFullPathName);
        }

        private void btDisplayGVars_Click(object sender, EventArgs e)
        {
            gv.mainWindow.assignInit1ParmsFromGlobals();
            if (gv.initParm1List.Count > 0)
                resetDGVParms();
        }

        private void btMyDocs_Click(object sender, EventArgs e)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            tbMyDocumentsFolder.Text = path;
            tb1.Text = path;
        }

        private void DialogParms_Activated(object sender, EventArgs e)
        {

        }

        private void btAssignParms_Click(object sender, EventArgs e)
        {
            assignParmsFromTextBoxes();
        }
        public void uncheckAll()
        {
            cb1.Checked = false;
            cb2.Checked = false;
            cb3.Checked = false;
            cb4.Checked = false;
            cb5.Checked = false;
            cb6.Checked = false;
            cbCopy.Checked = false;
            cbtarget1.Checked = false;
            cbtarget2.Checked = false;
            cbtarget3.Checked = false;
        }
        string temp;
        private void cb1_CheckedChanged(object sender, EventArgs e)
        {
            if (cb1.Checked)
            {
                if (cb2.Checked)
                {
                    temp = tbLastDir1.Text;
                    tbLastDir1.Text = tbLastDir2.Text;
                    tbLastDir2.Text = temp;
                    uncheckAll();
                }
                else if (cb3.Checked)
                {
                    temp = tbLastDir1.Text;
                    tbLastDir1.Text = tbLastDir3.Text;
                    tbLastDir3.Text = temp;
                    uncheckAll();
                }
                else if (cb4.Checked)
                {
                    temp = tbLastDir1.Text;
                    tbLastDir1.Text = tbLastDir4.Text;
                    tbLastDir4.Text = temp;
                    uncheckAll();
                }
                else if (cb5.Checked)
                {
                    temp = tbLastDir1.Text;
                    tbLastDir1.Text = tbLastDir5.Text;
                    tbLastDir5.Text = temp;
                    uncheckAll();
                }
                else if (cb6.Checked)
                {
                    temp = tbLastDir1.Text;
                    tbLastDir1.Text = tbLastDir6.Text;
                    tbLastDir6.Text = temp;
                    uncheckAll();
                }
                else if (cbCopy.Checked)
                {
                    tbLastDir1.Text = tbDirectoryPath.Text;
                    uncheckAll();
                }
            }
        }
        private void cb2_CheckedChanged(object sender, EventArgs e)
        {
            if (cb2.Checked)
            {
                if (cb1.Checked)
                {
                    temp = tbLastDir2.Text;
                    tbLastDir2.Text = tbLastDir1.Text;
                    tbLastDir1.Text = temp;
                    uncheckAll();
                }
                else if (cb3.Checked)
                {
                    temp = tbLastDir2.Text;
                    tbLastDir2.Text = tbLastDir3.Text;
                    tbLastDir3.Text = temp;
                    uncheckAll();
                }
                else if (cb4.Checked)
                {
                    temp = tbLastDir2.Text;
                    tbLastDir2.Text = tbLastDir4.Text;
                    tbLastDir4.Text = temp;
                    uncheckAll();
                }
                else if (cb5.Checked)
                {
                    temp = tbLastDir2.Text;
                    tbLastDir2.Text = tbLastDir5.Text;
                    tbLastDir5.Text = temp;
                    uncheckAll();
                }
                else if (cb6.Checked)
                {
                    temp = tbLastDir2.Text;
                    tbLastDir2.Text = tbLastDir6.Text;
                    tbLastDir6.Text = temp;
                    uncheckAll();
                }
                else if (cbCopy.Checked)
                {
                    tbLastDir2.Text = tbDirectoryPath.Text;
                    uncheckAll();
                }
            }

        }

        private void cb3_CheckedChanged(object sender, EventArgs e)
        {
            if (cb3.Checked)
            {
                if (cb1.Checked)
                {
                    temp = tbLastDir3.Text;
                    tbLastDir3.Text = tbLastDir1.Text;
                    tbLastDir1.Text = temp;
                    uncheckAll();
                }
                else if (cb2.Checked)
                {
                    temp = tbLastDir3.Text;
                    tbLastDir3.Text = tbLastDir2.Text;
                    tbLastDir2.Text = temp;
                    uncheckAll();
                }
                else if (cb4.Checked)
                {
                    temp = tbLastDir3.Text;
                    tbLastDir3.Text = tbLastDir4.Text;
                    tbLastDir4.Text = temp;
                    uncheckAll();
                }
                else if (cb5.Checked)
                {
                    temp = tbLastDir3.Text;
                    tbLastDir3.Text = tbLastDir5.Text;
                    tbLastDir5.Text = temp;
                    uncheckAll();
                }
                else if (cb6.Checked)
                {
                    temp = tbLastDir3.Text;
                    tbLastDir3.Text = tbLastDir6.Text;
                    tbLastDir6.Text = temp;
                    uncheckAll();
                }
                else if (cbCopy.Checked)
                {
                    tbLastDir3.Text = tbDirectoryPath.Text;
                    uncheckAll();
                }

            }

        }

        private void cb4_CheckedChanged(object sender, EventArgs e)
        {
            if (cb4.Checked)
            {
                if (cb1.Checked)
                {
                    temp = tbLastDir4.Text;
                    tbLastDir4.Text = tbLastDir1.Text;
                    tbLastDir1.Text = temp;
                    uncheckAll();
                }
                else if (cb2.Checked)
                {
                    temp = tbLastDir4.Text;
                    tbLastDir4.Text = tbLastDir2.Text;
                    tbLastDir2.Text = temp;
                    uncheckAll();
                }
                else if (cb3.Checked)
                {
                    temp = tbLastDir4.Text;
                    tbLastDir4.Text = tbLastDir3.Text;
                    tbLastDir3.Text = temp;
                    uncheckAll();
                }
                else if (cb5.Checked)
                {
                    temp = tbLastDir4.Text;
                    tbLastDir4.Text = tbLastDir5.Text;
                    tbLastDir5.Text = temp;
                    uncheckAll();
                }
                else if (cb6.Checked)
                {
                    temp = tbLastDir4.Text;
                    tbLastDir4.Text = tbLastDir6.Text;
                    tbLastDir6.Text = temp;
                    uncheckAll();
                }
                else if (cbCopy.Checked)
                {
                    tbLastDir4.Text = tbDirectoryPath.Text;
                    uncheckAll();
                }
            }

        }

        private void cb5_CheckedChanged(object sender, EventArgs e)
        {
            if (cb5.Checked)
            {
                if (cb1.Checked)
                {
                    temp = tbLastDir5.Text;
                    tbLastDir5.Text = tbLastDir1.Text;
                    tbLastDir1.Text = temp;
                    uncheckAll();
                }
                else if (cb2.Checked)
                {
                    temp = tbLastDir5.Text;
                    tbLastDir5.Text = tbLastDir2.Text;
                    tbLastDir2.Text = temp;
                    uncheckAll();
                }
                else if (cb3.Checked)
                {
                    temp = tbLastDir5.Text;
                    tbLastDir5.Text = tbLastDir3.Text;
                    tbLastDir3.Text = temp;
                    uncheckAll();
                }
                else if (cb4.Checked)
                {
                    temp = tbLastDir5.Text;
                    tbLastDir5.Text = tbLastDir4.Text;
                    tbLastDir4.Text = temp;
                    uncheckAll();
                }
                else if (cb6.Checked)
                {
                    temp = tbLastDir5.Text;
                    tbLastDir5.Text = tbLastDir6.Text;
                    tbLastDir6.Text = temp;
                    uncheckAll();
                }
                else if (cbCopy.Checked)
                {
                    tbLastDir5.Text = tbDirectoryPath.Text;
                    uncheckAll();
                }
            }
        }
        private void cb6_CheckedChanged(object sender, EventArgs e)
        {
            if (cb6.Checked)
            {
                if (cb1.Checked)
                {
                    temp = tbLastDir6.Text;
                    tbLastDir6.Text = tbLastDir1.Text;
                    tbLastDir1.Text = temp;
                    uncheckAll();
                }
                else if (cb2.Checked)
                {
                    temp = tbLastDir6.Text;
                    tbLastDir6.Text = tbLastDir2.Text;
                    tbLastDir2.Text = temp;
                    uncheckAll();
                }
                else if (cb3.Checked)
                {
                    temp = tbLastDir6.Text;
                    tbLastDir6.Text = tbLastDir3.Text;
                    tbLastDir3.Text = temp;
                    uncheckAll();
                }
                else if (cb4.Checked)
                {
                    temp = tbLastDir6.Text;
                    tbLastDir6.Text = tbLastDir4.Text;
                    tbLastDir4.Text = temp;
                    uncheckAll();
                }
                else if (cb5.Checked)
                {
                    temp = tbLastDir6.Text;
                    tbLastDir6.Text = tbLastDir5.Text;
                    tbLastDir5.Text = temp;
                    uncheckAll();
                }
                else if (cbCopy.Checked)
                {
                    tbLastDir6.Text = tbDirectoryPath.Text;
                    uncheckAll();
                }
            }
        }
        private void Cbtarget1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCopy.Checked)
            {
                tbTargetFolder1.Text = tbDirectoryPath.Text;
                uncheckAll();
                return;
            }
            if (cbtarget1.Checked)
            {
                if (cbtarget2.Checked)
                {
                    temp = tbTargetFolder1.Text;
                    tbTargetFolder1.Text = tbTargetFolder2.Text;
                    tbTargetFolder2.Text = temp;
                    cbtarget1.Checked = false;
                    cbtarget2.Checked = false;
                }
            }
        }

        private void Cbtarget2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCopy.Checked)
            {
                tbTargetFolder2.Text = tbDirectoryPath.Text;
                uncheckAll();
                return;
            }

            if (cbtarget1.Checked)
            {
                if (cbtarget2.Checked)
                {
                    temp = tbTargetFolder1.Text;
                    tbTargetFolder1.Text = tbTargetFolder2.Text;
                    tbTargetFolder2.Text = temp;
                    cbtarget1.Checked = false;
                    cbtarget2.Checked = false;
                    gv.initParm1List[0].SetTargetDir1(tbTargetFolder1.Text);
                    gv.initParm1List[0].targetDir2 = tbTargetFolder2.Text;
                    gv.mainWindow.setTargetFolder(tbTargetFolder1.Text);
                }
            }
        }
        private void cbtarget3_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCopy.Checked)
            {
                tbTargetFolder3.Text = tbDirectoryPath.Text;
                uncheckAll();
                return;
            }

            if (cbtarget1.Checked)
            {
                if (cbtarget2.Checked)
                {
                    temp = tbTargetFolder1.Text;
                    tbTargetFolder1.Text = tbTargetFolder2.Text;
                    tbTargetFolder2.Text = temp;
                    cbtarget1.Checked = false;
                    cbtarget2.Checked = false;
                    gv.initParm1List[0].SetTargetDir1(tbTargetFolder1.Text);
                    gv.initParm1List[0].targetDir3 = tbTargetFolder3.Text;
                    gv.mainWindow.setTargetFolder(tbTargetFolder1.Text);
                }
                else if (cbtarget3.Checked)
                {
                    temp = tbTargetFolder1.Text;
                    tbTargetFolder1.Text = tbTargetFolder3.Text;
                    tbTargetFolder3.Text = temp;
                    cbtarget1.Checked = false;
                    cbtarget3.Checked = false;
                    gv.initParm1List[0].SetTargetDir1(tbTargetFolder1.Text);
                    gv.initParm1List[0].targetDir3 = tbTargetFolder3.Text;
                    gv.mainWindow.setTargetFolder(tbTargetFolder1.Text);
                }
            }
            else if (cbtarget2.Checked)
            {
                if (cbtarget3.Checked)
                {
                    temp = tbTargetFolder2.Text;
                    tbTargetFolder2.Text = tbTargetFolder3.Text;
                    tbTargetFolder3.Text = temp;
                    cbtarget2.Checked = false;
                    cbtarget3.Checked = false;
                    gv.initParm1List[0].SetTargetDir1(tbTargetFolder2.Text);
                    gv.initParm1List[0].targetDir3 = tbTargetFolder3.Text;
                    gv.mainWindow.setTargetFolder(tbTargetFolder2.Text);
                }
            }
        }

        private void btUpdate1_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].SetWatchDir1(tbWatchFolder1.Text);
            gv.watchFolderPath = gv.initParm1List[0].targetDir1;
        }

        private void btUpdate2_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].targetDir2 = tbWatchFolder2.Text;
            gv.watchFolderPath = gv.initParm1List[0].targetDir2;
        }

        private void btSetTarget3_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].targetDir3 = tbWatchFolder3.Text;
            gv.watchFolderPath = gv.initParm1List[0].targetDir3;
        }
        public void GetFolder()
        {
            gv.directoryPromptDialog1 = new FolderBrowserDialog();
            var x = gv.directoryPromptDialog1.RootFolder;
            string dpath = null;

            dpath = gv.dirDialog(InitFolder.Special);///, dpath);// + "/";

            tbHistory1.Text = dpath;
            this.Refresh();
            tbHistory1.Refresh();
            //gv.debug.w("got folder ", dpath);
        }

        private void btGetFolder_Click(object sender, EventArgs e)
        {
            GetFolder();
        }

        private void btSetTarget1_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].SetTargetDir1(tbTargetFolder1.Text);
            gv.mainWindow.setTargetFolder(tbTargetFolder1.Text);
        }

        private void btSetTarget2_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].targetDir2 = tbTargetFolder2.Text;
            gv.mainWindow.setTargetFolder(tbTargetFolder2.Text);
        }

        private void btSetTarget3_Click_1(object sender, EventArgs e)
        {
            gv.initParm1List[0].targetDir3 = tbTargetFolder3.Text;
            gv.mainWindow.setTargetFolder(tbTargetFolder3.Text);
        }

        private void btSetWatch1_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].watchFolder1 = tbWatchFolder1.Text;
        }

        private void btSetWatch2_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].watchFolder2 = tbWatchFolder2.Text;
        }

        private void btSetWatch3_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].watchFolder3 = tbWatchFolder3.Text;
        }

        private void cmboSourceFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbDirectoryPath.Text = cmboSourceFolder.SelectedItem.ToString();
            int idx = cmboSourceFolder.SelectedIndex;
            tbDesc.Text = gv.folderHistoryList[idx].desc;
        }

        private void btVerifyMyDocs_Click(object sender, EventArgs e)
        {
            bool brc = ff.directoryExists(tbMyDocumentsFolder.Text);
            if (brc)
                MessageBox.Show($"Ini Folder does exist: {tbMyDocumentsFolder.Text}");
            else
                MessageBox.Show("Ini Folder was not found:" + tbMyDocumentsFolder.Text);
        }
        //tbInitFolderPath
        private void button1_Click(object sender, EventArgs e)
        {
            bool brc = ff.directoryExists(tbInitFolderPath.Text);
            if (brc)
            {
                MessageBox.Show($"Ini Folder does exist: {tbInitFolderPath.Text}");
                tbInitFolderPath.BackColor = Color.LightGreen;
            }
            else
            {
                MessageBox.Show("Ini Folder was not found:" + tbInitFolderPath.Text);
                tbInitFolderPath.BackColor = Color.LightCoral;
            }
        }
        public void VerifyIniFolder()
        {
            bool brc = ff.directoryExists(tbInitFolderPath.Text);
            if (brc)
            {
                tbInitFolderPath.BackColor = Color.LightGreen;
            }
            else
            {
                tbInitFolderPath.BackColor = Color.LightCoral;
            }
        }
        private void VerifyIniParmsFile()
        {
            bool brc = System.IO.File.Exists(gv.inifileFullPathName);
            if (brc)
            {
                tbInitFileName.BackColor = Color.LightGreen;
            }
            else
            {
                tbInitFileName.BackColor = Color.LightCoral;
            }
        }

        private void btUpdateMostRecent_Click(object sender, EventArgs e)
        {
            gv.initParm1List[0].mostRecent = tbMostRecent.Text;
        }
        DialogFolderAssignments assign;
        private void btFikderAssignments_Click(object sender, EventArgs e)
        {
            //cbShowDisplayNames.Checked = false;
            //lvScreenInfo.SendToBack();
            assign = new DialogFolderAssignments(gv);
            assign.Visible = true;
            assign.Activate();
        }
        private void tbInitFolderPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbNormalize_Click(object sender, EventArgs e)
        {
            //gv.folderHistoryList.OrderBy(file => file.folderPath).ToList();

        }

        private void cbSave_CheckedChanged(object sender, EventArgs e)
        {
            gv.initParm1List[0].mostRecent1 = tbHold1.Text;
            gv.initParm1List[0].mostRecent2 = tbHold2.Text;
            gv.initParm1List[0].mostRecent3 = tbHold3.Text;
            gv.initParm1List[0].mostRecent4 = tbHold4.Text;
            gv.initParm1List[0].mostRecent5 = tbHold5.Text;
            gv.initParm1List[0].mostRecent6 = tbHold6.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                tbMostRecentTEMP.Text = tbHold1.Text;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                tbMostRecentTEMP.Text = tbHold2.Text;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                tbMostRecentTEMP.Text = tbHold3.Text;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                tbMostRecentTEMP.Text = tbHold4.Text;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
                tbMostRecentTEMP.Text = tbHold5.Text;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                tbMostRecentTEMP.Text = tbHold6.Text;
        }
        //
        //
        String holdTemp;
        private void btSend1T_Click(object sender, EventArgs e)
        {
            holdTemp = tbHold1.Text;
            tbHold1.Text = tbMostRecentTEMP.Text;
            tbMostRecentTEMP.Text = holdTemp;
        }

        private void btSend2T_Click(object sender, EventArgs e)
        {
            holdTemp = tbHold2.Text;
            tbHold2.Text = tbMostRecentTEMP.Text;
            tbMostRecentTEMP.Text = holdTemp;
        }

        private void btSend3T_Click(object sender, EventArgs e)
        {
            holdTemp = tbHold3.Text;
            tbHold3.Text = tbMostRecentTEMP.Text;
            tbMostRecentTEMP.Text = holdTemp;
        }

        private void btSend4T_Click(object sender, EventArgs e)
        {
            holdTemp = tbHold3.Text;
            tbHold4.Text = tbMostRecentTEMP.Text;
            tbMostRecentTEMP.Text = holdTemp;
        }

        private void btSend5T_Click(object sender, EventArgs e)
        {
            holdTemp = tbHold5.Text;
            tbHold5.Text = tbMostRecentTEMP.Text;
            tbMostRecentTEMP.Text = holdTemp;
        }

        private void btSend6T_Click(object sender, EventArgs e)
        {
            holdTemp = tbHold6.Text;
            tbHold6.Text = tbMostRecentTEMP.Text;
            tbMostRecentTEMP.Text = holdTemp;
        }

        private void btMyNetwork_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            holdTemp = tbNew.Text;
            // tbHold6.Text = tbMostRecentTEMP.Text;
            tbMostRecentTEMP.Text = holdTemp;
        }
        public void VerifyFolderList()
        {
        }

        public void VerifyAndCleanFolderHistoryList()
        {
            // Filter out the folders that do not exist
            gv.folderHistoryList = gv.folderHistoryList.Where(item => Directory.Exists(item.folderPath)).ToList();
        }

        private void btVerifyFolders_Click(object sender, EventArgs e)
        {
            VerifyAndCleanFolderHistoryList();
            tbListCountAfter.Text = gv.folderHistoryList.Count.ToString();
        }

        private void btCopyPath_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetData(System.Windows.Forms.DataFormats.Text, (Object)cmboSourceFolder.Text);
            }
            catch (Exception)
            {

                gv.debug.w("clipboard error tbFpath_TextChanged");
            }
            tbTestPasteFP.Text = System.Windows.Forms.Clipboard.GetText();
        }

        private void btSaveDescription_Click(object sender, EventArgs e)
        {
            gv.mainWindow.SaveHistoryList();
        }

        private void tbDesc_TextChanged(object sender, EventArgs e)
        {
            int idx = cmboSourceFolder.SelectedIndex;
            gv.folderHistoryList[idx].desc = tbDesc.Text;
        }
        private void btSetRatingFileLocation_Click(object sender, EventArgs e)
        {
            PromptAndCreateRatingsFiles();
        }
        private void PromptAndCreateRatingsFiles()
        {
            // Prompt the user for the starting folder, default to gv.dazenMainFolder
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the starting folder for ratings files";
                folderDialog.SelectedPath = gv.dataFolder;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = folderDialog.SelectedPath;

                    // Ensure the folder exists
                    if (!Directory.Exists(selectedFolder))
                    {
                        Directory.CreateDirectory(selectedFolder);
                    }

                    // Define the full paths for the ratings files
                    string ratingsVideosPath = Path.Combine(selectedFolder, "ratingsVideos.xml");
                    string ratingsImagesPath = Path.Combine(selectedFolder, "ratingsImages.xml");

                    // Create the files if they do not exist
                    if (!File.Exists(ratingsVideosPath))
                    {
                        File.Create(ratingsVideosPath).Dispose();
                    }

                    if (!File.Exists(ratingsImagesPath))
                    {
                        File.Create(ratingsImagesPath).Dispose();
                    }

                    // Store the full path names in the global variables
                    gv.ratingsVideos = ratingsVideosPath;
                    gv.ratingsImages = ratingsImagesPath;
                }
            }
            tbRatingsVideo.Text = gv.ratingsVideos;
            tbRatingsImages.Text = gv.ratingsImages;
        }

        private void btGetRatingFiles_Click(object sender, EventArgs e)
        {
            tbRatingsVideo.Text = gv.ratingsVideos;
            tbRatingsImages.Text = gv.ratingsImages;
        }
        private void btVerifyRatingsFiles_Click(object sender, EventArgs e)
        {
            VerifyRatingsFilesExistence();
        }
        private void VerifyRatingsFilesExistence()
        {
            bool videosFileExists = File.Exists(gv.ratingsVideos);
            bool imagesFileExists = File.Exists(gv.ratingsImages);

            string message = $"Ratings Videos File: {(videosFileExists ? "Exists" : "Does Not Exist")}\n" +
                             $"Ratings Images File: {(imagesFileExists ? "Exists" : "Does Not Exist")}";

            MessageBox.Show(message, "Ratings Files Verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btDuplicateInitFile_Click(object sender, EventArgs e)
        {
            try
            {
                string src = gv?.inifileFullPathName;
                if (string.IsNullOrEmpty(src) || !File.Exists(src))
                {
                    MessageBox.Show($"Source init file was not found: {src}", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string newName = tbNewInitFileName.Text?.Trim();
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show("Please enter a new init file name in the textbox.", "Missing File Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Ensure only file name is used (keep same folder)
                newName = Path.GetFileName(newName);

                // If no extension provided, use source's extension
                if (string.IsNullOrEmpty(Path.GetExtension(newName)))
                {
                    string srcExt = Path.GetExtension(src);
                    newName = newName + srcExt;
                }

                string dir = Path.GetDirectoryName(src) ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string dest = Path.Combine(dir, newName);

                if (File.Exists(dest))
                {
                    var dr = MessageBox.Show($"The file already exists:\n{dest}\n\nDo you want to overwrite it?", "Confirm Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr != DialogResult.Yes)
                        return;
                }

                File.Copy(src, dest, true);
                MessageBox.Show($"Init file copied to:\n{dest}", "Copy Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying init file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbNewInitFileName_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string input = tbNewInitFileName.Text?.Trim();
                // Reference file to base increments on when textbox empty
                string reference = gv?.inifileFullPathName;
                if (string.IsNullOrEmpty(reference))
                    reference = GlobalVars.initFileName;;
                //                    reference = gv?reference = gv?.initFileName;

                // Choose filename to operate on
                string filename = input;
                if (string.IsNullOrEmpty(filename))
                {
                    filename = Path.GetFileName(reference ?? string.Empty) ?? string.Empty;
                }

                if (string.IsNullOrEmpty(filename))
                    return;

                // If user provided a path in the textbox, preserve it
                string pathPart = string.Empty;
                if (filename.IndexOf(Path.DirectorySeparatorChar) >= 0 || filename.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
                {
                    pathPart = Path.GetDirectoryName(filename) ?? string.Empty;
                    filename = Path.GetFileName(filename);
                }

                string nameNoExt = Path.GetFileNameWithoutExtension(filename);
                string ext = Path.GetExtension(filename);

                // Find trailing digits
                var m = System.Text.RegularExpressions.Regex.Match(nameNoExt, "(\\d+)$");
                string newName;
                if (m.Success)
                {
                    string digits = m.Groups[1].Value;
                    int width = digits.Length;
                    if (!int.TryParse(digits, out int num))
                        num = 0;
                    num++; // increment
                    string newDigits = num.ToString().PadLeft(width, '0');
                    newName = nameNoExt.Substring(0, nameNoExt.Length - digits.Length) + newDigits;
                }
                else
                {
                    // No trailing digits, append "1"
                    newName = nameNoExt + "1";
                }

                string resultFile = newName + ext;

                if (!string.IsNullOrEmpty(pathPart))
                    tbNewInitFileName.Text = Path.Combine(pathPart, resultFile);
                else
                    tbNewInitFileName.Text = resultFile;

                tbNewInitFileName.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error incrementing init file name: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        List<FolderHistory> folderHistoryList1;
        private void btOptimizeHistory_Click(object sender, EventArgs e)
        {
            folderHistoryList1 = gv.folderHistoryList.OrderBy(file => file.folderPath).ToList();
            gv.folderHistoryList = folderHistoryList1.OrderBy(file => file.folderPath).ToList();
            loadSourceCmboList();
        }


        private void cbSortByDate_CheckedChanged(object sender, EventArgs e)
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
                if (cbSortByDate.Checked)
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

        public void LoadDgvTargetHistory()
        {
            try
            {
                // Clear existing data
                dgvTargetHistory.DataSource = null;
                dgvTargetHistory.Rows.Clear();
                dgvTargetHistory.Columns.Clear();

                // Manually create columns
                dgvTargetHistory.Columns.Add("path", "Target Path");
                dgvTargetHistory.Columns.Add("lastUsed", "Last Used");

                // Build path to TargetHistory.xml
                string historyFilePath = System.IO.Path.Combine(gv.dataFolder, "TargetHistory.xml");

                if (!File.Exists(historyFilePath))
                {
                    gv?.debug?.w($"TargetHistory.xml not found at {historyFilePath}");
                    return;
                }

                // Load and parse the XML file
                var doc = System.Xml.Linq.XDocument.Load(historyFilePath);
                var targets = doc.Root?.Elements("Target")
                    .Select(e => new TargetHistory
                    {
                        Path = e.Element("Path")?.Value ?? string.Empty,
                        LastUsed = DateTime.TryParse(e.Element("LastUsed")?.Value, out var dt)
                            ? dt
                            : DateTime.MinValue
                    })
                    .OrderByDescending(t => t.LastUsed)
                    .ToList();

                if (targets == null || targets.Count == 0)
                {
                    gv?.debug?.w("No target entries found in TargetHistory.xml");
                    return;
                }

                // Populate rows
                foreach (var target in targets)
                {
                    dgvTargetHistory.Rows.Add(
                        target.Path,
                        target.LastUsed.ToString("yyyy-MM-dd HH:mm:ss")
                    );
                }

                // Auto-size columns
                foreach (DataGridViewColumn column in dgvTargetHistory.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                // Make the path column fill remaining space
                if (dgvTargetHistory.Columns.Count > 0)
                {
                    dgvTargetHistory.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                this.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading TargetHistory.xml:\n{ex.Message}",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                gv?.debug?.w("LoadDgvTargetHistory error", ex.Message);
            }
        }
        private void dgvTargetHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Ignore header row clicks
                if (e.RowIndex < 0)
                    return;

                // Get the folder path from the first column
                string folderPath = dgvTargetHistory.Rows[e.RowIndex].Cells[0].Value?.ToString();

                if (string.IsNullOrWhiteSpace(folderPath))
                    return;

                // Verify folder exists
                if (!Directory.Exists(folderPath))
                {
                    gv?.debug?.w($"Folder does not exist: {folderPath}");
                    return;
                }

                // Open File Explorer to the folder
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
            catch (Exception ex)
            {
                gv?.debug?.w("dgvTargetHistory_CellDoubleClick error", ex.Message);
            }
        }

        private void btSetTargetFromHistory_Click(object sender, EventArgs e)
        {
            AssignSelectedTargetFolder();
        }

        /// <summary>
        /// Assigns the currently selected folder from dgvTargetHistory to the target folder.
        /// Validates the folder exists before assignment.
        /// </summary>
        private void AssignSelectedTargetFolder()
        {
            // Use CurrentRow instead of SelectedRows
            if (dgvTargetHistory.CurrentRow == null)
            {
                MessageBox.Show("Please select a folder from the history list.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Get the current row
            DataGridViewRow selectedRow = dgvTargetHistory.CurrentRow;

            // Get the folder path from the selected row
            string folderPath = selectedRow.Cells[0].Value?.ToString();

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                MessageBox.Show("The selected folder path is empty.",
                    "Invalid Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Validate the folder exists
            if (!gv.mainWindow.VerifyFolderExists(folderPath))
            {
                MessageBox.Show($"The selected folder does not exist:\n{folderPath}",
                    "Folder Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Assign the folder using SetTargetDir1
            gv.initParm1List[0].SetTargetDir1(folderPath);
            
            // Optional: Show success message
            MessageBox.Show($"Target folder set to:\n{folderPath}", 
                "Success", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Information);
        }
        public void PerformHeavyInitialization()
        {
            // Example: Simulate slow work (replace with real logic)
            Thread.Sleep(1200); // Simulate delay

            // Load data, scan files, or perform other slow setup here.
            // For example:
            // LoadSettingsFromDisk();
            // ScanFolders();
            // InitializeLargeDataStructures();

            // If you need to update UI, use BeginInvoke or Invoke from the main thread.
            // Example:
            // this.BeginInvoke(new Action(() => { this.statusLabel.Text = "Initialization complete"; }));
        }
        /// <summary>
        /// Double-click handler for dgvTargetHistory to quickly assign selected folder
        /// </summary>
        private void dgvTargetHistory_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore header row clicks
            if (e.RowIndex < 0)
                return;

            // Get the folder path from the clicked row
            string folderPath = dgvTargetHistory.Rows[e.RowIndex].Cells[0].Value?.ToString();

            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            // Validate the folder exists
            if (!gv.mainWindow.VerifyFolderExists(folderPath))
            {
                MessageBox.Show($"The selected folder does not exist:\n{folderPath}",
                    "Folder Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Assign the folder
            gv.initParm1List[0].SetTargetDir1(folderPath);

            // Update the UI
            //tbTargetFolder.Text = folderPath;
           // tbTargetFolder.BackColor = Color.LightGreen;

           // tbMessage.Text = $"Target folder set to: {folderPath}";
          //  tbMessage.BackColor = Color.LightGreen;
        }
    }
    public class TargetHistory
    {
        public string Path { get; set; }
        public DateTime LastUsed { get; set; }
    }

}
