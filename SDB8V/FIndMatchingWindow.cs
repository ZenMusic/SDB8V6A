#nullable disable
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    /// <summary>
    /// Non-modal window that contains all Find-Matching-Image functionality
    /// previously hosted on panelFindMatching in Main.
    /// </summary>
    public partial class FindMatchingWindow : Form
    {
        // ── reference back to Main ──────────────────────────────────────────
        private readonly Main _main;

        // ── fields (previously in Main) ─────────────────────────────────────
        private CancellationTokenSource _cts;
        private readonly ImageMatcher _matcher = new ImageMatcher();
        private List<string> _matches = new List<string>();
        private string _lastSearchFolder;
        private double _matchThreshold = 0.9; // 90 %

        // ── ctor ─────────────────────────────────────────────────────────────
        GlobalVars gv;
        public FindMatchingWindow(Main main, GlobalVars g)
        {
            _main = main ?? throw new ArgumentNullException(nameof(main));
            gv = g ?? throw new ArgumentNullException(nameof(g));

            InitializeComponent();

            FormClosing += (s, e) =>
            {
                // hide instead of destroy so the window can be re-shown
                e.Cancel = true;
                Hide();
            };

            pbMatch.Image = _main.CurrentMainImage; // may be null — RefreshSourceImage handles it
            if (_main.CurrentMainImage == null)
            {
                MessageBox.Show("No source image is currently loaded. Please load an image first.", "No Source Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            pbMatch.SizeMode = PictureBoxSizeMode.Zoom;
            this.Refresh();
        }
        
        public void RefreshSourceImage()
        {
            var img = _main.CurrentMainImage;
            if (img != null)
                pbMatch.Image = img;
        }
        private void tbRowCount_TextChanged(object sender, EventArgs e)
        {
            // Implementation here
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Find Matching Image
        // ══════════════════════════════════════════════════════════════════════

        private void pbMatch_Click(object sender, EventArgs e)
        {
            // Show the path of the selected match in the search-result box
            if (lbMatches.SelectedItem is string path)
                tbSearchResult.Text = path;
        }

        private async void btFindMatchingImage_Click(object sender, EventArgs e)
        {
            var mainImage = _main.CurrentMainImage;
            if (mainImage == null)
            {
                MessageBox.Show("Please load a source image first.");
                return;
            }

            cbSwapNewList.Enabled = true;
            cbSwapNewList.Checked = false;
            tbSearchResult.Focus();
            try { _main.HideFileInfoPanel(); } catch { }

            // Show source image in the preview box
           // pbMatch.Image = mainImage;
            this.Refresh();

            // Ask the user for a folder to search (or reuse last one)
            string searchFolder;
            if (cbUseSameFolder.Checked && !string.IsNullOrEmpty(_lastSearchFolder))
            {
                searchFolder = _lastSearchFolder;
            }
            else
            {
                using var dlg = new FolderBrowserDialog();
                if (dlg.ShowDialog() != DialogResult.OK) return;
                searchFolder = dlg.SelectedPath;
                _lastSearchFolder = searchFolder;
            }
            tbSourceImageNumber.Text = searchFolder;

            // Reset cancellation token
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            btFindMatchingImage.Enabled = false;
            btDeleteMatchingImages.Enabled = false;
            lbMatches.Items.Clear();
            tbSearchResult.Text = "Searching…";

            // Show the full path of the source image being searched
            tbFileName.Text = _main.finfo1?.fpath ?? string.Empty;

            // Progress updates marshalled back to UI thread
            _matcher.ProgressCallback = (processed, found) =>
            {
                if (tbSearchResult.InvokeRequired)
                    tbSearchResult.BeginInvoke(new Action(() =>
                        tbSearchResult.Text = $"Compared {processed:N0} files, found {found:N0} matches…"));
                else
                    tbSearchResult.Text = $"Compared {processed:N0} files, found {found:N0} matches…";
            };

            List<string> results;
            try
            {
                results = await Task.Run(() =>
                    _matcher.FindMatches(
                        mainImage: new Bitmap(mainImage),
                        rootDirectory: searchFolder,
                        sourceImagePath: _main.finfo1?.fpath ?? string.Empty,
                        threshold: _matchThreshold,
                        cancellationToken: _cts.Token
                    ), _cts.Token);
            }
            catch (OperationCanceledException)
            {
                tbSearchResult.Text = "Cancelled";
                btFindMatchingImage.Enabled = true;
                btCancelFind.Enabled = false;
                return;
            }

            // Remove the source image itself from results
            string sourceNormalized = string.Empty;
            try
            {
                sourceNormalized = Path.GetFullPath(_main.finfo1?.fpath ?? string.Empty)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            catch { }

            if (!string.IsNullOrEmpty(sourceNormalized))
            {
                results.RemoveAll(m =>
                {
                    try
                    {
                        string mn = Path.GetFullPath(m)
                            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        return string.Equals(mn, sourceNormalized, StringComparison.OrdinalIgnoreCase);
                    }
                    catch { return false; }
                });
            }

            // Populate UI
            lbMatches.Items.AddRange(results.ToArray());
            tbSearchResult.Text = $"Done: {results.Count:N0} matches.";

            btFindMatchingImage.Enabled = true;
            btDeleteMatchingImages.Enabled = results.Count > 0;
            _matches = results;

            if (cbToNewList.Checked)
                BuildMatchingImagesList(results);
            else
                PrependMatchesAndEnsureMetadata(results);
        }

        private void lbMatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbMatches_MouseDoubleClick(null, null);
            int idx = lbMatches.SelectedIndex;
        }

        private void lbMatches_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(lbMatches.SelectedItem is string path) || !File.Exists(path))
                return;
            try
            {
                var newImg = LoadImageNoLock(path);
                var old = pbMatch.Image;
                pbMatch.Image = newImg;
                old?.Dispose();
            }
            catch (Exception ex)
            {
                _main.StopAll();
                MessageBox.Show($"Unable to load image:\n{ex.Message}",
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btDeleteMatchingImages_Click(object sender, EventArgs e)
        {
            if (_matches == null || _matches.Count == 0)
            {
                MessageBox.Show("No matches to delete.");
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to permanently delete {_matches.Count} files?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;
            _main.ClearAllPictureBoxesInUse();

            _matcher.DeleteMatches(_matches);

            // Remove from ImageFileList
            var toDelete = lbMatches.Items.Cast<string>().ToList();
            foreach (var fullpath in toDelete)
                gv.imageFileList1.FindItemAndDelete(fullpath);

            // Save count text, navigate, restore
            string slideNumber = gv.slideCount1.ToString("N0");
            _main.showNextSlideImageFileList1(0, 1);
            tbSearchResult.Text = "deleted";
            _matches.Clear();
            lbMatches.Items.Clear();
            btDeleteMatchingImages.Enabled = false;
            _main.SetGoToSlideText(slideNumber);
            gv.slideCount1 = gv.imageFileList1.getImageCount();
        }

        private void cbShowMatchPercentage_CheckedChanged(object sender, EventArgs e)
        {
            gv.showMatchPercentage = cbShowMatchPercentage.Checked;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Cancel
        // ══════════════════════════════════════════════════════════════════════
        private void btCancelFind_Click(object sender, EventArgs e)
        {
            btCancelFind.Enabled = false;
            _cts?.Cancel();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Swap / show-match-% checkboxes
        // ══════════════════════════════════════════════════════════════════════

        private void cbSwapNewList_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSwapNewList.Checked)
            {
                try
                {
                    if (gv.imageFileListMatches == null)
                    {
                        cbSwapNewList.Checked = false;
                        cbSwapNewList.Enabled = false;
                        return;
                    }

                    int matchCount = 0;
                    try { matchCount = gv.imageFileListMatches.getImageFileListLength(); }
                    catch
                    {
                        try { matchCount = gv.imageFileListMatches.getImageCount(); }
                        catch { matchCount = gv.imageFileListMatches.finfoList?.Count ?? 0; }
                    }

                    if (matchCount <= 0)
                    {
                        cbSwapNewList.Checked = false;
                        cbSwapNewList.Enabled = false;
                        return;
                    }

                    if (gv.imageFileListHolding == null)
                        gv.imageFileListHolding = new ImageFileList();

                    if (gv.imageFileList1 != null)
                        CopyImageFileList(gv.imageFileList1, gv.imageFileListHolding);

                    if (gv.imageFileList1 == null)
                        gv.imageFileList1 = new ImageFileList();

                    CopyImageFileList(gv.imageFileListMatches, gv.imageFileList1);

                    try { gv.slideCount1 = gv.imageFileList1.getImageFileListLength(); }
                    catch
                    {
                        try { gv.slideCount1 = gv.imageFileList1.getImageCount(); }
                        catch { gv.slideCount1 = gv.imageFileList1.finfoList?.Count ?? 0; }
                    }

                    _main.SetImageCountText(gv.slideCount1.ToString("N0"));
                    _main.SetGoToSlideText("0");
                    _main.DoGoToSlideNumber();
                    gv.debug.w($"Swap ON: swapped {matchCount} matches into main list.");
                }
                catch (Exception ex)
                {
                    gv.debug.w("cbSwapNewList_CheckedChanged ON failed:", ex.Message);
                    cbSwapNewList.Checked = false;
                }
            }
            else
            {
                try
                {
                    if (gv.imageFileListHolding == null) { gv.debug.w("Swap OFF: no holding list."); return; }
                    if (gv.imageFileList1 == null) gv.imageFileList1 = new ImageFileList();

                    CopyImageFileList(gv.imageFileListHolding, gv.imageFileList1);

                    try { gv.slideCount1 = gv.imageFileList1.getImageFileListLength(); }
                    catch
                    {
                        try { gv.slideCount1 = gv.imageFileList1.getImageCount(); }
                        catch { gv.slideCount1 = gv.imageFileList1.finfoList?.Count ?? 0; }
                    }

                    _main.SetImageCountText(gv.slideCount1.ToString("N0"));
                    _main.SetGoToSlideText("0");
                    _main.DoGoToSlideNumber();
                    gv.debug.w("Swap OFF: restored original list from holding.");
                }
                catch (Exception ex)
                {
                    gv.debug.w("cbSwapNewList_CheckedChanged OFF failed:", ex.Message);
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  Helpers (moved from Main)
        // ══════════════════════════════════════════════════════════════════════

        public int BuildMatchingImagesList(List<string> matches)
        {
            if (matches == null || matches.Count == 0) return 0;

            try
            {
                if (gv.imageFileListMatches == null)
                    gv.imageFileListMatches = new ImageFileList();
                else
                    gv.imageFileListMatches.clearList();
            }
            catch (Exception ex) { gv.debug.w("BuildMatchingImagesList: init failed", ex.Message); return 0; }

            int added = 0;
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var m in matches)
            {
                if (string.IsNullOrWhiteSpace(m)) continue;
                string full;
                try { full = Path.GetFullPath(m); } catch { full = m; }
                if (seen.Contains(full)) continue;

                var newItem = new FileInfoItem();
                try
                {
                    newItem.fpath = full;
                    var fi = new FileInfo(full);
                    newItem.fname = Path.GetFileName(full);
                    newItem.dpath = Path.GetDirectoryName(full) ?? "";
                    newItem.ext = Path.GetExtension(full) ?? "";
                    newItem.len = fi.Exists ? fi.Length : 0;
                    newItem.stimestamp = fi.Exists ? fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
                }
                catch { }

                try
                {
                    if (File.Exists(full))
                    {
                        using var fs = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var img = Image.FromStream(fs, false, false);
                        newItem.width = img.Width;
                        newItem.height = img.Height;
                    }
                }
                catch { }

                try { gv.imageFileListMatches.addItem(newItem); ++added; seen.Add(full); }
                catch (Exception ex) { gv.debug.w("BuildMatchingImagesList: addItem failed", ex.Message); }
            }

            int count = 0;
            try { count = gv.imageFileListMatches.getImageFileListLength(); }
            catch
            {
                try { count = gv.imageFileListMatches.getImageCount(); }
                catch { try { count = gv.imageFileListMatches.finfoList?.Count ?? added; } catch { count = added; } }
            }

            try { _main.SetImageCountText(count.ToString("N0")); } catch { }
            gv.debug.w($"BuildMatchingImagesList: added {added}, total {count}");
            return count;
        }

        private void CopyImageFileList(ImageFileList src, ImageFileList dst)
        {
            if (src == null || dst == null) return;
            try { dst.clearList(); } catch { }
            try
            {
                var items = src.getFinfo();
                if (items != null) { foreach (var fi in items) dst.addItem(fi); return; }
            }
            catch { }
            try
            {
                int cnt = src.getImageFileListLength();
                for (int i = 0; i < cnt; i++) dst.addItem(src.getIndexed(i));
            }
            catch { }
        }

        private void PrependMatchesAndEnsureMetadata(List<string> matches)
        {
            if (matches == null || matches.Count == 0) return;
            try
            {
                if (gv.imageFileList1 == null) gv.imageFileList1 = new ImageFileList();
            }
            catch { return; }

            var existing = gv.imageFileList1.finfoList ?? new List<FileInfoItem>();
            var existingPaths = new HashSet<string>(
                existing.Where(f => !string.IsNullOrEmpty(f?.fpath))
                        .Select(f => { try { return Path.GetFullPath(f.fpath); } catch { return f.fpath ?? ""; } }),
                StringComparer.OrdinalIgnoreCase);

            var prepended = new List<FileInfoItem>(matches.Count);
            foreach (var m in matches)
            {
                if (string.IsNullOrWhiteSpace(m)) continue;
                string full;
                try { full = Path.GetFullPath(m); } catch { full = m; }
                if (existingPaths.Contains(full)) continue;

                FileInfoItem dbItem = null;
                try
                {
                    var repo = _main.Repo;
                    var rt = repo.GetType();
                    var byFpath = rt.GetMethod("GetByFpath",
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic);
                    if (byFpath != null)
                    {
                        var res = byFpath.Invoke(repo, new object[] { full });
                        if (res is FileInfoItem fii) dbItem = fii;
                    }
                }
                catch { dbItem = null; }

                if (dbItem != null)
                {
                    dbItem.fpath = full;
                    prepended.Add(dbItem);
                    existingPaths.Add(full);
                    continue;
                }

                var ni = new FileInfoItem { fpath = full };
                try
                {
                    var fi = new FileInfo(full);
                    ni.fname = Path.GetFileName(full);
                    ni.dpath = Path.GetDirectoryName(full) ?? "";
                    ni.ext = Path.GetExtension(full) ?? "";
                    ni.len = fi.Exists ? fi.Length : 0;
                    ni.stimestamp = fi.Exists ? fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") : "";
                }
                catch { }
                prepended.Add(ni);
                existingPaths.Add(full);
            }

            if (prepended.Count == 0) return;
            try { gv.imageFileList1.finfoList?.InsertRange(0, prepended); } catch { }

            try { gv.slideCount1 = gv.imageFileList1.getImageFileListLength(); }
            catch
            {
                try { gv.slideCount1 = gv.imageFileList1.getImageCount(); }
                catch { gv.slideCount1 = gv.imageFileList1.finfoList?.Count ?? 0; }
            }

            try { _main.SetMaxSlideText($"{gv.slideCount1 - 1}"); } catch { }
            try { _main.SetImageCountText(gv.slideCount1.ToString("N0")); } catch { }
        }

        // ── image load helper ────────────────────────────────────────────────
        private static Image LoadImageNoLock(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var img = Image.FromStream(fs);
            return new Bitmap(img);
        }
    }
}