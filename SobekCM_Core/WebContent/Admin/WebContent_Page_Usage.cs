using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.WebContent.Single
{
    /// <summary> Usage information for a single page within a date range </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webContentUsage")]
    public class WebContent_Page_Usage
    {
        /// <summary> Title for this web content page, from the database </summary>
        /// <remarks> Generally, this is used for auto-creating sitemaps and reporting purposes </remarks>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlAttribute("title")]
        [ProtoMember(1)]
        public string Title { get; set; }

        /// <summary> Flag indicates if this web content page is currently deleted within the database </summary>
        [DataMember(EmitDefaultValue = false, Name = "deleted")]
        [XmlIgnore]
        [ProtoMember(2)]
        public bool? Deleted { get; set; }

        /// <summary> For XML serialization, flag indicates if this web content page is currently 
        /// deleted within the database </summary>
        [IgnoreDataMember]
        [XmlAttribute("deleted")]
        public bool Deleted_XML
        {
            get { return Deleted.Value; }
            set { Deleted = value; }
        }

        /// <summary> Level 1 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level1")]
        [XmlAttribute("level1")]
        [ProtoMember(3)]
        public string Level1 { get; set; }

        /// <summary> Level 2 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level2")]
        [XmlAttribute("level2")]
        [ProtoMember(4)]
        public string Level2 { get; set; }

        /// <summary> Level 3 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level3")]
        [XmlAttribute("level3")]
        [ProtoMember(5)]
        public string Level3 { get; set; }

        /// <summary> Level 4 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level4")]
        [XmlAttribute("level4")]
        [ProtoMember(6)]
        public string Level4 { get; set; }

        /// <summary> Level 5 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level5")]
        [XmlAttribute("level5")]
        [ProtoMember(7)]
        public string Level5 { get; set; }

        /// <summary> Level 6 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level6")]
        [XmlAttribute("level6")]
        [ProtoMember(8)]
        public string Level6 { get; set; }

        /// <summary> Level 7 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level7")]
        [XmlAttribute("level7")]
        [ProtoMember(9)]
        public string Level7 { get; set; }

        /// <summary> Level 8 of this recently changed static webcontent page </summary>
        [DataMember(EmitDefaultValue = false, Name = "level8")]
        [XmlAttribute("level8")]
        [ProtoMember(10)]
        public string Level8 { get; set; }

        /// <summary> Total number of (non-robotic) hits on this page </summary>
        [DataMember(Name = "hits")]
        [XmlAttribute("hits")]
        [ProtoMember(11)]
        public int Hits { get; set; }

        /// <summary> Total number of (non-robotic) hits on this page or on any child pages </summary>
        [DataMember(Name = "hitsHierarchical")]
        [XmlAttribute("hitsHierarchical")]
        [ProtoMember(12)]
        public int HitsHierarchical { get; set; }


        #region Methods to control XML serialization

        /// <summary> Method suppresses XML Serialization of the Deleted_XML property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeDeleted_XML()
        {
            return Deleted.HasValue && Deleted.Value;
        }

        #endregion

        /// <summary> Constructor for a new instance of the WebContent_Page_Usage class </summary>
        public WebContent_Page_Usage()
        {
            // Empty constructor for serialization
        }

    }
}
