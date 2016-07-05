using System;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Configuration;

namespace SobekCM.Builder_Library.Settings
{
    /// <summary> Information about a single instance of SobekCM maintained
    /// by the SobekCM builder application </summary>
    public class Single_Instance_Configuration
    {
        /// <summary> Database connection information for this instance </summary>
        public Database_Instance_Configuration DatabaseConnection { get; set; }

        /// <summary> Name for this instance of SobekCM </summary>
        /// <remarks> This is only used by the SobekCM builder to be able to report the instance
        /// name, in the event that the database referenced is inaccessible. </remarks>
        public string Name { get; set; }

        /// <summary> Flag indicates if this database instance is active for the builder </summary>
        public bool Is_Active { get; set; }

        /// <summary> URL for the engine, to retrieve all the setting information </summary>
        public string Engine_URL { get; set; }

        /// <summary> Protocol to use when pulling the configuration and setting information
        /// directly from the SobekCM engine </summary>
        public Microservice_Endpoint_Protocol_Enum Engine_Protocol { get; set;  }

        /// <summary> Constructor for a new instance of the Single_Instance_Configuration class </summary>
        public Single_Instance_Configuration()
        {
            Is_Active = true;
            Name = String.Empty;
            Engine_URL = String.Empty;
            Engine_Protocol = Microservice_Endpoint_Protocol_Enum.PROTOBUF;
            DatabaseConnection = new Database_Instance_Configuration();

        }
    }
}
