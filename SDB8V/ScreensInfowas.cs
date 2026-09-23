using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbolDB
{
    class ScreensInfo
    {
        public ScreensInfo(GlobalVars gv)
        {
            int nextScreen = 0;
            int nextScreen2 = 0;

            gv.screenSize = SystemInformation.PrimaryMonitorSize;
            gv.virtualScreenSize = SystemInformation.VirtualScreen;
            int scount = SystemInformation.MonitorCount;

            
            // Screen[0] is the MAIN DISPLAY   // Screen 1 is the right display secondary  // Screen 2 is the left display secondary
            // this is not necessarily the same for all systems -- only mine -- needs to be generalized

            Screen[] screen = Screen.AllScreens;
            int iScreenNumber = 1;
            Boolean[] bAssigned;

            bAssigned = new Boolean[4];
            bAssigned[0] = false;
            int iCount = 1;

            gv.displayCount = scount; // screen.Length;
            int test = screen.Length;
            gv.debug.w($"------------- display count {scount}  ------------");

            for (int idx = 0; idx < scount; ++idx)
            {
                if (screen[idx].Primary)
                {
                    gv.primaryScreen = idx;
                    gv.screen[0] = screen[idx];
                }
                else
                    gv.screen[iCount++] = screen[idx];
            }
            gv.displayCount = iCount;
            // portrait mode ?????
            // Boolean testb = portraitMode(screen[nextScreen2]);
            /*
            if (testb) ////// screen[nextScreen2].Bounds.Height > screen[nextScreen2].Bounds.Width)
            {
                int holdn = nextScreen2;
                nextScreen2 = nextScreen;
                nextScreen = holdn;
            }*/


            // form1.StartPosition = FormStartPosition.Manual;
            // form1.Bounds = Screen.AllScreens[0].Bounds;
            // form1.Show(); 
            // test1.Bounds // test1.WorkingArea // test1.DeviceName  // boolean test1.Primary

            ScreenOrientation so = SystemInformation.ScreenOrientation;
        }

        public Boolean portraitMode(Screen screen1)
        {
            return !landscapeMode(screen1);
        }
        public Boolean landscapeMode(Screen screen1)
        {
            int theScreenRectWidth = screen1.Bounds.Width;
            int theScreenRectHeight = screen1.Bounds.Height;

            //Compare height and width of screen and act accordingly.
            if (theScreenRectWidth < theScreenRectHeight)
            {
                // Run the application in portrait, as in:
                return false;
            }
            else
            {
                // Run the application in landscape, as in:
                return true;
            }
        }

    }
}
