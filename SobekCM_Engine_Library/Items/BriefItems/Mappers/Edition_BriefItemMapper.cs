#region Using directives

using System;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the editions from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Edition_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add all the origination information, primarily dates )
            if (Original.Bib_Info.Origin_Info != null)
            {

                // Collect the state/edition information
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.Edition))
                {
                    New.Add_Description("Edition", Original.Bib_Info.Origin_Info.Edition);
                }
            }

            return true;
        }
    }
}
