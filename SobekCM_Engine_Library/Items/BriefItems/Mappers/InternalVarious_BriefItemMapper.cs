#region Using directives

using SobekCM.Core.BriefItem;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the internal fields for the citation from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class InternalVarious_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Add the ticklers
            if (Original.Behaviors.Ticklers_Count > 0)
            {
                New.Add_Description("Ticklers", Original.Behaviors.Ticklers);
            }

            // Add some internal values (usually not displayed for non-internal users)
            New.Add_Description("Format", Original.Bib_Info.SobekCM_Type_String);
            New.Add_Description("Digital Resource Creation Date", Original.METS_Header.Create_Date.ToShortDateString());
            New.Add_Description("Last Modified", Original.METS_Header.Modify_Date.ToShortDateString());
            New.Add_Description("Last Type", Original.METS_Header.RecordStatus);
            New.Add_Description("Last User", Original.METS_Header.Creator_Individual);
            New.Add_Description("System Folder", Original.Web.AssocFilePath.Replace("/", "\\"));

            return true;
        }
    }
}
