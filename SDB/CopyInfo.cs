using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolDB
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class CopyInfo
    {
        public string source;
        public string target;
        public string fname;
        public Boolean success;

        public CopyInfo()
        {
        }
        public CopyInfo(string s, string t, string f, Boolean rc)
        {
            source = s;
            target = t;
            fname = f;
            success = rc;
        }
       
    }
   
}
