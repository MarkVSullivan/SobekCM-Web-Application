#region Using directives

using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Interface for the brief item mapper, which maps from the complete SobekCM item object
    /// to the BriefItemInfo object used by the front end </summary>
    public interface IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New);
    }
}
