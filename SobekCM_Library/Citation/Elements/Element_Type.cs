#region Using directives

using System;
using SobekCM.Library.Citation.Elements.implemented_elements;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    #region Element_Type Enumeration

    /// <summary> Types of elements which can (potentially) be displayed in the online metadata template </summary>
    public enum Element_Type : short
	{
        /// <summary> ERROR type occurs when mapping to the enumeration fails </summary>
		Error = 0,

        /// <summary> Abstract/summary element type maps to <see cref="Abstract_Summary_Element"/> or <see cref="Abstract_Complex_Element"/> </summary>
        Abstract = 1,

        /// <summary> This special note field maps to <see cref="Acquisition_Note_Element"/> and is generally reserved from editing by users </summary>
        Acquisition,

        /// <summary> Flag indicates if this item requires additional work </summary>
        Additional_Work_Needed,

        /// <summary> Allows entry of affiliation information either for the digital resource in 
        /// general or for the just a single named entity (FUTURE PLAN)</summary>
        Affiliation,

        /// <summary> List of aggregationPermissions (non-institutional) to which material is linked </summary>
        Aggregations,

        /// <summary> This special note field maps to <see cref="Attribution_Element"/> and is generally reserved from editing by users </summary>
        Attribution,

        /// <summary> Bibliographic identifier (BibID) element type maps to the <see cref="BibID_Element"/> </summary>
        BibID,

        /// <summary> Flag indicates the item is born digital </summary>
        Born_Digital,

        /// <summary> Identifier/number for this item in the primary catalog record system, used for inputting new items and maps to the <see cref="Catalog_Record_Number_Element" /> </summary>
        Catalog_Record_Number,

        /// <summary> Authority-established classification for this item which maps to <see cref="Classification_Element"/> </summary>
        Classification,

        /// <summary> Holds all the container (finding guide position) information for this item </summary>
        Container,

        /// <summary> Contributor element is used for dublin-core like entry and maps to <see cref="Contributor_Element"/> </summary>
        Contributor,

        /// <summary> Coordinate element type maps to <see cref="Coordinates_Point_Element"/> </summary>
        Coordinates,

        /// <summary> Creator element type maps to <see cref="Creator_Element"/>, <see cref="Creator_Complex_Element"/>, or <see cref="Name_Form_Element"/> </summary>
        Creator,

        /// <summary> Creator notes element type maps to <see cref="Creator_Notes_Element"/> </summary>
        CreatorNotes,

        /// <summary> Flag indicates the item is DARK, and should not be allowed to be made public </summary>
        Dark_Flag,
        
        /// <summary> Publication date element type maps to <see cref="Date_Element"/> </summary>
        Date,

        /// <summary> Copyright date element type maps to <see cref="Date_Copyrighted_Element"/> </summary>
        DateCopyrighted,

        /// <summary> Standard used for the encoding of this record </summary>
        DescriptionStandard,

        /// <summary> Advice on how the item should be disposed of once digitization is complete </summary>
        Disposition_Advice,

        /// <summary> Donor element type maps to <see cref="Donor_Element"/> </summary>
        Donor,

        /// <summary> Download element type maps to <see cref="Downloads_Element"/> </summary>
        Download,

        /// <summary> Related EAD element type maps to <see cref="EAD_Form_Element"/> </summary>
        EAD,

        /// <summary> Edition element type maps to <see cref="Edition_Element"/> </summary>
        Edition,

		/// <summary> Date the embargo on this item is set to expire </summary>
		EmbargoDate,

        /// <summary> Embedded video module stores all the HTML to embed a video </summary>
        EmbeddedVideo,

        /// <summary> Encoding level element type maps to <see cref="Encoding_Level_Element"/> </summary>
        EncodingLevel,

        /// <summary> Electronic Thesis and Disseratation information - name of the committee chair, surname first </summary>
        ETD_CommitteeChair,

        /// <summary> Electronic Thesis and Disseratation information - name of the committee co-chair, surname first </summary>
        ETD_CommitteeCoChair,

        /// <summary> Electronic Thesis and Disseratation information - name of the committee members, surname first </summary>
        ETD_CommitteeMember,

        /// <summary> Electronic Thesis and Disseratation information - type of degree granted </summary>
        ETD_Degree,

        /// <summary> Electronic Thesis and Disseratation information - discipline </summary>
        ETD_DegreeDiscipline,

		/// <summary> Electronic Thesis and Disseratation information - division </summary>
		ETD_DegreeDivision,

        /// <summary> Electronic Thesis and Disseratation information - name of the institution in standard form </summary>
        ETD_DegreeGrantor,

        /// <summary> Electronic Thesis and Disseratation information - either 'Thesis' or 'Dissertation' </summary>
        ETD_DegreeLevel,

        /// <summary> Electronic Thesis and Disseratation information - student's graduation date </summary>
        ETD_GraduationDate,

		/// <summary> Electronic Thesis and Dissertation information - student's graduation semester ( i.e., 'Fall 2012' ) </summary>
		ETD_GraduationSemester,

        /// <summary> Electronic Thesis and Dissertation inforamtion - send to UMI information </summary>
        ETD_UMI,

        /// <summary> Frequency element type maps to <see cref="FAST_Subject_Element"/> </summary>
        FAST_Subject,

        /// <summary> FDA Account element type does not currently map to any element (FUTURE PLAN) </summary>
        FDA_Account,

        /// <summary> FDA Project element type does not currently map to any element (FUTURE PLAN)</summary>
        FDA_Project,

        /// <summary> FDA SubAccount element type does not currently map to any element (FUTURE PLAN)</summary>
        FDA_SubAccount,

        /// <summary> FDA Format is a simple entry element for physical description and maps to <see cref="Format_Element"/> </summary>
        Format,

        /// <summary> Frequency element type maps to <see cref="Frequency_Element"/> </summary>
        Frequency,

        /// <summary> Genre element type maps to <see cref="Genre_Element"/> </summary>
        Genre,

        /// <summary> Group title element type maps to <see cref="Group_Title_Element"/> </summary>
        Group_Title,

        /// <summary> Work_History element type does not currently map to any element (FUTURE PLAN)</summary>
        History,

        /// <summary> Holding location element types maps to <see cref="Holding_Element"/> </summary>
        Holding,

        /// <summary> Identifier element type maps to <see cref="Identifier_Element"/> </summary>
        Identifier,

        /// <summary> Language element type maps to <see cref="Language_Element"/> </summary>
        Language,

		/// <summary> Library of congress call number </summary>
		LCCN,

		/// <summary> Literal element just allows user to add some text </summary>
		Literal,

        /// <summary> Learning object metadata - functional granularity of this learning object </summary>
        LOM_Aggregation_Level,

        /// <summary> Learning object metadata - category describes where this learning object falls within a particular classification schema </summary>
        LOM_Classification_Taxonomy,

        /// <summary> Learning object metadata - principal environment within which the learning and use of the learning object is intended to take place </summary>
        LOM_Context,

        /// <summary> Learning object metadata - how hard it is to work with or through this learning object for the typical intended target audience </summary>
        LOM_Difficulty_Level,

        /// <summary> Learning object metadata - principal user(s) for which this learning object was designed </summary>
        LOM_Intended_End_User_Role,

        /// <summary> Learning object metadata - degree of interactivity characterizing this learning object.  Refers to degree to which the learner can influence the aspect or behavior of the learning object. </summary>
        LOM_Interactivity_Level,

        /// <summary> Learning object metadata - predominant mode of learning supported by this learning object </summary>
        LOM_Interactivity_Type,

        /// <summary> Learning object metadata - specific kind of learning object </summary>
        LOM_Learning_Resource_Type,

        /// <summary> Learning object metadata - completion status or condition of this learning object </summary>
        LOM_Status,

        /// <summary> Learning object metadata - technical capabilities necessary for using this learning object </summary>
        LOM_System_Requirements,

        /// <summary> Learning object metadata - age (range) of the typical intended user </summary>
        LOM_Typical_Age_Range,

        /// <summary> Learning object metadata - approximate or typical time it takes to work with or through this learning object for the typical intended audience </summary>
        LOM_Typical_Learning_Time,

        /// <summary> Main thumbnail element type maps to <see cref="Main_Thumbnail_Element"/> </summary>
        MainThumbnail,

        /// <summary> Manufacturer element type maps to <see cref="Manufacturer_Element"/> or <see cref="Manufacturer_Complex_Element"/> </summary>
        Manufacturer,

        /// <summary> Materials received element type maps to <see cref="Material_Received_Date"/> </summary>
        Material_Received_Date,

        /// <summary> Object ID for the METS file </summary>
        METS_ObjectID,

        /// <summary> Note element type maps to <see cref="Note_Element"/> or <see cref="Note_Complex_Element"/> </summary>
        Note,

        /// <summary> Identifier/number for this item in the OCLC system, used for inputting new items and maps to the <see cref="OCLC_Record_Number_Element" /> </summary>
        OCLC_Record_Number,

        /// <summary> Other files element type does not currently map to any element (FUTURE PLAN) </summary>
        OtherFiles,

        /// <summary> Other URL element type maps to <see cref="Other_URL_Form_Element"/> or <see cref="Other_URL_Element" /> </summary>
        OtherURL,

        /// <summary> Primary alternate identifier associated with this item group, maps to <see cref="Primary_Alt_Identifier_Element"/> </summary>
        Primary_Identifier,
        
        /// <summary>Project element type maps to <see cref="Projects_Element"/> </summary>
        Project,

        /// <summary> Publication place element type maps to <see cref="Publication_Place_Element"/> </summary>
        Publication_Place,

        /// <summary> Publication status element type maps to <see cref="Publication_Status_Element"/> </summary>
        Publication_Status,

        /// <summary> Publisher element type maps to <see cref="Publisher_Element"/> and <see cref="Publisher_Complex_Element"/>  </summary>
        Publisher,

        /// <summary> Record origin element type maps to <see cref="Record_Origin_Element"/> </summary>
        RecordOrigin,

        /// <summary> Record status element type maps to <see cref="RecordStatus_Element"/> </summary>
        RecordStatus,

        /// <summary> Related item element type maps to <see cref="Related_Item_Form_Element"/> </summary>
        RelatedItem,

        /// <summary> Rights element type maps to <see cref="Rights_Element"/> </summary>
        Rights,

        /// <summary> Allows the embargo date to be set for an item which is currently under embargo </summary>
        Rights_Embargo_Date,


        /// <summary> Scale element type maps to <see cref="Scale_Element"/> </summary>
        Scale,

        /// <summary> Serial hierarchy element type maps to <see cref="Serial_Hierarchy_Form_Element"/> </summary>
        SerialHierarchy,

        /// <summary> Source institution element type maps to <see cref="Source_Element"/> </summary>
        Source,

        /// <summary> Spatial coverage element type maps to <see cref="Spatial_Coverage_Element"/> </summary>
        Spatial,

        /// <summary> Structure map element type does not currently map to any element (FUTURE PLAN)</summary>
        Structure_Map,

        /// <summary> Subject element type maps to <see cref="Subject_Element"/>, <see cref="Subject_Scheme_Element"/>, or <see cref="Subject_Keyword_Standard_Form_Element"/> </summary>
        Subject,

		/// <summary> Superintendent of Documents Classification System </summary>
		SuDoc,

        /// <summary> Target audience element type maps to <see cref="Target_Audience_Element"/> </summary>
        TargetAudience,

        /// <summary> Temporal covage element type maps to <see cref="Temporal_Coverage_Element"/> or <see cref="Temporal_Complex_Element"/> </summary>
        Temporal,

        /// <summary> Text displayable flag does not currently map to any element (FUTURE PLAN) </summary>
        TextDisplayable,

        /// <summary> Text searchable flag does not currently map to any element (FUTURE PLAN)</summary>
        TextSearchable,

        /// <summary> Tickler field is used for internally searching for sets </summary>
        Tickler,

        /// <summary> Main title element type maps to <see cref="Title_Main_Element"/> or <see cref="Title_Main_Form_Element"/> </summary>
        Title,

        /// <summary> Other titles element type maps to <see cref="Other_Title_Element"/> or <see cref="Other_Title_Form_Element"/> </summary>
        Title_Other,

        /// <summary> Identifies the tracking box the physical item is within for physical material tracking </summary>
        Tracking_Box,

        /// <summary> Resource type element type maps to <see cref="Type_Element"/> or <see cref="Type_Format_Form_Element"/> </summary>
        Type,

        /// <summary> Volume identifier (VID) element type maps to <see cref="VID_Element"/> </summary>
        VID,

        /// <summary> Viewer element type maps to <see cref="Viewer_Element"/> </summary>
        Viewer,

        /// <summary> Visibility element type maps to <see cref="Visibility_Element"/> </summary>
        Visibility,

        /// <summary> VRACore element for cultural context of the visual resource </summary>
        VRA_CulturalContext,

        /// <summary> VRACore element for inscriptions on the visual resource </summary>
        VRA_Inscription,

        /// <summary> VRACore element for material making up the visual resource </summary>
        VRA_Material,

        /// <summary> VRACore element for measurements related to the visual resource </summary>
        VRA_Measurement,

        /// <summary> VRACore element for state or edition of the visual resource </summary>
        VRA_StateEdition,

        /// <summary> VRACore element for the style or period represented by the visual resource </summary>
        VRA_StylePeriod,

        /// <summary> VRACore element for technique used for creation of the visual resource </summary>
        VRA_Technique,

        /// <summary> HTML SobekCM web skin element type maps to <see cref="Web_Skin_Element"/> </summary>
        Web_Skin,

        /// <summary> Wordmark/Icon element type maps to <see cref="Wordmark_Element"/> </summary>
        Wordmark,

        /// <summary> Zoological taxononic information - complete classification </summary>
        Zoological_Taxonomy,

        /// <summary> Zoological taxononic information - class classification </summary>
        ZT_Class,

        /// <summary> Zoological taxononic information - common or vernacular name </summary>
        ZT_CommonName,

        /// <summary> Zoological taxononic information - family classification </summary>
        ZT_Family,

        /// <summary> Zoological taxononic information - genus classification </summary>
        ZT_Genus,

        /// <summary> Zoological taxononic information - higher classification DarwinCore field </summary>
        ZT_HigherClassification,

        /// <summary> Zoological taxononic information - kingdom classification </summary>
        ZT_Kingdom,

        /// <summary> Zoological taxononic information - order classification </summary>
        ZT_Order,

        /// <summary> Zoological taxononic information - phylum classification </summary>
        ZT_Phylum,

        /// <summary> Zoological taxononic information - scientific name </summary>
        ZT_ScientificName,

        /// <summary> Zoological taxononic information - first or species epithet of the scientific name </summary>
        ZT_SpecificEpithet,

        /// <summary> Zoological taxononic information - taxonomic rank of the most specific name given </summary>
        ZT_TaxonRank
	}
    
    #endregion

    /// <summary> Static class performs conversions between <see cref="Element_Type"/> enumeration and strings </summary>
	public class Element_Type_Convertor
	{
        /// <summary> Static method converts from type string to the <see cref="Element_Type"/> enumeration </summary>
        /// <param name="Type"> Element type as a string </param>
        /// <returns> Element type as the enumerational value </returns>
		public static Element_Type ToType( string Type )
		{
			switch ( Type.ToUpper().Replace(" ","_") )
            {
                case "ABSTRACT":
                    return Element_Type.Abstract;

                case "ACQUISITION":
                    return Element_Type.Acquisition;

                case "ADDITIONAL_WORK_NEEDED":
                    return Element_Type.Additional_Work_Needed;

                case "AFFILIATION":
                    return Element_Type.Affiliation;

                case "AGGREGATIONS":
                case "COLLECTION":
                case "COLLECTION_PRIMARY":
                case "COLLECTION_ALTERNATE":
                    return Element_Type.Aggregations;

                case "ATTRIBUTION":
                    return Element_Type.Attribution;

                case "BIB":
                case "BIBID":
                    return Element_Type.BibID;

                case "BORN_DIGITAL":
                    return Element_Type.Born_Digital;

                case "CATALOGRECORDNUM":
                case "CATALOG_RECORD_NUM":
                    return Element_Type.Catalog_Record_Number;

                case "CLASSIFICATION":
                    return Element_Type.Classification;

                case "CONTAINER":
                    return Element_Type.Container;

                case "CONTRIBUTOR":
                    return Element_Type.Contributor;

                case "COORDINATES":
                    return Element_Type.Coordinates;

                case "CREATOR":
                    return Element_Type.Creator;

                case "CREATORNOTES":
                case "CREATOR_NOTES":
                    return Element_Type.CreatorNotes;

                case "DARKFLAG":
                case "DARK_FLAG":
                    return Element_Type.Dark_Flag;

                case "DATE":
                    return Element_Type.Date;

                case "DATECOPYRIGHTED":
                case "DATE_COPYRIGHTED":
                    return Element_Type.DateCopyrighted;

                case "DESCRIPTIONSTANDARD":
                case "DESCRIPTION_STANDARD":
                    return Element_Type.DescriptionStandard;

                case "DISPOSITIONADVICE":
                case "DISPOSITION_ADVICE":
                    return Element_Type.Disposition_Advice;

                case "DONOR":
                    return Element_Type.Donor;

                case "DOWNLOAD":
                    return Element_Type.Download;

                case "EAD":
                    return Element_Type.EAD;

                case "EDITION":
                    return Element_Type.Edition;

				case "EMBARGODATE":
					return Element_Type.EmbargoDate;

                case "EMBEDDEDVIDEO":
                case "EMBEDDED_VIDEO":
                    return Element_Type.EmbeddedVideo;

                case "ENCODINGLEVEL":
                case "ENCODING_LEVEL":
                    return Element_Type.EncodingLevel;

                case "ETD_COMMITTEECHAIR":
                case "ETD_COMMITTEE_CHAIR":
                    return Element_Type.ETD_CommitteeChair;

                case "ETD_COMMITTEECOCHAIR":
                case "ETD_COMMITTEE_COCHAIR":
                    return Element_Type.ETD_CommitteeCoChair;

                case "ETD_COMMITTEEMEMBER":
                case "ETD_COMMITTEE_MEMBER":
                    return Element_Type.ETD_CommitteeMember;

                case "ETD_DEGREE":
                    return Element_Type.ETD_Degree;

                case "ETD_DEGREEDISCIPLINE":
                case "ETD_DEGREE_DISCIPLINE":
                    return Element_Type.ETD_DegreeDiscipline;

				case "ETD_DEGREEDIVISION":
                case "ETD_DEGREE_DIVISION":
                    return Element_Type.ETD_DegreeDivision;

                case "ETD_DEGREEGRANTOR":
                case "ETD_DEGREE_GRANTOR":
                    return Element_Type.ETD_DegreeGrantor;

                case "ETD_DEGREELEVEL":
                case "ETD_DEGREE_LEVEL":
                    return Element_Type.ETD_DegreeLevel;

                case "ETD_GRADUATIONDATE":
                case "ETD_GRADUATION_DATE":
                    return Element_Type.ETD_GraduationDate;

				case "ETD_GRADUATIONSEMESTER":
                case "ETD_GRADUATION_SEMESTER":
                    return Element_Type.ETD_GraduationSemester;

                case "ETD_UMI":
                    return Element_Type.ETD_UMI;

                case "FAST_SUBJECT":
                    return Element_Type.FAST_Subject;

                case "FDA_ACCOUNT":
                    return Element_Type.FDA_Account;

                case "FDA_PROJECT":
                    return Element_Type.FDA_Project;

                case "FDA_SUBACCOUNT":
                    return Element_Type.FDA_SubAccount;

                case "FORMAT":
                    return Element_Type.Format;

                case "FREQUENCY":
                    return Element_Type.Frequency;

                case "GENRE":
                    return Element_Type.Genre;

                case "GROUPTITLE":
                case "GROUP_TITLE":
                    return Element_Type.Group_Title;

                case "HOLDING":
                    return Element_Type.Holding;

                case "IDENTIFIER":
                    return Element_Type.Identifier;

                case "LANGUAGE":
                    return Element_Type.Language;

				case "LCCN":
					return Element_Type.LCCN;

				case "LITERAL":
					return Element_Type.Literal;

                case "LOM_AGGREGATION_LEVEL":
                    return Element_Type.LOM_Aggregation_Level;

                case "LOM_CLASSIFICATION_TAXONOMY":
                    return Element_Type.LOM_Classification_Taxonomy;

                case "LOM_CONTEXT":
                    return Element_Type.LOM_Context;

                case "LOM_DIFFICULTY_LEVEL":
                    return Element_Type.LOM_Difficulty_Level;

                case "LOM_INTENDED_END_USER_ROLE":
                    return Element_Type.LOM_Intended_End_User_Role;

                case "LOM_INTERACTIVITY_LEVEL":
                    return Element_Type.LOM_Interactivity_Level;

                case "LOM_INTERACTIVITY_TYPE":
                    return Element_Type.LOM_Interactivity_Type;

                case "LOM_LEARNING_RESOURCE_TYPE":
                    return Element_Type.LOM_Learning_Resource_Type;

                case "LOM_STATUS":
                    return Element_Type.LOM_Status;

                case "LOM_SYSTEM_REQUIREMENTS":
                    return Element_Type.LOM_System_Requirements;

                case "LOM_TYPICAL_AGE_RANGE":
                    return Element_Type.LOM_Typical_Age_Range;

                case "LOM_TYPICAL_LEARNING_TIME":
                    return Element_Type.LOM_Typical_Learning_Time;

                case "THUMBNAIL":
                case "MAIN THUMBNAIL":
                    return Element_Type.MainThumbnail;

                case "MANUFACTURER":
                    return Element_Type.Manufacturer;

                case "MATERIAL_RECEIVED_DATE":
                    return Element_Type.Material_Received_Date;

                case "METS_OBJECTID":
                case "OBJECTID":
                    return Element_Type.METS_ObjectID;

                case "NOTE":
                case "NOTES":
                    return Element_Type.Note;

                case "OCLCRECORDNUM":
                case "OCLC_RECORD_NUM":
                case "OCLC_RECORD_NUMBER":
                    return Element_Type.OCLC_Record_Number;

                case "OTHERFILES":
                    return Element_Type.OtherFiles;

                case "OTHERURL":
                case "OTHER_URL":
                    return Element_Type.OtherURL;

                case "PRIMARYIDENTIFIER":
                case "PRIMARY_IDENTIIER":
                    return Element_Type.Primary_Identifier;

                case "PROJECT":
                    return Element_Type.Project;

                case "PUBLICATIONPLACE":
                case "PUBLICATION_PLACE":
                    return Element_Type.Publication_Place;

                case "PUBLICATIONSTATUS":
                case "PUBLICATION_STATUS":
                    return Element_Type.Publication_Status;

                case "PUBLISHER":
                    return Element_Type.Publisher;

                case "RECORDORIGIN":
                case "RECORD_ORIGIN":
                    return Element_Type.RecordOrigin;

                case "RECORDSTATUS":
                case "RECORD_STATUS":
                    return Element_Type.RecordStatus;

                case "RELATEDITEM":
                case "RELATED_ITEM":
                    return Element_Type.RelatedItem;

                case "RIGHTS":
                    return Element_Type.Rights;

                case "RIGHTS_EMBARGO_DATE":
                    return Element_Type.Rights_Embargo_Date;

                case "SCALE":
                    return Element_Type.Scale;

                case "SERIALHIERARCHY":
                case "SERIAL_HIERARCHY":
                    return Element_Type.SerialHierarchy;

                case "SOURCE":
                    return Element_Type.Source;

                case "SPATIAL":
                    return Element_Type.Spatial;

                case "STRUCTUREMAP":
                    return Element_Type.Structure_Map;

                case "SUBJECT":
                    return Element_Type.Subject;

				case "SUDOC":
					return Element_Type.SuDoc;

                case "TARGETAUDIENCE":
                case "TARGET_AUDIENCE":
                    return Element_Type.TargetAudience;

                case "TEMPORAL":
                    return Element_Type.Temporal;

                case "TEXTDISPLAYABLE":
                    return Element_Type.TextDisplayable;

                case "TEXTSEARCHABLE":
                    return Element_Type.TextSearchable;



                case "TICKLER":
                    return Element_Type.Tickler;

                case "TITLE":
                    return Element_Type.Title;

                case "OTHERTITLE":
                case "OTHER_TITLE":
                case "TITLE_OTHER":
                    return Element_Type.Title_Other;

                case "TRACKING_BOX":
                    return Element_Type.Tracking_Box;

                case "TYPE":
                    return Element_Type.Type;

                case "VID":
                    return Element_Type.VID;

                case "VIEWER":
                    return Element_Type.Viewer;

                case "VISIBILITY":
                    return Element_Type.Visibility;

                case "VRA_CULTURALCONTEXT":
                case "CULTURALCONTEXT":
                case "VRA_CULTURAL_CONTEXT":
                case "CULTURAL_CONTEXT":
                    return Element_Type.VRA_CulturalContext;

                case "VRA_INSCRIPTION":
                case "INSCRIPTION":
                    return Element_Type.VRA_Inscription;

                case "VRA_MATERIAL":
                case "MATERIAL":
                case "VRA_MATERIALS":
                case "MATERIALS":
                    return Element_Type.VRA_Material;

                case "VRA_MEASUREMENT":
                case "MEASUREMENT":
                case "VRA_MEASUREMENTS":
                case "MEASUREMENTS":
                    return Element_Type.VRA_Measurement;

                case "VRA_STATEEDITION":
                case "STATEEDITION":
                case "VRA_STATE_EDITION":
                case "STATE_EDITION":
                    return Element_Type.VRA_StateEdition;

                case "VRA_STYLEPERIOD":
                case "STYLEPERIOD":
                case "VRA_STYLE_PERIOD":
                case "STYLE_PERIOD":
                    return Element_Type.VRA_StylePeriod;

                case "VRA_TECHNIQUE":
                case "TECHNIQUE":
                    return Element_Type.VRA_Technique;

                case "WEBSKIN":
                case "WEB_SKIN":
                case "INTERFACE":
                    return Element_Type.Web_Skin;

                case "ICON":
                case "WORDMARK":
                    return Element_Type.Wordmark;

                case "ZOOLOGICAL_TAXONOMY":
                    return Element_Type.Zoological_Taxonomy;

                case "ZT_Class":
                    return Element_Type.ZT_Class;

                case "ZT_COMMONNAME":
                    return Element_Type.ZT_CommonName;

                case "ZT_FAMILY":
                    return Element_Type.ZT_Family;

                case "ZT_GENUS":
                    return Element_Type.ZT_Genus;

                case "ZT_HIGHERCLASSIFICATION":
                    return Element_Type.ZT_HigherClassification;

                case "ZT_KINGDOM":
                    return Element_Type.ZT_Kingdom;

                case "ZT_ORDER":
                    return Element_Type.ZT_Order;

                case "ZT_PHYLUM":
                    return Element_Type.ZT_Phylum;

                case "ZT_SCIENTIFICNAME":
                    return Element_Type.ZT_ScientificName;

                case "ZT_SCIENTIFICEPITHET":
                    return Element_Type.ZT_SpecificEpithet;

                case "ZT_TAXONRANK":
                    return Element_Type.ZT_TaxonRank;


			}

			// Default of empty string
			return Element_Type.Error;
		}

        /// <summary> Static method converts from the <see cref="Element_Type"/> enumeration to a string </summary>
        /// <param name="Type"> Element type as the enumerational value </param>
        /// <returns> Element type as a string </returns>
		public static string ToString( Element_Type Type )
		{
			switch ( Type )
			{
                case Element_Type.Abstract:
                    return "Abstract";

                case Element_Type.Acquisition:
                    return "Acquisition";

                case Element_Type.Additional_Work_Needed:
			        return "Additional_Work_Needed";

                case Element_Type.Affiliation:
                    return "Affiliation";

                case Element_Type.Aggregations:
                    return "Aggregations";

                case Element_Type.Attribution:
                    return "Attribution";

                case Element_Type.BibID:
                    return "BibID";

                case Element_Type.Born_Digital:
                    return "Born_Digital";

                case Element_Type.Catalog_Record_Number:
                    return "CatalogRecordNum";

                case Element_Type.Classification:
                    return "Classification";

                case Element_Type.Container:
                    return "Container";

                case Element_Type.Contributor:
                    return "Contributor";

                case Element_Type.Coordinates:
                    return "Coordinates";

                case Element_Type.Creator:
                    return "Creator";

                case Element_Type.CreatorNotes:
                    return "CreatorNotes";

                case Element_Type.Dark_Flag:
                    return "Dark_Flag";

                case Element_Type.Date:
                    return "Date";

                case Element_Type.DateCopyrighted:
                    return "DateCopyrighted";

                case Element_Type.DescriptionStandard:
                    return "DescriptionStandard";

                case Element_Type.Disposition_Advice:
                    return "Disposition_Advice";

                case Element_Type.Donor:
                    return "Donor";

                case Element_Type.Download:
                    return "Download";

                case Element_Type.EAD:
                    return "EAD";

                case Element_Type.Edition:
                    return "Edition";

				case Element_Type.EmbargoDate:
					return "EmbargoDate";

                case Element_Type.EmbeddedVideo:
			        return "EmbeddedVideo";

                case Element_Type.EncodingLevel:
                    return "EncodingLevel";

				case Element_Type.ETD_CommitteeChair:
					return "ETD_CommitteeChair";

				case Element_Type.ETD_CommitteeCoChair:
					return "ETD_CommitteeCoChair";

				case Element_Type.ETD_CommitteeMember: 
					return "ETD_CommitteeMember";

				case Element_Type.ETD_Degree:
					return "ETD_Degree";

				case Element_Type.ETD_DegreeDiscipline:
					return "ETD_DegreeDiscipline";

				case Element_Type.ETD_DegreeDivision:
					return "ETD_DegreeDivision";

				case Element_Type.ETD_DegreeGrantor:
					return "ETD_DegreeGrantor";

				case Element_Type.ETD_DegreeLevel:
					return "ETD_DegreeLevel";

				case Element_Type.ETD_GraduationDate:
					return "ETD_GraduationDate";

				case Element_Type.ETD_GraduationSemester: 
					return "ETD_GraduationSemester";

				case Element_Type.ETD_UMI:
					return "ETD_UMI";

                case Element_Type.FAST_Subject:
                    return "FAST_Subject";

                case Element_Type.FDA_Account:
                    return "FDA_Account";

                case Element_Type.FDA_Project:
                    return "FDA_Project";

                case Element_Type.FDA_SubAccount:
                    return "FDA_SubAccount";

                case Element_Type.Format:
                    return "Format";

                case Element_Type.Frequency:
                    return "Frequency";

                case Element_Type.Genre:
                    return "Genre";

                case Element_Type.Group_Title:
                    return "GroupTitle";

                case Element_Type.Holding:
                    return "Holding";

                case Element_Type.Identifier:
                    return "Identifier";

                case Element_Type.Language:
                    return "Language";

				case Element_Type.LCCN:
					return "LCCN";

				case Element_Type.Literal:
					return "Literal";

                case Element_Type.Manufacturer:
                    return "Manufacturer";

                case Element_Type.Material_Received_Date:
                    return "Material Received Date";

                case Element_Type.METS_ObjectID:
                    return "METS ObjectID";

                case Element_Type.Note:
                    return "Note";

                case Element_Type.OCLC_Record_Number:
                    return "OclcRecordNum";

                case Element_Type.OtherFiles:
                    return "OtherFiles";

                case Element_Type.Title_Other:
                    return "OtherTitle";

                case Element_Type.OtherURL:
                    return "OtherURL";

                case Element_Type.Primary_Identifier:
                    return "PrimaryIdentifier";

                case Element_Type.Publication_Place:
                    return "PublicationPlace";

                case Element_Type.Publication_Status:
                    return "PublicationStatus";

                case Element_Type.Publisher:
                    return "Publisher";

                case Element_Type.RecordOrigin:
                    return "RecordOrigin";

                case Element_Type.RecordStatus:
                    return "RecordStatus";

                case Element_Type.RelatedItem:
                    return "RelatedItem";

                case Element_Type.Rights:
                    return "Rights";

                case Element_Type.Scale:
                    return "Scale";

                case Element_Type.SerialHierarchy:
                    return "SerialHierarchy";

                case Element_Type.Source:
                    return "Source";

                case Element_Type.Spatial:
                    return "Spatial";

                case Element_Type.Structure_Map:
                    return "StructureMap";

                case Element_Type.Subject:
                    return "Subject";

				case Element_Type.SuDoc:
					return "SuDOC";

                case Element_Type.TargetAudience:
                    return "TargetAudience";

                case Element_Type.Temporal:
                    return "Temporal";

                case Element_Type.MainThumbnail:
                    return "Thumbnail";

                case Element_Type.Tickler:
                    return "Tickler";

                case Element_Type.Title:
                    return "Title";

                case Element_Type.Tracking_Box:
                    return "Tracking Box";

                case Element_Type.Type:
                    return "Type";

                case Element_Type.VID:
                    return "VID";

                case Element_Type.Viewer:
                    return "Viewer";

                case Element_Type.Visibility:
                    return "Visibility";

                case Element_Type.Web_Skin:
                    return "WebSkin";

                case Element_Type.Wordmark:
                    return "Wordmark";

				case Element_Type.TextDisplayable:
					return "TextDisplayable";

				case Element_Type.TextSearchable:
					return "TextSearchable";     
			}

			// Default of empty string
			return String.Empty;
		}
	}
}
