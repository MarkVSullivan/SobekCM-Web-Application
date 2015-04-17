using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Maps all the zoological taxonomic specific metadata from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Zoological_Taxonomy_BriefItemMapper: IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Try to get the zoological taxonomy data
            Zoological_Taxonomy_Info taxonInfo = Original.Get_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY) as Zoological_Taxonomy_Info;
            
            // Add the taxonomic data if it exists
            if ((taxonInfo != null) && (taxonInfo.hasData))
            {
                New.Add_Description("Scientific Name", taxonInfo.Scientific_Name);
                New.Add_Description("Kingdom", taxonInfo.Kingdom);
                New.Add_Description("Phylum", taxonInfo.Phylum);
                New.Add_Description("Class", taxonInfo.Class);
                New.Add_Description("Order", taxonInfo.Order);
                New.Add_Description("Family", taxonInfo.Family);
                New.Add_Description("Genus", taxonInfo.Genus);
                New.Add_Description("Species", taxonInfo.Specific_Epithet);
                New.Add_Description("Taxonomic Rank", taxonInfo.Taxonomic_Rank);
                New.Add_Description("Common Name", taxonInfo.Common_Name);
            }

            return true;
        }
    }
}
