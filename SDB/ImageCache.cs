using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolDB
{
    public class ImageCache : IDisposable
    {
        private readonly ImageFileList _fileList;
        public int _batchSize;
        private int _currentBatchStart = 0;
        private Bitmap[] _cache;
        private Task _preloadTask;
        private readonly object _lock = new object();

        public ImageCache(ImageFileList fileList, int batchSize = 132)
        {
            _fileList = fileList ?? throw new ArgumentNullException(nameof(fileList));
            _batchSize = batchSize;
            _cache = new Bitmap[_batchSize];
            // immediately load first batch
            PreloadBatchAsync(0);
        }

        /// <summary>
        /// Returns the image at the given global index, loading its batch if necessary.
        /// </summary>
        public Bitmap GetImage(int globalIndex)
        {
            int batchStart = (globalIndex / _batchSize) * _batchSize;
            int offset = globalIndex % _batchSize;

            // if we’re not already on that batch, synchronously load it
            if (batchStart != _currentBatchStart)
            {
                // wait for any in-flight preload
                _preloadTask?.Wait();
                PreloadBatchAsync(batchStart).Wait();
            }

            return _cache[offset];
        }

        /// <summary>
        /// Kicks off an async preload of the batch beginning at batchStart.
        /// </summary>
        public Task PreloadBatchAsync(int batchStart)
        {
            lock (_lock)
            {
                _currentBatchStart = batchStart;
                _preloadTask = Task.Run(() =>
                {
                    // dispose old images
                    foreach (var bmp in _cache)
                        bmp?.Dispose();

                    for (int i = 0; i < _batchSize; i++)
                    {
                        int idx = batchStart + i;
                        if (idx < _fileList.getImageCount())
                        {
                            var path = _fileList.getIndexed(idx).fpath;
                            try
                            {
                                _cache[i] = (Bitmap)Image.FromFile(path);
                            }
                            catch
                            {
                                _cache[i] = null;
                            }
                        }
                        else
                        {
                            _cache[i] = null;
                        }
                    }
                });
                return _preloadTask;
            }
        }

        public void Dispose()
        {
            _preloadTask?.Wait();
            foreach (var bmp in _cache)
                bmp?.Dispose();
        }
    }

}
