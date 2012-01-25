using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes custom SobekCM Map information for a given digital resource </summary>
    public class SobekCM_Map_SubWriter
    {
        /// <summary> Add the custom map information in the SobekCM standard to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        public static void Add_SobekCM_Metadata(System.IO.TextWriter Output, SobekCM_Item thisBib )
        {
            // Now, collect the data to include here
            string map_data = thisBib.Map.ToXML( "map:", false);
            Output.Write(map_data);
        }
    }
}
