#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Core.MicroservicesClient
{
    /// <summary> Configuration of all the endpoints for the microservices client </summary>
    public class MicroservicesClient_Configuration
    {
        private readonly Dictionary<string, MicroservicesClient_Endpoint> endpoints;

        /// <summary> Constructor for a new instance of the MicroservicesClient_Configuration class </summary>
        public MicroservicesClient_Configuration()
        {
            endpoints = new Dictionary<string, MicroservicesClient_Endpoint>();
            UseCache = true;
        }

        /// <summary> Reference a single microservices client endpoint configuration, via the lookup key </summary>
        /// <param name="Key"> Key to find the microservices client endpoint configuration </param>
        /// <returns> Specified microservices client endpoint configuration, or NULL </returns>
        public MicroservicesClient_Endpoint this[string Key]
        {
            get
            {
                return endpoints.ContainsKey(Key) ? endpoints[Key] : null;
            }
            set { endpoints[Key] = value; }
        }

        /// <summary> Add a new single endpoint to this configuration </summary>
        /// <param name="Key"> Key used for subsequent lookups from this configuration </param>
        /// <param name="URL"> Complete URL for this microservices endpoint </param>
        /// <param name="Protocol"> Protocol to use when connecting to this endpoint, via the URL ( JSON or ProtoBuf ) </param>
        public void Add_Endpoint(string Key, string URL, Microservice_Endpoint_Protocol_Enum Protocol)
        {
           Add_Endpoint(Key, new MicroservicesClient_Endpoint(URL, Protocol)); 
        }

        /// <summary> Add a new single endpoint to this configuration </summary>
        /// <param name="Key"> Key used for subsequent lookups from this configuration </param>
        /// <param name="URL"> Complete URL for this microservices endpoint </param>
        /// <param name="Protocol"> Protocol to use when connecting to this endpoint, via the URL ( JSON or ProtoBuf ) </param>
        public void Add_Endpoint(string Key, string URL, string Protocol)
        {
            Add_Endpoint(Key, new MicroservicesClient_Endpoint(URL, Protocol)); 
        }

        /// <summary> Add a new single endpoint to this configuration </summary>
        /// <param name="Key"> Key used for subsequent lookups from this configuration </param>
        /// <param name="Endpoint"> Fully pre-constructed microservice client endpoint configuration object </param>
        public void Add_Endpoint(string Key, MicroservicesClient_Endpoint Endpoint )
        {
            endpoints[Key] = Endpoint;
        }

        /// <summary> Any error associated with reading the configuration file into this object </summary>
        public string Error { get; internal set; }

        /// <summary> Flag indicates if the sobek engine client should natively use caching to avoid 
        /// additional round trips to the SobekCM engine endpoints </summary>
        public bool UseCache { get; set; }
    }
}
