using System;
using SobekCM.Core.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Resource_Object;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps the source institution from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Source_Institution_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the SOURCE INSTITUTION information
            if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Source.Statement))
            {
                // Add the source institution
                BriefItem_DescTermValue sourceValue = New.Add_Description("Source Institution", Original.Bib_Info.Source.Statement);

                // Was the code present, and active?
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Source.Code))
                {
                    if ((Engine_ApplicationCache_Gateway.Codes != null) && (Engine_ApplicationCache_Gateway.Codes.isValidCode("i" + Original.Bib_Info.Source.Code)))
                    {
                        Item_Aggregation_Related_Aggregations sourceAggr = Engine_ApplicationCache_Gateway.Codes["i" + Original.Bib_Info.Source.Code];
                        if (sourceAggr.Active)
                        {
                            sourceValue.Add_URI("[%BASEURL%]" + "i" + Original.Bib_Info.Source.Code + "[%URLOPTS%]");
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
