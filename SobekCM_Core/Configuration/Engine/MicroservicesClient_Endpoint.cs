#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.MicroservicesClient
{
    /// <summary> Enumeration indicates the type of protocol utilized by this endpoint</summary>
    public enum Microservice_Endpoint_Protocol_Enum : byte
    {
        /// <summary> Output of this endpoint is JSON </summary>
        JSON = 1,

        /// <summary> Output of this endpoint is Protocol Buffer octet-stream </summary>
        PROTOBUF = 2,

        /// <summary> Output of this endpoint is JSON-P </summary>
        JSON_P,

        /// <summary> Output of this endpoint is XML </summary>
        XML
    }

    /// <summary> Defines a single endpoint for a microservices client </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("MicroservicesEndpoint")]
    public class MicroservicesClient_Endpoint
    {
        /// <summary> Key used for subsequent lookups from this configuration </summary>
        [DataMember(Name = "key", EmitDefaultValue = false)]
        [XmlAttribute("key")]
        [ProtoMember(1)]
        public string Key { get; set; }

        /// <summary> Complete URL for this microservices endpoint </summary>
        [DataMember(Name = "url", EmitDefaultValue = false)]
        [XmlAttribute("url")]
        [ProtoMember(2)]
        public string URL { get; set; }

        /// <summary> Protocol to use when connecting to this endpoint, via the URL </summary>
        [DataMember(Name = "protocol", EmitDefaultValue = false)]
        [XmlAttribute("protocol")]
        [ProtoMember(3)]
        public Microservice_Endpoint_Protocol_Enum Protocol { get; set; }

        /// <summary> Constructor for a new instance of the MicroservicesClient_Endpoint class </summary>
        /// <remarks> Empty constructor for serialization purposes </remarks>
        public MicroservicesClient_Endpoint()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Constructor for a new instance of the MicroservicesClient_Endpoint class </summary>
        /// <param name="Key"> Key used for subsequent lookups from this configuration </param>
        /// <param name="URL"> Complete URL for this microservices endpoint </param>
        /// <param name="Protocol"> Protocol to use when connecting to this endpoint, via the URL </param>
        public MicroservicesClient_Endpoint(string Key, string URL, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            this.Key = Key;
            this.URL = URL;
            this.Protocol = Protocol;
        }

        /// <summary> Constructor for a new instance of the MicroservicesClient_Endpoint class </summary>
        /// <param name="Key"> Key used for subsequent lookups from this configuration </param>
        /// <param name="URL"> Complete URL for this microservices endpoint </param>
        /// <param name="Protocol"> Protocol to use when connecting to this endpoint, via the URL </param>
        public MicroservicesClient_Endpoint(string Key, string URL, string Protocol)
        {
            this.Key = Key;
            this.URL = URL;
            this.Protocol = String.Compare(Protocol, "protobuf", StringComparison.InvariantCultureIgnoreCase) == 0 ? Microservice_Endpoint_Protocol_Enum.PROTOBUF : Microservice_Endpoint_Protocol_Enum.JSON;
        }
    }
}
