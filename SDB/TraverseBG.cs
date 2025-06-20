using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Threading;


using System.ComponentModel;
using System.Reflection;

namespace SymbolDB
{
    class TraverserBG
    {
        GlobalVars gv;
        static TraverserDialog formTraverser2;
       // string path2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;

        const int HowDeepToScan = 999;
        //const int MaxFileCount = 99999;  /// 99,999
                                           /// 
        ///// traverserDialog  const int MaxListViewRows = 9999;
        
        private bool bShowProgress = false;

        // traverses a directory, and lists the contents recursively
        //Int Parameter = max depth to scan (optional)

        public TraverserBG(GlobalVars g, TraverserDialog list)
        {
            gv = g;
            formTraverser2 = list;
            gv.slideCount0 = 0;
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
          //  
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);

            bShowProgress = gv.bShowProgress;
            backgroundWorker1.WorkerReportsProgress = bShowProgress;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }


        public void doTraverseFolders(ARGS args)// string path)
        {
            gv.setCursorHourGlass();
            gv.bImageFileList1Loaded = true;
            backgroundWorker1.RunWorkerAsync(args); // do this work Async -----------  >>  backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)        
        }
        /// /////////////////////////////////////// WORKER //////////////////////// RunWorkerAsync 
        int row = 0;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs eventArgs)       
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            gv.setCursorHourGlass();

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.

            //e.Result = traverseStart((string)e.Argument);

            ARGS args = (ARGS)eventArgs.Argument;
            string path = args.dirpath;

            string test1 = "C:/temp";
            if (path == null || path.Length == 0)
            {
                path = test1;
                gv.message("Using default path to a directory to list");
            }
            if (!Directory.Exists(path))
            {
                MessageBox.Show(path + " does not exist");
            }
            //doAsyncWork(Path);
            string result = "test";
            string[] ImageExtentions = new string[] { "*.jpg", "*.png" , "*.bmp", "*.jpeg" }; //, "*.gif" does not work  "*.mpg"
            string[] MovieExtentions =   new string[] { "*.mp4", "*.wmv", "*.mov", "*.webp" , "*.avif"}; // mkv
            string[] MovieWEBM = new string[] { "*.mp4", "*.webm", "*.wmv", "*.mov", "*.webp", "*.avif"};
            string[] MIDI = new string[] { "*.mid", "*.mpe" };
            string[] ALL = new string[] { "*.*", "*.*" };
            string[] Allext = new string[] { "*.*", "*.bmp" };
            if (args.bMovies)
            {
                
                if (!args.bPictures)
                    ImageExtentions = MovieExtentions;
                if (args.bWEBM)
                    ImageExtentions = MovieWEBM;
            } else if (args.bMIDI)
            {
                ImageExtentions = MIDI;
            }
            else if (args.bALL)
            {
                ImageExtentions = ALL;
            }
            // fextention = Mextention;
            //ImageExtentions = new string[] { "webp" };
            //string[] fextention2 = new string[] { "*.bmp", "*.png" };//, "*.pgn" };
            foreach (string ext in ImageExtentions) //or MOVIE EXtensions
            {
                result = traverseFolders(path, 0 , ext);
                if (gv.bFindFileName)
                    break;

            }
            if (result == null)
            {
                eventArgs.Cancel = true;
                return;
            }
            if (result.Equals("cancel"))
            {
                eventArgs.Cancel = true;
            }
            else
            {
                eventArgs.Result = result;
            }

        }
        bool flag = false;
        public string traverseFolders(string path, int level, string fextention) //-------------------------main -------------------
        {
            string[] dirs = null;
            string[] files = new string[] { "test.pgn" };
            ArrayList allFiles = new ArrayList();
            //e.Result = return path;
            try
            {
                dirs = Directory.GetDirectories(path);
            }
            catch (Exception)
            {

                gv.debug.w("error could not access ", path);
                return null;
            }
            //string[] fextention = new string[] { "*.jpg" };//, "*.pgn" };
            //fextention = new string[] { "*.jpg", "*.png" };//, "*.pgn" };
            int fileTypes = fextention.Length;
            string fextension2 = fextention;

            try
            {
                if (gv.bFindFileName)
                {
                    fextention = gv.findFileName + "*.*";
                    fextension2 = fextention;
                }
                files = Directory.GetFiles(path, fextension2);
            }
            catch (Exception)
            {

                //no directories /// or no ACCESS ...
                gv.debug.w("error could not access files ", path);
                return null;
            }
            //allFiles.AddRange(Directory.GetFiles(path, fext));

            if (files == null)
                return null;
            //string[] files = Directory.GetFiles(path);

            ////////---------------------Traverser

            // for filtered search use the following
            // such as "*.jpg"
            //or ....   ("*"+search+"*",SearchOption.AllDirectories);
            //string [] fileEntries = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
            SortedList all = new SortedList();
            String indent = new String(' ', level * 2);

            // Add all the directories to the list
            if (dirs != null)
                for (int i = 0; i < dirs.Length; i++)
                {
                    all[dirs[i]] = "d";
                }

            // Add all the files to the list
            int filesCount = files.Length;
            if (files.Length > gv.iMaxFileCount)
                filesCount = gv.iMaxFileCount;
            if (files != null)
                for (int i = 0; i < filesCount; i++)
                {
                    all[files[i]] = "f";
                }
            // For each item in the directory, display it and possibly recurse
            foreach (string key in all.Keys)
            {
                // Do not iterate through reparse points  
                /*
                 * if ((File.GetAttributes(subdir) &
                     FileAttributes.ReparsePoint) !=
                         FileAttributes.ReparsePoint)
                   {

                    ProcessDir(subdir, recursionLvl + 1);
                   }
                 */
                // key = fpath
                if ((string)all[key] == "f")
                {
                    if (gv.bLoadingTraverser2)
                    {
                        row = gv.imageFileList2.addItem((string)key, (string)all[key], level);
                        gv.slideCount2++;
                    }
                    else
                    {
                        row = gv.imageFileList1.addItem((string)key, (string)all[key], level);
                     //   if (fextension2.Equals("webp") || fextension2.Equals("WEBP"))
                         //   flag = true;
                        gv.slideCount1++;
                    }
                    gv.slideCount0++;
                }
                if ((string)all[key] == "d")
                {
                    if (!backgroundWorker1.CancellationPending)
                        traverseFolders(key, level + 1, fextention);
                    else
                    {
                        return "cancel";
                    }
                }
                if (gv.slideCount0 > gv.iMaxFileCount)
                    return "maxfiles";
            }
            if (bShowProgress) backgroundWorker1.ReportProgress(gv.slideCount0 / 1000, DateTime.Now);///////////// report progress
            return path;
        }//-----------------end Traverser----------

   
        /// /////////////////////////////////////// COMPLETED ////////////////////////

        // This event handler deals with the results of the
        // background operation.
        private void backgroundWorker1_RunWorkerCompleted
            (object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (false) //e.Cancelled)
            {
                // Next, handle the case where the user canceled 
                // the operation.
                // Note that due to a race condition in 
                // the DoWork event handler, the Cancelled
                // flag may not have been set, even though
                // CancelAsync was called.
                gv.traverserStatus = "Canceled";
                MessageBox.Show("cancelled");
               
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
                //MessageBox.Show("done");
                gv.traverserStatus = e.Result.ToString();
                formTraverser2.OnWorkCompleted(sender, e);
            }
            
            
        }
        /// ///////////////////////////////////////PROGRESS CHANGED ////////////////////////
        // This event handler updates the progress bar.
        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            //this.progressBar1.Value = e.ProgressPercentage;
            formTraverser2.OnProgressChanged(sender, e);
        }
        /// ///////////////////////////////////////end ////////////////////////
        public int getResultList()
        {
            return gv.slideCount0;
        }
        

       

        public void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                formTraverser2.OnWorkCompleted(sender, e);
            }
            
        }

        public void cancel()
        {
            backgroundWorker1.CancelAsync();
            formTraverser2.OnCancel("cancel");
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            formTraverser2.OnProgressChanged(sender, e);
        }
    }//Traverser
}


