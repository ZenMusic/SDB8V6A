using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SymbolDB
{
    [Serializable()]
    public class FolderAssignments
    {
        public string fname { get; set; } // file name + ext
        public string dpath { get; set; } //directory
        public string ext { get; set; }
    }
}
