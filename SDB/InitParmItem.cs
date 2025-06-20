using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    [Serializable()]
    public struct InitParmsStruct
    {
        public string lastDir; // last transversal
        public string dirPath; //directory with init file
        public string fpath; //fullpath with init file
        public string targetDir; //
        public void setInitParms(InitParmsStruct fi, InitParmsStruct me)
        {
            me.fpath = fi.fpath;
            me.lastDir = fi.lastDir;
            me.dirPath = fi.dirPath;
            me.targetDir = fi.targetDir;
            // MessageBox.Show(timestamp);
        }
        public void setInitParms(string lastDir1)
        {
            lastDir = lastDir1;
        }
        public void setInitParms(string lastDir1, string dirpath2, string fpath2)
        {
            lastDir = lastDir1;
            dirPath = dirpath2;
            fpath = fpath2;
        }
        public void setInitParms(string lastDir1, string dirpath2, string fpath2, string targetDir2)
        {
            lastDir = lastDir1;
            dirPath = dirpath2;
            fpath = fpath2;
            targetDir = targetDir2;
        }
        public void setTargetFolder(string targetDir2)
        {
            targetDir = targetDir2;
        }

    }
}

/*

 FileInfo[] fiArr = di.GetFiles();
        // Display the names and sizes of the files.
        Console.WriteLine("The directory {0} contains the following files:", di.Name);
        foreach (FileInfo f in fiArr)
*/