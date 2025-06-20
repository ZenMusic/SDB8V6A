using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolDB
{
    class FileFunctions2
    {
        GlobalVars gv;

        public FileFunctions2(GlobalVars g)
        {
            gv = g;
        }

        // Simple synchronous file copy operations with no user interface.
        // ported from the Java version

        public void fileCopy(string sourcePath, string targetPath)
        {
            string fileName = "test.txt";
            sourcePath = @"C:\Users\Public\TestFolder";
            targetPath = @"C:\Users\Public\TestFolder\SubDir";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }

            // Keep console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

      //` System.IO.File.Move(@"C:\From.txt", @"C:\TO.txt");



        // Simple synchronous file move operations with no user interface.
        public void FileMove(string sourceFile, string destinationFile)
        {

            sourceFile = @"C:\Users\Public\public\test.txt";
            destinationFile = @"C:\Users\Public\private\test.txt";

            // To move a file or folder to a new location:
            System.IO.File.Move(sourceFile, destinationFile);

            // To move an entire directory. To programmatically modify or combine
            // path strings, use the System.IO.Path class.
            System.IO.Directory.Move(@"C:\Users\Public\public\test\", @"C:\Users\Public\private");
        }




        // Simple synchronous file deletion operations with no user interface.
        // To run this sample, create the following files on your drive:
        // C:\Users\Public\DeleteTest\test1.txt
        // C:\Users\Public\DeleteTest\test2.txt
        // C:\Users\Public\DeleteTest\SubDir\test2.txt
        static int MAXFILES = 1000;
        string[] filesToDelete = new string[MAXFILES];
        static int idx = 0;
        public int deleteFile(string fpath)
        {
            int rc = 0;
            if (idx < MAXFILES - 1)
            {
                filesToDelete[idx] = fpath;
                ++idx;
                rc = 1;
            }
            return rc;
            
        }
        public void FileDelete(string fname)
    {
            // Delete a file by using File class static method...
            if (System.IO.File.Exists(fname))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    System.IO.File.Delete(fname);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }

            // ...or by using FileInfo instance method.
            //System.IO.FileInfo fi = new System.IO.FileInfo(@"C:\Users\Public\DeleteTest\test2.txt");
            //try
            //{
              //  fi.Delete();
      }
        public void folderDelete(string dname)
        {
            try
            {
                System.IO.Directory.Delete(dname);
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }
            // Delete a directory and all subdirectories with Directory static method...
            if (System.IO.Directory.Exists(dname))
            {
                try
                {
                    System.IO.Directory.Delete(dname);
                }

                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            // ...or with DirectoryInfo instance method.
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dname);
            // Delete this dir and all subdirs.
            try
            {
                di.Delete(true);
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }

        }


    }
}
