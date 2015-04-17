using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the notes from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Notes_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {

            // Add the notes
            if (Original.Bib_Info.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in Original.Bib_Info.Notes)
                {
                    if (thisNote.Note_Type != Note_Type_Enum.NONE)
                    {
                        if (thisNote.Note_Type != Note_Type_Enum.internal_comments)
                        {
                            New.Add_Description("Note", thisNote.Note).SubTerm = thisNote.Note_Type_Display_String;
                        }
                    }
                    else
                    {
                        New.Add_Description("Note", thisNote.Note);
                    }
                }
            }

            return true;
        }
    }
}
