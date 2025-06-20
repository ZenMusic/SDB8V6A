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
    public partial class FolderAssignments : Form
    {
        GlobalVars gv;
        FileFunctions ff;

        InitFolder startFolder = InitFolder.MyComputer;
        bool bUseSpecial = true;

        public FolderAssignments()
        {
            InitializeComponent();
        }
        public FolderAssignments(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            ff = new FileFunctions(gv);
            //string[] driveinfo = ff.getDrivesInfo();
            ff.getDrivesInfo();
        }

        private void btDirectoryDialog_Click(object sender, EventArgs e)
        {

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
            if (gv.bImageFileListLoaded)
            {
                gv.imageFileList.clearList();
                gv.bImageFileListLoaded = false;

            }
            gv.imageFileList = new ImageFileList();
            //get 2 levels of subfolders for  %completion estimate   doEnumerateFiles(gv.dirFullpath);
            ARGS args = new ARGS();
            //args.dirpath = gv.initParm1List[0].sourceDir1;
            args.dirpath = dpath;

            traverser.doTraverseFolders(args);
        }
    }
}
