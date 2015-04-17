using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Maps all the containers from the METS-based 
    /// SobekCM_Item object to the BriefItem, used for most the public functions of the front-end </summary>
    public class Containers_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the containers
            if (Original.Bib_Info.Containers_Count > 0)
            {
                foreach (Finding_Guide_Container thisContainer in Original.Bib_Info.Containers)
                {
                    New.Add_Description("Physical Location", thisContainer.Name).SubTerm = thisContainer.Type;
                }
            }

            return true;
        }
    }
}
