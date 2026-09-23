using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class TraverserDialog5 : Form
    {
        private readonly GlobalVars gv;

        public TraverserDialog5(GlobalVars globalVars)
        {
            gv = globalVars ?? throw new ArgumentNullException(nameof(globalVars));
            InitializeComponent();
        }

        /// <summary>
        /// Called by TraverserBG5 when the worker reports a row-count update
        /// (every 5000 rows). You can show it in a label, the title bar, etc.
        /// </summary>
        public void UpdateStatusRowCount(int count)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateStatusRowCount), count);
                return;
            }

            // If you have a TextBox or Label, update it here.
            // Example: this.Text = $"Scanned: {count}";
            this.Text = $"Scanning... {count} files";
        }

        /// <summary>
        /// Bulk-apply scan results to imageFileList1/2 on the UI thread.
        /// </summary>
        public void ApplyScanResults(
            IEnumerable<TraverserBG5.FileResult> results,
            bool useList2)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IEnumerable<TraverserBG5.FileResult>, bool>(
                    ApplyScanResults), results, useList2);
                return;
            }

            foreach (var r in results)
            {
                if (useList2)
                {
                    gv.imageFileList2.addItem(r.Path, r.Kind, r.Level);
                }
                else
                {
                    gv.imageFileList1.addItem(r.Path, r.Kind, r.Level);
                }
            }
        }

        /// <summary>
        /// Optionally display which extensions were used in the scan.
        /// </summary>
        public void DisplayTraversalArgs(string[] extensions)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string[]>(DisplayTraversalArgs), extensions);
                return;
            }

            // You can show this in a TextBox or the form title.
            if (extensions != null && extensions.Length > 0)
            {
                string extSummary = string.Join(", ", extensions);
                this.Text = $"Scanning ({extSummary})";
            }
        }

        /// <summary>
        /// Called when the BackgroundWorker finishes or is cancelled.
        /// </summary>
        public void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, RunWorkerCompletedEventArgs>(
                    OnWorkCompleted), sender, e);
                return;
            }

            if (e.Cancelled)
            {
                this.Text = "Traversal cancelled.";
            }
            else
            {
                this.Text = "Traversal complete.";
            }

            // You can Close() here if this is a modal dialog
            // Close();
        }

        /// <summary>
        /// Called by TraverserBG5 when a cancel is requested.
        /// </summary>
        public void OnCancel(string reason)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(OnCancel), reason);
                return;
            }

            this.Text = "Cancelling traversal...";
        }

        /// <summary>
        /// Optional progress handling (if you later use percentage progress).
        /// </summary>
        public void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, ProgressChangedEventArgs>(
                    OnProgressChanged), sender, e);
                return;
            }

            // If you add a ProgressBar, you can update it here.
            // Example: progressBar1.Value = Math.Min(progressBar1.Maximum, e.ProgressPercentage);
        }
    }
}
