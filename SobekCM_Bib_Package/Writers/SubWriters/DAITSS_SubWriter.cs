using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes the DAITSS / Florida Digital Archive information as XML for a given digital resource </summary>
    public class DAITSS_SubWriter
    {
        /// <summary> Add the DAITSS / Florida Digital Archive information to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        public static void Add_DAITSS(System.IO.TextWriter Output, SobekCM_Item thisBib)
        {
            Output.Write(thisBib.DAITSS.METS_Administrative_Metadata);
        }
    }
}
