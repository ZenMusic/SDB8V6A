using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    public class MRListImages
    {

        public List<FileInfoItem> finfoList = new List<FileInfoItem>();

        FileInfoItem none = new FileInfoItem();

        public MRListImages()
        {
        }
        public List<FileInfoItem> getFinfo()
        {
            return finfoList;
        }
        public FileInfoItem getFinfoItem(int idx)
        {
            return finfoList[idx];
        }
        public int getImageFileListLength()
        {
            return finfoList.Count;
        }
        public int addItem(FileInfoItem fi)
        {

            finfoList.Add(new FileInfoItem(fi));
            return finfoList.Count();
        }
        static int previousIdx = -1;
        public int SetUpMRL() // use for MRL
        {
            if (finfoList.Count < 5)
            {
                for (int idx = 0; idx < 5; ++idx)
                    finfoList.Add(new FileInfoItem(idx));
            }
            return finfoList.Count();
        }
        public int addItemMRL(FileInfoItem fi, int idx2) // use for MRL
        {
            if (idx2 > previousIdx) //forward
            {
                fi.index = idx2; //position
                if (finfoList.Count < 5)
                {
                    return 0;
                }
                updateMRL(fi, idx2);
                previousIdx = idx2;
            }
            return finfoList.Count();
        }
        public int updateMRL(FileInfoItem fi, int idx) // use for MRL
        {
            finfoList[4] = finfoList[3];
            finfoList[3] = finfoList[2];
            finfoList[2] = finfoList[1];
            finfoList[1] = finfoList[0];
            finfoList[0].index = idx;
            finfoList[0] = fi;

            return finfoList.Count();
        }

        public void Reorder(FileInfoItem fi)
        {
            finfoList[4] = finfoList[3];
            finfoList[3] = finfoList[2];
            finfoList[2] = finfoList[1];
            finfoList[1] = finfoList[0];
            finfoList[0] = fi;
        }
        public int removeItem(string fpath, string type1, int level1)
        {

            finfoList.Remove(new FileInfoItem(fpath, type1, level1));
            return finfoList.Count();
        }
        public int copyFileList(ImageFileList newList)
        {
            finfoList.Clear();
            finfoList = newList.finfoList;
            return finfoList.Count();
        }
        public int copyFileList(List<FileInfoItem> list)
        {
            finfoList.Clear();
            for (int idx = 0; idx < list.Count(); ++idx)
            {
                finfoList.Add(list[idx]);
            }
            return finfoList.Count();
        }

        public int removeItemAt(int index)
        {
            if (index < finfoList.Count())
                finfoList.RemoveAt(index);
            else
            {
                System.Windows.Forms.MessageBox.Show(String.Format("ERROR attempt to remove invalid index from ImageFileList {0} max {1}", index, finfoList.Count()));
            }
            return finfoList.Count();
        }
        public int getImageCount()
        {
            return finfoList.Count();
        }
        public void clearList()
        {
            finfoList.Clear();
            /*
            for (int idx = finfoList.Count - 1; idx >= 0; --idx)
                    removeItemAt(idx);
                    */
        }
        public int updatePath(string fpath2, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];
            if (fpath2 != null)
                if (fpath2.Contains("\\"))
                {
                    fpath2 = fpath2.Replace("\\", "/");
                }

            finfo.fpath = fpath2;
            setIndexed(finfo, idx);
            return idx;
        }
        public int updateRating(char rated, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];

            finfo.rating = rated;
            setIndexed(finfo, idx);
            return idx;
        }
        public int updateRating(string rated, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];

            finfo.rating = rated[0];
            setIndexed(finfo, idx);
            return idx;
        }
        public int markRating(string rated, int idx)
        {
            if (idx >= finfoList.Count)
            {
                return 0;
            }
            FileInfoItem finfo = this.finfoList[idx];

            finfo.rating = rated[0];
            setIndexed(finfo, idx);
            return idx;
        }
        public bool markInvalid(bool bflag, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];
            finfo.bInvalid = bflag;
            setIndexed(finfo, idx);
            return bflag;
        }
        public bool markInvalidAndDeletion(bool bflag, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];
            finfo.bInvalid = bflag;
            finfo.bDelete = bflag;
            if (bflag)
                finfo.rating = '@';
            else
                finfo.rating = 'g';

            setIndexed(finfo, idx);
            return bflag;
        }
        public int updateDelete(bool bdelete, int idx) // delete? index 
        {
            FileInfoItem finfo2 = getIndexed(idx);
            finfo2.setDelete(true);

            //setIndexed(finfo2, idx);


            //  finfo2.Delete = true;

            return idx;
        }
        public string getFullPath(int idx)
        {
            return finfoList.ElementAt(idx).ToString();
        }

        public FileInfoItem getIndexed(int idx)
        {
            if (idx >= finfoList.Count)
            {
                none.fpath = null;
                return none;
            }
            return finfoList.ElementAt(idx);
        }

        public int setIndexed(FileInfoItem item, int idx)
        {
            // FileInfoItem row = getIndexed(idx);

            //  row.setFileInfoItem(item, row);

            if (idx >= finfoList.Count)
            {
                finfoList.Add(item); // xxxxxxxxx
            }
            finfoList[idx] = item;

            return idx;
        }

        public void setIndexed(string fname2, string fullpath, string dir, string ext2, string type2, string level2s, string len2s, string datetime2, string ts2, int idx)
        {
            FileInfoItem row = getIndexed(idx);
            //numVal = Convert.ToInt32(input);

            int level2 = Convert.ToInt32(level2s);
            long len2 = Convert.ToInt32(len2s);
            DateTime dt = Convert.ToDateTime(datetime2);

            row.setFileInfoItem(fname2, fullpath, dir, ext2, type2, level2, len2, dt, ts2);


        }

    }
}