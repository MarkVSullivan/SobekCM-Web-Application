using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Library.Database
{
    /// <summary> Enumeration tells the type of underlying database connection to create </summary>
    public enum SobekCM_Database_Type_Enum : byte
    {
        /// <summary> Microsoft SQL Server </summary>
        MSSQL = 1,

        /// <summary> PostgreSQL Server </summary>
        PostgreSQL
    }
}
