using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    public class InitParms1
    {
        public string dirPath { get; set; } //directory with init file
        public string fpath { get; set; } //fullpath with init file
        public string targetDir1 { get; set; } //
        public string sourceDir1 { get; set; } // last transversal
        public string sourceDir2 { get; set; } // last transversal
        public string sourceDir3 { get; set; } // last transversal
        public string sourceDir4 { get; set; } // last transversal
        public string sourceDir5 { get; set; } // last transversal
        public string sourceDir6 { get; set; } // last transversal
        public string targetDir2 { get; set; }
        public string targetDir3 { get; set; }
        public string alarmSound { get; set; }
        public string watchFolder1 { get; set; }
        public string watchFolder2 { get; set; }
        public string watchFolder3 { get; set; }
        public int maxScreens { get; set; }
        public string history1 { get; set; }
        public string history2 { get; set; }
        public string history3 { get; set; }
        public string mostRecent { get; set; }
        public string mostRecentTarget { get; set; }
        public string mostRecentSubfolder { get; set; }
        public string notesBaseName { get; set; }
        public string mostRecent1 { get; set; }
        public string mostRecent2 { get; set; }
        public string mostRecent3 { get; set; }
        public string mostRecent4 { get; set; }
        public string mostRecent5 { get; set; }
        public string mostRecent6 { get; set; }
        public InitParms1 ()
        {

        }
        public InitParms1(string lstDir, string dirPath2, string fpath2, string targetDir2)
        {
            sourceDir1 = lstDir;
            dirPath = dirPath2;
            fpath = fpath2;
            targetDir1 = targetDir2;
        }
        public void SetTargetDir1(string dir)
        {
            targetDir1 = dir;
            mostRecentTarget = dir;
        }
        public void SetWatchDir1(string dir)
        {
            watchFolder1 = dir;
            mostRecentTarget = dir;
        }
        public void SetnotesBaseName(string name)
        {
            notesBaseName = name;
        }
    }
}
