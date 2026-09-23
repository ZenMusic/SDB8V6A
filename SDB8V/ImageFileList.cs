#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SymbolDB
{
    public class ImageFileList
    {
        // public BindingList<FileInfoItem> finfoList = new BindingList<FileInfoItem>();
        public List<FileInfoItem> finfoList = new List<FileInfoItem>();

        FileInfoItem none = new FileInfoItem();

        public ImageFileList()
        {
        }
        /// <summary>
        /// Fast bulk-add of TraverserBG5 results.
        /// Minimizes allocations and completely bypasses addItem().
        /// </summary>
        public void AddResultsFastxxx(List<TraverserBG5.FileResult> results, bool includeFileMetadata = false)
        {
            if (results == null || results.Count == 0)
                return;

            // Pre-grow the underlying list to avoid repeated reallocations
            if (finfoList.Capacity < finfoList.Count + results.Count)
            {
                finfoList.Capacity = finfoList.Count + results.Count;
            }

            foreach (var r in results)
            {
                var item = new FileInfoItem
                {
                    // Basic info derivable from path
                    fpath = r.Path,
                    dpath = Path.GetDirectoryName(r.Path),
                    fname = Path.GetFileName(r.Path),
                    ext = Path.GetExtension(r.Path),
                    type = r.Kind,   // "f" or "d"
                    level = r.Level,

                    // Reasonable defaults for the rest.
                    // You can tweak these if your logic depends on them.
                    rating = ' ',
                    bDelete = false,
                    bInvalid = false,
                    width = 0,
                    height = 0,
                    source = string.Empty,
                    playTime = 0,
                    minutes = 0,
                    seconds = 0,
                    ndx = 0,
                    comment = string.Empty,
                    stimestamp = string.Empty
                };

                if (includeFileMetadata)
                {
                    try
                    {
                        var fi = new FileInfo(r.Path);
                        item.len = fi.Length;

                        var ts = fi.LastWriteTime;
                        item.stimestamp = ts.ToString("yyyy-MM-dd HH:mm:ss");
                        // if you want, you can also store ts in a public property
                    }
                    catch
                    {
                        // swallow I/O errors here – keep the scan robust
                    }
                }

                finfoList.Add(item);
            }
        }
        public ImageFileList(string fpath, string type1, int level1)
        {
            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }
            finfoList.Add(new FileInfoItem(fpath, type1, level1));

        }
        public List<FileInfoItem> getFinfo()
        {
            return finfoList;
        }
        public FileInfoItem getFinfoItem(int idx)
        {
            return finfoList[idx];
        }
        public bool FindFolder(string path)
        {
            var value = finfoList.FirstOrDefault(item => item.dpath == path);
            return (value != null);
        }
        public bool FindItemAndDelete(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // normalize incoming path
            string normalizedInput = Path
                .GetFullPath(path)
                .TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);

            for (int i = 0; i < finfoList.Count; i++)
            {
                var fi = finfoList[i];
                if (string.IsNullOrEmpty(fi.fpath))
                    continue;

                // normalize stored path
                string normalizedStored = Path
                    .GetFullPath(fi.fpath)
                    .TrimEnd(System.IO.Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                if (string.Equals(normalizedStored, normalizedInput, StringComparison.OrdinalIgnoreCase))
                {
                    finfoList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void AddRange(List<(string Path, string Kind, int Level)> rows)
        {
            foreach (var r in rows)
                addItem(r.Path, r.Kind, r.Level); // your existing method
        }

        public int addItem(string fpath, string type1, int level1)
        {
            if (fpath.Contains("\\"))
            {
                fpath = fpath.Replace("\\", "/");
            }
            if (!fpath.Contains(".db"))
                finfoList.Add(new FileInfoItem(fpath, type1, level1));
            return finfoList.Count();
        }
        public int addItem(FileInfoItem fi)
        {

            finfoList.Add(new FileInfoItem(fi));
            return finfoList.Count();
        }
        public int addItemMRL(FileInfoItem fi, int idx) // use for MRL
        {
            fi.level = idx;
            fi.ndx = idx;
            finfoList.Add(new FileInfoItem(fi));

            return finfoList.Count();
        }
        public int removeItem(string fpath, string type1, int level1)
        {

            finfoList.Remove(new FileInfoItem(fpath, type1, level1));
            return finfoList.Count();
        }
        
        public int copyFileList(List<FileInfoItem> list)
        {
            finfoList.Clear();
            for (int idx = 0; idx < list.Count(); ++idx)
            {
                finfoList.Add(list[idx]);
            }
            return finfoList.Count;
        }
        
        public int removeItemAt(int index)
        {
            if (index < finfoList.Count())
                finfoList.RemoveAt(index);
            else
            {
                System.Windows.Forms.MessageBox.Show(String.Format("ERROR attempt to remove invalid index from ImageFileList {0} max {1}", index, finfoList.Count()));
            }
            return finfoList.Count();
        }
        public int getImageCount()
        {
            return finfoList.Count();
        }
        public int getImageFileListLength()
        {
            return finfoList.Count;
        }
        public void clearList()
        {
            finfoList.Clear();
            /*
            for (int idx = finfoList.Count - 1; idx >= 0; --idx)
                    removeItemAt(idx);
                    */
        }
        public int updatePath(string fpath2, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];
            if (fpath2 != null)
                fpath2 = fpath2.Replace("\\", "/");
            /* rewritten by gpt
            if (fpath2.Contains("\\"))
            {
                fpath2 = fpath2.Replace("\\", "/");
            }
            */

            finfo.fpath = fpath2;
            setIndexed(finfo, idx);
            return idx;
        }
        public int updateRating(char rated, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];
            
            finfo.rating = rated;
            setIndexed(finfo, idx);
            return idx;
        }
        public int updateRating(string rated, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];

            finfo.rating = rated[0];
            setIndexed(finfo, idx);
            return idx;
        }
        public int markRating(string rated, int idx)
        {
            if (idx >= finfoList.Count)
            {
                return 0;
            }
            FileInfoItem finfo = this.finfoList[idx];

            finfo.rating = rated[0];
            setIndexed(finfo, idx);
            return idx;
        }
        public bool markInvalid(bool bflag, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];
            finfo.bInvalid = bflag;
            setIndexed(finfo, idx);
            return bflag;
        }
        public bool markInvalidAndDeletion(bool bflag, int idx)
        {
            FileInfoItem finfo = this.finfoList[idx];
            finfo.bInvalid = bflag;
            finfo.bDelete = bflag;
            if (bflag)
                finfo.rating = '@';
            else
                finfo.rating = 'g';

            setIndexed(finfo, idx);
            return bflag;
        }
        public int updateDelete(bool bdelete, int idx) // delete? index 
        {
            FileInfoItem finfo2 = getIndexed(idx);
            finfo2.setDelete(true);

            //setIndexed(finfo2, idx);
            
         
        //  finfo2.Delete = true;
            
            return idx;
        }
        public string getFullPath(int idx)
        {
            return finfoList.ElementAt(idx).ToString();
        }
        public FileInfoItem getIndexed(int idx)
        {
            if (idx >= finfoList.Count)
            {
                none.fpath = null;
                return none;
            } 
            return finfoList.ElementAt(idx);
        }

        public int setIndexed(FileInfoItem item, int idx)
        {
           // FileInfoItem row = getIndexed(idx);

          //  row.setFileInfoItem(item, row);

            if (idx >= finfoList.Count)
            {
                finfoList.Add(item); // xxxxxxxxx
            }
            finfoList[idx] = item;

            return idx;
        }

        public void setIndexed(string fname2, string fullpath, string dir, string ext2, string type2, string level2s, string len2s, string datetime2, string ts2, int idx)
        {
             FileInfoItem row = getIndexed(idx);
             //numVal = Convert.ToInt32(input);

             int level2 = Convert.ToInt32(level2s);
             long len2 = Convert.ToInt32(len2s);
             DateTime dt = Convert.ToDateTime(datetime2);

             row.setFileInfoItem(fname2, fullpath, dir, ext2, type2, level2, len2, dt, ts2);

            
        }
        /// <summary>
        /// Fast bulk-add of TraverserBG5 results.
        /// Minimizes allocations and completely bypasses addItem().
        /// </summary>
        /// <summary>
        /// High-speed bulk add of FileResult items into finfoList.
        /// If includeFileMetadata = true, also reads FileInfo.Length and LastWriteTime.
        /// </summary>
        public void AddResultsFast(
    List<TraverserBG5.FileResult> results,
    bool includeFileMetadata)
        {
            if (results == null || results.Count == 0)
                return;

            finfoList.Capacity = finfoList.Count + results.Count;

            foreach (var r in results)
            {
                var fi = includeFileMetadata ? new FileInfo(r.Path) : null;

                var item = new FileInfoItem
                {
                    fpath = r.Path,
                    keyPath = PathHelpers.ToNormalizedKey(r.Path),
                    dpath = Path.GetDirectoryName(r.Path) ?? string.Empty,
                    fname = Path.GetFileName(r.Path),
                    ext = Path.GetExtension(r.Path).TrimStart('.', ' '),
                    type = r.Kind,
                    level = r.Level,
                };

                if (includeFileMetadata && fi != null && fi.Exists)
                {
                    item.len = fi.Length;
                    item.stimestamp = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                }

                finfoList.Add(item);
            }
        }
        public void AddResultsFast1(List<TraverserBG1.FileResult> results, bool includeFileMetadata = false)
        {
            if (results == null || results.Count == 0)
                return;

            // Pre-grow the underlying list to avoid repeated reallocations
            if (finfoList.Capacity < finfoList.Count + results.Count)
            {
                finfoList.Capacity = finfoList.Count + results.Count;
            }

            foreach (var r in results)
            {
                var item = new FileInfoItem
                {
                    // Basic info derivable from path
                    fpath = r.Path,
                    dpath = Path.GetDirectoryName(r.Path),
                    fname = Path.GetFileName(r.Path),
                    ext = Path.GetExtension(r.Path),
                    type = r.Kind,   // "f" or "d"
                    level = r.Level,

                    // Reasonable defaults for the rest.
                    // You can tweak these if your logic depends on them.
                    rating = ' ',
                    bDelete = false,
                    bInvalid = false,
                    width = 0,
                    height = 0,
                    source = string.Empty,
                    playTime = 0,
                    minutes = 0,
                    seconds = 0,
                    ndx = 0,
                    comment = string.Empty,
                    stimestamp = string.Empty
                };

                if (includeFileMetadata)
                {
                    try
                    {
                        var fi = new FileInfo(r.Path);
                        item.len = fi.Length;

                        var ts = fi.LastWriteTime;
                        item.stimestamp = ts.ToString("yyyy-MM-dd HH:mm:ss");
                        // if you want, you can also store ts in a public property
                    }
                    catch
                    {
                        // swallow I/O errors here – keep the scan robust
                    }
                }

                finfoList.Add(item);
            }
        }

        //


    }
}