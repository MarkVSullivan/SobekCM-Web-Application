using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.BriefItem;

namespace SobekCM.Core.Configuration
{
    /// <summary> Configuration information for the mapping from the full SobekCM_Item to
    /// the BriefItem which is used by the item viewers </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("BriefItemMapperConfig")]
    public class BriefItemMapping_Configuration
    {
        [XmlIgnore]
        [IgnoreDataMember]
        private Dictionary<string, BriefItemMapping_Set> mappingSetsDictionary;

        /// <summary> Name of the default set </summary>
        [DataMember(Name = "default")]
        [XmlAttribute("default")]
        [ProtoMember(1)]
        public string DefaultSetName;

        /// <summary> Collection of all the mapping sets </summary>
        [DataMember(Name = "mappingSets")]
        [XmlArray("mappingSets")]
        [XmlArrayItem("mappingSet", typeof(BriefItemMapping_Set))]
        [ProtoMember(2)]
        public List<BriefItemMapping_Set> MappingSets { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="BriefItemMapping_Configuration"/> class </summary>
        public BriefItemMapping_Configuration()
        {
            MappingSets = new List<BriefItemMapping_Set>();
            mappingSetsDictionary = new Dictionary<string, BriefItemMapping_Set>();
        }

    }

    /// <summary> Set of mappings from the full SobekCM_Item class to the BriefItem
    /// class which is used by the item viewers </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("BriefItemMapperSet")]
    public class BriefItemMapping_Set
    {
        /// <summary> Name for this set of mappings (allows multiple different mappings
        /// to be utilized) </summary>
        [DataMember(Name = "setName")]
        [XmlAttribute("setName")]
        [ProtoMember(1)]
        public string SetName { get; set; }

        /// <summary> Collection of all the mappings to be used to convert from the SobekCM_Item
        /// object to a BriefItem object used by the item viewers </summary>
        [DataMember(Name = "mappings")]
        [XmlArray("mappings")]
        [XmlArrayItem("mapping", typeof(BriefItemMapping_Mapper))]
        [ProtoMember(2)]
        public List<BriefItemMapping_Mapper> Mappings { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="BriefItemMapping_Set"/> class </summary>
        public BriefItemMapping_Set()
        {
            Mappings = new List<BriefItemMapping_Mapper>();
        }
    }

    /// <summary> Settings for the mapping from the full SobekCM_Item to the BriefItem
    /// which is used by the item viewers </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("BriefItemMapperClass")]
    public class BriefItemMapping_Mapper
    {
        /// <summary> Assembly name for the mapping object </summary>
        [DataMember(Name = "assembly", EmitDefaultValue = false)]
        [XmlAttribute("assembly")]
        [ProtoMember(1)]
        public string Assembly { get; set; }

        /// <summary> Complete qualified (with namespace) name of the class for the mapping object </summary>
        [DataMember(Name = "class")]
        [XmlAttribute("class")]
        [ProtoMember(2)]
        public string Class { get; set; }

        /// <summary> Gets or sets a value indicating whether this mapping in enabled </summary>
        [DataMember(Name = "enabled")]
        [XmlAttribute("enabled")]
        [ProtoMember(3)]
        public bool Enabled { get; set; }

        /// <summary> Constructred mapping object class implementing the IBriefItemMapper interface </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public IBriefItemMapper MappingObject { get; set; }


        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Assembly property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeAssembly()
        {
            return (!String.IsNullOrEmpty(Assembly));
        }

        #endregion
    }
}
