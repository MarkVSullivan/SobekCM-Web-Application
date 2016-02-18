using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration.Localization;
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
        /// <remarks> This field also uniquely defines this citation element </remarks>
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
        [DataMember(Name = "options", EmitDefaultValue = false)]
        [XmlArray("options")]
        [XmlArrayItem("option", typeof(StringKeyValuePair))]
        [ProtoMember(7)]
        public List<StringKeyValuePair> Options { get; set; }

        /// <summary> Provided translations for the display term </summary>
        [DataMember(Name = "translations", EmitDefaultValue = false)]
        [XmlArray("translations")]
        [XmlArrayItem("translation", typeof(Web_Language_Translation_Value))]
        [ProtoMember(8)]
        public List<Web_Language_Translation_Value> Translations { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="CitationElement"/> class </summary>
        public CitationElement()
        {
            OverrideDisplayTerm = CitationElement_OverrideDispayTerm_Enum.NONE;
        }

        /// <summary> Constructor for a new instance of the <see cref="CitationElement"/> class </summary>
        /// <param name="MetadataTerm"> Metadata term this citation element displays (or a unique value) </param>
        /// <param name="DisplayTerm"> Default display term for this citation element </param>
        /// <param name="SearchCode"> SobekCM search code, if this element should be clickable
        /// to initiate a search within SobekCM </param>
        /// <param name="ItemProp"> Schema.org microdata tag to include for this element </param>
        public CitationElement(string MetadataTerm, string DisplayTerm, string SearchCode, string ItemProp)
        {
            this.MetadataTerm = MetadataTerm;
            this.DisplayTerm = DisplayTerm;
            this.SearchCode = SearchCode;
            this.ItemProp = ItemProp;
            OverrideDisplayTerm = CitationElement_OverrideDispayTerm_Enum.NONE;
        }

        /// <summary> Constructor for a new instance of the <see cref="CitationElement"/> class </summary>
        /// <param name="MetadataTerm"> Metadata term this citation element displays (or a unique value) </param>
        /// <param name="DisplayTerm"> Default display term for this citation element </param>
        /// <param name="SearchCode"> SobekCM search code, if this element should be clickable
        /// to initiate a search within SobekCM </param>
        /// <param name="ItemProp"> Schema.org microdata tag to include for this element </param>
        /// <param name="OverrideDisplayTerm"> Flag indicatse if the basic citation  section writer should override 
        /// the display term and use some other value from within the individual item metadata </param>
        public CitationElement(string MetadataTerm, string DisplayTerm, string SearchCode, string ItemProp, CitationElement_OverrideDispayTerm_Enum OverrideDisplayTerm )
        {
            this.MetadataTerm = MetadataTerm;
            this.DisplayTerm = DisplayTerm;
            this.SearchCode = SearchCode;
            this.ItemProp = ItemProp;
            this.OverrideDisplayTerm = OverrideDisplayTerm;
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

        /// <summary> Method suppresses XML Serialization of the Options property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeOptions()
        {
            return ((Options != null) && (Options.Count > 0));
        }

        /// <summary> Method suppresses XML Serialization of the Translations property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeTranslations()
        {
            return ((Translations != null) && (Translations.Count > 0));
        }

        #endregion

        /// <summary> Add a new option to this citation element configuration </summary>
        /// <param name="Key"> Key for this key/value pair </param>
        /// <param name="Value"> Value for this key/value pair </param>
        public void Add_Option(string Key, string Value)
        {
            if (Options == null)
                Options = new List<StringKeyValuePair>();

            Options.Add(new StringKeyValuePair(Key, Value));
        }

        /// <summary> Add a new translation for the display term </summary>
        /// <param name="Language"> Language in which this value is represented </param>
        /// <param name="Value"> Value in provided language </param>
        public void Add_Translation(Web_Language_Enum Language, string Value)
        {
            if (Translations == null)
                Translations = new List<Web_Language_Translation_Value>();

            Translations.Add(new Web_Language_Translation_Value(Language, Value));
        }
    }
}
