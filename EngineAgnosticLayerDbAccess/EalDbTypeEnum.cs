namespace EngineAgnosticLayerDbAccess
{
    /// <summary> Enumeration tells the type of underlying database connection to create </summary>
    public enum EalDbTypeEnum : byte
    {
        /// <summary> Microsoft SQL Server </summary>
        MSSQL = 1,

        /// <summary> PostgreSQL Server </summary>
        PostgreSQL
    }
}
