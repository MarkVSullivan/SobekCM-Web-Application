using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.SobekCM_Info
{
    /// <summary> Class represents titles which are related to the current title in the
    /// SobekCM database </summary>
    public class Related_Titles
    {
        /// <summary> String represents this relationship between the main title and the related title </summary>
        public readonly string Relationship;

        /// <summary> Title and the link for the title within this SobekCM library </summary>
        public readonly string Title_And_Link;

        /// <summary> Constructor for a new instance of the Related_Titles class </summary>
        /// <param name="Relationship"> String represents this relationship between the main title and the related title</param>
        /// <param name="Title_And_Link"> Title and the link for the title within this SobekCM library</param>
        public Related_Titles(string Relationship, string Title_And_Link)
        {
            this.Relationship = Relationship;
            this.Title_And_Link = Title_And_Link;
        }
    }
}
