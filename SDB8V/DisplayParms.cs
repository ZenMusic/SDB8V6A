#nullable disable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class DisplayParms : Form
    {
        GlobalVars gv;
        FileFunctions ff;

        public DisplayParms()
        {
            InitializeComponent();
        }

        public DisplayParms(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            ff = new FileFunctions(gv);

            tbEnvironmnet.Text = gv.bRunningInDevelopment ? "Development ENV" : "Production ENV";

            this.Refresh();
            this.Show();
            CompleteInitialization();
            gv.mainWindow.HideLoadingParms();
            this.Activate();

            if (gv.useTestSourceAndTargetFolder)
                btSaveInitFile.Enabled = false;
        }

        public void CompleteInitialization()
        {
            tbDotNetVersion.Text = GetDotNetVersion.Get45PlusVersionFromRegistry();
            // My Documents folder path
            tbMyDocumentsFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Init folder and ini file path — differs by environment
            if (gv.bRunningInDevelopment)
            {
                tbInitFolderPath.Text = gv.dataFolder;
                tbInitFileName.Text   = gv.inifileFullPathName;
            }
            else
            {
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

            // Populate the init parms grid
            resetDGVParms();

            // Colour-code the path text boxes green/red
            VerifyIniFolder();
            VerifyIniParmsFile();
        }

        // ── dgvInitParms ────────────────────────────────────────────────────
        public void resetDGVParms()
        {
            dgvInitParms.DataSource = null;
            this.Refresh();
            dgvInitParms.DataSource = gv.initParm1List;
        }

        // ── Folder / file verification ───────────────────────────────────────
        public void VerifyIniFolder()
        {
            bool brc = ff.directoryExists(tbInitFolderPath.Text);
            tbInitFolderPath.BackColor = brc ? Color.LightGreen : Color.LightCoral;
        }

        private void VerifyIniParmsFile()
        {
            bool brc = File.Exists(gv.inifileFullPathName);
            tbInitFileName.BackColor = brc ? Color.LightGreen : Color.LightCoral;
        }

        // ── Button handlers ──────────────────────────────────────────────────
        private void btMyDocs_Click(object sender, EventArgs e)
        {
            tbMyDocumentsFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void btVerifyMyDocs_Click(object sender, EventArgs e)
        {
            bool brc = ff.directoryExists(tbMyDocumentsFolder.Text);
            if (brc)
                MessageBox.Show($"Folder exists: {tbMyDocumentsFolder.Text}");
            else
                MessageBox.Show("Folder was not found: " + tbMyDocumentsFolder.Text);
        }

        /// <summary>button1 — verify the ini folder path</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            bool brc = ff.directoryExists(tbInitFolderPath.Text);
            if (brc)
            {
                MessageBox.Show($"Ini Folder exists: {tbInitFolderPath.Text}");
                tbInitFolderPath.BackColor = Color.LightGreen;
            }
            else
            {
                MessageBox.Show("Ini Folder was not found: " + tbInitFolderPath.Text);
                tbInitFolderPath.BackColor = Color.LightCoral;
            }
        }

        private void btVerifyIniFile_Click(object sender, EventArgs e)
        {
            bool brc = File.Exists(gv.inifileFullPathName);
            if (brc)
            {
                MessageBox.Show($"Ini Parameters file exists: {gv.inifileFullPathName}");
                tbInitFileName.BackColor = Color.LightGreen;
            }
            else
            {
                MessageBox.Show($"Ini Parameters file was not found: {gv.inifileFullPathName}");
                tbInitFileName.BackColor = Color.LightCoral;
            }
        }

        private void btCreateInitFile_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("create ini file");
            int rc = gv.mainWindow.createDefaultInitParm1File();
            System.Diagnostics.Debug.WriteLine("rc = " + rc.ToString());
        }

        private void btSaveInitFile_Click(object sender, EventArgs e)
        {
            if (gv.useTestSourceAndTargetFolder)
                return;

            if (gv.initParm1List.Count > 0)
                ff.saveInitParms1List(gv.inifileFullPathName, gv.initParm1List);
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ── Empty event stubs required by Designer ───────────────────────────
        private void tbInitFolderPath_TextChanged(object sender, EventArgs e) { }
    }
}