using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Citation
{
    /// <summary> Configuration for the citation within SobekCM, including the elements, group of element,
    /// order, and other details for rendering the citation within SobekCM </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("CitationConfig")]
    public class CitationConfig
    {
        /// <summary> Name of the default citation set </summary>
        [DataMember(EmitDefaultValue = false, Name = "default")]
        [XmlAttribute("default")]
        [ProtoMember(1)]
        public string DefaultCitationSet { get; set; }

        /// <summary> Collection of the citation sets for this citation configuration </summary>
        [DataMember(EmitDefaultValue = false, Name = "sets")]
        [XmlArray("sets")]
        [XmlArrayItem("set", typeof(CitationSet))]
        [ProtoMember(2)]
        public List<CitationSet> CitationSets { get; set; }

        /// <summary> Constuctor for a new instance of the <see cref="CitationConfig"/> class. </summary>
        public CitationConfig()
        {
            CitationSets = new List<CitationSet>();

            // Set defaults
            set_defaults();
        }

        /// <summary> Return the default (or first) citation set </summary>
        /// <returns> Citation set, which details how the citation should be written </returns>
        public CitationSet Get_CitationSet()
        {
            // If there are no sets, return NULL
            if ((CitationSets == null) || (CitationSets.Count == 0))
                return null;

            // Look for the default set
            if (!String.IsNullOrEmpty(DefaultCitationSet))
            {
                foreach (CitationSet set in CitationSets)
                {
                    if (String.Compare(set.Name, DefaultCitationSet, StringComparison.Ordinal) == 0)
                        return set;
                }
            }

            // Just return the first one then
            return CitationSets[0];
        }

        private void set_defaults()
        {
            // Create the new set
            CitationSet defaultSet = new CitationSet {Name = "DEFAULT"};
            DefaultCitationSet = "DEFAULT";

            // Add the purl in its own field set
            CitationFieldSet purlSet = new CitationFieldSet {ID = "PURL"};
            purlSet.Elements.Add(new CitationElement("Permanent Link", "Permanent Link", null, null ));
            defaultSet.FieldSets.Add(purlSet);

            // Add the main material information field set
            CitationFieldSet materialSet = new CitationFieldSet {ID = "MATERIAL", Heading = "Material Information"};
            materialSet.Elements.Add(new CitationElement("Title", "Title", null, "name"));
            materialSet.Elements.Add(new CitationElement("Series Title", "Series Title", "TI", null));
            materialSet.Elements.Add(new CitationElement("Uniform Title", "Uniform Title", null, null));
            materialSet.Elements.Add(new CitationElement("Alternate Title", "Alternate Title", null, null, CitationElement_OverrideDispayTerm_Enum.subterm));
            materialSet.Elements.Add(new CitationElement("Translated Title", "Translated Title", null, null));
            materialSet.Elements.Add(new CitationElement("Abbreviated Title", "Abbreviated Title", null, null));
            materialSet.Elements.Add(new CitationElement("Creator", "Creator", "AU", null));
            materialSet.Elements.Add(new CitationElement("Conference", "Conference", null, null));
            materialSet.Elements.Add(new CitationElement("Affiliation", "Affiliation", null, null));
            materialSet.Elements.Add(new CitationElement("Donor", "Donor", "DO", null));
            materialSet.Elements.Add(new CitationElement("Place of Publication", "Place of Publication", "PP", null));
            materialSet.Elements.Add(new CitationElement("Publisher", "Publisher", "PU", "publisher"));
            materialSet.Elements.Add(new CitationElement("Manufacturer", "Manufacturer", null, null));
            materialSet.Elements.Add(new CitationElement("Creation Date", "Creation Date", null, "dateCreated"));
            materialSet.Elements.Add(new CitationElement("Publication Date", "Publication Date", null, "datePublished"));
            materialSet.Elements.Add(new CitationElement("Copyright Date", "Copyright Date", null, "copyrightYear"));
            materialSet.Elements.Add(new CitationElement("Frequency", "Frequency", null, null));
            materialSet.Elements.Add(new CitationElement("Language", "Language", "LA", "inLanguage"));
            materialSet.Elements.Add(new CitationElement("Edition", "Edition", null, "edition"));
            materialSet.Elements.Add(new CitationElement("State / Edition", "State / Edition", null, "edition"));
            materialSet.Elements.Add(new CitationElement("Physical Description", "Permanent Link", null, null));
            materialSet.Elements.Add(new CitationElement("Scale", "Scale", null, null));
            materialSet.Elements.Add(new CitationElement("Materials", "Materials", "MA", null));
            materialSet.Elements.Add(new CitationElement("Measurements", "Measurements", null, null));
            materialSet.Elements.Add(new CitationElement("Cultural Context", "Cultural Context", null, null));
            materialSet.Elements.Add(new CitationElement("Style/Period", "Style/Period", null, null));
            materialSet.Elements.Add(new CitationElement("Technique", "Technique", null, null));
            materialSet.Elements.Add(new CitationElement("Physical Location", "Physical Location", null, null));
            defaultSet.FieldSets.Add(materialSet);

            // Add the thesis / dissertation field set
            CitationFieldSet thesesSet = new CitationFieldSet { ID = "THESIS", Heading = "Thesis/Dissertation Information" };
            thesesSet.Elements.Add(new CitationElement("Degree", "Degree", null, null));
            thesesSet.Elements.Add(new CitationElement("Degree Grantor", "Degree Grantor", null, null));
            thesesSet.Elements.Add(new CitationElement("Degree Divisions", "Degree Divisions", "EJ", null));
            thesesSet.Elements.Add(new CitationElement("Degree Disciplines", "Degree Disciplines", "EI", null));
            thesesSet.Elements.Add(new CitationElement("Committee Chair", "Committee Chair", "EC", null));
            thesesSet.Elements.Add(new CitationElement("Committee Co-Chair", "Committee Co-Chair", "EC", null));
            thesesSet.Elements.Add(new CitationElement("Committee Members", "Committee Members", "EC", null));
            defaultSet.FieldSets.Add(thesesSet);

            // Add the Darwin Core field set
            CitationFieldSet darwinSet = new CitationFieldSet { ID = "DARWIN", Heading = "Zoological Taxonomic Information" };
            darwinSet.Elements.Add(new CitationElement("Scientific Name", "Scientific Name", null, null));
            darwinSet.Elements.Add(new CitationElement("Kingdom", "Kingdom", null, null));
            darwinSet.Elements.Add(new CitationElement("Phylum", "Phylum", null, null));
            darwinSet.Elements.Add(new CitationElement("Class", "Class", null, null));
            darwinSet.Elements.Add(new CitationElement("Order", "Order", null, null));
            darwinSet.Elements.Add(new CitationElement("Family", "Family", null, null));
            darwinSet.Elements.Add(new CitationElement("Genus", "Genus", null, null));
            darwinSet.Elements.Add(new CitationElement("Species", "Species", null, null));
            darwinSet.Elements.Add(new CitationElement("Taxonomic Rank", "Taxonomic Rank", null, null));
            darwinSet.Elements.Add(new CitationElement("Common Name", "Common Name", null, null));
            defaultSet.FieldSets.Add(darwinSet);

            // Add the IEEE-LOM learning object field set
            CitationFieldSet lomSet = new CitationFieldSet { ID = "LOM", Heading = "Learning Resource Information" };
            lomSet.Elements.Add(new CitationElement("Aggregation Level", "Aggregation Level", null, null));
            lomSet.Elements.Add(new CitationElement("Learning Resource Type", "Learning Resource Type", null, null));
            lomSet.Elements.Add(new CitationElement("Status", "Status", null, null));
            lomSet.Elements.Add(new CitationElement("Interactivity Type", "Interactivity Type", null, null));
            lomSet.Elements.Add(new CitationElement("Interactivity Level", "Interactivity Level", null, null));
            lomSet.Elements.Add(new CitationElement("Difficulty Level", "Difficulty Level", null, null));
            lomSet.Elements.Add(new CitationElement("Intended User Roles", "Intended User Roles", null, null));
            lomSet.Elements.Add(new CitationElement("Context", "Context", null, null));
            lomSet.Elements.Add(new CitationElement("Typical Age Range", "Typical Age Range", null, "typicalAgeRange"));
            lomSet.Elements.Add(new CitationElement("Typical Learning Time", "Typical Learning Time", null, null));
            lomSet.Elements.Add(new CitationElement("System Requirements", "System Requirements", null, null));
            defaultSet.FieldSets.Add(lomSet);

            // Add the subjects field set
            CitationFieldSet subjectSet = new CitationFieldSet { ID = "SUBJECTS", Heading = "Subjects" };
            subjectSet.Elements.Add(new CitationElement("Subjects / Keywords", "Subjects / Keywords", null, null));
            subjectSet.Elements.Add(new CitationElement("Genre", "Genre", null, null));
            subjectSet.Elements.Add(new CitationElement("Temporal Coverage", "Temporal Coverage", null, null));
            subjectSet.Elements.Add(new CitationElement("Spatial Coverage", "Spatial Coverage", null, null));
            subjectSet.Elements.Add(new CitationElement("Coordinates", "Coordinates", null, null));
            subjectSet.Elements.Add(new CitationElement("Target Audience", "Target Audience", null, null));
            defaultSet.FieldSets.Add(subjectSet);

            // Add the notes field set
            CitationFieldSet notesSet = new CitationFieldSet { ID = "NOTES", Heading = "Notes" };
            notesSet.Elements.Add(new CitationElement("Abstract", "Abstract", null, "description", CitationElement_OverrideDispayTerm_Enum.subterm));
            notesSet.Elements.Add(new CitationElement("Note", "General Note", null, "notes", CitationElement_OverrideDispayTerm_Enum.subterm));
            notesSet.Elements.Add(new CitationElement("Inscription", "Inscription", null, null));
            notesSet.Elements.Add(new CitationElement("User Tags", "User Tags", null, null));
            defaultSet.FieldSets.Add(notesSet);

            // Add the record information field set
            CitationFieldSet recordSet = new CitationFieldSet { ID = "RECORD", Heading = "Record Information" };
            recordSet.Elements.Add(new CitationElement("Source Institution", "Source Institution", null, "sourceOrganization"));
            recordSet.Elements.Add(new CitationElement("Holding Location", "Holding Location", null, "contentLocation"));
            recordSet.Elements.Add(new CitationElement("Rights Management", "Rights Management", null, "rights"));
            recordSet.Elements.Add(new CitationElement("Embargo Date", "Embargo Date", null, null));
            recordSet.Elements.Add(new CitationElement("Resource Identifier", "Resource Identifier", null, "identifier"));
            recordSet.Elements.Add(new CitationElement("Classification", "Classification", null, "classification"));
            recordSet.Elements.Add(new CitationElement("System ID", "System ID", null, null));
            defaultSet.FieldSets.Add(recordSet);

            // Add the related items field set
            CitationFieldSet relatedSet = new CitationFieldSet { ID = "RELATED", Heading = "Related Items" };
            relatedSet.Elements.Add(new CitationElement("Related Item", "Related Item", null, null, CitationElement_OverrideDispayTerm_Enum.subterm));
            defaultSet.FieldSets.Add(relatedSet);

            // Add the entire citation set
            CitationSets.Add(defaultSet);

        }

    }
}
