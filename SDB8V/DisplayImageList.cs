using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SymbolDB
{
    public partial class DisplayImageList : Form
    {
        private GlobalVars gv;
        private ImageFileList currentList;
        private int currentIndex = 0;

        public DisplayImageList(GlobalVars globalVars)
        {
            InitializeComponent();
            gv = globalVars;

            // Enable keyboard navigation
            this.KeyPreview = true;

            // Initialize with the first available list
            LoadSelectedList();

            if (currentList != null && currentList.getImageCount() > 0)
            {
                DisplayCurrentImage();
            }
            else
            {
                lblStatus.Text = "No images in selected list";
            }
        }

        private void LoadSelectedList()
        {
            if (rbImageFileList1.Checked)
            {
                currentList = gv.imageFileList1;
                if (currentList == null || currentList.getImageCount() == 0)
                    this.Text = "Display Image List - 1 (Empty)";
                else
                    this.Text = "Display Image List - 1";
            }
            else if (rbImageFileList2.Checked)
            {
                currentList = gv.imageFileList2;
                if (currentList == null || currentList.getImageCount() == 0)
                    this.Text = "Display Image List - 2 (Empty)";
                else
                    this.Text = "Display Image List - 2";
            }
            else if (rbImageFileErrorList.Checked)
            {
                currentList = gv.imageFileErrorList;
                if (currentList == null || currentList.getImageCount() == 0)
                    this.Text = "Display Image List - Error List (Empty)";
                else
                    this.Text = "Display Image List - Error List";
            }
            else if (rbImageFileListMatches.Checked)
            {
                currentList = gv.imageFileListMatches;
                this.Text = "Display Image List - Matches List";
            }

            // Reset to first item when switching lists
            currentIndex = 0;

            // Check if list exists and has items
            if (currentList == null)
            {
                lblStatus.Text = "Selected list is null";
                btNext.Enabled = false;
                btPrevious.Enabled = false;
                btSaveList.Enabled = false;
                return;
            }

            int count = currentList.getImageCount();
            btNext.Enabled = count > 0;
            btPrevious.Enabled = count > 0;
            btSaveList.Enabled = count > 0;
        }

        private void DisplayCurrentImage()
        {
            if (currentList == null || currentList.getImageCount() == 0)
            {
                ClearDisplay();
                lblStatus.Text = "No images available";
                return;
            }

            // Bounds checking
            if (currentIndex < 0)
                currentIndex = 0;
            if (currentIndex >= currentList.getImageCount())
                currentIndex = currentList.getImageCount() - 1;

            FileInfoItem item = currentList.getIndexed(currentIndex);

            if (item == null || string.IsNullOrEmpty(item.fpath))
            {
                ClearDisplay();
                lblStatus.Text = $"Invalid item at index {currentIndex}";
                return;
            }

            // Update status
            lblStatus.Text = $"{currentIndex + 1} / {currentList.getImageCount()}";

            // Display file path
            tbFilePath.Text = item.fpath;

            // Load and display image (skip for error list)
            if (rbImageFileErrorList.Checked)
            {
                ClearImageOnly();
            }
            else
            {
                LoadImage(item.fpath);
            }

            // Display all FileInfoItem attributes
            DisplayFileInfoAttributes(item);

            // Update navigation buttons
            btPrevious.Enabled = currentIndex > 0;
            btNext.Enabled = currentIndex < currentList.getImageCount() - 1;
        }
        private void ClearImageOnly()
        {
            if (pbImage.Image != null)
            {
                var oldImage = pbImage.Image;
                pbImage.Image = null;
                oldImage.Dispose();
            }
        }
        private void LoadImage(string filePath)
        {
            // Dispose previous image to free memory
            if (pbImage.Image != null)
            {
                var oldImage = pbImage.Image;
                pbImage.Image = null;
                oldImage.Dispose();
            }

            if (!File.Exists(filePath))
            {
                pbImage.Image = null;
                return;
            }

            try
            {
                // Load image without locking the file
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var img = Image.FromStream(fs, false, false))
                    {
                        pbImage.Image = new Bitmap(img);
                    }
                }
            }
            catch (Exception ex)
            {
                pbImage.Image = null;
                MessageBox.Show($"Error loading image:\n{ex.Message}", "Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DisplayFileInfoAttributes(FileInfoItem item)
        {
            dgvFileInfo.Rows.Clear();

            if (item == null) return;

            // Add all FileInfoItem properties
            AddPropertyRow("Key Path", item.keyPath);
            AddPropertyRow("File Name", item.fname);
            AddPropertyRow("Extension", item.ext);
            AddPropertyRow("Directory Path", item.dpath);
            AddPropertyRow("Full Path", item.fpath);
            AddPropertyRow("Type", item.type);
            AddPropertyRow("Level", item.level.ToString());
            AddPropertyRow("Length (bytes)", item.len.ToString("N0"));
            AddPropertyRow("Timestamp", item.stimestamp);
            AddPropertyRow("Width", item.width.ToString());
            AddPropertyRow("Height", item.height.ToString());
            AddPropertyRow("Rating", item.rating.ToString());
            AddPropertyRow("Delete Flag", item.bDelete.ToString());
            AddPropertyRow("Invalid Flag", item.bInvalid.ToString());
            AddPropertyRow("Source", item.source);
            AddPropertyRow("Play Time", item.playTime.ToString("F2"));
            AddPropertyRow("Minutes", item.minutes.ToString());
            AddPropertyRow("Seconds", item.seconds.ToString());
            AddPropertyRow("Index", item.ndx.ToString());
            AddPropertyRow("Comment", item.comment);
        }

        private void AddPropertyRow(string property, string value)
        {
            dgvFileInfo.Rows.Add(property, value ?? string.Empty);
        }

        private void ClearDisplay()
        {
            if (pbImage.Image != null)
            {
                var oldImage = pbImage.Image;
                pbImage.Image = null;
                oldImage.Dispose();
            }

            tbFilePath.Text = string.Empty;
            dgvFileInfo.Rows.Clear();
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            if (currentList == null || currentList.getImageCount() == 0)
                return;

            if (currentIndex < currentList.getImageCount() - 1)
            {
                currentIndex++;
                DisplayCurrentImage();
            }
        }

        private void btPrevious_Click(object sender, EventArgs e)
        {
            if (currentList == null || currentList.getImageCount() == 0)
                return;

            if (currentIndex > 0)
            {
                currentIndex--;
                DisplayCurrentImage();
            }
        }

        private void btGoToIndex_Click(object sender, EventArgs e)
        {
            if (currentList == null || currentList.getImageCount() == 0)
                return;

            if (int.TryParse(tbGoToIndex.Text, out int targetIndex))
            {
                // Convert from 1-based user input to 0-based index
                targetIndex--;

                if (targetIndex >= 0 && targetIndex < currentList.getImageCount())
                {
                    currentIndex = targetIndex;
                    DisplayCurrentImage();
                }
                else
                {
                    MessageBox.Show($"Index must be between 1 and {currentList.getImageCount()}",
                        "Invalid Index", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void rbListSelection_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                LoadSelectedList();
                if (currentList != null && currentList.getImageCount() > 0)
                {
                    DisplayCurrentImage();
                }
                else
                {
                    ClearDisplay();
                    lblStatus.Text = "Selected list is empty";
                }
            }
        }

        private void btSaveList_Click(object sender, EventArgs e)
        {
            if (currentList == null || currentList.getImageCount() == 0)
            {
                MessageBox.Show("No list to save", "Save List",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
                sfd.Title = "Save Image File List";
                sfd.FileName = GetDefaultFileName();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveImageFileListToXml(sfd.FileName);
                }
            }
        }

        private string GetDefaultFileName()
        {
            string baseName = "ImageFileList";

            if (rbImageFileList1.Checked)
                baseName = "ImageFileList1";
            else if (rbImageFileList2.Checked)
                baseName = "ImageFileList2";
            else if (rbImageFileErrorList.Checked)
                baseName = "ImageFileErrorList";
            else if (rbImageFileListMatches.Checked)
                baseName = "ImageFileListMatches";

            return $"{baseName}_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
        }

        private void SaveImageFileListToXml(string filePath)
        {
            try
            {
                var root = new XElement("ImageFileList");

                for (int i = 0; i < currentList.getImageCount(); i++)
                {
                    var item = currentList.getIndexed(i);

                    if (item == null) continue;

                    var itemElement = new XElement("FileInfoItem",
                        new XElement("keyPath", item.keyPath ?? string.Empty),
                        new XElement("fname", item.fname ?? string.Empty),
                        new XElement("ext", item.ext ?? string.Empty),
                        new XElement("dpath", item.dpath ?? string.Empty),
                        new XElement("fpath", item.fpath ?? string.Empty),
                        new XElement("type", item.type ?? string.Empty),
                        new XElement("level", item.level),
                        new XElement("len", item.len),
                        new XElement("stimestamp", item.stimestamp ?? string.Empty),
                        new XElement("width", item.width),
                        new XElement("height", item.height),
                        new XElement("rating", item.rating),
                        new XElement("bDelete", item.bDelete),
                        new XElement("bInvalid", item.bInvalid),
                        new XElement("source", item.source ?? string.Empty),
                        new XElement("playTime", item.playTime),
                        new XElement("minutes", item.minutes),
                        new XElement("seconds", item.seconds),
                        new XElement("index", item.ndx),
                        new XElement("desc", item.comment ?? string.Empty)
                    );

                    root.Add(itemElement);
                }

                var doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    root
                );

                doc.Save(filePath);

                MessageBox.Show($"Successfully saved {currentList.getImageCount()} items to:\n{filePath}",
                    "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file:\n{ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DisplayImageList_KeyDown(object sender, KeyEventArgs e)
        {
            // Keyboard navigation
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
            {
                btNext_Click(null, null);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
            {
                btPrevious_Click(null, null);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Home)
            {
                currentIndex = 0;
                DisplayCurrentImage();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.End)
            {
                if (currentList != null && currentList.getImageCount() > 0)
                {
                    currentIndex = currentList.getImageCount() - 1;
                    DisplayCurrentImage();
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up image resources
                if (pbImage.Image != null)
                {
                    var img = pbImage.Image;
                    pbImage.Image = null;
                    img.Dispose();
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void btCopyFIleList_Click(object sender, EventArgs e)
        {
            if (currentList == null || currentList.getImageCount() == 0)
            {
                MessageBox.Show("No list to copy.", "Copy File List",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the target folder for the file list";
                fbd.UseDescriptionForTitle = true;

                if (fbd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return;
                }

                bool moveFiles = cbMoveFiles.Checked;
                int copied = 0;
                int moved = 0;
                int skipped = 0;
                int failed = 0;
                var details = new StringBuilder();

                string GetUniqueDestinationPath(string originalPath)
                {
                    if (!File.Exists(originalPath))
                    {
                        return originalPath;
                    }

                    string directory = Path.GetDirectoryName(originalPath) ?? string.Empty;
                    string name = Path.GetFileNameWithoutExtension(originalPath);
                    string ext = Path.GetExtension(originalPath);
                    string timestamp = DateTime.Now.ToString("yyMMddHHmmss");

                    string candidate = Path.Combine(directory, $"{name}_{timestamp}{ext}");
                    int suffix = 1;

                    while (File.Exists(candidate))
                    {
                        candidate = Path.Combine(directory, $"{name}_{timestamp}_{suffix}{ext}");
                        suffix++;
                    }

                    return candidate;
                }

                for (int i = 0; i < currentList.getImageCount(); i++)
                {
                    var item = currentList.getIndexed(i);
                    if (item == null || string.IsNullOrWhiteSpace(item.fpath))
                    {
                        skipped++;
                        continue;
                    }

                    if (!File.Exists(item.fpath))
                    {
                        skipped++;
                        details.AppendLine($"Missing: {item.fpath}");
                        continue;
                    }

                    string fileName = Path.GetFileName(item.fpath);
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        skipped++;
                        continue;
                    }

                    string destinationPath = Path.Combine(fbd.SelectedPath, fileName);

                    try
                    {
                        if (moveFiles)
                        {
                            string originalDestinationPath = destinationPath;
                            destinationPath = GetUniqueDestinationPath(destinationPath);

                            if (!string.Equals(destinationPath, originalDestinationPath, StringComparison.OrdinalIgnoreCase))
                            {
                                details.AppendLine($"Exists (renamed for move): {destinationPath}");
                            }

                            File.Move(item.fpath, destinationPath);
                            moved++;
                        }
                        else
                        {
                            if (File.Exists(destinationPath))
                            {
                                skipped++;
                                details.AppendLine($"Exists (copy skipped): {destinationPath}");
                                continue;
                            }

                            File.Copy(item.fpath, destinationPath, false);
                            copied++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        details.AppendLine($"{item.fpath} -> {destinationPath}: {ex.Message}");
                    }
                }

                string action = moveFiles ? "Moved" : "Copied";
                int count = moveFiles ? moved : copied;

                string summary = $"{action} {count} file(s). Skipped {skipped}. Failed {failed}.";
                if (details.Length > 0)
                {
                    summary += "\n\nDetails:\n" + details;
                }

                MessageBox.Show(summary, $"{action} File List",
                    MessageBoxButtons.OK,
                    failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
        }
    }
}