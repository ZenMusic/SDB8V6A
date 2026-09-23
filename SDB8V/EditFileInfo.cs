#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SymbolDB
{
    public partial class EditFileInfo : Form
    {
        public FileInfoItem _fileInfoItem;
        public DialogTraverser parentForm;
        public Main mainForm;
        // Constructors
        public EditFileInfo()
        {
            InitializeComponent();
            _fileInfoItem = new FileInfoItem();
        }

        public EditFileInfo(FileInfoItem fi, DialogTraverser parent)
        {
            parentForm = parent;
            InitializeComponent();
            if (fi != null)
            {
                PopulateFields(fi);
            }
        }
        public EditFileInfo(FileInfoItem fi, Main parent)
        {
            mainForm = parent;
            InitializeComponent();
            if (fi != null)
            {
                PopulateFields(fi);
            }
        }
        /// <summary>
        /// Populate UI from an existing FileInfoItem.
        /// Call this after constructing if you didn't pass the item in the constructor.
        /// </summary>
        public void PopulateFields(FileInfoItem currentFileInfo)
        {
            _fileInfoItem = currentFileInfo ?? new FileInfoItem();

            tbKeyPath.Text = _fileInfoItem.keyPath;
            tbFname.Text = _fileInfoItem.fname;
            tbExt.Text = _fileInfoItem.ext;
            tbDpath.Text = _fileInfoItem.dpath;
            tbFpath.Text = _fileInfoItem.fpath;
            tbType.Text = _fileInfoItem.type;
            numLevel.Text = _fileInfoItem.level.ToString();
            numLen.Text = _fileInfoItem.len.ToString();
            tbStimestamp.Text = _fileInfoItem.stimestamp.ToString();
            cbBDelete.Checked = _fileInfoItem.bDelete;
            cbBInvalid.Checked = _fileInfoItem.bInvalid;
            tbRating.Text = _fileInfoItem.rating.ToString();
            tbSource.Text = _fileInfoItem.source;
            numPlayTime.Text = _fileInfoItem.playTime.ToString();
            numMinutes.Text = _fileInfoItem.minutes.ToString();
            numSeconds.Text = _fileInfoItem.seconds.ToString();
            numWidth.Text = _fileInfoItem.width.ToString();
            numHeight.Text = _fileInfoItem.height.ToString();
            tbComment.Text = _fileInfoItem.comment;
            // Set the cursor position to the tbRating field
            tbRating.Focus();
        }

        /// <summary>
        /// Reads all fields and returns a copy (doesn't mutate the original reference).
        /// </summary>
        public FileInfoItem UpdatedFileInfo()
        {
            var result = _fileInfoItem != null ? Clone(_fileInfoItem) : new FileInfoItem();

            result.keyPath = _fileInfoItem.keyPath;
            result.fname = tbFname.Text;
            result.ext = tbExt.Text;
            result.dpath = tbDpath.Text;
            result.fpath = tbFpath.Text;
            result.type = tbType.Text;
            result.level = TryInt(numLevel?.Text, result.level);
            result.len = TryLong(numLen?.Text, result.len);
            result.stimestamp = tbStimestamp?.Text ?? result.stimestamp;
            result.bDelete = cbBDelete?.Checked ?? result.bDelete;
            result.bInvalid = cbBInvalid?.Checked ?? result.bInvalid;
            result.rating = TryChar(tbRating?.Text, result.rating);
            result.source = tbSource?.Text ?? result.source;
            result.playTime = TryDouble(numPlayTime?.Text, result.playTime);
            result.minutes = TryInt(numMinutes?.Text, result.minutes);
            result.seconds = TryInt(numSeconds?.Text, result.seconds);
            result.width = TryInt(numWidth?.Text, result.width);
            result.height = TryInt(numHeight?.Text, result.height);
            result.comment = tbComment?.Text ?? result.comment;

            return result;

            static int TryInt(string s, int fallback) => int.TryParse(s, out var v) ? v : fallback;
            static long TryLong(string s, long fallback) => long.TryParse(s, out var v) ? v : fallback;
            static char TryChar(string s, char fallback) => char.TryParse(s, out var v) ? v : fallback;

            static double TryDouble(string s, double fallback)
              => double.TryParse(s, out var v) ? v : fallback;

            static FileInfoItem Clone(FileInfoItem x) => new FileInfoItem
            {
                fname = x.fname,
                ext = x.ext,
                dpath = x.dpath,
                fpath = x.fpath,
                type = x.type,
                level = x.level,
                len = x.len,
                stimestamp = x.stimestamp,
                bDelete = x.bDelete,
                bInvalid = x.bInvalid,
                rating = x.rating,
                source = x.source,
                playTime = x.playTime,
                minutes = x.minutes,
                seconds = x.seconds,
                width = x.width,
                height = x.height,
                comment = x.comment
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Cancel
            var btCancel = new Button
            {
                Text = "Close",
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(100, 30),
                Location = new Point(this.ClientSize.Width - 220, this.ClientSize.Height - 50)
            };
            btCancel.Click += (sender, args) => this.Close();
            this.Controls.Add(btCancel);

            // Apply Rating
            var btApplyRating = new Button
            {
                Text = "Apply Rating",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(100, 30),
                Location = new Point(this.ClientSize.Width - 110, this.ClientSize.Height - 50)
            };
            btApplyRating.Click += btApplyRating_Click;
            this.Controls.Add(btApplyRating);
        }

        private void tbRating_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys (backspace, etc.)
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            // Reject whitespace
            if (char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            var tb = (TextBox)sender;
            // If there's already a char, replace it with the typed one
            if (tb.TextLength > 0)
            {
                tb.Text = e.KeyChar.ToString();
                tb.SelectionStart = tb.Text.Length;
                e.Handled = true; // handled because we updated text manually
            }
            // otherwise allow the typed character (MaxLength = 1 enforces single char)
        }

        private void tbRating_TextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            if (string.IsNullOrEmpty(tb.Text)) return;

            // Trim whitespace/paste and keep only first character
            var trimmed = tb.Text.Trim();
            if (trimmed.Length == 0)
            {
                tb.Text = string.Empty;
                tb.SelectionStart = 0;
                return;
            }

            if (trimmed.Length > 1)
            {
                tb.Text = trimmed.Substring(0, 1);
                tb.SelectionStart = tb.Text.Length;
                return;
            }

            if (tb.Text != trimmed)
            {
                tb.Text = trimmed;
                tb.SelectionStart = tb.Text.Length;
            }
        }

        // safer assignment in ApplyRating
        private void btApplyRating_Click(object sender, EventArgs e)
        {
            if (_fileInfoItem == null) return;

            var s = (tbRating?.Text ?? string.Empty).Trim();
            if (s.Length > 0)
            {
                _fileInfoItem.rating = s[0];
            }

            _fileInfoItem.source = tbSource.Text;
            _fileInfoItem.comment = tbComment.Text;

            parentForm?.ApplyRatingChange(_fileInfoItem);
        }

        private void EditFileInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
           // previousRecord = updatedFileInfo;
        }
    }

    // NOTE:
    // - FileInfoItem and TraverserDialog are assumed to exist elsewhere in your project.
    //   Example shape (for reference only):
    //   public class FileInfoItem {
    //       public string fname, ext, dpath, fpath, type, source, desc;
    //       public int level, minutes, seconds, width, height;
    //       public long len, stimestamp, playTime;
    //       public bool bDelete, bInvalid;
    //       public char rating;
    //   }
    //   public class TraverserDialog : Form {
    //       public void ApplyRatingChange(FileInfoItem item) { /* ... */ }
    //   }
}
