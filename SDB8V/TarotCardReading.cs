using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SymbolDB
{
    [Serializable()]
    public class  TarotCardReading
    {
     //string    tblCardInfo>;
        public int Index { get; set;}
        public string Notes {get; set; }
        public string NotesReversed {get; set; }
        public int DeckIndex { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool FaceUp { get; set; }
        public int ZIndex { get; set; }
        public TarotCardReading()
        {
        }
        public TarotCardReading(int i, string n, string r)
        {
            Index = i;
            Notes = n;
            NotesReversed = r;
        }
        public TarotCardReading(TarotCardReading info)
        {
            this.Index = info.Index;
            this.Notes = info.Notes;
            this.NotesReversed = info.NotesReversed;
            this.DeckIndex = info.DeckIndex;
            this.X = info.X;
            this.Y = info.Y;
            this.FaceUp = info.FaceUp;  
            this.ZIndex = info.ZIndex;
    }
}
    
    
}
