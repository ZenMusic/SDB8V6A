using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    public class SoundItems
    {
        public int idx { get; set; }
        public string fname { get; set; }
        public string fpath { get; set; }

        public SoundItems()
        {
        }
        public SoundItems(int index, string fname2, string sounditem)
        {
            idx = index;
            fname = fname2;
            fpath = sounditem;
        }
    }
}
