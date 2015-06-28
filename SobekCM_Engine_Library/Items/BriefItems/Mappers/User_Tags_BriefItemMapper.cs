#region Using directives

using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the user description tags from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class User_Tags_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the desciption user tags
            if (Original.Behaviors.User_Tags_Count > 0)
            {
                foreach (Descriptive_Tag tag in Original.Behaviors.User_Tags)
                {
                    New.Add_Description("User Description", tag.Description_Tag).Authority = tag.UserName + "|" + tag.Date_Added.ToShortDateString();
                }
            }

            return true;
        }
    }
}
