namespace SymbolDB
{
    /// <summary>
    /// Immutable database startup and health information for presentation by any UI.
    /// </summary>
    public sealed record DatabaseStatus(
        string DatabasePath,
        bool IsConnected,
        int CurrentSchemaVersion,
        int TargetSchemaVersion,
        string Message)
    {
        public string SchemaSummary =>
            $"Schema version {CurrentSchemaVersion}; target {TargetSchemaVersion}";
    }
}
