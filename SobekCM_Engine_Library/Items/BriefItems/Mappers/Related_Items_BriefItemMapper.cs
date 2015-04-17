using System;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the related items from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Related_Items_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the RELATED ITEMS
            if (Original.Bib_Info.RelatedItems_Count > 0)
            {
                foreach (Related_Item_Info relatedItem in Original.Bib_Info.RelatedItems)
                {
                    // Determine the field to display
                    string relatedDisplay = String.Empty;
                    if ((!String.IsNullOrWhiteSpace(relatedItem.URL)) && (!String.IsNullOrWhiteSpace(relatedItem.URL_Display_Label)))
                        relatedDisplay = relatedItem.URL_Display_Label;
                    else if (!String.IsNullOrWhiteSpace(relatedItem.Main_Title.Title))
                        relatedDisplay = relatedItem.Main_Title.Title;
                    else if (!String.IsNullOrWhiteSpace(relatedItem.URL))
                        relatedDisplay = relatedItem.URL;

                    // If nothing to display, move to next one
                    if (relatedDisplay.Length == 0)
                        continue;

                    // Add this related item
                    BriefItem_DescTermValue relatedObj = New.Add_Description("Related Item", relatedDisplay);

                    // Add the relationship
                    switch (relatedItem.Relationship)
                    {
                        case Related_Item_Type_Enum.host:
                            relatedObj.SubTerm = "Host material";
                            break;

                        case Related_Item_Type_Enum.otherFormat:
                            relatedObj.SubTerm = "Other format";
                            break;

                        case Related_Item_Type_Enum.otherVersion:
                            relatedObj.SubTerm = "Other version";
                            break;

                        case Related_Item_Type_Enum.preceding:
                            relatedObj.SubTerm = "Preceded by";
                            break;

                        case Related_Item_Type_Enum.succeeding:
                            relatedObj.SubTerm = "Succeeded by";
                            break;
                    }

                    // Add the URI if one was indicated
                    if (!String.IsNullOrWhiteSpace(relatedItem.URL))
                    {
                        relatedObj.Add_URI(relatedItem.URL);
                    }
                    else if (!String.IsNullOrWhiteSpace(relatedItem.SobekCM_ID))
                    {
                        relatedObj.Add_URI("[%BASEURL%]" + relatedItem.SobekCM_ID.Replace("_", "/") + "[%URLOPTS%]");
                    }
                }
            }

            return true;
        }
    }
}
