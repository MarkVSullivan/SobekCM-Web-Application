using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package
{
    /// <summary> Class holds the information for a secion from the METS file ( specifically, 
    /// a dmdSec or amdSec ) which was not recognized and remains unanalyzed.  This section 
    /// is retained in case the METS file is written again, to preserve the unanalyzable 
    /// information </summary>
    public class Unanalyzed_METS_Section
    {
        /// <summary> List of attributes in the top-level definition of this section </summary>
        public ReadOnlyCollection<KeyValuePair<string, string>> Section_Attributes;

        /// <summary> Complete XML include in this unanalyzed METS section </summary>
        public readonly string Inner_XML;

        /// <summary> ID for the top-level section (also included in the attribute list) </summary>
        public readonly string ID;

        /// <summary> Constructor for a new instance of the Unanalyzed_METS_Section class </summary>
        /// <param name="Section_Attributes"> List of attributes in the top-level definition of this section </param>
        /// <param name="ID"> ID for the top-level section (also included in the attribute list) </param>
        /// <param name="Inner_XML"> Complete XML include in this unanalyzed METS section </param>
        public Unanalyzed_METS_Section(List<KeyValuePair<string, string>> Section_Attributes, string ID, string Inner_XML)
        {
            this.Section_Attributes = new ReadOnlyCollection<KeyValuePair<string, string>>(Section_Attributes);
            this.Inner_XML = Inner_XML;
            this.ID = ID;
        }
    }
}
