using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymbolDB
{
    public static class ImageResizer
    {
        public static void ResizeToMinimum(
            string inputPath,
            string outputPath,
            int minWidth,
            int minHeight)
        {
            // If no output path is explicitly provided, prompt the user to choose the folder
            if (string.IsNullOrEmpty(outputPath))
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select the folder to save the resized image";
                    folderDialog.UseDescriptionForTitle = true;
                    folderDialog.ShowNewFolderButton = true;

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFolder = folderDialog.SelectedPath;
                        string fileName = Path.GetFileName(inputPath);
                        outputPath = Path.Combine(selectedFolder, fileName);
                    }
                    else
                    {
                        // User cancelled the operation
                        return;
                    }
                }
            }

            using (Image original = Image.FromFile(inputPath))
            {
                int originalWidth = original.Width;
                int originalHeight = original.Height;

                double scaleWidth = (double)minWidth / originalWidth;
                double scaleHeight = (double)minHeight / originalHeight;

                // Use the larger scale so BOTH width and height meet the minimum.
                double scale = Math.Max(scaleWidth, scaleHeight);

                // Do not shrink images that are already large enough.
                if (scale < 1.0)
                    scale = 1.0;

                int newWidth = (int)Math.Ceiling(originalWidth * scale);
                int newHeight = (int)Math.Ceiling(originalHeight * scale);

                using (Bitmap resized = new Bitmap(newWidth, newHeight))
                {
                    resized.SetResolution(
                        original.HorizontalResolution,
                        original.VerticalResolution);

                    using (Graphics graphics = Graphics.FromImage(resized))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        graphics.DrawImage(original, 0, 0, newWidth, newHeight);
                    }

                    SaveImage(resized, outputPath);
                }
            }
        }

        private static void SaveImage(Bitmap image, string outputPath)
        {
            string extension = Path.GetExtension(outputPath).ToLowerInvariant();

            if (extension == ".jpg" || extension == ".jpeg")
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                using (EncoderParameters encoderParams = new EncoderParameters(1))
                {
                    encoderParams.Param[0] =
                        new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 95L);

                    image.Save(outputPath, jpgEncoder, encoderParams);
                }
            }
            else if (extension == ".png")
            {
                image.Save(outputPath, ImageFormat.Png);
            }
            else
            {
                throw new ArgumentException("Output file must be .jpg, .jpeg, or .png");
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }

            throw new InvalidOperationException("Encoder not found.");
        }
    }
}