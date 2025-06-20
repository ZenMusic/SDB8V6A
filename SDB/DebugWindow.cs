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
    public partial class DebugWindow : Form
    {
        Display1 mainDisplay = null;
        
        
        Main main = null;
        Boolean bMainDisplayActivate = false;
        Boolean bDisplayMain = false;
        Boolean bDisplay1image = false;

        GlobalVars gv;

        public DebugWindow(GlobalVars g)
        {
            InitializeComponent();
            gv = g;
            debugLevel = 0;
        }
        public void registerDisplayMain(Display1 d1)
        {
            if (d1 != null)
            {
                mainDisplay = d1;
                bMainDisplayActivate = true;
            }
        }
        
        
        public void registerMain(Main mw)
        {
            if (main != null)
            {
                main = mw;
                bMainDisplayActivate = false;
            }
        }
        public void moveToScreen3()
        {
            this.Location = new Point(gv.screen[2].Bounds.X + 200, gv.screen[2].Bounds.Y + 100);
        }
        public void moveToScreen2()
        {
            this.Location = new Point(gv.screen[1].Bounds.X + 100, gv.screen[1].Bounds.Y + 100);
        }

        Point mylocation = new Point(1, 1);
        public void moveDebugWindow(int x, int y)
        {

            if (x < 0 || y < 0)
            {
                mylocation.X += 300;
                mylocation.Y += 200;
            }
            else
            {
                mylocation.X = x;
                mylocation.Y = y;
            }
            //this.Location = new Point(x, y);
            this.Location = mylocation;

        }
        public Point getLocation()
        {
            return mylocation;
        }
        private void winDebug_Load(object sender, EventArgs e)
        {

        }


        public void wdot()
        {
          //  write(".");
        }
        public void wdot(string txt)
        {
          //  write(txt);
        }

        public void w(string txt)
        {
            if (debugLevel <= 0)
                return;

            if (txt == null)
                return;
            if (!bDisplayDebugInfo)
                return;
            if (!txt.Equals("F5"))
                write(txt);
           
        }
        public void w(int intg)
        {
            if (debugLevel <= 0)
                return;
            if (!bDisplayDebugInfo)
                return;
            write(intg.ToString());
        }
        public void w(string txt, int intg)
        {
            if (debugLevel <= 0)
                return;
            if (!bDisplayDebugInfo)
                return;
            write(txt + " " + intg.ToString());
            if (txt.Substring(1, 1).Equals("M"))
            {
                w("Main");
            }
        }
        public void w(string txt, string txt2, string txt3)
        {
            if (debugLevel <= 0)
                return;
            if (!bDisplayDebugInfo)
                return;
            write(txt + " " + txt2 + " " + txt3);
        }
        public void w(string txt, string txt2, string txt3, string txt4)
        {
            return;
            if (debugLevel <= 0)
                return;
            if (!bDisplayDebugInfo)
                return;
            write(txt + " " + txt2 + " " + txt3 + " " + txt4);
        }
        public void w(string txt, string txt2, string txt3, string txt4, string txt5)
        {
            if (debugLevel <= 0)
                return;
            if (!bDisplayDebugInfo)
                return;
            write(txt + " " + txt2 + " " + txt3 + " " + txt4 + " " + txt5);
        }
        public void w(string txt, string txt2, string txt3, string txt4, string txt5, string txt6)
        {
            if (debugLevel <= 0)
                return;
            if (!bDisplayDebugInfo)
                return;
            write(txt + " " + txt2 + " " + txt3 + " " + txt4 + " " + txt5 + " " + txt6);
        }
        public void w(Point p)
        {
            if (debugLevel <= 0)
                return;
            if (!bDisplayDebugInfo)
                return;
            write("Point + " + p.X.ToString() + " " + p.Y.ToString());
        }
        public void w(string txt, string txt2)
        {
            if (debugLevel <= 0)
                return;
            if (!bDisplayDebugInfo)
                return;
            write(txt + " " + txt2);
        }
        public void wd(string txt, int debug)
        {
            return; // debug window causes out of memory error
            if (debug < 9)
            {
                if (debugLevel <= 0)
                    return;
                if (!bDisplayDebugInfo)
                    return;
            }
            write(txt);
        }
        public void wd(string txt, string txt2, int debug)
        {
            return; // debug window causes out of memory error
            if (debug < 9)
            {
                if (debugLevel <= 0)
                    return;
                if (!bDisplayDebugInfo)
                    return;
            }
            write(txt + " " + txt2);
        }
        public void wd(string txt, string txt2, string txt3, int debug)
        {
            return; // debug window causes out of memory error
            if (debug < 9)
            {
                if (debugLevel <= 0)
                    return;
                if (!bDisplayDebugInfo)
                    return;
            }
            write(txt + " " + txt2 + " " +txt3);
        }
        public void winfo(ListViewItem item)
        {
            // write(item.ToString());
            if (debugLevel < 0)
                return;
            string itemText = "subitems ";

            for (int idx2 = 0; idx2 < item.SubItems.Count; idx2++)//   ListView1.Items(i).SubItems.Count - 1
            {
                itemText = itemText + item.SubItems[idx2].ToString() + " ";
            }
            w(itemText);
            
        }

        bool bHandleActivation = false;

        int debugLevel = 1;

        int MAX_ITEMS_LISTBOX1 = 1000;

        public void write(string txt)
        {
            return;
           if (listBox1.Items.Count >= MAX_ITEMS_LISTBOX1) //// fix this 2016    to much data in debug
                return;

            bHandleActivation = false;
            if (bHandleActivation)
                this.Activate();

            if (listBox1.Items.Count > MAX_ITEMS_LISTBOX1)
            {
                for (int i = listBox1.Items.Count - 1; i > -1; i--)
                {
                    {
                        if (true) //(listBox1.Items[i].Contains("OBJECT"))
                        {
                            listBox1.Items.RemoveAt(i);
                        }
                    }
                }
            }

          //dmc2022temp  listBox1.Items.Add(txt);

                this.Update();

            listBox1.SelectedIndex = listBox1.Items.Count - 1;
                listBox1.SelectedIndex = -1;
                if (bHandleActivation)
                {
                    if (bMainDisplayActivate)
                        mainDisplay.Activate();
                    else if (main != null)
                    {
                        if (bDisplayMain)
                            main.Activate();
                    }
                }
        }

        public void setDisplayMainFocus(bool flg)
        {
            bDisplayMain = flg;
            if (bDisplayMain)
            {
                Point p = gv.mainWindow.Location;
                p.X = gv.mainWindow.Width + 4 + p.X;
                this.Location = p;
            }
        }

        public bool bDisplayImage = false;
        public bool bActivedDisplay1 = false;

        public void hideDisplay2()
        {
            pb1.Image = null;
            if (!bDisplayImage)
            {
                if (display1 != null && !display1.IsDisposed)
                    display1.Hide();
            }
        }
        public void displayThisImageOnce(Image img)
        {
            if (bDisplayImage)
                pb1.Image = img;
                pb1.Refresh();
        }
        public void displayThisImage(Image img)
        {
            if (bDisplay1)
            {
                if (display1 == null || display1.IsDisposed)
                    return;
              //  if (!bActivedDisplay1)
                //    display1.Activate();
                display1.Show();
               // gv.browser1.Focus();
                display1.displayThisImage(img);
            }
            if (!bDisplayImage)
                return;
            pb1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pb1.Image = img;
          // this.pb1.BorderStyle = BorderStyle.None;
         //  this.Refresh();
        }
        public void displayThisImageAlways(Image img)
        {
            pb1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pb1.Image = img;
          //  this.pb1.BorderStyle = BorderStyle.None;
          //  this.Refresh();
        }
        public void displayThisImage(string imgFile)
        {
            if (!bDisplayImage)
                return;
            pb1.SizeMode = PictureBoxSizeMode.Zoom ;
            pb1.Image = System.Drawing.Image.FromFile(imgFile);
            //pb1.Image = new Bitmap(open.imgFile);
         //   this.pb1.BorderStyle = BorderStyle.None;
         //   this.Refresh();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void relocate_button_Click(object sender, EventArgs e)
        {
            w("debug window location");
            w(this.Location);
        }

        private void DebugWindow_Activated(object sender, EventArgs e)
        {
            if (bDisplayMain)
                gv.mainWindow.Focus();
        }

        private void cbDisplayImage_CheckedChanged(object sender, EventArgs e)
        {
            bDisplayImage = cbDisplayImage.Checked;
            if (!bDisplayImage)
                pb1.Image = null;
            this.Refresh();
        }

        private void DebugWindow_Resize(object sender, EventArgs e)
        {
            //pb1.Width = pbNext.Location.X - 10;
            listBox1.Width = pb1.Location.X;

        }
        public Display1 display1;
        bool bDisplay1 = false;


        public void setDisplay1()
        {
            if (cbDisplay1.Checked)
            {
                bDisplay1 = true;
                if (display1 == null || display1.IsDisposed)
                    display1 = new Display1(gv);
                display1.setFullScreenMode(true);

                if (bPosition1)
                    display1.SetBounds(gv.screen[2].Bounds.X, gv.screen[2].Bounds.Y, gv.screen[2].Bounds.Width, gv.screen[2].Bounds.Height);
                else
                    display1.SetBounds(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height);

                display1.Show();
                this.Activate();
            }
            else
            {
                bDisplay1 = false;
                if (display1 != null)
                {
                    display1.Hide();
                    display1.Dispose();
                }
                gv.debug.w("display1 deleted");
            }
        }
        private void cbDisplay1_CheckedChanged(object sender, EventArgs e)
        {
            setDisplay1();

        }

        public bool bPosition1 = true;

        public void setDisplaySaved(bool bSet)
        {
            cbDisplaySave.Checked = bSet;
            cbPosition1.Checked = bSet;
            bPosition1 = bSet;
            cbDisplay1.Checked = bSet;
            setDisplay1();
            cbDisplayImage.Checked = bSet;
            bDisplayImage = cbDisplayImage.Checked;
                pb1.Image = null;
            this.Refresh();

        }

        private void cbPosition1_CheckedChanged(object sender, EventArgs e)
        {
            bPosition1 = cbPosition1.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            display1.Close();
            display1.Dispose();
            cbDisplay1.Checked = false;
            
        }

        

        private void btMain_Click(object sender, EventArgs e)
        {
            gv.mainWindow.Activate();
        }

        public void display2Reset()
        {
            if (display1 != null)
                display1.resetPB();
        }
        private void btReset_Click(object sender, EventArgs e)
        {
            if (display1 != null)
                display1.resetPB();
        }
        public void resetDisplaySave()
        {
            cbDisplaySave.Checked = false;
            setDisplay1();
            cbDisplaySave.Checked = true;
            setDisplay1();
        }
        private void cbDisplaySave_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplaySave.Checked)
            {
                cbPosition1.Checked = true;
                cbDisplay1.Checked = true;
                cbDisplayImage.Checked = true;
                bPosition1 = true;
                setDisplay1();
                bDisplayImage = true;
            }
            else
            {
                cbPosition1.Checked = false;
                cbDisplay1.Checked = false;
                cbDisplayImage.Checked = false;
                bPosition1 = false;
                cbDisplay1.Checked = false;
                setDisplay1();
                bDisplayImage = false;
            }
        }
        bool bDisplayDebugInfo = true;
        private void cbDisplayInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisplayInfo.Checked)
            {
                bDisplayDebugInfo = true;
            }
            else
            {
                bDisplayDebugInfo = true;
            }
        }
        private string getNewestImage()
        {
            string test = gv.mainWindow.ff.getLastUpdatedFile("C:/Users/david/Documents/111");
            w("last image ", test);
            this.display1.displayThisImage(test);
            return test;
        }

        public void displayWatched(string fpath)
        {
            w("watch noted change on ", fpath);
            
        }

        Watcher fileWatcher = null;


        private void cbMonitor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMonitor.Checked)
            {
                getNewestImage();
            }
            else
            {
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void cbDisplayNumber_CheckedChanged(object sender, EventArgs e)
        {
          /*
           * if (cbDisplayNumber.Checked)
                moveToScreen3();
            else
                moveToScreen2();
           */
        }

        private void btHideDebug_Click(object sender, EventArgs e)
        {
                debugLevel = 0;
                this.Hide();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int result;
            try
            {
                result = Convert.ToInt32(numericUpDown1.Value);
            }
            catch (OverflowException)
            {
                result = 0;
                numericUpDown1.Value = 0;
            }
            debugLevel = result; // result;
        }

        int myScreen = 0;
        private void button4_Click(object sender, EventArgs e)
        {
                ///MessageBox.Show(gv.displayCount.ToString() );

                if (gv.displayCount > 1)
                    ++myScreen;
                if (myScreen >= gv.screen.Length)
                    myScreen = 0;
                this.Location = new Point(gv.screen[myScreen].Bounds.X, gv.screen[myScreen].Bounds.Y);

        }
    }
}
