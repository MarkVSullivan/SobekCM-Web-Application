using System;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the classifications from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Classifications_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the CLASSIFICATIONS
            if (Original.Bib_Info.Classifications_Count > 0)
            {
                foreach (Classification_Info thisClassification in Original.Bib_Info.Classifications)
                {
                    if (!String.IsNullOrWhiteSpace(thisClassification.Authority))
                    {
                        New.Add_Description("Classification", thisClassification.Classification).Authority = thisClassification.Authority;
                    }
                    else
                    {
                        New.Add_Description("Classification", thisClassification.Classification);
                    }
                }
            }

            return true;
        }
    }
}
