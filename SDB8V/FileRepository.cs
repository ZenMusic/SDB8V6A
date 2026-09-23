using Microsoft.Data.Sqlite;
using SymbolDB.Data;
using System;
using System.Collections.Generic;

namespace SymbolDB.Data
{
    /// <summary>
    /// Thin repository around SqliteDb for the Files table.
    /// </summary>
    public sealed class FileRepository
    {
        public long Upsert(FileRecord f)
        {
            var now = DateTime.UtcNow;
            // Insert or replace by unique Path
            const string sql = @"
            INSERT INTO Files (Path, Name, SizeBytes, Hash, Tags, CreatedUtc, UpdatedUtc)
            VALUES ($path, $name, $size, $hash, $tags, $created, $updated)
            ON CONFLICT(Path) DO UPDATE SET
                Name = excluded.Name,
                SizeBytes = excluded.SizeBytes,
                Hash = excluded.Hash,
                Tags = excluded.Tags,
                UpdatedUtc = excluded.UpdatedUtc;

            SELECT Id FROM Files WHERE Path = $path LIMIT 1;";

            return SqliteDb.Scalar<long>(sql,
                ("$path", f.Path),
                ("$name", f.Name),
                ("$size", f.SizeBytes),
                ("$hash", (object?)f.Hash ?? DBNull.Value),
                ("$tags", (object?)f.Tags ?? DBNull.Value),
                ("$created", f.CreatedUtc == default ? DateTime.UtcNow.ToString("o") : f.CreatedUtc.ToString("o")),
                ("$updated", now.ToString("o"))
            );
        }

        public FileRecord? GetById(long id)
        {
            const string sql = @"SELECT Id, Path, Name, SizeBytes, Hash, Tags, CreatedUtc, UpdatedUtc
                                 FROM Files WHERE Id=$id LIMIT 1;";
            using var reader = SqliteDb.Query(sql, ("$id", id));
            if (!reader.Read()) return null;

            return Map(reader);
        }

        public FileRecord? GetByPath(string path)
        {
            const string sql = @"SELECT Id, Path, Name, SizeBytes, Hash, Tags, CreatedUtc, UpdatedUtc
                                 FROM Files WHERE Path=$path LIMIT 1;";
            using var reader = SqliteDb.Query(sql, ("$path", path));
            if (!reader.Read()) return null;

            return Map(reader);
        }

        public IEnumerable<FileRecord> ListRecent(int take = 100)
        {
            const string sql = @"SELECT Id, Path, Name, SizeBytes, Hash, Tags, CreatedUtc, UpdatedUtc
                                 FROM Files ORDER BY UpdatedUtc DESC LIMIT $take;";
            using var reader = SqliteDb.Query(sql, ("$take", take));
            while (reader.Read())
                yield return Map(reader);
        }

        public int DeleteById(long id)
        {
            const string sql = @"DELETE FROM Files WHERE Id=$id;";
            return SqliteDb.Execute(sql, ("$id", id));
        }

        private static FileRecord Map(System.Data.IDataRecord r)
        {
            return new FileRecord
            {
                Id = r.GetInt64(0),
                Path = r.GetString(1),
                Name = r.GetString(2),
                SizeBytes = r.GetInt64(3),
                Hash = r.IsDBNull(4) ? null : r.GetString(4),
                Tags = r.IsDBNull(5) ? null : r.GetString(5),
                CreatedUtc = DateTime.Parse(r.GetString(6), null, System.Globalization.DateTimeStyles.RoundtripKind),
                UpdatedUtc = DateTime.Parse(r.GetString(7), null, System.Globalization.DateTimeStyles.RoundtripKind),
            };
        }
    }
}
