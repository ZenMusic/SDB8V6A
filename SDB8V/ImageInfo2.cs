using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    public class ImageInfo2
    {
        public string fname { get; set; } // file name + ext
        public string dpath { get; set; } //directory
        public string fpath { get; set; } //fullpath
        public string ext { get; set; }
        public long len { get; set; }
        public string type { get; set; }  // (f)ile , (d)irectory 
        public int level { get; set; }    // nested subfolder level
        private DateTime ts { get; set; }
        public string stimestamp { get; set; }
        public char rating { get; set; }
        public bool bDelete { get; set; }
        public bool bInvalid { get; set; }
        public string comment { get; set; }

        public ImageInfo2()
        {
        }
        public ImageInfo2(FileInfoItem fi)
        {
            fname = fi.fname;
            dpath = fi.dpath;
            fpath = fi.fpath;
            ext = fi.ext;
            len = fi.len;
            type = fi.type;
            level = fi.level;
            ts = DateTime.Now;
            stimestamp = fi.stimestamp;
            rating = fi.rating;
            bDelete = fi.bDelete;
            bInvalid = fi.bInvalid;
            comment = fi.comment;
        }
    }
}
