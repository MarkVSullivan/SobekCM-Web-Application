#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Configuration.Engine
{
    /// <summary> Enumeration indicates the type of HTTP request expected for this microservice endpoint </summary>
    public enum Microservice_Endpoint_RequestType_Enum : byte
    {
        /// <summary> Endpoint is called with a HTTP DELETE call, which should remove a resource or link </summary>
        DELETE,

        /// <summary> Endpoint is called with a standard HTTP GET, perhaps including arguments in the command line </summary>
        GET,

        /// <summary> Endpoint is called with a HTTP POST call, including some object in the body of the post request and 
        /// usually adds a new resource </summary>
        POST,

        /// <summary> Endpoint is called with a HTTP PUT call, including some object in the body of the post request and 
        /// usually updates a new resource </summary>
        PUT,

        /// <summary> Invalid HTTP verb found </summary>
        ERROR
    }

    /// <summary> Enumeration indicates the type of protocol utilized by this endpoint</summary>
    public enum Microservice_Endpoint_Protocol_Enum : byte
    {
        /// <summary> Output of this endpoint is a binary stream </summary>
        BINARY,

        /// <summary> Nothing is output ( serialized, deserialized ) but the engine places the object
        /// within the (shared) cache </summary>
        CACHE,

        /// <summary> Output of this endpoint is standard JSON </summary>
        JSON,

        /// <summary> Output of this endpoint is JSON-P </summary>
        JSON_P,

        /// <summary> Output of this endpoint is Protocol Buffer octet-stream </summary>
        PROTOBUF,

        /// <summary> Serve the object, via SOAP </summary>
        SOAP,

        /// <summary> Return the information as text-only </summary>
        TEXT,

        /// <summary> Output in XML format </summary>
        XML
    }


    /// <summary> Class represents a single segment of a microservice endpoint's URI, including all child segments or endpoints  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("EnginePath")]
    public class Engine_Path_Endpoint
    {
        private Dictionary<string, Engine_Path_Endpoint> childDictionary;


        /// <summary> Single portion of a URI specifying a microservice endpoint </summary>
        [DataMember(Name = "segment")]
        [XmlAttribute("segment")]
        [ProtoMember(1)]
        public string Segment { get; set; }

        /// <summary> Flag indicates if this path actually defines a single endpoint </summary>
        [DataMember(Name = "endpoint")]
        [XmlAttribute("endpoint")]
        [ProtoMember(2)]
        public bool IsEndpoint { get; set; }

        /// <summary> Collection of child segments or endpoints, indexed by the next segment of the URI </summary>
        [DataMember(Name = "children", EmitDefaultValue = false)]
        [XmlArray("children")]
        [XmlArrayItem("endpoint", typeof(Engine_Path_Endpoint))]
        [ProtoMember(3)]
        public List<Engine_Path_Endpoint> Children { get; set; }

        /// <summary> Mapping to an individual method and protocol for the GET HTTP verb </summary>
        [DataMember(Name = "getMapping", EmitDefaultValue = false)]
        [XmlElement("getMapping")]
        [ProtoMember(4)]
        public Engine_VerbMapping GetMapping { get; set; }

        /// <summary> Mapping to an individual method and protocol for the DELETE HTTP verb </summary>
        [DataMember(Name = "deleteMapping", EmitDefaultValue = false)]
        [XmlElement("deleteMapping")]
        [ProtoMember(5)]
        public Engine_VerbMapping DeleteMapping { get; set; }

        /// <summary> Mapping to an individual method and protocol for the POST HTTP verb </summary>
        [DataMember(Name = "postMapping", EmitDefaultValue = false)]
        [XmlElement("postMapping")]
        [ProtoMember(6)]
        public Engine_VerbMapping PostMapping { get; set; }

        /// <summary> Mapping to an individual method and protocol for the PUT HTTP verb </summary>
        [DataMember(Name = "putMapping", EmitDefaultValue = false)]
        [XmlElement("putMapping")]
        [ProtoMember(7)]
        public Engine_VerbMapping PutMapping { get; set; }

        /// <summary> Constructor for a new instance of the Engine_Path_Endpoint class </summary>
        public Engine_Path_Endpoint()
        {
            childDictionary = new Dictionary<string, Engine_Path_Endpoint>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary> Returns flag if a C# method is mapped to the provided HTTP verb/method </summary>
        /// <param name="Method"> Method, as upper-case string (i.e., 'DELETE', 'GET', 'POST', 'PUT', etc..) </param>
        /// <returns> TRUE if a mapping exists for the provided HTTP verb/method </returns>
        public bool VerbMappingExists(string Method)
        {
            switch (Method)
            {
                case "DELETE":
                case "delete":
                    return ((DeleteMapping != null) && (DeleteMapping.Enabled));

                case "GET":
                case "get":
                    return ((GetMapping != null) && (GetMapping.Enabled));

                case "POST":
                case "post":
                    return ((PostMapping != null) && (PostMapping.Enabled));

                case "PUT":
                case "put":
                    return ((PutMapping != null) && (PutMapping.Enabled));

                default:
                    return false;
            }
        }

        /// <summary> Gets a flag indicating if any verb mapping exists for this endpoint </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public bool HasVerbMapping
        {
            get { return ((GetMapping != null) || (PostMapping != null) || (PutMapping != null) || (DeleteMapping != null)); }
        }

        /// <summary> Get a single verb mapping, by HTTP verb/method  </summary>
        /// <param name="Method"> Method, as upper-case string (i.e., 'DELETE', 'GET', 'POST', 'PUT', etc..)</param>
        /// <returns> Matching verb mapping, or NULL </returns>
        [XmlIgnore]
        [IgnoreDataMember]
        public Engine_VerbMapping this[string Method]
        {
            get
            {
                switch (Method)
                {
                    case "DELETE":
                    case "delete":
                        return DeleteMapping;

                    case "GET":
                    case "get":
                        return GetMapping;

                    case "POST":
                    case "post":
                        return PostMapping;

                    case "PUT":
                    case "put":
                        return PutMapping;

                    default:
                        return null;
                }
            }
        }

        /// <summary> Get the list of all verb mappings included in this endpoint </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public List<Engine_VerbMapping> AllVerbMappings
        {
            get
            {
                // Most common case
                if ((GetMapping != null) && (PostMapping == null) && (PutMapping == null) && (DeleteMapping == null))
                    return new List<Engine_VerbMapping> { GetMapping };

                // Build the list 
                List<Engine_VerbMapping> returnValue = new List<Engine_VerbMapping>();
                if (GetMapping != null) returnValue.Add(GetMapping);
                if (PostMapping != null) returnValue.Add(PostMapping);
                if (PutMapping != null) returnValue.Add(PutMapping);
                if (DeleteMapping != null) returnValue.Add(DeleteMapping);
                return returnValue;
            }
        }

        private void ensure_dictionary_built()
        {
            
        }

        public bool ContainsChildKey(string ChildSegment)
        {
            // Ensure the dictionary is built correctly
            ensure_dictionary_built();

            // check dictionary for key
            return childDictionary.ContainsKey(ChildSegment);
        }

        public Engine_Path_Endpoint GetChild(string ChildSegment)
        {
            // Ensure the dictionary is built correctly
            ensure_dictionary_built();

            // If it exists, return it
            if (childDictionary.ContainsKey(ChildSegment))
                return childDictionary[ChildSegment];

            return null;
        }

        public void AddChild(string ChildSegment, Engine_Path_Endpoint Child)
        {
            // Ensure the collection exists
            if (Children == null)
                Children = new List<Engine_Path_Endpoint>();

            // Ensure the dictionary is built correctly
            ensure_dictionary_built();

            // Does an endpoint already exist here?
            if (childDictionary.ContainsKey(ChildSegment))
            {
                Engine_Path_Endpoint matchingEndpoint = childDictionary[ChildSegment];
                Children.Remove(matchingEndpoint);
            }

            childDictionary[ChildSegment] = Child;
            Children.Add(Child);
        }

    }
}