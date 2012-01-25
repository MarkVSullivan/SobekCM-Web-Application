using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes custom SobekCM bibliographic and processing parameters for a given digital resource </summary>
    public class SobekCM_Metadata_SubWriter
    {
        /// <summary> Add the bibliographic and processing parameters in the SobekCM standard to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        /// <param name="embedded_metadata_types"> List of metadata types to embed for this item ( processing parameters and/or bibliographic description )</param>
        public static void Add_SobekCM_Metadata(System.IO.TextWriter Output, SobekCM_Item thisBib, List<Metadata_Type_Enum> embedded_metadata_types )
        {
            string sobekcm_namespace = "sobekcm";

            // Add the processing parameters first
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_ProcParam))
            {
                thisBib.SobekCM_Web.Add_METS_Processing_Metadata(sobekcm_namespace, Output);
            }

            // Check to see if bibliographic parts should be added
            if (embedded_metadata_types.Contains(Metadata_Type_Enum.SobekCM_BibDesc))
            {
                // Add the bibliographic description portion
                thisBib.Bib_Info.Add_SobekCM_BibDesc( Output);

                // Add the serial information, if there is some
                if ((thisBib.hasSerialInformation) && (thisBib.Serial_Info.Count > 0) && (thisBib.METS.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL))
                {
                    thisBib.Serial_Info.Add_METS(sobekcm_namespace, Output);
                }

                // Add the oral history section, if there is any data+
                if (thisBib.hasOralHistoryInformation)
                {
                    thisBib.Oral_Info.Add_METS( Output);
                }

                // Add the performing arts section, if there is any data
                if (thisBib.hasPerformingArtsInformation)
                {
                    thisBib.Performing_Arts_Info.Add_METS_Performing_Arts_Metadata( Output );
                }
            }
        }
    }
}
