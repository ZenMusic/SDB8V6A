using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    public static class ImageCompare
    {
        /// <summary>
        /// Compare two images by resizing both to <paramref name="thumbSize"/>²,
        /// then walking their 32-bpp ARGB buffers. Returns the percent of pixels
        /// whose average RGB difference exceeds <paramref name="tolerance"/>.
        /// </summary>
        public unsafe static double CompareImages(
            Bitmap src1,
            Bitmap src2,
            int tolerance = 10,
            int thumbSize = 128)
        {
            if (src1 == null || src2 == null)
                return -1;

            // 1) Resize both to thumbSize×thumbSize:
            using var img1 = new Bitmap(src1, thumbSize, thumbSize);
            using var img2 = new Bitmap(src2, thumbSize, thumbSize);

            var rect = new Rectangle(0, 0, thumbSize, thumbSize);

            // 2) Lock bits (32bpp ARGB)
            var bd1 = img1.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = img2.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte* p1 = (byte*)bd1.Scan0;
            byte* p2 = (byte*)bd2.Scan0;

            int stride = Math.Abs(bd1.Stride);
            long diffCount = 0;

            // 3) Walk the buffer in one pass (4 bytes per pixel: B,G,R,A)
            //    skip alpha channel (at offset+3)
            for (int y = 0; y < thumbSize; y++)
            {
                byte* row1 = p1 + y * stride;
                byte* row2 = p2 + y * stride;

                for (int x = 0; x < thumbSize; x++)
                {
                    int i = x * 4;
                    int b1 = row1[i + 0], g1 = row1[i + 1], r1 = row1[i + 2];
                    int b2 = row2[i + 0], g2 = row2[i + 1], r2 = row2[i + 2];

                    int dr = r1 - r2; if (dr < 0) dr = -dr;
                    int dg = g1 - g2; if (dg < 0) dg = -dg;
                    int db = b1 - b2; if (db < 0) db = -db;

                    int avgDiff = (dr + dg + db) / 3;
                    if (avgDiff > tolerance) diffCount++;
                }
            }

            // 4) Unlock and compute percent
            img1.UnlockBits(bd1);
            img2.UnlockBits(bd2);

            double total = (double)thumbSize * thumbSize;
            return (diffCount * 100.0) / total;
        }
    }
}
