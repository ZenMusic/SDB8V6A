using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SymbolDB
{
    public partial class DialogFolderAssignments : Form
    {
        GlobalVars gv;
        FileFunctions ff;

        InitFolder startFolder = InitFolder.MyComputer;
        bool bUseSpecial = true;

        public DialogFolderAssignments()
        {
            InitializeComponent();
        }
        public DialogFolderAssignments(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            ff = new FileFunctions(gv);
            //string[] driveinfo = ff.getDrivesInfo();
            ff.GetAllDrivesInfo();
        }

        private void btDirectoryDialog_Click(object sender, EventArgs e)
        {
            DialogFolderSelection getFolder = new DialogFolderSelection(gv, tbDirectoryPath);
            getFolder.Show();
            getFolder.Activate();
        }
        public void GetFolderPath()
        {
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
            if (!string.IsNullOrEmpty(dpath))
            {
                if (Directory.Exists(dpath))
                {
                    doTraversal(dpath);
                    gv.lastTraversedFolder = dpath;

                    //foreach (var mc in gv.initParmItemList.Where(x => x.lastDir == "lastDir"))
                    //  mc.Value = dpath;

                    //main.saveInitParms();
                }
                else
                    dpath = "no directory";
            }
            this.Text = dpath;
        }
        //
        // returns gv.imageFileList
        //
        private void doTraversal(string dpath)
        {
            if (string.IsNullOrEmpty(dpath))
                return;
            //2021
            //gv.initParm1List[0].sourceDir1 = dpath;
            //tbDirectoryPath.Text = gv.initParm1List[0].sourceDir1;
            tbDirectoryPath.Text = dpath;

            // registryWriteDirectory(dpath);

            gv.setCursorHourGlass();
            TraverserBG traverser = new TraverserBG(gv, null);
            //uses gv.imageFileList
            if (gv.bImageFileList1Loaded)
            {
                gv.imageFileList1.clearList();
                gv.bImageFileList1Loaded = false;

            }
            if (gv.bLoadingTraverser2)
                gv.imageFileList2 = new ImageFileList();
            else
            {
                gv.deleteFileList1 = new ImageFileList();
                gv.imageFileList1 = new ImageFileList();
            }
            //get 2 levels of subfolders for  %completion estimate   doEnumerateFiles(gv.dirFullpath);
            ARGS args = new ARGS();
            //args.dirpath = gv.initParm1List[0].sourceDir1;
            args.dirpath = dpath;

            traverser.doTraverseFolders(args);
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btEnterData_Click(object sender, EventArgs e)
        {
            dataGridViewConfig.DataSource = null;
            dataGridViewConfig.DataSource = gv.folderAssignmentList;
            formatDataGridViewFileList();
            this.dataGridViewConfig.EditMode = DataGridViewEditMode.EditOnEnter;


        }
        void formatDataGridViewFileList(int column0Width = 0) //2020
        {
            for (int idx = 0; idx < dataGridViewConfig.Columns.Count; idx++)
            {

                dataGridViewConfig.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                int colw = dataGridViewConfig.Columns[idx].Width;
                dataGridViewConfig.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridViewConfig.Columns[idx].Width = colw;
                if (idx == 0 && column0Width > 0)
                    dataGridViewConfig.Columns[idx].Width = column0Width;
                else if (idx == 4)
                    dataGridViewConfig.Columns[idx].Width = 6;
                else if (idx <= 6)
                    dataGridViewConfig.Columns[idx].ReadOnly = true;
            }
            return;
            //dgvFileList.Columns[0].Width = 350;
            //dgvFileList.Columns[1].Width = 600;
            int colCount2 = dataGridViewConfig.Columns.Count;
            int colCount = this.dataGridViewConfig.Columns.Count; // this returns the total number of columns (=6)
            //colCount = 3;                                                //MessageBox.Show(colCount.ToString());
            //colCount = colCount - 1; // =5
            for (int idx = 0; idx < colCount; idx++)
            {
                DataGridViewColumn column = dataGridViewConfig.Columns[idx]; // column[1] selects the required column 
                if (idx != 2)
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; // sets the AutoSizeMode of column defined in previous line
                else
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                int colWidth = column.Width; // store columns width after auto resize
                                             //MessageBox.Show(colWidth.ToString()); // show me the autoresize width (used as a visual check really)
                if (idx == 2)
                    colWidth = 42;
                //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // set the column resize mode to 'none' to allow manual/program changes
                //colWidth += 20; // add 20 pixels to what 'colWidth' already is
                this.dataGridViewConfig.Columns[idx].Width = colWidth; // set the columns width to the value stored in 'colWidth'

                //if ((idx == colCount - 1))
                //  column.DefaultCellStyle.Format = "yyyy.MM.dd HH:mm:ss";
            }
            this.Refresh();
        }
        int rowId = -1;

        private void dgvFolderAssignments_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                rowId = (int)dataGridViewConfig.CurrentCell.RowIndex;
                tbRowIdTemp.Text = rowId.ToString();
            }
            catch
            {
                //this.debug.w("marketDataGrid No selected rows or empty set");
                rowId = -1;
                tbPath.Text = "ERRORbbb";
                tbRowIdTemp.Text = "err";
                return;
            }
        }
    }
}
