using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class DialogTraverser : Form
    {
        /// <summary>
        /// Called by TraverserBG5.ProgressChanged when we send row counts in UserState.
        /// Updates tbStatus.Text safely on the UI thread.
        /// </summary>
        public void UpdateStatusRowCount5(int count)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateStatusRowCount5), count);
                return;
            }

            try
            {
                // tbStatus is on this form's Designer.
                tbStatus.Text = count.ToString();
            }
            catch
            {
                // Ignore if control missing / renamed.
            }
        }
        
        /// <summary>
        /// Called by TraverserBG5.RunWorkerCompleted to add all scan results
        /// (files + levels) to imageFileList1 or imageFileList2 in one batch,
        /// avoiding per-row UI work and per-row helper calls.
        /// </summary>
        public void ApplyScanResults(
            List<TraverserBG5.FileResult> results,
            bool useList2)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<List<TraverserBG5.FileResult>, bool>(
                    ApplyScanResults), results, useList2);
                return;
            }

            if (results == null || results.Count == 0)
                return;

            // Fast path: use our bulk add on the ImageFileList wrapper
            if (useList2)
            {
                // false = skip expensive FileInfo length/timestamp reads
                gv.imageFileList2.AddResultsFast(results, includeFileMetadata: false);
            }
            else
            {
                // In DialogTraverser.ApplyScanResults
                gv.imageFileList1.AddResultsFast(results, includeFileMetadata: false);
            }

            // If you’re binding a DataGridView via a BindingSource to finfoList,
            // this is the place to trigger a single refresh, e.g.:
            //
            //   bindingSource1.ResetBindings(false);
            //
            // so the UI updates once instead of per-row.
        }
        //   scan results (files + levels) to imageFileList1 or imageFileList2 in one batch,    
        public void ApplyScanResults1(
            List<TraverserBG1.FileResult> results,
            bool useList2)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<List<TraverserBG1.FileResult>, bool>(
                    ApplyScanResults1), results, useList2);
                return;
            }

            if (results == null || results.Count == 0)
                return;

            // Fast path: use our bulk add on the ImageFileList wrapper
            if (useList2)
            {
                // false = skip expensive FileInfo length/timestamp reads
                gv.imageFileList2.AddResultsFast1(results, includeFileMetadata: false);
            }
            else
            {
                // In DialogTraverser.ApplyScanResults
                gv.imageFileList1.AddResultsFast1(results, includeFileMetadata: false);
            }

            // If you’re binding a DataGridView via a BindingSource to finfoList,
            // this is the place to trigger a single refresh, e.g.:
            //
            //   bindingSource1.ResetBindings(false);
            //
            // so the UI updates once instead of per-row.
        }

    }
}
