using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace SymbolDB
{
    /// <summary>
    /// Small, self-contained SQLite bootstrap + helpers.
    /// Creates DB, applies migrations, exposes connection factory + helpers.       //  C:\Users\<yourname>\AppData\Local\SymbolDB\SymbolDB.db

    /// </summary>
    public static class SqliteDb
    {
        // Change as you like (e.g., "ZenMusic\\SDB7")
        private const string AppFolderName = "SDB8";
        private const string DbFileName = "SymbolDB3.db";
        private static string? _dbPath;

        public static string DbPath
        {
            get
            {
                if (_dbPath == null)
                {
                    var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var dir = Path.Combine(root, AppFolderName);
                    Directory.CreateDirectory(dir);
                    _dbPath = Path.Combine(dir, DbFileName);
                }
                return _dbPath!;
            }
        }

        public static string ConnectionString => $"Data Source={DbPath};";

        /// <summary>
        /// Must be called once at startup.
        /// </summary>
        public static void Initialize()
        {
            var isNew = !File.Exists(DbPath);

            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();

            // Pragmas: WAL improves concurrency; foreign_keys keeps referential integrity on.
            cmd.CommandText = @"
                PRAGMA journal_mode=WAL;
                PRAGMA foreign_keys=ON;
                ";
            cmd.ExecuteNonQuery();

            // Minimal meta table for migrations
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS _Meta (
                    Key   TEXT PRIMARY KEY,
                    Value TEXT NOT NULL
                );";
            cmd.ExecuteNonQuery();

            // Apply migrations in order
            var current = GetSchemaVersion(conn);
            var target = Migrations.Count; // number of migration steps below

            // TEMP: show what's going to run
            System.Diagnostics.Debug.WriteLine($"[DB] SchemaVersion current={current}, target={target}");
           // MessageBox.Show($"SchemaVersion current={current}, target={target}", "DB Migrations");

            
            for (int v = current; v < target; v++)
            {
                Migrations[v](conn);
                SetSchemaVersion(conn, v + 1);
            }

            if (isNew)
            {
                // Optional: seed data
                // Insert whatever you want here on first run.
            }
        }
        public static string NormalizePathKey(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "";
            path = path.Replace('/', '\\').Trim();

            // UNC? keep as-is
            if (path.StartsWith(@"\\"))
                return path;

            if (path.Length >= 3 && path[1] == ':' && (path[2] == '\\' || path[2] == '/'))
                path = path.Substring(3);
            else if (path.Length >= 2 && path[1] == ':')
                path = path.Substring(2);

            return path.TrimStart('\\').Replace('\\', '/');  // db-normalized to "/"
        }

        /// <summary>
        /// Opens a new connection (already opened).
        /// </summary>
        public static SqliteConnection OpenConnection()
        {
            var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Execute a non-query with parameters.
        /// </summary>
        public static int Execute(string sql, params (string name, object? value)[] args)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, args);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute a scalar query with parameters.
        /// </summary>
        public static T? Scalar<T>(string sql, params (string name, object? value)[] args)
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, args);
            var val = cmd.ExecuteScalar();
            if (val == null || val is DBNull) return default;
            return (T)Convert.ChangeType(val, typeof(T))!;
        }

        /// <summary>
        /// Query helper returning IDataReader (caller must dispose).
        /// </summary>
        public static IDataReader Query(string sql, params (string name, object? value)[] args)
        {
            var conn = OpenConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            AddParams(cmd, args);
            // CommandBehavior.CloseConnection ensures the connection is closed when reader is disposed
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// VACUUM + ANALYZE to compact and refresh statistics (optional maintenance).
        /// </summary>
        public static void Compact()
        {
            using var conn = OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "VACUUM; ANALYZE;";
            cmd.ExecuteNonQuery();
        }

        private static void AddParams(SqliteCommand cmd, (string name, object? value)[] args)
        {
            foreach (var (name, value) in args)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = name;
                p.Value = value ?? DBNull.Value;
                cmd.Parameters.Add(p);
            }
        }

        // ------------------------
        // Basic migration framework
        // ------------------------
        private static readonly List<Action<SqliteConnection>> Migrations = new()
        {
            // v1: initial schema
            conn => { },
            conn =>
            {
                using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS FileInfoItem (
            fpath        TEXT PRIMARY KEY,   -- unique file path
            fname        TEXT,
            ext          TEXT,
            len          INTEGER,
            type         TEXT,
            level        INTEGER,
            stimestamp   TEXT,
            rating       TEXT,
            bDelete      INTEGER,
            bInvalid     INTEGER,
            width        INTEGER,
            height       INTEGER,
            source       TEXT,
            playTime     REAL,
            minutes      INTEGER,
            seconds      INTEGER,
            dpath        TEXT,
            [index]      INTEGER,
            [desc]       TEXT
        );

        CREATE INDEX IF NOT EXISTS IX_FileInfoItem_fname ON FileInfoItem(fname);
        CREATE INDEX IF NOT EXISTS IX_FileInfoItem_desc  ON FileInfoItem([desc]);
        CREATE INDEX IF NOT EXISTS IX_FileInfoItem_dpath ON FileInfoItem(dpath);
    ";
    cmd.ExecuteNonQuery();
            },

            // v2: example future migration (uncomment and edit when needed)
            // conn => {
            //     using var cmd = conn.CreateCommand();
            //     cmd.CommandText = "ALTER TABLE Files ADD COLUMN Rating INTEGER;";
            //     cmd.ExecuteNonQuery();
            // },
        };


        private static int GetSchemaVersion(SqliteConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Value FROM _Meta WHERE Key='SchemaVersion' LIMIT 1;";
            var v = cmd.ExecuteScalar();
            if (v == null || v is DBNull) return 0;
            if (int.TryParse(v.ToString(), out var n)) return n;
            return 0;
        }

        private static void SetSchemaVersion(SqliteConnection conn, int version)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO _Meta(Key, Value) VALUES('SchemaVersion', $v)
                ON CONFLICT(Key) DO UPDATE SET Value=$v;";
            cmd.Parameters.AddWithValue("$v", version.ToString());
            cmd.ExecuteNonQuery();
        }

        public static (int current, int target) GetSchemaVersionInfo()
        {
            using var conn = OpenConnection();
            var current = GetSchemaVersion(conn);
            var target = Migrations.Count;
            return (current, target);
        }
    }
}
