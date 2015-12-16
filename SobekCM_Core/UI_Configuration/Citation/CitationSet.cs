using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Citation
{
    /// <summary> Single citation set used for display purposes </summary>
    /// <remarks> Generally, within a citation configuration there is only ONE citation set.
    /// If unique citation viewers are used or other custom citations used, having 
    /// multiplc citation sets may be useful. </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("CitationSet")]
    public class CitationSet
    {
        /// <summary> Name of this citation set that uniquely identifies it </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Collection of all the field sets, which contains the actual citation 
        /// elements </summary>
        [DataMember(Name = "fieldSets", EmitDefaultValue = false)]
        [XmlArray("fieldSet")]
        [XmlArrayItem("fieldSet", typeof(CitationFieldSet))]
        [ProtoMember(2)]
        public List<CitationFieldSet> FieldSets { get; set; }

        /// <summary> Constuctor for a new instance of the <see cref="CitationSet"/> class. </summary>
        public CitationSet()
        {
            FieldSets = new List<CitationFieldSet>();
        }
    }
}
