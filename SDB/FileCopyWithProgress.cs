using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SymbolDB
{
    public class FileCopyWithProgress
    {
        // Define the callback delegate.
        public delegate CopyProgressResult CopyProgressRoutine(
            long totalFileSize,
            long totalBytesTransferred,
            long streamSize,
            long streamBytesTransferred,
            uint dwStreamNumber,
            uint dwCallbackReason,
            IntPtr hSourceFile,
            IntPtr hDestinationFile,
            IntPtr lpData);

        // Define the callback result values.
        public enum CopyProgressResult : uint
        {
            PROGRESS_CONTINUE = 0,
            PROGRESS_CANCEL = 1,
            PROGRESS_STOP = 2,
            PROGRESS_QUIET = 3
        }

        // Import the CopyFileEx function from kernel32.dll.
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CopyFileEx(
            string lpExistingFileName,
            string lpNewFileName,
            CopyProgressRoutine lpProgressRoutine,
            IntPtr lpData,
            ref bool pbCancel,
            int dwCopyFlags);

        // Method to copy file with a progress callback.
        public static void CopyFileWithProgressBar(string sourceFile, string destinationFile, ProgressBar progressBar)
        {
            bool cancel = false;

            // Initialize the progress bar.
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            // Create the progress callback delegate.
            CopyProgressRoutine progressCallback = new CopyProgressRoutine(
                (totalFileSize, totalBytesTransferred, streamSize, streamBytesTransferred, dwStreamNumber, dwCallbackReason, hSourceFile, hDestinationFile, lpData) =>
                {
                    if (totalFileSize > 0)
                    {
                        int progressPercentage = (int)((totalBytesTransferred * 100) / totalFileSize);

                        // Update the progress bar on the UI thread.
                        if (progressBar.InvokeRequired)
                        {
                            progressBar.Invoke(new Action(() => progressBar.Value = progressPercentage));
                        }
                        else
                        {
                            progressBar.Value = progressPercentage;
                        }
                    }
                    return CopyProgressResult.PROGRESS_CONTINUE;
                });

            // Call CopyFileEx with the progress callback.
            bool result = CopyFileEx(sourceFile, destinationFile, progressCallback, IntPtr.Zero, ref cancel, 0);
            if (!result)
            {
                int error = Marshal.GetLastWin32Error();
                MessageBox.Show("File copy failed with error code: " + error);
            }
        }
    }
}
