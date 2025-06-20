using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class DialogParms : Form
    {
        GlobalVars gv;
        FileFunctions ff;
        //GetDotNetVersion getDotNetVersion = new GetDotNetVersion();

        public DialogParms()
        {
            InitializeComponent();
        }
        public DialogParms(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            ff = new FileFunctions(gv);
            //string[] driveinfo = ff.getDrivesInfo();
            ff.GetAllDrivesInfo();

            tbInitFile.Text = gv.inifileFullPathName;

            CreateLvDirectoryList(gv);

            tbDotNetVersion.Text = GetDotNetVersion.Get45PlusVersionFromRegistry();
            //displayAppParms();            gv.debug.w("read ini file :");
            // ff.writeIniFile
            resetDGVParms();
            loadSourceCmboList();
            LoadMostRecentList();
            tbMyDocumentsFolder.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            tbInitFolderPath.Text = tbMyDocumentsFolder.Text + @gv.dazenFolderName;
            
            string fileName = gv.mainWindow.CreateNotesFileName(1);
            string fpath = gv.myDataFolder + "/" + fileName;
            tbNotesFile.Text = fpath;
            tbInitFileName.Text = gv.initFileName2;
        }
        public void loadSourceCmboList()
        {
            cmboSourceFolder.Items.Clear();
            int idx = 0;
            for (idx = 0; idx < gv.folderHistoryList.Count; ++idx)
            {
                cmboSourceFolder.Items.Add(gv.folderHistoryList[idx].folderPath);
            }
            cmboSourceFolder.SelectedIndex = 0;
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
                MessageBox.Show($"Ini Parameters file does exist {gv.inifileFullPathName}");
            else
                MessageBox.Show($"Ini Parameters file was not found {gv.inifileFullPathName}");

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
            gv.default_init_Path = Path.GetDirectoryName(gv.inifileFullPathName);
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
                    tbLastDir6.Text = tbLastDir6.Text;
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
                    gv.initParm1List[0].targetDir2 = tbTargetFolder2.Text;
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

                    dpath = gv.dirDialog(InitFolder.Special);///, dpath);// + "/");

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

        private void button1_Click(object sender, EventArgs e)
        {
            bool brc = ff.directoryExists(tbInitFolderPath.Text);
            if (brc)
                MessageBox.Show($"Ini Folder does exist: {tbInitFolderPath.Text}");
            else
                MessageBox.Show("Ini Folder was not found:" + tbInitFolderPath.Text);
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
        List<FolderHistory> folderHistoryList1;
        private void btOptimizeHistory_Click(object sender, EventArgs e)
        {
            folderHistoryList1 = gv.folderHistoryList.OrderBy(file => file.folderPath).ToList();
            gv.folderHistoryList = folderHistoryList1.OrderBy(file => file.folderPath).ToList();
            loadSourceCmboList();
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
    }
}
