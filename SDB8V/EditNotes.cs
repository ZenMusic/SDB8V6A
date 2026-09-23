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
    public partial class EditNotes : Form
    {
        private readonly GlobalVars gv;
        public EditNotes(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            LoadText();
        }

        private GroupBox gbFont;
        private RadioButton btTahoma;
        private RadioButton rbSegoeUI;
        private RadioButton rbArial;
        private RichTextBox rtbNotes;
        private Button btOpenHoldingsFile;
        private DataGridView dgvHoldings;
        private Button btClose;
        private TextBox tbFilePath;
        private NumericUpDown numFileNumber;
        List<StockInfoHoldings> holdings = new List<StockInfoHoldings>();

        private Button btOpen;
        private Button btOpenPrompt;
        private Button button1;
        string fileName;

        private void btSave_Click(object sender, EventArgs e)
        {
            fileName = gv.mainWindow.CreateNotesFileName((int) numFileNumber.Value);
            SaveRTF_AsText(fileName, rtbNotes);
        }
        
        public bool LoadText()
        {
            string fpath = null;
            this.dgvHoldings.DefaultCellStyle.Font = new Font("Arial", 15);
            //fpath = gv.myDataFolder + "//" + gv.notes1path;
            fileName = gv.mainWindow.CreateNotesFileName((int)numFileNumber.Value);
            fpath = Path.Combine(gv.dataFolder, fileName);

            if (string.IsNullOrEmpty(fpath))
            {
                //tbStatus.Text = "Cancelled";
                return false;
            }
            if (verifyFileExists(fpath))
            {
                tbFilePath.Text = fpath;
                try
                {
                    rtbNotes.LoadFile(fpath, RichTextBoxStreamType.PlainText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}", "Load Error");
                    return false;
                }
            }
            else
            {
                rtbNotes.Clear();
                return false;
            }
            return true;
        }

        private Font currentFont;
        private void rbArial_CheckedChanged(object sender, EventArgs e)
        {
            currentFont?.Dispose();
            currentFont = new Font("Arial", 10);
            rtbNotes.Font = currentFont;
        }

        private void rbSegoeUI_CheckedChanged(object sender, EventArgs e)
        {
            rtbNotes.Font = new Font("Segoe UI", 10);
        }

        private void btTahoma_CheckedChanged(object sender, EventArgs e)
        {
            rtbNotes.Font = new Font("Tahoma", 10);
        }

        public Boolean verifyFileExists(string fn)
        {
            if (string.IsNullOrEmpty(fn))
                return false;
            return File.Exists(fn);
        }
        /// 
        public string prg86files = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        public Boolean stopOnImageError = false;
        //----------------------INIT FILE ----------------//        
        //----------------------INIT FILE ----------------//
        //----------------------INIT FILE ----------------//
        // public string stockInfoFileCSV = mydocsFolder + initFolderName + "\\IB_StockOrdersAndTargets.csv";
        //
        public void GetHoldings()
        {
            dgvHoldings.DataSource = null;
        }
        private List<StockInfoHoldings> DeserializeFromXMLTradeLog(string fpath)
        {
            if (string.IsNullOrEmpty(fpath))
            {
                fpath = getInputFileName(null, "xml");
                if (string.IsNullOrEmpty(fpath))
                    return null;
            }
            XmlSerializer deserializer = new XmlSerializer(typeof(List<StockInfoHoldings>));
            TextReader textReader = new StreamReader(fpath);
            //gv.main.tdList
            holdings.Clear();
            holdings = (List<StockInfoHoldings>)deserializer.Deserialize(textReader);
            textReader.Close();

            return holdings;
        }

        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        OpenFileDialog openFileDialog1 = new OpenFileDialog();

        string fpath;
        public string getInputFileName(string folder, string ext = "xml") //retucn null IF DIALOG CANCELLED
        {
            if (!string.IsNullOrEmpty(folder))
                openFileDialog1.InitialDirectory = folder;

            openFileDialog1.Filter = ext + " Files|*." + ext;
            if (ext.Equals("all"))
                openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            /*
             "Image Files (*.bmp, *.jpg)|*.bmp;*.jpg"
              OR --
                "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            */
            InitializeOpenFileDialog(true);
            //InitializeOpenFileDialogXML();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fpath = openFileDialog1.FileName;
                fpath = Path.ChangeExtension(fpath, ext);//"xml");
            }
            else
                fpath = null;
            return fpath;
        }

        public void InitializeOpenFileDialog(bool bXML)
        {
            if (this.openFileDialog1 == null)
                this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            // Set the file dialog to filter for graphics files.
            if (bXML)
                this.openFileDialog1.Filter = "XML Files (*.XML)|*.XML";
            else
                this.openFileDialog1.Filter = "CSV Files (*.CSV)|*.CSV";
            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = false;// .SupportMultiDottedExtensions = true; //.Multiselect = true;
            this.openFileDialog1.Title = "Select File (.XML or .CSV)";
        }
        public string SaveRTF_AsText(string cname, RichTextBox rtfText)
        {
            string status;

            if (string.IsNullOrEmpty(cname))
                return null;
            string fpath = Path.Combine(gv.dataFolder, cname);

            try
            {
                rtfText.SaveFile(fpath, RichTextBoxStreamType.PlainText);
                status = fpath;
            }
            catch (Exception e)
            {

                status = $"error {e.ToString()}";
            }
            return status;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //List<StockInfo> stockList = new List<StockInfo>();
        public class StockInfoHoldings
        {
            public string Symbol { get; set; }
            public string notes { get; set; }
            public decimal close { get; set; }    //----------CLOSE
            public decimal open { get; set; }  //----------------OPEN
            public decimal avgCost { get; set; }
            public decimal lastPrice { get; set; }  // duplicates TD.lastPrice  for the MasterControl display
            public int Position { get; set; }
            public decimal gain { get; set; }
            public decimal Cost { get; set; }
            public decimal marketValue { get; set; }
            public decimal marketGain { get; set; }  // ?? not used?

            public StockInfoHoldings()
            { }

            public StockInfoHoldings(string Symbol2, decimal close2, decimal open2, decimal avgCost2,
             decimal lastPrice2, int Position2, decimal gain2)
            {
                Symbol = Symbol2;
                close = close2;
                open = open2;
                avgCost = avgCost2;
                lastPrice = lastPrice2;
                Position = Position2;
                gain = gain2;
            }
        }

        private void btOpenHoldingsFile_Click(object sender, EventArgs e)
        {
            GetHoldings();
        }

        private void InitializeComponent2()
        {
            this.SuspendLayout();
            // 
            // EditTradingNotes
            // 
            this.ClientSize = new System.Drawing.Size(1253, 746);
            this.Name = "EditTradingNotes";
            this.ResumeLayout(false);

        }

        private void InitializeComponent3()
        {
            this.SuspendLayout();
            // 
            // EditTradingNotes
            // 
            this.ClientSize = new System.Drawing.Size(1186, 573);
            this.Name = "EditTradingNotes";
            this.ResumeLayout(false);

        }

        private void InitializeComponent()
        {
            this.gbFont = new System.Windows.Forms.GroupBox();
            this.btTahoma = new System.Windows.Forms.RadioButton();
            this.rbSegoeUI = new System.Windows.Forms.RadioButton();
            this.rbArial = new System.Windows.Forms.RadioButton();
            this.rtbNotes = new System.Windows.Forms.RichTextBox();
            this.btOpenHoldingsFile = new System.Windows.Forms.Button();
            this.dgvHoldings = new System.Windows.Forms.DataGridView();
            this.btClose = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.tbFilePath = new System.Windows.Forms.TextBox();
            this.numFileNumber = new System.Windows.Forms.NumericUpDown();
            this.btOpen = new System.Windows.Forms.Button();
            this.btOpenPrompt = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.gbFont.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoldings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFileNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // gbFont
            // 
            this.gbFont.Controls.Add(this.btTahoma);
            this.gbFont.Controls.Add(this.rbSegoeUI);
            this.gbFont.Controls.Add(this.rbArial);
            this.gbFont.Location = new System.Drawing.Point(1156, 264);
            this.gbFont.Margin = new System.Windows.Forms.Padding(2);
            this.gbFont.Name = "gbFont";
            this.gbFont.Padding = new System.Windows.Forms.Padding(2);
            this.gbFont.Size = new System.Drawing.Size(68, 109);
            this.gbFont.TabIndex = 13;
            this.gbFont.TabStop = false;
            this.gbFont.Text = "Font";
            this.gbFont.Enter += new System.EventHandler(this.gbFont_Enter);
            // 
            // btTahoma
            // 
            this.btTahoma.AutoSize = true;
            this.btTahoma.Location = new System.Drawing.Point(3, 78);
            this.btTahoma.Margin = new System.Windows.Forms.Padding(2);
            this.btTahoma.Name = "btTahoma";
            this.btTahoma.Size = new System.Drawing.Size(79, 20);
            this.btTahoma.TabIndex = 2;
            this.btTahoma.Text = "Tahoma";
            this.btTahoma.UseVisualStyleBackColor = true;
            this.btTahoma.CheckedChanged += new System.EventHandler(this.btTahoma_CheckedChanged);
            // 
            // rbSegoeUI
            // 
            this.rbSegoeUI.AutoSize = true;
            this.rbSegoeUI.Checked = true;
            this.rbSegoeUI.Location = new System.Drawing.Point(3, 54);
            this.rbSegoeUI.Margin = new System.Windows.Forms.Padding(2);
            this.rbSegoeUI.Name = "rbSegoeUI";
            this.rbSegoeUI.Size = new System.Drawing.Size(69, 20);
            this.rbSegoeUI.TabIndex = 1;
            this.rbSegoeUI.TabStop = true;
            this.rbSegoeUI.Text = "Segoe";
            this.rbSegoeUI.UseVisualStyleBackColor = true;
            this.rbSegoeUI.CheckedChanged += new System.EventHandler(this.rbSegoeUI_CheckedChanged);
            // 
            // rbArial
            // 
            this.rbArial.AutoSize = true;
            this.rbArial.Location = new System.Drawing.Point(3, 26);
            this.rbArial.Margin = new System.Windows.Forms.Padding(2);
            this.rbArial.Name = "rbArial";
            this.rbArial.Size = new System.Drawing.Size(55, 20);
            this.rbArial.TabIndex = 0;
            this.rbArial.Text = "Arial";
            this.rbArial.UseVisualStyleBackColor = true;
            this.rbArial.CheckedChanged += new System.EventHandler(this.rbArial_CheckedChanged);
            // 
            // rtbTradingNotes
            // 
            this.rtbNotes.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbNotes.Location = new System.Drawing.Point(3, 39);
            this.rtbNotes.Margin = new System.Windows.Forms.Padding(2);
            this.rtbNotes.Name = "rtbTradingNotes";
            this.rtbNotes.Size = new System.Drawing.Size(1149, 504);
            this.rtbNotes.TabIndex = 11;
            this.rtbNotes.Text = "";
            this.rtbNotes.TextChanged += new System.EventHandler(this.rtbTradingNotes_TextChanged);
            // 
            // btOpenHoldingsFile
            // 
            this.btOpenHoldingsFile.Location = new System.Drawing.Point(1178, 416);
            this.btOpenHoldingsFile.Margin = new System.Windows.Forms.Padding(2);
            this.btOpenHoldingsFile.Name = "btOpenHoldingsFile";
            this.btOpenHoldingsFile.Size = new System.Drawing.Size(113, 36);
            this.btOpenHoldingsFile.TabIndex = 16;
            this.btOpenHoldingsFile.Text = "Open Holdings";
            this.btOpenHoldingsFile.UseVisualStyleBackColor = true;
            this.btOpenHoldingsFile.Click += new System.EventHandler(this.btOpenHoldingsFile_Click);
            // 
            // dgvHoldings
            // 
            this.dgvHoldings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHoldings.Location = new System.Drawing.Point(4, 548);
            this.dgvHoldings.Name = "dgvHoldings";
            this.dgvHoldings.RowHeadersWidth = 51;
            this.dgvHoldings.Size = new System.Drawing.Size(1314, 187);
            this.dgvHoldings.TabIndex = 15;
            this.dgvHoldings.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHoldings_CellContentClick);
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(1168, 136);
            this.btClose.Margin = new System.Windows.Forms.Padding(2);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(56, 36);
            this.btClose.TabIndex = 14;
            this.btClose.Text = "Close";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(1168, 69);
            this.btSave.Margin = new System.Windows.Forms.Padding(2);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(56, 36);
            this.btSave.TabIndex = 12;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // tbFilePath
            // 
            this.tbFilePath.Location = new System.Drawing.Point(4, 12);
            this.tbFilePath.Name = "tbFilePath";
            this.tbFilePath.Size = new System.Drawing.Size(909, 22);
            this.tbFilePath.TabIndex = 17;
            // 
            // numFileNumber
            // 
            this.numFileNumber.Location = new System.Drawing.Point(950, 12);
            this.numFileNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFileNumber.Name = "numFileNumber";
            this.numFileNumber.Size = new System.Drawing.Size(56, 22);
            this.numFileNumber.TabIndex = 18;
            this.numFileNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFileNumber.ValueChanged += new System.EventHandler(this.numFileNumber_ValueChanged);
            // 
            // btOpen
            // 
            this.btOpen.Location = new System.Drawing.Point(1168, 191);
            this.btOpen.Margin = new System.Windows.Forms.Padding(2);
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(56, 36);
            this.btOpen.TabIndex = 19;
            this.btOpen.Text = "Open";
            this.btOpen.UseVisualStyleBackColor = true;
            this.btOpen.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // btOpenPrompt
            // 
            this.btOpenPrompt.Location = new System.Drawing.Point(1248, 191);
            this.btOpenPrompt.Margin = new System.Windows.Forms.Padding(2);
            this.btOpenPrompt.Name = "btOpenPrompt";
            this.btOpenPrompt.Size = new System.Drawing.Size(96, 36);
            this.btOpenPrompt.TabIndex = 20;
            this.btOpenPrompt.Text = "Open ?";
            this.btOpenPrompt.UseVisualStyleBackColor = true;
            this.btOpenPrompt.Click += new System.EventHandler(this.btOpenPrompt_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1248, 69);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 36);
            this.button1.TabIndex = 21;
            this.button1.Text = "Save As...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // EditNotes
            // 
            this.ClientSize = new System.Drawing.Size(1381, 731);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btOpenPrompt);
            this.Controls.Add(this.btOpen);
            this.Controls.Add(this.numFileNumber);
            this.Controls.Add(this.tbFilePath);
            this.Controls.Add(this.gbFont);
            this.Controls.Add(this.rtbNotes);
            this.Controls.Add(this.btOpenHoldingsFile);
            this.Controls.Add(this.dgvHoldings);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.btSave);
            this.Name = "EditNotes";
            this.Text = "Edit Notes";
            this.Load += new System.EventHandler(this.EditNotes_Load);
            this.gbFont.ResumeLayout(false);
            this.gbFont.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoldings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFileNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Button btSave;

        private void rtbTradingNotes_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvHoldings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gbFont_Enter(object sender, EventArgs e)
        {

        }

        private void EditNotes_Load(object sender, EventArgs e)
        {

        }

        private void numFileNumber_ValueChanged(object sender, EventArgs e)
        {
            LoadText(); // Automatically load when number changes
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            LoadText();
        }

        private void btOpenPrompt_Click(object sender, EventArgs e)
        {

        }
    }
}
