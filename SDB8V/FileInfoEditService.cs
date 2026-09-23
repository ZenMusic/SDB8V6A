using System;
using System.Linq;
using System.Windows.Forms;

namespace SymbolDB
{
    /// <summary>
    /// Shared service for editing FileInfoItem records across Main and DialogTraverser forms.
    /// </summary>
    public static class FileInfoEditService
    {
        /// <summary>
        /// Opens the EditFileInfo dialog and applies changes to the specified DataGridView row.
        /// </summary>
        /// <param name="dgv">The DataGridView containing file info data</param>
        /// <param name="rowIndex">The row index to edit</param>
        /// <param name="parentForm">The parent form (for dialog owner)</param>
        /// <returns>The updated FileInfoItem if changes were made, null otherwise</returns>
        public static FileInfoItem EditFileInfoRow(DataGridView dgv, int rowIndex, Form parentForm)
        {
            if (dgv == null || rowIndex < 0 || rowIndex >= dgv.Rows.Count)
            {
                MessageBox.Show("Invalid row selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            var row = dgv.Rows[rowIndex];
            if (row.IsNewRow)
            {
                MessageBox.Show("Cannot edit new row.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            // Build FileInfoItem from row data
            var currentFileInfo = BuildFileInfoFromRow(row);
            
            // Open the edit dialog
            EditFileInfo editForm;
            if (parentForm is DialogTraverser traverser)
            {
                editForm = new EditFileInfo(currentFileInfo, traverser);
            }
            else if (parentForm is Main mainForm)
            {
                editForm = new EditFileInfo(currentFileInfo, mainForm);
            }
            else
            {
                editForm = new EditFileInfo(currentFileInfo, null as DialogTraverser);
            }

            using (editForm)
            {
                editForm.PopulateFields(currentFileInfo);
                if (editForm.ShowDialog(parentForm) == DialogResult.OK)
                {
                    var updatedFileInfo = editForm.UpdatedFileInfo();
                    ApplyFileInfoToRow(updatedFileInfo, row);
                    return updatedFileInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Applies rating, source, and comment changes from EditFileInfo to a DGV row.
        /// </summary>
        public static void ApplyRatingChange(FileInfoItem fileInfo, DataGridView dgv, int rowIndex)
        {
            if (dgv == null || rowIndex < 0 || rowIndex >= dgv.Rows.Count || fileInfo == null)
                return;

            var row = dgv.Rows[rowIndex];
            row.Cells["rating"].Value = fileInfo.rating;
            row.Cells["source"].Value = fileInfo.source;
            row.Cells["comment"].Value = fileInfo.comment;
        }

        /// <summary>
        /// Builds a FileInfoItem from a DataGridViewRow.
        /// </summary>
        private static FileInfoItem BuildFileInfoFromRow(DataGridViewRow row)
        {
            return new FileInfoItem
            {
                keyPath = GetCellString(row, "keyPath"),
                fname = GetCellString(row, "fname"),
                ext = GetCellString(row, "ext"),
                dpath = GetCellString(row, "dpath"),
                fpath = GetCellString(row, "fpath"),
                type = GetCellString(row, "type"),
                level = GetCellInt(row, "level"),
                len = GetCellLong(row, "len"),
                stimestamp = GetCellString(row, "stimestamp"),
                bDelete = GetCellBool(row, "bDelete"),
                bInvalid = GetCellBool(row, "bInvalid"),
                rating = GetCellChar(row, "rating"),
                comment = GetCellString(row, "comment"),
                source = GetCellString(row, "source"),
                playTime = GetCellDouble(row, "playTime"),
                minutes = GetCellInt(row, "minutes"),
                seconds = GetCellInt(row, "seconds"),
                width = GetCellInt(row, "width"),
                height = GetCellInt(row, "height")
            };
        }

        /// <summary>
        /// Applies a FileInfoItem's data to a DataGridViewRow.
        /// </summary>
        private static void ApplyFileInfoToRow(FileInfoItem info, DataGridViewRow row)
        {
            SetCellValue(row, "fname", info.fname);
            SetCellValue(row, "ext", info.ext);
            SetCellValue(row, "dpath", info.dpath);
            SetCellValue(row, "fpath", info.fpath);
            SetCellValue(row, "type", info.type);
            SetCellValue(row, "level", info.level);
            SetCellValue(row, "len", info.len);
            SetCellValue(row, "stimestamp", info.stimestamp);
            SetCellValue(row, "bDelete", info.bDelete);
            SetCellValue(row, "bInvalid", info.bInvalid);
            SetCellValue(row, "rating", info.rating);
            SetCellValue(row, "source", info.source);
            SetCellValue(row, "playTime", info.playTime);
            SetCellValue(row, "minutes", info.minutes);
            SetCellValue(row, "seconds", info.seconds);
            SetCellValue(row, "width", info.width);
            SetCellValue(row, "height", info.height);
            SetCellValue(row, "comment", info.comment);
        }

        // Helper methods for cell access
        private static string GetCellString(DataGridViewRow row, string colName)
        {
            var col = FindColumn(row.DataGridView, colName);
            return col != null ? row.Cells[col.Index].Value?.ToString() ?? "" : "";
        }

        private static int GetCellInt(DataGridViewRow row, string colName)
        {
            var val = GetCellString(row, colName);
            return int.TryParse(val, out var result) ? result : 0;
        }

        private static long GetCellLong(DataGridViewRow row, string colName)
        {
            var val = GetCellString(row, colName);
            return long.TryParse(val, out var result) ? result : 0;
        }

        private static double GetCellDouble(DataGridViewRow row, string colName)
        {
            var val = GetCellString(row, colName);
            return double.TryParse(val, out var result) ? result : 0;
        }

        private static bool GetCellBool(DataGridViewRow row, string colName)
        {
            var val = GetCellString(row, colName);
            return bool.TryParse(val, out var result) && result;
        }

        private static char GetCellChar(DataGridViewRow row, string colName)
        {
            var val = GetCellString(row, colName);
            return string.IsNullOrEmpty(val) ? ' ' : val[0];
        }

        private static void SetCellValue(DataGridViewRow row, string colName, object value)
        {
            var col = FindColumn(row.DataGridView, colName);
            if (col != null)
                row.Cells[col.Index].Value = value;
        }

        private static DataGridViewColumn FindColumn(DataGridView dgv, string colName)
        {
            return dgv.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c => string.Equals(c.Name, colName, StringComparison.OrdinalIgnoreCase));
        }
    }
}