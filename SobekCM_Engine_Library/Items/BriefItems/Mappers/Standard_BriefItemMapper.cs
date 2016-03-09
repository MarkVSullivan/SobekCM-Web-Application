using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Does some of the initial mapping, copying over some of the very base data elements
    /// from the METS-based SobekCM_Item object to the BriefItem, used for most the public functions of the front-end </summary>
    public class Standard_BriefItemMapper : IBriefItemMapper
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
