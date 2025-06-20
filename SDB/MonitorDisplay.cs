using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbolDB
{
    // Screen 0 is the MAIN DISPLAY
    // Screen 1 is the left display secondary
    // Screen 2 is the right display secondary
    // this is not correct for all systems
    // only mine
    // needs to be generalized              gv.screen[1].Bounds.Width, gv.screen[1].Bounds.Height, gv.screen[1]x, gv.screen[1].Bounds.Width);
    public class MonitorDisplay
    {
        public Screen screen;
        public int screenNumber = 0;   //gv.screenCount = screen.Length;
        public int screenx = 0;////- gv.screen[1].Bounds.Width;
        /// gv.screen[2]x = gv.screen[0].Bounds.Width;

        ScreenOrientation so = SystemInformation.ScreenOrientation;
        //            insertRow(gv.screenCount, gv.virtualScreenSize.Width, gv.virtualScreenSize.Height, 0, 0);
        //            insertRow(0, gv.screen[0].Bounds.Width, gv.screen[0].Bounds.Height, 0, gv.screen[0].Bounds.Width);


        public MonitorDisplay()
        {
        }
        public MonitorDisplay(Screen thisScreen, int snumber, int xposition) /////// USE THIS CONSTRUCTOR
        {
            screen = thisScreen;
            screenx = xposition;
            //
            screenNumber = snumber;
        }

    }
}