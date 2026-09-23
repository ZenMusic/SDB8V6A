#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SymbolDB
{
    // ── Serialization helpers ────────────────────────────────────────────────
    [XmlRoot("SearchExtensionsData")]
    public class SearchExtensionsData
    {
        [XmlElement("Category")]
        public List<SearchExtensionCategory> Categories { get; set; } = new();
    }

    public class SearchExtensionCategory
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("Extension")]
        public List<string> Extensions { get; set; } = new();
    }

    // ── Editor form ──────────────────────────────────────────────────────────
    public partial class DialogSearchExtensionsEditor : Form
    {
        // ── Persistence ──────────────────────────────────────────────────────
        private static readonly string FileName = "searchExtensions.xml";

        public static string GetFilePath(GlobalVars gv)
            => Path.Combine(gv.dataFolder ?? GlobalVars.mydocsFolder, FileName);

        /// <summary>Load SearchExtensions from XML into gv.SearchExtensions.</summary>
        /// 
        /// <summary>Load SearchExtensions from XML into gv.SearchExtensions.
        /// If the file does not exist, creates default data, saves it, and loads it.</summary>
        public static int LoadFromFile(GlobalVars gv)
        {
            string path = GetFilePath(gv);
            if (!File.Exists(path))
            {
                gv.SearchExtensions = CreateDefaultData().Categories.ToDictionary(
                    c => c.Name,
                    c => c.Extensions.ToArray(),
                    StringComparer.OrdinalIgnoreCase);
                SaveToFile(gv);
                return gv.SearchExtensions.Count;
            }
            try
            {
                var xs = new XmlSerializer(typeof(SearchExtensionsData));
                using var reader = new StreamReader(path);
                var data = (SearchExtensionsData)xs.Deserialize(reader);
                gv.SearchExtensions = data.Categories.ToDictionary(
                    c => c.Name,
                    c => c.Extensions.ToArray(),
                    StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load search extensions:\n{ex.Message}",
                                "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return gv.SearchExtensions.Count;
        }

        /// <summary>Builds the built-in default SearchExtensionsData.</summary>
        private static SearchExtensionsData CreateDefaultData()
        {
            return new SearchExtensionsData
            {
                Categories = new List<SearchExtensionCategory>
                {
                    new() { Name = "all", Extensions = new List<string> { "*.*" } },
                    new() { Name = "html", Extensions = new List<string> { "*.htm*", "*.html", "*.mhtml" } },
                    new() { Name = "images", Extensions = new List<string> { "*.jpg", "*.png", "*.bmp", "*.jpeg", "*.avif" } },
                    new() { Name = "webp", Extensions = new List<string> { "*.webp" } },
                    new() { Name = "midi", Extensions = new List<string> { "*.mid", "*.mpe" } },
                    new() { Name = "videos", Extensions = new List<string> { "*.mp4", "*.wmv", "*.mov" } },
                    new() { Name = "videos_webm", Extensions = new List<string> { "*.webm" } },
                }
            };
        }
        public static int LoadFromFilexxxxxx(GlobalVars gv)
        {
            string path = GetFilePath(gv);
            if (!File.Exists(path))
            {
                return 0;
            }
            try
            {
                var xs = new XmlSerializer(typeof(SearchExtensionsData));
                using var reader = new StreamReader(path);
                var data = (SearchExtensionsData)xs.Deserialize(reader);
                gv.SearchExtensions = data.Categories.ToDictionary(
                    c => c.Name,
                    c => c.Extensions.ToArray(),
                    StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load search extensions:\n{ex.Message}",
                                "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return gv.SearchExtensions.Count;
        }

        /// <summary>Save gv.SearchExtensions to XML.</summary>
        public static void SaveToFile(GlobalVars gv)
        {
            if (gv.SearchExtensions == null) return;
            var data = new SearchExtensionsData
            {
                Categories = gv.SearchExtensions.Select(kv => new SearchExtensionCategory
                {
                    Name = kv.Key,
                    Extensions = kv.Value?.ToList() ?? new List<string>()
                }).OrderBy(c => c.Name).ToList()
            };
            string path = GetFilePath(gv);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                var xs = new XmlSerializer(typeof(SearchExtensionsData));
                using var writer = new StreamWriter(path);
                xs.Serialize(writer, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save search extensions:\n{ex.Message}",
                                "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Fields ───────────────────────────────────────────────────────────
        private readonly GlobalVars _gv;

        // Working copy — only committed to gv on Save
        private Dictionary<string, List<string>> _workingCopy;

        // Controls
        private ListBox    _lbCategories;
        private ListBox    _lbExtensions;
        private TextBox    _tbNewCategory;
        private TextBox    _tbNewExtension;
        private Button     _btnAddCategory;
        private Button     _btnDeleteCategory;
        private Button     _btnAddExtension;
        private Button     _btnDeleteExtension;
        private Button     _btnSave;
        private Button     _btnCancel;
        private Label      _lblCatHeader;
        private Label      _lblExtHeader;
        private Label      _lblFilePath;
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _statusLabel;

        // ── Constructor ──────────────────────────────────────────────────────
        public DialogSearchExtensionsEditor(GlobalVars gv)
        {
            _gv = gv;
            BuildWorkingCopy();
            BuildUI();
            RefreshCategoryList();
        }

        // ── Build working copy ───────────────────────────────────────────────
        private void BuildWorkingCopy()
        {
            _workingCopy = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            if (_gv.SearchExtensions != null)
            {
                foreach (var kv in _gv.SearchExtensions)
                    _workingCopy[kv.Key] = kv.Value?.ToList() ?? new List<string>();
            }

            // Seed defaults if empty
            if (_workingCopy.Count == 0)
            {
                _workingCopy["images"]      = new List<string> { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif", "*.webp" };
                _workingCopy["videos"]      = new List<string> { "*.mp4", "*.wmv", "*.mov", "*.avi", "*.mkv" };
                _workingCopy["videos_webm"] = new List<string> { "*.mp4", "*.wmv", "*.mov", "*.avi", "*.mkv", "*.webm" };
                _workingCopy["midi"]        = new List<string> { "*.mid", "*.mpe", "*.midi" };
                _workingCopy["html"]        = new List<string> { "*.htm*", "*.html", "*.mhtml" };
                _workingCopy["all"]         = new List<string> { "*.*" };
            }
        }

        // ── Build UI programmatically ─────────────────────────────────────────
        private void BuildUI()
        {
            Text            = "Search Extensions Editor";
            Size            = new Size(780, 520);
            MinimumSize     = new Size(700, 460);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            Font            = new Font("Segoe UI", 9f);

            // ── Status strip ─────────────────────────────────────────────────
            _statusStrip = new StatusStrip { Dock = DockStyle.Bottom };
            _statusLabel = new ToolStripStatusLabel("Ready") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            _statusStrip.Items.Add(_statusLabel);

            // ── File path label ──────────────────────────────────────────────
            _lblFilePath = new Label
            {
                Text      = $"File: {GetFilePath(_gv)}",
                Dock      = DockStyle.Bottom,
                Height    = 18,
                ForeColor = Color.Gray,
                Font      = new Font("Segoe UI", 7.5f)
            };

            // ── Main table layout ─────────────────────────────────────────────
            var table = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 4,
                Padding     = new Padding(8),
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));  // headers
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // list boxes
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));  // input + add
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));  // save / cancel

            // ── Headers ──────────────────────────────────────────────────────
            _lblCatHeader = new Label
            {
                Text      = "Categories",
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            _lblExtHeader = new Label
            {
                Text      = "Extensions  (select a category first)",
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            table.Controls.Add(_lblCatHeader, 0, 0);
            table.Controls.Add(_lblExtHeader, 1, 0);

            // ── List boxes ───────────────────────────────────────────────────
            _lbCategories = new ListBox
            {
                Dock          = DockStyle.Fill,
                IntegralHeight = false,
                Font          = new Font("Consolas", 9.5f)
            };
            _lbCategories.SelectedIndexChanged += LbCategories_SelectedIndexChanged;

            _lbExtensions = new ListBox
            {
                Dock          = DockStyle.Fill,
                IntegralHeight = false,
                Font          = new Font("Consolas", 9.5f)
            };

            table.Controls.Add(_lbCategories, 0, 1);
            table.Controls.Add(_lbExtensions, 1, 1);

            // ── Input row ─────────────────────────────────────────────────────
            var catInputPanel = new FlowLayoutPanel
            {
                Dock      = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                AutoSize      = false,
            };
            _tbNewCategory   = new TextBox { Width = 110, Height = 24 };
            _btnAddCategory  = MakeButton("Add",    Color.MediumSeaGreen, BtnAddCategory_Click);
            _btnDeleteCategory = MakeButton("Delete", Color.IndianRed,    BtnDeleteCategory_Click);
            catInputPanel.Controls.AddRange(new Control[] { _tbNewCategory, _btnAddCategory, _btnDeleteCategory });

            var extInputPanel = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                AutoSize      = false,
            };
            _tbNewExtension   = new TextBox { Width = 130, Height = 24 };
            _btnAddExtension  = MakeButton("Add",    Color.MediumSeaGreen, BtnAddExtension_Click);
            _btnDeleteExtension = MakeButton("Delete", Color.IndianRed,   BtnDeleteExtension_Click);
            extInputPanel.Controls.AddRange(new Control[] { _tbNewExtension, _btnAddExtension, _btnDeleteExtension });

            table.Controls.Add(catInputPanel, 0, 2);
            table.Controls.Add(extInputPanel, 1, 2);

            // ── Bottom buttons row ────────────────────────────────────────────
            var bottomPanel = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents  = false,
            };
            _btnCancel = MakeButton("Cancel", Color.LightGray, BtnCancel_Click, width: 80);
            _btnSave   = MakeButton("💾 Save", Color.SteelBlue, BtnSave_Click,  width: 90);
            _btnSave.ForeColor = Color.White;
            _btnSave.Font      = new Font("Segoe UI", 9f, FontStyle.Bold);
            bottomPanel.Controls.AddRange(new Control[] { _btnCancel, _btnSave });
            table.SetColumnSpan(bottomPanel, 2);
            table.Controls.Add(bottomPanel, 0, 3);

            // ── Assemble ──────────────────────────────────────────────────────
            Controls.Add(table);
            Controls.Add(_lblFilePath);
            Controls.Add(_statusStrip);

            AcceptButton = _btnSave;
            CancelButton = _btnCancel;

            // Enter key in text boxes
            _tbNewCategory.KeyDown  += (s, e) => { if (e.KeyCode == Keys.Enter) BtnAddCategory_Click(s, e); };
            _tbNewExtension.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnAddExtension_Click(s, e); };
        }

        private static Button MakeButton(string text, Color backColor, EventHandler handler, int width = 72)
        {
            var b = new Button
            {
                Text      = text,
                BackColor = backColor,
                Width     = width,
                Height    = 28,
                FlatStyle = FlatStyle.Flat,
                Margin    = new Padding(3, 2, 3, 2),
            };
            b.FlatAppearance.BorderColor = Color.Gray;
            b.Click += handler;
            return b;
        }

        // ── Refresh helpers ──────────────────────────────────────────────────
        private void RefreshCategoryList()
        {
            string selected = _lbCategories.SelectedItem as string;
            _lbCategories.BeginUpdate();
            _lbCategories.Items.Clear();
            foreach (var key in _workingCopy.Keys.OrderBy(k => k))
                _lbCategories.Items.Add(key);
            _lbCategories.EndUpdate();

            // Restore selection
            if (selected != null)
            {
                int idx = _lbCategories.FindStringExact(selected);
                if (idx >= 0) _lbCategories.SelectedIndex = idx;
            }
            else if (_lbCategories.Items.Count > 0)
                _lbCategories.SelectedIndex = 0;

            UpdateStatus();
        }

        private void RefreshExtensionList(string categoryKey)
        {
            _lbExtensions.BeginUpdate();
            _lbExtensions.Items.Clear();
            if (categoryKey != null && _workingCopy.TryGetValue(categoryKey, out var exts))
                foreach (var ext in exts)
                    _lbExtensions.Items.Add(ext);
            _lbExtensions.EndUpdate();
            _lblExtHeader.Text = categoryKey != null
                ? $"Extensions  —  category: \"{categoryKey}\""
                : "Extensions  (select a category first)";
        }

        private string SelectedCategory
            => _lbCategories.SelectedItem as string;

        private void UpdateStatus()
        {
            int totalExt = _workingCopy.Values.Sum(v => v.Count);
            _statusLabel.Text = $"{_workingCopy.Count} categories  |  {totalExt} total extensions";
        }

        // ── Event handlers ───────────────────────────────────────────────────
        private void LbCategories_SelectedIndexChanged(object sender, EventArgs e)
            => RefreshExtensionList(SelectedCategory);

        private void BtnAddCategory_Click(object sender, EventArgs e)
        {
            string name = _tbNewCategory.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                SetStatus("Enter a category name first.", Color.OrangeRed); return;
            }
            if (_workingCopy.ContainsKey(name))
            {
                SetStatus($"Category \"{name}\" already exists.", Color.OrangeRed); return;
            }
            _workingCopy[name] = new List<string>();
            _tbNewCategory.Clear();
            RefreshCategoryList();
            // Select the new one
            int idx = _lbCategories.FindStringExact(name);
            if (idx >= 0) _lbCategories.SelectedIndex = idx;
            SetStatus($"Added category \"{name}\".", Color.MediumSeaGreen);
        }

        private void BtnDeleteCategory_Click(object sender, EventArgs e)
        {
            string cat = SelectedCategory;
            if (cat == null) { SetStatus("Select a category to delete.", Color.OrangeRed); return; }
            var result = MessageBox.Show(
                $"Delete category \"{cat}\" and all its extensions?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;
            _workingCopy.Remove(cat);
            RefreshCategoryList();
            RefreshExtensionList(SelectedCategory);
            SetStatus($"Deleted category \"{cat}\".", Color.IndianRed);
        }

        private void BtnAddExtension_Click(object sender, EventArgs e)
        {
            string cat = SelectedCategory;
            if (cat == null) { SetStatus("Select a category first.", Color.OrangeRed); return; }
            string ext = _tbNewExtension.Text.Trim();
            if (string.IsNullOrEmpty(ext)) { SetStatus("Enter an extension pattern (e.g. *.jpg).", Color.OrangeRed); return; }

            var list = _workingCopy[cat];
            if (list.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                SetStatus($"\"{ext}\" already exists in \"{cat}\".", Color.OrangeRed); return;
            }
            list.Add(ext);
            _tbNewExtension.Clear();
            RefreshExtensionList(cat);
            _lbExtensions.SelectedIndex = _lbExtensions.Items.Count - 1;
            SetStatus($"Added \"{ext}\" to \"{cat}\".", Color.MediumSeaGreen);
            UpdateStatus();
        }

        private void BtnDeleteExtension_Click(object sender, EventArgs e)
        {
            string cat = SelectedCategory;
            if (cat == null) { SetStatus("Select a category first.", Color.OrangeRed); return; }
            string ext = _lbExtensions.SelectedItem as string;
            if (ext == null) { SetStatus("Select an extension to delete.", Color.OrangeRed); return; }

            _workingCopy[cat].Remove(ext);
            RefreshExtensionList(cat);
            SetStatus($"Deleted \"{ext}\" from \"{cat}\".", Color.IndianRed);
            UpdateStatus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Commit working copy → gv.SearchExtensions
            _gv.SearchExtensions = _workingCopy.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.ToArray(),
                StringComparer.OrdinalIgnoreCase);

            // Persist to XML
            SaveToFile(_gv);
            SetStatus($"Saved {_gv.SearchExtensions.Count} categories to {GetFilePath(_gv)}", Color.SteelBlue);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SetStatus(string msg, Color color)
        {
            _statusLabel.ForeColor = color;
            _statusLabel.Text      = msg;
        }

        // ── Static opener (call from DialogTraverser) ─────────────────────────
        /// <summary>
        /// Open the editor as a modal dialog. 
        /// Call from DialogTraverser with:
        ///   DialogSearchExtensionsEditor.Open(gv, this);
        /// </summary>
        public static void Open(GlobalVars gv, Form owner = null)
        {
            using var dlg = new DialogSearchExtensionsEditor(gv);
            dlg.ShowDialog(owner);
        }
    }
}