using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace SymbolDB
{
    /// <summary>
    /// High-performance folder traverser.
    /// - Single pass using Directory.EnumerateFiles with EnumerationOptions.
    /// - Skips system/temp/reparse folders.
    /// - Batches UI updates via DialogTraverser.ApplyScanResults.
    /// </summary>
    public sealed class TraverserBG1
    {
        private readonly GlobalVars _gv;
        private readonly DialogTraverser _dialog;
        private readonly BackgroundWorker _worker;
        private readonly List<FileResult> _results = new List<FileResult>(8192);

        private bool _showProgress;

        private bool bAlsoLoadMetadata = false;
        public void EnableMetadataLoading(bool enable) => bAlsoLoadMetadata = enable;


        private sealed class TraversalResult
        {
            public string FinalStatus { get; }
            public string[] Extensions { get; }

            public TraversalResult(string finalStatus, string[] extensions)
            {
                FinalStatus = finalStatus;
                Extensions = extensions;
            }
        }

        /// <summary>
        /// Simple record for each file discovered.
        /// </summary>
        public sealed class FileResult
        {
            public string Path { get; }
            public string Kind { get; }
            public int Level { get; }

            // NEW: optional metadata
            public long? Length { get; }
            public DateTime? LastWriteTime { get; }

            public FileResult(string path, string kind, int level,
                              long? length = null,
                              DateTime? lastWrite = null)
            {
                Path = path;
                Kind = kind;
                Level = level;
                Length = length;
                LastWriteTime = lastWrite;
            }
        }


        /// <summary>
        /// Extension / filename filter.
        /// </summary>
        private sealed class ExtensionFilter
        {
            public bool MatchAll { get; }
            public HashSet<string> Extensions { get; }
            public bool HtmlWildcardHtmStar { get; }
            public bool FindByName { get; }
            public string NamePrefix { get; }

            public ExtensionFilter(
                bool matchAll,
                HashSet<string> extensions,
                bool htmlWildcardHtmStar,
                bool findByName,
                string namePrefix)
            {
                MatchAll = matchAll;
                Extensions = extensions;
                HtmlWildcardHtmStar = htmlWildcardHtmStar;
                FindByName = findByName;
                NamePrefix = namePrefix ?? string.Empty;
            }

            public bool IsMatch(string filePath)
            {
                // Name-based search overrides extension filters.
                if (FindByName)
                {
                    string name = Path.GetFileName(filePath);
                    if (string.IsNullOrEmpty(name)) return false;
                    return name.StartsWith(NamePrefix, StringComparison.OrdinalIgnoreCase);
                }

                if (MatchAll)
                    return true;

                string ext = Path.GetExtension(filePath);
                if (string.IsNullOrEmpty(ext))
                    return false;

                if (HtmlWildcardHtmStar && ext.StartsWith(".htm", StringComparison.OrdinalIgnoreCase))
                    return true;

                return Extensions.Contains(ext);
            }
        }

        public TraverserBG1(GlobalVars gv, DialogTraverser dialog)
        {
            _gv = gv;
            _dialog = dialog;

            _gv.slideCount0 = 0;
            _gv.slideCount1 = 0;
            _gv.slideCount2 = 0;

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _worker.ProgressChanged += Worker_ProgressChanged;

            _showProgress = _gv.bShowProgress;
        }

        /// <summary>
        /// Called from DialogTraverser to start a new traversal.
        /// Example:
        ///   var traverser = new TraverserBG1(gv, this);
        ///   traverser.StartTraversal(args);
        /// </summary>
        public void StartTraversal(ARGS args)
        {
            _gv.setCursorHourGlass();
            _gv.bImageFileList1Loaded = true;
            _results.Clear();
            _worker.RunWorkerAsync(args);
        }

        public void Cancel()
        {
            _worker.CancelAsync();
            _dialog.OnCancel("cancel");
        }

        public int GetResultCount()
        {
            return _gv.slideCount0;
        }

        // ----------------- BackgroundWorker handlers -----------------

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            _gv.setCursorHourGlass();

            var args = (ARGS)e.Argument;

            string path = args.dirpath;
            if (string.IsNullOrWhiteSpace(path))
            {
                return; // path = "C:/temp";
                _gv.message("Using default path to a directory to list");
            }

            if (!Directory.Exists(path))
            {
                System.Windows.Forms.MessageBox.Show(path + " does not exist");
                e.Cancel = true;
                return;
            }

            string[] extensionPatterns = GetExtensionsForArgs(args);
            ExtensionFilter filter = BuildExtensionFilter(extensionPatterns);

            string status = TraverseSinglePass(path, worker, filter);

            if (status == null || status == "cancel")
            {
                e.Cancel = true;
                return;
            }

            e.Result = new TraversalResult(status, extensionPatterns);
        }
        
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Windows.Forms.MessageBox.Show(e.Error.Message);
                return;
            }

            if (e.Cancelled)
            {
                _gv.traverserStatus = "Canceled";
                _dialog.OnWorkCompleted(sender, e);
                return;
            }

            var result = e.Result as TraversalResult;
            if (result != null)
            {
                _gv.traverserStatus = result.FinalStatus;

                // Apply results to UI (tbStatus, grids, etc.) via DialogTraverser
                _dialog.ApplyScanResults1(_results, _gv.bLoadingTraverser2);

                // Preserve any existing behavior that shows which extensions were scanned
                _dialog.DisplayTraversalArgs(result.Extensions);
            }

            _dialog.OnWorkCompleted(sender, e);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _dialog.OnProgressChanged(sender, e);

            // When we report progress with row count in UserState, update tbStatus.
            if (e.UserState is int rowCount)
            {
                // With an explicit cast to resolve ambiguity:
                ((DialogTraverser)_dialog).UpdateStatusRowCount(rowCount);
            }
        }

        // ----------------- Traversal core -----------------

        private string TraverseSinglePass(
            string rootPath,
            BackgroundWorker worker,
            ExtensionFilter filter)
        {
            // Normalize rootPath once to avoid repeated work and to compute a stable base depth.
            char sep = Path.DirectorySeparatorChar;
            string rootFull;
            try
            {
                rootFull = Path.GetFullPath(rootPath).TrimEnd(sep);
            }
            catch
            {
                // If Path.GetFullPath fails for some reason, fall back to original value.
                rootFull = rootPath ?? string.Empty;
            }

            // Precompute the number of directory separators in the root path (base depth).
            int baseDepth = 0;
            foreach (char c in rootFull)
                if (c == sep) baseDepth++;

            var options = new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true,
                ReturnSpecialDirectories = false,
                AttributesToSkip =
                    FileAttributes.ReparsePoint |
                    FileAttributes.System |
                    FileAttributes.Temporary
                // add FileAttributes.Hidden if you want to skip hidden dirs too
                // | FileAttributes.Hidden
            };

            try
            {
                foreach (string file in Directory.EnumerateFiles(rootPath, "*", options))
                {
                    if (worker.CancellationPending)
                        return "cancel";

                    if (!filter.IsMatch(file))
                        continue;

                    string? dirName = Path.GetDirectoryName(file);
                    int level = CountDirectoryDepth(dirName ?? rootPath) - baseDepth;
                    if (level < 0) level = 0;

                    long? len = null;
                    DateTime? ts = null;
                    
                    if (bAlsoLoadMetadata)
                    {
                        try
                        {
                            var fi = new FileInfo(file);
                            len = fi.Length;
                            ts = fi.LastWriteTime;
                        }
                        catch
                        {
                            // Skip metadata on errors
                        }
                    }

                    _results.Add(new FileResult(file, "f", level, len, ts));


                    if (_gv.bLoadingTraverser2)
                        _gv.slideCount2++;
                    else
                        _gv.slideCount1++;

                    _gv.slideCount0++;

                    if (_gv.slideCount0 % 5000 == 0)
                    {
                        worker.ReportProgress(0, _gv.slideCount0);
                    }

                    if (_gv.slideCount0 > _gv.iMaxFileCount)
                        return "maxfiles";
                }
            }
            catch (Exception ex)
            {
                _gv.debug.w("error scanning ", rootPath + " : " + ex.Message);
                return null;
            }

            if (_showProgress)
            {
                worker.ReportProgress(_gv.slideCount0 / 1000, DateTime.Now);
            }

            return rootPath;
        }

        private static int CountDirectoryDepth(string path)
        {
            if (string.IsNullOrEmpty(path))
                return 0;

            char sep = Path.DirectorySeparatorChar;
            int count = 0;
            foreach (char c in path)
                if (c == sep) count++;
            return count;
        }

        // ----------------- Extension / ARGS helpers -----------------

        private string[] GetExtensionsForArgs(ARGS args)
        {
            string[] imageExtensions = new[] { "*.jpg", "*.png", "*.bmp", "*.jpeg" };
            string[] movieExtensions = new[] { "*.mp4", "*.wmv", "*.mov", "*.webp", "*.avif" };
            string[] movieWEBM = new[] { "*.mp4", "*.webm", "*.wmv", "*.mov", "*.webp", "*.avif" };
            string[] midi = new[] { "*.mid", "*.mpe" };
            string[] html = new[] { "*.htm*", "*.html", "*.mhtml" };
            string[] all = new[] { "*.*" };

            if (args.bMovies)
            {
                if (!args.bPictures)
                    imageExtensions = movieExtensions;
                if (args.bWEBM)
                    imageExtensions = movieWEBM;
            }
            else if (args.bMIDI)
            {
                imageExtensions = midi;
            }
            else if (args.bALL)
            {
                imageExtensions = all;
            }
            else if (args.bHTML)
            {
                imageExtensions = html;
            }

            return imageExtensions;
        }

        private ExtensionFilter BuildExtensionFilter(string[] patterns)
        {
            bool matchAll = false;
            bool htmlWildcard = false;
            var exts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (patterns != null)
            {
                foreach (string pattern in patterns)
                {
                    if (string.IsNullOrWhiteSpace(pattern))
                        continue;

                    string p = pattern.Trim();

                    if (p == "*.*" || p == "*")
                    {
                        matchAll = true;
                        break;
                    }

                    if (p.StartsWith("*.", StringComparison.Ordinal))
                    {
                        string extPattern = p.Substring(1); // ".jpg" or ".htm*"

                        if (extPattern.Equals(".*", StringComparison.Ordinal))
                        {
                            matchAll = true;
                            break;
                        }

                        if (extPattern.Equals(".htm*", StringComparison.OrdinalIgnoreCase))
                        {
                            htmlWildcard = true;
                            continue;
                        }

                        if (extPattern.EndsWith("*", StringComparison.Ordinal))
                        {
                            string clean = extPattern.TrimEnd('*');
                            if (!string.IsNullOrEmpty(clean))
                                exts.Add(clean);
                        }
                        else
                        {
                            exts.Add(extPattern);
                        }
                    }
                }
            }

            bool findByName = _gv.bFindFileName && !string.IsNullOrWhiteSpace(_gv.findFileName);
            string namePrefix = findByName ? _gv.findFileName : string.Empty;

            return new ExtensionFilter(matchAll, exts, htmlWildcard, findByName, namePrefix);
        }
    }
}
