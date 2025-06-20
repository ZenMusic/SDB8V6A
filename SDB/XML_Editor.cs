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
using System.Xml.Serialization;

namespace SymbolDB
{
    public partial class XML_Editor : Form
    {
        Color tagBackgroundColor = Color.Gray;
        GlobalVars gv;

        public XML_Editor(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            CreateListViewFinfo();
            initListViewFinfo();
            CreateListView1();
            CreateListViewTarot();

        }


        private void CreateListViewFinfo()
        {
            lvFinfo.View = View.Details;
            lvFinfo.Scrollable = true;
            // Allow the user to edit item text.
            lvFinfo.LabelEdit = false;
            // Allow the user to rearrange columns.
            lvFinfo.AllowColumnReorder = true;
            // Display check boxes.
            lvFinfo.CheckBoxes = false;
            // Select the item and subitems when selection is made.
            lvFinfo.FullRowSelect = true;
            // Display grid lines.
            lvFinfo.GridLines = true;
            // Sort the items in the list in ascending order.
            lvFinfo.Sorting = SortOrder.None;

            Rectangle wndBounds = this.Bounds;

            Boolean test = true;
            // Create columns for the items and subitems.
            lvFinfo.Columns.Add("parm", 80, HorizontalAlignment.Center);
            lvFinfo.Columns.Add("value", 2000, HorizontalAlignment.Left);

            lvFinfo.Columns[0].Tag = "String";
            lvFinfo.Columns[1].Tag = "String";


        }

        public Boolean initListViewFinfo()
        {
            ListViewItem item1 = new ListViewItem("full path", 0);
            item1.SubItems.Add(" ");
            lvFinfo.Items.Add(item1);

            ListViewItem item2 = new ListViewItem("file name", 1);
            item2.SubItems.Add(" ");
            lvFinfo.Items.Add(item2);
            ListViewItem item3 = new ListViewItem("length", 2);
            item3.SubItems.Add(" ");
            lvFinfo.Items.Add(item3);
            ListViewItem item4 = new ListViewItem("timestamp", 3);
            item4.SubItems.Add(" ");
            lvFinfo.Items.Add(item4);

            ListViewItem item5 = new ListViewItem("width", 3);
            item5.SubItems.Add(" ");
            lvFinfo.Items.Add(item5);
            ListViewItem item6 = new ListViewItem("height", 3);
            item6.SubItems.Add(" ");
            lvFinfo.Items.Add(item6);

            ListViewItem item7 = new ListViewItem("rating", 3);
            item7.SubItems.Add(" ");
            lvFinfo.Items.Add(item7);

            ListViewItem item8 = new ListViewItem("bDelete", 3);
            item8.SubItems.Add(" ");
            lvFinfo.Items.Add(item8);

            ListViewItem item9 = new ListViewItem("bInvalid", 3);
            item9.SubItems.Add(" ");
            lvFinfo.Items.Add(item9);

            ListViewItem item10 = new ListViewItem("folder", 3);
            item10.SubItems.Add(" ");
            lvFinfo.Items.Add(item10);

            ListViewItem item11 = new ListViewItem("ext", 3);
            item11.SubItems.Add(" ");
            lvFinfo.Items.Add(item11);

            return true;
        }

        public Boolean initLVFinfo()
        {
            // ListViewItem[] item = new ListViewItem[] ;
            string[] name = new string[] {
             "file name", 
             "folder",
             "full path", 
             "ext" ,
             "timestamp",
             "length",
             "width", 
             "height", 
             "rating",
             "bDelete", 
             "bInvalid"
            };

            ListViewItem item;
            for (int idx = 0; idx < name.Length; ++idx)
            {
                item = new ListViewItem(name[idx], idx);
                item.SubItems.Add(" ");
                lvFinfo.Items.Add(item);
            }
            return true;
        }

        public void updateDgvFinfo(FileInfoItem iinfo)
        {
            // 1) Update text fields, picture box, etc.
            tbDescription.Text = iinfo.desc;
            tbFpath.Text = iinfo.fpath;
            tbFileName.Text = Path.GetFileName(iinfo.fpath);

            try
            {
                pb1.Load(iinfo.fpath);
            }
            catch
            {
                // Handle image load exception as needed
            }

            pb1.SizeMode = PictureBoxSizeMode.Zoom;
            pb1.BorderStyle = BorderStyle.None;

            // 2) Prepare a list of (Property, Value) pairs:
            var dataRows = new List<(string Prop, string Value)>
    {
        ("File Name", iinfo.fname),
        ("Directory Path", iinfo.dpath),
        ("Full Path", iinfo.fpath),
        ("Extension", iinfo.ext),
        ("Timestamp", iinfo.stimestamp),
        ("Length", string.Format("{0:##,###,##0}", iinfo.len)),
        ("Width", pb1.Image?.Width.ToString() ?? "0"),
        ("Height", pb1.Image?.Height.ToString() ?? "0"),
        ("Rating", iinfo.rating.ToString()),
        ("Delete?", iinfo.bDelete.ToString()),
        ("Invalid?", iinfo.bInvalid.ToString())
    };

            // 3) Clear out existing rows in the DataGridView
            dgvFinfo.Rows.Clear();

            // 4) Add each pair as a new row
            foreach (var row in dataRows)
            {
                dgvFinfo.Rows.Add(row.Prop, row.Value);
            }

            // 5) Additional UI details
            // Equivalent to your tagFileName logic:
            if (iinfo.bInvalid)
                tagFileName.BackColor = Color.Red;
            else
                tagFileName.BackColor = tagBackgroundColor;

            // This 'idx' in your original code was used for row indexing
            // but here it's effectively replaced by dataRows.Count. 
            tbSlideNumber.Text = dataRows.Count.ToString();

            // If gv.slideCount1 was some global or external count, keep it:
            tbCount.Text = string.Format("{0:###,###,###}", gv.slideCount1);

            // Show/activate the form if desired
            this.Visible = true;
            this.Activate();
            this.Refresh();
        }

        public void updateLvFinfo(FileInfoItem iinfo)
        {
            int idx = 0;
            tbDescription.Text = iinfo.desc;
            try
            {
                this.pb1.Load(iinfo.fpath);
            }
            catch
            {

            }

            lvFinfo.Items[idx++].SubItems[1].Text = iinfo.fname;
            lvFinfo.Items[idx++].SubItems[1].Text = iinfo.dpath.ToString();
            lvFinfo.Items[idx++].SubItems[1].Text = iinfo.fpath;
            lvFinfo.Items[idx++].SubItems[1].Text = iinfo.ext.ToString();
            lvFinfo.Items[idx++].SubItems[1].Text = iinfo.stimestamp;
            lvFinfo.Items[idx++].SubItems[1].Text = String.Format("{0:##,###,##0}", iinfo.len);

            if (pb1.Image != null)
            {
                lvFinfo.Items[idx++].SubItems[1].Text = pb1.Image.Width.ToString();
                lvFinfo.Items[idx++].SubItems[1].Text = pb1.Image.Height.ToString();
            }
            else
            {
                lvFinfo.Items[idx++].SubItems[1].Text = "0";
                lvFinfo.Items[idx++].SubItems[1].Text = "0";
            }


            lvFinfo.Items[idx++].SubItems[1].Text = iinfo.rating.ToString();
            lvFinfo.Items[idx++].SubItems[1].Text = iinfo.bDelete.ToString();
            lvFinfo.Items[idx++].SubItems[1].Text = iinfo.bInvalid.ToString();

            //tagFileName.Text = iinfo.fname;
            if (iinfo.bInvalid)
            {
                tagFileName.BackColor = Color.Red;
            }
            else
                tagFileName.BackColor = tagBackgroundColor;
            tbSlideNumber.Text = String.Format("{0:###,###,##0}", idx );
            tbCount.Text = String.Format("{0:###,###,###}", gv.slideCount1);

            tbFpath.Text = iinfo.fpath;
            tbFileName.Text = Path.GetFileName(iinfo.fpath);

            this.Visible = true;
            this.Activate();

            //pb1.SizeMode = PictureBoxSizeMode.Normal;
            pb1.SizeMode = PictureBoxSizeMode.Zoom; //keeps aspect ratio
            //                pb1.SizeMode = PictureBoxSizeMode.CenterImage;

            pb1.BorderStyle = BorderStyle.None; // BorderStyle.Fixed3D;

            this.Refresh();
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
            LVcs_ImageList.Visible = false;
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (!gv.bImageFileList1Loaded)
                return;
            InitializeSaveFileDialogXML();
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fpath = saveFileDialog1.FileName;
                fpath = Path.ChangeExtension(fpath, "xml");

                //                string extension;
                //                extension = Path.GetExtension(fpath);
                //                gv.debug.w(String.Format("GetExtension('{0}') returns '{1}'", fpath, extension));

                //if (!extension.Equals("xml"))
                //  fpath += ".xml";
                tbFileName.Text = fpath;
                XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoItem>));
                TextWriter textWriter = new StreamWriter(fpath);
                try
                {
                    serializer.Serialize(textWriter, gv.imageFileList1.getFinfo());
                }
                catch (Exception ex)
                {

                    gv.debug.w("Serilize Exception ", ex.ToString(), ex.Message);
                }
                textWriter.Close();

            }
        }
        int idx = -1;
        private void btLoad_Click(object sender, EventArgs e)
        {
            InitializeOpenFileDialogXML();
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (gv.imageFileList1 == null)
                    gv.imageFileList1 = new ImageFileList();
                gv.setCursorHourGlass();
                gv.imageFileList1.finfoList = DeserializeFromXML(openFileDialog1.FileName);
                tbFileName.Text = openFileDialog1.FileName;
                gv.nextIdx = 0;
                gv.bImageFileList1Loaded = true;
                gv.slideCount1 = gv.imageFileList1.getImageFileListLength();
                this.loadListViewRange();
                if (gv.imageFileList1 != null)
                    updateLvFinfo(gv.imageFileList1.getIndexed(0)); //FileInfoItem
                updateDgvFinfo(gv.imageFileList1.getIndexed(0));

                idx = 0;

            }
            gv.setCursorDefault();
            tbIndex.Text = idx.ToString();
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

        List<FileInfoItem> DeserializeFromXMLInitParms(string fpath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<FileInfoItem>));
            TextReader textReader = new StreamReader(fpath);
            List<FileInfoItem> imageInfo;
            imageInfo = (List<FileInfoItem>)deserializer.Deserialize(textReader);
            textReader.Close();

            return imageInfo;
        }

        List<TarotCardInfo> DeserializeFromXMLTarot(string fpath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<TarotCardInfo>));
            TextReader textReader = new StreamReader(fpath);
            List<TarotCardInfo> tarotDeckInfo;
            tarotDeckInfo = (List<TarotCardInfo>)deserializer.Deserialize(textReader);
            textReader.Close();

            return tarotDeckInfo;
        }

        int rowCount = 0;

        public int loadListViewRange() /////////////////////////////////////////// LOAD LIST VIEW all rows at once ////////////////
        {
            if (this.LVcs_ImageList.Items.Count > 0)
            {
                MessageBox.Show("LIST VIEW has data before loadListViewRange");
                //reloadListView();
                return LVcs_ImageList.Items.Count;
            }
            // MessageBox.Show("==-=-=-=-=-=-=  RESULT SET SAMPLE FOLLOWS");
            this.tbCount.Text = gv.slideCount1.ToString();
            // tbLoadedCount.Text = " 0 ";
            //  this.tbCount.Update();

            int maxrows = gv.slideCount1 > gv.iMaxFileCount ? gv.iMaxFileCount : gv.slideCount1;

            rowCount = 0;

            // insertRange(FileInfoList fiList, int count)  FileInfoList donotuse;



            this.insertRange(gv.imageFileList1, maxrows);//    .insertRow(gv.imageFileList.getIndexed(idx));

            if (true)
            {
                tbLoadedCount.Text = maxrows.ToString();
                tbLoadedCount.Update();
            }

            //  tbLoadedCount.Text = maxrows.ToString();
            return gv.slideCount1; // traverser.getResultList();
        }

        public int loadListViewTarot() /////////////////////////////////////////// LOAD LIST VIEW all rows at once ////////////////
        {
            if (this.lvTarotInfo.Items.Count > 0)
            {
                MessageBox.Show("LIST VIEW has data before loadListViewRange");
                //reloadListView();
                return lvTarotInfo.Items.Count;
            }
            // MessageBox.Show("==-=-=-=-=-=-=  RESULT SET SAMPLE FOLLOWS");
            //this.tbCount.Text = gv.slideCount.ToString();
            // tbLoadedCount.Text = " 0 ";
            //  this.tbCount.Update();

            int maxrows = 10;

            rowCount = 0;

            // insertRange(FileInfoList fiList, int count)  FileInfoList donotuse;



            insertTarot(tarotDeckInfo, maxrows);//    .insertRow(gv.imageFileList.getIndexed(idx));


            //  tbLoadedCount.Text = maxrows.ToString();
            return gv.slideCount1; // traverser.getResultList();
        }

        public Boolean insertTarot(List<TarotCardInfo> fiList, int count)
        {
            ListViewItem[] itemlist = new ListViewItem[count];

            // // listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });
            int idx = 0;
            foreach (TarotCardInfo tc in fiList)
            {

                if (idx > 9)
                    break;
                itemlist[idx] = new ListViewItem(tc.Index.ToString());
                itemlist[idx].SubItems.Add(tc.Notes.ToString());
                if (tc.NotesReversed == null)
                    itemlist[idx].SubItems.Add(" ");
                else
                itemlist[idx].SubItems.Add(tc.NotesReversed.ToString());
                ++idx;
            }
             lvTarotInfo.Items.AddRange(itemlist);
            return true;
        }



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
                if (fi.desc == null)
                    fi.desc = "";
                itemlist[idx].SubItems.Add(fi.desc.ToString());
            }
            LVcs_ImageList.Items.AddRange(itemlist);
            return true;
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

        private void CreateListViewTarot()
        {
            // Created a new ListView in Designer:
            // ListView LVcs_ImageList;
            //  private ListViewColumnSorter lvwColumnSorter;
            //  .... ALSO CREATED a ListViewColumnSorter and assigned 

            //  listView1.Bounds = new Rectangle(new Point(0, 0), new Size(1200, 400));

            lvTarotInfo.View = View.Details;
            lvTarotInfo.Scrollable = true;
            // Allow the user to edit item text.
            lvTarotInfo.LabelEdit = false;
            // Allow the user to rearrange columns.
            lvTarotInfo.AllowColumnReorder = true;
            // Display check boxes.
            lvTarotInfo.CheckBoxes = true;
            // Select the item and subitems when selection is made.
            lvTarotInfo.FullRowSelect = true;
            // Display grid lines.
            lvTarotInfo.GridLines = true;
            // Sort the items in the list in ascending order.
            lvTarotInfo.Sorting = SortOrder.None;

            Rectangle wndBounds = this.Bounds;

            //  listViewLoc = LVcs_ImageList.Location;
            //  wndBounds.Height -= 250;
            //   wndBounds.Width -= 50;
            //  wndBounds.X = listViewLoc.X;
            //  wndBounds.Y = listViewLoc.Y;
            //  this.LVcs_ImageList.Bounds = wndBounds;

            // Create columns for the items and subitems.
            lvTarotInfo.Columns.Add("index", 300, HorizontalAlignment.Left);
            lvTarotInfo.Columns.Add("desc", 600, HorizontalAlignment.Center);
            lvTarotInfo.Columns.Add("reversed", 600, HorizontalAlignment.Center);

            //listviewX.ListView.Columns[ColumnToSort].Tag = "Text";
            //ColumnHeader header1 = listView1.InsertColumn(0, "Name", 10 * listView1.Font.SizeInPoints, HorizontalAlignment.Center);
            //Add the items to the ListView.
            // listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3 });

            this.Controls.Add(lvTarotInfo);

            lvTarotInfo.Columns[0].Tag = "Integer";
            lvTarotInfo.Columns[1].Tag = "Text";
            lvTarotInfo.Columns[2].Tag = "Text";
        }

        public TarotInfo2 tci;
        List<TarotCardInfo> tarotDeckInfo;
        private void btLoadTarot_Click(object sender, EventArgs e)
        {
            InitializeOpenFileDialogXML();
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (tci == null)
                    tci = new TarotInfo2();
                       
                gv.setCursorHourGlass();
                tarotDeckInfo =  DeserializeFromXMLTarot(openFileDialog1.FileName);
                tbFileName.Text = openFileDialog1.FileName;
            }
            gv.setCursorDefault();
            loadListViewTarot();
        }

        private void btNext_Click(object sender, EventArgs e)
        {
         //   gv.imageFileList.finfo[0].
            if (idx < gv.imageFileList1.finfoList.Count - 1)
            {
                ++idx;
                updateLvFinfo(gv.imageFileList1.getIndexed(idx)); //FileInfoItem

                updateDgvFinfo(gv.imageFileList1.getIndexed(idx));
                tbIndex.Text = idx.ToString();
            }
        }

        private void btPrevious_Click(object sender, EventArgs e)
        {
            if (idx > 0)
            {
                --idx;
                updateLvFinfo(gv.imageFileList1.getIndexed(idx)); //FileInfoItem
                updateDgvFinfo(gv.imageFileList1.getIndexed(idx));
                tbIndex.Text = idx.ToString();
            }
        }

        private void pb1_Click(object sender, EventArgs e)
        {

        }

        private void btSetDescription_Click(object sender, EventArgs e)
        {
            gv.imageFileList1.finfoList[idx].desc = tbDescription.Text;
        }
    }

}
