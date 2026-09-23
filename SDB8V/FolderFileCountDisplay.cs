using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class FolderFileCountDisplay : Form
    {
        private DataGridView dgvFolderCounts;
        private Label lblStatus;
        private Button btRefresh;
        private Button btExport;
        
        public class FolderCount
        {
            public string FolderPath { get; set; }
            public int FileCount { get; set; }
            public long TotalSize { get; set; }
            public string FormattedSize => FormatBytes(TotalSize);
            
            private static string FormatBytes(long bytes)
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = bytes;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }
                return $"{len:0.##} {sizes[order]}";
            }
        }
        
        private List<FolderCount> folderCounts = new List<FolderCount>();
        
        public FolderFileCountDisplay()
        {
            InitializeComponent();
            SetupForm();
        }
        
        private void InitializeComponent()
        {
            this.dgvFolderCounts = new DataGridView();
            this.lblStatus = new Label();
            this.btRefresh = new Button();
            this.btExport = new Button();
            this.SuspendLayout();
            
            // dgvFolderCounts
            this.dgvFolderCounts.AllowUserToAddRows = false;
            this.dgvFolderCounts.AllowUserToDeleteRows = false;
            this.dgvFolderCounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvFolderCounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFolderCounts.Location = new Point(12, 50);
            this.dgvFolderCounts.Name = "dgvFolderCounts";
            this.dgvFolderCounts.ReadOnly = true;
            this.dgvFolderCounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvFolderCounts.Size = new Size(760, 450);
            this.dgvFolderCounts.TabIndex = 0;
            
            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(12, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(200, 15);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Folder file counts";
            
            // btRefresh
            this.btRefresh.Location = new Point(12, 510);
            this.btRefresh.Name = "btRefresh";
            this.btRefresh.Size = new Size(100, 30);
            this.btRefresh.TabIndex = 2;
            this.btRefresh.Text = "Refresh";
            this.btRefresh.UseVisualStyleBackColor = true;
            this.btRefresh.Click += BtRefresh_Click;
            
            // btExport
            this.btExport.Location = new Point(120, 510);
            this.btExport.Name = "btExport";
            this.btExport.Size = new Size(100, 30);
            this.btExport.TabIndex = 3;
            this.btExport.Text = "Export CSV";
            this.btExport.UseVisualStyleBackColor = true;
            this.btExport.Click += BtExport_Click;
            
            // FolderFileCountDisplay
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 561);
            this.Controls.Add(this.btExport);
            this.Controls.Add(this.btRefresh);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.dgvFolderCounts);
            this.Name = "FolderFileCountDisplay";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Folder File Counts";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private void SetupForm()
        {
            // Setup DataGridView columns
            dgvFolderCounts.Columns.Clear();
            dgvFolderCounts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FolderPath",
                HeaderText = "Folder Path",
                DataPropertyName = "FolderPath",
                FillWeight = 60
            });
            dgvFolderCounts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FileCount",
                HeaderText = "File Count",
                DataPropertyName = "FileCount",
                FillWeight = 20
            });
            dgvFolderCounts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalSize",
                HeaderText = "Total Size",
                DataPropertyName = "FormattedSize",
                FillWeight = 20
            });
            
            dgvFolderCounts.AutoGenerateColumns = false;
            
            // Enable sorting
            dgvFolderCounts.ColumnHeaderMouseClick += DgvFolderCounts_ColumnHeaderMouseClick;
        }
        
        public void SetFolderCounts(List<FolderCount> counts)
        {
            folderCounts = counts ?? new List<FolderCount>();
            RefreshDisplay();
        }
        
        private void RefreshDisplay()
        {
            dgvFolderCounts.DataSource = null;
            dgvFolderCounts.DataSource = folderCounts;
            lblStatus.Text = $"Total folders: {folderCounts.Count} | Total files: {folderCounts.Sum(f => f.FileCount):N0}";
        }
        
        private void DgvFolderCounts_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var columnName = dgvFolderCounts.Columns[e.ColumnIndex].DataPropertyName;
            
            // Toggle sort order
            var currentOrder = dgvFolderCounts.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection;
            var ascending = currentOrder != SortOrder.Ascending;
            
            switch (columnName)
            {
                case "FolderPath":
                    folderCounts = ascending
                        ? folderCounts.OrderBy(f => f.FolderPath).ToList()
                        : folderCounts.OrderByDescending(f => f.FolderPath).ToList();
                    break;
                case "FileCount":
                    folderCounts = ascending
                        ? folderCounts.OrderBy(f => f.FileCount).ToList()
                        : folderCounts.OrderByDescending(f => f.FileCount).ToList();
                    break;
                case "FormattedSize":
                    folderCounts = ascending
                        ? folderCounts.OrderBy(f => f.TotalSize).ToList()
                        : folderCounts.OrderByDescending(f => f.TotalSize).ToList();
                    break;
            }
            
            RefreshDisplay();
            dgvFolderCounts.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection =
                ascending ? SortOrder.Ascending : SortOrder.Descending;
        }
        
        private void BtRefresh_Click(object sender, EventArgs e)
        {
            // Signal parent to rescan
            DialogResult = DialogResult.Retry;
            Close();
        }
        
        private void BtExport_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                saveDialog.Title = "Export Folder Counts";
                saveDialog.FileName = $"FolderCounts_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportToCsv(saveDialog.FileName);
                        MessageBox.Show($"Exported to:\n{saveDialog.FileName}", "Export Successful",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed:\n{ex.Message}", "Export Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void ExportToCsv(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Folder Path,File Count,Total Size (Bytes),Formatted Size");
                
                // Write data
                foreach (var folder in folderCounts)
                {
                    writer.WriteLine($"\"{folder.FolderPath}\",{folder.FileCount},{folder.TotalSize},\"{folder.FormattedSize}\"");
                }
            }
        }
    }
}