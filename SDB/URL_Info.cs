using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolDB
{
    public struct URL   
    {
        public string URLaddress; //fullpath
        public string type;  // (f)ile , (d)irectory 
        public int level;    // nested subfolder level
        private DateTime ts;
        public string stimestamp;
        public char rating;
        public bool bDelete;
        public bool bInvalid;

    public class URL_List
    {

        public List<URL> URLinfo = new List<URL>();

        URL none = new URL();

        public URL_List()
        {
        }

        public URL_List(string URLaddress)
        {
            if (URLaddress.Contains("\\"))
            {
                URLaddress = URLaddress.Replace("\\", "/");
            }
            URLinfo.Add(new URL(URLaddress, type1, level1));

        }
        public List<URL> getURLinfo()
        {
            return URLinfo;
        }
        public int getURL_ListLength()
        {
            return URLinfo.Count;
        }
        public int addItem(string URLaddress, string type1, int level1)
        {
            if (URLaddress.Contains("\\"))
            {
                URLaddress = URLaddress.Replace("\\", "/");
            }

            URLinfo.Add(new URL(URLaddress, type1, level1));
            return URLinfo.Count();
        }
        public int addItem(URL fi)
        {

            URLinfo.Add(new URL(fi));
            return URLinfo.Count();
        }
        public int removeItem(string URLaddress, string type1, int level1)
        {

            URLinfo.Remove(new URL(URLaddress, type1, level1));
            return URLinfo.Count();
        }
        public int removeItemAt(int index)
        {

            URLinfo.RemoveAt(index);
            return URLinfo.Count();
        }
        public int getImageCount()
        {
            return URLinfo.Count();
        }
        public void clearList()
        {
            for (int idx = URLinfo.Count - 1; idx >= 0; --idx)
                removeItemAt(idx);
        }
        public int updatePath(string URLaddress2, int idx)
        {
            URL URLinfo = this.URLinfo[idx];
            if (URLaddress2 != null)
                if (URLaddress2.Contains("\\"))
                {
                    URLaddress2 = URLaddress2.Replace("\\", "/");
                }

            URLinfo.URLaddress = URLaddress2;
            setIndexed(URLinfo, idx);
            return idx;
        }
        public int updateRating(char rated, int idx)
        {
            URL URLinfo = this.URLinfo[idx];

            URLinfo.rating = rated;
            setIndexed(URLinfo, idx);
            return idx;
        }
        public int updateRating(string rated, int idx)
        {
            URL URLinfo = this.URLinfo[idx];

            URLinfo.rating = rated[0];
            setIndexed(URLinfo, idx);
            return idx;
        }
        public int markRating(string rated, int idx)
        {
            URL URLinfo = this.URLinfo[idx];

            URLinfo.rating = rated[0];
            setIndexed(URLinfo, idx);
            return idx;
        }
        public bool markInvalid(bool bflag, int idx)
        {
            URL URLinfo = this.URLinfo[idx];
            URLinfo.bInvalid = bflag;
            setIndexed(URLinfo, idx);
            return bflag;
        }
        public int updateDelete(bool bdelete, int idx, GlobalVars g)
        {
            URL URLinfo2 = getIndexed(idx);
            URLinfo2.setDelete(true);

            setIndexed(URLinfo2, idx);


            //  URLinfo2.Delete = true;

            return idx;
        }
        public string getFullPath(int idx)
        {
            return URLinfo.ElementAt(idx).ToString();
        }

        public URL getIndexed(int idx)
        {
            if (idx >= URLinfo.Count)
            {
                none.URLaddress = null;
                return none;
            }
            return URLinfo.ElementAt(idx);
        }

        public int setIndexed(URL item, int idx)
        {
            // URL row = getIndexed(idx);

            //  row.setURL(item, row);

            URLinfo[idx] = item;

            return idx;
        }

        public void setIndexed(string fname2, string fullpath, string dir, string ext2, string type2, string level2s, string len2s, string datetime2, string ts2, int idx)
        {
            URL row = getIndexed(idx);
            //numVal = Convert.ToInt32(input);

            int level2 = Convert.ToInt32(level2s);
            long len2 = Convert.ToInt32(len2s);
            DateTime dt = Convert.ToDateTime(datetime2);

            row.setURL(fname2, fullpath, dir, ext2, type2, level2, len2, dt, ts2);


        }

    }
}