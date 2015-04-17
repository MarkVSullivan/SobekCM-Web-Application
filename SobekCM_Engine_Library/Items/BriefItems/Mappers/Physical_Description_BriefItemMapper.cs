using System;
using SobekCM.Resource_Object;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps the physical description from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Physical_Description_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the physical description
            if ((Original.Bib_Info.Original_Description != null) && (!String.IsNullOrWhiteSpace(Original.Bib_Info.Original_Description.Extent)))
            {
                New.Add_Description("Physical Description", Original.Bib_Info.Original_Description.Extent);
            }

            return true;
        }
    }
}
