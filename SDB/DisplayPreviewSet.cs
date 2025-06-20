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
    public partial class DisplayPreviewSet : Form
    {
        GlobalVars gv;
        Image image = null;
        Size originalSize;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        int startingImageFileIdx = 0;


        int lastSelectedPbIdx = -1;

        FileInfoItem finfo;
        string displayName;
        PictureBox[] pb;

        int nextpb = -1;
        int MAXPB = 300;
        int pbCreatedCount = 0;
        int maxPbIdxLoaded = 0;
        int maxImagesToLoad = 0;
        int pbLoadedCount = 0; /// may be at the end of the file and so not all PB's on the screen have images

        Boolean loadB = true;
        Boolean firstTime = true;
        Rectangle rec2;
        System.Drawing.Rectangle rec;
        ToolTip pbToolTip = new ToolTip();

        decimal aspectShift = .7M;  //// make tarot cards skinnier , closer together
        int numRows = 6;
        int MAXROWS = 6;
        // 4 x 3
        int numColumns = 14;
        const int spaceH = 6;
        int pbWidth = 80;  //160
        int pbHeight = 120;
        int pbFrameHeight = 120; /// size of "frame around the PB " 
        int pbFrameWidth = 120; /// size of "frame around the PB" 

        Boolean borderB = false;
        int leftMarginSpace = 5;
        int topMarginSpace = 5;
        int vSpace = 15;
        int hSpace = 10;
        int spacerH = 20;
        int spacerV = 25;
        // int loadFromNum = 0;
        FileFunctions ff;


        public DisplayPreviewSet() //not used
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;
            rec2 = this.DisplayRectangle;
        }

        public DisplayPreviewSet(GlobalVars g, int startingImageNumber = 0) /////////////////////// main init
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            gv = g;

            // tbLoading.Hide();
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called

            pb = new PictureBox[MAXPB];
            rec2 = this.DisplayRectangle;
            getWindowSize();
            ff = new FileFunctions(gv); ////////// FILE
            computeResizeByRowCount(numRows);
            ResizeDisplaySet();
            this.Visible = true;
            this.Activate();
            LoadSet(startingImageNumber);
            this.Refresh();
            tbx = new TextBox[MAXPB];
            Cursor.Position = new Point(0, 0);
            timer.Stop();
        }

        public void getWindowSize() ////////////////////////////  using the ClientRectangle causes the window NOT to maximize on opening
        {
            rec = Screen.FromControl(this).Bounds;

            // rec = Screen.PrimaryScreen.WorkingArea;
            //rec = this.ClientRectangle;

            rec.Width = rec.Width - spaceH;
            //rec.Height = rec.Height - spaceH; ;

        }
        bool bDisplayText = false;
        bool loaded1 = false;
        bool bAllowResize = true;
        public void AllowResize(bool bAllow)
        {
            bAllowResize = bAllow;
        }
        public int ResizeDisplaySet(bool resize = true)
        {
            Point pbLocation = new Point(0, 0);
            int pbNumber = 0;
            if (loaded1 && !resize)
                return 0;
            loaded1 = true;
            setTitle("resizing...");
            for (int rowIdx = 0; rowIdx < numRows; ++rowIdx) //rows
            {
                pbLocation.Y = (rowIdx == 0 ? topMarginSpace : 0) + (rowIdx * pbFrameHeight);//// (rowidx == 0 ? 0 : spacerV);
                                                                                             //     gv.debug.w("=============pbLocation.Y", pbLocation.Y.ToString());
                for (int columnIdx = 0; columnIdx < numColumns && pbNumber < MAXPB; ++columnIdx) //columns
                {
                    pbLocation.X = (columnIdx == 0 ? leftMarginSpace : 0) + (columnIdx * pbFrameWidth);//// (columnIdx == 0 ? 0 : spacerH);
                    pbNumber = (rowIdx * numColumns) + columnIdx;
                    if (pb[pbNumber] == null) //firstTime || pb[counter] == null) /////////////////////////////////---------------- CREATION --------FIRST TIME ONLY----------------
                    {
                        if (pbNumber >= MAXPB)
                        {
                            gv.debug.w("ERROR LEVEL 1! pbNumber exceeds MAXPB");
                            return pbNumber - 1;
                        }
                        pbCreatedCount = pbNumber;
                        pb[pbNumber] = new PictureBox();
                        pbToolTip.ToolTipTitle = "image fpath";
                        pbToolTip.UseFading = true;
                        pbToolTip.UseAnimation = true;
                        pbToolTip.IsBalloon = false;
                        pbToolTip.ShowAlways = true;
                        pbToolTip.AutoPopDelay = 10000;
                        pbToolTip.InitialDelay = 10;
                        pbToolTip.ReshowDelay = 10;
                        pbToolTip.SetToolTip(pb[pbNumber], "image");
                        //pb1.SizeMode = PictureBoxSizeMode ; 
                        pb[pbNumber].SizeMode = PictureBoxSizeMode.Zoom;
                        //control.MouseDown += SomeMethod;
                        pb[pbNumber].MouseUp += PbClicked;
                        pb[pbNumber].MouseDoubleClick += PbMouseDoubleClicked;
                        // pb[counter].PreviewKeyDown += PbKey2;
                        this.Controls.Add(pb[pbNumber]);
                        //    gv.debug.w("created pb[", pbNumber.ToString());
                    }
                    //      RESIZE and position
                    //  gv.debug.w("updating pb[", pbNumber.ToString());
                    pb[pbNumber].Height = pbHeight;
                    pb[pbNumber].Width = pbWidth;
                    pb[pbNumber].Location = pbLocation;
                    if (borderB)
                        pb[pbNumber].BorderStyle = BorderStyle.FixedSingle;
                    else
                        pb[pbNumber].BorderStyle = BorderStyle.None;
                    //  gv.debug.w("PictureBox ", pbNumber.ToString());
                    //  gv.debug.w(pb[pbNumber].Bounds.ToString());
                    pb[pbNumber].Visible = true;
                }
                this.Refresh();
            } /////////// FOR
            firstTime = false;
            int pbCount = pbNumber + 1;
            //  imagesDisplayedCount = counter;
            maxPbIdxLoaded = numRows * numColumns - 1; //////////////// this is temporary ... reset to the actual count of images loaded
            maxImagesToLoad = numRows * numColumns;
            gv.debug.w("--RepositionDisplaySet--  maxPbIdxLoaded", maxPbIdxLoaded.ToString(), ", maxImagesToLoad", maxImagesToLoad.ToString());
            // if (pbCount < imagesDisplayedCount)
            //   imagesDisplayedCount = pbCount;
            setTitle(startingImageFileIdx.ToString() + " thru " + (startingImageFileIdx + pbNumber).ToString() + " " + pb[0].ImageLocation);
            return maxImagesToLoad;
        }

        TextBox[] tbx;
        //pb = new PictureBox[MAXPB];
        bool bDisplayFileName = false;

        public void hideDisplayFileName()
        {
            if (bDisplayFileName)
                displayFileName(false); /// toggle off
        }

        public int displayFileName(bool bShow)
        {
            bDisplayFileName = bShow;

            int imageFileIdx = startingImageFileIdx;

            if (bDisplayFileName)
            {
                // pbToolTip.ShowAlways = false;
                gv.debug.w("^^imagesDisplayedCount ", maxPbIdxLoaded.ToString());
                gv.debug.w("^^startingImageFileIdx", startingImageFileIdx.ToString());
            }
            else
            {
                // pbToolTip.ShowAlways = true;
                for (int idx = 0; idx <= maxPbIdxLoaded; ++idx)
                    if (tbx[idx] != null)
                        tbx[idx].Hide();
                return 0;
            }

            for (int idx = 0; idx <= maxPbIdxLoaded && imageFileIdx < gv.imageFileList1.getImageCount(); ++idx)
            {
                string fpath = gv.imageFileList1.getIndexed(imageFileIdx++).fpath;
                string fname = System.IO.Path.GetFileName(fpath);
                //   TextBox tb = new TextBox();
                bool bAddTbx = false;
                if (tbx[idx] == null)
                {
                    tbx[idx] = new TextBox();
                    bAddTbx = true;
                }
                tbx[idx].Text = fname;
                tbx[idx].Width = pb[idx].Width;
                tbx[idx].Height = 10;
                tbx[idx].Location = pb[idx].Location;
                if (bAddTbx) this.Controls.Add(tbx[idx]);
                tbx[idx].BringToFront();
                tbx[idx].Visible = true;
                tbx[idx].Enabled = false;
            }
            return maxPbIdxLoaded;
        }


        bool bRunSlideShow = false;
        int numToAdvance = 1;
        void timer_Tick(object sender, EventArgs e) // TIMER EVENT <<<<<<<<<<<<<<<<<<<< gv.nextSlide 
        {

            //label.Text = DateTime.Now.ToString();
            if (bRunSlideShow)
            {
                if (this.startingImageFileIdx < gv.imageFileList1.getImageCount())
                {
                    hideDisplayFileName();
                    if (numToAdvance > 0)
                        this.LoadSet(this.startingImageFileIdx + numToAdvance);
                    else
                        this.LoadSet(this.startingImageFileIdx + numColumns);
                    gv.debug.w("ShowNextSlide in preview set  .... timer_Tick");
                }

                else
                    bRunSlideShow = false;
            }
        }


        public int getTitleBarHeight()
        {

            Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);

            return (screenRectangle.Top - this.Top);
        }
        public int computeResizeByRowCount(int rows)   //10 initially
        {
            int windowHeight = rec.Height - getTitleBarHeight();
            int windowWidth = rec.Width - spaceH;

            if (rows > MAXROWS)
                rows = MAXROWS;
            else if (rows < 1)
                rows = 1;

            numRows = rows;

            pbFrameHeight = ((windowHeight - (vSpace * rows)) / rows); //// allow for space between rows
            //int nwidth = (nheight / 4) * 3;
            pbHeight = pbFrameHeight - vSpace;
            pbWidth = (int)(pbHeight * (aspectShift));// 70% width for Tarot
            pbFrameWidth = pbWidth + hSpace;
            numColumns = windowWidth / pbFrameWidth;
            maxPbIdxLoaded = numRows * numColumns;
            maxImagesToLoad = numRows * numColumns;
            gv.debug.w("imagesDisplayedCount", maxPbIdxLoaded);

            return numRows;

        }
        public int SetColumnCount(int colc)
        {
            if (colc < 1)
                colc = 1;
            else if (colc > MAXROWS)
                colc = MAXROWS;
            numRows = colc;

            timer.Interval = 2000 + (2000 * (numRows - 1));

            computeResizeByRowCount(numRows);
            for (int idx = 0; idx <= pbCreatedCount; ++idx) //was MAXPB      was < pbCreatedCount
                if (pb[idx] != null)
                    pb[idx].Visible = false;
            ResizeDisplaySet();
            // Cursor.Position = new Point(5, 5);

            return numRows;
        }

        public int ResizePreviewList(int sizeInc)
        {
            if (sizeInc > 0)
                numRows++;
            else
                numRows--;
            computeResizeByRowCount(numRows);
            for (int idx = 0; idx < MAXPB; ++idx)
                if (pb[idx] != null)
                    pb[idx].Visible = false;
            ResizeDisplaySet();
            //Cursor.Position = new Point(pb[MAXPB].Location.X, 10);

            return numRows;
        }

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public int lastStart()
        {
            return nextpb;
        }
        public void setTitle(string n)
        {
            this.Text = n;
        }

        bool bUSE_OPTIMIZED_LOAD = true;
        int countMemFree = 0;
        bool bColorGray = true;
        public int LoadSet(int startingImageNo) //// load images ///////////////////////////////////////////////////////////////////
        {
            int rc = 0;
            int iStartPb = 0;
            int imagenoWas = startingImageNo;
            int previousStartingFileIdx = startingImageFileIdx;

            bool bSingle = false;

            if (bColorGray)
                this.BackColor = Color.LightGray;
            else
                this.BackColor = Color.LightBlue;

            bColorGray = !bColorGray;
            this.Refresh();
            if (++countMemFree > 44)
            {
                countMemFree = 0;
                try
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception)
                {

                    MessageBox.Show("Exception in memory free");
                }
            }




            lastSelectedPbIdx = -1; //unselect

            if (startingImageNo < 0)
                startingImageNo = 0;
            else if (startingImageNo >= gv.imageFileList1.getImageCount())
                startingImageNo = gv.imageFileList1.getImageCount() - maxPbIdxLoaded;
            startingImageFileIdx = startingImageNo;

            int imageFileItemIdx = startingImageFileIdx;
            int nextFileIdx4Single = 0;

            this.Text = "Loading images ... from imageFile IDX " + startingImageNo.ToString();

            // HAD THE FREE MEMORY HERE
            // MAXPB
            //maxPbIdxLoaded

            bool skipMemFree = false;
            if (bUSE_OPTIMIZED_LOAD)
            {
                if (startingImageNo == previousStartingFileIdx + 1)
                {
                    skipMemFree = true;
                }
                else if (startingImageNo == previousStartingFileIdx - 1)
                    skipMemFree = true;
            }
            int idx2 = 0;
            if (!skipMemFree)
            {
                for (idx2 = 0; idx2 < maxPbIdxLoaded; ++idx2) //pbCreatedCount   since it is < doesn't need subtract 1
                {
                    if (pb[idx2].Image != null)
                    {

                        pb[idx2].Image.Dispose();
                    }
                }
            }

            if (bUSE_OPTIMIZED_LOAD)
            {
                if (startingImageNo == previousStartingFileIdx + 1) //////////////// RIGHT         optimize  shift images 
                {

                    int idx;
                    int limit = maxPbIdxLoaded - 1;
                    for (idx = 0; idx < maxPbIdxLoaded; ++idx) //pbCreatedCount   since it is < doesn't need subtract 1
                    {
                        pb[idx].Image = pb[idx + 1].Image;
                    }
                    imageFileItemIdx++;
                    for (idx = 0; idx < maxPbIdxLoaded; ++idx)
                    {
                        pbToolTip.SetToolTip(pb[idx], gv.imageFileList1.getIndexed(imageFileItemIdx).fpath);
                        imageFileItemIdx++;
                    }

                    pb[maxPbIdxLoaded].Image = null;

                    iStartPb = maxPbIdxLoaded; ///will go to load PB[iStartPb].images
                    bSingle = true;
                    gv.debug.w(" ... STEP FORWARD optimized used copy PB sequence " + startingImageNo.ToString());
                    nextFileIdx4Single = startingImageNo;
                    Load1Image(imageFileItemIdx, maxPbIdxLoaded);
                    // return rc;
                }
                else if (startingImageNo == previousStartingFileIdx - 1)//////////////// LEFT        optimize  shift images
                {
                    int idx;
                    for (idx = maxPbIdxLoaded; idx > 0; --idx) //pbCreatedCount
                    {
                        pb[idx].Image = pb[idx - 1].Image;
                    }
                    for (idx = 0; idx > 0; --idx)
                    {
                        imageFileItemIdx++;
                        pbToolTip.SetToolTip(pb[idx], gv.imageFileList1.getIndexed(imageFileItemIdx).fpath);
                    }

                    pb[0].Image = null;

                    iStartPb = 0; ///will go to load PB[iStartPb].images
                    bSingle = true;
                    nextFileIdx4Single = startingImageFileIdx - 1;
                    Load1Image(startingImageFileIdx, 0);
                    //return rc;
                }
            }
            else
            {
                // not needed now
                /*
                for (int idx = 0; idx < maxPbIdxLoaded; ++idx) //pbCreatedCount   since it is < doesn't need subtract 1
                {
                    if (pb[idx].Image != null)
                        pb[idx].Image = null;
                }
                */
            }
            if (!bSingle)
                rc = LoadFromImageList2(startingImageFileIdx, iStartPb);
            setTitle(startingImageNo.ToString() + " thru " + rc.ToString() + " of " + gv.slideCount1.ToString() + " >>>>>  " + pb[0].ImageLocation
                + gv.imageFileList1.getIndexed(startingImageNo).fpath
                );

            tbLoading.Hide();
            return rc;
        }

        public int getImageFileIdx(int pbx)
        {
            return startingImageFileIdx + pbx;
        }
        bool bLoadFromStream = true;

        public int Load1Image(int idxImageFileItem1, int nextpb)
        {
            string fpath;
            bool bError = false;
            bool bEOF = false;
            bool bGotImage = false;
            int lastLoadedIdx = 0;

            fpath = gv.imageFileList1.getIndexed(idxImageFileItem1).fpath; ////////////////////////////////// iStartImageNum
            if (fpath == null)
            {
                gv.debug.w(" NULL FILEPATH ... ", gv.imageFileList1.getIndexed(idxImageFileItem1).ToString());
                if (bDeleteInvalidImages)
                    gv.imageFileList1.removeItemAt(idxImageFileItem1);
            }
            else // path okay
            {
                if (fpath.Contains("\\"))
                {
                    fpath = fpath.Replace("\\", "/");
                }
                bError = false;
                //----------------- LOAD IMAGE FILE ito PB[nextpb]
                //----------------- LOAD IMAGE FILE ito PB[nextpb]                 dmcdmc FIX THIS  if read < full screen previously .. next STEP1 will read too far

                if (bLoadFromStream)
                {
                    if (nextpb > maxPbIdxLoaded) //pbCreatedCount
                    {
                        bError = true;
                        bEOF = true;
                    }
                    else
                    {
                        // gv.debug.w("--LoadFromImageList with Stream method", fpath);
                        if (gv.bLoadFromStreamPreview)
                            LoadPbFromStream(nextpb, fpath);
                        else
                            LoadPbFromThumbnail(nextpb, fpath);
                    }
                }

                //-----------------made a load attempt
                //-----------------END LOAD IMAGE FILE ito PB[nextpb]
                if (!bError)
                {
                    if (nextpb > maxPbIdxLoaded) //pbCreatedCount
                    {
                        bError = true;
                        bEOF = true;
                    }
                    else
                    {
                        bGotImage = true;
                        lastLoadedIdx = idxImageFileItem1;
                        pbToolTip.SetToolTip(pb[nextpb], fpath);
                        // resizeImage(true, pb[nextpb]);
                    }
                }
                else
                    gv.debug.w("--ERROR >>>> LoadFromImageList with Stream method", fpath);
            }
            return lastLoadedIdx;
        }
        //-------------- LOADER 
        // load the set of PB images from ImageList
        //
        bool bDeleteInvalidImages = false;

        public int LoadFromImageList2(int iStartImageNum, int iStartPbNum) //// load images
        {
            bool bBreak = false; //break out of 2nd level
            string fpath;
            nextpb = 0;
            bool bError = false;
            int idxImageFileItem = iStartImageNum;
            int lastLoadedIdx = 0;
            int loadedCount = 0;

            if (bDisplayText)
                tbLoading.Visible = true;
            //  this.Refresh();

            maxImagesToLoad = numRows * numColumns;

            if (iStartImageNum < 0)
            {
                idxImageFileItem = 0;
                iStartPbNum = 0;
            }
            else if (iStartImageNum >= gv.slideCount1)
            {
                iStartImageNum = gv.slideCount1 - maxImagesToLoad - 1;
            }
            // counter position   -- idxImageFileItem;
            bool bEOF = false;
            for (int rowidx = 0; rowidx < numRows && !bEOF; ++rowidx) //rows
                for (int idx = 0; idx < numColumns && !bEOF; ++idx) //columns
                {
                    if (bBreak)
                        break;
                    bool bGotImage = false;
                    while (!bEOF && nextpb <= pbCreatedCount) // was pbCreatedCount imagesDisplayedCount  was  !bGotImage && 
                    {
                        if (idxImageFileItem < gv.slideCount1)
                        {
                            if (nextpb >= iStartPbNum && nextpb < maxImagesToLoad && nextpb <= pbCreatedCount) //dmc fix this
                            {
                                fpath = gv.imageFileList1.getIndexed(idxImageFileItem).fpath; ////////////////////////////////// iStartImageNum
                                if (fpath == null)
                                {
                                    gv.debug.w(" NULL FILEPATH ... ", gv.imageFileList1.getIndexed(idxImageFileItem).ToString());
                                    if (bDeleteInvalidImages)
                                        gv.imageFileList1.removeItemAt(idxImageFileItem);
                                }
                                else // path okay
                                {
                                    if (fpath.Contains("\\"))
                                    {
                                        fpath = fpath.Replace("\\", "/");
                                    }
                                    bError = false;
                                    //----------------- LOAD IMAGE FILE ito PB[nextpb]
                                    //----------------- LOAD IMAGE FILE ito PB[nextpb]

                                    if (bLoadFromStream)
                                    {
                                        /* if (nextpb > maxPbIdxLoaded) //pbCreatedCount
                                         {
                                             bError = true;
                                             bEOF = true;
                                         }*
                                         else*/
                                        {
                                            //------------------------------- LOAD Image into pb[nextpb] ----------------------------------- NEW LOADER
                                            //bool rc = LoadPbFromStream(nextpb, fpath);
                                            bool rc;
                                            if (gv.bLoadFromStreamPreview)
                                                rc = LoadPbFromStream(nextpb, fpath);
                                            else
                                                rc = LoadPbFromThumbnail(nextpb, fpath);
                                            
                                            bError = !rc;
                                        }
                                    }
                                    else
                                    {
                                        if (nextpb > maxPbIdxLoaded) //pbCreatedCount
                                        {
                                            bError = true;
                                            bEOF = true;
                                        }
                                        else
                                        {
                                            bool bRetry = false;
                                            //------------------------------- LOAD Image into pb[nextpb] -----------------------------------
                                            try
                                            {
                                                pb[nextpb].Load(fpath);//openFileDialog1.FileName); ///////////////////// nextpb 
                                            }
                                            catch (Exception)
                                            {
                                                this.Enabled = true;
                                                gv.debug.w("LoadFromImageList2 Load  Error.  File not found or Load Err:", fpath);
                                                bError = true;
                                                /*
                                                gv.imageFileList.markInvalid(true, idxImageFileItem);
                                                if (bDeleteInvalidImages)
                                                {
                                                    gv.imageFileList.updateDelete(true, idxImageFileItem, gv);
                                                    gv.imageFileList.removeItemAt(idxImageFileItem);
                                                }
                                                */
                                                bRetry = true;
                                            }
                                            if (bRetry)
                                            {
                                                Bitmap bmp = null;
                                                try
                                                {
                                                    bError = false;
                                                    bmp = new Bitmap(fpath);
                                                }
                                                catch (Exception)
                                                {
                                                    bError = true;
                                                    gv.debug.w("RETRY FAILED");
                                                }
                                                if (bRetry && !bError)
                                                {
                                                    pb[nextpb].Image = null;
                                                    pb[nextpb].Image = bmp;
                                                    gv.debug.w("RETRY SUCCESSFULL!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                                                }
                                            }
                                        }

                                    }
                                    //----------------AFTER >> -made a load attempt
                                    //-----------------END LOAD IMAGE FILE ito PB[nextpb]
                                    if (!bError)
                                        bGotImage = true;

                                    lastLoadedIdx = idxImageFileItem;
                                    pbToolTip.SetToolTip(pb[nextpb], fpath); /////////// nextpb 84 ?
                                                                             // resizeImage(true, pb[nextpb]);
                                    nextpb++;
                                    loadedCount = nextpb; ///////// actually counting nextpb
                                    idxImageFileItem++;

                                    if (bError) //bError
                                    {
                                        gv.debug.w("ERROR loading ", fpath, " into PB[", nextpb.ToString());
                                    }
                                    if (nextpb > maxImagesToLoad) //pbCreatedCount maxPbIdxLoaded
                                    {
                                        bEOF = true;
                                    }

                                }
                            }
                            else
                            {
                                bEOF = true;
                                gv.debug.w("--- loaded through imageFile IDX ", lastLoadedIdx.ToString());
                                bBreak = true;
                                break;
                            }


                        }
                        else
                        {
                            bEOF = true;
                            gv.debug.w("--- loaded through imageFile IDX ", lastLoadedIdx.ToString());
                            bBreak = true;
                            break;
                        }

                    } // while !bGotImage 

                }//for

            //imagesDisplayedCount = nextpb - 1; ///////// MAXPB - 1;
            maxPbIdxLoaded = loadedCount - 1;
            gv.debug.w("--LoadFromImageList2--  maxPbIdxLoaded", maxPbIdxLoaded);
            gv.debug.w("maxImagesToLoad", maxImagesToLoad);

            //Cursor.Position = new Point(pb[imgCount].Location.X, 10);
            gv.debug.w("imageFileIDX ", idxImageFileItem.ToString(), "<count--- loaded thru ImageFileIDX ", lastLoadedIdx.ToString());
            // this.Refresh();
            return lastLoadedIdx;
        }
        int THUMB_SIZE = 48; // 256;
        int THUMB_SIZE2 = 108; // 256;
        //Remember that you need to Dispose() the bitmap after using it.

        public bool LoadPbFromThumbnail(int pbx, string fpathname)
        {
            Bitmap thumbnail;
            thumbnail = WindowsThumbnailProvider.GetThumbnail(fpathname, THUMB_SIZE, THUMB_SIZE2, ThumbnailOptions.None);
            pb[pbx].Image = thumbnail;
            return (thumbnail != null);
        }
        public bool LoadPbFromStream(int pbx, string fpathname)
        {

            bool rc = true;

            // Open each file and display the image in pictureBox1.
            // Call Application.DoEvents to force a repaint after each
            // file is read.        
            System.IO.FileInfo fileInfo;
            System.IO.FileStream fileStream2;

            fileInfo = new System.IO.FileInfo(fpathname);
            fileStream2 = fileInfo.OpenRead();
            try
            {
                var oldImage = pb[pbx].Image;
                pb[pbx].Image = System.Drawing.Image.FromStream(fileStream2);
                if (oldImage != null)
                    oldImage.Dispose();
            }
            catch (global::System.Exception e)
            {
                //  gv.debug.wd("ERROR READING filestream into PB ", fpathname, 9);
                gv.debug.wd("LoadPbFromStream error>", fpathname, e.ToString(), 9);
                //MessageBox.Show("ERROR loading image");
                this.setTitle("ERROR loading image " + pbx.ToString());
                rc = false;
                //
                //
               // pb[pbx].Dispose();
               // pb[pbx] = new PictureBox();
                //Bitmap bmp = new Bitmap(fpathname);
                pb[pbx].Image = null;
                //
                //test
                /*
                        if (fileStream2.CanRead)
                        {
                            fileStream2.Close();
                            fileStream2 = fileInfo.OpenRead();
                            if (pb[pbx].Image != null)
                            {
                                pb[pbx].Image.Dispose();
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }
                            pb[pbx].Image = System.Drawing.Image.FromStream(fileStream2);
                        }
                */
            }
            //Application.DoEvents();
            fileStream2.Flush();
            fileStream2.Close();
            return rc;
            // Call Sleep so the picture is briefly displayed, 
            //which will create a slide-show effect.
            //pictureBox1.Image = null;
        }

        public int sLoadFromImageList2(int iStartImageNum, int iStartPbNum, bool bSingleLoad) //// load images
        {

            // 
            string fpath;
            nextpb = 0;
            int lastLoadedPb = 0;
            bool bError = false;
            int idxImageFileItem = iStartImageNum;

            if (iStartImageNum < 0 || iStartImageNum >= gv.slideCount1)
            {
                idxImageFileItem = 0;
                iStartPbNum = 0;
            }
            // counter position   -- idxImageFileItem;

            for (int rowidx = 0; rowidx < numRows; ++rowidx) //rows
                for (int idx = 0; idx < numColumns; ++idx) //columns
                {
                    bool bGotImage = false;
                    while (!bGotImage)
                    {
                        if (idxImageFileItem < gv.slideCount1)
                        {
                            if (nextpb >= iStartPbNum)
                            {
                                fpath = gv.imageFileList1.getIndexed(idxImageFileItem).fpath; ////////////////////////////////// iStartImageNum
                                if (fpath == null)
                                {
                                    gv.debug.w(" NULL FILEPATH ... ", gv.imageFileList1.getIndexed(idxImageFileItem).ToString());
                               //     gv.imageFileList.removeItemAt(idxImageFileItem);
                                }
                                else
                                {
                                    if (fpath.Contains("\\"))
                                    {
                                        fpath = fpath.Replace("\\", "/");
                                    }
                                    bError = false;
                                    try
                                    {
                                        pb[nextpb].Load(fpath);//openFileDialog1.FileName); ///////////////////// nextpb 
                                    }
                                    catch (Exception)
                                    {
                                        this.Enabled = true;
                                        gv.debug.w("LoadFromImageList2 Load  Error.  File not found or Load Err:", fpath);
                                        bError = true;
                                        /*
                                         * gv.imageFileList.markInvalid(true, idxImageFileItem);
                                        if (bDeleteInvalidImages)
                                        {
                                            gv.imageFileList.updateDelete(true, idxImageFileItem, gv);
                                            gv.imageFileList.removeItemAt(idxImageFileItem);
                                        }
                                        */
                                    }
                                    if (!bError)
                                    {
                                        bGotImage = true;
                                        pbToolTip.SetToolTip(pb[nextpb], fpath);
                                      //  resizeImage(true, pb[nextpb]);
                                        lastLoadedPb = nextpb;
                                        nextpb++;
                                        idxImageFileItem++;
                                    }
                                    if (bSingleLoad)
                                    {
                                        return idxImageFileItem;
                                    }

                                }
                            }


                        }
                    } // while !bGotImage 
                    
                }
            if (nextpb < MAXPB)
                for (int idx = nextpb; idx < MAXPB; ++idx)
                {
                    pb[idx].Image = null;
                }
            //imagesDisplayedCount = nextpb - 1; ///////// MAXPB - 1;
            maxPbIdxLoaded = lastLoadedPb;
            gv.debug.w("imagesDisplayedCount", maxPbIdxLoaded);

            //Cursor.Position = new Point(pb[imgCount].Location.X, 10);

            this.Refresh();
            return iStartImageNum;
        }



        FileInfoItem myinfo;

        public int showFpath(string fpath)
        {
            //FileInfoItem(string fname2, string fullpath, string dir, string ext2, string type2, int level2, long len2, DateTime datetime2, string ts2)
            myinfo = new FileInfoItem(fpath);

            this.Text = buildPictureLabel(finfo) + " " + displayName; /////////////////  IMAGE NAME SIZE PATH  ////////////////
            return 1;
        }

        public Boolean displayThisImage(string fpath) //// DISPLAY IMAGE after LOADING <<<<<<<<<<<<<<<<<< MAIN CALL TO DISPLAY THIS IMAGE << LOAD AND DISPLAY FPATH image
        {
            //setFullScreenMode(true);
            // 
            nextpb++;
            if (nextpb == 0)
            {
                pb[nextpb].Tag = fpath;
            }
            if (nextpb >= MAXPB)
            {
                nextpb = 0;
            }
            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }
            try
            {
                pb[nextpb].Load(fpath);//openFileDialog1.FileName);
            }
            catch (Exception)
            {
                MessageBox.Show("Error File not found " + fpath);
                return false;
            }
            pbToolTip.SetToolTip(pb[nextpb], fpath);

        //    resizeImage(true, pb[nextpb]);
            return true;
        }

        public string buildPictureLabel(FileInfoItem fi)
        {
            string title = String.Format("{0:###############} {1:###,###,###}    fpath = {2}", fi.fname, fi.len, fi.fpath);
            return title;
        }

        //setDisplay1Mode(PictureBoxSizeMode.AutoSize); --- this would resize the PB1 itself to the size of the image -- not useful here

        private void resizeImage(Boolean bestFit, PictureBox pb1)
        {
            bestFit = true;
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

                // gv.debug.w("resize bestFit");// String.Format("resize bestFit {0} has been accessed {1} times.", _principal.Identity.Name, _nAccesses);



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
                // gv.debug.w("resize custom");
                if (image != null)
                {
                    Graphics gdi = Graphics.FromImage(image);//this.pb.Image);
                }
                pb1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            // pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // fill
            // pb1.BorderStyle = BorderStyle.None; // BorderStyle.Fixed3D;

        }

        bool bPopupActive = false;
        private void DisplaySet_MouseDown(object sender, MouseEventArgs e) // RIGHT mouse click
        {
            /* popup is a ContextMenuStrip  * */
            // Called when either mouse button is pressed. 
            if (!bPopupActive)
                return;
            int newx = this.Location.X + e.X;
            int newy = this.Location.Y + e.Y;
            Point newloc = new Point(newx, newy);
            //this.Focus();
            //this.contextMenuStrip1.SetBounds(e.X + newx, e.Y + newy, 100, 100); //// right popup context menu
            gv.debug.w("!!!!!popup (contextMenuStrip) position", newx.ToString(), newy.ToString());


            contextMenuStrip1.Show(newloc);
            this.contextMenuStrip1.Visible = true;
        }

        private void DisplaySet_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            gv.debug.w("DisplaySet_MouseClick");
            if (e.Button.Equals(MouseButtons.Right))
            {
                DisplaySet_MouseDown(sender, e);
                return;
            }
            for (int idx = 0; idx < MAXPB; ++idx)
            {
                if (pb[idx] == null)
                    return;
                if (pb[idx].Bounds.Location.X <= x)
                    if (pb[idx].Bounds.Location.X + pbWidth >= x)
                        if (pb[idx].Bounds.Location.Y <= y)
                            if (pb[idx].Bounds.Location.Y + pbHeight >= y)
                            {
                               // this.Text = "disp set found pb " + idx.ToString();
                              //  MessageBox.Show("disp set found " + idx.ToString());
                                gv.debug.w("Preview Set ... mouse click on image number: " +idx.ToString());
                                return;
                                break;
                            }
            }
        }

        string fullpath;
        private void PbKey2(object sender, MouseEventArgs e)
        {
            gv.debug.w("PB KEY ", e.ToString());
        }
        private void PbMouseDoubleClicked(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            int idx = 0;
            gv.debug.w(">>PbMouseDoubleClicked<< pb DOUBLE clicked position eX eY", e.X.ToString(), e.Y.ToString());

            string msg = e.X.ToString() + " " + e.Y.ToString();
            Point pxy = PointToClient(System.Windows.Forms.Cursor.Position);

            x = pxy.X;
            y = pxy.Y;
            idx = getPB(x, y);
            gv.debug.w("pb DOUBLE clicked pb[", idx.ToString() + "]");
            if (idx < 0)
                return;
            lastSelectedPbIdx = idx;
            //dmc add here 
            if (lastSelectedPbIdx >= 0)
            {
                gv.debug.displayThisImage(pb[lastSelectedPbIdx].Image);
                int imageIdx = lastSelectedPbIdx + startingImageFileIdx;
                //  gv.nextIdx = imageIdx;
                gv.mainWindow.showNextSlideImageFileList1(imageIdx, 0);
            }
        }
        private void PbClicked(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            int idx = 0;
            gv.debug.w("method PbClicked: pb clicked position eX eY", e.X.ToString(), e.Y.ToString());

            string msg = e.X.ToString() + " " + e.Y.ToString();
            Point pxy = PointToClient(System.Windows.Forms.Cursor.Position);

            x = pxy.X;
            y = pxy.Y;
            idx = getPB(x, y);
            gv.debug.w("pb clicked pb[", idx.ToString() + "]");
            if (idx < 0)
                return;
            lastSelectedPbIdx = idx;
            //gv.main2.displayThisImage(fullpath);
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                MessageBox.Show("Pressed " + Keys.Shift);
            }
            else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
              //  MessageBox.Show("Control " + Keys.Control);
                gv.mainWindow.startSlideShow(idx + startingImageFileIdx,  true, 1);
                this.Close();
                return;
            }
            else if (e.Button.Equals(MouseButtons.Right))
            {
             //   MessageBox.Show("RIGHT MOUSE click");
                DisplaySet_MouseDown(sender, e);
                int newx = pb[idx].Location.X+ e.X;
                int newy = pb[idx].Location.Y + e.Y;
                Point newloc = new Point(newx, newy);
                //this.Focus();
                //this.contextMenuStrip1.SetBounds(e.X + newx, e.Y + newy, 100, 100); //// right popup context menu


                contextMenuStrip1.Show(newloc);
                this.contextMenuStrip1.Visible = true;

            }
            else
            {
                if (false)
                {
                    if (idx >= maxPbIdxLoaded)
                        idx = maxPbIdxLoaded - 1;
                    lastSelectedPbIdx = idx;
                    pb[idx].Select();
                    gv.debug.w("selected PB", idx.ToString(), " imagesDisplyed ", maxPbIdxLoaded.ToString()); ////////////// ------ SELECTED ---------------------
                    pb[idx].BorderStyle = BorderStyle.Fixed3D;
                    if (bcopy)
                        copyImage(pb[idx].ImageLocation, gv.dirTargetFolder);
                    else if (brename)
                        renameImage();
                }
            }
            this.Focus();
        }
        /*
         *            
         * idx = idx + startIdx;
           this.LoadSet(idx);
*/
                private int getPB(int x, int y)
                {

                    for (int idx = 0; idx < MAXPB && idx < pbCreatedCount; ++idx)
                    {
                        if (pb[idx].Bounds.Location.X <= x)
                            if (pb[idx].Bounds.Location.X + pbWidth >= x)
                                if (pb[idx].Bounds.Location.Y <= y)
                                    if (pb[idx].Bounds.Location.Y + pbHeight >= y)
                                    {
                                        // this.Text = "found pb " + idx.ToString() + " " + pb[idx].ImageLocation; ///////////////.Tag.ToString();
                                        //  MessageBox.Show("found " + idx.ToString());
                                        fullpath = pb[idx].ImageLocation;
                                        return idx;
                                    }
                    }
                    return -1;
                }

                public string getNextTarotName()
                {
                    if (bGetValue)
                    {
                        bGetValue = false;
                        renameSeq = gv.iValue;
                    }
                    sName = renameSeq.ToString();
                    if (sName.Length < 2)
                        sName = "0" + sName;
                    sName += ".jpg";
                    this.setTitle(sName);
                    return sName;
                }
        private void copyImage(string fpath, string targetDir)
                {
                    string newName = getNextTarotName();
                    gv.debug.w("copy ", newName);
                        ff.CopyFileRename(fpath, gv.dirTargetFolder, newName);
                        renameSeq++;
                }
                private void DisplaySet_MouseUp(object sender, MouseEventArgs e)
                {
                    int x = e.X;
                    int y = e.Y;

                    for (int idx = 0; idx < maxPbIdxLoaded; ++idx) // was PBMAX   was pbcreatedCount
                    {
                        if (pb[idx].Bounds.Location.X <= x)
                            if (pb[idx].Bounds.Location.X + pbWidth >= x)
                                if (pb[idx].Bounds.Location.Y <= y)
                                    if (pb[idx].Bounds.Location.Y + pbHeight >= y)
                                    {
                                        this.Text = "found pb " + idx.ToString();
                                        MessageBox.Show("found " + idx.ToString());
                                        break;
                                    }
                    }
                }
                public void MovePreviewCursor()
                {
                    Cursor.Position = new Point(5, 5);
                    //Cursor.Hide();
                }

        public int syncImageIdx()
        {
            hideDisplayFileName();
            this.LoadSet(gv.nextIdx);
            return startingImageFileIdx;
        }
        public int stepForward()
        {
            return syncImageIdx();
        }
        public int step()
        {
            hideDisplayFileName();
            this.LoadSet(this.startingImageFileIdx + 1);
            return startingImageFileIdx;
        }
        public int stepReverse()
        {
            hideDisplayFileName();
            this.LoadSet(this.startingImageFileIdx - 1);
            return startingImageFileIdx;
        }
                /*
            * through overriding the ProcessCmdKey() method of the form. That way, 
             * your key handling logic gets executed no matter what control 
            * has focus at the time of keypress. 
            * Beside that, you even get to choose whether the focused control gets the key 
            * after you processed it (return false) or not (return true).
            * */
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) /////// override dmcdmc123
        {
            gv.debug.w("ProcessCmdKey");
            if (keyData == Keys.F2)
            {
                this.Width = 1920;
                this.Height = 1080;
                ResizeDisplaySet();
            }
            else if (keyData == Keys.F5)
            {
                gv.mainWindow.Activate();
            }
            else if (keyData == Keys.F6)
            {
                for (int idx = 0; idx < this.maxPbIdxLoaded + 1; ++idx)
                {
                    pb[idx].Image = null;
                    pb[idx].Refresh();
                }
            }
            else if (keyData == Keys.Left)
            {
                // gv.debug.w("ProcessCmdKey  Key   Left"); //left arrow dmcdmc arrow 
                hideDisplayFileName();
                stopTimer();
                this.LoadSet(this.startingImageFileIdx - 1);
                return true; //for the active control to see the keypress, return false
            }
            else if (keyData == Keys.Right) //right arrow 
            {
                // gv.debug.w("ProcessCmdKey RIGHT");
                hideDisplayFileName();
                this.LoadSet(this.startingImageFileIdx + 1);
                return true; //for the active control to see the keypress, return false
            }
            else if (keyData == Keys.Down ) // down arrow 
            {
                // gv.debug.w("ProcessCmdKey  Key   DOWN");
                hideDisplayFileName();
                this.LoadSet(this.startingImageFileIdx + numColumns);
                // this.ScrollToControl(this.pb[imgCount]);
                return true; //for the active control to see the keypress, return false
            }
            else if ( keyData == Keys.PageDown)
            {
                // gv.debug.w("ProcessCmdKey  Key   PageDOWN");
                hideDisplayFileName();
                this.LoadSet(this.startingImageFileIdx + maxImagesToLoad); // was maxPbIdxLoaded
                // this.ScrollToControl(this.pb[imgCount]);
                return true; //for the active control to see the keypress, return false
            }
            else if (keyData == Keys.Up )
            {
                int nStart = this.startingImageFileIdx - numColumns;
                if (nStart < 0)
                    nStart = 0;
                hideDisplayFileName();
                this.LoadSet(nStart);
                return true;
                //   gv.debug.w("ProcessCmdKey  Key   UP");

              //  return true; //for the active control to see the keypress, return false
            }
            else if (keyData == Keys.PageUp)
            {
                int nStart = this.startingImageFileIdx - maxImagesToLoad;
                if (nStart < 0)
                    nStart = 0;
                hideDisplayFileName();
                this.LoadSet(nStart);
                return true;
                //   gv.debug.w("ProcessCmdKey  Key   UP");

                return true; //for the active control to see the keypress, return false
            }else if (keyData == Keys.End)
            {
                hideDisplayFileName();
                this.LoadSet(gv.slideCount1 - maxPbIdxLoaded);  //go to last page of images
                return true;
            }
            else if (keyData == Keys.Home)
            {
                hideDisplayFileName();
                this.LoadSet(0);  //go to last page of images
                return true;
            }
            else if (keyData == Keys.F) //show file names
            {
                bDisplayFileName = !bDisplayFileName;

                displayFileName(bDisplayFileName);
                return true;
            }
            else if (keyData == Keys.L)
            {
                hideDisplayFileName();
                this.ResizePreviewList(-1);
                return true;
            }
            //pbToolTip.ShowAlways = true;
            else if (keyData == Keys.H)
            {
                
                return true;
            }
            else if (keyData == Keys.R) // RUN SLIDE SHOW MODE 
            {
                bRunSlideShow = true;
                numToAdvance = maxImagesToLoad;
                startTimer(true);
                return true;
            }
            else if (keyData == Keys.T) // RUN SLIDE SHOW MODE 
            {
                bRunSlideShow = true;
                numToAdvance = maxImagesToLoad;
                startTimer();
                return true;
            }
            else if (keyData == Keys.S)
            {
                hideDisplayFileName();
                this.ResizePreviewList(1);
                return true;
            }
            else if (keyData == Keys.Z) //show file names
            {
                renumberAll();
                return true;
            }
            else if (keyData == Keys.Q) //exit
            {
                this.Close();
                return true;
            }

            return false;
        }
        private void stopTimer()
        {
            timer.Enabled = false;
        }
        private void startTimer(bool faster = false)
        {
            timer.Interval = 2000 * numRows;             // Timer will tick every 3 seconds
            timer.Enabled = true;                       // Enable the timer
            if (faster)
                timer.Interval = 1000 * numRows;
            timer.Start();                              // Start the timer
        }
        private void DisplaySet_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key_char = e.KeyChar;
            //   gv.debug.w(e.ToString());
            //    this.Focus();
            switch (key_char)
            {
                case '0':
                    gv.debug.displayThisImage(pb[0].Image);
                    gv.nextIdx = startingImageFileIdx;
                    gv.mainWindow.showNextSlideImageFileList1(startingImageFileIdx, 1);
                    this.LoadSet(this.startingImageFileIdx);
                    break;
                case '9':
                case '8':
                case '7':
                case '6':
                case '5':
                case '4':
                case '3':
                case '2':
                case '1':
                    //displayFileName(false);
                    int rowc = key_char - '0';
                    //gv.debug.w(key_char.ToString());
                    int prevnumRows = numRows;
                    SetColumnCount(rowc);
                    if (prevnumRows < rowc)
                        LoadSet(startingImageFileIdx);
                    ///ResizePreviewList(-1);
                    ///this.LoadSet(this.startingImageFileIdx);
                    break;
                case (char)Keys.Escape:
                    //gv.main2.Focus();
                    this.Close();
                    break;
                case 'a':
                    //gv.main2.Focus();
                    MovePreviewCursor();
                    break;
                case 'b':
                    borderB = !borderB;
                    this.ResizeDisplaySet();
                    break;
                case 'f':
                    // startSoundPlayer.Play();
                    //gv.debug.write("right arrow key pressed");
                    this.LoadSet(this.startingImageFileIdx + maxPbIdxLoaded);
                    break;
                case 'n':
                    // startSoundPlayer.Play();
                    //gv.debug.write("right arrow key pressed");
                    this.LoadSet(this.startingImageFileIdx + 1);

                    break;
                case '+':
                    // copy
                    this.copyImage(pb[0].ImageLocation, gv.initParm1List[0].targetDir1);
                    break;

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void DisplaySet_Move(object sender, EventArgs e)
        {
            return;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        // renaming  PB images 
        int renameSeq = 0;
        string sName;
        bool bcopy = false;
        bool brename = false;

        private void renameSeqToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bcopy)
            {
                bcopy = false;
                renameSeq = 0;
                gv.debug.w("RESET RENAME SEQ 0 and OFF");
                this.setTitle("Toggle OFF --- renaming ");
            }
            else
            {
                gv.debug.w("RENAMEcopy 0 > ON < to target ", gv.dirTargetFolder);
                bcopy = true;
                this.setTitle("Toggle ON --- select image to automatically rename with sequence number " + getNextTarotName());
            }
            
        }
        public void renumberAll()
        {
            string oldName;
            string newName;
            renameSeq = 0;
            for (int idx = 0; idx < gv.imageFileList1.getImageCount(); ++idx)
            {
                oldName = gv.imageFileList1.getIndexed(idx).fpath; //////sequence thru
                newName = getNextTarotName();
                newName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(oldName), newName);
                if (ff.renameFile(oldName, newName))
                {
                    pbToolTip.SetToolTip(pb[idx], newName);
                    gv.imageFileList1.updatePath(newName, getImageFileIdx(idx));
                    gv.debug.w("renamed ", oldName, " to ", newName);
                    renameSeq++;
                }
                else
                    gv.debug.w("rename failed for ", oldName);
            }
        }
        public void renameImage()
        {
            if (lastSelectedPbIdx < 0 || lastSelectedPbIdx >= maxPbIdxLoaded)
                return;

            string newName = getNextTarotName();
            string oldName = gv.imageFileList1.getIndexed(lastSelectedPbIdx).fpath;
            if (renameSeq == 999)
            {
                newName = "z" +  System.IO.Path.GetFileName(oldName);
            }
            else
            {
                renameSeq++;
            }
            newName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(oldName), newName);
            if (ff.renameFile(oldName, newName))
            {
                pbToolTip.SetToolTip(pb[lastSelectedPbIdx], newName);
                gv.imageFileList1.updatePath(newName , getImageFileIdx(lastSelectedPbIdx));
            }
            else
                gv.debug.w("rename failed for ", oldName);
        }
        // right click to RENAME PB with SEQUENCE
        private void renameItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            brename = !brename;
        }
        bool bGetValue = false;
        private void promptForSequenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PromptData p = new PromptData(gv);
            p.Visible = true;
            p.Activate();
            bGetValue = true;
        }
        private void toggleImageWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void display1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lastSelectedPbIdx >= 0)
            {
                gv.debug.displayThisImage(pb[lastSelectedPbIdx].Image);
                int imageIdx = lastSelectedPbIdx + startingImageFileIdx;
              //  gv.nextIdx = imageIdx;
              gv.mainWindow.showNextSlideImageFileList1(imageIdx, 0);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (lastSelectedPbIdx >= 0)
            {
                int imageIdx = lastSelectedPbIdx + startingImageFileIdx;
                startingImageFileIdx = imageIdx;
                hideDisplayFileName();
                this.LoadSet(this.startingImageFileIdx);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int idx = -1;
            int rc = -1;
            if (lastSelectedPbIdx >= 0)
            {
                idx = lastSelectedPbIdx + startingImageFileIdx;
                rc = gv.imageFileList1.removeItemAt(lastSelectedPbIdx); ///////////addItem((string)key, (string)all[key], level)
                gv.slideCount1 = rc;
                gv.debug.w("------ removed Item entry for deleted file gv.ImageFileList idx  and total now", lastSelectedPbIdx.ToString(), rc.ToString());
                this.LoadSet(this.startingImageFileIdx);
            }

        }

        private void DisplayPreviewSet_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            gv.debug.w("DisplayPreviewSet_MouseDoubleClick");
            if (e.Button.Equals(MouseButtons.Right))
            {
                DisplaySet_MouseDown(sender, e);
                return;
            }
            for (int idx = 0; idx < MAXPB; ++idx)
            {
                if (pb[idx] == null)
                    return;
                if (pb[idx].Bounds.Location.X <= x)
                    if (pb[idx].Bounds.Location.X + pbWidth >= x)
                        if (pb[idx].Bounds.Location.Y <= y)
                            if (pb[idx].Bounds.Location.Y + pbHeight >= y)
                            {
                                // this.Text = "disp set found pb " + idx.ToString();
                                //  MessageBox.Show("disp set found " + idx.ToString());
                                gv.debug.w("Preview Set ... mouse DOUBLE-CLICK  on image number: " + idx.ToString());
                                return;
                            }
            }
        }

        private void DisplayPreviewSet_DoubleClick(object sender, EventArgs e)
        {
            getWindowSize();
            computeResizeByRowCount(numRows);
            ResizeDisplaySet();

        }
    }
}

        
    

// This getYoungestChildUnderMouse(Control) method will recursively navigate a        
// control tree and return the deepest non-container control found under the cursor. 
// It will return null if there is no control under the mouse (the mouse is off the 
// form, or in an empty area of the form). 
// For example, this statement would output the name of the control under the mouse 
// pointer (assuming it is in some method of Windows.Form class): 
//  
// Console.Writeline(ControlNavigatorHelper.getYoungestChildUnderMouseControl(this).Name); 


public class ControlNavigationHelper
{
    public static Control getYoungestChildUnderMouse(Control topControl)
    {
        return ControlNavigationHelper.getYoungestChildAtDesktopPoint(topControl, System.Windows.Forms.Cursor.Position);
    }

    private static Control getYoungestChildAtDesktopPoint(Control topControl, System.Drawing.Point desktopPoint)
    {
        Control foundControl = topControl.GetChildAtPoint(topControl.PointToClient(desktopPoint));
        if ((foundControl != null) && (foundControl.HasChildren))
            return getYoungestChildAtDesktopPoint(foundControl, desktopPoint);
        else
            return foundControl;
    }
}
