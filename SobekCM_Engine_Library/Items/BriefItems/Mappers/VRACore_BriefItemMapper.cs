using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;
using SobekCM.Core.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Maps all the VRA Core (visual resources) specific metadata from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class VRACore_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Try to get the VRA Core metadata
            VRACore_Info vraInfo = Original.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;

            // Add the learning object metadata if it exists
            if ((vraInfo != null) && (vraInfo.hasData))
            {
                // Collect the state/edition information
                if (vraInfo.State_Edition_Count > 0)
                {
                    New.Add_Description("State / Edition", vraInfo.State_Editions);
                }

                // Collect and display all the material information
                if (vraInfo.Material_Count > 0)
                {
                    foreach (VRACore_Materials_Info materials in vraInfo.Materials)
                    {
                        New.Add_Description("Materials", materials.Materials);
                    }
                }

                // Collect and display all the measurements information
                if (vraInfo.Measurement_Count > 0)
                {
                    foreach (VRACore_Measurement_Info measurement in vraInfo.Measurements)
                    {
                        New.Add_Description("Measurements", measurement.Measurements);
                    }
                }

                // Display all cultural context information
                if (vraInfo.Cultural_Context_Count > 0)
                {
                    New.Add_Description("Cultural Context", vraInfo.Cultural_Contexts );
                }

                // Display all style/period information
                if (vraInfo.Style_Period_Count > 0)
                {
                    New.Add_Description("Style/Period", vraInfo.Style_Periods);
                }

                // Display all technique information
                if (vraInfo.Technique_Count > 0)
                {
                    New.Add_Description("Technique", vraInfo.Techniques);
                }

                // Add the inscriptions 
                if (vraInfo.Inscription_Count > 0)
                {
                    New.Add_Description("Inscription", vraInfo.Inscriptions);
                }
            }

            return true;
        }
    }
}
