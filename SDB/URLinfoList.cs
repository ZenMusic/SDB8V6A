using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolDB
{
    public class URLinfoList
    {

        public List<URLitem> uinfo = new List<URLitem>();

        URLitem none = new URLitem();

        public URLinfoList()
        {
        }

        public URLinfoList(string fpath)
        {
            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }
            uinfo.Add(new URLitem(fpath));

        }

        public List<URLitem> getURLinfo()
        {
            return uinfo;
        }
        public int getURLinfoListLength()
        {
            return uinfo.Count;
        }
        public int addItem(string fpath)
        {
            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }

            uinfo.Add(new URLitem(fpath, 'g'));
            return uinfo.Count();
        }
        public int addItem(string fpath, char rating)
        {
            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }

            uinfo.Add(new URLitem(fpath, rating));
            return uinfo.Count();
        }
        /*
        public int addItem(URLitem fi)
        {

            uinfo.Add(new URLitem(fi));
            return uinfo.Count();
        }
         * */
        public int removeItem(string fpath)
        {

            uinfo.Remove(new URLitem(fpath));
            return uinfo.Count();
        }
        public int removeItemAt(int index)
        {

            uinfo.RemoveAt(index);
            return uinfo.Count();
        }
        public int getUrlCount()
        {
            return uinfo.Count();
        }
        public void clearList()
        {
            for (int idx = uinfo.Count - 1; idx >= 0; --idx)
                removeItemAt(idx);
        }
        public int updateURLaddress(string address, int idx)
        {
            URLitem uinfo = this.uinfo[idx];
            if (address != null)
                if (address.Contains("\\"))
                {
                    address = address.Replace("\\", "/");
                }

            uinfo.URLaddress = address;
            setIndexed(uinfo, idx);
            return idx;
        }
        public int updateRating(char rated, int idx)
        {
            URLitem uinfo = this.uinfo[idx];

            uinfo.rating = rated;
            setIndexed(uinfo, idx);
            return idx;
        }
        public int updateRating(string rated, int idx)
        {
            URLitem uinfo = this.uinfo[idx];

            uinfo.rating = rated[0];
            setIndexed(uinfo, idx);
            return idx;
        }
        public int markRating(string rated, int idx)
        {
            URLitem uinfo = this.uinfo[idx];

            uinfo.rating = rated[0];
            setIndexed(uinfo, idx);
            return idx;
        }
        public bool markInvalid(bool bflag, int idx)
        {
            URLitem uinfo = this.uinfo[idx];
            uinfo.bInvalid = bflag;
            setIndexed(uinfo, idx);
            return bflag;
        }
        public int updateDelete(bool b2delete, int idx, GlobalVars g)
        {
            URLitem uinfo2 = getIndexed(idx);
            uinfo2.bDelete = b2delete;

            setIndexed(uinfo2, idx);


            //  uinfo2.Delete = true;

            return idx;
        }
        public string getFullPath(int idx)
        {
            return uinfo.ElementAt(idx).ToString();
        }

        public URLitem getIndexed(int idx)
        {
            if (idx >= uinfo.Count)
            {
                none.URLaddress = null;
                return none;
            }
            return uinfo.ElementAt(idx);
        }

        public int setIndexed(URLitem item, int idx)
        {
            // URLitem row = getIndexed(idx);

            //  row.setURLitem(item, row);

            uinfo[idx] = item;

            return idx;
        }

    }
}