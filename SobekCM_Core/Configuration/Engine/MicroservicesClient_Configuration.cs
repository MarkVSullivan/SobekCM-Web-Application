#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.MicroservicesClient
{
    /// <summary> Configuration of all the endpointsDictionary for the microservices client </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("MicroservicesConfig")]
    public class MicroservicesClient_Configuration
    {
        private Dictionary<string, MicroservicesClient_Endpoint> endpointsDictionary;

        /// <summary> Constructor for a new instance of the MicroservicesClient_Configuration class </summary>
        public MicroservicesClient_Configuration()
        {
            endpointsDictionary = new Dictionary<string, MicroservicesClient_Endpoint>();
            Endpoints = new List<MicroservicesClient_Endpoint>();
            UseCache = true;
        }

        /// <summary> Reference a single microservices client endpoint configuration, via the lookup key </summary>
        /// <param name="Key"> Key to find the microservices client endpoint configuration </param>
        /// <returns> Specified microservices client endpoint configuration, or NULL </returns>
        public MicroservicesClient_Endpoint this[string Key]
        {
            get
            {
                // Check and build the dictionary as needed
                build_dictionary();

                // If no endpoint, return
                if (Endpoints.Count == 0)
                    return null;

                // (endpoints dictionary ensured to be built now).. return endpoint by key
                return endpointsDictionary.ContainsKey(Key) ? endpointsDictionary[Key] : null; 
            }
            set
            {
                // Check and build the dictionary as needed
                build_dictionary();

                // If a value already exists for this key, remove it from the list
                if (endpointsDictionary.ContainsKey(Key))
                {
                    MicroservicesClient_Endpoint foundEndpoint = endpointsDictionary[Key];
                    if (Endpoints.Contains(foundEndpoint))
                        Endpoints.Remove(foundEndpoint);
                }

                // Add this new value
                Endpoints.Add(value);
                endpointsDictionary[Key] = value; 
            }
        }

        /// <summary> Add a new single endpoint to this configuration </summary>
        /// <param name="Key"> Key used for subsequent lookups from this configuration </param>
        /// <param name="URL"> Complete URL for this microservices endpoint </param>
        /// <param name="Protocol"> Protocol to use when connecting to this endpoint, via the URL ( JSON or ProtoBuf ) </param>
        public void Add_Endpoint(string Key, string URL, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            // Build the endpoint object
            MicroservicesClient_Endpoint endpoint = new MicroservicesClient_Endpoint(Key, URL, Protocol);

            // Add this endpoint
            Add_Endpoint(Key, endpoint);
        }

        /// <summary> Add a new single endpoint to this configuration </summary>
        /// <param name="Key"> Key used for subsequent lookups from this configuration </param>
        /// <param name="URL"> Complete URL for this microservices endpoint </param>
        /// <param name="Protocol"> Protocol to use when connecting to this endpoint, via the URL ( JSON or ProtoBuf ) </param>
        public void Add_Endpoint(string Key, string URL, string Protocol)
        {
            // Build the endpoint object
            MicroservicesClient_Endpoint endpoint = new MicroservicesClient_Endpoint(Key, URL, Protocol);

            // Add this endpoint
            Add_Endpoint(Key, endpoint);
        }

        /// <summary> Add a new single endpoint to this configuration </summary>
        /// <param name="Key"> Key used for subsequent lookups from this configuration </param>
        /// <param name="Endpoint"> Fully pre-constructed microservice client endpoint configuration object </param>
        public void Add_Endpoint(string Key, MicroservicesClient_Endpoint Endpoint)
        {
            // Check and build the dictionary as needed
            build_dictionary();

            // If a value already exists for this key, remove it from the list
            if (endpointsDictionary.ContainsKey(Key))
            {
                MicroservicesClient_Endpoint foundEndpoint = endpointsDictionary[Key];
                if (Endpoints.Contains(foundEndpoint))
                    Endpoints.Remove(foundEndpoint);
            }

            // Add this new value
            Endpoints.Add(Endpoint);
            endpointsDictionary[Key] = Endpoint;
        }

        /// <summary> Flag indicates if the sobek engine client should natively use caching to avoid 
        /// additional round trips to the SobekCM engine endpointsDictionary </summary>
        [DataMember(Name = "useCache")]
        [XmlAttribute("useCache")]
        [ProtoMember(1)]
        public bool UseCache { get; set; }

        /// <summary> Collection of all the active micro-service endpoints </summary>
        [DataMember(Name = "endpoints")]
        [XmlArray("endpoints")]
        [XmlArrayItem("endpoint", typeof(MicroservicesClient_Endpoint))]
        [ProtoMember(2)]
        public List<MicroservicesClient_Endpoint> Endpoints { get; set; }

        /// <summary> Any error associated with reading the configuration file into this object </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public string Error { get; internal set; }


        /// <summary> Changes the base URL for all of these endpoints </summary>
        /// <param name="Original">The original.</param>
        /// <param name="New">The new.</param>
        public void Change_Base_URL(string Original, string New)
        {
            foreach (MicroservicesClient_Endpoint thisEndpoint in Endpoints)
            {
                thisEndpoint.URL = thisEndpoint.URL.Replace(Original, New);
            }
        }

        private void build_dictionary()
        {
            // If the endpoints dictionary is NULL or empty, populate it
            if ((((endpointsDictionary == null) || (endpointsDictionary.Count == 0)) && (Endpoints.Count > 0)) ||
                ((endpointsDictionary != null) && ( endpointsDictionary.Count != Endpoints.Count)))
            {
                endpointsDictionary = new Dictionary<string, MicroservicesClient_Endpoint>();
                foreach (MicroservicesClient_Endpoint endpoint in Endpoints)
                {
                    endpointsDictionary[endpoint.Key] = endpoint;
                }
            }
        }
    }
}
