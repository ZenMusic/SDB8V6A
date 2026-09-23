#nullable disable
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



namespace SymbolDB
{
    public partial class DialogFolderSelection : Form
    {
        string dirpath;
        GlobalVars gv;
        TextBox tb;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public DialogFolderSelection(GlobalVars g, TextBox tb1)
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            this.Text = "select the target directory";
            gv = g;
            tb = tb1;

            timer.Interval = 1000;             // Timer will tick every (speed /100) seconds
            timer.Enabled = true;                       // Enable the timer
            timer.Start();

            GetTargetHistory();
        }

        InitFolder startFolder;// = InitFolder.MyComputer;

        FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
        void timer_Tick(object sender, EventArgs e) // TIMER EVENT <<<<<<<<<<<<<<<<<<<< gv.nextSlide 
        {
            rbDesktop.Checked = true;
        }
        string dirPath2 = null;

        private void setFolder(string path)
        {
            folderBrowserDialog1.SelectedPath = @"C:\Users\david\Documents\111";
            folderBrowserDialog1.ShowDialog();
            tbDirPath2.Text = folderBrowserDialog1.SelectedPath;
            this.Text = tbDirPath2.Text;
        }

        private void setFolder()
        {
            if (this.rbMyComputer.Checked)
            {
                startFolder = InitFolder.MyComputer;
            }
            else if (this.rbMyDocs.Checked)
            {
                startFolder = InitFolder.MyDocs;

            }
            else if (this.rbMyPics.Checked)
            {
                startFolder = InitFolder.MyPics;
            }
            else if (this.rbC.Checked)
                startFolder = InitFolder.C_Drive;
            else if (this.rbDesktop.Checked)
                startFolder = InitFolder.MyDesktop;
            else if (this.rbTarot.Checked)
            {
                startFolder = InitFolder.Special;
                //tbDirectoryPath.Text = @"C:";
            }
            else //rbRoot.Checked
            {

                startFolder = InitFolder.MyDesktop;
            }
        }
        string[] folders = new string[99];
        string folder;

        public void GetTargetHistory()
        {
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
            comboTargetHistory.Items.Clear();
            foreach (var target in targets)
            {
                comboTargetHistory.Items.Add(target.Path);
            }
            if (comboTargetHistory.Items.Count > 0)
                comboTargetHistory.SelectedIndex = 0;
        }
        public void GetSubFolders()
        {
            int idx = 0;
            // Make a reference to a directory.
            DirectoryInfo di = new DirectoryInfo(dpath);  //  "c:\\");

            // Get a reference to each directory in that directory.
            DirectoryInfo[] diArr = di.GetDirectories();

            // Display the names of the directories.
            gv.folderAssignmentList = new List<FolderAssignment>();
            foreach (DirectoryInfo dri in diArr)
            {
                folder = dri.Name;
                FolderAssignment fa = new FolderAssignment(folder, dpath + "/" + folder);
                gv.folderAssignmentList.Add(fa);
            }
        }

        private void rbMyComputer_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void rbMyDocs_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void rbC_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void rbDesktop_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void rbTarot_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        private void rbRoot_CheckedChanged(object sender, EventArgs e)
        {
            setFolder();
        }

        string dpath;
        private void btOpen_Click(object sender, EventArgs e)
        {

            if (cbUseBaseDirectory.Checked)
                dpath = gv.dirDialog(tbBaseDirectory.Text);
            else
                dpath = gv.dirDialog(startFolder);// (startFolder);
            if (dpath != null)
            {
                if (Directory.Exists(dpath))
                {
                    tb.Text = dpath;
                    dirPath2 = dpath;
                    tbDirPath2.Text = dpath;
                }
                else
                {
                    gv.initParm1List[0].SetTargetDir1("C:");
                    this.Text = "no directory";
                }
            }
            return;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dirPath2))
                return;
            SetDirPath();
            this.Close();
        }
        public void SetDirPath()
        {
            if (rbWatchFolder.Checked)
                gv.initParm1List[0].watchFolder1 = dirPath2;
            else if (rbWatchFolder2.Checked)
                gv.initParm1List[0].watchFolder2 = dirPath2;
            else if (rbWatchFolder3.Checked)
                gv.initParm1List[0].watchFolder3 = dirPath2;
            else if (rbTargetDir1.Checked)
                gv.initParm1List[0].SetTargetDir1(dirPath2);
            else if (rbTargetDir2.Checked)
            {
                gv.mainWindow.setTargetFolder(dirPath2);
                gv.initParm1List[0].targetDir2 = dirPath2;
            }
            else if (rbSource4.Checked)
                gv.initParm1List[0].sourceDir4 = dirPath2;
            else if (rbSource5.Checked)
                gv.initParm1List[0].sourceDir5 = dirPath2;
        }

        private void cbUseBaseDirectory_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseBaseDirectory.Checked)
            {
                tbBaseDirectory.Text = tbDirPath2.Text;
            }
        }

        private void btSubFolders_Click(object sender, EventArgs e)
        {
            GetSubFolders();
            this.Close();
        }

        private void btSetTargetFolder_Click(object sender, EventArgs e)
        {
            dirPath2 = tbDirPath2.Text;
            SetDirPath();
            gv.mainWindow.setTargetFolder(dirpath);
        }

        private void DialogFolderSelection_Load(object sender, EventArgs e)
        {

        }

        private void btSetFolder_Click(object sender, EventArgs e)
        {

        }

        private void btVerifyFolder_Click(object sender, EventArgs e)
        {
            bool test = Directory.Exists(tbDirPath2.Text);
            tbStatus.Text = test.ToString();
        }

        private void btSetWatchFolder_Click(object sender, EventArgs e)
        {
            rbWatchFolder.Checked = true;
            gv.initParm1List[0].watchFolder1 = dirPath2;

        }

        private void btResetTarget1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(tbTargetAssign.Text))
            {
                gv.initParm1List[0].SetTargetDir1(tbTargetAssign.Text);
                gv.mainWindow.setTargetFolder(tbTargetAssign.Text);
                this.Close();
            }
            else
            {
                MessageBox.Show("Directory does not exist. Cannot set target.");
            }
        }

        private void comboTargetHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbTargetAssign.Text = comboTargetHistory.SelectedItem.ToString();
        }
    }
}
