using System;
using SobekCM.Library.Database;

namespace SobekCM.Library.Configuration
{
	/// <summary> Contains all the information to connect to a single SobekCM instance's database </summary>
	/// <remarks> This was added to allow the SobekCM builder to process pending requests for multiple instances </remarks>
	public class Database_Instance_Configuration
	{
		/// <summary> Constructor for a new instance of the Database_Instance_Configuration class </summary>
		public Database_Instance_Configuration()
		{
			Database_Type = SobekCM_Database_Type_Enum.MSSQL;
			Is_Active = true;
			Name = String.Empty;
		}

		/// <summary> Database connection string includes all the information to connect to a single instance </summary>
		public string Connection_String { get; set;  }

		/// <summary> Database type </summary>
        public SobekCM_Database_Type_Enum Database_Type { get; set;  }

		/// <summary> Flag indicates if this database instance is active for the builder </summary>
		/// <remarks> The configuration file for the builder may have multiple database settings to allow a single
		/// SobekCM builder to support multiple SobekCM instances.  </remarks>
		public bool Is_Active { get; set; }

		/// <summary> Name for this database instance </summary>
		/// <remarks> This is only used by the SobekCM builder to be able to report the instance
		/// name, in the event that the database referenced is inaccessible. </remarks>
		public string Name { get; set;  }

        /// <summary> Database type (as a string) </summary>
        public string Database_Type_String
        {
            get
            {
				switch (Database_Type)
                {
                    case SobekCM_Database_Type_Enum.MSSQL:
                        return "Microsoft SQL Server";
                       
                    case SobekCM_Database_Type_Enum.PostgreSQL:
                        return "PostgreSQL";
                }
                return "Unrecognized";
            }
        }
	}
}
