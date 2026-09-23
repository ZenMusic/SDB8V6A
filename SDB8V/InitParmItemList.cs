using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    class InitParmItemList
    {

        public List<InitParmsStruct> initParms = new List<InitParmsStruct>();

        InitParmsStruct none; // = new InitParmItem();
        InitParmsStruct temp; 

        public InitParmItemList()
        {
        }

        public InitParmItemList(string dirPath2, string lastDir2, string fpath2)
        {
            if (dirPath2.Contains("\\"))
            {
                dirPath2 = dirPath2.Replace("\\", "/");
            }
            temp.dirPath = dirPath2;
            temp.setLastDir(lastDir2);
            temp.fpath = fpath2;

            initParms.Add(temp);

        }
        public List<InitParmsStruct> getParmList()
        {
            return initParms;
        }
        public int getInitParmListLength()
        {
            return initParms.Count;
        }
        public int addItem(string dirPath2, string lastDir2, string fpath2)
        {
            InitParmsStruct newip;
            if (dirPath2.Contains("\\"))
            {
                dirPath2 = dirPath2.Replace("\\", "/");
            }
            temp.dirPath = dirPath2;
            temp.setLastDir(lastDir2);
            temp.fpath = fpath2;

            initParms.Add(temp);
            return initParms.Count();
        }
/*        public int removeItem(string fpath, string type1, int level1)
        {

            finfo.Remove(new FileInfoItem(fpath, type1, level1));
            return finfo.Count();
        }
 */
        public int removeItemAt(int index)
        {
            if (index < initParms.Count())
                initParms.RemoveAt(index);
            else
            {
                System.Windows.Forms.MessageBox.Show(String.Format("ERROR attempt to remove invalid index from ImageFileList {0} max {1}", index, initParms.Count()));
            }
            return initParms.Count();
        }
 
        public void clearList()
        {
            for (int idx = initParms.Count - 1; idx >= 0; --idx)
                    removeItemAt(idx);
        }
        public int updateLastDirPath(string fpath2)
        {
            int idx = 2;
            InitParmsStruct finfo =  this.initParms[idx];
            if (fpath2 != null)
                if (fpath2.Contains("\\"))
                {
                    fpath2 = fpath2.Replace("\\", "/");
                }

            finfo.setLastDir(fpath2);
            setIndexed(finfo, idx);
            return idx;
        }

        public string getFullPath(int idx)
        {
            return initParms.ElementAt(idx).ToString();
        }

        public InitParmsStruct getIndexed(int idx)
        {
            if (idx >= initParms.Count)
            {
                return none;
            }
            return initParms.ElementAt(idx);
        }

        public int setIndexed(InitParmsStruct item, int idx)
        {
           // FileInfoItem row = getIndexed(idx);

          //  row.setFileInfoItem(item, row);

            initParms[idx] = item;

            return idx;
        }


    }
}