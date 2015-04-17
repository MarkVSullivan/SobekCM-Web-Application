using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the titles from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Temporal_Coverage_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the subjects and coordinate information if that exists
            if (Original.Bib_Info.TemporalSubjects_Count > 0)
            {
                foreach (Temporal_Info thisTemporal in Original.Bib_Info.TemporalSubjects)
                {
                    if (thisTemporal.TimePeriod.Length > 0)
                    {
                        if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year > 0))
                        {
                            New.Add_Description("Temporal Coverage", thisTemporal.TimePeriod + " ( " + thisTemporal.Start_Year + " - " + thisTemporal.End_Year + " )");
                        }
                        if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year <= 0))
                        {
                            New.Add_Description("Temporal Coverage", thisTemporal.TimePeriod + " ( " + thisTemporal.Start_Year + " - )");
                        }
                        if ((thisTemporal.Start_Year <= 0) && (thisTemporal.End_Year > 0))
                        {
                            New.Add_Description("Temporal Coverage", thisTemporal.TimePeriod + " (  - " + thisTemporal.End_Year + " )");
                        }
                    }
                    else
                    {
                        if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year > 0))
                        {
                            New.Add_Description("Temporal Coverage", thisTemporal.Start_Year + " - " + thisTemporal.End_Year);
                        }
                        if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year <= 0))
                        {
                            New.Add_Description("Temporal Coverage", thisTemporal.Start_Year + " - ");
                        }
                        if ((thisTemporal.Start_Year <= 0) && (thisTemporal.End_Year > 0))
                        {
                            New.Add_Description("Temporal Coverage", " - " + thisTemporal.End_Year);
                        }
                    }
                }
            }

            return true;
        }

    }
}
