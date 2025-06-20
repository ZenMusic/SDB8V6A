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

    public partial class FileList : Form
    {
        List<FileInfoItem> imageFileList1;
        FileFunctions ff;
        GlobalVars gv;
        public List<FileInfoItem> finfoList = new List<FileInfoItem>();

        public FileList(GlobalVars g, FileFunctions fileFunctions)
        {
            InitializeComponent();
            gv = g;
            ff = fileFunctions;
            imageFileList1 = new List<FileInfoItem>();
        }
        public void AddItem(FileInfoItem finfoItem)
        {
            imageFileList1.Add(finfoItem);
            resetDGV();
        }
        
        //List<FileInfoItem>

        public void resetDGV()
        {
            dgv1.DataSource = null;
            dgv1.DataSource = imageFileList1;
            tbCount.Text = dgv1.RowCount.ToString();
        }
        int rowIdMovieList = 0;
        public FileInfoItem GetSelectedFileInfoItem()
        {
            string fname = dgv1.Rows[rowIdMovieList].Cells["fname"].Value.ToString();
            string ext = dgv1.Rows[rowIdMovieList].Cells["ext"].Value.ToString();
            string dpath = dgv1.Rows[rowIdMovieList].Cells["dpath"].Value.ToString();
            string fpath = dgv1.Rows[rowIdMovieList].Cells["fpath"].Value.ToString();
            string desc = dgv1.Rows[rowIdMovieList].Cells["desc"].Value.ToString();

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

        private void cbTopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = cbTopMost.Checked;
        }
        System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
        System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

        string imageFileName = "none";
        private void btSave_Click(object sender, EventArgs e) // XML save file 
        {
            InitializeSaveFileDialogXML();
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fpath = saveFileDialog1.FileName;
                imageFileName = Path.ChangeExtension(fpath, "xml");
                tbFileName.Text = imageFileName;
                SaveFile1();
            }
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
        bool bSavedFile = false;
       // ImageFileList imageFileList1;
        public void SaveFile1()
        {
            //XML Serializer 
            if (imageFileList1 == null)
            {
                MessageBox.Show("NULL ImageFileListOriginal");
                return;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoItem>));
            TextWriter textWriter = new StreamWriter(imageFileName);
            try
            {
                    serializer.Serialize(textWriter, imageFileList1);  // this returns    List<FileInfoItem> 
            }
            catch (Exception ex)
            {

                gv.debug.w("Serilize Exception ", ex.ToString(), ex.Message);
            }
            textWriter.Close();
            bSavedFile = true;
        }
        private void SaveDeleteFileToXML()
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fpath = Path.ChangeExtension(saveFileDialog1.FileName, "xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<FileInfoItem>));
                TextWriter textWriter = new StreamWriter(fpath);
                try
                {
                  //  serializer.Serialize(textWriter, GetDeleteList().getFinfo());
                }
                catch (Exception ex)
                {

                    gv.debug.w("Serilize Exception ", ex.ToString(), ex.Message);
                }
                textWriter.Close();

            }
        }
        private void btSaveDeleteList_Click(object sender, EventArgs e)
        {
            SaveFile1();
        }

        private void dgv1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                rowIdMovieList = (int)dgv1.CurrentCell.RowIndex;
                tbRowId.Text = rowIdMovieList.ToString();
            }
            catch
            {
                //this.debug.w("marketDataGrid No selected rows or empty set");
                rowIdMovieList = -1;
              //  tbMovieFpath.Text = "ERRORbbb";
                tbRowId.Text = "err";
                return;
            }
            GetSelectedFileInfoItem();
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
        
        bool bLoadedList = false;
        public void LoadImageListFile1()
        {
            InitializeOpenFileDialogXML();
            
            bLoadedList = true;
            //2022
            imageFileList1 = new List<FileInfoItem>();
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gv.setCursorHourGlass();
                //
                imageFileList1 = DeserializeFromXML(openFileDialog1.FileName);
                //
                imageFileName = openFileDialog1.FileName;
                tbFileName.Text = openFileDialog1.FileName;
              
                bSavedFile = true;
            }
            dgv1.DataSource = null;

            dgv1.DataSource = imageFileList1;
            gv.setCursorDefault();
            formatDataGridViewFileList();
        }
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

        private void tbLoad_Click(object sender, EventArgs e)
        {
            LoadImageListFile1();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
