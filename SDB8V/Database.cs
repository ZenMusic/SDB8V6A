using System;
using System.Collections.Generic;
using System.ComponentModel;
//
using Microsoft.Data.Sqlite;
using System.Data;
//
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class Database : Form
    {
        private readonly FileInfoRepository _repo = new FileInfoRepository();

        // This is what dgvDatabase will bind to:
        private readonly BindingList<FileInfoItem> _items = new BindingList<FileInfoItem>();

        // Track which rows were deleted in the grid (by fpath)
        private readonly HashSet<string> _deletedFpaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public Database()
        {
            InitializeComponent();
            GetDbInfo();
            InitializeBackupFolderView();
            DBconnect();
        }
        private void LoadFileInfoItemsIntoGrid()
        {
            _items.Clear();
            _deletedFpaths.Clear();

            var list = _repo.GetAllOrderedByFname();
            foreach (var item in list)
                _items.Add(item);
        }
        public void GetDbInfo()
        {
            tbMessage.Text = $"DB = {SqliteDb.DbPath}";
            tbFpath.Text = "schema version and target " + SqliteDb.GetSchemaVersionInfo().ToString();
            btBackupDB.Focus();
        }

        public bool DBconnect()
        {
            string sourcePath = SqliteDb.DbPath;
            using (var conn = new SqliteConnection($"Data Source={sourcePath}"))
            {
                try
                {
                    conn.Open();
                    tbMessage.Text = $"DB Connection opened successfully!   >> {sourcePath}";
                }
                catch (SqliteException ex)
                {
                    tbMessage.Text = $"SQLite error: {ex.Message}";
                    return false;
                }
                catch (Exception ex)
                {
                    tbMessage.Text = $"General error: {ex.Message}";
                    return false;
                }
                conn.Close();
            }
            return true;
        }
        private void btBackupDB_Click(object sender, EventArgs e)
        {
            try
            {
                string sourcePath = SqliteDb.DbPath;  // your app’s live DB
                string backupFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SDB7Backups");

                Directory.CreateDirectory(backupFolder);

                // Build timestamped backup filename
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string backupFile = Path.Combine(backupFolder, $"SDB7_backup_{timeStamp}.db");

                // Use SQLite backup API for a consistent copy
                using (var source = new SqliteConnection($"Data Source={sourcePath}"))
                using (var dest = new SqliteConnection($"Data Source={backupFile}"))
                {
                    try
                    {
                        source.Open();
                        tbMessage.Text = "Connection opened successfully!";
                    }
                    catch (SqliteException ex)
                    {
                        tbMessage.Text = $"SQLite error: {ex.Message}";
                    }
                    catch (Exception ex)
                    {
                        tbMessage.Text = $"General error: {ex.Message}";
                    }
                    dest.Open();
                    source.BackupDatabase(dest);
                }

                System.Windows.Forms.MessageBox.Show(
                    $"Database backup created successfully:\n{backupFile}",
                    "Backup Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Refresh the backup folder view
                DisplayBackupFolderContents();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Backup failed:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private ListBox lbBackupFiles;

        private void InitializeBackupFolderView()
        {
            // Initialize the ListBox to display backup files
            lbBackupFiles = new ListBox
            {
                Dock = DockStyle.Bottom,
                Height = 150
            };

            // Add column headings as the first item in the ListBox
            lbBackupFiles.Items.Add("Filename - Creation Time - Size (KB)");

            this.Controls.Add(lbBackupFiles);

            // Display the contents of the backup folder
            DisplayBackupFolderContents();
        }

        private void DisplayBackupFolderContents()
        {
            try
            {
                string backupFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SDB7Backups");

                if (!Directory.Exists(backupFolder))
                {
                    lbBackupFiles.Items.Clear();
                    lbBackupFiles.Items.Add("Filename - Creation Time - Size (KB)");
                    lbBackupFiles.Items.Add("No backups found.");
                    return;
                }

                var backupFiles = Directory.GetFiles(backupFolder, "*.db")
                                           .OrderByDescending(File.GetCreationTime)
                                           .ToList();

                lbBackupFiles.Items.Clear();
                lbBackupFiles.Items.Add("Filename - Creation Time - Size (KB)");
                if (backupFiles.Any())
                {
                    foreach (var file in backupFiles)
                    {
                        var fileInfo = new FileInfo(file);
                        string displayText = $"{fileInfo.Name} - {fileInfo.CreationTime:yyyy-MM-dd HH:mm:ss} - {fileInfo.Length / 1024} KB";
                        lbBackupFiles.Items.Add(displayText);
                    }
                }
                else
                {
                    lbBackupFiles.Items.Add("No backups found.");
                }
            }
            catch (Exception ex)
            {
                lbBackupFiles.Items.Clear();
                lbBackupFiles.Items.Add("Filename - Creation Time - Size (KB)");
                lbBackupFiles.Items.Add($"Error loading backups: {ex.Message}");
            }
        }
        private static FileInfoItem Map(System.Data.IDataRecord r)
        {
            return new FileInfoItem
            {
                keyPath = r.IsDBNull(0) ? "" : r.GetString(0),
                fname = r.IsDBNull(1) ? "" : r.GetString(1),
                ext = r.IsDBNull(2) ? "" : r.GetString(2),
                len = r.IsDBNull(3) ? 0 : r.GetInt64(3),
                stimestamp = r.IsDBNull(4) ? "" : r.GetString(4),
                rating = r.IsDBNull(5) ? ' ' : (r.GetString(5).Length > 0 ? r.GetString(5)[0] : ' '),
                comment = r.IsDBNull(6) ? "" : r.GetString(6),
                bDelete = !r.IsDBNull(7) && r.GetInt32(7) != 0,
                bInvalid = !r.IsDBNull(8) && r.GetInt32(8) != 0,
                width = r.IsDBNull(9) ? 0 : r.GetInt32(9),
                height = r.IsDBNull(10) ? 0 : r.GetInt32(10),
                source = r.IsDBNull(11) ? "" : r.GetString(11),
                playTime = r.IsDBNull(12) ? 0 : r.GetDouble(12),
                minutes = r.IsDBNull(13) ? 0 : r.GetInt32(13),
                seconds = r.IsDBNull(14) ? 0 : r.GetInt32(14),
                dpath = r.IsDBNull(15) ? "" : r.GetString(15),
                ndx = r.IsDBNull(16) ? 0 : r.GetInt32(16),
                type = r.IsDBNull(17) ? "" : r.GetString(17),
                level = r.IsDBNull(18) ? 0 : r.GetInt32(18),
                fpath = r.IsDBNull(19) ? "" : r.GetString(19),
            };
        }
        public FileInfoItem? GetByKeyPath(string fullPath)
        {
            var key = PathHelpers.ToNormalizedKey(fullPath);

            const string sql = @"
                SELECT
                    keyPath, fname, ext, len, stimestamp, rating, comment,
                    bDelete, bInvalid, width, height, source, playTime,
                    minutes, seconds, dpath, ndx, type, level, fpath
                FROM FileInfoItem
                WHERE keyPath = $n
                LIMIT 1;";

            using var reader = SqliteDb.Query(sql, ("$n", key));
            if (!reader.Read()) return null;
            return Map(reader);
        }
        private void Database_Load(object sender, EventArgs e)
        {

        }

        //   normalized,

        private void LoadDatabaseAsObjects()
        {
            var list = new BindingList<FileInfoItem>();

            using var conn = new SqliteConnection(SqliteDb.ConnectionString);
            conn.Open();

            const string sql = @"
        SELECT
            keyPath, fname, ext, len, stimestamp, rating, comment,
                    bDelete, bInvalid, width, height, source, playTime,
                    minutes, seconds, dpath, ndx, type, level, fpath
        FROM FileInfoItem
        ORDER BY fname COLLATE NOCASE;
    ";

            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var item = new FileInfoItem
                {
                    keyPath = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    fname = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    ext = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    len = reader.IsDBNull(3) ? 0 : reader.GetInt64(3),
                    stimestamp = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    rating = reader.IsDBNull(5) ? ' ' : reader.GetString(5)[0],
                    comment = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    bDelete = !reader.IsDBNull(7) && reader.GetInt32(7) != 0,
                    bInvalid = !reader.IsDBNull(8) && reader.GetInt32(8) != 0,
                    width = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                    height = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                    source = reader.IsDBNull(11) ? "" : reader.GetString(11),
                    playTime = reader.IsDBNull(12) ? 0.0 : reader.GetDouble(12),
                    minutes = reader.IsDBNull(13) ? 0 : reader.GetInt32(13),
                    seconds = reader.IsDBNull(14) ? 0 : reader.GetInt32(14),
                    dpath = reader.IsDBNull(15) ? "" : reader.GetString(15),
                    ndx = reader.IsDBNull(16) ? 0 : reader.GetInt32(16),
                    type = reader.IsDBNull(17) ? "f" : reader.GetString(17),
                    level = reader.IsDBNull(18) ? 0 : reader.GetInt32(18),
                    fpath = reader.IsDBNull(19) ? "" : reader.GetString(19)
                };

                list.Add(item);
            }

            dgvDatabase.AutoGenerateColumns = true;
            dgvDatabase.DataSource = list;

            formatDataGridViewFileList();
            ConfigureDatabaseGrid();
        }
        public void AllowEditing()
        {
            dgvDatabase.ReadOnly = false;
            dgvDatabase.AllowUserToAddRows = true;
            dgvDatabase.AllowUserToDeleteRows = true;
            // dgvDatabase.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvDatabase.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
        }
        private void ConfigureDatabaseGrid()
        {
            dgvDatabase.ReadOnly = false;
            dgvDatabase.AllowUserToAddRows = true;
            dgvDatabase.AllowUserToDeleteRows = true;
            dgvDatabase.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvDatabase.MultiSelect = false;
            //dgvDatabase.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvDatabase.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            dgvDatabase.AutoGenerateColumns = true;
        }
        public void AutoSizeDGVColumns()
        {
            dgvDatabase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDatabase.AutoResizeColumns();
            SetMaxColumnWidth("keyPath", 800);
            SetMaxColumnWidth("fname", 400);
          //  SetMaxColumnWidth("dpath", 300);
           // SetMaxColumnWidth("fname", 200);
        }
        /// <summary>
        /// Enforces a max width on a specific column *after* autosizing.
        /// </summary>
        private void SetMaxColumnWidth(string columnName, int maxWidth)
        {
            if (!dgvDatabase.Columns.Contains(columnName))
                return;

            var col = dgvDatabase.Columns[columnName];

            if (col.Width > maxWidth)
                col.Width = maxWidth;

            // Optional: Prevent user from stretching past max
            col.Resizable = DataGridViewTriState.True;
            col.MinimumWidth = Math.Min(col.Width, maxWidth);
            col.FillWeight = 1;
        }
        private void btLoadFileInfoItem_Click(object sender, EventArgs e)
        {
            LoadDatabaseAsObjects();
            AutoSizeDGVColumns();
        }

        private void Database_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        void formatDataGridViewFileList(int column0Width = 0) //2020
        {
            for (int idx = 0; idx < dgvDatabase.Columns.Count; idx++)
            {
                if (idx == 15)
                {
                    dgvDatabase.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvDatabase.Columns[idx].Width = 10;
                }
                else
                {
                    dgvDatabase.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    int colw = dgvDatabase.Columns[idx].Width;
                    dgvDatabase.Columns[idx].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvDatabase.Columns[idx].Width = colw;
                    if (idx == 0 && column0Width > 0)
                        dgvDatabase.Columns[idx].Width = column0Width;
                    else if (idx == 4)
                        dgvDatabase.Columns[idx].Width = 6;
                    // else if (idx <= 6)
                    //   dgvDatabase.Columns[idx].ReadOnly = true;
                }
            }
            return;
        }

        private void btAddRow_Click(object sender, EventArgs e)
        {
            var item = new FileInfoItem
            {
                keyPath = "",
                fname = "",
                ext = "",
                len = 0,
                stimestamp = "",
                rating = ' ',
                comment = "",
                bDelete = false,
                bInvalid = false,
                width = 0,
                height = 0,
                source = "",
                playTime = 0,
                minutes = 0,
                seconds = 0,
                dpath = "",
                ndx = 0,
                type = "f",
                level = 0,
                fpath = ""
            };

            _items.Add(item);

            // optional: move focus to the new row
            int newIndex = _items.Count - 1;
            if (newIndex >= 0)
            {
                dgvDatabase.ClearSelection();
                dgvDatabase.Rows[newIndex].Selected = true;
                dgvDatabase.CurrentCell = dgvDatabase.Rows[newIndex].Cells[0];
                dgvDatabase.FirstDisplayedScrollingRowIndex = newIndex;
            }
        }

        private void btDeleteRow_Click(object sender, EventArgs e)
        {
            if (dgvDatabase.CurrentRow == null || dgvDatabase.CurrentRow.IsNewRow)
                return;

            if (dgvDatabase.CurrentRow.DataBoundItem is FileInfoItem item)
            {
                if (!string.IsNullOrWhiteSpace(item.fpath))
                {
                    _deletedFpaths.Add(item.fpath);
                }

                _items.Remove(item);
            }
        }

        private void btApplyUpdates_Click(object sender, EventArgs e)
        {
            try
            {
                using var conn = new SqliteConnection(SqliteDb.ConnectionString);
                conn.Open();

                using var tx = conn.BeginTransaction();

                // 1) Delete rows that were removed in the grid
                foreach (var fpath in _deletedFpaths.ToList())
                {
                    _repo.DeleteByFpath(fpath, conn, tx);
                }

                // 2) Upsert all current items in the grid
                foreach (var item in _items)
                {
                    // Skip rows without a key
                    if (string.IsNullOrWhiteSpace(item.fpath))
                        continue;

                    // Ensure normalized is filled
                    item.keyPath = SqliteDb.NormalizePathKey(item.fpath);

                    _repo.Upsert(item, conn, tx);
                }

                tx.Commit();
                _deletedFpaths.Clear();

                MessageBox.Show("Database updated successfully.", "Apply Updates",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Optionally reload from DB to reflect any triggers/defaults
                LoadFileInfoItemsIntoGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying updates:\r\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbFpath_TextChanged(object sender, EventArgs e)
        {

        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //
        //
        //




    }
}
