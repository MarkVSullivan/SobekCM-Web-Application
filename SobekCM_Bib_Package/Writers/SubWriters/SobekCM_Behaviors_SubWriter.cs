using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes custom SobekCM behavior section for a given digital resource </summary>
    public class SobekCM_Behaviors_SubWriter
    {
        /// <summary> Add the custom SobekCM behavior section to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        public static void Add_SobekCM_Behaviors(System.IO.TextWriter Output, SobekCM_Item thisBib )
        {
            thisBib.SobekCM_Web.Add_BehaviorSec_METS(Output, thisBib.Divisions.Physical_Tree.Has_Files);
        }
    }
}
