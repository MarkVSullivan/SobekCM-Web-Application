using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.EAD
{
    /// <summary> Class contains all the information for rendering a table of contents for EAD Finding Guides
    /// which allow this to be rendered correctly within a SobekCM digital library </summary>
    [Serializable]
    public class EAD_TOC_Included_Section
    {
        /// <summary> Name of the internal link with the EAD Finding Guide which is used to allow a
        /// user to move to a section within the complete EAD Finding Guide HTML </summary>
        public readonly string Internal_Link_Name;

        /// <summary> Title of this EAD Finding Guide section to be displayed in the table of contents </summary>
        public readonly string Section_Title;

        /// <summary> Constructor for a new instance of the EAD_TOC_Included_Section class </summary>
        /// <param name="Internal_Link_Name"> Name of the internal link with the EAD Finding Guide which is used to allow a
        /// user to move to a section within the complete EAD Finding Guide HTML </param>
        /// <param name="Section_Title"> Title of this EAD Finding Guide section to be displayed in the table of contents </param>
        public EAD_TOC_Included_Section(string Internal_Link_Name, string Section_Title)
        {
            this.Internal_Link_Name = Internal_Link_Name;
            this.Section_Title = Section_Title;
        }
    }
}
