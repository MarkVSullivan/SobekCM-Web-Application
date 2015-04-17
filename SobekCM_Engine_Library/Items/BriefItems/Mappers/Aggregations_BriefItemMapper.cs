using SobekCM.Core.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the aggregations from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Aggregations_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the aggregations
            if ((Original.Behaviors.Aggregation_Count > 0) && (Engine_ApplicationCache_Gateway.Codes != null))
            {
                foreach (Aggregation_Info thisAggr in Original.Behaviors.Aggregations)
                {
                    Item_Aggregation_Related_Aggregations aggrObj = Engine_ApplicationCache_Gateway.Codes[thisAggr.Code];
                    if (aggrObj.Active)
                    {
                        New.Add_Description("Aggregations", aggrObj.ShortName).Add_URI("[%BASEURL%]" + aggrObj.Code + "[%URLOPTS%]");
                    }
                    else
                    {
                        New.Add_Description("Aggregations", aggrObj.ShortName);
                    }
                }
            }

            return true;
        }
    }
}
