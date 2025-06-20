using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    [Serializable()]
    public class TarotInfo2
    {
        public int VersionData;
        public string tblCardInfo;
        public List<tblCardInfo> tci = new List<tblCardInfo>();

        public TarotInfo2()
        {
           
        }
        
    }
}
