#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace SymbolDB
{
    public partial class Display3AI : Form
    {
        private bool bHaveMouse;
        private System.Drawing.Point ptOriginal;
        private System.Drawing.Point ptLast;
        private Rectangle outlineBounds = Rectangle.Empty;
        public GlobalVars gv;

        public ImageFileList imageFileList1;
        public FileInfoItem finfo1;

        int slideCount1 = 0;
        int nextSlide1 = 0;

        public Display3AI(GlobalVars g)
        {
            InitializeComponent();
            gv = g;

            pb1.MouseDown += pb1_MouseDown;
            pb1.MouseMove += pb1_MouseMove;
            pb1.MouseUp += pb1_MouseUp;
            pb1.Paint += pb1_Paint;

            this.KeyPreview = true;

            PreSetupDGV(); //2025
            dgvFileInfo.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvFileInfo.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvFileInfo.Columns[1].MinimumWidth = 100;
            dgvFileInfo.Columns[1].FillWeight = 1;
        }

        public void Diplay3Finfo(ImageFileList fl)
        {
            imageFileList1 = new ImageFileList();
            imageFileList1 = fl;
            slideCount1 = imageFileList1.getImageCount();
            nextIdx = 0;
        }

        public int GetImageFileCount()
        {
            if (imageFileList1 != null)
                return imageFileList1.getImageCount();
            else
                return 0;
        }

        private void pb1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            bHaveMouse = true;
            ptOriginal = e.Location;
            ptLast = e.Location;
            outlineBounds = Rectangle.Empty;
            pb1.Invalidate();
        }

        private void pb1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!bHaveMouse)
                return;

            ptLast = e.Location;
            outlineBounds = new Rectangle(
                Math.Min(ptOriginal.X, ptLast.X),
                Math.Min(ptOriginal.Y, ptLast.Y),
                Math.Abs(ptLast.X - ptOriginal.X),
                Math.Abs(ptLast.Y - ptOriginal.Y)
            );
            pb1.Invalidate();
        }

        Bitmap Image = null;
        int nextIdx = 0;

        public int showNextImageFromList(int idx, int iDirection)
        {
            int rcIDX = 1;
            Boolean rc = false;
            bool bBadImage = false;

            if (idx < 0)
            {
                nextIdx = 0;
                return 0;
            }
            if (idx > slideCount1)
            {
                pb1.Image = Image;
                return gv.slideCount1;
            }
            nextIdx = idx;

            rcIDX = getImageInfoByIndex(nextIdx, iDirection, false);
            if (rcIDX < 0 || rcIDX > gv.slideCount1)
                return -1;
            FileInfoItem fi;
            fi = imageFileList1.getIndexed(rcIDX);
            if (fi.fpath == null)
            {
                nextIdx -= 1;
                return 0;
            }
            if (fi.fpath.Contains("mp4"))
                return 0;

            fi.ndx = rcIDX;

            if (rcIDX >= gv.slideCount1)
            {
                return 0;
            }
            updateDgvFinfo(fi, rcIDX);

            bool loadedImage = false;
            try
            {
                loadedImage = LoadImage(fi.fpath, fi);
            }
            catch (Exception ex)
            {
                this.Text = $"Exception pb1.Load {ex.Message}";
            }
            return rcIDX;
        }

        public int getImageInfoByIndex(int idx, int iDirection, bool previewGet)
        {
            gv.debug.w(String.Format(">>>>>>>>>>>>>>>>>>>>>-------------------------- get NextSlide>>>{0} << of MAX {1}", idx, (gv.slideCount1 - 1)));
            bool bGotImage = false;
            FileInfoItem tempFI = null;

            while (!bGotImage)
            {
                if (idx >= 0 && idx <= slideCount1)
                {
                    tempFI = gv.imageFileList1.getIndexed(idx);
                    if (!previewGet)
                        finfo1 = tempFI;

                    gv.debug.w($" got >> MAIN.getImageByIndex fname: {tempFI.fname}, rating:{tempFI.rating}, bInvalid={tempFI.bInvalid} preview={previewGet}");
                    return idx;
                }
                else if (idx > gv.slideCount1)
                {
                    return -1;
                }
                else if (idx < 0)
                {
                    return -1;
                }
                if (finfo1 == null || finfo1.fpath == null || tempFI == null)
                {
                    System.Windows.Forms.MessageBox.Show("ERROR invalid file info in getImageInfoByIndex");
                    if (idx > gv.imageFileList1.getImageCount())
                        return -1;
                    if (idx < 0)
                        return -1;
                }
            }
            return idx;
        }

        bool bCorrectedLastNbr = false;

        public bool DisplayContinueOrAbort(int ndx)
        {
            var result = System.Windows.Forms.MessageBox.Show("Do you want to continue (YES) or stop (NO)?", "Please Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.Yes)
            {
                nextIdx = 0;
                return true;
            }
            else
            {
                if (!bCorrectedLastNbr)
                {
                    bCorrectedLastNbr = true;
                    gv.iMaxFileCount = ndx;
                    gv.slideCount1 = gv.iMaxFileCount;
                    tbMaxSlideNumber.Text = gv.iMaxFileCount.ToString();
                    gv.nextIdx = ndx - 4;
                    showNextImageFromList(gv.nextIdx, gv.iMaxFileCount);
                    return false;
                }
            }
            return false;
        }

        private void pb1_Left(object sender, MouseEventArgs e)
        {
            showNextImageFromList(nextIdx, -1);
        }

        private void pb1_Right(object sender, MouseEventArgs e)
        {
            showNextImageFromList(nextIdx, 1);
        }

        private void pb1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!bHaveMouse)
                return;

            bHaveMouse = false;
            pb1.Invalidate();

            if (outlineBounds.Width > 0 && outlineBounds.Height > 0)
            {
                captureScreen(outlineBounds);
                showTag(outlineBounds);
                openGetInfoDialog();
            }
        }

        private void pb1_Paint(object sender, PaintEventArgs e)
        {
            if (bHaveMouse && outlineBounds != Rectangle.Empty)
            {
                using (var pen = new Pen(Color.Red) { DashStyle = DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(pen, outlineBounds);
                }
            }
        }

        public void SetImageList(ImageFileList list)
        {
            imageFileList1 = new ImageFileList();
            imageFileList1 = list;
            slideCount1 = GetImageFileCount();
            pb1.SizeMode = PictureBoxSizeMode.Zoom;
            showNextImageFromList(0, 1);
        }

        private void captureScreen(Rectangle bounds)
        {
            // existing implementation from Display1
        }

        private void showTag(Rectangle bounds)
        {
            // existing implementation from Display1
        }

        private void openGetInfoDialog()
        {
            // existing implementation from Display1
        }

        public void setTitle(FileInfoItem fi, string txt)
        {
            this.Text = $"file {buildPictureLabel(finfo1)}         {txt}";
        }

        public string buildPictureLabel(FileInfoItem fi)
        {
            string title = String.Format("image# {0:#,###,###} of {1:#,###,###} size={2:##,###,###} KB   >>{3}     {4}", nextIdx + 1, slideCount1, (fi.len / 1024) + 1, fi.fname, "       ", fi.fpath);
            return title;
        }

        string lastFolderPath = "";

        public void updateDgvFinfo(FileInfoItem iinfo, int idxRow = 0)
        {
            int idx = 0;
            setTitle(iinfo, iinfo.dpath);
            tbDescription.Text = iinfo.comment;

            try
            {
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.fname;
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.dpath;
                lastFolderPath = iinfo.dpath;

                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.fpath;
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.ext;
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.stimestamp;
                dgvFileInfo.Rows[idx++].Cells[1].Value = string.Format("{0:##,###,##0}", iinfo.len);

                if (pb1.Image != null)
                {
                    dgvFileInfo.Rows[idx++].Cells[1].Value = pb1.Image.Width.ToString();
                    dgvFileInfo.Rows[idx++].Cells[1].Value = pb1.Image.Height.ToString();
                    iinfo.width = pb1.Image.Width;
                    iinfo.height = pb1.Image.Height;
                }
                else
                {
                    dgvFileInfo.Rows[idx++].Cells[1].Value = "0";
                    dgvFileInfo.Rows[idx++].Cells[1].Value = "0";
                }

                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.rating;
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.bDelete;
            }
            catch (Exception ex)
            {
                gv.debug.w($"DataGridView row mismatch: {ex.Message}");
                return;
            }

            try
            {
                dgvFileInfo.Rows[idx++].Cells[1].Value = iinfo.bInvalid;
            }
            catch (Exception ex)
            {
                gv.debug.w($"DataGridView row mismatch (bInvalid): {ex.Message}");
            }

            dgvFileInfo.BackColor = iinfo.bInvalid ? Color.LightPink : Color.LightGray;

            if (nextIdx > slideCount1)
                nextIdx = slideCount1;

            tbSlideNumber.Text = string.Format("{0:###,###,##0}", nextIdx);
            tbImageNumber.Text = tbSlideNumber.Text;
            tbMaxSlideNumber.Text = string.Format("{0:###,###,###}", slideCount1 - 1);

            try
            {
                dgvFileInfo.Refresh();
            }
            catch (Exception e)
            {
                gv.debug.w("update error dgvFinfo: " + e.Message);
            }
        }

        string lastImage = "";
        Bitmap mainImage = null;
        long p1Size = 0;

        public bool LoadImage(string fpath, FileInfoItem fi = null)
        {
            bool loadedOK = true;
            lastImage = fpath;

            mainImage = LoadBitmap(fpath);
            pb1.Image = mainImage;
            if (fi != null)
                p1Size = fi.len;

            return loadedOK;
        }

        private Bitmap LoadBitmap(string imageFilePath)
        {
            using var fs = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var temp = System.Drawing.Image.FromStream(fs, useEmbeddedColorManagement: false, validateImageData: false);
            return new Bitmap(temp);
        }

        private void btShowNext_Click(object sender, EventArgs e)
        {
            if (slideCount1 == 0)
                return;
            showNextImageFromList(nextIdx + 1, 1);
        }

        string[] labels = {
            "file name",
            "folder",
            "full path",
            "ext",
            "timestamp",
            "length",
            "width",
            "height",
            "rating",
            "bDelete",
            "bInvalid"
        };

        public void PreSetupDGV()
        {
            dgvFileInfo.Columns.Clear();
            dgvFileInfo.Rows.Clear();
            dgvFileInfo.AllowUserToAddRows = false;
            dgvFileInfo.AllowUserToDeleteRows = false;
            dgvFileInfo.RowHeadersVisible = false;
            dgvFileInfo.ReadOnly = true;

            dgvFileInfo.Columns.Add("colLabel", "Label");
            dgvFileInfo.Columns.Add("colValue", "Value");

            foreach (var lbl in labels)
            {
                dgvFileInfo.Rows.Add(lbl, "");
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                    Arrow_Right();
                    return true;
                case Keys.Left:
                    Arrow_Left();
                    return true;
                case Keys.Escape:
                    this.Close();
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void Arrow_Right()
        {
            showNextImageFromList(++nextIdx, 1);
        }

        private void Arrow_Left()
        {
            showNextImageFromList(--nextIdx, -1);
        }

        private void btGoToSlideNumber_Click(object sender, EventArgs e)
        {
            GoToSlideNumber();
            enteringSlideNumber = false;
        }

        bool enteringSlideNumber = false;

        public void GoToSlideNumber()
        {
            int next = 0;
            enteringSlideNumber = false;
            if (string.IsNullOrEmpty(tbGoToSlide.Text))
                return;
            try
            {
                next = Convert.ToInt32(tbGoToSlide.Text);
            }
            catch
            {
                gv.debug.w("INVALID SLIDE NUMBER");
            }
            if (next >= 0 && next < gv.slideCount1)
            {
                showNextImageFromList(next, 1);
            }
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btResizePB_Click(object sender, EventArgs e)
        {
            if (pb1 == null) return;

            pb1.SuspendLayout();
            try
            {
                pb1.Left = 0;
                pb1.Width = this.ClientSize.Width;
                pb1.SizeMode = PictureBoxSizeMode.Zoom;
                pb1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            }
            finally
            {
                pb1.ResumeLayout();
            }
        }
    }
}
