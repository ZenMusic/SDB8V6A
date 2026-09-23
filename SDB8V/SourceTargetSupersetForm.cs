using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SymbolDB
{
    public sealed class SourceTargetSupersetForm : Form
    {
        private readonly FileFunctions _ff;
        private readonly string _sourceRoot;
        private readonly string _targetRoot;

        private readonly DataGridView _grid;
        private readonly Button _btApply;
        private readonly Button _btClearChecks;
        private readonly Button _btClose;
        private readonly CheckBox _cbHideBoth;

        private readonly BindingList<SourceTargetEntry> _items = new();
        private List<SourceTargetEntry> _allItems = new();
        private bool _suppressBoth;

        public SourceTargetSupersetForm(FileFunctions ff, string sourceRoot, string targetRoot)
        {
            _ff = ff;
            _sourceRoot = NormalizeRoot(sourceRoot);
            _targetRoot = NormalizeRoot(targetRoot);

            Text = "Source/Target Superset";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1300, 720);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SourceTargetEntry.RootRelativePath),
                HeaderText = "RootRelativePath",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SourceTargetEntry.Status),
                HeaderText = "Status",
                Width = 80
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SourceTargetEntry.SourcePath),
                HeaderText = "SourcePath",
                Width = 360
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(SourceTargetEntry.TargetPath),
                HeaderText = "TargetPath",
                Width = 360
            });
            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(SourceTargetEntry.CopyToTarget),
                HeaderText = "Copy→Target",
                Width = 90
            });
            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(SourceTargetEntry.CopyToSource),
                HeaderText = "Copy→Source",
                Width = 90
            });
            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(SourceTargetEntry.DeleteSource),
                HeaderText = "Delete Src",
                Width = 90
            });
            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = nameof(SourceTargetEntry.DeleteTarget),
                HeaderText = "Delete Tgt",
                Width = 90
            });

            _grid.DataSource = _items;

            _btApply = new Button
            {
                Text = "Apply Actions",
                Width = 140,
                Height = 30
            };
            _btApply.Click += (_, __) => ApplyActions();

            _btClearChecks = new Button
            {
                Text = "Clear Checks",
                Width = 120,
                Height = 30
            };
            _btClearChecks.Click += (_, __) => ClearChecks();

            _cbHideBoth = new CheckBox
            {
                Text = "Hide both (show differences only)",
                AutoSize = true
            };
            _cbHideBoth.CheckedChanged += (_, __) =>
            {
                _suppressBoth = _cbHideBoth.Checked;
                ApplyFilter();
            };

            _btClose = new Button
            {
                Text = "Close",
                Width = 100,
                Height = 30
            };
            _btClose.Click += (_, __) => Close();

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(8)
            };
            panel.Controls.Add(_btApply);
            panel.Controls.Add(_btClearChecks);
            panel.Controls.Add(_cbHideBoth);
            panel.Controls.Add(_btClose);

            Controls.Add(_grid);
            Controls.Add(panel);

            _grid.AllowUserToResizeColumns = true;
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            // Optional: Set specific columns to be resizable
            foreach (DataGridViewColumn col in _grid.Columns)
            {
                col.Resizable = DataGridViewTriState.True;
            }
        }

        public void SetItems(IEnumerable<SourceTargetEntry> items)
        {
            _allItems = items?.ToList() ?? new List<SourceTargetEntry>();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            _items.Clear();

            IEnumerable<SourceTargetEntry> view = _allItems;
            if (_suppressBoth)
            {
                view = view.Where(i => !string.Equals(i.Status, "both", StringComparison.OrdinalIgnoreCase));
            }

            foreach (var item in view)
            {
                _items.Add(item);
            }
        }

        private void ApplyActions()
        {
            foreach (var item in _items)
            {
                if (item == null)
                    continue;

                if (item.CopyToTarget)
                {
                    TryCopy(item.SourcePath, _targetRoot, item.RootRelativePath, out string targetPath);
                    item.TargetPath = targetPath ?? item.TargetPath;
                }

                if (item.CopyToSource)
                {
                    TryCopy(item.TargetPath, _sourceRoot, item.RootRelativePath, out string sourcePath);
                    item.SourcePath = sourcePath ?? item.SourcePath;
                }

                if (item.DeleteSource && File.Exists(item.SourcePath))
                {
                    _ff.FileDelete(item.SourcePath);
                }

                if (item.DeleteTarget && File.Exists(item.TargetPath))
                {
                    _ff.FileDelete(item.TargetPath);
                }

                UpdateStatus(item);
            }

            ApplyFilter();
            _grid.Refresh();
        }

        private void TryCopy(string sourcePath, string destRoot, string rootRelativePath, out string? fullDestPath)
        {
            fullDestPath = null;

            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return;

            if (string.IsNullOrWhiteSpace(destRoot))
                return;

            string destPath = Path.Combine(destRoot, rootRelativePath);
            string destDir = Path.GetDirectoryName(destPath) ?? destRoot;

            try
            {
                Directory.CreateDirectory(destDir);
                if (_ff.CopyFile(sourcePath, destDir))
                {
                    fullDestPath = destPath;
                }
            }
            catch
            {
                // best-effort; keep current state
            }
        }

        private void UpdateStatus(SourceTargetEntry item)
        {
            bool inSource = !string.IsNullOrWhiteSpace(item.SourcePath) && File.Exists(item.SourcePath);
            bool inTarget = !string.IsNullOrWhiteSpace(item.TargetPath) && File.Exists(item.TargetPath);

            item.Status = inSource && inTarget ? "both" : inSource ? "source" : inTarget ? "target" : "none";
        }

        private void ClearChecks()
        {
            foreach (var item in _allItems)
            {
                item.CopyToTarget = false;
                item.CopyToSource = false;
                item.DeleteSource = false;
                item.DeleteTarget = false;
            }
            ApplyFilter();
            _grid.Refresh();
        }

        private static string NormalizeRoot(string root)
        {
            if (string.IsNullOrWhiteSpace(root))
                return string.Empty;

            try
            {
                var full = Path.GetFullPath(root.Trim());
                return Path.TrimEndingDirectorySeparator(full);
            }
            catch
            {
                return root.Trim();
            }
        }
    }

    public sealed class SourceTargetEntry
    {
        public string RootRelativePath { get; set; } = "";
        public string Status { get; set; } = ""; // "both" | "source" | "target" | "none"

        public string SourcePath { get; set; } = "";
        public string TargetPath { get; set; } = "";

        public bool CopyToTarget { get; set; }
        public bool CopyToSource { get; set; }
        public bool DeleteSource { get; set; }
        public bool DeleteTarget { get; set; }
    }
}