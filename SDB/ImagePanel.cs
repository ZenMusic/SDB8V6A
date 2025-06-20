using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SymbolDB
{
    public partial class ImagePanel : UserControl
    {
        GlobalVars gv;
        public Image image;
        public Image image2;
        int x, y, w, h;
        public PictureBoxSizeMode SizeMode = PictureBoxSizeMode.Normal;
        public ImagePanel()
        {
            InitializeComponent();
            init2();
        }
        public ImagePanel(GlobalVars g, Image img)
        {
            gv = g;
            image = img;
            InitializeComponent();
            init2();
        }

        public void init2()
        {
            x = 0;
            y = 0;
            w = this.Width;
            h = this.Height;
        }

        public void setImage(Image img)
        {
            image = img;
        }
        public void setImageBounds(Rectangle rec)
        {
            x = rec.X;
            y = rec.Y;
            w = rec.Width;
            h = rec.Height;
        }
        public void setImageBounds(int x2, int y2, int w2, int h2)
        {
            x = x2;
            y = y2;
            w = w2;
            h = h2;
        }
        public void redraw(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.White, this.ClientRectangle);
            if (image != null)
            {
                g.DrawImageUnscaled(image, x, y, w, h);

                //  g.FillRectangle(Brushes.Gray, ClientRectangle.Width 
            }
        }

        private void ImagePanel_Load(object sender, EventArgs e)
        {

        }
        public void LoadImage(Image fname)
        {

            Stream s = this.GetType().Assembly.GetManifestResourceStream("PictureViewer" + fname);
            Bitmap bmp = new Bitmap(s);
            s.Close();
            Graphics g = CreateGraphics();
            g.DrawImage(bmp, 0, 0);
            bmp.Dispose();
            g.Dispose();
        }
        public  void LoadImage(string fname)
        {
           
            Stream s = this.GetType().Assembly.GetManifestResourceStream("PictureViewer" + fname);
            Bitmap bmp = new Bitmap(s);
            s.Close();
            Graphics g = CreateGraphics();
            g.DrawImage(bmp, 0, 0);
           // bmp.Dispose();
            g.Dispose();
        }
    }
}


/*

Baramuse,

I would custom paint the splash screen in this case. What you do is
start by blanking out the image, or flooding the background with the color
you want. Once you do that, you can call the DrawImage property on the
Graphics class to draw your images. You want the overload that takes an
ImageAttributes instance. You would construct this instance and set the
ColorMatrix in the image so that it has an alpha value.

Then, as time progresses, you would fade out one image (set the alpha in
increments to 100) and fade in the other (set the alpha in increments to 0).

Check out the document on MSDN titled "Using a Color Matrix to Set Alpha
Values in Images", located at (watch for line wrap):

http://msdn.microsoft.com/library/de..._usecsharp.asp


*/