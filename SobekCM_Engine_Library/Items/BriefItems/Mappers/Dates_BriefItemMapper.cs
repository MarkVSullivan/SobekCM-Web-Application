using System;
using SobekCM.Resource_Object;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the dates from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Dates_BriefItemMapper : IBriefItemMapper
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
                // Add the creation date
                if ((!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.Date_Created)) && (Original.Bib_Info.Origin_Info.Date_Created.Trim() != "-1"))
                {
                    New.Add_Description("Creation Date", Original.Bib_Info.Origin_Info.Date_Created);
                }

                // Add the publication date, looking under DATE ISSUED, or under MARC DATE ISSUED
                if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.Date_Issued))
                {
                    if (Original.Bib_Info.Origin_Info.Date_Issued.Trim() != "-1")
                    {
                        New.Add_Description("Publication Date", Original.Bib_Info.Origin_Info.Date_Issued);
                    }
                }
                else if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.MARC_DateIssued))
                {
                    New.Add_Description("Publication Date", Original.Bib_Info.Origin_Info.MARC_DateIssued);
                }

                // Add the copyright date
                if ((!String.IsNullOrWhiteSpace(Original.Bib_Info.Origin_Info.Date_Copyrighted)) && (Original.Bib_Info.Origin_Info.Date_Copyrighted.Trim() != "-1"))
                {
                    New.Add_Description("Copyright Date", Original.Bib_Info.Origin_Info.Date_Copyrighted);
                }
            }


            return true;
        }
    }
}
