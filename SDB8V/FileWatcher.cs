using System;
using System.IO;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace SymbolDB
{
    public class Watcher
    {
        GlobalVars gv;
        Display1 disp;
        Main dispMain;
        // Create a new FileSystemWatcher and set its properties.
        FileSystemWatcher watcher = new FileSystemWatcher();

        MessageWindow msg;
        public Watcher(GlobalVars g, string sdir, Display1 d)
        {
            gv = g;

            if (sdir == null)
                sdir = gv.watchFolderPath;

            disp = d;
            if (disp == null)
                dispMain = gv.mainWindow;

            watcher.Path = sdir;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";

            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Error += new ErrorEventHandler(OnError);

            watcher.EnableRaisingEvents = true;
        }

        public void showThisImage(string spath)
        {
            spath = spath.Replace("\\", "/");
            if (!File.Exists(spath))
                return;
            try
            {
                if (disp == null)
                    dispMain.displayThisImage1(spath);
                else
                    disp.displayThisImage1(spath);
            }
            catch (Exception)
            {
                // swallow - caller logs when needed
            }
        }

        private static void OnError(object source, ErrorEventArgs e)
        {
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                Console.WriteLine(("The file system watcher experienced an internal buffer overflow: " + e.GetException().Message));
                MessageBox.Show("Error ", e.GetException().Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void OnStop()
        {
            try
            {
                watcher.EnableRaisingEvents = false;
            }
            catch { }

            try
            {
                watcher.Dispose();
            }
            catch { }
        }

        bool bShowTitle = true;
        public void showMessage(string txt)
        {

        }

        // Helper: wait until file size is stable and can be opened as an Image.
        private async Task<bool> WaitForFileReadyAsync(string path, int attempts = 10, int delayMs = 300)
        {
            for (int i = 0; i < attempts; ++i)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        await Task.Delay(delayMs);
                        continue;
                    }

                    var fi1 = new FileInfo(path);
                    long size1 = fi1.Length;
                    if (size1 == 0)
                    {
                        await Task.Delay(delayMs);
                        continue;
                    }

                    await Task.Delay(delayMs);

                    var fi2 = new FileInfo(path);
                    long size2 = fi2.Length;

                    // size stable -> attempt to open
                    if (size1 == size2)
                    {
                        try
                        {
                            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                // Try to create an Image from stream to ensure header is complete.
                                using var img = System.Drawing.Image.FromStream(fs, true, true);
                                return true;
                            }
                        }
                        catch (Exception)
                        {
                            // may still be writing; wait and retry
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore and retry
                }
                await Task.Delay(delayMs);
            }

            // final attempt: try to open once more before giving up
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using var img = System.Drawing.Image.FromStream(fs, true, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                gv?.debug?.w($"WaitForFileReadyAsync failed for {path}: {ex.Message}");
                return false;
            }
        }

        // OnCreated now: wait for the file to become stable then load a fresh Bitmap
        // and assign it to the UI on the UI thread. This prevents using an Image
        // that was created from a disposed stream or a partially-written file.
        private void OnCreated(object source, FileSystemEventArgs e)
        {
            // Run asynchronously so we don't block the FileSystemWatcher thread.
            _ = Task.Run(async () =>
            {
                string fullPath = e.FullPath;
                try
                {
                    // Wait until file is stable and openable as an image
                    bool ready = await WaitForFileReadyAsync(fullPath, attempts: 12, delayMs: 250);
                    if (!ready)
                    {
                        gv?.debug?.w($"File watch: file not stable after retries: {fullPath}");
                        // continue anyway and attempt to load once
                    }

                    // Attempt to load a copy of the image into a Bitmap (so the stream can close)
                    Bitmap loadedBmp = null;
                    try
                    {
                        using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using var tmp = System.Drawing.Image.FromStream(fs, true, true);
                            // clone into a new Bitmap instance that owns its data
                            loadedBmp = new Bitmap(tmp);
                        }
                    }
                    catch (Exception ex)
                    {
                        gv?.debug?.w($"Failed to load image for display: {fullPath}  {ex.Message}");
                        loadedBmp = null;
                    }

                    if (loadedBmp != null)
                    {
                        // Assign on the appropriate UI thread. Display1 and Main are both Controls/Forms.
                        if (disp != null && !disp.IsDisposed)
                        {
                            try
                            {
                                if (disp.InvokeRequired)
                                    disp.BeginInvoke(new Action(() => disp.displayThisImage(loadedBmp)));
                                else
                                    disp.displayThisImage(loadedBmp);
                            }
                            catch (Exception ex)
                            {
                                gv?.debug?.w($"Invoke to Display1 failed: {ex.Message}");
                                loadedBmp.Dispose();
                            }
                        }
                        else if (dispMain != null && !dispMain.IsDisposed)
                        {
                            try
                            {
                                if (dispMain.InvokeRequired)
                                    dispMain.BeginInvoke(new Action(() => dispMain.displayThisImage(loadedBmp)));
                                else
                                    dispMain.displayThisImage(loadedBmp);
                            }
                            catch (Exception ex)
                            {
                                gv?.debug?.w($"Invoke to Main failed: {ex.Message}");
                                loadedBmp.Dispose();
                            }
                        }
                        else
                        {
                            // No UI to show to — dispose the image.
                            loadedBmp.Dispose();
                        }
                    }
                    else
                    {
                        // fallback: call the path-based method (which attempts to open again on UI thread)
                        showThisImage(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    gv?.debug?.w($"OnCreated task failed for {fullPath}: {ex.Message}");
                }
            });
        }

        // Legacy / not used directly by OnCreated flow but kept for completeness.
        private void OnCreatedxxx(object sourece, FileSystemEventArgs e)
        {
            showMessage(e.Name);
            showThisImage(e.FullPath);
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            return;
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            return;
        }
    }
}
    /*

    The Filter property only supports one filter at a time. From the documentation:
    Use of multiple filters such as *.txt|*.doc is not supported.

    You need to create a FileSystemWatcher for each file type. You can then bind them all to the same set of event handlers:

    string[] filters = { "*.txt", "*.doc", "*.docx", "*.xls", "*.xlsx" };

    List<FileSystemWatcher> watchers = new List<FileSystemWatcher>;

    foreach(string f in filters)
    {
        FileSystemWatcher w = new FileSystemWatcher();
        w.Filter = f;
        w.Changed = MyChangedHandler;
        watchers.Add(w);
    } 



    */


