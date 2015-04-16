using System;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Maps all the Thesis and Dissertation specific metadata from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Thesis_Dissertation_Info_BriefInfoMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Try to get the thesis/dissertation metadata
            Thesis_Dissertation_Info thesisInfo = Original.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;

            // Add the thesis/dissertation data if it exists
            if ((thesisInfo != null) && (thesisInfo.hasData))
            {
                // Add the degree information
                if ( !String.IsNullOrEmpty(thesisInfo.Degree))
                {
                    if (thesisInfo.Degree_Level != Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Unknown)
                    {
                        switch (thesisInfo.Degree_Level)
                        {
                            case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Bachelors:
                                New.Add_Description("Degree", "Bachelor's ( " + thesisInfo.Degree + ")");
                                break;

                            case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters:
                                New.Add_Description("Degree", "Master's ( " + thesisInfo.Degree + ")");
                                break;

                            case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate:
                                New.Add_Description("Degree", "Doctorate ( " + thesisInfo.Degree + ")");
                                break;

                            case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.PostDoctorate:
                                New.Add_Description("Degree", "Post-Doctorate ( " + thesisInfo.Degree + ")");
                                break;
                        }
                    }
                    else
                    {
                        New.Add_Description("Degree", thesisInfo.Degree);
                    }
                }
                else if (thesisInfo.Degree_Level != Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Unknown)
                {
                    switch (thesisInfo.Degree_Level)
                    {
                        case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Bachelors:
                            New.Add_Description("Degree", "Bachelor's");
                            break;

                        case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters:
                            New.Add_Description("Degree", "Master's");
                            break;

                        case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate:
                            New.Add_Description("Degree", "Doctorate");
                            break;

                        case Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.PostDoctorate:
                            New.Add_Description("Degree", "Post-Doctorate");
                            break;
                    }
                }

                // Add the degree grantor
                if ( !String.IsNullOrEmpty(thesisInfo.Degree_Grantor))
                    New.Add_Description("Degree Grantor", thesisInfo.Degree_Grantor);

                // Add the degree divisions
                if (thesisInfo.Degree_Divisions_Count > 0)
                {
                    foreach (string thisDivision in thesisInfo.Degree_Divisions)
                    {
                        New.Add_Description("Degree Divisions", thisDivision);
                    }
                }

                // Add the degree disciplines
                if (thesisInfo.Degree_Disciplines_Count > 0)
                {
                    foreach (string thisDiscipline in thesisInfo.Degree_Disciplines)
                    {
                        New.Add_Description("Degree Disciplines", thisDiscipline);
                    }
                }

                // Add the committee chair
                if ( !String.IsNullOrEmpty(thesisInfo.Committee_Chair))
                    New.Add_Description("Committee Chair", thesisInfo.Committee_Chair);

                // Add the committee co-chair
                if (!String.IsNullOrEmpty(thesisInfo.Committee_Co_Chair))
                    New.Add_Description("Committee Co-Chair", thesisInfo.Committee_Co_Chair);

                // Add all the committee members
                if (thesisInfo.Committee_Members_Count > 0)
                {
                    foreach (string thisMember in thesisInfo.Committee_Members)
                    {
                        New.Add_Description("Committee Members", thisMember);
                    }
                }
            }

            return true;
        }

    }
}
