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

        private Dictionary<string, CitationElement> elementsDictionary;

        /// <summary> ID that uniquely defines this field set </summary>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public string ID { get; set; }

        /// <summary> Default heading for this field set </summary>
        [DataMember(Name = "heading", EmitDefaultValue = false)]
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

        /// <summary> Clear the elements in this field set </summary>
        public void Clear_Elements()
        {
            Elements.Clear();
        }

        /// <summary> Removes a single citation element from this field set, if it exists </summary>
        /// <param name="MetadataTerm"> Unique identifier for the citation element to remove </param>
        public void Remove_Element(string MetadataTerm)
        {
            // Ensure the dictionary is built (i.e., not null)
            if (elementsDictionary == null) elementsDictionary = new Dictionary<string, CitationElement>( StringComparer.OrdinalIgnoreCase );

            // Check that the count in the dictionary seems right
            if (elementsDictionary.Count != Elements.Count)
            {
                foreach (CitationElement thisElement in Elements)
                    elementsDictionary[thisElement.MetadataTerm] = thisElement;
            }

            // If this doesn't exist, do nothin
            if (!elementsDictionary.ContainsKey(MetadataTerm))
                return;

            // Find the match from the dictionary first
            CitationElement match = elementsDictionary[MetadataTerm];
            Elements.Remove(match);
            elementsDictionary.Remove(MetadataTerm);
        }

        /// <summary> Adds a new citation element to the end of the current elements in this field set </summary>
        /// <param name="NewElement"> New citation element to add </param>
        /// <remarks> If an element exists with the same MetadataTerm, it is removed first. </remarks>
        public void Append_Element(CitationElement NewElement)
        {
            // Ensure the dictionary is built (i.e., not null)
            if (elementsDictionary == null) elementsDictionary = new Dictionary<string, CitationElement>(StringComparer.OrdinalIgnoreCase);

            // Check that the count in the dictionary seems right
            if (elementsDictionary.Count != Elements.Count)
            {
                foreach (CitationElement thisElement in Elements)
                    elementsDictionary[thisElement.MetadataTerm] = thisElement;
            }

            // If this element already exists, remove it
            if (elementsDictionary.ContainsKey(NewElement.MetadataTerm))
            {
                CitationElement existing = elementsDictionary[NewElement.MetadataTerm];
                Elements.Remove(existing);
            }

            // Append the new one
            Elements.Add(NewElement);
            elementsDictionary[NewElement.MetadataTerm] = NewElement;
        }

        /// <summary> Adds a new citation element after an existing elements in this field set </summary>
        /// <param name="NewElement"> New citation element to add </param>
        /// <param name="RelativeElementID"> MetadataTerm for the element after which the new citation element should be inserted </param>
        /// <remarks> If an element exists with the same MetadataTerm, it is removed first. If the ID provided
        /// to add this element after does not exist, this is just appended to the very end. </remarks>
        public void Insert_Element_After(CitationElement NewElement, string RelativeElementID )
        {
            // Ensure the dictionary is built (i.e., not null)
            if (elementsDictionary == null) elementsDictionary = new Dictionary<string, CitationElement>(StringComparer.OrdinalIgnoreCase);

            // Check that the count in the dictionary seems right
            if (elementsDictionary.Count != Elements.Count)
            {
                foreach (CitationElement thisElement in Elements)
                    elementsDictionary[thisElement.MetadataTerm] = thisElement;
            }

            // Does the relative element id exist?
            if (!elementsDictionary.ContainsKey(RelativeElementID))
            {
                // Relative doesn't exist.. just append
                Append_Element(NewElement);
            }

            // Find the index of the relative element
            int relativeIndex = Elements.IndexOf(elementsDictionary[RelativeElementID]);
            if ((relativeIndex < 0) || (relativeIndex + 1 == Elements.Count))
                Append_Element(NewElement);
            else
            {
                // If this element already exists, remove it
                if (elementsDictionary.ContainsKey(NewElement.MetadataTerm))
                {
                    CitationElement existing = elementsDictionary[NewElement.MetadataTerm];
                    Elements.Remove(existing);
                }

                // Insert at the right spot
                Elements.Insert(relativeIndex + 1, NewElement);
                elementsDictionary[NewElement.MetadataTerm] = NewElement;
            }
        }

        /// <summary> Adds a new citation element before an existing elements in this field set </summary>
        /// <param name="NewElement"> New citation element to add </param>
        /// <param name="RelativeElementID"> MetadataTerm for the element before which the new citation element should be inserted </param>
        /// <remarks> If an element exists with the same MetadataTerm, it is removed first. If the ID provided
        /// to add this element after does not exist, this is just appended to the very end. </remarks>
        public void Insert_Element_Before(CitationElement NewElement, string RelativeElementID)
        {
            // Ensure the dictionary is built (i.e., not null)
            if (elementsDictionary == null) elementsDictionary = new Dictionary<string, CitationElement>(StringComparer.OrdinalIgnoreCase);

            // Check that the count in the dictionary seems right
            if (elementsDictionary.Count != Elements.Count)
            {
                foreach (CitationElement thisElement in Elements)
                    elementsDictionary[thisElement.MetadataTerm] = thisElement;
            }

            // Does the relative element id exist?
            if (!elementsDictionary.ContainsKey(RelativeElementID))
            {
                // Relative doesn't exist.. just append
                Append_Element(NewElement);
            }

            // Find the index of the relative element
            int relativeIndex = Elements.IndexOf(elementsDictionary[RelativeElementID]);
            if (relativeIndex < 0) 
                Append_Element(NewElement);
            else
            {
                // If this element already exists, remove it
                if (elementsDictionary.ContainsKey(NewElement.MetadataTerm))
                {
                    CitationElement existing = elementsDictionary[NewElement.MetadataTerm];
                    Elements.Remove(existing);
                }

                // Insert at the right spot
                Elements.Insert(relativeIndex, NewElement);
                elementsDictionary[NewElement.MetadataTerm] = NewElement;
            }
        }
    }
}
