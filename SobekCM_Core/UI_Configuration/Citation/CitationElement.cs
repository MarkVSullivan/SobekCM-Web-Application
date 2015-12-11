using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Tools;

namespace SobekCM.Core.UI_Configuration.Citation
{
    /// <summary> Enumeration used to indicate when the basic citation
    /// section writer should override the display term and use some other
    /// value from within the individual item metadata </summary>
    public enum CitationElement_OverrideDispayTerm_Enum : byte
    {
        /// <summary> Do not override the dispay term </summary>
        NONE = 0,

        /// <summary> Use any provided subterm as the display term, if one exists </summary>
        subterm = 1
    }

    /// <summary> Information about a single citation element to be displayed within 
    /// a citation set </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("CitationElement")]
    public class CitationElement
    {
        /// <summary> Metadata term this citation element displays (or a unique value) </summary>
        [DataMember(Name = "term")]
        [XmlAttribute("term")]
        [ProtoMember(1)]
        public string MetadataTerm { get; set; }

        /// <summary> Default display term for this citation element </summary>
        [DataMember(Name = "display", EmitDefaultValue = false)]
        [XmlAttribute("display")]
        [ProtoMember(2)]
        public string DisplayTerm { get; set; }

        /// <summary> SobekCM search code, if this element should be clickable
        /// to initiate a search within SobekCM </summary>
        [DataMember(Name = "searchCode", EmitDefaultValue = false)]
        [XmlAttribute("searchCode")]
        [ProtoMember(3)]
        public string SearchCode { get; set; }

        /// <summary> Schema.org microdata tag to include for this element </summary>
        [DataMember(Name = "itemProp", EmitDefaultValue = false)]
        [XmlAttribute("itemProp")]
        [ProtoMember(4)]
        public string ItemProp { get; set; }

        /// <summary> Flag indicatse if the basic citation  section writer should override 
        /// the display term and use some other value from within the individual item metadata </summary>
        [DataMember(Name = "overrideDisplayTerm")]
        [XmlAttribute("overrideDisplayTerm")]
        [ProtoMember(5)]
        public CitationElement_OverrideDispayTerm_Enum OverrideDisplayTerm { get; set; }

        /// <summary> Custom section writer information, if a non-standard citation section
        /// writer should be used for this element </summary>
        [DataMember(Name = "sectionWriter", EmitDefaultValue = false)]
        [XmlElement("sectionWriter")]
        [ProtoMember(6)]
        public SectionWriter SectionWriter { get; set; }

        /// <summary> Any additional options for the citation section writer </summary>
        public List<StringKeyValuePair> Options { get; set; } 

        // Translations of display term also acceptable

        /// <summary> Constructor for the <see cref="CitationElement"/> class. </summary>
        public CitationElement()
        {
            OverrideDisplayTerm = CitationElement_OverrideDispayTerm_Enum.NONE;
        }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the OverrideDisplayTerm property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeOverrideDisplayTerm()
        {
            return (OverrideDisplayTerm != CitationElement_OverrideDispayTerm_Enum.NONE);
        }

        /// <summary> Method suppresses XML Serialization of the SearchCode property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeSearchCode()
        {
            return (!String.IsNullOrEmpty(SearchCode));
        }

        /// <summary> Method suppresses XML Serialization of the ItemProp property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeItemProp()
        {
            return (!String.IsNullOrEmpty(ItemProp));
        }

        #endregion
    }
}
