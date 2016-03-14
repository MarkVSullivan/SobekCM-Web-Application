using System.Linq;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the behavior information from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Behaviors_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Copy aggregation codes over
            New.Behaviors.Aggregation_Code_List = Original.Behaviors.Aggregation_Code_List.ToList();
            New.Behaviors.Holding_Location_Aggregation = Original.Bib_Info.HoldingCode;
            if (Original.Bib_Info.Source != null)
                New.Behaviors.Source_Institution_Aggregation = Original.Bib_Info.Source.Code;


            // Copy the behavior information
            New.Behaviors.Dark_Flag = Original.Behaviors.Dark_Flag;
            New.Behaviors.Embedded_Video = Original.Behaviors.Embedded_Video;
            New.Behaviors.GroupTitle = Original.Behaviors.GroupTitle;
            New.Behaviors.GroupType = Original.Behaviors.GroupType;
            New.Behaviors.IP_Restriction_Membership = Original.Behaviors.IP_Restriction_Membership;
            New.Behaviors.Single_Use = Original.Behaviors.CheckOut_Required;

            // Copy over the viewers
            New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer("CITATION", 1, false));
            New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer("PDF", 1, false));
            New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer("JPEG", 1, false));
            New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer("JPEG2000", 1, false));
            New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer("RELATED_IMAGES", 1, false));
            New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer("MANAGE_MENU", 1, false));
            New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer("MARC", 1, false));
            New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer("METADATA", 1, false));

            // Copy over the wordmarks
            if (Original.Behaviors.Wordmark_Count > 0)
            {
                foreach (Wordmark_Info origWordmark in Original.Behaviors.Wordmarks)
                {
                    New.Behaviors.Add_Wordmark(origWordmark.Code, origWordmark.Title, origWordmark.HTML, origWordmark.Link );
                }
            }


            return true;
        }

    }
}
