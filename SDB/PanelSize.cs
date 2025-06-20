using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SymbolDB
{
    public class PanelSize
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public PanelSize()
        {
        }
        public PanelSize(Rectangle rec)
        {
            x = rec.X;
            y = rec.Y;
            width = rec.Width;
            height = rec.Height;

        }
        public PanelSize(int x1, int y1, int w1, int h1)
        {
            x = x1;
            y = y1;
            width = w1;
            height = h1;
        }
    }
}
