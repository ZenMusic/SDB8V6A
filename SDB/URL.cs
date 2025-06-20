using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SymbolDB
{
    public struct URLitem
    {
        public string URLaddress; // file name + ext
        private string type;  // (f)ile , (d)irectory 
        private DateTime ts;
        private string stimestamp;
        public char rating;
        public bool bDelete;
        public bool bInvalid;


        public URLitem(string fullpath)
        {
            URLaddress = fullpath;
            type = "";
            rating = 'g';
            bDelete = false;
            bInvalid = false;
            ts = DateTime.Now;
            stimestamp = ts.ToLongTimeString();
        }
        public URLitem(string fullpath, char cRating)
        {
            URLaddress = fullpath;
            type = "";
            rating = cRating;
            bDelete = false;
            bInvalid = false;
            ts = DateTime.Now;
            stimestamp = ts.ToLongTimeString();
        }

        /// <summary>
        /// ////////////////////////////////////////////////  properties 
        /// </summary>
        public string Stimestamp
        {
            get
            {
                return stimestamp;
            }
            set
            {
                stimestamp = value;
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
        public URLitem getURLinfo()
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