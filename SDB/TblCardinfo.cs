using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SymbolDB
{
    [Serializable()]
    public class tblCardInfo
    {
        //string    tblCardInfo>;
        public int Index { get; set; }
        public string Notes { get; set; }
        public string NotesReversed { get; set; }

        public tblCardInfo()
        {
        }
        public tblCardInfo(int i, string n, string r)
        {
            Index = i;
            Notes = n;
            NotesReversed = r;
        }
        public tblCardInfo(TarotCardInfo info)
        {
            this.Index = info.Index;
            this.Notes = info.Notes;
            this.NotesReversed = info.NotesReversed;
        }
    }


}
