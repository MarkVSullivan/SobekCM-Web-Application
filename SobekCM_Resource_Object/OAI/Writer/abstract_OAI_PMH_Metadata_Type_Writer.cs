using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SobekCM.Resource_Object.OAI.Writer
{
    public abstract class abstract_OAI_PMH_Metadata_Type_Writer : iOAI_PMH_Metadata_Type_Writer
    {
        /// <summary> Gets the formatted metadata OAI-PMH record from the provided item, as a string </summary>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>String, or NULL if there is some sort of error </returns>
        public string Get_Metadata(SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            Error_Message = String.Empty;

            StringBuilder builder = new StringBuilder();


            return builder.ToString();
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public abstract bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message);
    }
}
