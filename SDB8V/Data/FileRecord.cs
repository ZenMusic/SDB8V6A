using System;

namespace SymbolDB
{
    public sealed class FileRecord
    {
        public long Id { get; set; }
        public string Path { get; set; } = "";
        public string Name { get; set; } = "";
        public long SizeBytes { get; set; }
        public string? Hash { get; set; }
        public string? Tags { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}
