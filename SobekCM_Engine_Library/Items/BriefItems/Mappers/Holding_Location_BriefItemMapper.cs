using System;
using SobekCM.Core.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Resource_Object;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps the holding location from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Holding_Location_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the HOLDING LOCATION information
            if ((Original.Bib_Info.hasLocationInformation) && (!String.IsNullOrWhiteSpace(Original.Bib_Info.Location.Holding_Name)))
            {
                // Add the source institution
                BriefItem_DescTermValue sourceValue = New.Add_Description("Holding Location", Original.Bib_Info.Location.Holding_Name);

                // Was the code present, and active?
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Location.Holding_Code))
                {
                    if ((Engine_ApplicationCache_Gateway.Codes != null) && (Engine_ApplicationCache_Gateway.Codes.isValidCode("i" + Original.Bib_Info.Location.Holding_Code)))
                    {
                        Item_Aggregation_Related_Aggregations sourceAggr = Engine_ApplicationCache_Gateway.Codes["i" + Original.Bib_Info.Location.Holding_Code];
                        if (sourceAggr.Active)
                        {
                            sourceValue.Add_URI("[%BASEURL%]" + "i" + Original.Bib_Info.Location.Holding_Code + "[%URLOPTS%]");
                        }

                        // Was there an external link on this agggreation?
                        if (!String.IsNullOrWhiteSpace(sourceAggr.External_Link))
                        {
                            sourceValue.Add_URI(sourceAggr.External_Link);
                        }
                    }
                }
            }

            return true;
        }
    }
}
