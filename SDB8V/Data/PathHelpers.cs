using System;
using System.Data;
using System.IO;
using System.Linq;

namespace SymbolDB
{
    public static class PathHelpers
    {
        /// <summary>
        /// Build a stable key for a full Windows path:
        /// - Normalizes slashes to '\'
        /// - Resolves to a full absolute path
        /// - Removes the drive/root (e.g. "C:\")
        /// - Keeps original case (no ToUpper)
        /// - Ensures it starts with a leading '\'
        /// 
        /// Example:
        ///   "C:\\Users\\David\\Videos\\Recording 2025-08-22 203031.mp4"
        /// -> "\\Users\\David\\Videos\\Recording 2025-08-22 203031.mp4"
        /// </summary>
        /// 
        static GlobalVars gv;

        public static string ToNormalizedKey(string? fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return string.Empty;

            // normalize slashes to '\'
            string path = fullPath.Replace('/', '\\');

            // resolve relative pieces, "..", etc.
            try
            {
                path = Path.GetFullPath(path);
            }
            catch
            {
                // if invalid, just fall back to what we have
            }

            // strip drive / root ("C:\" or "\\SERVER\Share\")
            string? root = Path.GetPathRoot(path); // e.g. "C:\"
            if (!string.IsNullOrEmpty(root) && path.Length > root.Length)
            {
                path = path.Substring(root.Length); // "Users\David\Videos\..."
            }

            // ensure leading '\'
            if (!path.StartsWith("\\"))
                path = "\\" + path;

            // IMPORTANT: do NOT change case; we want case-preserving
            return path;
        }
        public static string NormalizeFolderPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            path = path.Trim();

            // Check if it's a drive letter without backslash (e.g., "C:", "Y:")
            if (path.Length == 2 && path[1] == ':')
            {
                return path + @"\";
            }

            // Ensure trailing backslash for consistency
            return path.TrimEnd('\\') + @"\";
        }
        public static void NormalizeFolderHistoryPaths(GlobalVars g)
        {
            gv = g;
            foreach (var item in gv.folderHistoryList)
            {
                item.folderPath = NormalizeFolderPath(item.folderPath);
            }

            // Remove duplicates that may now be identical after normalization
            gv.folderHistoryList = gv.folderHistoryList
                .GroupBy(fh => fh.folderPath, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.OrderByDescending(fh => fh.lastAccessDate).First())
                .ToList();
        }
        public static string ToNormalizedKeywas(string? fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return string.Empty;

            // Trim whitespace and normalize separators to '\'
            var path = fullPath.Trim().Replace('/', '\\');

            // Remove drive or UNC root
            var root = Path.GetPathRoot(path);  // "C:\", or "\\server\share\"
            if (!string.IsNullOrEmpty(root))
            {
                // Path.GetPathRoot always ends with '\'
                path = path.Substring(root.Length);
            }

            // Remove any leading '\' in remainder, then add exactly one
            path = path.TrimStart('\\');

            if (path.Length == 0)
                return "\\";    // extremely rare case, but safe

            return "\\" + path;
        }
    }
}
