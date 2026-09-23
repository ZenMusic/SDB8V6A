using System;
using System.Windows.Forms;

namespace SymbolDB
{
    // Single canonical location for the UI update helper to avoid duplicate partial-class definitions.
    public partial class DialogTraverser : Form
    {
        /// <summary>
        /// Safely update tbStatus.Text from any thread.
        /// Keep this method in exactly one partial-class file (this file).
        /// Remove any other copies from TraverserBG*.cs files.
        /// </summary>
        public void UpdateStatusRowCount(int count)
        {
            // If the form is already being disposed or the handle isn't created, ignore the update.
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateStatusRowCount), count);
                return;
            }

            try
            {
                tbStatus.Text = count.ToString();
            }
            catch
            {
                // Swallow to remain tolerant of designer/control changes.
            }
        }
    }
}