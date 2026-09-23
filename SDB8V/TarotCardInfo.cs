using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SymbolDB
{
    [Serializable()]
    public class  TarotCardInfo
    {
     //string    tblCardInfo>;
        public int Index { get; set;}
        public string Notes {get; set; }
        public string NotesReversed {get; set; }

        public TarotCardInfo()
        {
        }
        public TarotCardInfo(int i, string n, string r)
        {
            Index = i;
            Notes = n;
            NotesReversed = r;
        }
        public TarotCardInfo(TarotCardInfo info)
        {
            this.Index = info.Index;
            this.Notes = info.Notes;
            this.NotesReversed = info.NotesReversed;
        }
    }
    
    
}
