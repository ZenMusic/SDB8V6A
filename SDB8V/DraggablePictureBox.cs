    using System;
    using System.Drawing;
    using System.Windows.Forms;

namespace SymbolDB
{
    public class ZoomablePictureBox : PictureBox
    {
        public bool bEnableMomentumScrolling = true;
        public bool bEnableElasticEdges = true;
        public bool bZoomToFill = false;      // false = Fit, true = Fill
        public float MaxZoom = 4.0f;          // 4x
        public float MinZoom = 0.1f;          // 10%

        private bool dragging = false;
        private Point dragStart = Point.Empty;
        private Point imageOffset = Point.Empty;

        private bool imageInitialized = false;

        // Zoom state
        private float currentScale = 1.0f;
        private float fitScale = 1.0f;
        private float fillScale = 1.0f;
        private float previousScale = 1.0f;

        // Momentum
        private Timer momentumTimer;
        private float velocityX = 0f;
        private float velocityY = 0f;
        private const float friction = 0.90f;

        public ZoomablePictureBox()
        {
            this.DoubleBuffered = true;
            this.SizeMode = PictureBoxSizeMode.Normal;

            momentumTimer = new Timer();
            momentumTimer.Interval = 16; // ~60 FPS
            momentumTimer.Tick += MomentumTimer_Tick;

            this.MouseWheel += ZoomablePictureBox_MouseWheel;
            this.MouseDoubleClick += ZoomablePictureBox_MouseDoubleClick;
            this.MouseClick += ZoomablePictureBox_MouseClick;
            this.MouseEnter += ZoomablePictureBox_MouseEnter;
        }

        private void ZoomablePictureBox_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            imageInitialized = false;
            this.Invalidate();
        }

        private void InitImageLayout()
        {
            if (this.Image == null || this.Width == 0 || this.Height == 0)
                return;

            ComputeScales();
            currentScale = bZoomToFill ? fillScale : fitScale;
            previousScale = currentScale;
            CenterImage();
        }

        private void ComputeScales()
        {
            if (this.Image == null || this.Width == 0 || this.Height == 0)
                return;

            float sx = (float)this.Width / (float)this.Image.Width;
            float sy = (float)this.Height / (float)this.Image.Height;

            fitScale = Math.Min(sx, sy);
            fillScale = Math.Max(sx, sy);

            if (fitScale <= 0f) fitScale = 0.01f;
            if (fillScale <= 0f) fillScale = fitScale;
        }

        private Size GetScaledImageSize()
        {
            if (this.Image == null)
                return Size.Empty;

            int w = (int)(this.Image.Width * currentScale);
            int h = (int)(this.Image.Height * currentScale);
            return new Size(w, h);
        }

        private void CenterImage()
        {
            Size sz = GetScaledImageSize();
            imageOffset = new Point(
                (this.Width - sz.Width) / 2,
                (this.Height - sz.Height) / 2
            );
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.Image == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragStart = e.Location;

                momentumTimer.Stop();
                velocityX = 0f;
                velocityY = 0f;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (dragging && this.Image != null)
            {
                int dx = e.X - dragStart.X;
                int dy = e.Y - dragStart.Y;

                imageOffset.X += dx;
                imageOffset.Y += dy;

                dragStart = e.Location;

                velocityX = dx;
                velocityY = dy;

                ApplyBounds(false);
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                dragging = false;

                if (bEnableMomentumScrolling &&
                    (Math.Abs(velocityX) > 0.1f || Math.Abs(velocityY) > 0.1f))
                {
                    momentumTimer.Start();
                }
            }
        }

        private void MomentumTimer_Tick(object sender, EventArgs e)
        {
            imageOffset.X += (int)velocityX;
            imageOffset.Y += (int)velocityY;

            velocityX *= friction;
            velocityY *= friction;

            ApplyBounds(true);

            if (Math.Abs(velocityX) < 0.1f && Math.Abs(velocityY) < 0.1f)
            {
                momentumTimer.Stop();
            }

            this.Invalidate();
        }

        private void ZoomablePictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
                return;

            float oldScale = currentScale;
            float zoomFactor = (e.Delta > 0) ? 1.1f : 0.9f;

            float newScale = oldScale * zoomFactor;
            newScale = Math.Max(MinZoom, Math.Min(MaxZoom, newScale));

            if (Math.Abs(newScale - oldScale) < 0.0001f)
                return;

            // Snap near 100%
            if (Math.Abs(newScale - 1.0f) < 0.05f)
                newScale = 1.0f;

            PointF imgPointBefore = ScreenToImage(e.Location);

            currentScale = newScale;

            PointF imgPointAfter = imgPointBefore;
            Point newOffset = new Point(
                (int)(e.X - imgPointAfter.X * currentScale),
                (int)(e.Y - imgPointAfter.Y * currentScale)
            );

            imageOffset = newOffset;

            ApplyBounds(false);
            this.Invalidate();
        }

        private void ZoomablePictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                // FIT → 100%
                if (Math.Abs(currentScale - fitScale) < 0.01f)
                {
                    previousScale = fitScale;
                    currentScale = 1.0f;
                    CenterImage(); // Option B: zoom to center
                }
                // 100% → PREVIOUS
                else if (Math.Abs(currentScale - 1.0f) < 0.01f)
                {
                    currentScale = previousScale;
                    CenterImage();
                }
                // PREVIOUS → FIT
                else
                {
                    previousScale = currentScale;
                    currentScale = fitScale;
                    CenterImage();
                }

                ApplyBounds(false);
                this.Invalidate();
            }
        }

        private void ZoomablePictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                ComputeScales();
                currentScale = bZoomToFill ? fillScale : fitScale;
                CenterImage();
                ApplyBounds(false);
                this.Invalidate();
            }
        }

        private PointF ScreenToImage(Point p)
        {
            if (this.Image == null || currentScale == 0f)
                return PointF.Empty;

            return new PointF(
                (p.X - imageOffset.X) / currentScale,
                (p.Y - imageOffset.Y) / currentScale
            );
        }

        private void ApplyBounds(bool fromMomentum)
        {
            if (this.Image == null)
                return;

            Size imgSize = GetScaledImageSize();

            int boxW = this.Width;
            int boxH = this.Height;

            int minX = boxW - imgSize.Width;
            int minY = boxH - imgSize.Height;
            int maxX = 0;
            int maxY = 0;

            if (imgSize.Width <= boxW)
            {
                imageOffset.X = (boxW - imgSize.Width) / 2;
            }
            else
            {
                if (bEnableElasticEdges && fromMomentum)
                {
                    int elastic = boxW / 4;
                    if (imageOffset.X > maxX + elastic) imageOffset.X = maxX + elastic;
                    if (imageOffset.X < minX - elastic) imageOffset.X = minX - elastic;
                }
                else
                {
                    if (imageOffset.X > maxX) imageOffset.X = maxX;
                    if (imageOffset.X < minX) imageOffset.X = minX;
                }
            }

            if (imgSize.Height <= boxH)
            {
                imageOffset.Y = (boxH - imgSize.Height) / 2;
            }
            else
            {
                if (bEnableElasticEdges && fromMomentum)
                {
                    int elastic = boxH / 4;
                    if (imageOffset.Y > maxY + elastic) imageOffset.Y = maxY + elastic;
                    if (imageOffset.Y < minY - elastic) imageOffset.Y = minY - elastic;
                }
                else
                {
                    if (imageOffset.Y > maxY) imageOffset.Y = maxY;
                    if (imageOffset.Y < minY) imageOffset.Y = minY;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Image != null && !imageInitialized)
            {
                InitImageLayout();
                imageInitialized = true;
            }

            if (this.Image == null)
            {
                base.OnPaint(e);
                return;
            }

            e.Graphics.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            Size sz = GetScaledImageSize();
            Rectangle dest = new Rectangle(imageOffset, sz);
            e.Graphics.DrawImage(this.Image, dest);
        }
    }
}
