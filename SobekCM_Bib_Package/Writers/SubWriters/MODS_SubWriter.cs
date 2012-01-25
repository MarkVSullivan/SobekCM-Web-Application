using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes MODS ( Metadata Object Description Schema ) for a given digital resource </summary>
    public class MODS_SubWriter
    {
        /// <summary> Add the bibliographic information as MODS to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        public static void Add_MODS( System.IO.TextWriter Output, SobekCM_Item thisBib )
        {
            thisBib.Bib_Info.Add_MODS( Output );
        }
    }
}
