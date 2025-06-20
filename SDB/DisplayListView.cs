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
   public struct FileDialogInput22
   {
      public FileDialogInput22(string fpathIn)
      {
         this.fpath = fpathIn;
         this.fname = fpathIn;
      }
      public string fpath;
      public string fname;
   }

    public partial class DisplayListView : Form
    {
        GlobalVars gv;
        public string fullpath;
        const int MaxListViewRows = 9999;
        Point listViewLoc;

        public DisplayListView(GlobalVars g)
        {
            gv = g;
            InitializeComponent();
            CreateListView1();
        }

        // Enable 
        public DisplayListView(GlobalVars g, string fpath)
        {
            InitializeComponent();
            gv = g;
            fullpath = fpath;
            CreateListView1();

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public int loadListView() /////////////////////////////////////////// LOAD LIST VIEW ////////////////
        {
            
              //this.insertRow(gv.imageFileList.getIndexed(i));
                return 0;
        }

        public Boolean insertRow(FileInfoItem finfo)
        {
            ListViewItem item1 = new ListViewItem(finfo.fname, 0);
            // Place a check mark next to the item.
            item1.Checked = true;
            string num = finfo.level.ToString();
            item1.SubItems.Add(finfo.type);
            item1.SubItems.Add(finfo.level.ToString());
            item1.SubItems.Add(finfo.fpath);

            listView1.Items.Add(item1);

            
            return true;
        }
        

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

            listViewLoc = listView1.Location;
            wndBounds.Height -= 250;
            wndBounds.Width -= 50;
            wndBounds.X = listViewLoc.X;
            wndBounds.Y = listViewLoc.Y;
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



     

      //private int Add(int x, int y)
      //{
      //   Thread.Sleep(5000);
      //   return x + y;
      //}

      

    



      private void buttonFileDialog_Click(object sender, EventArgs e)
      {
          // Show the Open File dialog. If the user clicks OK, load the
          // picture that the user chose.
          gv.InitializeOpenFileDialog();
          if (gv.openFileDialog.ShowDialog() == DialogResult.OK)
          {
              
            
              
          }
          this.listView1.Clear();
          CreateListView1();
      }

       
       


        private void buttonFileDialog_Click_1(object sender, EventArgs e)
        {
            // Show the Open File dialog. If the user clicks OK, load the
            // picture that the user chose.
            gv.InitializeOpenFileDialog();
            if (gv.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                
                
             
              
            }
        }

       

        private void listView1_Resize(object sender, EventArgs e)
        {
          //  listView1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Bottom;
            
        }

      

        private void FormTraverser_Resize(object sender, EventArgs e)
        {
            Rectangle wndBounds = this.Bounds;

            
            this.listView1.Bounds = wndBounds;
        }

       

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        

      

       
       

       

        private void FormTraverser2_Resize(object sender, EventArgs e)
        {
            Rectangle mybounds = this.Bounds;

            

            this.listView1.Size = new Size((int) mybounds.Width, (int) mybounds.Height);
            this.listView1.Location = listViewLoc;
        }

       

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void FormTraverser2_FormClosed(object sender, FormClosedEventArgs e)
        {
           gv.mainWindow.ClearPB1Image();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int index = e.ItemIndex;
            gv.debug.w("item changed", index);
            ListViewItem lvItem = e.Item;


            gv.debug.w(e.ItemIndex);
            if (e.IsSelected)
            {
                gv.debug.winfo(e.Item);
                //gv.debug.w(e.Item.GetSubItemAt(0, 1).ToString());
            }

        }

        private void btTraverser1_Click(object sender, EventArgs e)
        {
            Traverser1 tv = new Traverser1();
            tv.Activate();
            tv.Visible = true;
        }

    }
}


  /////////////////////////////////////////////////////////
