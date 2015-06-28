#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the frequencies from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Frequency_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add all the origination information, primarily dates )
            if (Original.Bib_Info.Origin_Info != null)
            {
                // Add the frequency
                if (Original.Bib_Info.Origin_Info.Frequencies_Count > 0)
                {
                    Dictionary<string, string> frequencies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (Origin_Info_Frequency thisFrequency in Original.Bib_Info.Origin_Info.Frequencies)
                    {
                        if (!frequencies.ContainsKey(thisFrequency.Term))
                        {
                            frequencies.Add(thisFrequency.Term, thisFrequency.Term);
                            New.Add_Description("Frequency", thisFrequency.Term);
                        }
                    }
                }
            }

            return true;
        }
    }
}
