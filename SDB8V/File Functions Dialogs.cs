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
    public partial class FileFunctionsDialog : Form
    {
        GlobalVars gv;

        FileFunctions ff;

        ListViewItem item = new ListViewItem();




        public FileFunctionsDialog(GlobalVars g, string fpath)
        {
            InitializeComponent();
            gv = g;
            ff = new FileFunctions(gv);

            if (fpath != null)
                this.tbSourceFpath.Text = fpath;

            this.tbInfo.Text = fpath;

            this.tbTargetDir.Text = gv.dataFolder;

            this.tbFileName.Text = Path.GetFileName(this.tbSourceFpath.Text);
            this.tbSourceDir.Text = Path.GetDirectoryName(this.tbSourceFpath.Text);


            listView1.View = View.Details;
            item.Text = "This is Item 1";

            item.SubItems.Add("This is subitem 1 for item 1");

            listView1.Items.Add(item);

            listView1.Columns.Add("Column 1", -1, HorizontalAlignment.Left);
            listView1.Columns.Add("Column 2", -2, HorizontalAlignment.Left);

            listView1.Columns[0].Width = 1000;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btCopy_Click(object sender, EventArgs e)
        {
            ff.CopyFile(tbSourceFpath.Text, tbTargetDir.Text);
        }

        public void w(string status1)
        {
            this.listView1.Items.Add(status1);
        }
        public void w(string status1, string status2)
        {
            this.listView1.Items.Add(status1 + " " + status2);
        }
        public void w(string status1, string status2, string status3)
        {
            this.listView1.Items.Add(status1 + " " + status2 + " " + status3);
        }

        private void btCopyPathFname_Click(object sender, EventArgs e)
        {
            ff.CopyFile(this.tbFileName.Text, this.tbSourceDir.Text, this.tbTargetDir.Text);
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btCopyFolderItems_Click(object sender, EventArgs e)
        {
            ff.CopyFolderItems(this.tbSourceDir.Text, this.tbTargetDir.Text);
        }

        private void btMove_Click(object sender, EventArgs e)
        {
            tbSourceDir.Text = Path.GetDirectoryName(tbSourceFpath.Text);
            ff.MoveFile(tbFileName.Text, tbSourceDir.Text, tbTargetDir.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ff.MoveFolder(tbSourceDir.Text, tbTargetDir.Text);
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            ff.FileDelete(tbSourceFpath.Text);
        }

        private void btCreateDir_Click(object sender, EventArgs e)
        {
            ff.createDirectory(tbTargetDir.Text);
        }

        private void btRename_Click(object sender, EventArgs e)
        {
            ff.renameFile(tbSourceFpath.Text, tbTargetDir.Text);

        }
    }
}
