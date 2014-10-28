#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Stores the descriptive metadata for the bibliographic resource </summary>
    [DataContract]
    public class DisplayItem_Description
    {
        /// <summary> Collection of summary/abstracts associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Abstract> abstracts { get; internal set; }

        /// <summary> Rights associated with the use of this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_AccessCondition accessCondition  { get; internal set; }

        /// <summary> Collection of classifications associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Classification> classifications { get; internal set; }

        /// <summary> Protected field contains the donor object for this material  </summary>
        [DataMember(EmitDefaultValue = false)]
        public string donor  { get; internal set; }

        /// <summary> Collection of genre terms associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Genre> genres { get; internal set; }

        /// <summary> Identifiers associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Identifier> identifiers { get; internal set; }

        /// <summary> Languages associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Language> languages { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_Institution holdingLocation { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_Institution sourceInstitution { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public string permanentUrl { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_URL otherUrl { get; internal set; }

        /// <summary> Main title associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_Title mainTitle { get; internal set; }
        
        /// <summary> All named entity information (main author, etc..) for this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Name> names { get; internal set; }

        /// <summary> Collection of notes associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Note> notes { get; internal set; }

        /// <summary> Collection of dates associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Date> dates { get; internal set; }

        /// <summary> Edition of this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public string edition { get; internal set; }

        /// <summary> Frequency that this item is published ( i.e., 'monthly', 'daily', etc.. )  </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> frequencies { get; internal set; }

        /// <summary> Information about the issuance of this digital resource ( i.e., continuing, monographic, single_unit, etc.. ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> issuances { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_OriginPlace> places  { get; internal set; }

        /// <summary> Collection of other titles associated with this digital resource ( i.e., translated, abbreviated, etc.. ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Title> otherTitles { get; internal set; }

        /// <summary> Collection of target audiences associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<string> targetAudiences { get; internal set; }

        /// <summary> Original resource type such as map, aerial photography, book, serial, etc..   </summary>
        [DataMember(EmitDefaultValue = false)]
        public string type { get; internal set; }

        /// <summary> Physical description (extent and notes) of the original resource, which is encoded as a related item in the MODS </summary>
        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_PhysicalDescription physicalDescription { get; internal set; }

        /// <summary> Information about the origin of this record </summary> 
        [DataMember(EmitDefaultValue = false)]
        public string recordOrigin { get; internal set; }

        /// <summary> Collection of related items associated with this digital resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_RelatedItem> relatedItems { get; internal set; }

        /// <summary> Series part information for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_SerialInfo seriesPartInfo { get; internal set; }

        /// <summary> All the subjects, topics, and subject keywords used to describe the subject matter for this resource </summary>
        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_Subjects subjectMatter { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Affiliation> affiliations { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Container> containers { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Publisher> manufacturers { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Publisher> publishers { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Temporal> temporalSubjects { get; internal set; }
        

    }
}