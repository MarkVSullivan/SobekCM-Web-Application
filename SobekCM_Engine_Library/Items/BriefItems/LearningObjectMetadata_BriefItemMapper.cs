using System;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.LearningObjects;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Maps all the learning object specific metadata from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class LearningObjectMetadata_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Try to get the learning object metadata
            LearningObjectMetadata lomInfo = Original.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;

            // Add the learning object metadata if it exists
            if ((lomInfo != null) && (lomInfo.hasData))
            {
                // Add the LOM Aggregation level
                if (lomInfo.AggregationLevel != AggregationLevelEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.AggregationLevel)
                    {
                        case AggregationLevelEnum.level1:
                            lom_temp = "Level 1 - a single, atomic object";
                            break;

                        case AggregationLevelEnum.level2:
                            lom_temp = "Level 2 - a lesson plan";
                            break;

                        case AggregationLevelEnum.level3:
                            lom_temp = "Level 3 - a course, or set of lesson plans";
                            break;

                        case AggregationLevelEnum.level4:
                            lom_temp = "Level 4 - a set of courses";
                            break;
                    }

                    New.Add_Citation("Aggregation Level", lom_temp);
                }


                // Add the LOM Learning resource type
                if (lomInfo.LearningResourceTypes.Count > 0)
                {
                    foreach (LOM_VocabularyState thisState in lomInfo.LearningResourceTypes)
                    {
                        New.Add_Citation("Learning Resource Type", thisState.Value );
                    }
                }

                // Add the LOM Status
                if (lomInfo.Status != StatusEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.Status)
                    {
                        case StatusEnum.draft:
                            lom_temp = "Draft";
                            break;

                        case StatusEnum.final:
                            lom_temp = "Final";
                            break;

                        case StatusEnum.revised:
                            lom_temp = "Revised";
                            break;

                        case StatusEnum.unavailable:
                            lom_temp = "Unavailable";
                            break;
                    }

                    New.Add_Citation("Status", lom_temp);
                }

                // Add the LOM Interactivity Type
                if (lomInfo.InteractivityType != InteractivityTypeEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.InteractivityType)
                    {
                        case InteractivityTypeEnum.active:
                            lom_temp = "Active";
                            break;

                        case InteractivityTypeEnum.expositive:
                            lom_temp = "Expositive";
                            break;

                        case InteractivityTypeEnum.mixed:
                            lom_temp = "Mixed";
                            break;
                    }

                    New.Add_Citation("Interactivity Type", lom_temp );
                }

                // Add the LOM Interactivity Level
                if (lomInfo.InteractivityLevel != InteractivityLevelEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.InteractivityLevel)
                    {
                        case InteractivityLevelEnum.very_low:
                            lom_temp = "Very low";
                            break;

                        case InteractivityLevelEnum.low:
                            lom_temp = "Low";
                            break;

                        case InteractivityLevelEnum.medium:
                            lom_temp = "Mediuim";
                            break;

                        case InteractivityLevelEnum.high:
                            lom_temp = "High";
                            break;

                        case InteractivityLevelEnum.very_high:
                            lom_temp = "Very high";
                            break;
                    }

                    New.Add_Citation("Interactivity Level", lom_temp);
                }

                // Add the LOM Difficulty Level
                if (lomInfo.DifficultyLevel != DifficultyLevelEnum.UNDEFINED)
                {
                    string lom_temp = String.Empty;
                    switch (lomInfo.DifficultyLevel)
                    {
                        case DifficultyLevelEnum.very_easy:
                            lom_temp = "Very easy";
                            break;

                        case DifficultyLevelEnum.easy:
                            lom_temp = "Easy";
                            break;

                        case DifficultyLevelEnum.medium:
                            lom_temp = "Mediuim";
                            break;

                        case DifficultyLevelEnum.difficult:
                            lom_temp = "Difficult";
                            break;

                        case DifficultyLevelEnum.very_difficult:
                            lom_temp = "Very difficult";
                            break;
                    }

                    New.Add_Citation("Difficulty Level", lom_temp);
                }


                // Add the LOM Intended End User Role
                if (lomInfo.IntendedEndUserRoles.Count > 0)
                {
                    foreach (IntendedEndUserRoleEnum thisUser in lomInfo.IntendedEndUserRoles)
                    {
                        switch (thisUser)
                        {
                            case IntendedEndUserRoleEnum.teacher:
                                New.Add_Citation("Intended User Roles", "Teacher");
                                break;

                            case IntendedEndUserRoleEnum.learner:
                                New.Add_Citation("Intended User Roles", "Learner");
                                break;

                            case IntendedEndUserRoleEnum.author:
                                New.Add_Citation("Intended User Roles", "Author");
                                break;

                            case IntendedEndUserRoleEnum.manager:
                                New.Add_Citation("Intended User Roles", "Manager");
                                break;

                        }
                    }
                }

                // Add the LOM Context 
                if (lomInfo.Contexts.Count > 0)
                {
                    foreach (LOM_VocabularyState thisContext in lomInfo.Contexts)
                    {
                        if (thisContext.Source.Length > 0)
                            New.Add_Citation("Context",thisContext.Source + " " + thisContext.Value);
                        else
                            New.Add_Citation("Context",thisContext.Value);
                    }
                }


                // Add the LOM Typical Age Range
                if (lomInfo.TypicalAgeRanges.Count > 0)
                {
                    foreach (LOM_LanguageString languageString in lomInfo.TypicalAgeRanges)
                    {
                        New.Add_Citation("Typical Age Range", languageString.Value);
                    }
                }

                // Add the typical learning time
                New.Add_Citation("Typical Learning Time", lomInfo.TypicalLearningTime);

                // Add the system requirements
                if (lomInfo.SystemRequirements.Count > 0)
                {
                    foreach (LOM_System_Requirements thisContext in lomInfo.SystemRequirements)
                    {
                        string start = String.Empty;
                        switch (thisContext.RequirementType)
                        {
                            case RequirementTypeEnum.operating_system:
                                start = "Operating System: " + thisContext.Name;
                                break;

                            case RequirementTypeEnum.browser:
                                start = "Browser: " + thisContext.Name;
                                break;

                            case RequirementTypeEnum.hardware:
                                start = "Hardware: " + thisContext.Name;
                                break;

                            case RequirementTypeEnum.software:
                                start = "Software: " + thisContext.Name;
                                break;
                        }

                        // Add with version, if included
                        if (thisContext.MinimumVersion.Length == 0)
                        {
                            if (thisContext.MaximumVersion.Length > 0)
                            {
                                New.Add_Citation("System Requirements", start + " ( - " + thisContext.MaximumVersion + " )");
                            }
                            else
                            {
                                New.Add_Citation("System Requirements", start);
                            }
                        }
                        else
                        {
                            if (thisContext.MaximumVersion.Length > 0)
                            {
                                New.Add_Citation("System Requirements", start + " ( " + thisContext.MinimumVersion + " - " + thisContext.MaximumVersion + " )");
                            }
                            else
                            {
                                New.Add_Citation("System Requirements", start + " ( " + thisContext.MinimumVersion + " - )");
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
