using System;
using SobekCM.Resource_Object;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the links ( PURL, external link, and finding guide name/link from the METS-based 
    /// SobekCM_Item object to the BriefItem, used for most the public functions of the front-end </summary>
    public class Links_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // If there is an External link, PURL EAD link, or EAD Name, add them
            if (Original.Bib_Info.hasLocationInformation)
            {
                New.Add_Description("External Link", Original.Bib_Info.Location.Other_URL);
                New.Add_Description("Permanent Link", Original.Bib_Info.Location.PURL);

                // Add the EAD, with the URL if it exists
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Location.EAD_URL))
                    New.Add_Description("Finding Guide Name", Original.Bib_Info.Location.EAD_Name).Add_URI(Original.Bib_Info.Location.EAD_URL);
                else
                    New.Add_Description("Finding Guide Name", Original.Bib_Info.Location.EAD_Name);
            }

            return true;
        }
    }
}
