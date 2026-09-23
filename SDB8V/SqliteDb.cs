using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;

namespace SymbolDB.Data
{
    /// <summary>
    /// Small, self-contained SQLite bootstrap + helpers.
    /// Creates DB, applies migrations, exposes connection factory + helpers.
    /// </summary>
    public static class SqliteDb
    {
        // Change as you like (e.g., "ZenMusic\\SDB7")
        private const string AppFolderName = "SDB7";
        private const string DbFileName = "SDB7.db";
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
            conn =>
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Files (
                    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                    Path         TEXT NOT NULL UNIQUE,
                    Name         TEXT NOT NULL,
                    SizeBytes    INTEGER NOT NULL DEFAULT 0,
                    Hash         TEXT,
                    Tags         TEXT,
                    CreatedUtc   TEXT NOT NULL,
                    UpdatedUtc   TEXT NOT NULL
                );

                CREATE INDEX IF NOT EXISTS IX_Files_Name ON Files(Name);
                CREATE INDEX IF NOT EXISTS IX_Files_UpdatedUtc ON Files(UpdatedUtc);
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
    }
}
