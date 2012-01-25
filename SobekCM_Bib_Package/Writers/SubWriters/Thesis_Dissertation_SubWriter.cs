using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes thesis dissertation information in the PALMM ETD schema for a given digital resource </summary>
    public class Thesis_Dissertation_SubWriter
    {
        /// <summary>thesis dissertation information in the PALMM ETD-compliant XML to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        public static void Add_Thesis_Dissertation_Metadata(System.IO.TextWriter Output, SobekCM_Item thisBib)
        {
            Output.Write( thisBib.ETD.To_Metadata_Section );
        }
    }
}
