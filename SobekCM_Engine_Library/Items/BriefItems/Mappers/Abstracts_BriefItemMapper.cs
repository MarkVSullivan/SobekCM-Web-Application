#region Using directives

using System;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the abstracts from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Abstracts_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the abstracts
            if (Original.Bib_Info.Abstracts_Count > 0)
            {
                foreach (Abstract_Info thisAbstract in Original.Bib_Info.Abstracts)
                {
                    if (!String.IsNullOrWhiteSpace(thisAbstract.Display_Label))
                    {
                        New.Add_Description("Abstract", thisAbstract.Abstract_Text).SubTerm = thisAbstract.Display_Label;
                    }
                    else
                    {
                        New.Add_Description("Abstract", thisAbstract.Abstract_Text);
                    }
                }
            }

            return true;
        }
    }
}
