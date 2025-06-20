using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Xml.Serialization;

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
        GlobalVars gv;
        //set root folder
        InitFolder startFolder = InitFolder.MyComputer; //starting Folder is set my radiobutton choice or by parm

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

        public int rowCount = 0; // row count in listView1

        private ListViewColumnSorter lvwColumnSorter;

        public TraverserDialog(GlobalVars g, Main mw, int imode) /////////////////////////////////////////////////////////////////////////
        {
            gv = g;
            InitializeComponent();
            main = mw;
            tbMax.Text = gv.iMaxFileCount.ToString();
            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            this.LVcs_ImageList.ListViewItemSorter = lvwColumnSorter;
            this.CenterToScreen();
            traverser = new TraverserBG(gv, this);
            CreateListView1();
            this.tbInfo.Text = "create ListView1";

            if (main != null)
            {
              //  if (main.getRootDir() == null)
                  //  gv.setRootFolder(0);
            }
            
            if (gv.initParms[0] != null)
                if (gv.initParms[0].pname.Equals("dirname"))
                {
                    gv.dirFullpath = gv.initParms[0].pvalue;
                    tbDirectoryPath.Text = gv.dirFullpath;
                }
        }
        

        // Enable 
        public TraverserDialog(GlobalVars g, string fpath)
        {
            InitializeComponent();
            gv = g;
            fullpath = fpath;
            traverser = new TraverserBG(gv, this);
            CreateListView1();
            this.tbInfo.Text = "new TraverserBG";

        }

        private void CreateListView1()
        {
            // Created a new ListView in Designer:
            // ListView LVcs_ImageList;
            //  private ListViewColumnSorter lvwColumnSorter;
            //  .... ALSO CREATED a ListViewColumnSorter and assigned 

            //  listView1.Bounds = new Rectangle(new Point(0, 0), new Size(1200, 400));

            LVcs_ImageList.View = View.Details;
            LVcs_ImageList.Scrollable = true;
            // Allow the user to edit item text.
            LVcs_ImageList.LabelEdit = false;
            // Allow the user to rearrange columns.
            LVcs_ImageList.AllowColumnReorder = true;
            // Display check boxes.
            LVcs_ImageList.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            LVcs_ImageList.FullRowSelect = true;
            // Display grid lines.
            LVcs_ImageList.GridLines = true;
            // Sort the items in the list in ascending order.
            LVcs_ImageList.Sorting = SortOrder.None;

            Rectangle wndBounds = this.Bounds;

          //  listViewLoc = LVcs_ImageList.Location;
          //  wndBounds.Height -= 250;
         //   wndBounds.Width -= 50;
          //  wndBounds.X = listViewLoc.X;
          //  wndBounds.Y = listViewLoc.Y;
          //  this.LVcs_ImageList.Bounds = wndBounds;

            // Create columns for the items and subitems.
            LVcs_ImageList.Columns.Add("fname", 300, HorizontalAlignment.Left);
            LVcs_ImageList.Columns.Add("rating", 60, HorizontalAlignment.Center);
            LVcs_ImageList.Columns.Add("deleted", 60, HorizontalAlignment.Center);
            LVcs_ImageList.Columns.Add("timestamp", 160, HorizontalAlignment.Center);
            LVcs_ImageList.Columns.Add("size", 100, HorizontalAlignment.Left);
            LVcs_ImageList.Columns.Add("fullpath", 400, HorizontalAlignment.Left);
            LVcs_ImageList.Columns.Add("order", 140, HorizontalAlignment.Left);

            //listviewX.ListView.Columns[ColumnToSort].Tag = "Text";
            //ColumnHeader header1 = listView1.InsertColumn(0, "Name", 10 * listView1.Font.SizeInPoints, HorizontalAlignment.Center);
            //Add the items to the ListView.
            // listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

            // Create two ImageList objects.
            ImageList imageListSmall = new ImageList();
            ImageList imageListLarge = new ImageList();

            // Initialize the ImageList objects with bitmaps.
            // imageListSmall.Images.Add(Bitmap.FromFile("C:\\01.jpg"));
            // imageListSmall.Images.Add(Bitmap.FromFile("C:\\01.bmp"));
            // imageListLarge.Images.Add(Bitmap.FromFile("C:\\01.jpg"));
            // imageListLarge.Images.Add(Bitmap.FromFile("C:\\01.bmp"));

            //Assign the ImageList objects to the ListView.
            //    listView1.LargeImageList = imageListLarge;
            //    listView1.SmallImageList = imageListSmall;

            // Add the ListView to the control collection.
            this.Controls.Add(LVcs_ImageList);

            LVcs_ImageList.Columns[0].Tag = "Text";
            LVcs_ImageList.Columns[1].Tag = "Text";
            LVcs_ImageList.Columns[2].Tag = "Integer";
            LVcs_ImageList.Columns[3].Tag = "Text";
            LVcs_ImageList.Columns[4].Tag = "Text";
            LVcs_ImageList.Columns[5].Tag = "Integer";
            LVcs_ImageList.Columns[6].Tag = "Integer";
        }
        //ListViewItem item1 = new ListViewItem("full path", 0);
        //ListViewItem item2 = new ListViewItem("file name", 1);
        //ListViewItem item3 = new ListViewItem("length", 2);
        //ListViewItem item4 = new ListViewItem("timestamp", 3);
        //ListViewItem item5 = new ListViewItem("width", 3);
        //ListViewItem item6 = new ListViewItem("height", 3);
        //ListViewItem item7 = new ListViewItem("rating", 3);
        //ListViewItem item8 = new ListViewItem("bDelete", 3);
        //ListViewItem item9 = new ListViewItem("bInvalid", 3);
        //ListViewItem item10 = new ListViewItem("folder", 3);
        //ListViewItem item11 = new ListViewItem("ext", 3);


        //"fname", "rating", "bDelete", "timestamp",  "size", "fullpath","order"

        public Boolean insertRange(ImageFileList fiList, int count)
        {
            ListViewItem[] itemlist = new ListViewItem[count];

            // // listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });
            for (int idx = 0; idx < count; ++idx)
            {
                FileInfoItem fi = fiList.getIndexed(idx);

                itemlist[idx] = new ListViewItem(fi.fname, idx);
                itemlist[idx].SubItems.Add(fi.rating.ToString());
                itemlist[idx].SubItems.Add(fi.bDelete.ToString());
                itemlist[idx].SubItems.Add(fi.stimestamp);
                itemlist[idx].SubItems.Add(fi.len.ToString());
                itemlist[idx].SubItems.Add(fi.fpath);
                itemlist[idx].SubItems.Add(idx.ToString());
            }
            LVcs_ImageList.Items.AddRange(itemlist);
            return true;
        }

        public Boolean insertRow(FileInfoItem finfo)
        {
            //-- NEW ITEM
            ListViewItem item1 = new ListViewItem(finfo.fname, 0);
            item1.Checked = true;
            string num = finfo.level.ToString();
            //-- SUB-ITEMS
            item1.SubItems.Add(finfo.rating.ToString());
            item1.SubItems.Add(finfo.bDelete.ToString());
            item1.SubItems.Add(finfo.stimestamp);
            item1.SubItems.Add(finfo.len.ToString());
            item1.SubItems.Add(finfo.fpath);
            //counter

            item1.SubItems.Add(LVcs_ImageList.Items.Count.ToString());
            ++rowCount;
            LVcs_ImageList.Items.Add(item1);
            return true;
        }

        
        
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void sorter()
        {
            ColumnHeader columnheader;		// Used for creating column headers.
            ListViewItem listviewitem;		// Used for creating listview items.

            // Ensure that the view is set to show details.
            LVcs_ImageList.View = View.Details;

            // Create some listview items consisting of first and last names.
            listviewitem = new ListViewItem("John");
            listviewitem.SubItems.Add("Smith");
            this.LVcs_ImageList.Items.Add(listviewitem);

            listviewitem = new ListViewItem("Bob");
            listviewitem.SubItems.Add("Taylor");
            this.LVcs_ImageList.Items.Add(listviewitem);

            listviewitem = new ListViewItem("Kim");
            listviewitem.SubItems.Add("Zimmerman");
            this.LVcs_ImageList.Items.Add(listviewitem);

            listviewitem = new ListViewItem("Olivia");
            listviewitem.SubItems.Add("Johnson");
            this.LVcs_ImageList.Items.Add(listviewitem);

            // Create some column headers for the data. 
            columnheader = new ColumnHeader();
            columnheader.Text = "First Name";
            this.LVcs_ImageList.Columns.Add(columnheader);

            columnheader = new ColumnHeader();
            columnheader.Text = "Last Name";
            this.LVcs_ImageList.Columns.Add(columnheader);

            // Loop through and size each column header to fit the column header text.
            foreach (ColumnHeader ch in this.LVcs_ImageList.Columns)
            {
                ch.Width = -2;
            }

        }

        public int loadListView() /////////////////////////////////////////// LOAD LIST VIEW ////////////////
        {
            // MessageBox.Show("==-=-=-=-=-=-=  RESULT SET SAMPLE FOLLOWS");
            this.tbCount.Text = gv.slideCount.ToString();
            // tbLoadedCount.Text = " 0 ";
            //  this.tbCount.Update();

            int maxrows = gv.slideCount > gv.iMaxFileCount ? gv.iMaxFileCount : gv.slideCount;

            rowCount = 0;

            for (int idx = 0; idx < maxrows; ++idx)
            {
                this.insertRow(gv.imageFileList.getIndexed(idx));

                if (true)
                {
                    tbLoadedCount.Text = idx.ToString();
                    tbLoadedCount.Update();
                }
            }
            //  tbLoadedCount.Text = maxrows.ToString();
            return gv.slideCount; // traverser.getResultList();
        }


        public int loadListViewRange() /////////////////////////////////////////// LOAD LIST VIEW all rows at once ////////////////
        {
            if (this.LVcs_ImageList.Items.Count > 0)
            {
                MessageBox.Show("LIST VIEW has data before loadListViewRange");
               //reloadListView();
               return LVcs_ImageList.Items.Count;
            }
            // MessageBox.Show("==-=-=-=-=-=-=  RESULT SET SAMPLE FOLLOWS");
            this.tbCount.Text = gv.slideCount.ToString();
            // tbLoadedCount.Text = " 0 ";
            //  this.tbCount.Update();

            int maxrows = gv.slideCount > gv.iMaxFileCount ? gv.iMaxFileCount : gv.slideCount;

            rowCount = 0;

            // insertRange(FileInfoList fiList, int count)  FileInfoList donotuse;

          

            this.insertRange(gv.imageFileList, maxrows);//    .insertRow(gv.imageFileList.getIndexed(idx));

                if (true)
                {
                    tbLoadedCount.Text = maxrows.ToString();
                    tbLoadedCount.Update();
                }
            
            //  tbLoadedCount.Text = maxrows.ToString();
            return gv.slideCount; // traverser.getResultList();
        }


        public int loadListViewItem(int start, int count) /////////////////////////////////////////// LOAD LIST VIEW //////////////// BACKGROUND 
        {
            int maxrows = gv.slideCount > gv.iMaxFileCount ? gv.iMaxFileCount : gv.slideCount;


            rowCount = 0;
            int top = start + count;
            int idx = start;
            for (idx = start; idx < top; ++idx)
            if (idx >= 0 && idx < maxrows)
            {
                this.insertRow(gv.imageFileList.getIndexed(idx));

                if (true)
                {
                    tbLoadedCount.Text = idx.ToString();
                }
            }
            else
            {
                this.cbDisplayListView.Checked = true;
            }
            //  tbLoadedCount.Text = maxrows.ToString();
            return idx; // traverser.getResultList();
        }

        public int reloadListView()
        {
            rowCount = 0;
            this.LVcs_ImageList.Clear();
            //this.CreateListView1();
            // this.insertTestRow();
            //  this.Update();
            //  MessageBox.Show("cleared");
            //  this.CreateListView1();
            //lvwColumnSorter.Order = SortOrder.None;
            //loadListViewRange();
            return LVcs_ImageList.Items.Count;
        }

        public int rebuildImageList() /////////////////////////////////////////// LOAD LIST VIEW ////////////////
        {

            gv.fileList2  = new ImageFileList();
            //replication LIST
            for (int idx = 0; idx < gv.imageFileList.getImageFileListLength(); ++idx)
            {
                gv.fileList2.addItem(gv.imageFileList.getIndexed(idx));
            }
            gv.debug.w("rebuildImageList");
            for (int idx = 0; idx < this.LVcs_ImageList.Items.Count; ++idx)
            {
                int jdx = Convert.ToInt32(LVcs_ImageList.Items[idx].SubItems[6].Text);
                gv.imageFileList.setIndexed(gv.fileList2.getIndexed(jdx), idx);
            }

            // reloadListView();
         //   MessageBox.Show("sort completed");
            return LVcs_ImageList.Items.Count;
        }

        private void insertTestRow()
        {
            // Create three items and three sets of subitems for each item.

            ListViewItem item1 = new ListViewItem("item1", 0);

            // Place a check mark next to the item.
            item1.Checked = true;
            item1.SubItems.Add("1");
            item1.SubItems.Add("2");
            item1.SubItems.Add("3");
            item1.SubItems.Add("4");
            item1.SubItems.Add("5");
            item1.SubItems.Add("6");

            LVcs_ImageList.Items.Add(item1);

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
            this.btTraverse.Enabled = true;
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

        private void filePrompt()
        {
            // Show the Open File dialog. If the user clicks OK, load the
            // picture that the user chose.
            gv.InitializeOpenFileDialog();
            //gv.SetOpenFileDialogXML();
            if (gv.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                gv.dirFullpath = gv.openFileDialog.FileName;
                this.tbFullPath.Text = gv.dirFullpath;
                string fname = Path.GetFileName(tbFullPath.Text);
                string dirpath = Path.GetDirectoryName(tbFullPath.Text);
                this.tbDirectoryPath.Text = dirpath;
                this.tbFileName.Text = fname;

            }
            this.LVcs_ImageList.Clear();
            CreateListView1();
        }


        private void buttonTraverse_Click(System.Object sender,
        System.EventArgs e)
        {
            // Reset the text in the result label.
            this.tbResult.Text = String.Empty;

            this.btTraverse.Enabled = false;
            this.btCancel.Enabled = true;



        }


        private void buttonFileDialog_Click_1(object sender, EventArgs e)
        {
            // Show the Open File dialog. If the user clicks OK, load the
            // picture that the user chose.
            gv.InitializeOpenFileDialog();
            if (gv.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.tbFullPath.Text = gv.openFileDialog.FileName;

                string fname = Path.GetFileName(tbFullPath.Text);
                string dirpath = Path.GetDirectoryName(tbFullPath.Text);
                this.tbDirectoryPath.Text = dirpath;
                this.tbFileName.Text = fname;
                gv.dirFullpath = dirpath;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

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

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCancel_Click_1(object sender, EventArgs e)
        {
            this.btTraverse.Enabled = true;
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
        private void buttonTraverse_Click_1(object sender, EventArgs e)
        {
            bTraversing.Visible = true;
            tbCount.BackColor = Color.White;
            gv.setCursorHourGlass();
            action = Action.TRAVERSING;
            TraverserBG traverser = new TraverserBG(gv, this);

            //uses gv.imageFileList

            if (gv.bImageFileListLoaded)
            {
                gv.imageFileList.clearList();
                gv.bImageFileListLoaded = false;
                
            }
            gv.imageFileList = new ImageFileList();

            //get 2 levels of subfolders for  %completion estimate   doEnumerateFiles(gv.dirFullpath);

            traverser.doTraverseFolders(gv.dirFullpath);

        }

        public void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gv.setCursorDefault();
            bTraversing.Visible = false;
            if (e.Cancelled)
            {
                this.tbResult.Text = "Cancelled";
                action = Action.NONE;
            }
            else
            {
              ////  main.directoryPath(tbDirectoryPath.Text + " " + gv.imageFileList.getIndexed(0));

                gv.mainWindow.setTitle(gv.imageFileList.getIndexed(0), tbDirectoryPath.Text);
                
                action = Action.COMPLETED;

                this.tbDirectoryPath.Text = e.Result.ToString();
                if (gv.messageCount > 0)
                    this.tbResult.Text = gv.getMessage();
                else
                    this.tbResult.Text = e.Result.ToString();

            }
            tbCount.Text = gv.slideCount.ToString();
            tbCount.BackColor = Color.Yellow;



            this.btTraverse.Enabled = true;
            //   this.buttonCancel.Enabled = false;
            this.progressBar1.Value = 100;

            if (cbDisplayListView.Checked)
            {
                this.loadListViewRange();
            }

        }

        public void OnCancel(string stat)
        {
            //backgroundWorker1.CancelAsync();
            bTraversing.Visible = false;
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
            if (gv.imageFileList != null)
            if (gv.imageFileList.getImageCount() > 0)
            gv.mainWindow.displayThisImage(gv.imageFileList.getIndexed(0));
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int index = e.ItemIndex;
            //   gv.debug.w("item changed", index);
            ListViewItem lvItem = e.Item;


            //    gv.debug.w(e.ItemIndex);
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
            this.Close();
        }

        public void setNoSort()
        {
            lvwColumnSorter.Order = SortOrder.None;
        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.LVcs_ImageList.Sort();

        }


        Boolean insertRangeB = true;

        private void btDisplayList_Click(object sender, EventArgs e)
        {
            gv.setCursorHourGlass();

            action = Action.LOADING_DISPLAY;

            this.tbResult.Text = "starting listView load...";

            if (!insertRangeB)
            {
                // Fire up a new thread to run some stuff in the background
                // on a different thread.

                this.LVcs_ImageList.BeginUpdate();
                LVcs_ImageList.SuspendLayout();

                System.Threading.Thread t = new System.Threading.Thread(LoaderThread);
                t.IsBackground = true;

                t.Start();
            }
            else
            {
                this.LVcs_ImageList.BeginUpdate();
                LVcs_ImageList.SuspendLayout();

                this.insertRange(gv.imageFileList, gv.slideCount);

                LVcs_ImageList.EndUpdate();
                LVcs_ImageList.ResumeLayout();
            }

        }



        private void LoaderThread()
        {
            // This code is running on a different thread, while it is running
            // the main GUI thread is still running code checking for mouse
            // clicks etc (you can close the form etc)

            int maxrows = gv.slideCount > gv.iMaxFileCount ? gv.iMaxFileCount : gv.slideCount;

            int count = 500;
            for (int i = 0; i <= maxrows; i+=count)
            {

                // Wait one second
                Thread.Sleep(1);
                // We can't do the following, since we are not running on the
                // thread which owns label1. If you try you will get an exception
                // indicating you are trying to do something on the wrong thread.
                // label1.Text = "This isn't safe, I am not on the main GUI thread";
                // Instead we have to do this, we wrap up the logic which
                // must be performed on the GUI thread into a method
                // and ask the Invoke method to cause the method to
                // be executed on the correct thread for us.

                this.LVcs_ImageList.Invoke(new UpdateListViewDelegate(UpdateListView), i, count);  //  label1.Invoke(new UpdateLabelDelegate(UpdateLabel), i.ToString());

            }
            
        }



        private delegate void UpdateListViewDelegate(int idx, int count);//string newText);



        private void UpdateListView(int idx, int count)//string newText)
        {

            // This is safe, the call to label1.Invoke above

            // ensured we are currently running on the main thread

            // and hence can update the GUI.

           // label1.Text = newText;

            loadListViewItem(idx, count);

        }

        private void cbDone_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.cbDisplayListView.Checked)
                this.LVcs_ImageList.EndUpdate();
        }

        private void cbDone_CheckedChanged(object sender, EventArgs e)
        {

        }



        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        OpenFileDialog openFileDialog1 = new OpenFileDialog();

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

        private void btSave_Click(object sender, EventArgs e)
        {
            if (!gv.bImageFileListLoaded)
                return;
            InitializeSaveFileDialogXML();
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbFileName.Text = saveFileDialog1.FileName;
                XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoItem>));
                TextWriter textWriter = new StreamWriter(saveFileDialog1.FileName);
                try
                {
                    serializer.Serialize(textWriter, gv.imageFileList.getFinfo());
                }
                catch (Exception ex)
                {

                    gv.debug.w("Serilize Exception ", ex.ToString(), ex.Message);
                }
                textWriter.Close();

            }
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


        private void btLoad_Click(object sender, EventArgs e)
        {
            InitializeOpenFileDialogXML();
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (gv.imageFileList == null)
                    gv.imageFileList = new ImageFileList();
                gv.imageFileList.finfo = DeserializeFromXML(openFileDialog1.FileName);
                tbFileName.Text = openFileDialog1.FileName;
                gv.nextIdx = 0;
                gv.bImageFileListLoaded = true;
                gv.slideCount = gv.imageFileList.getImageFileListLength();
                this.loadListViewRange();
            }

        }



        private void writeToTextFile()
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TextWriter tw = new StreamWriter(@saveFileDialog1.FileName);
                StringBuilder listViewContent = new StringBuilder();
                for (int item = 0; item < this.LVcs_ImageList.Items.Count; item++)
                {
                    for (int subitem = 0;
                       subitem < this.LVcs_ImageList.Columns.Count;
                       subitem++)
                    {
                        listViewContent.Append
                        (this.LVcs_ImageList.Items[item].SubItems[subitem].Text);
                        if (subitem < this.LVcs_ImageList.Columns.Count - 1)
                            listViewContent.Append(",");
                    }
                    tw.WriteLine(listViewContent);
                    listViewContent = new StringBuilder();
                }
                tw.Close();
            }
        }
        private void writeListView2File(string targetFile)
        {
            TextWriter tw = new StreamWriter(@targetFile);
            StringBuilder listViewContent = new StringBuilder();
            for (int item = 0; item < this.LVcs_ImageList.Items.Count; item++)
            {
                for (int subitem = 0;
                   subitem < this.LVcs_ImageList.Columns.Count;
                   subitem++)
                {
                    listViewContent.Append
                    (this.LVcs_ImageList.Items[item].SubItems[subitem].Text);
                    if (subitem < this.LVcs_ImageList.Columns.Count - 1)
                        listViewContent.Append(",");
                }
                tw.WriteLine(listViewContent);
                listViewContent = new StringBuilder();
            }
            tw.Close();
        }

        public void readImageFIle()
        {
            try
            {
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                    StreamReader m_streamReader = new StreamReader(fs);
                    // Read to the file using StreamReader class
                    m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    string strLine = m_streamReader.ReadLine();
                    int nStart = 0;
                    int count = 0;

                    ListViewItem newitem;// = new ListViewItem();

                    // Read each line of the stream and parse until last line is reached
                    while (strLine != null)
                    {
                        int nPos1 = strLine.IndexOf(",", nStart);
                        string str1 = strLine.Substring(0, nPos1); // get first column string NAME

                        nStart = nPos1 + 1;
                        int nPos2 = strLine.IndexOf(",", nStart);
                        string str2 = strLine.Substring(nStart, nPos2 - nStart); // get second column string  TYPE 
                        nStart = nPos2 + 1;
                        int nPos3 = strLine.IndexOf(",", nStart);
                        string str3 = strLine.Substring(nStart, nPos3 - nStart); // get last column string   0 ?
                        nStart = nPos3 + 1;
                        int nPos4 = strLine.IndexOf(",", nStart);
                        string str4 = strLine.Substring(nStart, nPos4 - nStart); // get last column string   PATHNAME
                        nStart = nPos4 + 1;
                        int nPos5 = strLine.IndexOf(",", nStart);
                        string str5 = strLine.Substring(nStart, nPos5 - nStart); // get last column string  DATE TIME
                        nStart = nPos5 + 1;
                        int nPos6 = strLine.IndexOf(",", nStart);
                        string str6 = strLine.Substring(nStart, nPos6 - nStart); // get last column string  LENGTH 
                        nStart = nPos6 + 1;
                        int nPos7 = strLine.IndexOf(",", nStart);
                        string str7 = strLine.Substring(nStart); // get last column string   IDX

                        //   (count, str1, 0, new string[] { str2, str3 }); // Add the row to the ListView

                        newitem = new ListViewItem(str1, 0);

                        newitem.SubItems.Add(str2);
                        newitem.SubItems.Add(str3);
                        newitem.SubItems.Add(str4);
                        newitem.SubItems.Add(str5);
                        newitem.SubItems.Add(str6);
                        newitem.SubItems.Add(str7);

                        LVcs_ImageList.Items.Add(newitem);
                        count++; // increment row
                        this.tbCount.Text = count.ToString();
                        nStart = 0; // reset
                        strLine = m_streamReader.ReadLine(); // get next line from the stream
                    }
                    // Close the stream
                    m_streamReader.Close();
                }
            }
            catch (Exception em)
            {
                MessageBox.Show(em.Message);
                Application.Exit();
            }
            this.tbResult.Text = "Read file";

        }
        private void btDBwrite_Click(object sender, EventArgs e)
        {
            // Get some sample data to populate list view with here...
            DataSet ds = new DataSet();
            DataTable tbl = new DataTable("tblTest");
            tbl.Columns.Add("ID").Unique = true; // the Primary Key of the record
            tbl.Columns.Add("Col1");
            for (int i = 1; i <= 20; i++)
            {
                DataRow rowNew = tbl.NewRow();

                rowNew["ID"] = "ID" + i.ToString();
                rowNew["Col1"] = "Number #" + i.ToString();
                tbl.Rows.Add(rowNew);
            }
            ds.Tables.Add(tbl);

            // Populate ListView
            foreach (DataRow row in tbl.Rows)
            {
                ListViewItem lvi = new ListViewItem();
                // ********* Store primary key ID of database record in the Tag of the ListViewItem *********:
                lvi.Tag = row["ID"];
                // Store display data here:
                lvi.Text = row["Col1"].ToString();
                this.LVcs_ImageList.Items.Add(lvi);
            }

            // Setup ListView properties:
            this.LVcs_ImageList.MultiSelect = false;

            // Setup ListView SelectedIndexChanged Event Handler:
            this.LVcs_ImageList.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
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
        private void rbTarot_CheckedChanged_1(object sender, EventArgs e)
        {
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
            else if (this.radioButton5.Checked)
                startFolder = InitFolder.MyDesktop;
            else if (this.rbTarot.Checked)
            {
                startFolder = InitFolder.Special;
                tbDirectoryPath.Text = @"C:";
            }
        }

        private void cbProgress_CheckedChanged(object sender, EventArgs e)
        {
            gv.bShowProgress = cbProgress.Checked;
        }

        private void cbTraverse_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btDirectoryDialog_Click(object sender, EventArgs e)
        {
            // RootSetter setRootDirectory = new RootSetter();

            this.LVcs_ImageList.Clear();
            CreateListView1();

            string dpath = gv.dirDialog(startFolder);// (startFolder);
            if (dpath != null)
            {
                if (Directory.Exists(dpath))
                {
                    gv.dirFullpath = dpath;
                    this.tbFullPath.Text = gv.dirFullpath;
                    string fname = "";
                    this.tbDirectoryPath.Text = dpath;
                    this.tbFileName.Text = "";

                    //  registryWriteDirectory(dpath);

                    if (this.cbTraverse.Checked)
                        if (!dpath.Equals("no directory"))
                            buttonTraverse_Click_1(null, null);

                }
                else
                    dpath = "no directory";
            }
            this.Text = dpath;
        }


    }
}
        
       
    



  /////////////////////////////////////////////////////////
