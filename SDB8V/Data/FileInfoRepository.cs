using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace SymbolDB
{
    public sealed class FileInfoRepository
    {
        public IEnumerable<FileInfoItem> GetAll()
        {
            const string sql = @"
                SELECT
                    keyPath, fname, ext, len, stimestamp, rating, comment,
                    bDelete, bInvalid, width, height, source, playTime,
                    minutes, seconds, dpath, ndx, type, level, fpath
                FROM FileInfoItem
                ORDER BY fname COLLATE NOCASE;";

            using var reader = SqliteDb.Query(sql);
            while (reader.Read())
            {
                yield return Map(reader);
            }
        }

        public FileInfoItem? GetByFpath(string fpath)
        {
            const string sql = @"
                SELECT
                    keyPath, fname, ext, len, stimestamp, rating, comment,
                    bDelete, bInvalid, width, height, source, playTime,
                    minutes, seconds, dpath, ndx, type, level, fpath
                FROM FileInfoItem
                WHERE fpath = $fpath
                LIMIT 1;";

            using var reader = SqliteDb.Query(sql, ("$fpath", fpath));
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public FileInfoItem? GetByKeyPath(string fullPath)
        {
            var key = PathHelpers.ToNormalizedKey(fullPath);  // case-preserving

            const string sql = @"
        SELECT
            keyPath, fname, ext, len, stimestamp, rating, comment,
            bDelete, bInvalid, width, height, source, playTime,
            minutes, seconds, dpath, ndx, type, level, fpath
        FROM FileInfoItem
        WHERE keyPath = $n
        LIMIT 1;";

            using var reader = SqliteDb.Query(sql, ("$n", key));
            if (!reader.Read()) return null;
            return Map(reader);
        }

        // Transaction-aware upsert (so we can reuse one connection)
        public void Upsert(FileInfoItem fitem, SqliteConnection conn, SqliteTransaction? tx = null)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;

            var keyPath = !string.IsNullOrWhiteSpace(fitem.keyPath)
                ? fitem.keyPath
                : PathHelpers.ToNormalizedKey(fitem.fpath ?? string.Empty);

            cmd.CommandText = @"
        INSERT INTO FileInfoItem (
            keyPath,
            fname,
            ext,
            len,
            stimestamp,
            rating,
            comment,
            bDelete,
            bInvalid,
            width,
            height,
            source,
            playTime,
            minutes,
            seconds,
            dpath,
            ndx,
            type,
            level,
            fpath
        )
        VALUES (
            $keyPath,
            $fname,
            $ext,
            $len,
            $stimestamp,
            $rating,
            $comment,
            $bDelete,
            $bInvalid,
            $width,
            $height,
            $source,
            $playTime,
            $minutes,
            $seconds,
            $dpath,
            $ndx,
            $type,
            $level,
            $fpath
        )
        ON CONFLICT(keyPath) DO UPDATE SET
            fname      = excluded.fname,
            ext        = excluded.ext,
            len        = excluded.len,
            stimestamp = excluded.stimestamp,
            rating     = excluded.rating,
            comment    = excluded.comment,
            bDelete    = excluded.bDelete,
            bInvalid   = excluded.bInvalid,
            width      = excluded.width,
            height     = excluded.height,
            source     = excluded.source,
            playTime   = excluded.playTime,
            minutes    = excluded.minutes,
            seconds    = excluded.seconds,
            dpath      = excluded.dpath,
            ndx        = excluded.ndx,
            type       = excluded.type,
            level      = excluded.level,
            fpath      = excluded.fpath;";

            cmd.Parameters.AddWithValue("$keyPath", keyPath);
            cmd.Parameters.AddWithValue("$fname", fitem.fname ?? "");
            cmd.Parameters.AddWithValue("$ext", fitem.ext ?? "");
            cmd.Parameters.AddWithValue("$len", fitem.len);
            cmd.Parameters.AddWithValue("$stimestamp", fitem.stimestamp ?? "");
            cmd.Parameters.AddWithValue("$rating", fitem.rating.ToString());
            cmd.Parameters.AddWithValue("$comment", fitem.comment ?? "");
            cmd.Parameters.AddWithValue("$bDelete", fitem.bDelete ? 1 : 0);
            cmd.Parameters.AddWithValue("$bInvalid", fitem.bInvalid ? 1 : 0);
            cmd.Parameters.AddWithValue("$width", fitem.width);
            cmd.Parameters.AddWithValue("$height", fitem.height);
            cmd.Parameters.AddWithValue("$source", fitem.source ?? "");
            cmd.Parameters.AddWithValue("$playTime", fitem.playTime);
            cmd.Parameters.AddWithValue("$minutes", fitem.minutes);
            cmd.Parameters.AddWithValue("$seconds", fitem.seconds);
            cmd.Parameters.AddWithValue("$dpath", fitem.dpath ?? "");
            cmd.Parameters.AddWithValue("$ndx", fitem.ndx);
            cmd.Parameters.AddWithValue("$type", fitem.type ?? "f");
            cmd.Parameters.AddWithValue("$level", fitem.level);
            cmd.Parameters.AddWithValue("$fpath", fitem.fpath ?? ""); 

            cmd.ExecuteNonQuery();
        }



        private static FileInfoItem Map(System.Data.IDataRecord r)
        {
            return new FileInfoItem
            {
                keyPath = r.IsDBNull(0) ? "" : r.GetString(0),
                fname = r.IsDBNull(1) ? "" : r.GetString(1),
                ext = r.IsDBNull(2) ? "" : r.GetString(2),
                len = r.IsDBNull(3) ? 0 : r.GetInt64(3),
                stimestamp = r.IsDBNull(4) ? "" : r.GetString(4),
                rating = r.IsDBNull(5) ? ' ' : (r.GetString(5).Length > 0 ? r.GetString(5)[0] : ' '),
                comment = r.IsDBNull(6) ? "" : r.GetString(6),
                bDelete = !r.IsDBNull(7) && r.GetInt32(7) != 0,
                bInvalid = !r.IsDBNull(8) && r.GetInt32(8) != 0,
                width = r.IsDBNull(9) ? 0 : r.GetInt32(9),
                height = r.IsDBNull(10) ? 0 : r.GetInt32(10),
                source = r.IsDBNull(11) ? "" : r.GetString(11),
                playTime = r.IsDBNull(12) ? 0 : r.GetDouble(12),
                minutes = r.IsDBNull(13) ? 0 : r.GetInt32(13),
                seconds = r.IsDBNull(14) ? 0 : r.GetInt32(14),
                dpath = r.IsDBNull(15) ? "" : r.GetString(15),
                ndx = r.IsDBNull(16) ? 0 : r.GetInt32(16),
                type = r.IsDBNull(17) ? "" : r.GetString(17),
                level = r.IsDBNull(18) ? 0 : r.GetInt32(18),
                fpath = r.IsDBNull(19) ? "" : r.GetString(19),
            };
        }

        public List<FileInfoItem> GetAllOrderedByFname()
        {
            const string sql = @"
                SELECT
                    keyPath, fname, ext, len, type, level, stimestamp, rating, comment, 
                    bDelete, bInvalid, width, height, source, playTime,
                    minutes, seconds, dpath, ndx, fpath
                FROM FileInfoItem
                ORDER BY fname COLLATE NOCASE;";

            var list = new List<FileInfoItem>();

            using var reader = SqliteDb.Query(sql);
            while (reader.Read())
            {
                var item = new FileInfoItem
                {
                    fpath = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    keyPath = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    fname = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    ext = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    len = reader.IsDBNull(4) ? 0 : reader.GetInt64(4),
                    type = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    level = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                    stimestamp = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    rating = reader.IsDBNull(8) ? ' ' : reader.GetString(8)[0],
                    bDelete = !reader.IsDBNull(9) && reader.GetInt32(9) != 0,
                    bInvalid = !reader.IsDBNull(10) && reader.GetInt32(10) != 0,
                    width = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                    height = reader.IsDBNull(12) ? 0 : reader.GetInt32(12),
                    source = reader.IsDBNull(13) ? "" : reader.GetString(13),
                    playTime = reader.IsDBNull(14) ? 0d : reader.GetDouble(14),
                    minutes = reader.IsDBNull(15) ? 0 : reader.GetInt32(15),
                    seconds = reader.IsDBNull(16) ? 0 : reader.GetInt32(16),
                    dpath = reader.IsDBNull(17) ? "" : reader.GetString(17),
                    ndx = reader.IsDBNull(18) ? 0 : reader.GetInt32(18),
                    comment = reader.IsDBNull(19) ? "" : reader.GetString(19),
                };

                list.Add(item);
            }

            return list;
        }
        public void DeleteByFpath(string fpath, SqliteConnection conn, SqliteTransaction? tx = null)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "DELETE FROM FileInfoItem WHERE fpath = $fpath;";
            cmd.Parameters.AddWithValue("$fpath", fpath);
            cmd.ExecuteNonQuery();
        }
    }
}
