#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Configuration.Engine
{
    /// <summary> Class represents a single segment of a microservice endpoint's URI, including all child segments or endpoints  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("EnginePath")]
    public class Engine_Path
    {
        /// <summary> Single portion of a URI specifying a microservice endpoint </summary>
        [DataMember(Name = "segment")]
        [XmlAttribute("segment")]
        [ProtoMember(1)]
        public string Segment { get; set; }

        /// <summary> Collection of child segments or endpoints, indexed by the next segment of the URI </summary>
        public Dictionary<string, Engine_Path> Children { get; set; }

        /// <summary> Flag indicates if this path actually defines a single endpoint </summary>
        /// <remarks> This always returns 'FALSE' in this class, although a child class may override this property </remarks>
        [XmlIgnore]
        [IgnoreDataMember]
        public virtual bool IsEndpoint { get { return false;  } }
    }
}