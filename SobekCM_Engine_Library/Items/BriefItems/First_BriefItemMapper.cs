using SobekCM.Resource_Object;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    public class First_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            return true;
        }
    }
}
