using System;
using System.IO;
using System.Threading.Tasks;

namespace SymbolDB.Services
{
    public static class AsyncFileCopier
    {
        /// <summary>
        /// Copies a file asynchronously in buffered chunks, reporting progress.
        /// </summary>
        /// <param name="sourcePath">Path to the source file.</param>
        /// <param name="destPath">Path to the destination file.</param>
        /// <param name="progress">
        ///     Progress reporter (0.0–1.0). Pass null if you don’t need progress updates.
        /// </param>
        public static async Task CopyFileAsync(
            string sourcePath,
            string destPath,
            IProgress<double>? progress = null)
        {
            const int BufferSize = 1024 * 1024; // 1 MB chunks

            using var sourceStream = new FileStream(
                sourcePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                useAsync: true);

            using var destStream = new FileStream(
                destPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                BufferSize,
                useAsync: true);

            long totalBytes = sourceStream.Length;
            long bytesCopied = 0;
            var buffer = new byte[BufferSize];

            int read;
            while ((read = await sourceStream
                    .ReadAsync(buffer, 0, BufferSize)
                    .ConfigureAwait(false)) > 0)
            {
                await destStream
                      .WriteAsync(buffer, 0, read)
                      .ConfigureAwait(false);

                bytesCopied += read;
                progress?.Report(bytesCopied / (double)totalBytes);
            }

        }
    }
}
