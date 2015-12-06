using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Tools.IpRangeUtilities;

namespace SobekCM.Core.Configuration.Engine
{
    /// <summary> Individual mapping from a HTML verb to a component/method </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("EngineVerbMapping")]
    public class Engine_VerbMapping
    {
        private IpRangeSetV4 rangeTester;

        /// <summary> Protocol which this endpoint utilizes ( JSON or Protocol Buffer ) </summary>
        [DataMember(Name = "protocol", EmitDefaultValue = false)]
        [XmlAttribute("protocol")]
        [ProtoMember(1)]
        public Microservice_Endpoint_Protocol_Enum Protocol { get; set; }

        /// <summary> Request type expected for this endpoint ( either a GET or a POST ) </summary>
        [DataMember(Name = "requestType", EmitDefaultValue = false)]
        [XmlAttribute("requestType")]
        [ProtoMember(2)]
        public Microservice_Endpoint_RequestType_Enum RequestType { get; set; }

        /// <summary> Method within the class specified by the component that should be called to fulfil the request </summary>
        [DataMember(Name = "method", EmitDefaultValue = false)]
        [XmlAttribute("method")]
        [ProtoMember(3)]
        public string Method { get; set; }

        /// <summary> Flag indicates if this endpoint is enabled or disabled </summary>
        [DataMember(Name = "enabled", EmitDefaultValue = false)]
        [XmlAttribute("enabled")]
        [ProtoMember(4)]
        public bool Enabled { get; set; }

        /// <summary> Component defines the class which is used to fulfil the request </summary>
        [DataMember(Name = "component", EmitDefaultValue = false)]
        [XmlElement("component")]
        [ProtoMember(5)]
        public Engine_Component Component { get; set; }

        /// <summary> Key for the component from the XML file </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public string ComponentId { get; set; }

        /// <summary> If this endpoint is restricted to some IP ranges, this is the list of restriction ranges that
        /// can access this endpoint </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public List<Engine_RestrictionRange> RestrictionRanges { get; set; }

        /// <summary> Restriction IDs that point to any sets of IP addresses to restrict access
        ///  to this endpoint </summary>
        [DataMember(Name = "ipRestrictionRangeId", EmitDefaultValue = false)]
        [XmlAttribute("ipRestrictionRangeId")]
        [ProtoMember(3)]
        public string RestrictionRangeSetId { get; set; }

        /// <summary> Constructor for a new instance of the Engine_VerbMapping class </summary>
        /// <remarks> Empty constructor for serialization </remarks>
        public Engine_VerbMapping()
        {
            // Empty constructor for serialization
        }

        /// <summary> Constructor for a new instance of the Engine_VerbMapping class </summary>
        /// <param name="Method"> Method within the class specified by the component that should be called to fulfil the request </param>
        /// <param name="Enabled"> Flag indicates if this endpoint is enabled or disabled </param>
        /// <param name="Protocol"> Protocol which this endpoint utilizes ( JSON or Protocol Buffer ) </param>
        /// <param name="RequestType"> Request type expected for this endpoint ( either a GET or a POST ) </param>
        public Engine_VerbMapping(string Method, bool Enabled, Microservice_Endpoint_Protocol_Enum Protocol, Microservice_Endpoint_RequestType_Enum RequestType)
        {
            this.Method = Method;
            this.Enabled = Enabled;
            this.Protocol = Protocol;
            this.RequestType = RequestType;
        }

        /// <summary> Constructor for a new instance of the Engine_VerbMapping class </summary>
        /// <param name="Method"> Method within the class specified by the component that should be called to fulfil the request </param>
        /// <param name="Enabled"> Flag indicates if this endpoint is enabled or disabled </param>
        /// <param name="Protocol"> Protocol which this endpoint utilizes ( JSON or Protocol Buffer ) </param>
        /// <param name="RequestType"> Request type expected for this endpoint ( either a GET or a POST ) </param>
        public Engine_VerbMapping(string Method, bool Enabled, Microservice_Endpoint_Protocol_Enum Protocol, Microservice_Endpoint_RequestType_Enum RequestType, string ComponentId, string RestrictionRangeId )
        {
            this.Method = Method;
            this.Enabled = Enabled;
            this.Protocol = Protocol;
            this.RequestType = RequestType;
            this.ComponentId = ComponentId;
            this.RestrictionRangeSetId = RestrictionRangeId;

        }

        /// <summary> Check to see if this endpoint can be invoked from this IP address </summary>
        /// <returns> TRUE if permitted, otherwise FALSE </returns>
        public bool AccessPermitted(string IpAddress)
        {
            // If no restriction exists, return TRUE
            if ((RestrictionRanges == null) || (RestrictionRanges.Count == 0))
                return true;

            // Was the comparison set built?
            if (rangeTester == null)
            {
                rangeTester = new IpRangeSetV4();
                foreach (Engine_RestrictionRange thisRangeSet in RestrictionRanges)
                {
                    foreach (Engine_IpRange thisRange in thisRangeSet.IpRanges)
                    {
                        if (!String.IsNullOrEmpty(thisRange.EndIp))
                            rangeTester.AddIpRange(thisRange.StartIp, thisRange.EndIp);
                        else
                            rangeTester.AddIpRange(thisRange.StartIp);
                    }
                }
            }

            // You can always acess from the same machine (first may only be relevant in Visual Studio while debugging)
            if ((IpAddress == "::1") || (IpAddress == "127.0.0.1"))
                return true;

            // Now, test the IP against the tester
            return rangeTester.Contains(new ComparableIpAddress(IpAddress));
        }

        /// <summary> Adds a restriction range to this verb mapping </summary>
        /// <param name="RestrictionRange"> Range to add </param>
        public void Add_RestrictionRange(Engine_RestrictionRange RestrictionRange)
        {
            if (RestrictionRanges == null)
                RestrictionRanges = new List<Engine_RestrictionRange>();

            RestrictionRanges.Add(RestrictionRange);
        }
    }
}
