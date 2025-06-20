using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class URLHistory : Form
    {
        bool bTrackHistory = false;
        Display1 display1 = null;
        DebugWindow debugw = null;
       // Browser1 browser1 = null;

        GlobalVars gv;

        Main main = null;
        Boolean bDisplay1 = false;
        Boolean bDisplayMain = false;
        Boolean bDisplay1image = false;
        
        public URLHistory(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
        }

       
        Point mylocation = new Point(1, 1);
        public void moveWindow(int x, int y)
        {

            mylocation.X = x;
            mylocation.Y = y;
            this.Location = mylocation;
        }
        public void moveWindow(Point loc)
        {

            mylocation.X = loc.X;
            mylocation.Y = loc.Y;
            this.Location = mylocation;
        }
        
        public void registerDisplay1(Display1 d1)
        {
            if (d1 != null)
            {
                display1 = d1;
                bDisplay1image = true;
            }
        }

        public void registerMain(Main mw)
        {
            if (main != null)
            {
                main = mw;
                bDisplay1 = false;
            }
        }

        private int FindMyString(string searchString)
        {
            // Ensure we have a proper string to search for. 
            if (searchString != string.Empty)
            {
                // Find the item in the list and store the index to the item. 
                int index = listBox1.FindString(@searchString);
                // Determine if a valid index is returned. Select the item if it is valid. 
                if (index != -1)
                {
                    listBox1.SetSelected(index, true);

                    return index;
                }
            }
            return 0;
        }

        public void write(string txt)
        {
            if (!bTrackHistory)
                return;
            if (FindMyString(txt) == 0)
            {
                if (listBox1.Items.Count > 1000)
                    return;

                //dmc2022temp    listBox1.Items.Add(txt);
                this.Update();

                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                listBox1.SelectedIndex = -1;

            }
        }

        public void w(string txt)
        {
            if (!bTrackHistory)
                return;
            if (!txt.Equals("F5"))
                write(txt);

        }
        public void w(int intg)
        {
            if (!bTrackHistory)
                return;
            write(intg.ToString());
        }
        public void w(string txt, int intg)
        {
            if (!bTrackHistory)
                return;
            write(txt + " " + intg.ToString());
            if (txt.Substring(1, 1).Equals("M"))
            {
                w("Main");
            }
        }
        public void w(string txt, string txt2, string txt3)
        {
            if (!bTrackHistory)
                return;
            write(txt + " " + txt2 + " " + txt3);
        }
        public void w(string txt, string txt2, string txt3, string txt4)
        {
            if (!bTrackHistory)
                return;
            write(txt + " " + txt2 + " " + txt3 + " " + txt4);
        }
        public void w(Point p)
        {
            if (!bTrackHistory)
                return;
            write("Point + " + p.X.ToString() + " " + p.Y.ToString());
        }
        public void w(string txt, string txt2)
        {
            if (!bTrackHistory)
                return;
            write(txt + " " + txt2);
        }

    }
}
