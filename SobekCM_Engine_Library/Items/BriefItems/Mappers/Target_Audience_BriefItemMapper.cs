using System;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the target audiences from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Target_Audience_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the target audiences
            if (Original.Bib_Info.Target_Audiences_Count > 0)
            {
                foreach (TargetAudience_Info thisAudience in Original.Bib_Info.Target_Audiences)
                {
                    if (!String.IsNullOrWhiteSpace(thisAudience.Authority))
                    {
                        New.Add_Description("Target Audience", thisAudience.Audience).Authority = thisAudience.Authority;
                    }
                    else
                    {
                        New.Add_Description("Target Audience", thisAudience.Audience).Authority = thisAudience.Authority;
                    }
                }
            }

            return true;
        }
    }
}
