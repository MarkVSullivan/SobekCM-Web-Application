using System.Collections.Generic;
using SobekCM.Resource_Object.OAI.Writer;


namespace SobekCM.Resource_Object.OAI
{
    /// <summary> Abstract class is implemented by all OAI-PMH metadata writers </summary>
    public abstract class abstract_OAI_PMH_Metadata_Type_Writer : iOAI_PMH_Metadata_Type_Writer
    {
        /// <summary> Returns the OAI-PMH metadata of a particular metadata format for this item </summary>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns> Metadata for a OAI-PMH record of a particular metadata format/type </returns>
        public abstract string Create_OAI_PMH_Metadata(SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message);
    }
}
