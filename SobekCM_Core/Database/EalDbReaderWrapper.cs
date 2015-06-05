using System.Data.Common;

namespace SobekCM.Core.Database
{
    /// <summary> Wrapper for a database data reader and the database connection, to simplify the process
    /// of closing the reader and connection simultaneously </summary>
    public class EalDbReaderWrapper
    {
        private readonly DbConnection connection;

        /// <summary> Open database data reader </summary>
        public readonly DbDataReader Reader;

        /// <summary> Constructor for an engine agnostic layer data reader </summary>
        /// <param name="Connection"> Database connection (assumed open) </param>
        /// <param name="Reader"> Database reader (assumed open) </param>
        public EalDbReaderWrapper(DbConnection Connection, DbDataReader Reader)
        {
            this.Reader = Reader;
            connection = Connection;
        }

        /// <summary> Close the data reader and the underlying database connection </summary>
        public void Close()
        {
            Reader.Close();
            connection.Close();
        }
    }
}
