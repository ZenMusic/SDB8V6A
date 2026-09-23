#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
/*
 fixing rubber-banding lasso for .NET8+
 * */

//
// Mode     Shows Entire Image?	Keeps Aspect Ratio?	 Notes
// Zoom	              ✅ Yes	           ✅ Yes	  Best for full-image display
// StretchImage	      ✅ Yes	           ❌ No	  Image may look distorted
// Normal	          ❌ No	               ✅ Yes	  Shows at original size; may clip
// AutoSize	          ✅ Yes	           ✅ Yes	  PictureBox resizes to image size
// CenterImage	      ❌ No	               ✅ Yes	  Centers but doesn’t resize


namespace SymbolDB
{
    public partial class Display1 : Form
    {
        GlobalVars gv;
        Boolean debug = false;

        FileFunctions ff;
        Boolean bCopyMode = false; //////////////////////////////////////////////////////// bCopyMode
        DialogGetInfo infod; ///  get annotation inforamtion = new DialogGetInfo(outlineBox, outlineBounds, this, finfo);

        D1function displayMode = D1function.SLIDESHOW;
        string myName = "display1";

        public PanelSize panel_size = new PanelSize(0, 0, 100, 100);

        //using System.Drawing  IMAGE 
        Image image = null;
        Size originalSize;

        FileInfoItem finfo;

        ImageInfo iinfo = new ImageInfo();

        ImageInfo[] tagInfo = new ImageInfo[100];
        int tagCount = 0;

        //---- d1mode    OPERATIONS
        // 'd'= display
        // 'a' = annotation
        // 's' = slideshow
        // 'c' = catalog  images (manual slideshow)

        ImgDisplayParms dp = new ImgDisplayParms();

        //System.Media.SoundPlayer startSoundPlayer = new System.Media.SoundPlayer(@"C:\Windows\Media\chord.wav");

        //outlining Rubber Band on PB /////////////////////////////// Picture Box SELECTION OUTLINING ///////////////
        Boolean bHaveMouse;
        Point ptOriginal = new Point();
        Point ptLast = new Point();
        Rectangle outlineBox = new Rectangle();
        Rectangle outlineBounds = new Rectangle();
        Boolean bKeepOutlineOnUp = true; // leave the OUTLINE DISPLAYED when mouse UP
        TagPB tag;
        //PictureBox tagpb;
        Boolean useTagpb = true;
        Boolean showingTag = false;
        Bitmap captured;
        //Reticle
        Point dialogLocation = new Point(1200, 200); // this.Location = new Point(1200, 200);
        int lineWidth = 2;
        int offsetScreenX = 8;
        int offsetScreenY = 8;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        Boolean togglePB1 = true;
        //---------------------- DISPLAY ------------ and MONITOR --------------------
        DisplayMode dm;
        MonitorDisplay md;
        int screenX;
        string displayName;

        // Rubber‐band state  AI2025
        private bool _rbDragging = false;
        private Point _rbStart, _rbEnd;
        private Rectangle _rbRect = Rectangle.Empty;

        public Display1(GlobalVars g) ///////////////////////////////////////<<<<<<<<<<<<<<< Default Constructor>>>>>
        {
            InitializeComponent();
            //AI2025
            // Hook up lasso events on pb1:
            pb1.MouseDown += Pb1_MouseDown;
            pb1.MouseMove += Pb1_MouseMove;
            pb1.MouseUp += Pb1_MouseUp;
            pb1.Paint += Pb1_Paint;
            //
            //
            pb1.MouseWheel += new MouseEventHandler(pb1_MouseWheel);

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            this.AutoScaleMode = AutoScaleMode.Dpi;


            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            gv = g;
            // gv.bResize = true;
            gv.debug.registerDisplayMain(this);
            ff = new FileFunctions(gv);

            this.WindowState = FormWindowState.Maximized;
            // Rectangle rec = new Rectangle(0, -4, 444, 844); //   width height
            // pb.Bounds = rec;

            tagpb1.Visible = false;
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called

            dm = new DisplayMode(ImageSize.BESTFIT, ImageLocation.CENTER, true, true);

            //  startWatcher();
        }
        //
        /* popup is a ContextMenuStrip  
        * should appear when a user right-clicks, reacting to the surroundings.
        * 
        * 
        * */
       
        // Called when either mouse button is pressed. 
        private void pb_MouseDown(object sender, MouseEventArgs e)
        {
            // 1) Right‐click still shows context menu/etc.
            if (e.Button == MouseButtons.Right)
            {
                gv.mainWindow.stopSlideShow();
                gv.debug.w("----pointer position", e.X.ToString(), e.Y.ToString());

                gv.debug.w("screen X Y", this.Location.X.ToString(), this.Location.Y.ToString());
                int newx = this.Location.X + e.X;
                int newy = this.Location.Y + e.Y;
                Point newloc = new Point(newx, newy);
                //this.Focus();
                //this.contextMenuStrip1.SetBounds(e.X + newx, e.Y + newy, 100, 100); //// right popup context menu
                gv.debug.w("popup (contextMenuStrip) position", newx.ToString(), newy.ToString());


                contextMenuStrip1.Show(newloc);
                this.contextMenuStrip1.Visible = true;
               // handled = true;
                return;
            }

            // 2) Left‐click in ANNOTATE mode starts rubber‐band
            if (displayMode == D1function.ANNOTATE && e.Button == MouseButtons.Left)
            {
                _rbDragging = true;
                _rbStart = e.Location;
                _rbEnd = e.Location;
                _rbRect = Rectangle.Empty;
                pb1.Invalidate();   // kick off a repaint
            }
        }
        //
        //AI2025 --------------------------------------------------------------------------------------------------///////////////////////////////////////////////
        //
        private void Pb1_MouseDown(object sender, MouseEventArgs e)
        {
            if (displayMode != D1function.ANNOTATE || e.Button != MouseButtons.Left)
                return;
            _rbDragging = true;
            _rbStart = e.Location;
            _rbEnd = e.Location;
            _rbRect = Rectangle.Empty;
            pb1.Invalidate();
        }

        private void Pb1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_rbDragging) return;
            _rbEnd = e.Location;
            _rbRect = new Rectangle(
                Math.Min(_rbStart.X, _rbEnd.X),
                Math.Min(_rbStart.Y, _rbEnd.Y),
                Math.Abs(_rbEnd.X - _rbStart.X),
                Math.Abs(_rbEnd.Y - _rbStart.Y)
            );
            pb1.Invalidate();
        }
        private void Pb1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_rbDragging) return;
            _rbDragging = false;
            pb1.Invalidate();

            if (_rbRect.Width > 0 && _rbRect.Height > 0)
            {
                var capture = cropSelection(_rbRect);
                if (capture != null)
                {
                    captured = capture;
                    // send it to your dialog or debug display
                    gv.debug.displayThisImage(capture);
                    showTag(_rbRect);
                    openGetInfoDialog();
                }
            }
        }
        private void xxPb1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_rbDragging) return;
            _rbDragging = false;
            pb1.Invalidate();    // erase the last box

            if (_rbRect.Width > 0 && _rbRect.Height > 0)
            {
                // your existing logic that used outlineBounds:
                captureScreen(_rbRect);
                showTag(_rbRect);
                openGetInfoDialog();
            }
            pb1.Image = image;
        }

        private void Pb1_Paint(object sender, PaintEventArgs e)
        {
            if (_rbDragging && _rbRect != Rectangle.Empty)
            {
                using var pen = new Pen(Color.Red) { DashStyle = DashStyle.Dash };
                e.Graphics.DrawRectangle(pen, _rbRect);
            }
        }

        //
        //AI2025 END
        //

        Watcher fileWatcher = null;

        public void startWatcher()
        {
            if (fileWatcher == null)
            {
                fileWatcher = new Watcher(gv, null, this);
            }
            // 2017 in for the folder monitor
            Point lpoint = new Point(0, 0);
            pb1.Location = lpoint;
            pb1.Height = this.Height;
            pb1.Width = this.Width;
            // end of test code 2017
        }

        // Returns the SCREEN in gv.screen[] whose DeviceName ends with the given GDI index.
        public Screen GetScreenByGdiIndex(int gdiIndex)
        {
            foreach (var s in gv.screen)
            {
                if (s == null) continue;
                var m = System.Text.RegularExpressions.Regex.Match(s.DeviceName ?? "", @"\d+$");
                if (m.Success && int.Parse(m.Value) == gdiIndex)
                    return s;
            }
            return null;
        }
        /// Returns the index in gv.screen[] whose DeviceName ends with the given GDI index.
        /// e.g. argument 2 matches "\\.\DISPLAY2"
        /// Returns -1 if not found.
        /// </summary>
        public int GetScreenIndexByGdiIndex(int gdiIndex)
        {
            for (int i = 0; i < gv.screen.Length; i++)
            {
                var s = gv.screen[i];
                if (s == null) continue;
                var m = System.Text.RegularExpressions.Regex.Match(s.DeviceName ?? "", @"\d+$");
                if (m.Success && int.Parse(m.Value) == gdiIndex)
                    return i;
            }
            return -1;
        }
        public void setDisplayMonitor(Screen s, int snumber)
        {
            displayName = "Display " + snumber.ToString();

            // s.Bounds.X = gv.screen[1]x;
            if (snumber > gv.screen.Length)
                return;
            Screen screenA = GetScreenByGdiIndex(snumber);
            md = new MonitorDisplay(screenA, snumber);// gv.screen[snumber - 1].Bounds.X);
            screenX = md.screen.WorkingArea.X;
            this.Bounds = md.screen.Bounds;
            //new Rectangle(md.screen.Bounds.X, md.screen.Bounds.Y, md.screen.Bounds.Width, md.screen.Bounds.Height);/// screen1x, 200, gv.screen[1].Bounds.Width - 100, gv.screen[1].Bounds.Height - 100);
            this.myName = "Display " + snumber.ToString();
            if (true)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(s.WorkingArea.X, s.WorkingArea.Y);
            }
        }
        Image originalImage;

        public void resetPB()
        {
            this.Focus();
            pb1.Image = null;
            pb1.Refresh();
            this.Refresh();
        }
        public void showMessage(string txt)
        {
            this.Text = txt;
        }
        public Image LoadPbFromStream(string fpathname)  // called ONLY from fileWatch  revise (merge into code) and delete 
        {

            //this.Text = fpathname;

            // Open each file and display the image in pictureBox1.
            // Call Application.DoEvents to force a repaint after each
            // file is read.        
            System.IO.FileInfo fileInfo;
            System.IO.FileStream fileStream2 = null;

            fileInfo = new System.IO.FileInfo(@fpathname);
            // error across thread     this.Text = fpathname;
            try
            {
                fileStream2 = fileInfo.OpenRead();
            }
            catch (global::System.Exception e)
            {
                string errorFile = fpathname;
                bool test = fileInfo.Exists;
                if (test)
                    gv.debug.w("file does exist ", fpathname);
                else
                {
                    gv.debug.w("file does not exist ", fpathname);
                    fpathname = fpathname.Replace("\\", "/");
                    fileInfo = new System.IO.FileInfo(@fpathname);
                    try
                    {
                        fileStream2 = fileInfo.OpenRead();
                    }
                    catch (global::System.Exception e2)
                    {
                        gv.debug.w("display1 file does not exists TWICE ", e2.ToString());
                        return null;
                    }
                }
                //return;
            }
            bool bLoadedImage = true;
            try
            {
                pb1.Image = System.Drawing.Image.FromStream(fileStream2);
            }
            catch (global::System.Exception e)
            {
                gv.debug.w("ERROR READING filestream into PB ", fpathname);
                MessageBox.Show("ERROR loading image ",  fpathname);
                bLoadedImage = false;
            }
            if (bLoadedImage)
            {
                image = pb1.Image;
                originalSize = image.Size;      
            }

            Application.DoEvents();
            try
            {
                fileStream2.Close();
            }
            catch (global::System.Exception e)
            {
                gv.debug.w("display1 fileStream2 close fail ", e.ToString());
               // return;
            }

            if (bLoadedImage)
            {
                //resizePictureBox(false);
                //this.resizeBestFit();   can't call across processes so
            }
            //   displayThisImage(image);
            //this.resizeBestFit();

            // Call Sleep so the picture is briefly displayed, 
            //which will create a slide-show effect.
            //pictureBox1.Image = null;
            return pb1.Image;
        }
        public void SetFullScreenMode()
        {

        }
        public Boolean displayThisImage(FileInfoItem iinfo2)
        {
            if (bHoldThisImage)
                return false;

            finfo = iinfo2;

            return displayThisImage(finfo.fpath);
        }

        public bool bHoldThisImage = false;

        public Boolean displayThisImage1(string fpath) //// DISPLAY IMAGE after LOADING <<<<<<<<<<<<<<<<<< MAIN CALL TO DISPLAY THIS IMAGE << LOAD AND DISPLAY FPATH image
        {
            if (bHoldThisImage)
                return false;
            try
            {
                pb1.Load(fpath);//openFileDialog1.FileName);
            }
            catch (Exception)
            {
                pb1.Image = null;
                gv.debug.w("Error File not found ", fpath);
                return false;

            }
        //    resizeZoom();
            return true;
        }
        Image thisImage;
        public void displayThisImage(Image img)  ///////////// DISPLAY IMAGE      1st explicit display after open window
        {
            thisImage = img;
            if (bHoldThisImage)
                return;

            if (img == null)
            {
                image = null;
                return;
            }
            else
            {

                image = img;
                originalSize = img.Size;
            }
            //setFullScreenMode(true);
            this.resizePictureBox(true);
            pb1.Image = image;
            resizeBestFit();
            pb1.BorderStyle = BorderStyle.Fixed3D;//.None;
            this.Text = "direct image";
            this.Refresh();
        }

        public void displayThisImage2(string fpath)
        {
            if (bHoldThisImage)
                return;

            finfo = new FileInfoItem(fpath);

            image = pb1.Image;
            originalImage = image;
            originalSize = image.Size;
            //this.resizeImage();
            this.resizePictureBox(true);
            this.Text = buildPictureLabel(finfo) + " " + displayName; /////////////////  IMAGE NAME SIZE PATH  ////////////////
            
        }
        public Boolean displayThisImage(string fpath) //// DISPLAY IMAGE after LOADING <<<<<<<<<<<<<<<<<< MAIN CALL TO DISPLAY THIS IMAGE << LOAD AND DISPLAY FPATH image
        {
            //setFullScreenMode(true);
            if (bHoldThisImage)
                return false;

            try
            {
                pb1.Load(fpath);//openFileDialog1.FileName);
            }
            catch (Exception)
            {
                pb1.Image = null;
                gv.debug.w("Error File not found " ,  fpath);
                return false;

            }
            //FileInfoItem(string fname2, string fullpath, string dir, string ext2, string type2, int level2, long len2, DateTime datetime2, string ts2)
            finfo = new FileInfoItem(fpath);

            image =  pb1.Image;
            originalImage = image;
            originalSize = image.Size;
            //this.resizeImage();
            this.resizePictureBox(true);
            this.Text = buildPictureLabel(finfo) + " " + displayName; /////////////////  IMAGE NAME SIZE PATH  ////////////////
            return true;
        }

        public void displayThisImage(Image img, string title) ///////// DISPLAY IMAGE 
        {
            if (bHoldThisImage)
                return;

            image = img;
            originalSize = img.Size;
            //setFullScreenMode(true);
            this.resizePictureBox(true);
            
             pb1.Image = img;
            resizeBestFit();
            this.Text = title;
        }

        public void clearDisplay()
        {
            pb1.Image = null;
           // this.Hide();
        }

        public bool setCopyMode(bool bCopy)
        {
            gv.debug.w(">>>> ONLY COPY IMAGES IN MAIN WINDOWS ... disabled in Display1 windows");
            bCopyMode = bCopy;
            return bCopyMode;
        }
        public string buildPictureLabel(FileInfoItem fi)
        {
            string title = String.Format("{0:###############} {1:###,###,###}    fpath = {2}", fi.fname, fi.len, fi.fpath);
            return title;
        }
        public void setMode(D1function d1mode2)
        {
            displayMode = d1mode2;

            
        }
        public D1function getDisplayMode()
        {
            return displayMode;
        }
        private void loadInitialImage(string imgPath)
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(imgPath));
            Image img = Image.FromStream(ms); //copy frees any locks on original file
            ms.Dispose();

            //   Image Columns should be defined as OleDbType.Binary.
        }
        private void Display1_Load(object sender, EventArgs e)
        {

        }

        public void setFullScreenMode(Boolean full)
        {
            if (full)
            {
              //  if (md != null)
                //     this.Bounds = md.screen.Bounds;
                //this.Bounds = Screen.PrimaryScreen.Bounds;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
                resizeBestFit();
                
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                //this.FormBorderStyle = FormBorderStyle.
                this.WindowState = FormWindowState.Normal;
                resizeBestFit();
            }

        }



        public void imageName(string title)
        {
            this.Text = title;
        }

        /*
         * PictureBoxSizeMode.
         * AutoSize - the PB is RESIZED to match the image size
         * CenterImage - clipped if too large, bordered if smaller than PB
         * Normal - placed upper left of PB and is clipped if image is too large
         * Zoom - maintaining the size RATIO  make image fit (BESTFIT)
         * StretchImage - to fit pb
         * */

        private void resizeActual()
        {
            if (bLockAutoSize)
                return;
             pb1.Image = image;
            pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            
            if (image != null)
            {
                Graphics gdi = Graphics.FromImage(image);//this.pb.Image);
            }
        }
        private void resizeBestFit()
        {
            // gv.bResize = true;
             pb1.Image = image;
            resizeImage(true);
        }
        private void resizeImage2()
        {
            // gv.bResize = false;
            pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            return;
        }
        private void resizeImage(Boolean bestFit)
        {
            if (bLockAutoSize)
                return;
            pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            if (bestFit)
            {
                //  gv.bResize = true;
                pb1.SizeMode = PictureBoxSizeMode.Zoom; //keeps aspect ratio
            }
            else
            {
                //  gv.bResize = true;
                pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            if (bestFit)
            {
                if (gv.debugLevel > 5)
                gv.debug.w("resize bestFit");// String.Format("resize bestFit {0} has been accessed {1} times.", _principal.Identity.Name, _nAccesses);



                if (image != null)
                {
                    Graphics gdi = Graphics.FromImage(image);//this.pb.Image);
                }
                pb1.SizeMode = PictureBoxSizeMode.Zoom; //keeps aspect ratio
                //RectangleF rec = pb.Image.
            }

            else
            {
                // pb.SizeMode = PictureBoxSizeMode.Normal;
                gv.debug.w("resize custom");
                if (image != null)
                {
                    Graphics gdi = Graphics.FromImage(image);//this.pb.Image);
                }
                pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            // pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // fill
            pb1.BorderStyle = BorderStyle.None; // BorderStyle.Fixed3D;

        }

        private void backgroundButton_Click(object sender, EventArgs e)
        {
            // Show the color dialog box. If the user clicks OK, change the
            // PictureBox control's background to the color the user chose.
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                pb1.BackColor = colorDialog1.Color;

        }

        private void resizeFullScreen()
        {
            resizePictureBox(true);
            //  this.setFullScreenMode(false);
        }

        public void repaint()
        {
            this.Update();
        }

       

       
        /// <summary>
        /// ///////////////////////////////////////   Picture Box   OUTLINE Rubber Band lasso FUNCTIONS ////////////////////////////////
        /// </summary>
        /// 
        public void eraseOutline()
        {
            this.disposeTag();
            //MyDrawReversibleRectangle(ptOriginal, ptLast);
        }
        public void cancelOutlining()  //////////// CANCEL
        {
            if (!this.showingTag)
                return;
            if (ptLast.X != -1)
            {
                MyDrawReversibleRectangle(ptOriginal, ptLast);
            }
            HaveMouse(false);
            // Set flags to know that there is no "previous" line to reverse.
            HaveMouse(false);
            ptLast.X = -1;
            ptLast.Y = -1;
            ptOriginal.X = -1;
            ptOriginal.Y = -1;
        }/// <summary>
        /// //////////////////  ANNOTATION ---------------- ANNOTATION ---------------- ANNOTATION ---------------- ANNOTATION ----------------
        /// </summary>
        /// <param name="e"></param>
        private void startAnnotationOutline(MouseEventArgs e)
        {
            if (showingTag)
            {
                this.disposeTag();
            }
            if (bHaveMouse)
                return;
            // Make a note that we "have the mouse".
            HaveMouse(true);
            // Store the "starting point" for this rubber-band rectangle.
            ptOriginal.X = e.X;
            ptOriginal.Y = e.Y;
            // Special value lets us know that no previous
            // rectangle needs to be erased.
            ptLast.X = -1;
            ptLast.Y = -1;
        }


        // Called when the left mouse button is released. -------------------- not used... use above PB mouseup
        private void pb1_MouseDown(object sender, MouseEventArgs e)
        {
            if (displayMode != D1function.ANNOTATE || e.Button != MouseButtons.Left)
                return;

            // Clear any previous tag so showTag() works on the next selection
            if (showingTag)
                disposeTag();

            _rbDragging = true;
            _rbStart = e.Location;
            _rbEnd = e.Location;
            _rbRect = Rectangle.Empty;
            pb1.Invalidate();
        }

        private void pb1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_rbDragging) return;
            _rbEnd = e.Location;
            _rbRect = new Rectangle(
                Math.Min(_rbStart.X, _rbEnd.X),
                Math.Min(_rbStart.Y, _rbEnd.Y),
                Math.Abs(_rbEnd.X - _rbStart.X),
                Math.Abs(_rbEnd.Y - _rbStart.Y)
            );
            // Invalidate marks dirty; Update flushes it immediately —
            // without Update the paint queue backs up during fast drags
            // and the rubber-band never appears while moving.
            pb1.Invalidate();
            pb1.Update();       // <-- force synchronous repaint NOW
        }

        private void pb1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_rbDragging) return;
            _rbDragging = false;
            pb1.Invalidate();
            pb1.Update();       // erase the last rubber-band immediately

            if (_rbRect.Width > 0 && _rbRect.Height > 0)
            {
                var capture = cropSelection(_rbRect);
                if (capture != null)
                {
                    captured = capture;
                    gv.debug.displayThisImage(capture);
                    showTag(_rbRect);
                    openGetInfoDialog();
                }
            }
        }

        private void pb1_Paint(object sender, PaintEventArgs e)
        {
            if (!_rbDragging || _rbRect.IsEmpty) return;

            // Two-layer pen: black outer + white inner for visibility on any image
            using var outerPen = new Pen(Color.Black, 2) { DashStyle = DashStyle.Dash };
            using var innerPen = new Pen(Color.White, 1) { DashStyle = DashStyle.Dash };
            e.Graphics.DrawRectangle(outerPen, _rbRect);
            e.Graphics.DrawRectangle(innerPen, Rectangle.Inflate(_rbRect, -1, -1));
        }

        private Boolean openGetInfoDialog()
        {
            // Use _rbRect — the new system's rectangle — not the old ptOriginal/ptLast vars
            tagInfo[tagCount++] = new ImageInfo(finfo, _rbRect);

            infod = new DialogGetInfo(_rbRect, _rbRect, this, finfo, captured);
            dialogLocation = gv.mainWindow.Location;
            infod.Location = dialogLocation;
            infod.TopMost = true;
            infod.Visible = true;
            infod.Activate();
            return true;
        }

        private Boolean captureTagBounds()
        {
            outlineBox.X = ptOriginal.X;
            outlineBox.Y = ptOriginal.Y;
            outlineBox.Width = ptLast.X;
            outlineBox.Height = ptLast.Y;

            outlineBounds.X = ptOriginal.X;
            outlineBounds.Y = ptOriginal.Y;
            outlineBounds.Width = ptLast.X - ptOriginal.X;
            outlineBounds.Height = ptLast.Y - ptOriginal.Y;
            return true;
        }

        /// <summary>
        /// Given a PictureBox in Zoom mode, returns the rectangle (in client coords)
        /// where the actual image is drawn inside that control.
        /// </summary>
        private Rectangle GetImageDisplayRect(PictureBox pb)
        {
            if (pb.Image == null) return Rectangle.Empty;
            var img = pb.Image;

            // original and client sizes
            int iw = img.Width, ih = img.Height;
            int cw = pb.ClientSize.Width, ch = pb.ClientSize.Height;

            // compute scale
            float ratio = Math.Min((float)cw / iw, (float)ch / ih);
            int scaledW = (int)(iw * ratio);
            int scaledH = (int)(ih * ratio);

            // center it
            int offsetX = (cw - scaledW) / 2;
            int offsetY = (ch - scaledH) / 2;
            return new Rectangle(offsetX, offsetY, scaledW, scaledH);
        }

        private Bitmap cropSelection(Rectangle selRect)
        {
            // 1) where is the image drawn inside pb1?
            Rectangle imgRect = GetImageDisplayRect(pb1);
            // 2) intersect with what the user dragged
            selRect = Rectangle.Intersect(imgRect, selRect);
            if (selRect.IsEmpty) return null;

            // 3) map that back to source‐image coordinates
            float scaleX = (float)pb1.Image.Width / imgRect.Width;
            float scaleY = (float)pb1.Image.Height / imgRect.Height;

            var srcRect = new Rectangle(
              (int)((selRect.X - imgRect.X) * scaleX),
              (int)((selRect.Y - imgRect.Y) * scaleY),
              (int)(selRect.Width * scaleX),
              (int)(selRect.Height * scaleY)
            );

            // 4) pull that pixel‐perfect region out of the bitmap
            var bmp = new Bitmap(srcRect.Width, srcRect.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(pb1.Image,
                            new Rectangle(0, 0, bmp.Width, bmp.Height),
                            srcRect, GraphicsUnit.Pixel);

                // 5) (optional) draw a red border on it
                using var pen = new Pen(Color.Red, 2);
                g.DrawRectangle(pen, 0, 0, bmp.Width - 1, bmp.Height - 1);
            }

            return bmp;
        }



        

        public void reshowTag(Rectangle bounds)
        {
            if (useTagpb)
                tagpb1.Bounds = bounds;
            else
                tag.Bounds = bounds;
        }

        public void hideTag()
        {
            tagpb1.Hide();
        }

        // SHOW ANNOTATION TAGPG ----------------------------
        public void showTag(Rectangle bounds)
        {
            if (this.useTagpb && !this.showingTag)
            {
                this.showingTag = true;
                this.tagpb1.Show();
                

                //offset is the difference from Screen/Client
                // Point offset = PointToScreen(new Point(bounds.X, bounds.Y));
                //  bounds.X += offsetScreenX;
                //   bounds.Y += offsetScreenY;


                tagpb1.Bounds = bounds;// (new Rectangle(109, 109, bounds.Width, bounds.Height));//bounds;
                tagpb1.Visible = true;
                tagpb1.Update();
                this.Update();
                // MessageBox.Show("loaded okay");
            }
            else
            {
                tag = new TagPB(bounds);
                //    tag.setTag(bounds);
                tag.Visible = true;
                // tag.Location = (new Point(bounds.X, bounds.Y));
            }

        }


        public int showAllTags()
        {
            contextMenuStrip1.Hide();
            this.Refresh();
            for (int idx = 0; idx < tagCount; ++idx)
            {
               redrawTag( tagInfo[idx].bounds);
            }
            return tagCount;
        }
        // Convert and normalize the points and draw the reversible frame.
        private void redrawTag(Rectangle rc)
        {

            this.Focus();
            // Draw the reversible frame.
           // ControlPaint.DrawReversibleFrame(rc,
                //                            Color.Red, FrameStyle.Dashed);
            //                Color.White, FrameStyle.Thick);
            //                            Color.White, FrameStyle.Dashed);
            Point p1 = new Point(rc.X, rc.Y);
            Point p2 = new Point(rc.X + rc.Width, rc.Y + rc.Height);
            MyDrawReversibleRectangle(p1, p2);
            OutlineRectangle(p1, p2);
        }

        // Convert and normalize the points and draw the reversible frame.
        private void MyDrawReversibleRectangle(Point p1, Point p2)
        {
            Rectangle rc = new Rectangle();

            // Convert the points to screen coordinates.
            p1 = PointToScreen(p1);
            p2 = PointToScreen(p2);
            // Normalize the rectangle.
            if (p1.X < p2.X)
            {
                rc.X = p1.X;
                rc.Width = p2.X - p1.X;
            }
            else
            {
                rc.X = p2.X;
                rc.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                rc.Y = p1.Y;
                rc.Height = p2.Y - p1.Y;
            }
            else
            {
                rc.Y = p2.Y;
                rc.Height = p1.Y - p2.Y;
            }
            // Draw the reversible frame.
            ControlPaint.DrawReversibleFrame(rc,
                            Color.Black, FrameStyle.Dashed);
            ///             Color.Black, FrameStyle.Thick);
            //                            Color.Red, FrameStyle.Dashed);
            //                            Color.White, FrameStyle.Dashed);
        }

        // DRAW AROUND THE TAG
        private void OutlineRectangle(Point p1, Point p2)
        {
            Rectangle rc = new Rectangle();

            // Convert the points to screen coordinates.
            p1 = PointToScreen(p1);
            p2 = PointToScreen(p2);
            p1.X -= 2;
            p1.Y -= 2;
            p2.X += 2;
            p2.Y += 2;

            // Normalize the rectangle.
            if (p1.X < p2.X)
            {
                rc.X = p1.X;
                rc.Width = p2.X - p1.X;
            }
            else
            {
                rc.X = p2.X;
                rc.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                rc.Y = p1.Y;
                rc.Height = p2.Y - p1.Y;
            }
            else
            {
                rc.Y = p2.Y;
                rc.Height = p1.Y - p2.Y;
            }
            // Draw the reversible frame.
            ControlPaint.DrawReversibleFrame(rc,
                //                            Color.Red, FrameStyle.Dashed);
                            Color.Black, FrameStyle.Thick);
            //                            Color.White, FrameStyle.Dashed);
        }



        // Called when the mouse is moved.
        public void MyMouseMove(Object sender, MouseEventArgs e)
        {
            Point ptCurrent = new Point(e.X, e.Y);
            // If we "have the mouse", then we draw our lines.
            if (bHaveMouse)
            {
                // If we have drawn previously, draw again in
                // that spot to remove the lines.
                if (ptLast.X != -1)
                {
                    MyDrawReversibleRectangle(ptOriginal, ptLast);
                }
                // Update last point.
                ptLast = ptCurrent;
                // Draw new lines.
                MyDrawReversibleRectangle(ptOriginal, ptCurrent); /////////////////// draw tag box
            }
        }

        public void MyMouseDown(Object sender, MouseEventArgs e)
        {
            // Make a note that we "have the mouse".
            HaveMouse(true);
            SystemSounds.Beep.Play();
            
            // Store the "starting point" for this rubber-band rectangle.
            ptOriginal.X = e.X;
            ptOriginal.Y = e.Y;
            // Special value lets us know that no previous
            // rectangle needs to be erased.
            ptLast.X = -1;
            ptLast.Y = -1;
        }
        
        private void pb_MouseClick(object sender, MouseEventArgs e)
        {
            MyMouseDown(sender, e);


        }//slideShow // Set up delegates for mouse events.
        protected override void OnLoad(System.EventArgs e)
        {
           // MouseDown += new MouseEventHandler(MyMouseDown);
          //  MouseUp += new MouseEventHandler(MyMouseUp);
            //  MouseMove += new MouseEventHandler(MyMouseMove);
            HaveMouse(false);
        }
        /// <summary>
        /// ///////////////////////////////////end of SELECT Rubber Band Lasso functions////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display1_Resize(object sender, EventArgs e)
        {
            int X, Y;
            if (this.pb1.Width < this.ClientSize.Width)
                X = (this.ClientSize.Width - this.pb1.Width) / 2;
            else
                X = -this.HorizontalScroll.Value;
            if (this.pb1.Height < this.ClientSize.Height)
                Y = (this.ClientSize.Height - this.pb1.Height) / 2;
            else
                Y = -this.VerticalScroll.Value;

            this.pb1.Location = new Point(X, Y);



        }


        //was private void picFrame_LoadCompleted(object sender, AsyncCompletedEventArgs e)

        bool bLockAutoSize = false;

        public void setPictureBoxSize(PictureBoxSizeMode sm)
        {
            if (sm == PictureBoxSizeMode.AutoSize)
                bLockAutoSize = true;
            pb1.SizeMode = sm;
            this.Refresh();
        }
        private void resizePictureBox(Boolean max)
        {
            if (bLockAutoSize)
                return;
            pb1.Height = this.Height;
            pb1.Width = this.Width;
   
            if (!max)
            {

                if (pb1.Height < this.Height || pb1.Width < this.Width)
                {

                    pb1.Dock = DockStyle.Fill;

                    pb1.SizeMode = PictureBoxSizeMode.CenterImage;

                }

                else
                {

                    pb1.Dock = DockStyle.None;

                    pb1.SizeMode = PictureBoxSizeMode.AutoSize;

                }
            }
           
        }
        private void pb_DoubleClick(object sender, EventArgs e)
        {
            //this.Location.X = gdi.CopyFromScreen(bounds.Left - offsetScreenX, bounds.Top - offsetScreenY, 0, 0, bounds.Size);
            pb1.Image = thisImage;
        }

        private void pb_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            gv.debug.write("pb mouse double clock ");

        }

        /*
         * if a user presses a key and keeps it pressed, ProcessCmdKey keeps triggering WM_KEYDOWN events.
         * You might want to override ProcessKeyPreview() instead, which handles all the keyboard messages received by child controls.
         * */
        /*
         * dmcdmcdmc   1/20/2013
    * through overriding the ProcessCmdKey() method of the form. That way, 
     * your key handling logic gets executed no matter what control 
    * has focus at the time of keypress. 
    * Beside that, you even get to choose whether the focused control gets the key 
    * after you processed it (return false) or not (return true).
         * 
         * Keys.Shift | Keys.Control | Keys.F1
         * 
         *   F5 reserved by Windows 10
         * IF YOU PROCESSED THE EVENT KEY - set  RETURN FALSE!!!
    * */
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) /////// override dmcdmc
        {
            if (keyData == Keys.Left)
            {
                gv.debug.w("Left  in Display1");

                gv.mainWindow.stopSlideShow();
                gv.mainWindow.showPreviousSlide();
                return false; //for the active control to see the keypress, return false
            }
            else if (keyData == (Keys.Alt | Keys.PageDown))
            {
                gv.mainWindow.scanForward(10);
                gv.debug.w("------------------------alt pagedown");
                return true; 
            }
            else if (keyData == (Keys.Control | Keys.PageDown))
            {
                gv.mainWindow.scanForward(500);
                gv.debug.w("--------------------------cntl pagedown");
                return true; 
            }
            else if (keyData == ( Keys.PageDown))
            {
                gv.mainWindow.scanForward(1000);
                gv.debug.w("-----------------------pagedown");
                return true; 
            }
            else if (keyData == Keys.Right)
            {
                gv.debug.w("showNextSlide2  -----  RIGHT  in Display1");
                gv.mainWindow.showNextSlide3(gv.nextIdx, 1); //right
                return false; //for the active control to see the keypress, return false
            }
          // else if (keyData == Keys.Down)
          // {
            //    gv.debug.w("DOWN  in Display1");
              // gv.mainWindow.scanForward(3000);
              // return true; //for the active control to see the keypress, return false
          // }
           // else if (keyData == Keys.Up)
          // {
           //     gv.debug.w("UP  in Display1");

            //    return true; //for the active control to see the keypress, return false
          //  }
            else if (keyData == Keys.Escape)
            {
                //Application.Exit();
                this.Close();
                return true;
            }
            else if (keyData == Keys.Home)
            {
                gv.mainWindow.Activate();
            }
            /*else if (keyData == Keys.Back)
            {
                autoZoom();
            }*/
            if (keyData >= Keys.A && keyData <= Keys.Z) //COPY image to C:\\aImages\x  folder //////////////////////////////// COPY from main windows  A-Z 0-9
            {
                gv.debug.w(" key press in Display1", keyData.ToString());
                //gv.mainWindow.copyImage((char)keyData);
                return true;
            }
            else if (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9)
            {
                if (bCopyMode)
                {
                    gv.debug.w("bCopyMode key press in Display1", keyData.ToString());
                    gv.mainWindow.copyOrMoveImage((char)keyData);
                }
                
                return true;

            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// ///////////////////// KEYPRESS //////////////////////////////dmcdmc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key_char = e.KeyChar;
            string key = e.ToString();
            // if (key_char.Equals('Q'))
               // if (gv.browser1 != null)
                 //   gv.browser1.stopAll();
            return;
        }
        public void dmc1(object sender, KeyPressEventArgs e)
        {
            char key_char = e.KeyChar;
            string key = e.ToString();
            gv.debug.w("Display1 KeyPress >> ", e.KeyChar);

            Cursor.Show();
            Cursor.Position = new Point(111, 111);

            if (displayMode == D1function.CATALOG) ///////////////////////// catalog function allows MOVE image 
            {
               // startSoundPlayer.Play();
                if (key_char >= 'a' && key_char <= 'z') //move image to C:\\aImages\x  folder
                {
                    gv.mainWindow.moveImage(key_char);
                    return;
                }
                if (key_char >= '0' && key_char <= '9')
                {
                    gv.mainWindow.moveImage(key_char);
                    return;
                }
                return;
            }

            
            //----------------------------- keys handled in KeyPRESS 
            //keys
            //     + n q s b ENTER DOWN UP LEFT RIGHT TAB ESCAPE 

            CopyInfo rc;
            if (bCopyMode) ////////////////////////////////////////////////////////////////////////// bCopyMode COPY image 
            {
                if ((key_char >= 'a' && key_char <= 'z') || (key_char >= '0' && key_char <= '9'))
                {
                    rc = ff.CopyFileArchive(finfo.fpath, Path.Combine(gv.targetFolder, key_char.ToString()));
                    this.Text = "COPY " + rc.fname + " FROM " + rc.source + " >> " + rc.target;
                    //gv.mainWindow.message(this.Text);
                    // gv.main2.stopSlideShow();
                    if (rc.success)
                        gv.mainWindow.showNextNoTimer();
                    return;
                }
            }
            
            // NOT IN COPYMODE (bCopyMode = false)-------------
            gv.debug.w(this.Name,  key_char);
            switch (key_char)
            {
                case (char)Keys.Add:
                    //  gv.bResize = false;
                    this.resizeZoomIn();
                    break;
                //note use KeyUp for Keys.Right:
                //Keys.Left etc.
                case 'n':
                    // startSoundPlayer.Play();
                    //gv.debug.write("right arrow key pressed");
                    gv.mainWindow.showNextSlideNow();
                    break;
                case 'q':
                    gv.mainWindow.stopSlideShow();
                    break;
                case 's': // START slideShow
                    gv.debug.w("Display1 keypress s");
                    setSlideShow(true);
                    gv.mainWindow.showNextSlideNow();
                    break;
                case 'b':
                    gv.mainWindow.showPreviousSlide();
                    break;
                case (char)Keys.Enter:
                    //   MessageBox.Show("ENTER KEY PRESSED");
                    break;
               // case (char)Keys.Down:

                  //  break;

               // case (char)Keys.Up:
                    //   Console.WriteLine("Up Arrow Captured");
                    break;

                case (char)Keys.Tab:
                    //  Console.WriteLine("Tab Key Captured");
                    break;

                case (char)Keys.Escape: ////////////////////////////////////////  CLOSE D1 Window //////////
                    //  Console.WriteLine("Escape Key Captured");
                    ////////don't put print in here ... focus change hangs program ///////gv.debug.w(e.KeyChar.ToString());
                    if (bHaveMouse)
                    {
                        cancelOutlining();
                    }
                    else
                    {
                        gv.mainWindow.stopSlideShow();
                        //gv.main2.displayThisImage(null);
                        gv.mainWindow.Focus();
                        this.Close();
                    }
                    break;
                case (char) Keys.Right:
                    //gv.debug.write("right arrow key (PRESS) ");
                    gv.mainWindow.showNextSlideNow();
                    break;
                default:

                    break;
            }

        }
        public bool setSlideShow(bool bOn)
        {
            //gv.bSlideShow = bOn;
            return gv.mainWindow.setSlideShowOn(bOn);
            //cbSlideShow.Checked = bOn;
            //return gv.bSlideShow;
        }
        private void Display1_KeyUp(object sender, KeyEventArgs e)
        {
            Cursor.Show();
            Cursor.Position = new Point(111, 111);

            Keys keyData = e.KeyCode;// e.KeyChar;
            if (gv.debugLevel > 1)
                return;
            gv.debug.w("Display1 keyUp");

            switch (keyData) //list of keys not to send to debug window
            {
                //  Keys. ----------------------- DO NOT HANDLE a-z, A-Z here !!!
                case Keys.F5:
                    break;
                case Keys.Right:
                    break;
                default:
                    gv.debug.w(this.displayName, e.KeyData.ToString());
                    break;
            }
 

            switch (keyData)
            {
                  //  Keys. ----------------------- DO NOT HANDLE a-z, A-Z !!!
                case Keys.Escape:
                    gv.mainWindow.Activate();
                    break;
                case Keys.LaunchMail:
                    gv.mainWindow.setCopyMode(true);
                    break;
                case Keys.PageDown:
                     gv.debug.write("Display1 >> PAGE DOWN key (up) ");

                     if (e.Control)
                         gv.mainWindow.scanForward(1000);

                     else if (e.Alt)
                         gv.mainWindow.scanForward(5000);
                     else
                        gv.mainWindow.scanForward(1);
                    break;
                case Keys.PageUp:
                    gv.debug.write("Display1 >> PAGE UP key (up) ");
                    setSlideShow(true);
                    gv.mainWindow.showNextSlideNow();
                   
                    break;
                case Keys.Add:
                    //  gv.bResize = false;
                    this.resizeZoomIn();
                    break;
               // case Keys.Down:
                case Keys.Subtract:
                    //   gv.bResize = false;
                    this.resizeZoomOut();
                    break;
                case Keys.Multiply:
                    //  gv.bResize = false;
                    this.resizeBestFit();
                    break;
                case Keys.Divide:
                    //  gv.bResize = false;
                    this.resizeActual();
                    break;
                case Keys.Enter:
                    // MessageBox.Show("ENTER KEY UP");
                    break;

                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    gv.debug.w("NumPad IN DISPLAY1 ");
                    gv.mainWindow.initTimer((int) keyData);
                    break;
                case Keys.Tab:
                    //   Console.WriteLine("Tab Key Captured");
                    break;
                case Keys.Right:////////////-------------------------------------------------------------------------------------start slide show----------------
                    gv.debug.write("Display1 >> right arrow key (up) ");
                //    gv.mainWindow.showNextSlide2(-1, 1);
                    break;
                case Keys.Left:
                    setSlideShow(false);
                    if (displayMode == D1function.ANNOTATE)
                    {
                        gv.debug.w("-- catalog mode can not back up to previous ");
                    }
                    else
                    {
                  //      gv.debug.write("left arrow key (up) ");
                        gv.mainWindow.showPreviousSlide();
                    }
                    break;


                //case (char) Keys.Control | (char) Keys.M:
                //     Console.WriteLine("<CTRL> + m Captured");
                //    break;

                // case (char) Keys.Alt | (char) Keys.Z:
                //     Console.WriteLine("<ALT> + z Captured");
                //     break;
            }

        }

        private void Display1_FormClosing(object sender, FormClosingEventArgs e)
        {
            gv.mainWindow.stopSlideShow();
            if (fileWatcher != null)
                fileWatcher.OnStop();
            if (md != null)
                gv.mainWindow.closingThisDisplay(md.screenNumber);
        }
        public Bitmap ResizeBitmap(Image originalImage, int width, int height)
        {
            //Image originalImage = Image.FromFile(filepath);

            Bitmap result;
            try
            {
                result = new Bitmap(width, height);
            }
            catch
            {
                return null;
            }
            using (Graphics g = Graphics.FromImage(result))
            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                if (originalImage == null)
                    g.DrawImage(image, 0, 0, width, height);
                else
                    g.DrawImage(originalImage, 0, 0, width, height);
                // Fill a part of the result image with a specific color.
                // Rectangle toFill = new Rectangle(0, 0, width / 2, height / 2);
                // g.FillRectangle(brush, toFill);
            }

            return result;
        }

        int offx = -300;
        int offy = -300;

        int zoomCount = 0;

        PictureBox pictureBox1 = new PictureBox();

        private void Zoom(int xx, int yy, int ww, int hh)
        {
            if (zoomCount == 0)
            {
                //pictureBox1.Size = new Size(210, 110);
                pictureBox1.BorderStyle = BorderStyle.Fixed3D;
                pictureBox1.SizeMode = PictureBoxSizeMode.Normal;//.CenterImage;//.Zoom;//.CenterImage;

                pictureBox1.SetBounds(100, 100, 800, 800);
                this.Controls.Add(pictureBox1);
                pb1.Visible = false;
                pictureBox1.Visible = true;
            }

            //Create a graphics object and draw a portion of the image in the PictureBox.
            Graphics g = pictureBox1.CreateGraphics();

            int xWidth = pictureBox1.Width;
            int yHeight = pictureBox1.Height;

            int x;
            int y;


           pictureBox1.Image = image;
           pictureBox1.Image = ResizeBitmap(image, image.Width * 2, image.Height * 2);  //(int) size.Width * 1.1, (int)size.Height * 1.1);
                                                                                        //
           image = pictureBox1.Image;

            g.DrawImage(pictureBox1.Image,
              new Rectangle(-300, -300, xWidth, yHeight),  //where to draw the image
              new Rectangle(200, 200,  xWidth, yHeight),  //the portion of the image to draw
              GraphicsUnit.Pixel);

            pictureBox1.Invalidate();
            pictureBox1.Update();

            this.Update();

                zoomCount += 1;

        }

//        float zoomFactor = 1.0051;
        double zoomFactor = 1.051 * 1;

        private void resizeZoomIn()
        {
            if (bLockAutoSize)
                return;
            Size size =  pb1.Image.Size;
            //pb1.SizeMode = PictureBoxSizeMode.Normal;   //.CenterImage;
            gv.debug.w(size.Width.ToString() + " " + size.Height.ToString());
            double Width = pb1.Image.Width * 1.051;

            double Height = pb1.Image.Size.Height * 1.051;

              pb1.Image = ResizeBitmap(image, (int)Width, (int)Height);  //(int) size.Width * 1.1, (int)size.Height * 1.1);
            ///

            size =   pb1.Image.Size;
            gv.debug.w("ZoomIN new size >>" + size.Width.ToString() + " " + size.Height.ToString());

           

            //if (zoomCount == 0)
            {
               // pb1.SizeMode = PictureBoxSizeMode.Normal;
                pb1.Update();
            //image = pb1.Image;
            }

            offx -= 111;
            offy -= 111;

              //Zoom(offx, offy, Convert.ToInt32(Width * 2), Convert.ToInt32(Height * 2));
        }

        //             Matrix transform = new Matrix();

        private void resizeZoomOut()
        {
            Size size = pb1.Image.Size;
            pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            gv.debug.w(size.Width.ToString() + " " + size.Height.ToString());
            double Width = pb1.Image.Width * 1.0051;
            Width = pb1.Image.Width * (.95);
            double Height = pb1.Image.Size.Height * 1.0051;
            Height = pb1.Image.Size.Height * (.95);
            pb1.Image = ResizeBitmap(image, (int)Width, (int)Height);  //(int) size.Width * 1.1, (int)size.Height * 1.1);
            ///

            size = pb1.Image.Size;
            gv.debug.w("ZoomOUT new size >>" + size.Width.ToString() + " " + size.Height.ToString());

        }

        private void resizeZoomOut2()
        {
            if (bLockAutoSize)
                return;

            Size size =  pb1.Image.Size;
            pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            gv.debug.w(size.Width.ToString() + " " + size.Height.ToString());
            double Width = ( pb1.Image.Width * .95);

            double Height = ( pb1.Image.Size.Height * .95);

             pb1.Image = ResizeBitmap(image, (int)Width, (int)Height);  //(int) size.Width * 1.1, (int)size.Height * 1.1);
            ///
            size =  pb1.Image.Size;
            gv.debug.w("new size " + size.Width.ToString() + " " + size.Height.ToString());

            pb1.Update();
        }

        private Bitmap MyImage;
        public void ShowMyImage(String fileToDisplay, int xSize, int ySize)
        {
            // Sets up an image object to be displayed.
            if (MyImage != null)
            {
                MyImage.Dispose();
            }
            if (bLockAutoSize)
                return;

            // Stretches the image to fit the pictureBox.
            pb1.SizeMode = PictureBoxSizeMode.StretchImage;
            MyImage = new Bitmap(fileToDisplay);
            pb1.ClientSize = new Size(xSize, ySize);
             pb1.Image = (Image)MyImage;
        }

        private void resizeImage(double width2, double height2)
        {
            if (bLockAutoSize)
                return;

            Size size =  pb1.Image.Size;
           // pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            pb1.SizeMode = PictureBoxSizeMode.Normal;
            gv.debug.w(size.Width.ToString() + " " + size.Height.ToString());
            double Width = ( pb1.Image.Width * width2);

            double Height = ( pb1.Image.Size.Height * height2);

             pb1.Image = ResizeBitmap(image, (int)Width, (int)Height);  //(int) size.Width * 1.1, (int)size.Height * 1.1);
            ///

            pb1.ClientSize = new Size(100, 100);

            size =  pb1.Image.Size;
            gv.debug.w("new size " + size.Width.ToString() + " " + size.Height.ToString());

            pb1.Update();
        }
        private void pb_Click(object sender, EventArgs e)
        {
          //  if (displayMode == D1function.SLIDESHOW)
            //    if (gv.main2.showNextSlideNow())
              //      Cursor.Hide();

            
        }
        private void disposeTag()
        {
            this.tagpb1.Hide();
           // Tag pb1.image.Tag = null;

            captured.Dispose();
            showingTag = false;
        }
        /// <summary>
        /// Captures the screen and displays it in the picture box. //////////////////////// CAPTURE SCREEN or SELECTION
        /// </summary>
        /// 
        private void captureScreen(Rectangle bounds)
        {
            // 1) Where is the PB on the screen?
            Point pbScreen = pb1.PointToScreen(Point.Empty);

            // 2) Build the absolute rectangle to grab
            Rectangle screenRect = new Rectangle(
                pbScreen.X + bounds.X,
                pbScreen.Y + bounds.Y,
                bounds.Width,
                bounds.Height
            );

            // 3) Allocate the bitmap
            captured?.Dispose();
            captured = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

            // 4) Copy that exact area from the screen into our bitmap
            using (Graphics g = Graphics.FromImage(captured))
            {
                g.CopyFromScreen(screenRect.Location, Point.Empty, screenRect.Size);
            }
        }

        private void captureScreena(Rectangle bounds)
        {
            // Convert the top-left of the bounds from pb1’s client coords to screen coords
            Point screenOrigin = pb1.PointToScreen(bounds.Location);

            // Dispose any prior bitmap
            captured?.Dispose();
            captured = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(captured))
            {
                // Copy exactly the rectangle you lassoed
                g.CopyFromScreen(screenOrigin, Point.Empty, bounds.Size);
            }
        }


        private void captureScreenAIWAS(Rectangle bounds)
        {
            // translate pb1‐client coords to screen‐coords
            var screenLoc = pb1.PointToScreen(bounds.Location);

            // make a bitmap of exactly the rubber‐band size
            captured?.Dispose();
            captured = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(captured))
            {
                // copy that exact region of the screen
                g.CopyFromScreen(screenLoc, Point.Empty, bounds.Size);
            }

            // (optional) draw your red border if you like:
            using (var g = Graphics.FromImage(captured))
            using (var brush = new SolidBrush(Color.Red))
            {
                int w = bounds.Width, h = bounds.Height, t = 2;
                // top
                g.FillRectangle(brush, new Rectangle(0, 0, w, t));
                // left
                g.FillRectangle(brush, new Rectangle(0, 0, t, h));
                // bottom
                g.FillRectangle(brush, new Rectangle(0, h - t, w, t));
                // right
                g.FillRectangle(brush, new Rectangle(w - t, 0, t, h));
            }
        }


        private void captureScreenxxxx(Rectangle bounds)
        {
            // translate pb1‐client coords to screen‐coords
            var screenLoc = pb1.PointToScreen(bounds.Location);

            // make a bitmap of exactly the rubber‐band size
            captured?.Dispose();
            captured = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(captured))
            {
                // copy that exact region of the screen
                g.CopyFromScreen(screenLoc, Point.Empty, bounds.Size);
            }

            // (optional) draw your red border if you like:
            using (var g = Graphics.FromImage(captured))
            using (var brush = new SolidBrush(Color.Red))
            {
                int w = bounds.Width, h = bounds.Height, t = 2;
                // top
                g.FillRectangle(brush, new Rectangle(0, 0, w, t));
                // left
                g.FillRectangle(brush, new Rectangle(0, 0, t, h));
                // bottom
                g.FillRectangle(brush, new Rectangle(0, h - t, w, t));
                // right
                g.FillRectangle(brush, new Rectangle(w - t, 0, t, h));
            }
        }


        private void captureScreenWAS(Rectangle bounds)
        {
            if (bounds == null)
                bounds = Screen.PrimaryScreen.Bounds;

            int colourDepth = Screen.PrimaryScreen.BitsPerPixel;

            PixelFormat format;
            switch (colourDepth)
            {
                case 8:
                case 16:
                    format = PixelFormat.Format16bppRgb565;
                    break;

                case 24:
                    format = PixelFormat.Format24bppRgb;
                    break;

                case 32:
                    format = PixelFormat.Format32bppArgb;
                    break;

                default:
                    format = PixelFormat.Format32bppArgb;
                    break;
            }
            try
            {
                captured = new Bitmap(bounds.Width, bounds.Height, format);
            }
            catch (Exception)
            {

               // Application.Exit();
            }
            Graphics gdi;
            try
            {
                gdi = Graphics.FromImage(captured);
            }
            catch (Exception)
            {

                return;
            }
            gv.debug.w("---");
            Point xyoff = new Point(bounds.X, bounds.Y);
            Point offby8 = PointToScreen(xyoff);
            gv.debug.w(xyoff);
            gv.debug.w(offby8);
            offsetScreenX = xyoff.X - offby8.X;
            offsetScreenY = xyoff.Y - offby8.Y;
            gdi.CopyFromScreen(bounds.Left - offsetScreenX, bounds.Top - offsetScreenY, 0, 0, bounds.Size);
            //if (false)
            using (SolidBrush brush = new SolidBrush(Color.Red))
            {
                //--- top
                Rectangle toFill = new Rectangle(0, 0, bounds.Width, lineWidth);
                Point xy = new Point(bounds.X, bounds.Y);
                Point offby = PointToScreen(xy);
                gv.debug.w(xy);
                gv.debug.w(offby);
                Point wh = new Point(bounds.Width, bounds.Height);
                Point offby2 = PointToScreen(wh);
                gv.debug.w(wh);
                gv.debug.w(offby2);
                //bounds.Height = PointToScreen(bounds.Height);

                offsetScreenX = xy.X - offby.X;
                offsetScreenY = xy.Y - offby.Y;

                gdi.FillRectangle(brush, toFill);
                //-- left
                //toFill.X = 0;
                //toFill.Y = 0;
                toFill.Width = lineWidth;
                toFill.Height = bounds.Height;
                gdi.FillRectangle(brush, toFill);
                //-- right
                toFill.X = bounds.Width - lineWidth;
                toFill.Y = 0;
                toFill.Width = bounds.Width;
                toFill.Height = bounds.Height;
                gdi.FillRectangle(brush, toFill);
                //-- bottom
                toFill.X = 0;
                toFill.Y = bounds.Height - lineWidth;
                toFill.Width = bounds.Width;
                toFill.Height = bounds.Height;
                gdi.FillRectangle(brush, toFill);
                //
            }

            if (tagpb1 == null)
                tagpb1 = new PictureBox();
           tagpb1.Image = captured;
            
        }




        // Track the image size and the type of animation ----------------------------===========================
        // (expanding or shrinking).
        private bool isShrinking = false;
        private int imageSize = 0;

        // Store the logo that will be painted on the form.
        // private Image image;

        bool bZoom = false;
        private void autoZoom()
        {

            // Start the timer that invalidates the form.
            bZoom = !bZoom;
            gv.debug.w("bZoom", bZoom.ToString());
            initTimer(bZoom);
        }
        public void initTimer(bool bRun)
        {


            timer.Interval = 1500;// (10) * (1);              // Timer will tick every 
            timer.Enabled = bRun;
            if (bRun)            // Enable the timer
                timer.Start();                              // Start the timer
            else
                timer.Stop();
            //  label.Location = new Point(100, 100);
            //  label.AutoSize = true;
            //  label.Text = String.Empty;

            //   this.Controls.Add(label);
        }
        void timer_Tick(object sender, EventArgs e)
        {
            //resizeZoomIn();
            if (true)
                return;
        }
        void holdit(object sender, EventArgs e)
        {
            // Change the desired image size according to the animation mode.
            if (isShrinking)
            {
                imageSize--;
            }
            else
            {
                imageSize++;
            }

            // Change the sizing direction if it nears the form border.
            if (imageSize > (this.Width - 150))
            {
                isShrinking = true;
            }
            else if (imageSize < 1)
            {
                isShrinking = false;
            }

            // Repaint the form.
            this.Invalidate();
        }

        private void p2bDoPaint(object sender, PaintEventArgs e)
        {
            Graphics g;

            g = e.Graphics;

            g.SmoothingMode = SmoothingMode.HighQuality;

            // Draw the background.
            g.FillRectangle(Brushes.Yellow, new Rectangle(new Point(0, 0),
            this.ClientSize));

            // Draw the logo image.
            g.DrawImage(image, 0, 0, 500 + imageSize, 500 + imageSize);
            //g.DrawImage(image, 50, 50, 50 + imageSize, 50 + imageSize);
        }

        private void chkUseDoubleBuffering_CheckedChanged(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Cursor.Show();
        }

        private void pb2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void closeThisWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void setCatalogMode()
        {
            displayMode = D1function.CATALOG;
            Cursor.Show();
            gv.mainWindow.stopSlideShow();
            
        }
        //add scrolling
        private void Display1_Scroll(object sender, ScrollEventArgs e)
        {
            int a = e.NewValue;
            int b = e.OldValue;
            string test = e.ToString();
            ScrollOrientation c = e.ScrollOrientation;
            a = 0;

           // moveImage(111, 111);
        }


        ////////////////////////////////////////////////////////////// resize //////////////////////////
        public DisplayMode getDisplayModeDM()
        {
            return dm;
        }

        // for Frame1 keyaction ------- no IINFO parm
        public void resizeImage()
        {
            //+++++++++++++ important ++++++++++ determines state of userOverRide Image size/loc !!!!!
            //getImageDisplaySize();

            if (getDisplayModeDM().imageSize == ImageSize.BESTFIT)
            {
                this.resizeBestFit();
            }
            else if (getDisplayModeDM().imageSize == ImageSize.STORED)
            {
                this.resizeStored(iinfo);//resizeStored();
            }
            else if (getDisplayModeDM().imageSize == ImageSize.FULLSCREEN)
            {
                this.resizeFullScreen();
            }
            else if (getDisplayModeDM().imageSize == ImageSize.MAX)
            {
                resizeZoom();
            }
            else //ACTUAL SIZE
            {
                this.resizeActual();
            }

            repaint();
            if (false) // in Display1Image.resizeImage()
            {
               // message(this.formatDisplayInfoTitle(info1), formatDisplayInfo(info1), 9);

            }

            return;
        }

      

        public PanelSize getPanelBounds()
        {
            return panel_size;
        }
        private void resizeActual2()
    {
        int iw, ih, iwidth, iheight;
        double wRatioBest = 0, hRatioBest, bestRatio;
        Boolean allowBestFit = false;
            
        panel_size = getPanelBounds();
        dp.imageWidth = image.Width;
        dp.imageHeight = image.Height;
        if (debug)
        {
            gv.debug.w ("image resize actual iw= ih=%d",
                     dp.imageWidth.ToString(), dp.imageHeight.ToString());
        }

        if (dm.imageLocation == ImageLocation.CENTER)
        {
            dp.imagePosX = (panel_size.width - dp.imageWidth) / 2;
            dp.imagePosY = (panel_size.height - dp.imageHeight) / 2;

        }
    }

        private void resizeBestFit2() //bestFit
    {
        Boolean rc = false; //success?
        int width = 0;
        int height = 0;
        int iw = 0;
        int ih = 0;
        double wRatio, hRatio, resizeRatio, wRatioBest, hRatioBest, bestRatio;

        if (image == null)
        {
            gv.debug.w("image  Images NULL", myName);
         //   Application.Exit();
        }

        panel_size = getPanelBounds();
        iw = image.Width;
        ih = image.Height;

        wRatio = (double) panel_size.width / iw;
        hRatio = (double) panel_size.height / ih;

        if (wRatio < 1 && hRatio < 1) //larger img
        {
            resizeRatio = Math.Min(wRatio, hRatio); //smaller number will insure all shows
        } else if (wRatio > 1 && hRatio > 1) //smaller img
        {
            resizeRatio = Math.Min(wRatio, hRatio);//smaller number will insure all shows
        } else  //one side larger , one side smaller img
        {
            resizeRatio = Math.Min(wRatio, hRatio);//smaller number will insure all shows
        }
        if (debug)
        {
            gv.debug.w("raw ratio iw=%f ih=%f bestfit ratio%f\n", wRatio.ToString(), hRatio.ToString(), resizeRatio.ToString());
        }

        dp.imageWidth = (int) (iw * resizeRatio);
        dp.imageHeight = (int) (ih * resizeRatio);

        DisplayMode dm1 = this.getImageSizeDM();
        if (dm.imageLocation == ImageLocation.CENTER) // david fix this probably wrong
        {
            dp.imagePosX = (panel_size.width - dp.imageWidth) / 2;
            dp.imagePosY = (panel_size.height - dp.imageHeight) / 2;
        } else
        {
            dp.imagePosX = 0;//left
            dp.imagePosY = 0;//left
        }
        repaint();
        //captureImageInfo();
        return;
    }
        public DisplayMode getImageSizeDM()
        {
            //(ImageSize imgs, ImageLocation al, Boolean alocked, Boolean aset)
            //DisplayMode dm = new DisplayMode(ImageSize.BESTFIT, ImageLocation.CENTER, true, true);
            return dm;
        }

        private void resizeStored(ImageInfo info_arg) //ImageSize.STORED
    {
        panel_size = null;
        Boolean rc = false; //success?
        int width = 1;
        int height = 1;
        int iw = 0;
        int ih = 0;
        int p_w = 0;
        int p_h = 0;
        double wRatio = 0, hRatio = 0, resizeRatio;
        double wRatioBest = 0, hRatioBest = 0, bestRatio;

        dp.imagePosX = info_arg.x;//.getX();
        dp.imagePosY = info_arg.y;//.getY();

        dp.imageWidth = info_arg.width;
        dp.imageHeight = info_arg.height;
        if (debug)
        {

            gv.debug.w(String.Format("{0,10:G} Display1Image resizeSTORED ix={1,10:G}, iy={2,10:G}, iw={3,10:G} ih={4,10:G}",
                    myName, dp.imagePosX, dp.imagePosY, dp.imageWidth, dp.imageHeight));
        }

        repaint();
        return;
    }

        public void resizeZoom()
    {
        int iWidth = 1;
        int iHeight = 1;
        double wRatio = 0, hRatio, resizeRatio;
        int currentSlot = -1;
        Boolean allowBestFit = false;
        //Rectangle panel_size = getPanelBounds();

        //currentSlot = imageLoader.slotNUM_currentDisplay;
        iWidth = image.Width;
        iHeight = image.Height;

        int xd = 0;//(int) ((double) this.imageWidth * .1);
        int yd = 0;//(int) ((double) this.imageHeight * .1);

        //resizeImage(dm);

        if (iWidth > iHeight)//wider than tall IMAGE
        {
            hRatio = (double) this.Height / iHeight;
        } else
        {
            hRatio = (double) this.Width / iWidth;
        }

        resizeRatio = hRatio;
        //fit to on the screen
        iWidth = (int) (iWidth * resizeRatio);
        iHeight = (int) (iHeight * resizeRatio);
        if (debug)
        {
            gv.debug.w(String.Format("{0} REsize ZOOM WIDE image {1} by {2}, screen {3} by {4}  {5} {6} {7}",
                    myName, iWidth, iHeight,
                    panel_size.width, panel_size.height, wRatio, hRatio, resizeRatio));
        }

            
        resizeImage(iWidth, iHeight);



            
    }//ZOOM

        private void xresizeZoom()
    {
        int iw, ih, iwidth, iheight;
        double wRatioBest = 0, hRatioBest, bestRatio;
        Boolean allowBestFit = false;
        panel_size = getPanelBounds();
        int midPointX; // center
        int midPointY;

        iw = image.Width;
        ih = image.Height;
        wRatioBest = (double) panel_size.width / iw;
        hRatioBest = (double) panel_size.height / ih;
        //resizeImage(ImageSize.MAX);


        if (wRatioBest < hRatioBest)
        {
            bestRatio = wRatioBest;
        } else
        {
            bestRatio = hRatioBest;
        }
        if (debug)
        {
            gv.debug.w("IF ZOOM (MAX) ..use larger number = %f\n", bestRatio.ToString());
        }
        iwidth = (int) (iw * bestRatio);
        iheight = (int) (ih * bestRatio);

        midPointX = dp.imagePosX + (dp.imageWidth / 2);
        midPointY = dp.imagePosY + (dp.imageHeight / 2);

        dp.imagePosX = midPointX - (dp.imageWidth / 2);
        dp.imagePosY = midPointY - (dp.imageHeight / 2);
        resizeImage(iwidth, iheight);
    }//ZOOM

        private Boolean resizeFullScreen2()
    {
        Boolean larger = false;
        int iWidth = 1;
        int iHeight = 1;
        double wRatio, hRatio, resizeRatio;
        int slotCurrentlyDisplayed = -1;
        Boolean allowBestFit = true;

        panel_size = getPanelBounds();
        if (image != null)
        {
            iWidth = image.Width;
            iHeight = image.Height;

            if (debug)
            {
                gv.debug.w(String.Format("{0} FULLSCREEN image {1} by {2} for Display {3} by {4}",
                        myName, iWidth, iHeight, this.Width, this.Height));
            }
            if (iWidth < panel_size.width)
            {
                wRatio = (double) panel_size.width / iWidth;
            } else
            {
                wRatio = 1;
            }

            if (iHeight < panel_size.height)
            {
                hRatio = (double) panel_size.height;// / iHeight;
            } else
            {
                hRatio = 1;
            }

            if (wRatio != 1 || hRatio != 1) //needs resizing
            {
                larger = true;
                //
                resizeRatio = Math.Min(wRatio, hRatio);
                //fit to the PANEL
                iWidth = (int) (iWidth * resizeRatio);
                iHeight = (int) (iHeight * resizeRatio);

                if (debug)
                {
                    gv.debug.w(String.Format("{0} REsize image {1} by {2}, screen {3} by {4}  {5} {6} {7}",
                            myName, iWidth, iHeight,
                            this.Width, this.Height, wRatio, hRatio, resizeRatio));
                }
                resizeImage( iWidth, iHeight);
            }//resize
            else if (allowBestFit)
            { // image is LARGER than display area (panel) so resize
                if (iWidth > panel_size.width)
                {
                    wRatio = (double) panel_size.width / iWidth;
                } else
                {
                    wRatio = 1;
                }
                if (iHeight > panel_size.height)
                {
                    hRatio = (double) panel_size.height / iHeight;
                } else
                {
                    hRatio = 1;
                }
                if (wRatio != 1 || hRatio != 1) //needs resizing
                {
                    resizeRatio = Math.Min(wRatio, hRatio);
                    //resize
                    dp.imageWidth = (int) (iWidth * resizeRatio);
                    dp.imageHeight = (int) (iHeight * resizeRatio);
                    if (debug)
                    {
                        gv.debug.w(String.Format("{0} image FULLSCREEN choose bestfit iw={1} ih={2}, wRatio={3} hRatio={4}, xx={5} yy={6}",
                                myName, iWidth, iHeight, wRatio, hRatio, dp.imageWidth, dp.imageHeight));
                    }
                }//resize bestfit
            }//resize
           // showBounds();
        } else
        {
            gv.debug.w("%s FULLSCREEN resize failed (no Images)\n", myName);
          // Application.Exit();
        }
        if (dm.imageLocation == ImageLocation.CENTER)
        {
            dp.imagePosX = (panel_size.width - iWidth) / 2;
            dp.imagePosY = (panel_size.height - iHeight) / 2;
        } else
        {
            dp.imagePosX = 0; //(panel_size.width - iWidth) / 2;
            dp.imagePosY = 0; //(panel_size.height - iHeight) / 2;
            dp.imagePosX = (panel_size.width - iWidth) / 2;
            dp.imagePosY = (panel_size.height - iHeight) / 2;
        }
        return larger;
    }//resize MAX


        public void HaveMouse(bool bHave)
        {
            bHaveMouse = bHave;
            gv.mainWindow.HaveMouse(bHave);
        }
        //
        //move relative
        //
        public void moveImage(int x_move, int y_move)
        {
            dp.imagePosX += x_move;
            dp.imagePosY += y_move;
            repaint();
        }
        /* POPUP TOOLSTRIPMENU 
         * events
         * */
        private void miRedraw_Click(object sender, EventArgs e)
        {
            this.Invalidate();
            pb1.Image = null;
            
            displayThisImage(originalImage);
          
        }
        public void Zoom()
        {

        }
        private void miResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pb1.Image = null;
            pb1.Refresh();
            displayThisImage(pb1.Image);
            Image i = pb1.Image;
            gv.debug.w("RESET");
          this.Refresh();
          Rectangle r = pb1.Bounds;
          pb1.Dispose();
          pb1 = new PictureBox();
          pb1.Bounds = r;
            displayThisImage(i);
            pb1.Refresh();
        }

        private void miCatalogImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            displayMode = D1function.CATALOG;
            Cursor.Show();
            gv.mainWindow.stopSlideShow();
            this.Text = "Catalog mode .. ";
            setFullScreenMode(false);

        }

        private void miFullScreenModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setFullScreenMode(true);
            resizeBestFit();
            Cursor.Hide();
        }
        public void SetFullScreenModeOn()
        {
            setFullScreenMode(true);
            resizeBestFit();
            Cursor.Hide();
        }
        private void miShowAllTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showAllTags();
        }


        private void showResizableWithTitleBar(object sender, EventArgs e)
        {
            //setFullScreenMode(false);
            Rectangle b = this.Bounds;
            b.X = b.X + 100;
            b.Y = b.Y + 100;
            b.Width = (int) (b.Width * .8);
            b.Height = (int) (b.Height * .8);
            this.Bounds = b;
            setFullScreenMode(false);
            //resizeBestFit();
           // this.AutoScroll = true;
          //  this.VerticalScroll.Enabled = true;
          //  this.HorizontalScroll.Enabled = true;
            this.Refresh();


        }
        private void miAnnotation_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Annotation");
            if (!miAnnotation.Checked)
            {
                displayMode = D1function.ANNOTATE;
                HaveMouse(false);
                gv.mainWindow.stopSlideShow();
                tagCount = 0;
                miAnnotation.Checked = true;
            }
            else
            {
                miAnnotation.Checked = false;
                displayMode = D1function.DISPLAY1;
            }
        }

        private void miSlideShowMode_Click(object sender, EventArgs e)
        {
            displayMode = D1function.SLIDESHOW;
            gv.mainWindow.startSlideShow(-1, true, gv.iShowDirection);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
          //  Application.Exit();
        }

        private void pb1_SizeChanged(object sender, EventArgs e)
        {
          //  pb2.Bounds = pb1.Bounds;
        }

        bool bMouseOver = false;
        private void pb1_MouseEnter(object sender, EventArgs e)
        {
            bMouseOver = true;
            pb1.Focus();
            gv.debug.w("Display1 pb1_MouseEnter focus");
        }

        private void pb1_MouseLeave(object sender, EventArgs e)
        {
            bMouseOver = false;
        }
        void pb1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                resizeZoomIn();
            }
            else
            {
                resizeZoomOut();
            }
            // here I can use e.Delta        
        }

        private void monitorFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showResizableWithTitleBar(sender, e);
            startWatcher();
        }

        private void holdThisSlideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bHoldThisImage = !bHoldThisImage;
        }
    }
    
}

/*
pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
pictureBox1.MouseHover += new EventHandler(pictureBox1_MouseHover);
 
void pictureBox1_MouseHover(object sender, EventArgs e)
{
    pictureBox1.Focus();
}
 

void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
{
    // here I can use e.Delta        
}
*/
