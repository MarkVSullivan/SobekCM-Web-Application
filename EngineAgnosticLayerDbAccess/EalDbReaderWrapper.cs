#region Using directives

using System;
using System.Collections.Generic;
using System.Data.Common;

#endregion

namespace EngineAgnosticLayerDbAccess
{
    /// <summary> Wrapper for a database data reader and the database connection, to simplify the process
    /// of closing the reader and connection simultaneously </summary>
    public class EalDbReaderWrapper
    {
        private readonly DbConnection connection;

        /// <summary> Open database data reader </summary>
        public readonly DbDataReader Reader;

        private List<Tuple<EalDbParameter, DbParameter>> parameterCopy;

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
            // Close the reader
            Reader.Close();

            // Were there parameter values to copy over?
            if (parameterCopy != null)
            {
                foreach (Tuple<EalDbParameter, DbParameter> paramPair in parameterCopy)
                {
                    paramPair.Item1.Value = paramPair.Item2.Value;
                }
            }


            // Close the connection
            connection.Close();
        }

        /// <summary> Store the relationship between the EAL ( Engine Agnostic Level ) parameter and the database parameter. </summary>
        /// <param name="EalParam"></param>
        /// <param name="DbParam"></param>
        /// <remarks> This is only used for parameters that include an output direction, so the returned parameter value
        /// can be copied bac from the database parameter to the EAL parameter. </remarks>
        public void Add_Parameter_Copy_Pair(EalDbParameter EalParam, DbParameter DbParam)
        {
            if (parameterCopy == null)
                parameterCopy = new List<Tuple<EalDbParameter, DbParameter>>();

            parameterCopy.Add(new Tuple<EalDbParameter, DbParameter>(EalParam, DbParam));
        }
    }
}
