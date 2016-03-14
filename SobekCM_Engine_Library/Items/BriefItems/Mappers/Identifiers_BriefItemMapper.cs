#region Using directives

using System;
using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the identifiers from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Identifiers_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {

            // Add the IDENTIFIERS
            if (Original.Bib_Info.Identifiers_Count > 0)
            {
                foreach (Identifier_Info thisIdentifier in Original.Bib_Info.Identifiers)
                {
                    // Add this identifier
                    if (!String.IsNullOrWhiteSpace(thisIdentifier.Type))
                    {
                        New.Add_Description("Resource Identifier", thisIdentifier.Identifier).Authority = thisIdentifier.Type;

                        // Special code for accession number
                        if (thisIdentifier.Type.IndexOf("ACCESSION", StringComparison.OrdinalIgnoreCase) >= 0)
                            New.Add_Description("Accession Number", thisIdentifier.Identifier).Authority = thisIdentifier.Type;
                    }
                    else
                    {
                        New.Add_Description("Resource Identifier", thisIdentifier.Identifier);
                    }
                }
            }



            return true;
        }
    }
}
