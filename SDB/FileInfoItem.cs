using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace SymbolDB
{
    [Serializable()]
    public class FileInfoItem
    {
        public string fname { get; set; } // file name + ext
        public string ext { get; set; }
        public long len { get; set; }
        public string type { get; set; }  // (f)ile , (d)irectory 
        public int level { get; set; }    // nested subfolder level    // MOST RECENT COUNTER 1-5
        private DateTime ts { get; set; }
        public string stimestamp { get; set; }
        public char rating { get; set; }
        public bool bDelete { get; set; }
        public bool bInvalid { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string source { get; set; }
        public double playTime { get; set; }
        public int minutes { get; set; }
        public int seconds { get; set; }
        public string dpath { get; set; } //directory
        public string fpath { get; set; } //fullpath
        public int index { get; set; }    // slideShow Index
        public string desc { get; set; } //comments description
        public FileInfoItem()
        {

        }
        public FileInfoItem(string fullpath, string t, int lev)
        {
            fpath = fullpath;
            FileInfo fileInfo = new FileInfo(fpath);
            if (fileInfo.Exists)
            {
                //fullpathandname = fi.FullName;
                fname = fileInfo.Name;
                dpath = fileInfo.DirectoryName;
                if (dpath.Contains("\\"))
                {
                    dpath = dpath.Replace("\\", "/");
                }
                len = fileInfo.Length;
                ext = fileInfo.Extension;
                
                type = t;
                level = lev;
                //fi.LastWriteTime
                ts = fileInfo.CreationTime;
                //stimestamp = ts.ToLongTimeString();
               stimestamp =  fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ffff");
               rating = 'g';
               bDelete = false;
               bInvalid = false;
               desc = "";
            }
            else
            {
                desc = "";
                fname = "";
                dpath = "";
                len = 0;
                ext = "";
                type = "";
                level = 0;
                rating = 'g';
                bDelete = false;
                bInvalid = false;
                ts = DateTime.Now;
                stimestamp = ts.ToLongTimeString();
            }
        }
        public FileInfoItem(FileInfoItem fi)
        {
            this.fname = fi.fname;
            this.fpath = fi.fpath;
            this.dpath = fi.dpath;
            if (dpath.Contains("\\"))
            {
                dpath = dpath.Replace("\\", "/");
            }
            this.ext = fi.ext;
            this.type = fi.type;
            this.level = fi.level;
            this.len = fi.len;
            this.ts = fi.ts;
            this.stimestamp = fi.stimestamp;
            rating = fi.rating;
            bDelete = fi.bDelete;
            bInvalid = fi.bInvalid;
            source = fi.source;
            //
            width = fi.width;
            height = fi.height;
            //
            playTime = fi.playTime;
            minutes = fi.minutes;
            seconds = fi.seconds;
            desc = fi.desc; //2025
        }

        public FileInfoItem(int value)
        {
            dpath = value.ToString();
            level = value;
        }
        public FileInfoItem(string fullpath)
        {
            fpath = fullpath;
            FileInfo fi = new FileInfo(fpath);
            if (fi.Exists)
            {
                fname = fi.Name;
                dpath = fi.DirectoryName;
                if (dpath.Contains("\\"))
                {
                    dpath = dpath.Replace("\\", "/");
                }
                ext = fi.Extension;
                type = "f";
                level = 0;
                len = fi.Length;
                rating = 'g';
                bDelete = false;
                bInvalid = false;
                ts = fi.CreationTime;
                stimestamp =  ts.ToLongDateString() +  ts.ToLongTimeString();
               // MessageBox.Show(timestamp);
            }
            else
            {
                fname = "";
                dpath = "";
                len = 0;
                ext = "";
                type = "";
                level = 0;
                rating = 'g';
                bDelete = false;
                bInvalid = false;
                desc = "";

                ts = DateTime.Now;
                stimestamp =  ts.ToLongDateString() + ts.ToLongTimeString();
            }
        }

        public FileInfoItem(string fname2, string fullpath, string dir, string ext2, string type2, int level2, long len2, DateTime datetime2, string ts2)
        {
            this.fpath = fullpath;
            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }

            this.fname = fname2;
            this.dpath = dir;
            if (dpath.Contains("\\"))
            {
                dpath = dpath.Replace("\\", "/");
            }
            this.ext = ext2;
            this.type = type2;
            this.level = level2;
            this.len = len2;
            rating = 'g';
            desc = "";
            bDelete = false;
            bInvalid = false;

            this.ts = datetime2;
            this.stimestamp = ts2;

            // MessageBox.Show(timestamp);
        }
        //full assignment
        public FileInfoItem(string fname2, string ext2, string dir, string fullpath, string type2, int level2, long len2, DateTime ts2, string stimestamp2,
            bool bDelete2, bool bInvalid2, char rating2, string source2, double playTime2, int minutes2, int seconds2, int width2, int height2, string desc2)
        {
            this.fname = fname2;
            this.ext = ext2;
            this.len = len2;
            this.type = type2;
            this.level = level2;

            bDelete = bDelete2;
            bInvalid = bInvalid2;
            this.ts = ts2;
            this.stimestamp = ts2.ToString();
            rating = rating2;
            source = source2;
            desc = desc2; //2025
            ts = ts2;
            stimestamp = stimestamp2;

            playTime = playTime2;
            minutes = minutes2;
            seconds = seconds2;
            width = width2;
            height = height2;
            this.dpath = dir;
            this.fpath = fullpath;

            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }
            if (dpath.Contains("\\"))
            {
                dpath = dpath.Replace("\\", "/");
            }
            // MessageBox.Show(timestamp);
        }

        //full assignment minus TIMESTAMP
        public FileInfoItem(string fname2, string ext2, string dir, string fullpath, string type2, int level2, long len2, string stimestamp2,
            bool bDelete2, bool bInvalid2, char rating2, string source2, double playTime2, int minutes2, int seconds2, int width2, int height2, string desc2)
        {
            this.fname = fname2;
            this.ext = ext2;
            this.len = len2;
            this.type = type2;
            this.level = level2;

            bDelete = bDelete2;
            bInvalid = bInvalid2;
            this.stimestamp = stimestamp2;
            rating = rating2;
            source = source2;
            desc = desc2; //2025
            stimestamp = stimestamp2;

            playTime = playTime2;
            minutes = minutes2;
            seconds = seconds2;
            width = width2;
            height = height2;
            this.dpath = dir;
            this.fpath = fullpath;

            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }
            if (dpath.Contains("\\"))
            {
                dpath = dpath.Replace("\\", "/");
            }
            // MessageBox.Show(timestamp);
        }

        public void setDelete(bool bflag)
        {
            this.bDelete = bflag;
        }
        public void setValid(bool bflag)
        {
            this.bInvalid = bflag;
        }
        public void setFileInfoItem(string fname2, string fullpath, string dir, string ext2, string type2, int level2, long len2, DateTime datetime2, string ts2)
        {
            fpath = fullpath;
           
                this.fname = fname2;
                this.dpath = dir;
                this.ext = ext2;
                this.type = type2;
                this.level = level2;
                this.len = len2;

                this.ts = datetime2;
                this.stimestamp = ts2;
               
              // MessageBox.Show(timestamp);
        }
        public void setFileInfoItem(FileInfoItem fi, FileInfoItem me)
        {
            me.fpath = fi.fpath;

            me.fname = fi.fname;
            me.dpath = fi.dpath;
            me.ext = fi.ext;
            me.type = fi.type;
            me.level = fi.level;
            me.len = fi.len;
            me.desc = fi.desc;
            me.ts = fi.ts;
            me.stimestamp = fi.stimestamp;
            me.rating = fi.rating;
            me.bDelete = fi.bDelete;
            me.bInvalid = fi.bInvalid;
            // MessageBox.Show(timestamp);
        }
        public char getFileType(string fp)
        {
            string type;
            fpath = fp;
            if (File.Exists(fp))
            {
                return 'f';
            }
            else if (Directory.Exists(fp))
            {
                return 'd';
            }
            else
            {
                return 'x';
            }
        }
        public void SetResolution(int h, int w)
        {
            height = h;
            width = w;
        }
        public Point GetResolution()
        {
            Point xy = new Point(height, width);
            return xy;
        }
        public void SetDuration(double dur)
        {
            minutes = (int) dur / 60;
            seconds = (int) dur - (minutes * 60);
            playTime = dur;
        }
        /*
/// <summary>
/// ////////////////////////////////////////////////  properties 
/// </summary>
        public string Fname
        {
            get
            {
                return fname;
            }
            set
            {
                fname = value;
            }
        }
        public string Dpath
        {
            get
            {
                return dpath;
            }
            set
            {
                dpath = value;
            }
        }
        public string Fpath
        {
            get
            {
                return fpath;
            }
            set
            {
                fpath = value;
            }
        }
        public string Ext
        {
            get
            {
                return ext;
            }
            set
            {
                ext = value;
            }
        }
        public DateTime Ts
        {
            get
            {
                return ts;
            }
            set
            {
                ts = value;
            }
        }
        public long Len
        {
            get
            {
                return len;
            }
            set
            {
                len = value;
            }
        }
        
        public char Rating
        {
            get
            {
                return rating;
            }
            set
            {
                rating = value;
            }
        }
        public bool Delete
        {
            get
            {
                return bDelete;
            }
            set
            {
                bDelete = value;
            }
        }
//        public string stimestamp;

        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }*/

        public FileInfoItem getFileInfo()
        {
            return this;
        }
    }
}

/*

 FileInfo[] fiArr = di.GetFiles();
        // Display the names and sizes of the files.
        Console.WriteLine("The directory {0} contains the following files:", di.Name);
        foreach (FileInfo f in fiArr)
*/