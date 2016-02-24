using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.TemplateElements
{
    /// <summary> Configuration class handles the mapping between all of the
    /// type/subtype attributes in each individual template cofiguration file
    /// and the actual classes to render that element in the online metadata forms </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("TemplateElementsConfig")]
    public class TemplateElementsConfig
    {
        private Dictionary<string, TemplateElement> elementDictionary;

            /// <summary> Collection of all the template elements </summary>
        [DataMember(Name = "elements", EmitDefaultValue = false)]
        [XmlArray("elements")]
        [XmlArrayItem("element", typeof(TemplateElement))]
        [ProtoMember(1)]
        public List<TemplateElement> Elements { get; set; }

        /// <summary> Constructor for a new instance of the <see cref="TemplateElementsConfig"/> class </summary>
        public TemplateElementsConfig()
        {
            Elements = new List<TemplateElement>();

            set_defaults();
        }

        private void set_defaults()
        {
            Elements.Clear();

            Add_Element("abstract", "simple", "Abstract_Summary_Element");
            Add_Element("abstract", null, "Abstract_Complex_Element");
            Add_Element("acquisition", null, "Acquisition_Note_Element");
            Add_Element("additional work needed", null, "Additional_Work_Needed_Element");
            Add_Element("aggregations", null, "Aggregations_Element");
            Add_Element("attribution", null, "Attribution_Element");
            Add_Element("bibid", null, "BibID_Element");
            Add_Element("born digital", null, "Born_Digital_Element");
            Add_Element("catalog record num", null, "Catalog_Record_Number_Element");
            Add_Element("classification", null, "Classification_Element");
            Add_Element("container", null, "Container_Element");
            Add_Element("contributor", null, "Contributor_Element");
            Add_Element("coordinates", null, "Coordinates_Point_Element");
            Add_Element("creator", "complex", "Creator_Complex_Element");
            Add_Element("creator", "fixed_role", "Creator_Fixed_Role_Element");
            Add_Element("creator", "simple", "Creator_Element");
            Add_Element("creator", null, "Name_Form_Element");
            Add_Element("creator notes", null, "Creator_Notes_Element");
            Add_Element("dark flag", null, "Dark_Flag_Element");
            Add_Element("date", null, "Date_Element");
            Add_Element("date copyrighted", null, "Date_Copyrighted_Element");
            Add_Element("description standard", null, "Description_Standard_Element");
            Add_Element("disposition advice", null, "Disposition_Advice_Element");
            Add_Element("donor", null, "Donor_Element");
            Add_Element("ead", null, "EAD_Form_Element");
            Add_Element("edition", null, "Edition_Element");
            Add_Element("embedded video", null, "EmbeddedVideo_Element");
            Add_Element("encoding level", null, "Encoding_Level_Element");
            Add_Element("etd committee chair", null, "ETD_CommitteeChair_Element");
            Add_Element("etd committee cochair", null, "ETD_CommitteeCoChair_Element");
            Add_Element("etd committee member", null, "ETD_CommitteeMember_Element");
            Add_Element("etd degree", null, "ETD_Degree_Element");
            Add_Element("etd degree discipline", null, "ETD_DegreeDiscipline_Element");
            Add_Element("etd degree division", null, "ETD_DegreeDivision_Element");
            Add_Element("etd degree grantor", null, "ETD_DegreeGrantor_Element");
            Add_Element("etd degree level", null, "ETD_DegreeLevel_Element");
            Add_Element("etd graduation semester", null, "ETD_GraduationSemester_Element");
            Add_Element("fast subject", null, "FAST_Subject_Element");
            Add_Element("format", null, "Format_Element");
            Add_Element("frequency", null, "Frequency_Element");
            Add_Element("genre", null, "Genre_Element");
            Add_Element("group title", null, "Group_Title_Element");
            Add_Element("holding", null, "Holding_Element");
            Add_Element("identifier", "fixed_type", "Identifier_Fixed_Type_Element");
            Add_Element("identifier", null, "Identifier_Element");
            Add_Element("language", "select", "Language_Select_Element");
            Add_Element("language", null, "Language_Element");
            Add_Element("lccn", null, "LCCN_Element");
            Add_Element("literal", null, "Literal_Element");
            Add_Element("lom aggregation level", null, "LOM_AggregationLevel_Element");
            Add_Element("lom context", null, "LOM_Contenxt_Element");
            Add_Element("lom difficulty level", null, "LOM_DifficultyLevel_Element");
            Add_Element("lom intended end user role", null, "LOM_IntendedUser_Element");
            Add_Element("lom interactivity level", null, "LOM_InteractivityLevel_Element");
            Add_Element("lom interactivity type", null, "LOM_InteractivityType_Element");
            Add_Element("lom learning resource type", null, "LOM_ResourceType_Element");
            Add_Element("lom status", null, "LOM_Status_Element");
            Add_Element("lom typical age range", null, "LOM_TypicalAgeRange_Element");
            Add_Element("main thumbnail", null, "Main_Thumbnail_Element");
            Add_Element("manufacturer", "simple", "Manufacturer_Element");
            Add_Element("manufacturer", null, "Manufacturer_Complex_Element");
            Add_Element("material received date", null, "Material_Received_Date");
            Add_Element("note", "simple", "Note_Element");
            Add_Element("note", null, "Note_Complex_Element");
            Add_Element("oclc record num", null, "OCLC_Record_Number_Element");
            Add_Element("other title", "simple", "Other_Title_Element");
            Add_Element("other title", null, "Other_Title_Form_Element");
            Add_Element("other url", "simple", "Other_URL_Element");
            Add_Element("other url", null, "Other_URL_Form_Element");
            Add_Element("primary identifier", null, "Primary_Alt_Identifier_Element");
            Add_Element("project", null, "Projects_Element");
            Add_Element("publication place", null, "Publication_Place_Element");
            Add_Element("publication status", null, "Publication_Status_Element");
            Add_Element("publisher", "simple", "Publisher_Element");
            Add_Element("publisher", null, "Publisher_Complex_Element");
            Add_Element("record origin", null, "Record_Origin_Element");
            Add_Element("record status", null, "RecordStatus_Element");
            Add_Element("related item", null, "Related_Item_Form_Element");
            Add_Element("rights", "ir", "IR_Rights_Element");
            Add_Element("rights", null, "Rights_Element");
            Add_Element("scale", null, "Scale_Element");
            Add_Element("serial hierarchy", "panel", "Serial_Hierarchy_Panel_Element");
            Add_Element("serial hierarchy", null, "Serial_Hierarchy_Form_Element");
            Add_Element("source", null, "Source_Element");
            Add_Element("spatial", "simple", "Spatial_Coverage_Element");
            Add_Element("spatial", null, "Hierarchical_Spatial_Form_Element");
            Add_Element("subject", "scheme", "Subject_Scheme_Element");
            Add_Element("subject", "simple", "Subject_Element");
            Add_Element("subject", null, "Subject_Keyword_Standard_Form_Element");
            Add_Element("sudoc", null, "SuDOC_Element");
            Add_Element("target audience", null, "Target_Audience_Element");
            Add_Element("temporal", "simple", "Temporal_Coverage_Element");
            Add_Element("temporal", null, "Temporal_Complex_Element");
            Add_Element("tickler", null, "Tickler_Element");
            Add_Element("title", "simple", "Title_Main_Element");
            Add_Element("title", null, "Title_Main_Form_Element");
            Add_Element("tracking box", null, "Tracking_Box_Element");
            Add_Element("type", "ir", "IR_Type_Element");
            Add_Element("type", "simple", "Type_Element");
            Add_Element("type", null, "Type_Format_Form_Element");
            Add_Element("vid", null, "VID_Element");
            Add_Element("viewer", null, "Viewer_Element");
            Add_Element("visibility", null, "Visibility_Element");
            Add_Element("vra cultural context", null, "VRA_CulturalContext_Element");
            Add_Element("vra inscription", null, "VRA_Inscription_Element");
            Add_Element("vra material", null, "VRA_Material_Element");
            Add_Element("vra measurement", null, "VRA_Measurement_Element");
            Add_Element("vra state edition", null, "VRA_StateEdition_Element");
            Add_Element("vra style period", null, "VRA_StylePeriod_Element");
            Add_Element("vra technique", null, "VRA_Technique_Element");
            Add_Element("web skin", null, "Web_Skin_Element");
            Add_Element("wordmark", null, "Wordmark_Element");
            Add_Element("zoological taxonomy", null, "Zoological_Taxonomy_Form_Element");
        }

        /// <summary> Add a new metadata template element configuration to this class </summary>
        /// <param name="Type"> The 'type' value used in the template configuration files
        ///  to select this metadata template element </param>
        /// <param name="Subtype"> The 'subtype' value used in the template configuration files
        ///  to select this metadata template element </param>
        /// <param name="Class"> Fully qualified (including namespace) name of the class used 
        /// for this template element </param>
        /// <returns> Built and added <see cref="TemplateElement" /> object </returns>
        public TemplateElement Add_Element(string Type, string Subtype, string Class)
        {
            return Add_Element(Type, Subtype, Class, null);
        }

        /// <summary> Add a new metadata template element configuration to this class </summary>
        /// <param name="Type"> The 'type' value used in the template configuration files
        ///  to select this metadata template element </param>
        /// <param name="Subtype"> The 'subtype' value used in the template configuration files
        ///  to select this metadata template element </param>
        /// <param name="Class"> Fully qualified (including namespace) name of the class used 
        /// for this template element </param>
        /// <param name="Assembly"> Name of the assembly within which this class resides, unless this
        /// is one of the default elements included in the core code </param>
        /// <returns> Built and added <see cref="TemplateElement" /> object </returns>
        public TemplateElement Add_Element(string Type, string Subtype, string Class, string Assembly)
        {
            // Ensure the dictionary is built
            if (elementDictionary == null)
                elementDictionary = new Dictionary<string, TemplateElement>(StringComparer.OrdinalIgnoreCase);

            // Does the element dictionary match the current elements list?
            if (elementDictionary.Count != Elements.Count)
            {
                foreach (TemplateElement existing in Elements)
                {
                    if (!String.IsNullOrEmpty(existing.Subtype))
                        elementDictionary[existing.Type + "|" + existing.Subtype] = existing;
                    else
                        elementDictionary[existing.Type] = existing;
                }
            }

            // Create the dictionary match key
            string key = Type;
            if (!String.IsNullOrEmpty(Subtype))
                key = Type + "|" + Subtype;

            // Does this already exist?
            if (elementDictionary.ContainsKey(key))
            {
                // Already exists
                TemplateElement existing = elementDictionary[key];
                existing.Type = Type;
                existing.Subtype = Subtype;
                existing.Assembly = Assembly;
                existing.Class = Class;
                return existing;
            }


            // New, so add it
            TemplateElement newElement = new TemplateElement
            {
                Type = Type,
                Subtype = Subtype,
                Assembly = Assembly,
                Class = Class
            };
            elementDictionary[key] = newElement;

            // Return the newly built element
            return newElement;

        }
    }
}
