using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SymbolDB
{
    /// <summary>
    /// Finds all images under a directory whose pixel‐difference score against
    /// a “main” image is ≥ threshold, skipping files of a different file‐size.
    /// Reports progress and supports cancellation.
    /// </summary>
    public class ImageMatcher
    {
        private readonly int _thumbSize;
        private readonly int _tolerance;

        /// <summary>
        /// Called every time another batch of 1 000 files have been scanned.
        /// (totalFilesSeen, totalMatchesFound)
        /// </summary>
        public Action<int, int> ProgressCallback { get; set; }

        public ImageMatcher(int thumbSize = 128, int tolerance = 10)
        {
            _thumbSize = thumbSize;
            _tolerance = tolerance;
        }
        int updateAfterCount = 100;
        /// <summary>
        /// Scan every image under <paramref name="rootDirectory"/>,
        /// compare to <paramref name="mainImage"/>, and return all
        /// file paths whose match fraction ≥ <paramref name="threshold"/>.
        /// </summary>
        public List<string> FindMatches(
            Bitmap mainImage,
            string rootDirectory,
            string sourceImagePath,
            double threshold,
            CancellationToken cancellationToken,
            IEnumerable<string> candidateFiles = null)
        {
            // 1) enumerate only image extensions
            var exts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };

            var allFiles = candidateFiles?.ToList() ?? Directory
                .EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories)
                .Where(f => exts.Contains(Path.GetExtension(f)))
                .ToList();

            // 2) prepare locked bits of the “main” thumbnail
            using var resizedMainBmp = new Bitmap(mainImage, new Size(_thumbSize, _thumbSize));
            var mainData = resizedMainBmp.LockBits(
                new Rectangle(0, 0, _thumbSize, _thumbSize),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            var matches = new ConcurrentBag<string>();
            int seen = 0;
            int matched = 0;
          //  int reportInterval = 100;
            var po = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationToken
            };
            try
            {
                Parallel.ForEach(allFiles, po, file =>
                {
                    po.CancellationToken.ThrowIfCancellationRequested();

                    int count = Interlocked.Increment(ref seen);
                    int reportInterval = (count <= 1_000) ? 100 : 1_000;   // declared once here

                    // skip the source image itself
                    if (string.Equals(file, sourceImagePath, StringComparison.OrdinalIgnoreCase))
                    {
                        if (count % reportInterval == 0)
                            ProgressCallback?.Invoke(count, matched);
                        return;
                    }

                    // load & compare
                    double score = 0;
                    try
                    {
                        using var testOrig = LoadImageNoLock(file);
                        using var thumbTest = new Bitmap(testOrig, new Size(_thumbSize, _thumbSize));
                        score = CompareLocked(mainData, thumbTest);
                    }
                    catch
                    {
                        // skip on any error
                    }

                    // record match
                    if (score >= threshold)
                    {
                        matches.Add(file);
                        Interlocked.Increment(ref matched);
                    }

                    if (count % reportInterval == 0)
                        ProgressCallback?.Invoke(count, matched);
                });



            }
            catch (OperationCanceledException) 
            {
                // swallow — we’ll return whatever we’ve collected so far
            }


            resizedMainBmp.UnlockBits(mainData);
            return matches.ToList();
        }

        /// <summary>
        /// Deletes every file in the list (ignores failures).
        /// </summary>
        public void DeleteMatches(IEnumerable<string> files)
        {
            foreach (var f in files)
            {
                try { File.Delete(f); }
                catch { /* swallow or log */ }
            }
        }

        /// <summary>
        /// Core pixel‐difference routine: locks the test bitmap internally,
        /// walks both 32bpp ARGB buffers, and returns a 0…1 match fraction.
        /// </summary>
        private unsafe double CompareLocked(BitmapData mainData, Bitmap testBmp)
        {
            int w = mainData.Width;
            int h = mainData.Height;
            int stride = Math.Abs(mainData.Stride);

            // lock the test thumbnail
            var testData = testBmp.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            byte* p1 = (byte*)mainData.Scan0;
            byte* p2 = (byte*)testData.Scan0;

            long diffSum = 0;

            for (int y = 0; y < h; y++)
            {
                byte* row1 = p1 + y * stride;
                byte* row2 = p2 + y * stride;

                for (int x = 0; x < w; x++)
                {
                    int i = x * 4;   // B,G,R,A
                    int db = row1[i + 0] - row2[i + 0];
                    int dg = row1[i + 1] - row2[i + 1];
                    int dr = row1[i + 2] - row2[i + 2];
                    diffSum += Math.Abs(db) + Math.Abs(dg) + Math.Abs(dr);
                }
            }

            testBmp.UnlockBits(testData);

            // maximum possible diff = w*h * 255 * 3
            double maxDiff = (double)w * h * 255 * 3;
            double fractionMatch = 1.0 - (diffSum / maxDiff);

            return fractionMatch;
        }

        private static Image LoadImageNoLock(string path)
        {
            // open for read with shared read so other processes can access file
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var img = Image.FromStream(fs);
            // return a new Bitmap copy so we can close the stream immediately
            return new Bitmap(img);
        }
    }
}
