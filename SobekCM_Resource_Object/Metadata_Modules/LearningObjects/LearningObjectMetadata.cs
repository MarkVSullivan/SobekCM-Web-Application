using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SobekCM.Resource_Object.Metadata_Modules.LearningObjects
{

    #region Enumerations used for learning object metadata

    /// <summary> Enumeration of possible level of aggregation for a learning object </summary>
    public enum AggregationLevelEnum : byte
    {
        /// <summary> Aggregation level is not defined </summary>
        UNDEFINED,

        /// <summary> Aggregation level 1 - a single, atomic object </summary>
        level1,

        /// <summary> Aggregation level 2 - a lesson plan </summary>
        level2,

        /// <summary> Aggregation level 3 - a course, or set of lesson plans </summary>
        level3,

        /// <summary> Aggregation level 4 - a set of courses </summary>
        level4
    }

    /// <summary> Enumeration of possible statuses for a learning object </summary>
    public enum StatusEnum : byte
    {
        /// <summary> Status is not defined </summary>
        UNDEFINED,

        /// <summary> Status of DRAFT </summary>
        draft,

        /// <summary> Status of FINAL </summary>
        final,

        /// <summary> Status of REVISED </summary>
        revised,

        /// <summary> Status of UNAVAILABLE </summary>
        unavailable
    }

    /// <summary> Enumeration of different requirement types </summary>
    public enum RequirementTypeEnum : byte
    {
        /// <summary> Requirement type is not defined </summary>
        UNDEFINED,

        /// <summary> Requirements on the BROWSER for using this learning object </summary>
        browser,

        /// <summary> Requirements on the OPERATING SYSTEM for using this learning object </summary>
        operating_system,

        /// <summary> Requirements on the HARDWARE for using this learning object </summary>
        hardware,

        /// <summary> Requirements on the SOFTWARE for using this learning object </summary>
        software
    }

    /// <summary> Enumeration of different interactivity types </summary>
    public enum InteractivityTypeEnum : byte
    {
        /// <summary> Interactivity type is not defined </summary>
        UNDEFINED,

        /// <summary> Learning object is primarily ACTIVE interactivity </summary>
        active,

        /// <summary> Learning object is primarily passive, or EXPOSITIVE, interactivity </summary>
        expositive,

        /// <summary> Learning object is both ACTIVE and EXPOSITIVE </summary>
        mixed
    }

    /// <summary> Enumeration of different interactivity levels </summary>
    public enum InteractivityLevelEnum : byte
    {
        /// <summary> Interactivity level is not defined </summary>
        UNDEFINED,

        /// <summary> VERY LOW interactivity level </summary>
        very_low,

        /// <summary> LOW interactivity level </summary>
        low,

        /// <summary> MEDIUM interactivity level </summary>
        medium,

        /// <summary> HIGH interactivity level </summary>
        high,

        /// <summary> VERY HIGH interactivity level </summary>
        very_high
    }

    /// <summary> Enumeration of intended end user roles </summary>
    public enum IntendedEndUserRoleEnum : byte
    {
        /// <summary> Intended end user role is not defined </summary>
        UNDEFINED,

        /// <summary> Intended end user of this learning object is a TEACHER </summary>
        teacher,

        /// <summary> Intended end user of this learning object is an AUTHOR </summary>
        author,

        /// <summary> Intended end user of this learning object is a LEARNER </summary>
        learner,

        /// <summary> Intended end user of this learning object is a MANAGER </summary>
        manager
    }

    /// <summary> Enumeration of different difficulty levels </summary>
    public enum DifficultyLevelEnum : byte
    {
        /// <summary> Difficulty level is not defined </summary>
        UNDEFINED,

        /// <summary> VERY EASY difficulty level </summary>
        very_easy,

        /// <summary> EASY difficulty level </summary>
        easy,

        /// <summary> MEDIUM difficulty level </summary>
        medium,

        /// <summary> DIFFICULT difficulty level </summary>
        difficult,

        /// <summary> VERY DIFFICULT difficulty level </summary>
        very_difficult
    }

    #endregion

    /// <summary> Contains all the additional data about learning objects and learning resources </summary>
    /// <remarks> Metadata extension module defines the subset of IEEE-LOM, LOM-NL which will be used initially 
    /// by the SobekCM digital repository system as an extension schema in the METS.  Not included are all 
    /// the fields which are already present in the MODS/DC/MARC, such as title, author, keywords, 
    /// related items, etc.. </remarks>
    [Serializable]
    public class LearningObjectMetadata : iMetadata_Module
    {

        private List<LOM_VocabularyState> learningResourceTypes;
        private List<LOM_System_Requirements> systemRequirements;
        private List<IntendedEndUserRoleEnum> intendedEndUserRole;
        private List<LOM_VocabularyState> context;
        private List<LOM_LanguageString> typicalAgeRange;
        private List<LOM_Classification> classifications;

        /// <summary> The functional granularity of this learning object ( IEEE-LOM 1.8 ) </summary>
        public AggregationLevelEnum AggregationLevel { get; set; }

        /// <summary> The completion status or condition of this learning object ( IEEE-LOM 2.2 ) </summary>
        public StatusEnum Status { get; set; }

        /// <summary> Predominant mode of learning supported by this learning object ( IEEE-LOM 5.1 ) </summary>
        public InteractivityTypeEnum InteractivityType { get; set; }

        /// <summary> Degree of interactivity characterizing this learning object.  Refers to degree to which 
        /// the learner can influence the aspect or behavior of the learning object. ( IEEE-LOM 5.3 )  </summary>
        public InteractivityLevelEnum InteractivityLevel { get; set;  }

        /// <summary> How hard it is to work with or through this learning object for the typical intended target audience ( IEEE-LOM 5.8 ) </summary>
        public DifficultyLevelEnum DifficultyLevel { get; set; }

        /// <summary> Approximate or typical time it takes to work with or through this learning object for the typical intended audience ( IEEE-LOM 5.9 ) </summary>
        public string TypicalLearningTime { get; set; }

        /// <summary> Constructor for a new instance of the LearningObjectMetadata class </summary>
        public LearningObjectMetadata()
        {
            AggregationLevel = AggregationLevelEnum.UNDEFINED;
            Status = StatusEnum.UNDEFINED;
            InteractivityType = InteractivityTypeEnum.UNDEFINED;
            InteractivityLevel = InteractivityLevelEnum.UNDEFINED;
            DifficultyLevel = DifficultyLevelEnum.UNDEFINED;
            TypicalLearningTime = String.Empty;

            learningResourceTypes = new List<LOM_VocabularyState>();
            systemRequirements = new List<LOM_System_Requirements>();
            intendedEndUserRole = new List<IntendedEndUserRoleEnum>();
            context = new List<LOM_VocabularyState>();
            typicalAgeRange = new List<LOM_LanguageString>();
            classifications = new List<LOM_Classification>();
        }

        /// <summary> Returns a flag indicating if there is any data stored in this metadata
        /// extension module </summary>
        public bool hasData
        {
            get
            {
                // Check the enumerations first
                if ((AggregationLevel != AggregationLevelEnum.UNDEFINED) || (Status != StatusEnum.UNDEFINED) ||
                    (InteractivityType != InteractivityTypeEnum.UNDEFINED) || (InteractivityLevel != InteractivityLevelEnum.UNDEFINED) ||
                    (DifficultyLevel != DifficultyLevelEnum.UNDEFINED) || (!String.IsNullOrWhiteSpace(TypicalLearningTime)))
                    return true;

                // Check the collections next
                if ((learningResourceTypes.Count > 1) || (systemRequirements.Count > 0) || (intendedEndUserRole.Count > 0) ||
                    (context.Count > 0) || (typicalAgeRange.Count > 0) || (classifications.Count > 0))
                    return true;

                // No data, so return false
                return false;
            }
        }

        #region Properties and methods related to the SYSTEM REQUIREMENTS ( IEEE-LOM 4.4 )

        /// <summary> The technical capabilities necessary for using this learning object ( IEEE-LOM 4.4 ) </summary>
        public ReadOnlyCollection<LOM_System_Requirements> SystemRequirements
        {
            get
            {
                return new ReadOnlyCollection<LOM_System_Requirements>(systemRequirements);
            }
        }

        /// <summary> Add a new system requirement ( IEEE-LOM 4.4 ) </summary>
        /// <param name="Value"></param>
        public void Add_SystemRequirements(LOM_System_Requirements Value)
        {
            systemRequirements.Add(Value);
        }

        /// <summary> Clears the list of all the system requirements ( IEEE-LOM 4.4 ) </summary>
        public void Clear_SystemRequirements()
        {
            systemRequirements.Clear();
        }

        #endregion

        #region Properties and methods related to the LEARNING RESOURCE TYPES ( IEEE-LOM 5.2 )

        /// <summary> Specific kind of learning object.  The most dominant kind should be first ( IEEE-LOM 5.2 ) </summary>
        public ReadOnlyCollection<LOM_VocabularyState> LearningResourceTypes
        {
            get
            {
                return new ReadOnlyCollection<LOM_VocabularyState>(learningResourceTypes);
            }
        }

        /// <summary> Add a new learning resource type ( IEEE-LOM 5.2 ) </summary>
        /// <param name="Value"> Learning resource type string </param>
        public void Add_LearningResourceType( string Value )
        {
            learningResourceTypes.Add(new LOM_VocabularyState(Value, String.Empty));
        }

        /// <summary> Add a new learning resource type ( IEEE-LOM 5.2 )  </summary>
        /// <param name="Value"> Learning resource type string </param>
        /// <param name="Source"> Source or vocabulary from which the learning resource string is derived </param>
        public void Add_LearningResourceType( string Value, string Source )
        {
            learningResourceTypes.Add( new LOM_VocabularyState(Value, Source));
        }

        /// <summary> Add a new learning resource type ( IEEE-LOM 5.2 )  </summary>
        /// <param name="Value"> Learning resource type with possible source value </param>
        public void Add_LearningResourceType(LOM_VocabularyState Value )
        {
            learningResourceTypes.Add( Value );
        }

        /// <summary> Clears the list of all the learning resource types ( IEEE-LOM 5.2 ) </summary>
        public void Clear_LearningResourceTypes()
        {
            learningResourceTypes.Clear();
        }

        #endregion

        #region Properties and methods related to the INTENDED END USER ROLES ( IEEE-LOM 5.5 )

        /// <summary> Principal user(s) for which this learning object was designed, most dominant first ( IEEE-LOM 5.5 ) </summary>
        public ReadOnlyCollection<IntendedEndUserRoleEnum> IntendedEndUserRoles
        {
            get
            {
                return new ReadOnlyCollection<IntendedEndUserRoleEnum>(intendedEndUserRole);
            }
        }

        /// <summary> Add a new intended end user role ( IEEE-LOM 5.5 ) </summary>
        /// <param name="Value"></param>
        public void Add_IntendedEndUserRole(IntendedEndUserRoleEnum Value)
        {
            intendedEndUserRole.Add(Value);
        }

        /// <summary> Clears the list of all the intended end user roles ( IEEE-LOM 5.5 ) </summary>
        public void Clear_IntendedEndUserRoles()
        {
            intendedEndUserRole.Clear();
        }

        #endregion

        #region Properties and methods related to the CONTEXT ( IEEE-LOM 5.6 )

        /// <summary> Principal environment (or context) within which the learning and use of the learning object is intended to take place ( IEEE-LOM 5.6 ) </summary>
        public ReadOnlyCollection<LOM_VocabularyState> Contexts
        {
            get
            {
                return new ReadOnlyCollection<LOM_VocabularyState>(context);
            }
        }

        /// <summary> Add a new educational context ( IEEE-LOM 5.6 ) </summary>
        /// <param name="Value"> Educational context string </param>
        public void Add_Context(string Value)
        {
            context.Add(new LOM_VocabularyState(Value, String.Empty));
        }

        /// <summary> Add a new educational context ( IEEE-LOM 5.6 )  </summary>
        /// <param name="Value"> Educational context string </param>
        /// <param name="Source"> Source or vocabulary from which the context string is derived </param>
        public void Add_Context(string Value, string Source)
        {
            context.Add(new LOM_VocabularyState(Value, Source));
        }

        /// <summary> Add a new educational context ( IEEE-LOM 5.6 )  </summary>
        /// <param name="Value"> Educational context </param>
        public void Add_Context(LOM_VocabularyState Value )
        {
            context.Add(Value);
        }

        /// <summary> Clears the list of all the educational contexts ( IEEE-LOM 5.6 ) </summary>
        public void Clear_Contexts()
        {
            context.Clear();
        }

        #endregion

        #region Properties and methods related to the TYPICAL AGE RANGE ( IEEE-LOM 5.7 )

        /// <summary> Age (range) of the typical intended user of this learning object ( IEEE-LOM 5.7 ) </summary>
        public ReadOnlyCollection<LOM_LanguageString> TypicalAgeRanges
        {
            get
            {
                return new ReadOnlyCollection<LOM_LanguageString>(typicalAgeRange);
            }
        }

        /// <summary> Add a new typical age range ( IEEE-LOM 5.7 ) </summary>
        /// <param name="Value"></param>
        public void Add_TypicalAgeRange(string Value)
        {
            typicalAgeRange.Add(new LOM_LanguageString(Value, String.Empty));
        }

        /// <summary> Add a new typical age range ( IEEE-LOM 5.7 )  </summary>
        /// <param name="Value"></param>
        /// <param name="Language"></param>
        public void Add_TypicalAgeRange(string Value, string Language)
        {
            typicalAgeRange.Add(new LOM_LanguageString(Value, Language));
        }

        /// <summary> Add a new typical age range ( IEEE-LOM 5.7 )  </summary>
        /// <param name="Value"></param>
        public void Add_TypicalAgeRange(LOM_LanguageString Value)
        {
            typicalAgeRange.Add( Value );
        }

        /// <summary> Clears the list of all the typical age ranges ( IEEE-LOM 5.7 ) </summary>
        public void Clear_TypicalAgeRanges()
        {
            typicalAgeRange.Clear();
        }

        #endregion

        #region Properties and methods related to the CLASSIFICATIONS ( IEEE-LOM 9 )

        /// <summary> Category describes where this learning object falls within a particular classification schema ( IEEE-LOM 9 ) </summary>
        public ReadOnlyCollection<LOM_Classification> Classifications
        {
            get
            {
                return new ReadOnlyCollection<LOM_Classification>(classifications);
            }
        }

        /// <summary> Add a new classification ( IEEE-LOM 9 ) </summary>
        /// <param name="Value"> New classification to add to this learning object </param>
        public void Add_Classification(LOM_Classification Value)
        {
            classifications.Add(Value);
        }

        /// <summary> Clears the list of all the classifications ( IEEE-LOM 9 ) </summary>
        public void Clear_Classifications()
        {
            classifications.Clear();
        }

        #endregion

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'IEEE-LOM'</value>
        public string Module_Name
        {
            get { return "IEEE-LOM"; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get { return null; }
        }

        /// <summary> Chance for this metadata module to perform any additional database work
        /// such as saving digital resource data into custom tables </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString"> Connection string for the current database </param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Save_Additional_Info_To_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        /// <summary> Chance for this metadata module to load any additional data from the 
        /// database when building this digital resource  in memory </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString">Connection string for the current database</param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Retrieve_Additional_Info_From_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        #endregion
    }
}
 