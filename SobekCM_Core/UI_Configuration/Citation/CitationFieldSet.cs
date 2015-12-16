using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration.Localization;

namespace SobekCM.Core.UI_Configuration.Citation
{
    /// <summary> Field set surrounds a number of citation element configuration objects
    /// within a single citation set in the user interface configuration </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("CitationFieldSet")]
    public class CitationFieldSet
    {
        /// <summary> ID that uniquely defines this field set </summary>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public string ID { get; set; }

        /// <summary> Default heading for this field set </summary>
        [DataMember(Name = "heading")]
        [XmlAttribute("heading")]
        [ProtoMember(2)]
        public string Heading { get; set; }

        /// <summary> Provided translations for the heading term </summary>
        [DataMember(Name = "translations", EmitDefaultValue = false)]
        [XmlArray("translations")]
        [XmlArrayItem("translation", typeof(Web_Language_Translation_Value))]
        [ProtoMember(3)]
        public List<Web_Language_Translation_Value> Translations { get; set; }

        /// <summary> List of the individual citation elements within this field set </summary>
        [DataMember(Name = "citationElements", EmitDefaultValue = false)]
        [XmlArray("citationElement")]
        [XmlArrayItem("citationElement", typeof(CitationElement))]
        [ProtoMember(4)]
        public List<CitationElement> Elements { get; set; }

        #region Methods that controls XML serialization

        /// <summary> Method suppresses XML Serialization of the Heading property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeHeading()
        {
            return (!String.IsNullOrEmpty(Heading));
        }

        /// <summary> Method suppresses XML Serialization of the Translations property if it is empty </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeTranslations()
        {
            return ((Translations != null) && (Translations.Count > 0));
        }

        #endregion

        /// <summary> Add a new translation for the heading term </summary>
        /// <param name="Language"> Language in which this value is represented </param>
        /// <param name="Value"> Value in provided language </param>
        public void Add_Translation(Web_Language_Enum Language, string Value)
        {
            if (Translations == null)
                Translations = new List<Web_Language_Translation_Value>();

            Translations.Add(new Web_Language_Translation_Value(Language, Value));
        }

        /// <summary> Constructor for a new instance of the <see cref="CitationFieldSet"/> class. </summary>
        public CitationFieldSet()
        {
            Elements = new List<CitationElement>();
        }
    }
}
