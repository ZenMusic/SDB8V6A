using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SymbolDB
{
   public struct FileDialogInput2
   {
      public FileDialogInput2(string fpathIn)
      {
         this.fpath = fpathIn;
         this.fname = fpathIn;
      }
      public string fpath;
      public string fname;
   }

    public partial class FormTraverser2 : Form
    {
        GlobalVars gv;
        TraverserBG traverser;
        public string fullpath;
        const int MaxListViewRows = 9999;
        

        public FormTraverser2(GlobalVars g)
        {
        //ListView listView1 = new ListView();
            gv = g;
            string fullpath;

            InitializeComponent();

            traverser = new TraverserBG(gv, this);
            CreateListView1();
            
        }

        

        // Enable 
        public FormTraverser2(GlobalVars g, string fpath)
        {
            InitializeComponent();
            gv = g;
            fullpath = fpath;
            traverser = new TraverserBG(gv, this);
            CreateListView1();

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public int loadListView() /////////////////////////////////////////// LOAD LIST VIEW ////////////////
        {
           // MessageBox.Show("==-=-=-=-=-=-=  RESULT SET SAMPLE FOLLOWS");
            this.tbCount.Text = gv.fileCount.ToString();

            this.tbCount.Update();

            int maxrows = gv.fileCount > MaxListViewRows ? MaxListViewRows : gv.fileCount;

            for (int i = 0; i < maxrows; ++i)
            {
                this.insertRow(gv.imageFileList.getIndexed(i));

                if (true)
                {
                    tbLoadedCount.Text = i.ToString();
                    tbLoadedCount.Update();
                }
            }
            tbLoadedCount.Text = maxrows.ToString();
            return gv.fileCount; // traverser.getResultList();
        }

        public Boolean insertRow(FileInfoItem finfo)
        {
            // return insertRow(finfo.fname, finfo.type, finfo.level, finfo.fpath);
            string fn2 = String.Copy(finfo.fpath);

            string fn;
            if (finfo.type.Equals("d"))
               // fn = "dir";
                fn = Path.GetFileName(finfo.fname);
            else
                fn = Path.GetFileName(finfo.fname);

            string result;

            result = Path.GetFileName(@finfo.fpath);
            Console.WriteLine("GetFileName('{0}') returns '{1}'",
                @finfo.fpath, result);

            ListViewItem item1 = new ListViewItem(fn, 0);
            // Place a check mark next to the item.
            item1.Checked = true;
            string num = finfo.level.ToString();
            item1.SubItems.Add(finfo.type);
            item1.SubItems.Add(finfo.level.ToString());
            item1.SubItems.Add(finfo.fname);

            listView1.Items.Add(item1);
            return true;
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
        



        

        public Boolean insertRow(string fname, string type, int level, string fpath)
        {
            string fn2 =  String.Copy(fpath);

            string fn ;
            if (type.Equals("d"))
                fn = "dir";
            else
                fn = Path.GetFileName(fn2);
            ListViewItem item1 = new ListViewItem(fn, 0);
            // Place a check mark next to the item.
            item1.Checked = true;
            string num = level.ToString();
            item1.SubItems.Add(type);
            item1.SubItems.Add(num);
            item1.SubItems.Add(fname);

            listView1.Items.Add(item1);
            return true;
        }

        private void CreateListView1()
        {
            // Created a new ListView control at top:
            // ListView listView1 = new ListView();
           
          //  listView1.Bounds = new Rectangle(new Point(0, 0), new Size(1200, 400));

            // Set the view to show details.
            listView1.View = View.Details;
            listView1.Scrollable = true;
            // Allow the user to edit item text.
            listView1.LabelEdit = false;
            // Allow the user to rearrange columns.
            listView1.AllowColumnReorder = true;
            // Display check boxes.
            listView1.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            listView1.FullRowSelect = true;
            // Display grid lines.
            listView1.GridLines = true;
            // Sort the items in the list in ascending order.
            listView1.Sorting = SortOrder.None;


            Rectangle wndBounds = this.Bounds;

            Point loc = tbLength.Location;
            wndBounds.Height -= 250;
            wndBounds.Width -= 50;
            wndBounds.X = loc.X;
            wndBounds.Y = loc.Y + 40;
            this.listView1.Bounds = wndBounds;


            // Create three items and three sets of subitems for each item.
            ListViewItem item1 = new ListViewItem("item1", 0);
            // Place a check mark next to the item.
            item1.Checked = true;
            item1.SubItems.Add("1");
            item1.SubItems.Add("2");
            item1.SubItems.Add("3");
            ListViewItem item2 = new ListViewItem("item2", 1);
            item2.SubItems.Add("4");
            item2.SubItems.Add("5");
            item2.SubItems.Add("6");
            ListViewItem item3 = new ListViewItem("item3", 0);
            // Place a check mark next to the item.
            item3.Checked = true;
            item3.SubItems.Add("7");
            item3.SubItems.Add("8");
            item3.SubItems.Add("9");

            // Create columns for the items and subitems.
            listView1.Columns.Add("fname",300 , HorizontalAlignment.Left);
            listView1.Columns.Add("type", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("level", 40, HorizontalAlignment.Center);
            listView1.Columns.Add("fullpath", -2, HorizontalAlignment.Left);


           
            //ColumnHeader header1 = listView1.InsertColumn(0, "Name", 10 * listView1.Font.SizeInPoints, HorizontalAlignment.Center);

            //Add the items to the ListView.
           // listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

           // listView1.Items.Add(item1);
            
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
            this.Controls.Add(listView1);
        }
        private void CreateListView2()
        {
            // Create a new ListView control.
            ListView listView1 = new ListView();
            listView1.Bounds = new Rectangle(new Point(110, 110), new Size(600, 600));

            // Set the view to show details.
            listView1.View = View.Details;
            // Allow the user to edit item text.
            listView1.LabelEdit = true;
            // Allow the user to rearrange columns.
            listView1.AllowColumnReorder = true;
            // Display check boxes.
            listView1.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            listView1.FullRowSelect = true;
            // Display grid lines.
            listView1.GridLines = true;
            // Sort the items in the list in ascending order.
            listView1.Sorting = SortOrder.Ascending;

            listView1.Location = new Point(110, 110);

            listView1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Bottom;
            //btTopLeft.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;


            // Create three items and three sets of subitems for each item.
            ListViewItem item1 = new ListViewItem("item1", 0);
            // Place a check mark next to the item.
            item1.Checked = true;
            item1.SubItems.Add("1");
            item1.SubItems.Add("2");
            item1.SubItems.Add("3");
            ListViewItem item2 = new ListViewItem("item2", 1);
            item2.SubItems.Add("4");
            item2.SubItems.Add("5");
            item2.SubItems.Add("6");
            ListViewItem item3 = new ListViewItem("item3", 0);
            // Place a check mark next to the item.
            item3.Checked = true;
            item3.SubItems.Add("7");
            item3.SubItems.Add("8");
            item3.SubItems.Add("9");

            // Create columns for the items and subitems.
            listView1.Columns.Add("Item Column", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Column 2", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Column 3", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Column 4", -2, HorizontalAlignment.Center);

            //Add the items to the ListView.
            listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

            // Create two ImageList objects.
            ImageList imageListSmall = new ImageList();
            ImageList imageListLarge = new ImageList();

            // Initialize the ImageList objects with bitmaps.
            imageListSmall.Images.Add(Bitmap.FromFile("C:\\01.jpg"));
            imageListSmall.Images.Add(Bitmap.FromFile("C:\\01.bmp"));
            imageListLarge.Images.Add(Bitmap.FromFile("C:\\01.jpg"));
            imageListLarge.Images.Add(Bitmap.FromFile("C:\\01.bmp"));

            //Assign the ImageList objects to the ListView.
            listView1.LargeImageList = imageListLarge;
            listView1.SmallImageList = imageListSmall;

            // Add the ListView to the control collection.
            this.Controls.Add(listView1);
        }



      private void OnCalculate(object sender, EventArgs e)
      {
         this.buttonTraverse.Enabled = true;
         this.textBox1.Text = String.Empty;
         this.buttonCancel.Enabled = true;
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


      private void buttonFileDialog_Click(object sender, EventArgs e)
      {
          // Show the Open File dialog. If the user clicks OK, load the
          // picture that the user chose.
          gv.InitializeOpenFileDialog();
          if (gv.openFileDialog.ShowDialog() == DialogResult.OK)
          {
              this.tbFullPath.Text = gv.openFileDialog.FileName;

              string fname = Path.GetFileName(tbFullPath.Text);
              string dirpath = Path.GetDirectoryName(tbFullPath.Text);
              this.textBox1.Text = dirpath;
              this.tbFileName.Text = fname;
          }
          this.listView1.Clear();
          CreateListView1();
      }

       
        private void buttonTraverse_Click(System.Object sender,
        System.EventArgs e)
        {
            // Reset the text in the result label.
            this.textBoxResult.Text = String.Empty;

            this.buttonTraverse.Enabled = false;
            this.buttonCancel.Enabled = true;

            
            
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
                this.textBox1.Text = dirpath;
                this.tbFileName.Text = fname;
                gv.fullpath = dirpath;
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
            Rectangle wndBounds = this.Bounds;

            Point loc = tbLength.Location;
            wndBounds.Height -= 250;
            wndBounds.Width -= 50;
            wndBounds.X = loc.X;
            wndBounds.Y = loc.Y + 40;
            this.listView1.Bounds = wndBounds;
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
            this.buttonTraverse.Enabled = true;
            this.buttonCancel.Enabled = false;
            this.textBox1.Text = ".... cancelling operation ... ";
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

        private void buttonTraverse_Click_1(object sender, EventArgs e)
        {
            TraverserBG traverser = new TraverserBG(gv, this);
           

            traverser.doTraverseFolders((string) this.textBox1.Text);


        }
        public void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.textBoxResult.Text = "Cancelled";
            }
            else
            {
                this.textBoxResult.Text = e.Result.ToString();
            }
            this.buttonTraverse.Enabled = true;
            this.buttonCancel.Enabled = false;
            // this.progressBar.Value = 100;
            loadListView();
        }

        public void OnCancel(string stat)
        {
            //backgroundWorker1.CancelAsync();
            this.textBox1.Text = "Cancelled.";
        }

        public void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //this.progressBar.Value = e.ProgressPercentage;
        }

        private void FormTraverser2_Resize(object sender, EventArgs e)
        {
            Rectangle mybounds = this.Bounds;

            mybounds.Width -= 50;

            Point xy = this.tbLength.Location;

            mybounds.Height -= xy.Y;

            mybounds.Height -= 70;
            xy.Y += 40;

            this.listView1.Size = new Size((int) mybounds.Width, (int) mybounds.Height);
            this.listView1.Location = xy;
        }

    }
}


  /////////////////////////////////////////////////////////
