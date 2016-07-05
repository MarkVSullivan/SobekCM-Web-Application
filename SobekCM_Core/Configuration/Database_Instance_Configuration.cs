#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using EngineAgnosticLayerDbAccess;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Configuration
{
    /// <summary> Contains all the information to connect to a single SobekCM instance's database </summary>
    /// <remarks> This was added to allow the SobekCM builder to process pending requests for multiple instances </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("DatabaseConfig")]
    public class Database_Instance_Configuration
	{
		/// <summary> Constructor for a new instance of the Database_Instance_Configuration class </summary>
		public Database_Instance_Configuration()
		{
			Database_Type = EalDbTypeEnum.MSSQL;
		}

        /// <summary> Database connection string includes all the information to connect to a single instance </summary>
        [DataMember(Name = "connectionString")]
        [XmlAttribute("connectionString")]
        [ProtoMember(1)]
        public string Connection_String { get; set;  }

        /// <summary> Database type </summary>
        [DataMember(Name = "databaseType")]
        [XmlAttribute("databaseType")]
        [ProtoMember(2)]
        public EalDbTypeEnum Database_Type { get; set;  }

        /// <summary> Database type (as a string) </summary>
        [XmlIgnore]
        public string Database_Type_String
        {
            get
            {
				switch (Database_Type)
                {
                    case EalDbTypeEnum.MSSQL:
                        return "Microsoft SQL Server";
                       
                    case EalDbTypeEnum.PostgreSQL:
                        return "PostgreSQL";
                }
                return "Unrecognized";
            }
        }
	}
}
