using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SymbolDB
{

    /// <summary>
    /// ///////////////////////////////////// MSDN   file functions ///////////////////////////////////////////////////
    /// </summary>
    /*
     * FileFunctions.java
     * Created on October 31, 2007, 11:57 AM
     * ported to C# 2010
     * @author mcclandx
     */
    // FileFunctions requires several Globals 
    //     gv.bRunningInDevelopment
    //     gv.dataFolder
    //     gv.dazenFolderName
    //     gv.inifileFullPathName
    //     gv.initParm1List
    //     gv.mainWindow

    public class FileFunctions
    {
        //public  FileFunctions fil;
        GlobalVars gv;
       //DebugWindow ffd;
        Boolean debug = false;
        string status;
        string dirRoot = "C:\\aImages\\";
        //string dir = "c";
        string lastFileName;
        string renameFileName;

        string lastDateGiven = "start";

        public int OVERWRITE_ALWAYS = 1;
        public int OVERWRITE_NEVER = 2;
        public int OVERWRITE_ASK = 3;
        // program options initialized to default values
        private int bufferSize = 4 * 1024;
        private Boolean clock = true;
        private Boolean copyOriginalTimestamp = true;
        private Boolean verify = true;
        private int overrideOption;
        private int dirBaseLength;
        long copyDirCount = 0;
        long copyFileCount = 0;

        public FileFunctions(GlobalVars g)
        {
            gv = g;
            status = "open";
            //ffd = gv.debug;
            if (debug)
            {
                //System.out.println("constructor FileFunctions");
            }
            gv.bRunningInDevelopment = System.Diagnostics.Debugger.IsAttached;
        }

        /// <summary>
        /// Sets up the application folders and inifileFullPathName based on whether
        /// the app is running in development (VS) or deployed.
        /// Dev:      MyDocuments\_dazen\{appName}\{initFileName}
        /// Deployed: AppData\Roaming\{appName}\Config\{initFileName}
        /// </summary>
        public void setFoldersForApp(bool bRunningInDevelopment)
        {
            string iniFolder;

            if (bRunningInDevelopment)
            {
                // e.g. C:\Users\davidmc\Documents\_dazen\SymbolDB8v6\
                iniFolder = Path.Combine(
                    GlobalVars.mydocsFolder,
                    GlobalVars.companyFolderName,
                    gv.appName);
            }
            else
            {
                // e.g. C:\Users\davidmc\AppData\Roaming\SymbolDB8v6\Config\
                iniFolder = Path.Combine(
                    GlobalVars.appDataFolder,
                    gv.appName,
                    "Config");
            }

            // Create the folder if it doesn't exist
            createDirectory(iniFolder);

            // Set the full path to the ini file on GlobalVars
            gv.inifileFullPathName = Path.Combine(iniFolder, GlobalVars.initFileName);
            gv.dataFolder = iniFolder; // Set the data folder to the ini folder for now

            System.Diagnostics.Debug.WriteLine("bRunningInDevelopment" + " " + bRunningInDevelopment.ToString());
            System.Diagnostics.Debug.WriteLine("inifileFullPathName" + " " + gv.inifileFullPathName);

        }

        /*
         *             string.
        rcLen = this.createDirectory(dirRoot + newDir );
        //System.out.printf("idx=%d newDir to create %s rcLen = %b\n", idx, newDir.tostring(), rcLen);
        ch1 = newDir.charAt(0);
        ch2 = ch1;
        newDir = newDir.replace(ch1, ++ch2); //old new
        //System.out.printf("%s %c  new newDir = %c\n", newDir.tostring(), ch1, ch2);
         */
        public DateTime ConvertStringToDateTime(string dtime)
        {
            DateTime test = DateTime.ParseExact(dtime, "yyyy-MM-dd HH:mm:fff", System.Globalization.CultureInfo.CurrentCulture); // 3F not 4
            return test;
        }
        //
        public string NormalizeFileFullName(string fileFullName)
        {
            if (string.IsNullOrEmpty(fileFullName))
            {
                throw new ArgumentException("The file full name cannot be null or empty.", nameof(fileFullName));
            }

            // Clean up potential issues BEFORE calling GetFullPath
            string cleanedPath = fileFullName;

            // Fix drive letter without backslash (e.g., "Y:" → "Y:\")
            if (cleanedPath.Length == 2 &&
                char.IsLetter(cleanedPath[0]) &&
                cleanedPath[1] == ':')
            {
                cleanedPath += "\\";
            }

            // Remove duplicate drive references (e.g., "Y:Y:\" → "Y:\")
            var driveMatch = System.Text.RegularExpressions.Regex.Match(cleanedPath, @"^([A-Za-z]:)\1");
            if (driveMatch.Success)
            {
                cleanedPath = cleanedPath.Substring(2); // Remove first "Y:"
            }

            // Now normalize
            string normalizedPath = System.IO.Path.GetFullPath(cleanedPath);

            // Handle drive root case: if it's just "X:\", don't trim the backslash
            if (normalizedPath.Length == 3 &&
                char.IsLetter(normalizedPath[0]) &&
                normalizedPath[1] == ':' &&
                normalizedPath[2] == '\\')
            {
                return normalizedPath;
            }

            // Otherwise, trim trailing separators
            normalizedPath = normalizedPath.TrimEnd(
                System.IO.Path.DirectorySeparatorChar,
                System.IO.Path.AltDirectorySeparatorChar);

            return normalizedPath;
        }
        //
        public Boolean verifyFileExists(string fn)
        {

            return File.Exists(fn);
        }

        public string getStatus()
        {
            return status;
        }

        public string getLastFileName()
        {
            return lastFileName;
        }
        public int GetSubfolders1()
        {
            int count = 0;
            try
            {
                string[] dirs = Directory.GetDirectories(@"c:\", "*", SearchOption.TopDirectoryOnly);
                // Console.WriteLine("The number of directories starting with p is {0}.", dirs.Length);
                foreach (string dir in dirs)
                {
                    //Console.WriteLine(dir);
                    ++count;
                }
            }
            catch (Exception e)
            {
                // Console.WriteLine("The process failed: {0}", e.ToString());
            }
            return count;
        }

        //MOVE with FILE handle    
        // "yyyy-MM-ddTHH:mm:ss"
        // SimpleDateFormat sdf = new SimpleDateFormat("yyMMddHHmmss");
        public string getTimeStamp() //and pause 1 
        {
            DateTime ts = DateTime.Now;//.ToString("yyyy-MM-ddTHH:mm:ss"); 
            string dateStr = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            do
            {
                ts = DateTime.Now;
                dateStr = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            } while (lastDateGiven.Equals(dateStr));



            lastDateGiven = dateStr;
            return dateStr;

        }

        public string getFileRename()
        {
            return renameFileName;
        }

        public string renameFile2(string hCurrentFile, string dir)
        {
            return renameFile2Timestamp(hCurrentFile, dir, null);
        }

        //MAIN RENAME/MOVE with FILE HANDLE    returns FILE or null
        /// <summary>
        ///    public string renameFile2Timestamp(string filepath, string TargetDir, string newName2)
        /// </summary>
        public string renameFile2Timestamp(string filepath, string TargetDir, string newName2)
        {
            Boolean rc = false;
            // File (or directory) to be moved is hCurrentFile
            // Destination directory
            string fname = filepath.ToString();
            int idx = fname.IndexOf('.');
            string fileExtension = fname.Substring(idx);
            string newName = getTimeStamp() + fileExtension;
            if (newName2 != null)
            {
                if (newName2.Length > 0)
                {
                    newName = newName2 + fileExtension;
                }
            }
            // Move currentFile to new directory

            renameFileName = newName;
            return renameFileName.Replace("\\", "/");
        }
        //MAIN RENAME/MOVE with FILE HANDLE    returns FILE or null
        //------------------end -------------------------------------

        /// <summary>
        ///    public void renameFile(string TargetDir, string newName)
        /// </summary>
        public bool renameFile(string TargetDir, string newName)
        {
            System.Diagnostics.Debug.WriteLine("RENAME/MOVE file:" + " " + TargetDir + " " + ">>>" + " " + newName);
            try
            {
                File.Move(TargetDir, newName);
            }
            catch (Exception)
            {

                return false;
            }
            return true;

        }
        public FileInfo getFileInfo(string fpath)
        {
            FileInfo f = new FileInfo(fpath);
            return f;
        }
        public string[] getDrivesLetters()
        {
            string[] drives = Directory.GetLogicalDrives();
            System.Diagnostics.Debug.WriteLine("---------- drives info -------------");
            foreach (string s in drives)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
            return drives;
        }
        public DriveInfo GetDriveInfo(string path)
        {
            DriveInfo dInfo = new DriveInfo(path);
            double gbdiv = 1048576 * 1024;////1024000000;
            //dInfo.TotalSize / gbdiv;

            return dInfo;
        }
        public DriveInfo[] GetAllDrivesInfo()
        {
            DriveInfo[] mydrives = DriveInfo.GetDrives();
            System.Diagnostics.Debug.WriteLine("---------- drives info -------------");
            System.Diagnostics.Debug.WriteLine(" ");
            // getDrivesLetters();
            double gbdiv = 1048576 * 1024;////1024000000;
            foreach (DriveInfo d in mydrives)
            {
                if (d.IsReady)
                {
                    System.Diagnostics.Debug.WriteLine(d.VolumeLabel + " " + "    " + " " + "(" + " " + d.Name + " " + ")");
                    System.Diagnostics.Debug.WriteLine((d.TotalFreeSpace / gbdiv).ToString("N0") + " " + "GB" + " " + "free of " + " " + (d.TotalSize / gbdiv).ToString("N0") + " " + "GB");
                    System.Diagnostics.Debug.WriteLine("-----");
                }
            }
            return mydrives;
        }
        public DirectoryInfo getDirInfo(string dirpath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirpath);
            return dir;
        }
        public string getLastUpdatedFile(string dirPath)
        {
            GetLastUpdatedFileInDirectory(getDirInfo(dirPath));
            if (!string.IsNullOrEmpty(lastUpdatedFilePath))
            {

                if (verifyFileExists(lastUpdatedFilePath))
                    return lastUpdatedFilePath;
            }
            return null;

        }
        //
        public string getFileNameFromPath(string fpath)
        {
            if (fpath == null)
                return null;
            string fname = Path.GetFileName(fpath);
            return fname;
        }
        public string getFolder(string fpath)
        {
            fpath = Path.GetDirectoryName(fpath);

            if (fpath == null)
                return null;
            if (fpath[fpath.Length - 1] != '/')
                fpath += "/";
            string DirName;
            if (fpath.Length > 4)
            {
                try
                {
                    DirName = System.IO.Directory.GetParent(fpath).Name;
                }
                catch
                {
                    DirName = fpath;
                }
            }
            else
            {
                DirName = fpath;
            }
            return DirName;
        }
        /// <summary>
        /// ///////////////////////////////////// MSDN   file functions ///////////////////////////////////////////////////
        /// </summary>
        public Boolean createDirectory(string sRoot, string path)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine("That path exists already.");
                    return true;
                }

                // Create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                System.Diagnostics.Debug.WriteLine("The directory was created successfully at " + " " + Directory.GetCreationTime(path).ToString());
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("The process failed: {0}" + " " + e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        ///    public Boolean directoryExists(string TargetDir)
        /// </summary>
        public Boolean directoryExists(string TargetDir)
        {
            return Directory.Exists(TargetDir);


        }
        /// <summary>
        ///    public void createDirectory(string TargetDPath)
        /// </summary>
        public void createDirectory(string TargetDPath)
        {
            if (!System.IO.Directory.Exists(TargetDPath))
            {
                System.IO.Directory.CreateDirectory(TargetDPath);
                System.Diagnostics.Debug.WriteLine("created target dir" + " " + TargetDPath);
            }
            else
                System.Diagnostics.Debug.WriteLine(TargetDPath + " " + " already existed");
        }
        /// <summary>
        ///    public void CopyFolderItems(string sourcePath, string targetPath)
        /// </summary>
        public void CopyFolderItems(string sourcePath, string targetPath)
        {
            string fileName, destFpath;

            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
                System.Diagnostics.Debug.WriteLine("created target dir" + " " + targetPath);
            }

            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    fileName = System.IO.Path.GetFileName(s);
                    destFpath = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Copy(s, destFpath, true);
                    System.Diagnostics.Debug.WriteLine("copy" + " " + Path.Combine(s, destFpath) + " " + destFpath);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Source path does not exist!");
            }

            // Keep console window open in debug mode.
            // System.Diagnostics.Debug.WriteLine("Press any key to exit.");
            //Console.ReadKey();
        }


        /// <summary>
        ///    public void MoveFile(string fname, string sourceDir, string targetDir)
        /// </summary>
        public void MoveFile(string fname, string sourceDir, string targetDir)
        {
            string sourceFile = Path.Combine(sourceDir, fname);
            string destinationFile = Path.Combine(targetDir, fname);

            // To move a file or folder to a new location:
            System.Diagnostics.Debug.WriteLine("move" + " " + sourceFile + " " + destinationFile);
            try
            {
                System.IO.File.Move(sourceFile, destinationFile);

            }
            catch (Exception)
            {

                System.Diagnostics.Debug.WriteLine("move FAILED" + " " + sourceFile + " " + destinationFile);
            }

        }

        /// <summary>
        ///    public void MoveFile(string sourceFile, string targetDir)
        /// </summary>
        public bool MoveFile(string sourceFile, string targetDir)
        {
            // string sourceFile = Path.Combine(sourceDir, fname);
            string destinationFile = Path.Combine(targetDir, Path.GetFileName(sourceFile));

            // To move a file or folder to a new location:
            System.Diagnostics.Debug.WriteLine("try to move:" + " " + sourceFile + " " + destinationFile);
            System.Diagnostics.Debug.WriteLine("move" + " " + sourceFile + " " + destinationFile);
            try
            {

                System.IO.File.Move(@sourceFile, @destinationFile);
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("move FAILED" + " " + sourceFile + " " + destinationFile + " " + e.Message + " " + e.ToString());
                //deleteFile(sourceFile);
                return false;
            }
            return true;

        }
        /// <summary>
        ///    public void MoveFile(string sourceFile, string targetDir)
        /// </summary>
        public bool MoveFileRename(string sourceFile, string targetDir, string fname)
        {
            // string sourceFile = Path.Combine(sourceDir, fname);
            string destinationFile = Path.Combine(targetDir, fname);

            // To move a file or folder to a new location:
            System.Diagnostics.Debug.WriteLine("try to move:" + " " + sourceFile + " " + destinationFile);
            System.Diagnostics.Debug.WriteLine("move" + " " + sourceFile + " " + destinationFile);
            try
            {
                System.IO.File.Move(@sourceFile, @destinationFile);
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("move FAILED" + " " + sourceFile + " " + destinationFile + " " + e.Message + " " + e.ToString());
                //deleteFile(sourceFile);
                return false;
            }
            return true;

        }
        public bool MoveFileFullPath(string sourceFile, string destinationFile)
        {
            // To move a file or folder to a new location:
            System.Diagnostics.Debug.WriteLine("try to move:" + " " + sourceFile + " " + destinationFile);
            System.Diagnostics.Debug.WriteLine("move" + " " + sourceFile + " " + destinationFile);
            try
            {
                System.IO.File.Move(@sourceFile, @destinationFile);
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("move FAILED" + " " + sourceFile + " " + destinationFile + " " + e.Message + " " + e.ToString());
                //deleteFile(sourceFile);
                return false;
            }
            return true;

        }

        /// <summary>
        ///    public void MoveFolder(string sourceDir, string targetDir)
        /// </summary>
        public void MoveFolder(string sourceDir, string targetDir)
        {

            // To move a file or folder to a new location:

            System.IO.Directory.Move(sourceDir, targetDir);
            System.Diagnostics.Debug.WriteLine("move FOLDER " + " " + sourceDir + " " + targetDir);

        }

        /// <summary>
        /// ///////////////////////////////////// MSDN   file functions ///////////////////////////////////////////////////
        /// </summary>
        /*
         * Recently I faced problem with File.Exist, I hate this function. After than I've used Fileinfo class Exist function then my program works correct.
         * */
        public void deleteFile(string fname)
        {
            try
            {
                File.Delete(fname);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION FF.deleteFile" + " " + fname);
            }
            Boolean rc = File.Exists(fname);
            if (rc)
                System.Diagnostics.Debug.WriteLine("file still exists");
            else
                System.Diagnostics.Debug.WriteLine("file deleted");
            return;
        }

        /// <summary>
        ///    public void FileDelete(string fpath)
        /// </summary>
        public bool FileDelete(string fpath)

        {
            // Delete a file by using File class static method...
            if (System.IO.File.Exists(fpath))
            {
                try
                {
                    System.IO.File.Delete(fpath);
                }
                catch (System.IO.IOException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    return false;
                }
                if (System.IO.File.Exists(fpath))
                    return false;
                System.Diagnostics.Debug.WriteLine("deleted file" + " " + fpath);
                System.Diagnostics.Debug.WriteLine("<<<<<<<<<<<<<<DELETED FILE>>>>>>>>>>>>>>" + " " + fpath);
                return true;
            }

            // ...or by using FileInfo instance method.
            System.IO.FileInfo fi = new System.IO.FileInfo(fpath);
            try
            {
                fi.Delete();
            }
            catch (System.IO.IOException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            // Delete a directory. Must be writable or empty.
            try
            {
                System.IO.Directory.Delete(fpath);
            }
            catch (System.IO.IOException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            // Delete a directory and all subdirectories with Directory static method...
            if (System.IO.Directory.Exists(fpath))
            {
                try
                {
                    System.IO.Directory.Delete(fpath, true);
                }

                catch (System.IO.IOException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            // ...or with DirectoryInfo instance method.
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(fpath);
            // Delete this dir and all subdirs.
            try
            {
                di.Delete(true);
            }
            catch (System.IO.IOException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return false;
        }


        //    string sourceDirectory = @"c:\sourceDirectory";
        //    string targetDirectory = @"c:\targetDirectory";
        //  SimpleCopyDirectory(sourceDirectory, targetDirectory);
        /// <summary>
        /// ///////////////////////////////////// MSDN   file functions ///////////////////////////////////////////////////
        /// </summary>
        /// 
        public string Normalize(string sdir)
        {
            string stext = sdir;
            {
                if (sdir.Contains("//"))
                {
                    stext = sdir.Replace("/", "\\");
                }
            }
            int ilen = stext.Length - 1;
            string slast = stext.Substring(ilen, 1);

            if (slast.Equals("/"))
            {
                return stext;
            }
            else
            {
                stext += "/";
            }
            return stext;
        }
        public string validateDirectoryFormat(string sdir)
        {
            string stext = sdir;

            if (sdir.Contains("\\"))
            {
                stext = sdir.Replace("\\", "/");
            }
            int ilen = stext.Length - 1;
            string slast = stext.Substring(ilen, 1);

            if (slast.Equals("/"))
            {
                return stext;
            }
            else
            {
                stext += "/";
            }
            return stext;
        }
        public void CopyDirectory(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        /// <summary>
        ///    public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        /// </summary>
        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                System.Diagnostics.Debug.WriteLine("Copying " + " " + target.FullName + " " + fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        //public void MoveFile(string fname, string sourceDir, string targetDir)
        /// <summary>
        /// ///////////////////////////////////// MSDN   file functions ///////////////////////////////////////////////////
        /// </summary>
        public bool CopyFile(String fpath, string targetPath2, bool displayErrorDialog = false) // FROM path/filename  >>>> to >>>> path2 
        {
            string fname = Path.GetFileName(fpath);

            // Copy the file.
            System.Diagnostics.Debug.WriteLine("try to copy " + " " + fpath + " " + Path.Combine(targetPath2, fname));
            bool test = true;
            bool f1 = false;
            f1 = verifyFileExists(fname);
            bool d1 = false;
            d1 = directoryExists(targetPath2);
            string targetFile = Path.Combine(targetPath2, fname);
            if (targetFile.Contains("100"))
                System.Diagnostics.Debug.WriteLine("target file" + " " + targetFile);
            else
                System.Diagnostics.Debug.WriteLine("target file" + " " + targetFile);
            System.Diagnostics.Debug.WriteLine("file tocopy" + " " + fpath);
            System.Diagnostics.Debug.WriteLine("target dir exists" + " " + d1.ToString());
            try
            {
                File.Copy(fpath, Path.Combine(targetPath2, fname));
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("File Copy Failed" + " " + e.Message + " " + e.ToString());
                System.Diagnostics.Debug.WriteLine("path, path2/fname" + " " + fpath + " " + Path.Combine(targetPath2, fname));
                if (displayErrorDialog) MessageBox.Show(e.Message, "copy file failed");
                return false;
            }
            System.Diagnostics.Debug.WriteLine("copied" + " " + fpath + " " + Path.Combine(targetPath2, fname));
            return true;
        }
        /// <summary>
        /// ///////////////////////////////////// MSDN   file functions ///////////////////////////////////////////////////
        /// </summary>
        public bool CopyFileRename(String path, string path2, string fname) // FROM path/filename  >>>> to >>>> path2 
        {
            try
            {
                // Copy the file.
                System.Diagnostics.Debug.WriteLine("try to copy " + " " + path + " " + Path.Combine(path2, fname));
                File.Copy(path, Path.Combine(path2, fname));
            }

            catch (Exception e)
            {
                MessageBox.Show("copy file failed");
                System.Diagnostics.Debug.WriteLine("Fil Copy Failed" + " " + e.Message + " " + e.ToString());
                return false;
            }
            System.Diagnostics.Debug.WriteLine("copied" + " " + path + " " + Path.Combine(path2, fname));
            return true;
        }
        /// <summary>
        ///    public void CopyFile(string fileName, string sourcePath, string targetPath)
        /// </summary>
        public void CopyFile(string fileName, string sourcePath, string targetPath)
        {
            bool btest = false;
            if (btest)
            {
                fileName = "test.txt";
                sourcePath = @"C:\aImages";
                targetPath = @"C:\aImages\test";
            }
            // Use Path class to manipulate file and directory paths.
            string sourceFpath = System.IO.Path.Combine(sourcePath, fileName);
            string destFpath = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
                System.Diagnostics.Debug.WriteLine("created target dir" + " " + targetPath);
            }

            try
            {
                System.IO.File.Copy(sourceFpath, destFpath, true);
            }
            catch (Exception)
            {

                MessageBox.Show("File Exception source: " + sourceFpath + " copy to " + destFpath);
            }

            System.Diagnostics.Debug.WriteLine("copy okay " + " " + sourceFpath + " " + destFpath);
        }

        //CopyFileArchive
        Boolean debug2 = false;
        public CopyInfo copyrc;

        public CopyInfo CopyFileArchive(String path, string path2) // FROM path/filename  >>>> to >>>> path2 
        {
            string fname = Path.GetFileName(path);

            if (!this.directoryExists(path2))
            {
                try
                {
                    this.createDirectory(path2);
                }
                catch (Exception)
                {

                    MessageBox.Show("ERROR: Unable to create folder " + path2);
                    return null;
                }
            }
            copyrc = new CopyInfo(path, path2, fname, false);
            try
            {
                try
                {
                    File.Copy(path, Path.Combine(path2, fname));
                }
                catch (Exception)
                {

                    MessageBox.Show("ERROR: unable to copy file: " + path + " to " + path2 + fname);
                    return null;
                }
                System.Diagnostics.Debug.WriteLine("copied" + " " + path + " " + Path.Combine(path2, fname));

                copyrc.success = true;
            }

            catch
            {
                string fpath2 = Path.Combine(path2, fname);
                if (debug2) System.Diagnostics.Debug.WriteLine("----try secocnd time to copy with OVERWRITE option " + " " + path + " " + Path.Combine(path2, fname));
                try
                {
                    File.Copy(path, fpath2, true);
                }
                catch (Exception)
                {

                    MessageBox.Show("ERROR could not copy file " + path + "  " + fpath2);
                    return null;
                }
                if (debug2) System.Diagnostics.Debug.WriteLine("----The second Copy operation succeeded, which was expected.");
            }
            return copyrc;
        }

        // Output will vary based on the contents of the source directory.------------------------------------------- END OF MSDN FILE FUNCTIONS


        private void saveJpeg(string path, Bitmap img, long quality)
        {
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = this.getEncoderInfo("image/jpeg");

            if (jpegCodec == null)
                return;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }

        private ImageCodecInfo getEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }


        private static Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        /// <summary>
        /// public long countFilesInFolder
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns> long = count of files in folder</returns>
        public long countFilesInFolder(string dirPath)
        {
            long dirSize = 0;
            long len = 0;
            int counter = 0;

            return counter;
        }

        SaveFileDialog saveFileDialog1 = new SaveFileDialog();

        string lastError = "none";


        // -------- INIT PARMS now in XML file format -----------------------------------
        //2019 
        public int readInitParms1File(string fname) ///, DialogParms parms)  //parms may be null
        {
            gv.initParm1List.Clear();

            int rc = DeserializeInitParms1FromXML(fname);

            System.Diagnostics.Debug.WriteLine("read program init (parm) file: " + " " + fname);
            if (rc < 1)
            {
                if (gv.initParm1List.Count < 1)
                    gv.initParm1List.Add(new InitParms1());
                MessageBox.Show("Initialization Error: Create a new SDB initialization parameter file. Use the InitParms editor from the main window " + fname);
                return -2;
            }

            if (gv.initParm1List.Count() < 1)
            {
                MessageBox.Show("unable to continue ERROR IF001");
                Environment.Exit(1);
            }
            return gv.initParm1List.Count();

        }
        public void InitializeSaveFileDialogXML()
        {
            if (this.saveFileDialog1 == null)
                this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            // Set the file dialog to filter for graphics files.
            this.saveFileDialog1.Filter =
                "XML Files (*.XML)|*.XML";

            this.saveFileDialog1.Title = "Save INIT PARMS List File as (.XML)";

        }
        //2019
        int LoadInitFileXMl(string fpath)
        {
            return DeserializeInitParms1FromXML(fpath);
        }
        //2019
        int DeserializeInitParms1FromXML(string fpath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<InitParms1>));

            string dir = Path.GetDirectoryName(fpath);
            if (!System.IO.Directory.Exists(dir))
            {
                MessageBox.Show("ERROR: Init Parm file directory does not exist");
                return -1;
            }

            if (!System.IO.File.Exists(fpath))
            {
                MessageBox.Show("ERROR: Init Parm file ! does not exist");
                return -1;
            }
            TextReader textReader = new StreamReader(fpath);
            List<InitParms1> initParms1List;
            try
            {
                initParms1List = (List<InitParms1>)deserializer.Deserialize(textReader);
            }
            catch (Exception ex)
            {
                textReader.Close();
                return 0;
            }
            gv.initParm1List = initParms1List;
            textReader.Close();

            return gv.initParm1List.Count();
        }

        public int saveInitParmsListToDefault(List<InitParms1> lparms)
        {
            TextWriter textWriter;
            bool bSuccess = true;
            string fpath = gv.inifileFullPathName;
            if (string.IsNullOrEmpty(fpath))
                fpath = gv.inifileFullPathName;

            XmlSerializer serializer = new XmlSerializer(typeof(List<InitParms1>));

            try
            {
                textWriter = new StreamWriter(fpath);
            }
            catch (Exception e)
            {
                return -1;
            }

            try
            {
                serializer.Serialize(textWriter, gv.initParm1List);
            }
            catch (Exception ex)
            {
                bSuccess = false;
                System.Diagnostics.Debug.WriteLine("Serialize Exception " + " " + ex.ToString() + " " + ex.Message);
            }
            textWriter.Close();
            if (!bSuccess)
                return 0;
            return gv.initParm1List.Count;
        }

        public bool saveInitParms1List(string fpath, List<InitParms1> plist) // XML save file 
        {
            if (fpath == null)
            {
                InitializeSaveFileDialogXML();
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    fpath = saveFileDialog1.FileName;
                }
            }
            if (fpath == null)
                return false;
            fpath = Path.ChangeExtension(fpath, "xml");

            if (gv.initParm1List == null || gv.initParm1List.Count < 1)
            {
                if (gv.initParm1List == null)
                {
                    gv.initParm1List = new List<InitParms1>();
                }
                InitParms1 temp = new InitParms1();

                gv.initParm1List.Add(temp);
            }
            if (String.IsNullOrEmpty(gv.initParm1List[0].targetDir1))
                gv.initParm1List[0].SetTargetDir1(gv.initParm1List[0].targetDir2);

            if (!string.IsNullOrEmpty(gv.lastTraversedFolder))
            {
                if (string.IsNullOrEmpty(gv.initParm1List[0].sourceDir1))
                    gv.initParm1List[0].sourceDir1 = gv.lastTraversedFolder;
            }

            gv.initParm1List[0].fpath = gv.inifileFullPathName;
            gv.initParm1List[0].dirPath = Path.GetDirectoryName(gv.inifileFullPathName);

            XmlSerializer serializer = new XmlSerializer(typeof(List<InitParms1>));
            TextWriter textWriter = new StreamWriter(fpath);

            try
            {
                serializer.Serialize(textWriter, gv.initParm1List);
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine("Serialize Exception " + " " + ex.ToString() + " " + ex.Message);
            }
            textWriter.Close();

            return true;
        }
        OpenFileDialog openFileDialog1;

        public void InitializeOpenFileDialogXML()
        {
            if (this.openFileDialog1 == null)
                this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "XML Files (*.XML)|*.XML";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = false;
            this.openFileDialog1.Title = "Select SDB Init Parms File (.XML)";

        }



        // ------------ end of INIT PARMS in XML file

        public DateTime getLastUpdateTime(string path)
        {
            DateTime dt = new DateTime(1990, 1, 1);

            try
            {
                if (File.Exists(path))
                {
                    dt = File.GetLastWriteTime(path);

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("FileFunctions getLastUpdateTime Failed" + " " + e.ToString());
            }
            return dt;
        }
        string lastUpdatedFilePath = "none";

        private List<FileInfo> GetLastUpdatedFileInDirectory(DirectoryInfo directoryInfo)
        {
            FileInfo[] files = directoryInfo.GetFiles();
            List<FileInfo> lastUpdatedFile = null;
            DateTime lastUpdate = new DateTime(2010, 1, 1);

            lastUpdatedFilePath = "none";
            foreach (FileInfo file in files)
            {
                if (file.LastWriteTime > lastUpdate)
                {
                    // lastUpdatedFile.Add(file);
                    lastUpdate = file.LastWriteTime;
                    lastUpdatedFilePath = file.FullName;
                    // System.Diagnostics.Debug.WriteLine(lastUpdate.ToString() + " " + file.FullName + " " + file.LastWriteTime.ToString());
                }
            }

            return lastUpdatedFile;
        }
    }
}