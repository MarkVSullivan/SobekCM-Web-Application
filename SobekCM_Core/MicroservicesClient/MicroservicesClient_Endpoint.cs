#region Using directives

using System;

#endregion

namespace SobekCM.Core.MicroservicesClient
{
    /// <summary> Enumeration indicates the type of protocol utilized by this endpoint</summary>
    public enum Microservice_Endpoint_Protocol_Enum : byte
    {
        /// <summary> Output of this endpoint is JSON </summary>
        JSON = 1,

        /// <summary> Output of this endpoint is Protocol Buffer octet-stream </summary>
        PROTOBUF = 2
    }

    /// <summary> Defines a single endpoint for a microservices client </summary>
    public class MicroservicesClient_Endpoint
    {
        /// <summary> Complete URL for this microservices endpoint </summary>
        public readonly string URL;

        /// <summary> Protocol to use when connecting to this endpoint, via the URL </summary>
        public readonly Microservice_Endpoint_Protocol_Enum Protocol;

        /// <summary> Constructor for a new instance of the MicroservicesClient_Endpoint class </summary>
        /// <param name="URL"> Complete URL for this microservices endpoint </param>
        /// <param name="Protocol"> Protocol to use when connecting to this endpoint, via the URL </param>
        public MicroservicesClient_Endpoint(string URL, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            this.URL = URL;
            this.Protocol = Protocol;
        }

        /// <summary> Constructor for a new instance of the MicroservicesClient_Endpoint class </summary>
        /// <param name="URL"> Complete URL for this microservices endpoint </param>
        /// <param name="Protocol"> Protocol to use when connecting to this endpoint, via the URL </param>
        public MicroservicesClient_Endpoint(string URL, string Protocol)
        {
            this.URL = URL;
            this.Protocol = String.Compare(Protocol, "protobuf", StringComparison.InvariantCultureIgnoreCase ) == 0 ? Microservice_Endpoint_Protocol_Enum.PROTOBUF : Microservice_Endpoint_Protocol_Enum.JSON;
        }
    }
}
