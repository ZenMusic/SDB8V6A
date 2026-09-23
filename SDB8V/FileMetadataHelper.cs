using System;
using System.Runtime.InteropServices;
using System.IO;

internal static class FileMetadataHelper
{
    [StructLayout(LayoutKind.Sequential)]
    private struct Win32FileAttributeData
    {
        public FileAttributes dwFileAttributes;
        public FILETIME ftCreationTime;
        public FILETIME ftLastAccessTime;
        public FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FILETIME
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;
    }

    private enum GetFileExInfoLevels : int
    {
        GetFileExInfoStandard = 0
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetFileAttributesEx(
        string lpFileName,
        GetFileExInfoLevels fInfoLevelId,
        out Win32FileAttributeData fileData);

    // Returns (lengthUtc, lastWriteUtc); returns (null, null) on error.
    public static (long? length, DateTime? lastWriteUtc) TryGetMetadata(string path)
    {
        if (string.IsNullOrEmpty(path))
            return (null, null);

        if (!GetFileAttributesEx(path, GetFileExInfoLevels.GetFileExInfoStandard, out var data))
            return (null, null);

        long length = ((long)data.nFileSizeHigh << 32) | data.nFileSizeLow;
        long fileTime = ((long)data.ftLastWriteTime.dwHighDateTime << 32) | data.ftLastWriteTime.dwLowDateTime;
        // Convert FILETIME (UTC 100-ns since 1601) to DateTime (UTC)
        DateTime lastWriteUtc = DateTime.FromFileTimeUtc(fileTime);

        return (length, lastWriteUtc);
    }
}