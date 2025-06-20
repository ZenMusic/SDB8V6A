using System;
using System.IO;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public class Watcher
    {
        GlobalVars gv;
        Display1 disp;
        Main dispMain;
        // static DebugWindow myDebug;
        // Create a new FileSystemWatcher and set its properties.
        FileSystemWatcher watcher = new FileSystemWatcher();
        //[PermissionSet(SecurityAction.Demand, Name="FullTrust")]



        MessageWindow msg;
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public Watcher(GlobalVars g, string sdir, Display1 d)
        {

            gv = g;
            //msg = new MessageWindow(gv);
            //msg.Activate();
            //msg.Show();
            //  myDebug = new DebugWindow(g);
            //   myDebug.Show();
            //string fdir = @"C:\Users\david\Documents\111";
            // string readDir = @"C:\share\2\watcher";


            //"C:\Users\david.mcclanahan\Documents\_dmc";

            if (sdir == null)
                sdir = gv.watchFolderPath;

            disp = d;
            if (disp == null)
                dispMain = gv.mainWindow;
            //  myDebug = d;

            // Create a new FileSystemWatcher and set its properties.
            ////// FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = sdir;
            /* Watch for changes in LastAccess and LastWrite times, and
               the renaming of files or directories. */
            /*
             * watcher.NotifyFilter = 
             *       NotifyFilters.LastAccess
                   | NotifyFilters.LastWrite
                   | NotifyFilters.FileName
                   | NotifyFilters.DirectoryName;
             * */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = "*.*";

            // Add event handlers.
            //  watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            //   watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            watcher.Error += new ErrorEventHandler(OnError);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
            // gv.debug.w("watching ", sdir);

            // Console.WriteLine("Press \'q\' to quit the sample.");
            // while (Console.Read() != 'q') ;


        }
        public void showThisImage(string spath)
        {
            //gv.debug.displayWatched(spath);
            // disp.displayThisImage1(spath);

            spath = spath.Replace("\\", "/");
            //MessageBox.Show(spath);
            if (!File.Exists(spath))
                return;
            try
            {
                //GDI generic error may need to copy the image first
                //pictureBox1.Image = Image.FromFile("C:\\test\\test1.jpg");
                if (disp == null)
                    dispMain.displayThisImage1(spath);
                else
                    disp.displayThisImage1(spath);
                //disp.LoadPbFromStream(spath);
            }

            catch (Exception ex)
            {

            }
            //disp.showMessage(spath);
            //gv.mainWindow.displayThisImage();
        }
        private static void OnError(object source, ErrorEventArgs e)
        {
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                //  This can happen if Windows is reporting many file system events quickly 
                //  and internal buffer of the  FileSystemWatcher is not large enough to handle this
                //  rate of events. The InternalBufferOverflowException error informs the application
                //  that some of the file system events are being lost.
                Console.WriteLine(("The file system watcher experienced an internal buffer overflow: " + e.GetException().Message));
                string text = e.GetException().Message;
                MessageBox.Show("Error ", e.GetException().Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();

            //   LogEvent("Monitoring Stopped");
        }

        // Define the event handlers.  static

        bool bShowTitle = true;
        public void showMessage(string txt)
        {

        }

        private void OnCreated(object sourece, FileSystemEventArgs e)
        {
            ///msg.messageUpdate(e.Name, null);
            showMessage(e.Name);
            showThisImage(e.FullPath);
            //    form.Hide();
            //  Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            // MessageBox.Show("OnCreate trigger ", e.Name.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            //              MessageBox.Show("File: " + e.FullPath + " " + e.ChangeType);
            //   Type.GetType(String).GetMethod(showThisImage).Invoke(null, e.FullPath);
            // myDebug.displayWatched(e.FullPath);
            showMessage(e.Name);
            showThisImage(e.FullPath);
            //     form.Hide();
            //  Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            //     Watcher w = new Watcher(gv, null, myDebug);
            //    w.showThisImage(e.FullPath);
            // myDebug.w(e.FullPath);
            //   gv.debug.display1.displayThisImage1(e.FullPath);
            //    gv.debug.display1.displayThisImage2(e.FullPath);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            //                MessageBox.Show("File: " + e.OldFullPath + " renamed to " + e.FullPath);
            //   Type.GetType(String).GetMethod(showThisImage).Invoke(null, e.FullPath);
            //gv.debug.displayThisImage(e.FullPath);
            //  Watcher w = new Watcher(gv, null, myDebug);
            showThisImage(e.FullPath);
            //  w.showThisImage(e.FullPath);
            //                myDebug.displayWatched(e.FullPath);
            //                gv.debug.displayWatched(e.FullPath);
            //  gv.debug.display1.displayThisImage1(e.FullPath);
            //   gv.debug.display1.displayThisImage2(e.FullPath);
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


