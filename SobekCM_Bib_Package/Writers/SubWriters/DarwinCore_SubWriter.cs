using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes zoological taxonomy information as Darwin Core-compiant XML for a given digital resource </summary>
    public class DarwinCore_SubWriter
    {
        /// <summary> Add the zoological taxonomy information as Darwin Core-compiant XML to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        public static void Add_DarwinCore(System.IO.TextWriter Output, SobekCM_Item thisBib)
        {
            Output.Write(thisBib.Zoological_Taxonomy.To_Simple_Darwin_Core_RecordSet );
        }
    }
}
