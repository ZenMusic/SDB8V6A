using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class DisplayMessage : Form
    {
        GlobalVars gv;
        DialogTraverser pTraverser = null;

        // Plan (pseudocode):
        // - Add helper: FormatBytes(long) -> string for human-readable sizes.
        // - Add helper: UpdateSpaceAvailableForPath(string? path)
        //     - If path is null/empty or not rooted -> clear tbSpaceAvailable.Text and return.
        //     - Get drive root with Path.GetPathRoot(path).
        //     - Create DriveInfo for root; if not ready -> set "Drive not ready".
        //     - Read AvailableFreeSpace; format with FormatBytes and set tbSpaceAvailable.Text.
        //     - Catch any exceptions and set "Unknown".
        // - In DisplayCopyMessageText:
        //     - Determine best candidate path for the target:
        //         - Prefer rooted targetDir (and not default "root"), else rooted targetDir1,
        //           else directory of sourceFile.
        //     - Call UpdateSpaceAvailableForPath(candidate) before Refresh().
        //     - Compute file size for sourceFile and put it in tbFileSize.Text.

        public DisplayMessage(GlobalVars g, DialogTraverser p)
        {
            InitializeComponent();
            gv = g;
            pTraverser = p;
        }

        public void DisplayCopyMessageText(bool specialFolder, string targetDir1, string sourceFile = null, string targetDir = "root")
        {
            if (!specialFolder)
                tbAAA.Text = "";
            else
                tbAAA.Text = "AAAA";

            string folderName = Path.GetFileName(targetDir1);
            tbFolder.Text = folderName;
            tbSourceFolder.Text = Path.GetDirectoryName(sourceFile);
            tbMessage.Text = targetDir;
            if (sourceFile != null)
            {
                tbCopy2.Text = sourceFile;
                tbCopy3.Text = targetDir;
            }

            string fileName = Path.GetFileName(sourceFile);
            tbFileName.Text = fileName;

            // File size -> tbFileSize.Text
            if (!string.IsNullOrWhiteSpace(sourceFile) && File.Exists(sourceFile))
            {
                try
                {
                    var fi = new FileInfo(sourceFile);
                    tbFileSize.Text = FormatBytes(fi.Length);
                }
                catch
                {
                    tbFileSize.Text = "Unknown";
                }
            }
            else
            {
                tbFileSize.Text = "";
            }

            // Determine the best candidate path to evaluate disk space
            string candidate = null;
            if (!string.IsNullOrWhiteSpace(targetDir)
                && !string.Equals(targetDir, "root", StringComparison.OrdinalIgnoreCase)
                && Path.IsPathRooted(targetDir))
            {
                candidate = targetDir;
            }
            else if (!string.IsNullOrWhiteSpace(targetDir1) && Path.IsPathRooted(targetDir1))
            {
                candidate = targetDir1;
            }
            else if (!string.IsNullOrWhiteSpace(sourceFile))
            {
                candidate = Path.GetDirectoryName(sourceFile);
            }

            UpdateSpaceAvailableForPath(candidate);

            this.Refresh();
        }
        public void Completed()
        {
            progressBar1.Value = 100;
            this.WindowState = FormWindowState.Minimized;
            this.SendToBack();
        }
        public void SetTopMost(bool on)
        {
            this.TopMost = on;
        }

        public void DisplayProgress(int progress)
        {
            progressBar1.Value = progress;
        }

        public void HideWin()
        {
            tbMessage.Text = "copy";
            SetTopMost(false);
        }

        public void ShowWin()
        {
            SetTopMost(true);
        }

        private void btHideMovie_Click(object sender, EventArgs e)
        {
            pTraverser.btHideMovie_Click(sender, e);
        }

        private void btDisplayFolder_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(tbMessage.Text);
            string folder = tbMessage.Text;
            if (true)
            {
                // Open File Explorer to the _rtfFolder path
                if (Directory.Exists(folder))
                {
                    Process.Start("explorer.exe", folder);
                }
                else
                {
                    MessageBox.Show("folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return; // Skip rest of click logic if Ctrl was held
            }
        }

        private void UpdateSpaceAvailableForPath(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path))
                {
                    tbSpaceAvailable.Text = "";
                    return;
                }

                string root = Path.GetPathRoot(path);
                if (string.IsNullOrWhiteSpace(root))
                {
                    tbSpaceAvailable.Text = "";
                    return;
                }

                var drive = new DriveInfo(root);
                if (!drive.IsReady)
                {
                    tbSpaceAvailable.Text = "Drive not ready";
                    return;
                }

                long free = drive.AvailableFreeSpace;
                tbSpaceAvailable.Text = $"{FormatBytes(free)} free";
            }
            catch
            {
                tbSpaceAvailable.Text = "Unknown";
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB" };
            double size = bytes;
            int unitIndex = 0;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024d;
                unitIndex++;
            }

            return $"{size:0.##} {units[unitIndex]}";
        }
    }
}
