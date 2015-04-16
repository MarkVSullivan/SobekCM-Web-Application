using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Maps all the zoological taxonomic specific metadata from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Zoological_Taxonomy_Info_BriefInfoMapper: IBriefItemMapper
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
                New.Add_Citation("Scientific Name", taxonInfo.Scientific_Name);
                New.Add_Citation("Kingdom", taxonInfo.Kingdom);
                New.Add_Citation("Phylum", taxonInfo.Phylum);
                New.Add_Citation("Class", taxonInfo.Class);
                New.Add_Citation("Order", taxonInfo.Order);
                New.Add_Citation("Family", taxonInfo.Family);
                New.Add_Citation("Genus", taxonInfo.Genus);
                New.Add_Citation("Species", taxonInfo.Specific_Epithet);
                New.Add_Citation("Taxonomic Rank", taxonInfo.Taxonomic_Rank);
                New.Add_Citation("Common Name", taxonInfo.Common_Name);
            }

            return true;
        }
    }
}
